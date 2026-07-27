# Cross-platform text/font abstraction — decompilation/implementation log

Append-only. Do not edit previous entries.

---

## 2026-07-27 — Proposed follow-up: full cross-platform text engine

Not implemented — this is a proposal only, written down at the user's request
after discussing what a "full text engine" would take, so it isn't lost
before someone picks it up. Follows on directly from the entry below: it
addresses the two things that entry scoped *out* of the `TextDocument`/
`FontResource` abstraction (`RichText.Measure`'s multi-range formatting, and
`RichText`'s interactive-editing surface).

Recommend splitting this into two independent pieces rather than trying to
grow `TextDocument` to cover everything:

**1. Multi-range formatting (closes the `RichText.Measure` gap).**
`SixLabors.Fonts`/`SixLabors.ImageSharp.Drawing` (already referenced by
`UIX.RenderApi.csproj` as of this work) has `RichTextOptions.TextRuns`
(`IReadOnlyList<RichTextRun>`) for applying different styles to sub-ranges of
one text block — this was found during research for the abstraction above but
not used, since `TextDocument.Measure` only accepts one `TextStyleInfo` per
call. Extending `TextDocument.Measure` (or adding a parallel method) to accept
a list of styled ranges instead of a single style would let
`SixLaborsTextDocument` match `NativeApi.SpRichTextMeasure`/
`Drawing.TextMeasureParams`'s formatted-range capability (used today for
things like differently-colored hyperlink runs inside one `RichText` control)
without inventing new machinery — the SixLabors-side support already exists.

**2. Editing state (cursor, selection, undo/redo, clipboard, IME) is a
different problem from rendering and should not be a `TextDocument` backend.**
Recommend a pure-managed C# text-editor core (buffer + selection + undo
stack) shared across *all* platforms, including Windows, rather than
continuing to wrap the native RichEdit-style control via
`NativeApi.SpRichText*`. This is a larger, standalone effort — a real
text-editor implementation, not a thin backend swap like `TextDocument`/
`FontResource` — but it's the only path to genuine cross-platform parity for
`RichText`, and it would let `RichText` eventually drop its `NativeApi`
dependency entirely (today it's the last text-related class that's still
Windows/native-only unconditionally, per the scope decision in the entry
below). IME composition and clipboard access genuinely need OS hooks and
would stay behind a small per-platform adapter interface (Windows already has
IMM32 access via `Win32Api`; other platforms would start with minimal/stubbed
support, explicitly documented as a gap, rather than blocking the rest of the
engine on full IME parity).

No decision has been made on priority or timing for either piece; logged here
so the reasoning and the `RichTextOptions.TextRuns` finding aren't lost.

---

## 2026-07-27 — TextDocument/FontResource abstraction (UIX.RenderApi + UIX)

**Goal:** isolate `NativeApi.SpLoadFontResource`, `NativeApi.SpRichTextGetSimpleContentLength`,
and related font/text P/Invokes to a Windows-only implementation, with a
SixLabors.Fonts/SixLabors.ImageSharp.Drawing backend for other platforms,
following the `BitmapInformation`/`ImageLoader` precedent in
`UIX.RenderApi/Microsoft/Iris/Render/Bitmaps/`.

### Scope decision: RichText's interactive-editing surface stays native-only

`RichText` (`UIX/Microsoft/Iris/Drawing/RichText.cs`) is not just a text
renderer — it wraps a stateful native rich-edit control that also does IME
composition, keyboard/mouse forwarding, undo/redo, clipboard, scrollbars, and
caret-blink timers (`SetSelectionRange`, `NotifyOfFocusChange`,
`ForwardKeyStateNotification`/`ForwardKeyCharacterNotification`,
`ForwardMouseInput`, `ForwardImeMessage`, `CanUndo`/`Undo`/`Cut`/`Copy`/`Paste`/
`Delete`, `Scroll*`/`SetScrollbars`, `SetTimer`/`KillTimer`, `MaxLength`,
`ReadOnly`, `DetectUrls`, `SetWordWrap`). SixLabors.Fonts has no equivalent —
it's a font-shaping/rendering library, not a text editor. Confirmed with the
user (2026-07-27, via AskUserQuestion) that these stay calling
`NativeApi.SpRichText*` directly, unconditionally, exactly as before. A
cross-platform interactive text-editing engine is a separate, much larger
project for a future stage, not attempted here. (See the follow-up entry
above for a proposed shape of that project.)

