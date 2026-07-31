# UIX.RenderApi — Text rendering log

Reverse-chronological log (prepend new entries; never edit older ones).

## 2026-07-30 (even later) — Fixing the offset bug below exposed a third, previously-masked bug: spurious re-wrap during `Rasterize` drops trailing words (e.g. FUE title "WELCOME TO ZUNE" renders as "WELCOME TO", "ZUNE" silently missing)

**Symptom (user report, with screenshot):** scaling and clipping now look correct for
most text, but some strings are still missing content in "strange" ways -- specifically
the FUE welcome title renders only "WELCOME TO", dropping "ZUNE" entirely (no visible
clipped/cut-off glyph fragment -- the word is just gone).

### Diagnosis

`Rasterize` was setting `richTextOptions.WrappingLength = glyphRun.RenderBoundsWidth` --
the *tight ink bounding box width* computed by `MeasureCore`'s
`TextMeasurer.MeasureBounds`, reused as if it were the layout width to wrap against.
These are not the same number: `MeasureBounds`'s ink box only covers visible pixels, and
can be a hair narrower than the actual glyph-advance width the text occupies (e.g.
trailing right-side bearing on the last glyph isn't "ink" but is still layout width).
Re-running `DrawText` with `WrappingLength` set to that slightly-too-narrow value can
make SixLabors decide the last word doesn't fit and wrap it onto a second line -- a wrap
`MeasureCore` never made (it measured "WELCOME TO ZUNE" as a single line, so
`RenderBoundsHeight`/the bitmap are only tall enough for one line). The wrapped-away
second line is drawn outside the single-line-tall bitmap, so it's not merely clipped at
an edge -- it never appears in the output at all, which matches the report of "ZUNE"
being wholly missing rather than partially cut off.

This was invisible before the `Dpi` fix (glyphs rendered ~25%+ smaller than the box, so
even a full extra word's worth of advance still fit under the same nominal
`WrappingLength`), and invisible before the `Origin` fix (clipping from that separate bug
dominated). Fixing both prior bugs finally let this latent one manifest on its own.

### Fix

`Rasterize` must reuse the *same* wrap width `MeasureCore` actually wrapped against, not
recompute one from the ink box. Added `GlyphRunInfo.WrapWidth` (defaults to `-1f`,
matching `SixLabors.Fonts.TextOptions.WrappingLength`'s own "unconstrained" sentinel, so
it's safe to assign unconditionally). `MeasureCore` now has an `out float wrapWidth`
returning `options.WrappingLength` after the existing `constraint.Width > 0` check sets
it (or leaves it at `-1f`); the single-run `Measure()` overload stores it on the
`GlyphRunInfo` it builds. `Rasterize` now sets `WrappingLength = glyphRun.WrapWidth`
instead of `glyphRun.RenderBoundsWidth`. The formatted-range overload
(`BuildFormattedGlyphRuns`/`FlushGlyphRun`) leaves `WrapWidth` at its `-1f` default
deliberately -- each `GlyphRunInfo` it produces is already a single already-wrapped
line/range fragment (see that method's own comments), so it must never be re-wrapped at
all, which `-1f` (no constraint) guarantees. `dotnet build .../UIX.RenderApi.csproj`
succeeds with no new errors. Not yet visually re-verified against a running `ZuneHost`
(no interactive session this pass) -- please confirm on your end, including a
multi-line-wrapped caption (not just a short unconstrained title) since that's the one
path `WrapWidth` is intended to still actively constrain.

## 2026-07-30 (later) — Fixing the DPI mismatch below exposed a second, previously-masked bug: glyphs draw clipped against the bitmap edges

**Symptom (user report):** after the `Dpi` fix immediately below, text is no longer
blurry/undersized, but is now visibly cut off (e.g. descenders/tops of tall glyphs
missing).

### Diagnosis

`SixLaborsTextDocument.Rasterize` sizes its output `Image<Bgra32>` to the *tight ink
bounding box* — `width/height = ceil(glyphRun.RenderBoundsWidth/Height)`, which come
straight from `MeasureCore`'s `TextMeasurer.MeasureBounds` result (`fontRect.Width/
Height`). That same `fontRect` also has an `X`/`Y` — the offset from the text layout's
nominal origin to the top-left of the actual ink (e.g. the gap above a string with no
ascenders, or shifted bearing) — carried through as `glyphRun.RenderBoundsX/Y` and used
verbatim as `TextRun`'s/the sprite's world position in `ViewItems/Text.cs`
(`sprite1.Position = new Vector3(x, run.RenderBounds.Top, 0f)`), confirming the bitmap is
meant to contain *only* the tight ink box, positioned externally at `(RenderBoundsX,
RenderBoundsY)`.

But `Rasterize` drew with `Origin = PointF.Empty`, i.e. at the text layout's nominal
origin, not shifted back by `(-RenderBoundsX, -RenderBoundsY)`. So the ink was drawn
`RenderBoundsY` pixels lower (and `RenderBoundsX` pixels further right) than the bitmap
actually accounts for. Before the `Dpi` fix this was invisible: the glyph rendered ~25%+
smaller than the bitmap, so there was enough blank margin at the bottom/right to absorb
the mispositioned ink without touching the edge. Once glyphs render at their correct
(full) size, that same offset walks the ink past the bitmap's bottom/right edge and it
clips.

### Fix

Set `Origin = new PointF(-glyphRun.RenderBoundsX, -glyphRun.RenderBoundsY)` in the
`RichTextOptions` used by `Rasterize`, so the ink's own bounding box — not the nominal
line layout box — aligns to the bitmap's `(0,0)`. `dotnet build .../UIX.RenderApi.csproj`
succeeds with no new errors. Not yet visually re-verified against a running `ZuneHost`
(no interactive session this pass) — please confirm on your end.

## 2026-07-30 — Text renders too small (excess whitespace) and blurry: `SixLaborsTextDocument.Rasterize` never set `Dpi`, `MeasureCore` did

**Symptom (user report):** rendered text is noticeably smaller than its layout box
(leaving too much whitespace around it), and the glyphs themselves look blurry/soft,
while other on-screen graphics (images, solid-color panels) render sharp. Scoped to the
cross-platform `SixLaborsTextDocument` backend (`Microsoft.Iris.Render.Text` in
`UIX.RenderApi`), which is used wherever the native Windows RichEdit-based `TextDocument`
implementation isn't available.

### Diagnosis

`SixLaborsTextDocument.MeasureCore` (used by both `Measure` overloads and
`GetNaturalBounds`) builds its `SixLabors.Fonts.TextOptions` with:
```csharp
var options = new TextOptions(font) { Dpi = MonitorSystem.Instance.GetDpi() };
```
`GetDpi()` returns the real screen DPI on both backends that implement `IMonitorSystem`
(`Win32MonitorSystem.GetDpi()` calls native `SpGetDpi()`; `GlfwMonitorSystem.GetDpi()` is
`GetDisplayScale() * 96f`) — i.e. 96 or higher, never 72, regardless of platform or
display scaling.

`SixLaborsTextDocument.Rasterize` (the method that actually draws the glyphs into the
`Image<Bgra32>` backing `RasterizedGlyphBitmap`) builds a separate `RichTextOptions`
without ever setting `Dpi`:
```csharp
var richTextOptions = new RichTextOptions(font)
{
    Origin = PointF.Empty,
    WrappingLength = glyphRun.RenderBoundsWidth
};
```
Decompiled `SixLabors.Fonts.TextOptions` (checked directly via ILSpy against the
installed `SixLabors.Fonts.dll`, both the 2.1.2 and 3.0.0 packages resolve the same
default) shows `Dpi` defaults to `private float dpi = 72f;`. SixLabors scales a font's
point size to actual rendered pixels by `size * dpi / 72`, so:
- `MeasureCore` lays out/sizes the glyph run assuming `dpi = 96` (or the real, possibly
  higher, screen DPI) → the `GlyphRunInfo`/`RasterizedGlyphBitmap` box (`RenderBoundsWidth/
  Height`, consumed as `sprite1.Size` in `ViewItems/Text.cs`'s `CreateVisuals`) is sized
  for `size * 96/72` (or more) pixels of glyph.
- `Rasterize` then draws the actual glyph ink at the SixLabors default `dpi = 72` →
  `size * 72/72 = size` pixels — i.e. only `72/96 ≈ 75%` (or less, on scaled displays) of
  the size the box was built for.

Net effect: glyphs are rasterized measurably smaller than the box they're placed in
(explains the excess whitespace), and because the true rendered size is smaller than
intended, the same nominal font size ends up rendering at a smaller effective pixel size
than the rest of the (correctly-DPI-aware) UI expects — small anti-aliased glyph edges
read as soft/blurry relative to sharp, natively-sized image content, which goes through
an entirely separate loading path (`ImageLoader`/`RawImage`) that isn't affected by this
mismatch at all. This is consistent with the report that only text — never other
graphics — looked wrong.

### Fix

Added the same `Dpi = MonitorSystem.Instance.GetDpi()` to the `RichTextOptions` built in
`Rasterize`, matching `MeasureCore`, so glyphs are drawn at the exact pixel size the box
was measured for. `dotnet build
libs/ZuneUIXTools/libs/MicrosoftIris/UIX.RenderApi/UIX.RenderApi.csproj` succeeds with no
new errors/warnings after the change.

**Not yet visually verified against a running `ZuneHost` instance** (no interactive
session available this pass) — if the fix doesn't fully resolve the blur on a real
display, the next thing to check is `HintingMode` (currently left at its SixLabors
default of `None`) on both `TextOptions`/`RichTextOptions`, since small anti-aliased
glyphs without grid-fit hinting can still look softer than the original Zune client's
ClearType/GDI-hinted text even once the size matches.
