# Cross-platform text/font abstraction — decompilation/implementation log

Append-only. Do not edit previous entries.

---

## 2026-07-27 — Correction: missed `NativeApi.SpSimpleTextIsAvailable()` call in the piece-1 RichText fix

User caught this by inspection after the piece-1 entry below. The
`SharedOversampledRasterizer`/`SharedNonOversampledRasterizer` lazy-init
getters in `ViewItems/Text.cs` - the exact code path made cross-platform in
the piece-1 entry below (constructing the shared non-hosted `RichText`
instances) - each also called `NativeApi.SpSimpleTextIsAvailable()`
immediately afterward, completely unconditionally, to seed the static
`s_simpleTextMeasureAvailable` flag. That call was missed when auditing the
construction path, so it would still have crashed on non-Windows at that
call site, immediately undoing the point of the piece-1 fix the very first
time any `Text` viewitem got measured.

Traced what the flag actually gates: `s_simpleTextMeasureAvailable` controls
whether `Text` ever attempts the "fast measure" path
(`SharedSimpleTextRasterizer`, backed by `Microsoft.Iris.Drawing.SimpleText`)
at all - `Text`'s constructor and `MarkTextLayoutInvalid()` both check it
before touching `Bits.FastMeasurePossible`. On real Windows this is a
capability probe (does this build of `UIXRender.dll` have the native STO
fast-measure family) - Windows-specific by construction, not something to
port. But `SimpleText` itself is *already* fully cross-platform (confirmed by
re-reading `SimpleText.cs`, part of the pre-existing `TextDocument`
abstraction from before either follow-up entry below): it goes through
`TextDocumentFactory.CreateStandalone()`, which falls back to
`SixLaborsTextDocument` with no native dependency at all. So there is no
non-Windows equivalent gap to probe - fast-measure is unconditionally
available there, not merely defaulted-off to avoid a crash.

Fix: extracted a `private static bool IsSimpleTextMeasureAvailable()` helper
in `Text.cs`, `#if WINDOWS` calling `NativeApi.SpSimpleTextIsAvailable()`
exactly as before (zero Windows behavior change), `#else` returning `true`
directly (not `false` - returning `false` would have been the "safe-looking"
but actually-wrong fix: it compiles and doesn't crash, but silently disables
a working, fully cross-platform optimization for no reason). Both call sites
in the two shared-rasterizer getters now call this helper instead of
`NativeApi.SpSimpleTextIsAvailable()` directly. Verified `UIX.csproj` builds
clean on net8.0 after the fix.

Lesson for future audits of this kind: grepping for `RichText`/`TextDocument`
call sites (what the piece-1 entry below did) isn't sufficient to find every
native call reachable from a "now cross-platform" construction path - need to
also check for *other*, unrelated `NativeApi.*`/`Win32Api.*` calls sitting in
the same method bodies, since a lazily-initialized static getter can bundle
multiple concerns (construct the object *and* seed an unrelated capability
flag, here) that don't all show up under a search for the one type being
fixed.

---

## 2026-07-27 — Enacted follow-up, piece 2: TextEditBuffer (text-editing engine foundation)

Second half of the "enact both pieces, in sequence" request. Piece 1 (above,
but logged *after* this one since the log is prepend-only - piece 1 was done
first chronologically) made non-hosted `RichText` cross-platform; this entry
covers the interactive-editing side.

### Scope: built the foundation only, did not integrate it into RichText/TextEditingHandler

The "Proposed follow-up" entry below recommends "a pure-managed C# text-editor
core (buffer + selection + undo stack) shared across all platforms" plus small
per-platform IME/clipboard adapters, explicitly calling this "a larger,
standalone effort... not a thin backend swap." Confirmed that characterization
by tracing what a real integration would require:
`Microsoft.Iris.InputHandlers.TextEditingHandler` (717 lines) drives
`RichText`'s hosted mode through `ForwardKeyStateNotification`/
`ForwardKeyCharacterNotification` (raw virtual-key/scan-code/modifier-state
forwarding), `ForwardImeMessage` (raw Win32 WM_IME_* message forwarding),
timer-driven caret blink (`SetTimer`/`KillTimer`/`IRichTextCallbacks`), and
scrollbar sync (`ITextScrollModelCallback`) - all native-message-shaped
surface with no existing cross-platform input abstraction to sit on top of.
Rewiring that safely, without regressing the native Windows editing behavior
CLAUDE.md's stage-2 rule requires staying byte-compatible with, needs its own
dedicated design pass (mapping whatever cross-platform input events this
codebase eventually gets onto `TextEditBuffer` operations) - attempting it
inline here, on top of an already-large piece-1 change, would risk exactly
the kind of hasty, under-tested change the project's stage discipline exists
to prevent. So: this entry adds the reusable core the future integration
would sit on, and explicitly does *not* touch `RichText.cs` or
`TextEditingHandler.cs`.