### Scope decision: RichText.Measure/Rasterize also stay native-only

`RichText.Measure` uses `NativeApi.SpRichTextMeasure` with
`Drawing.TextMeasureParams`, which supports multiple formatted ranges
(different styles applied to sub-ranges of one text block — see
`TextMeasureParams._formattedRanges`/`_formattedRangeStyles`), used for things
like differently-colored hyperlink runs within one RichText control. The new
cross-platform `TextDocument.Measure` abstraction (see below) only supports a
single style per call — matching what `Drawing.SimpleText` already needed, but
strictly less capable than `TextMeasureParams`. Extending the abstraction to
carry multiple formatted ranges was judged out of proportion to the task, and
silently downgrading `RichText.Measure` to single-style would have regressed
existing Windows rich-text behavior, which CLAUDE.md's stage-2 rule ("must be
entirely backwards-compatible") forbids. So `RichText.Measure` and the static
`RichText.Rasterize` helper keep calling `NativeApi.SpRichTextMeasure`/
`SpRichTextRasterize` directly, unchanged. Only `RichText.Content` (set),
`RichText.SimpleContent` (get — this is what
`NativeApi.SpRichTextGetSimpleContentLength`/`SpRichTextGetSimpleContent` back),
and `RichText.GetNaturalBounds()` were routed through the new
`TextDocument` abstraction, since none of those touch formatted ranges.
(See the follow-up entry above: `SixLabors.Fonts` does have a multi-range
formatting API — `RichTextOptions.TextRuns` — that could close this gap in a
later pass.)

`Drawing.SimpleText` (used for static/label text, not editable) has no such
formatting-range capability to begin with, so it was fully routed through the
abstraction with no fidelity loss — this is also the only consumer that is
now genuinely cross-platform-constructible (its constructor no longer calls
`NativeApi` directly).

### Architecture deviation from the initial plan: Windows backend lives in `UIX`, not `UIX.RenderApi`

The approved plan assumed the Windows backend classes (`SpFontResource`,
`SpTextDocument`) would live alongside the abstract classes in
`UIX.RenderApi/Microsoft/Iris/Render/Text/`, switched via a `#if WINDOWS`
factory exactly like `ImageLoader.CreateBitmapInformation()` does for bitmaps.

This turned out not to be possible: `Microsoft.Iris.OS.NativeApi` (all the
`Sp*` P/Invokes into `UIXRender.dll`) and the unsafe marshalling types it
needs (`Drawing.TextStyle.MarshalledData`, `Drawing.TextMeasureParams`,
`NativeApi.RasterizeRunPacket`, `NativeApi.ReportRunCallback`) live in the
`UIX` project, which *depends on* `UIX.RenderApi` — not the other way around
(confirmed: `UIX.csproj` has `<ProjectReference Include="...UIX.RenderApi..." />`,
and `grep -n "SpBitmap" NativeApi.cs` returns nothing, i.e. bitmaps' native
P/Invokes were declared fresh in `UIX.RenderApi/.../ExtensionsApi.cs` from the
start, with no pre-existing `UIX`-side implementation to reuse — unlike text,
which already has ~900 lines of working, decompiled, Windows-tested
marshalling in `NativeApi.cs`). A class in `UIX.RenderApi` cannot reference
`NativeApi.SpRichTextMeasure` or `TextMeasureParams` at all.

Duplicating that marshalling code a second time inside `UIX.RenderApi` (a new
set of `[DllImport]` declarations plus re-deriving `RasterizeRunPacket`'s
exact struct layout) was rejected as needless duplication of already-correct,
already-tested code, contrary to the project's "don't duplicate" guidance.