### What was added

`UIX.RenderApi/Microsoft/Iris/Render/Text/Editing/`:
- `TextEditBuffer` - content buffer (backed by `StringBuilder`), selection,
  and undo/redo. `Insert`/`Delete`/`Replace` cover typing, backspace/forward-
  delete, and programmatic replace (IME commit would eventually call
  `Insert` too). `Undo`/`Redo` use a simple one-entry-per-edit stack (each
  `Replace` call is its own undo step) - not coalesced into
  typing-session-sized undo units the way a native RichEdit control does;
  logged as a `// TODO`, deferred as a refinement rather than a correctness
  requirement. `MaxLength`/`ReadOnly` mirror `RichText.MaxLength`/`ReadOnly`.
  `Cut`/`Copy`/`Paste` delegate to an injected `IClipboardAdapter` and no-op
  when none is set.
- `TextSelection` - a `Start`/`End` (not anchor/caret) readonly struct,
  deliberately shaped to match `NativeApi.SpRichTextSetSelectionRange(start,
  end)` so a future integration doesn't need to reconcile different
  selection models.
- `IClipboardAdapter`, `IImeAdapter` - the "small per-platform adapter
  interface" the proposal called for. Neither has an implementation
  registered anywhere (no `TextBackendRegistration`-style seam was added
  either, since there's nothing to register yet - see below). `IImeAdapter`
  is explicitly documented as unwired: Windows' real IME support stays where
  it already is, inside `RichText`'s native hosted mode via
  `NativeApi.SpRichTextForwardImeMessage`/`Win32Api`.

Deliberately excluded caret movement / word-boundary logic (arrow keys,
Ctrl+arrow, Home/End): that's `TextEditingHandler`'s job today (deciding
*where* the caret goes), not the buffer's (which only needs to expose
`SetSelection`) - keeping that split matches the existing separation between
input handling and the underlying edit surface, and keeps `TextEditBuffer`
focused on exactly what the proposal named ("buffer + selection + undo
stack").

### Verification

No test project exists in this repo (per CLAUDE.md). Wrote a disposable
console harness under the scratchpad (not committed) exercising
`TextEditBuffer` directly: insert, selection-replace, backspace, forward-
delete, undo/redo (including "new edit after undo clears the redo stack"),
`MaxLength` truncation (both on `SetText` and on an `Insert` that would
overflow), `ReadOnly` no-op, `Cut`/`Copy`/`Paste` against a fake
`IClipboardAdapter`, and that `TextChanged`/`SelectionChanged` fire. All
passed. `UIX.RenderApi` builds clean on net8.0 (only TFM available on this
Linux dev machine, per CLAUDE.md).

### Not done / explicitly deferred

- Any integration with `Microsoft.Iris.Drawing.RichText` or
  `Microsoft.Iris.InputHandlers.TextEditingHandler` - see "Scope" above. Both
  are completely untouched by this entry.
- Undo coalescing (typing several characters = one undo step).
- Any concrete `IClipboardAdapter`/`IImeAdapter` implementation, for any
  platform including Windows.
- Rich formatting within `TextEditBuffer` (bold/italic/color runs while
  editing) - out of scope; that's `RichText`'s multi-range formatting
  surface (piece 1, above), which is about *measuring/rendering* formatted
  ranges, not editing them interactively.

---

## 2026-07-27 — Enacted follow-up, piece 1: multi-range TextDocument.Measure + non-hosted RichText

User asked to enact both pieces of the "Proposed follow-up: full cross-platform
text engine" entry below, in sequence. This entry covers piece 1 (multi-range
formatting); piece 2 (interactive text-editing engine) is a separate entry
below this one.