**Resolution:** `SpFontResource.cs` and `SpTextDocument.cs` live in
`UIX/Microsoft/Iris/Drawing/`, next to `NativeApi`/`RichText`/`SimpleText`,
reusing the existing marshalling directly. `UIX.RenderApi`'s
`FontResourceLoader`/`TextDocumentFactory` expose a
`RegisterWindowsBackend(Func<T> factory)` seam instead of a `#if WINDOWS`
switch; `UIX/Microsoft/Iris/OS/TextBackendRegistration.cs` calls it from a
`[ModuleInitializer]`-attributed method, itself gated on `#if WINDOWS` (the
`WINDOWS` compile constant, defined in `Directory.Build.props`, is only set
for Windows-targeted TFMs — `UIX.csproj`, like `UIX.RenderApi.csproj`, also
targets plain `net8.0`, so on that TFM the registration body compiles out and
the factories fall back to their default `SixLabors.*` implementations).
Verified this doesn't leave anything unregistered-by-accident: `NativeApi.cs`
itself has zero `#if` guards and already compiles unconditionally on plain
`net8.0` (only failing at P/Invoke-resolution time if actually called on a
non-Windows OS), so gating only the *registration call* (not the backend
class definitions) matches that existing convention.

### `SpTextDocument`'s two construction modes

Mirrors the two native object families `NativeApi` already exposes for text:
`SpSimpleTextBuildObject`/`hSto` (no `SetContent`/`GetSimpleContent`/
`GetNaturalBounds` native equivalent exists for this family — grepped
`NativeApi.cs` for `SpSimpleText`, only `BuildObject`/`DestroyObject`/
`Measure`/`MeasurePossible`/`IsAvailable` exist) vs `SpRichTextBuildObject`/
`hRto` (has `SetContent`/`GetSimpleContent`/`GetNaturalBounds`, but Measure
needs the formatted-range machinery excluded above). So:
- *standalone* mode (owns its own STO) implements `Measure`/`MeasurePossible`
  for real; `SetContent`/`GetSimpleContent`/`GetNaturalBounds` throw
  `NotSupportedException` (never called — `SimpleText` never touches content).
- *bound* mode (constructed with an existing RTO handle, supplied by
  `RichText`) implements `SetContent`/`GetSimpleContent`/`GetNaturalBounds`
  for real; `Measure`/`MeasurePossible` throw `NotSupportedException` (never
  called — `RichText.Measure` stays on `NativeApi.SpRichTextMeasure` directly,
  per the scope decision above).
- `Rasterize` is handle-agnostic in the original code too (`SpRichTextRasterize`
  takes only the glyph run's own `hGlyphRunInfo`, not `hRto`/`hSto`), so it's
  implemented identically regardless of mode.

### `Dib` (`UIX/Microsoft/Iris/RenderAPI/Drawing/Dib.cs`) generalized for non-native disposal

Its `Dispose()` was hardcoded to `NativeApi.SpFreeDib(m_hdib)`, which is a
no-op when `m_hdib == IntPtr.Zero` — true for bitmaps produced by
`SixLaborsTextDocument.Rasterize`, which allocates its pixel buffer with
`Marshal.AllocHGlobal` instead of a native DIB handle. Without a way to free
that buffer, it would leak whenever a `SimpleText`-produced `TextRun` gets
rasterized on a non-Windows backend. Added an optional `Action onDispose`
constructor parameter, invoked in addition to (not instead of) the existing
`SpFreeDib` guard; existing Windows call sites are unaffected (new parameter
defaults to `null` via a preserved original-signature overload).

### Assumption: `TextStyleInfo`'s "set" flags

`Drawing.TextStyle`'s native marshalling (`TextStyle.MarshalledData._flags`)
packs both "was this field explicitly set" bits and, for boolean fields, a
separate "value" bit (see `TextStyle.SetFlags`). `Render.Text.TextStyleInfo`
(the new cross-platform DTO) has no tri-state concept — every field always has
a definite value. `SpTextDocument.ToMarshalledData` treats every
`TextStyleInfo` field as "explicitly set" except `TextColor` (only set if
`TextStyleInfo.HasColor`) and `AltFontHeight` (only set if `AltFontSize != 0`),
mirroring `SimpleText.CanMeasure`/`Measure`'s pre-existing behavior of always
constructing a fresh `TextStyle.MarshalledData` from a fully-populated
`TextStyle` (the original code had the same "always set" characteristic for
non-color/non-alt-size fields — verified against `TextStyle.MarshalledData`'s
constructor, which unconditionally copies `_fontHeightPts`/`_lineSpacing`/
`_characterSpacing` regardless of flags). This is a direct behavior carry-over,
not a new assumption about untested behavior.