### Discovery that changed the scope of piece 1: RichText is used for two very different roles

Before implementing, traced every call site of `Microsoft.Iris.Drawing.RichText`
members outside `RichText.cs` itself (grep across `UIX/`). This surfaced
something the two entries below didn't account for: `RichText` is not only the
interactive-editing control they describe - it is also the *general
multi-line/word-wrapped/formatted text measurement engine* used by
`Microsoft.Iris.ViewItems.Text`, via two static shared instances
(`SharedNonOversampledRasterizer`/`SharedOversampledRasterizer`, both
`new RichText(true)` with `callbacks: null`) plus `DebugOutlines._sharedRichText`
(`new RichText(false)`). These are constructed with `callbacks == null`, so
`_hosted` is `false` and none of the interactive-editing members (selection,
key/mouse forwarding, IME, undo, clipboard, scrollbars, timers) are ever
exercised on them - confirmed by grep: every call to those members goes
through `TextEditingHandler._editControl`, which is a *separate*,
always-hosted instance (`new RichText(true, this)`) swapped in via
`Text.ExternalRasterizer` only while a `Text` viewitem is actively bound to an
editing handler (`UsedForEditing => _externalEditingHandler != null`).

This means: any `Text` viewitem that needs word wrap, multiple lines, or
per-range formatting (`_namedStyles`, e.g. differently-colored hyperlink
spans - see `Text.ApplyContentFormatting`) goes through the *non-hosted*
`RichText.Measure`/`GetNaturalBounds`/`SimpleContent`, not just interactive
text boxes. Since `RichText`'s constructor called
`NativeApi.SpRichTextBuildObject` completely unconditionally (no `#if WINDOWS`
guard, unlike `TextBackendRegistration`'s pattern), this path already crashes
today on non-Windows the first time any wrapped/multi-line `Text` viewitem is
measured - which is extremely common UI. This made "extend the abstraction to
support multi-range formatting" both more valuable and, on its own, useless
without also fixing `RichText`'s construction: the earlier entry's format
(`TextDocument.Measure` gaining formatted-range support) would have had no
caller, because `RichText` could still never be constructed cross-platform to
reach it.

### Revised piece 1: make the non-hosted (display-only) role of RichText cross-platform; hosted role stays exactly as scoped below

Split `RichText` construction on `#if WINDOWS`:
- **`WINDOWS`**: byte-identical to the original code - still unconditionally
  calls `NativeApi.SpRichTextBuildObject`, still wraps `_rtoHandle` in
  `SpTextDocument`, still assigns `_rrcb`. Zero behavior change on Windows,
  satisfying the stage-2 backwards-compatibility rule with no risk, since nothing
  Windows-observable moved.
- **non-`WINDOWS`**: if `callbacks != null` (hosted/interactive), throws
  `PlatformNotSupportedException` with a pointer to this log - explicit,
  documented gap, not a silent stub, consistent with the "scope decision"
  entry below (interactive editing stays native-only for now). If
  `callbacks == null` (display-only), constructs a `SixLaborsTextDocument`
  directly (not `TextDocumentFactory.CreateStandalone()` - that name/seam is
  for ephemeral STO-style documents like `SimpleText`; `RichText` needs a
  *persistent* document, which is what `SixLaborsTextDocument` already is,
  unlike native `SpTextDocument` which has the STO/bound split documented in
  the entry below).

Same `#if WINDOWS` split applied to `Dispose()` (guards
`NativeApi.SpRichTextDestroyObject`) and `Oversample`'s setter (guards
`NativeApi.SpRichTextSetOversampleMode`; on non-Windows the flag is stored but
has no effect yet - `SixLaborsTextDocument`'s rasterizer has no oversampled/
higher-res-AA mode, logged as a `// TODO`, not implemented here). All the
hosted-only members (`ForwardKeyStateNotification`, `Undo`/`Cut`/`Copy`/
`Paste`/`Delete`, `SetTimer`/`KillTimer`, `Scroll*`, etc.) were left
unconditional/untouched: they're only ever called on the always-hosted
`TextEditingHandler._editControl` instance, which now can't be constructed at
all on non-Windows (throws in the constructor first), so they're unreachable
there - matches the established "NativeApi.cs compiles everywhere, only fails
if actually invoked on the wrong OS" convention instead of adding redundant
guards to every method.

### `TextDocument.Measure`'s new formatted-range overload

Added `TextStyleRun` (`UIX.RenderApi/.../Render/Text/TextStyleRun.cs`) - a
plain `FirstCharacter`/`LastCharacter`/`TextStyleInfo Style` DTO, the
cross-platform counterpart to `TextMeasureParams.FormattedRange` +
`_formattedRangeStyles`. Added a `virtual` (not `abstract`, so existing/future
`TextDocument` implementations aren't forced to implement it)
`Measure(content, alignment, baseStyle, IReadOnlyList<TextStyleRun>
formattedRanges, constraint, wordWrap, out GlyphRunInfo[] glyphRuns)` to
`TextDocument`, defaulting to a single-run fallback via the existing
single-style `Measure`.

`SixLaborsTextDocument` overrides it: resolves a font per range via
`SixLabors.Fonts.TextOptions.TextRuns` (confirmed via the package's shipped
XML doc comments, not decompilation - `SixLabors.Fonts`/
`SixLabors.ImageSharp.Drawing` are open-source, so decompiling them would be
the wrong tool; `TextLayout.BuildTextRuns`'s doc comment confirms gaps between
supplied ranges automatically fall back to `TextOptions.Font`, so ranges don't
need to fully cover the content). Bounds are then reconstructed per
"contiguous run of graphemes sharing the same formatted range" *and* per
visual line (grouped by watching `GraphemeMetrics.Bounds.Y` change across
`TextMeasurer.GetGraphemeMetrics`'s output) - not just per formatted range -
because a single styled span can wrap across multiple visual lines, and
downstream rendering positions each returned `GlyphRunInfo` at one bounding
rectangle, so a multi-line span in one run would render as one squashed box
spanning all its lines. This is a "good enough, functionally correct"
reconstruction, not a byte-for-bit match of native `SpRichTextMeasure`'s
per-run callback granularity (which the earlier entry below already
established isn't required - only Windows needs bit-identical behavior, and
Windows doesn't go through this code path at all).

`SpTextDocument`'s bound mode does **not** implement the new overload (no
override added) - it's still unreachable in practice, since on Windows
`RichText.Measure` keeps calling `NativeApi.SpRichTextMeasure` directly
(byte-identical original code, see above) rather than going through
`_textDocument`. Implementing it there would be dead code; if a future change
ever wants `SpTextDocument`-bound formatted measurement, implement it then
against a real caller rather than speculatively now.

`RichText.Measure`'s non-Windows branch converts the caller-supplied
`TextMeasureParams` (a struct callers like `Text.ApplyContentFormatting`
already build in full, unchanged) into `TextStyleInfo`/`TextStyleRun` inline:
reads `TextMeasureParams._textStyle` (a plain managed `TextStyle`, not
marshalled) directly for the base style, and
`TextMeasureParams._formattedRangeStyles[i]` (already-built
`TextStyle.MarshalledData`, populated earlier by
`TextMeasureParams.SetFormattedRangeStyle`) for per-range styles. The latter
is safe to read on any OS despite being an "unsafe" struct with a raw
`char* _fontFace` field: that pointer is pinned managed memory
(`GCHandle.Alloc(fontFace, GCHandleType.Pinned)`), not a native/P-Invoke
handle, so `new string(marshalled._fontFace)` works identically cross-platform
- confirmed by reading `TextMeasureParams.SetFormattedRangeStyle`'s existing
implementation rather than assuming. `measureParams`'s edit-mode-only fields
(`SetEditMode`/`SetPasswordChar`/`TrimLeftSideBearing`) are not honored on
this path - documented as an accepted gap in a code comment, not a silent
regression, since those only matter for hosted/editable usage, which already
can't reach this method on non-Windows.

### Not done / explicitly deferred

- Oversampled (higher-quality AA) rendering on the SixLabors backend - `// TODO`
  in `RichText.Oversample`.
- RTL text in the formatted-range path (`SixLaborsTextDocument.Measure`
  doesn't consult `MeasureFlags.IsRtl`) - matches the pre-existing gap in the
  single-style path, not a new one.
- `SpTextDocument`'s bound-mode formatted `Measure` override - dead code today,
  see above.

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
