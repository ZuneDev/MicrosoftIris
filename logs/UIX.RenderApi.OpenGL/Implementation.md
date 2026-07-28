# UIX.RenderApi.OpenGL

Reverse-chronological log (prepend new entries; never edit older ones).

## 2026-07-27 (later still, cont'd) — RelativeSize fix confirmed correct, but window now renders solid BLACK: a separate, pre-existing bug outside the render backend, in Iris's markup script/data-binding engine

After the `RelativeSize` fix (previous entry, below) the user reported the window was
now solid **black** instead of white -- a different symptom, meaning the fix changed
something real. Re-ran `ZuneHost` for real (screen wasn't locked this time) and confirmed
visually via `spectacle` screenshot: solid black "OpenZune" window.

### Diagnosis method

Added temporary tracing again (reverted after; confirmed via `git diff` that
`GLSprite.cs`/`GLVisual.cs` only retain the `RelativeSize` fix itself, no debug leftovers)
to `GLSprite.Render`, logging every quad's resolved position/size. Confirmed the
`RelativeSize` fix itself is correct: sizes now resolve to real, sane values (root
container 1012x693 matching the actual window; `Wizard.Background.Highlight.png` at
627x693; text captions at their real glyph-run sizes; etc.) -- so that fix stands.

One outlier: a `DrawColoredQuad` at `pos=(-9902,-9999) size=(20625,20691)
color=(0,0,0,1)`, drawn partway through the frame. Fully opaque, solid black, and its
bounds (`x: -9902..10723, y: -9999..10692`) comfortably cover the entire 1012x693
viewport regardless of the odd off-screen anchor -- explains the solid black.

Used reflection on the sprite's `OwnerData`/`ParentContainer.OwnerData` chain (temporary,
reverted) to identify the owning managed objects without needing a `.uib` decompiler:
`Microsoft.Iris.ViewItems.Panel` named `"ColorFill"`, parented under another `Panel`
named `"ColorFill"`, in turn parented under a `Panel` named `**"ModalLayer"**`. Dumping
`LayoutBounds`/`VisualSize` via reflection showed the huge size (`20625 x 20691`) is set
directly on the *managed* `ViewItem`/`Panel` itself, at `VisualPosition=(0,0,0)` -- i.e.
this is a genuine bug in the `UIX`/`ZuneShell` layout/markup layer, not a GL-side
marshaling or sizing bug.

### Root cause

`grep -rl "ModalLayer"` found the readable markup source at
`libs/ZuneUIXTools/test/2.1/POPUPLAYER.UIX` (`PopupLayer.uix`, referenced as
`res://ZuneShellResources!Popup.uix`'s sibling), the always-present layer that dims the
screen behind an active popup/modal dialog:
```xml
<Script>
  bool isModal = [PopupManager.IsModal];
  ...
  ModalLayer.Visible = isModal;
  ...
</Script>
...
<Panel Name="ModalLayer" Visible="true" MouseInteractive="true"/>
```
`ModalLayer.Visible` starts `true` in markup and is meant to be continuously data-bound
to `PopupManager.IsModal` via that `<Script>` block -- `false` whenever nothing is
actually showing a modal popup, which is the case on the setup wizard screen we're
looking at. Runtime confirmed `ModalLayer.Visible == True` regardless, meaning that
script/binding is never (re-)evaluating -- the element is stuck at its markup-authored
initial value forever. `ModalLayer`'s own child background sprite (`"ColorFill"`, the
generic name `ViewItem`'s background-fill mechanism uses for every panel, not something
`PopupLayer.uix` names itself) then paints solid black across whatever its `RelativeSize`
resolves against.

This is a bug in Iris's markup script-execution / data-binding engine
(`UIX/Microsoft/Iris/Markup/ScriptRunScheduler.cs` and the surrounding `Script`/trigger
infrastructure), which predates the OpenGL port entirely and is unrelated to the render
backend -- any renderer would show the same stuck-visible modal layer. The
`20625 x 20691` size itself is a separate, so-far-unexplained detail of *why* an
always-`Visible=true`-by-default `ModalLayer` ends up that particular huge size rather
than, say, the full window size (0,0)-(1012,693) -- not yet root-caused; whatever
mechanism sizes `ModalLayer` (likely meant to cover the current top-level window, or
historically the whole multi-monitor desktop for true modal dimming) is also computing a
wrong value, though this may be moot once the `Visible` binding is fixed and the layer
correctly hides itself when no popup is active.

**Deliberately did not attempt a workaround in the GL render backend** (e.g. special-
casing `Panel` named `"ModalLayer"`, or clipping children to parent bounds as a general
fix) -- the actual bug is that a real feature (data-bound `Visible`) isn't running, and
papering over its symptom in the renderer would mask that AND break the real modal-
dimming behavior once popups/dialogs are used deliberately. This needs the markup
script/binding engine itself fixed, which is a distinctly separate, likely substantial
piece of work from the OpenGL rendering fixes done so far this session -- flagged to the
user rather than diving into rewriting `ScriptRunScheduler`/trigger evaluation blind.

## 2026-07-27 (later still) — Window still all-white after the stride fix: real root cause, `ISprite.RelativeSize` silently ignored by the whole GL scene graph

User reported the window was still all white after the `ImageSharpBitmapInformation`
stride fix (previous entry, below). This time the session's desktop wasn't locked, so
instead of reasoning from code alone, actually built and ran `ZuneHost`
(`dotnet build ZuneHost/ZuneHost.csproj -f net8.0` then ran the produced binary with
`DISPLAY=:0`) and took a real screenshot (`spectacle -b -n -f -o ...`) -- confirmed the
"OpenZune" window really does render as flat solid white, not an inference.

### Diagnosis method

Added temporary `Console.Error.WriteLine` tracing (reverted after, confirmed via
`git diff`/`git status` showing no residual changes to `GLImage.cs`/`SceneRenderer.cs`)
to `GLImage.LoadContent` (pixel sample + whether all pixels were identical),
`SceneRenderer.DrawColoredQuad`/`DrawTexturedQuad` (call counts + the width/height they
were invoked with), and `GLSprite.Render` (Position/Size/Scale/matrix). Ran `ZuneHost`
headless-piped-to-log for a few seconds. The image pixel data itself looked fine (real,
varying byte content, not the all-zero or all-same patterns that would indicate a decode
bug) -- but **every single `DrawColoredQuad`/`DrawTexturedQuad` call reported
`size=(1,1)`**, except a handful of text-caption images which had correct real pixel
dimensions (e.g. `size=(186.44678,15.196289)`). Adding `Scale` to the trace showed it was
*also* stuck at `(1,1)` (`Vector3.UnitVector`, `GLVisual`'s field default) for literally
every sprite. So nearly everything in the UI -- every `ViewItem` background fill, every
button, every non-text image -- was being drawn as a genuine 1x1-device-pixel quad. That
is indistinguishable from "nothing rendered" at any normal window size, and explains
"all white" far better than a pixel-format bug would (a bad format/stride produces
visible garbage, not literal invisibility).

### Root cause

`grep -rn "RelativeSize"` across the whole solution turned up `ISprite.RelativeSize`
(`UIX.RenderApi/Microsoft/Iris/Render/ISprite.cs`) being set to `true` in several places
in the managed `UIX` project that predate the GL backend entirely:
- `UIX/Microsoft/Iris/UI/ViewItem.cs:467-468` -- every `ViewItem`'s background sprite:
  `_backgroundSprite.RelativeSize = true; _backgroundSprite.Size = Vector2.UnitVector;`
- `UIX/Microsoft/Iris/ViewItems/Graphic.cs:341,372,378` -- image content sprites, toggled
  based on stretch mode.
- `UIX/Microsoft/Iris/ViewItems/TextRunRenderer.cs:106` -- text highlight sprites.

The contract (confirmed via the original Dx9-era `Visual.cs`/`RemoteVisual.cs`, which
marshal a `SendSetRelativeSize` message to the native renderer) is: when
`RelativeSize == true`, `Size` is a **fraction of the parent container's size**, not
absolute device pixels -- `Size = Vector2.UnitVector` (1,1) means "100% of
`ParentContainer.Size`" (the normal stretch-to-fill case). `GLSprite` (added from scratch
for this port, with no native/original counterpart) declared the `RelativeSize` property
to satisfy the `ISprite` interface but never once *read* it anywhere -- `GLSprite.Render`
and `GLVisual.ContainsPoint` both always treated `Size` as literal device pixels. So
every relatively-sized sprite (nearly all chrome in the app) rendered as a literal 1x1
quad.

### Fix

`UIX.RenderApi.OpenGL/Scene/GLSprite.cs`: added a private `EffectiveSize` computed
property -- `RelativeSize && ParentContainer != null ? Size * ParentContainer.Size
(component-wise) : Size` -- and used it in `Render` (for both `DrawTexturedQuad`/
`DrawColoredQuad`) and `HitTest`. `IVisualContainer`/`GLVisualContainer` doesn't have a
`RelativeSize` concept (it's `ISprite`-only per the interface), so containers keep using
their own absolute `Size` as before -- confirmed by `ViewItem.cs:198`
(`_container.SetSize(value, bit)`) always being called with absolute layout bounds.

`UIX.RenderApi.OpenGL/Scene/GLVisual.cs`: `ContainsPoint` used to read `Size` off `this`
internally; changed it to take an explicit `size` parameter so `GLSprite.HitTest` can
pass its resolved `EffectiveSize` while `GLVisualContainer.HitTest` keeps passing its own
(always-absolute) `Size` -- otherwise hit-testing would have stayed broken (a 1x1 dead
zone) even after the rendering fix.

This is purely additive to the GL backend (`UIX.RenderApi.OpenGL`); no decompiled/stage-1
code (`ISprite`, `ViewItem`, `Graphic`, etc.) was touched, consistent with the project's
stage discipline -- we're just now honoring a contract those callers were already relying
on.

`dotnet build ZuneHost/ZuneHost.csproj -f net8.0` succeeds (0 errors). Could not get a
second screenshot after this fix -- the desktop session locked itself again partway
through this investigation (same intermittent environment limitation noted in earlier
entries in this log), so this fix is verified by the trace evidence above and the
`RelativeSize` contract cross-referenced against `ViewItem`/`Graphic`/`Visual`/
`RemoteVisual`, not a before/after screenshot comparison. Asked the user to rebuild and
confirm.

## 2026-07-27 (later still) — Images load without erroring but render as (near-)solid white: third bug, bogus stride in `ImageSharpBitmapInformation`

User report, after the two fixes immediately below got `GLImage.LoadContent` actually
being called: images are loading successfully now (per user's own instrumentation), but
the window is all white.

### Root cause

`ImageSharpBitmapInformation.UpdateImageInfo(ImageInfo imageInfo, nint buffer)`
(`UIX.RenderApi/Microsoft/Iris/Render/Bitmaps/ImageSharpBitmapInformation.cs`) computed:
```
nStride = imageInfo.PixelType.BitsPerPixel / 8
```
Two independent problems, verified against ImageSharp 4.0.0's own
`ImageInfo.GetPixelMemorySize()` (`SixLabors.ImageSharp.dll`, decompiled via ilspy MCP):
that method computes total buffer size as
`Size.Width * Size.Height * (PixelType.BitsPerPixel / 8)`, confirming stride (bytes per
*row*) must be `width * bytesPerPixel` -- the code above was missing the `* width`
entirely, giving bytes-per-*pixel* instead (e.g. `4`, not `width * 4`). Separately,
`imageInfo.PixelType` here comes from `ImageInfo`'s constructor reading
`metadata.GetDecodedPixelTypeInfo()` off the *original* decoded file's metadata (e.g. 24bpp
for a JPEG, 8bpp indexed for some PNGs) -- not the 32bpp `Bgra32` buffer that
`UpdateImageInfo(Image)` actually converts to and hands a pointer into.

Net effect on `GLImage.LoadContent`'s row loop (`row = data + y * stride`): with stride
wrong by roughly a factor of `width`, each row advanced by only a few bytes instead of a
full scanline, so for any image wider than one pixel, every row past the first read pixel
data from the wrong offset (effectively walking a diagonal/wrapped slice of the flat
buffer instead of real rows). For icons/thumbnails with light padding or borders, reading
scrambled data like this plausibly washes out to a near-uniform pale result -- consistent
with "images load, but the window is white."

### Fix

Hardcoded `nStride`/`nFormat` in that method instead of deriving them from
`imageInfo.PixelType`: the buffer this overload is *always* called with is the
Bgra32-converted clone from `UpdateImageInfo(Image)`, guaranteed contiguous (that's the
precondition `DangerousTryGetSinglePixelMemory` + `PreferContiguousImageBuffers` enforce)
and always 4 bytes/pixel by construction. Set `nStride = imageSize.Width * 4` and
`nFormat = SurfaceFormat.ARGB32` (which maps to `ImageFormat.A8R8G8B8` via
`SurfaceFormatInfo.ToImageFormat`, matching Bgra32's B,G,R,A byte order exactly -- the
same layout `GLImage.LoadContent`'s default branch already assumes).

`dotnet build UIX.RenderApi.OpenGL/UIX.RenderApi.OpenGL.csproj` succeeds (0 errors). Not
yet visually confirmed in a real window (same environment limitation as prior entries in
this log) -- confirmed via the ImageSharp API contract (`GetPixelMemorySize`'s formula)
and the `GLImage.LoadContent`/`SurfaceFormatInfo` byte-order match, not a screenshot.

## 2026-07-27 (later) — Images never render FIXED: GLImage never requested content, and requesting it the naive way (defaulting `ImageCacheItem.m_fFullLoadRequested = true`) stack-overflowed

User report: after the ColorElem.Color fix (below) made solid fills render, images still
never appear. Traced one concrete case (an `ImageCacheItem`-backed image) and found
`GLImage.LoadContent` is never called, so `GLImage.EnsureUploaded` always bails (texture
pointer/pixel buffer null). Root: `ImageCacheItem.ProcessBuffer` only calls the cheap
`DoHeaderLoad()` (dimensions only, no pixels) unless `m_fFullLoadRequested` is true, and
nothing on the GL side ever sets that flag. Defaulting the field to `true` to test this
caused a stack overflow instead of loading anything.

### Root cause 1: Acquire/Release notification fired backwards, causing the stack overflow

Compared `UIX.RenderApi.OpenGL/Scene/GLImage.cs` against the original decompiled D3D
`Image` class (`UIX.RenderApi/Microsoft/Iris/Render/Graphics/Image.cs`), which is the
only other implementor of the same `ContentNotifyHandler` contract
(`UIX.RenderApi/Microsoft/Iris/Render/ContentNotifyHandler.cs`). In `Image`,
`ContentNotification.Acquire` is a *request* sent by the image itself, from
`OnUsageChange`/`AcquireContent()` (`Image.cs:173-188`), meaning "I have no content, load
me" -- and `IImage.LoadContent(...)` is the *response* the owner (`ImageCacheItem`,
via `ImageLoader.FromBuffer`/`FromFile`) sends back to fulfill that request.
`Release` is sent later, once the owner's data buffer is done being consumed
(`Image.OnDataBufferConsumed`), telling `ImageCacheItem.ReloadImage` to run
`EndLoadImageData()` and free its tracking state.

`GLImage.LoadContent` (as first written) instead invoked
`m_notify?.Invoke(ContentNotification.Acquire, this, data)` itself, at the *end* of
`LoadContent`, after storing the pixels -- i.e. it fired the request notification as a
response, and nothing else ever fired a real request. That's why `LoadContent` was never
called with `m_fFullLoadRequested` defaulting to `false` (no Acquire ever reached
`ImageCacheItem.ReloadImage`). Forcing `m_fFullLoadRequested = true` instead made
`ImageCacheItem.ProcessBuffer` eagerly call `DoImageLoad()` -> `ImageLoader.FromBuffer()`
-> `GLImage.LoadContent()` on every load attempt; `LoadContent`'s own (backwards) Acquire
call then re-entered `ImageCacheItem.ReloadImage(Acquire, ...)` -> `LoadBuffer()` ->
`ProcessBuffer()` (now permanently in full-load mode) -> `DoImageLoad()` ->
`LoadContent()` again, unconditionally, forever -- the observed stack overflow.

Fix (`UIX.RenderApi.OpenGL/Scene/GLImage.cs`): removed the erroneous Acquire call from
`LoadContent`. Added the real request point to `EnsureUploaded` (called once per frame
per drawn image from `SceneRenderer.DrawTexturedQuad`): the first time it's asked to
upload an image with no pixel data yet, it fires `Acquire` itself (guarded by a
`m_loadRequested` flag so it only asks once), then fires `Release` right after --
*after* the Acquire call has fully returned, i.e. entirely outside `LoadContent`'s own
call stack, so there is no reentrancy. (`ImageCacheItem.ReloadImage`'s `Release` handler
`EndLoadImageData()` never reads the `data` parameter, so passing `IntPtr.Zero` there is
faithful to the original contract.) Left `m_fFullLoadRequested`'s default at `false` --
the real fix is that something now actually requests the full load; flipping that
default is unnecessary and was only ever a symptom-not-cause workaround.

Did not port the original `SharedResource`/`ResourceTracker`/`AddActiveUser` machinery
that drives `Image.OnUsageChange` in the D3D backend -- that's a materially bigger,
separate "is this resource visible in the active scene graph this frame" abstraction
that doesn't exist anywhere in the GL renderer yet, and the GL scene graph doesn't need
it to fix this bug: `EnsureUploaded` already only runs for images that are actually being
drawn, which is a faithful-enough substitute for "acquire on first real use" here.
**TODO**: cache invalidation / resize-driven reloads (`ImageCacheItem.RemoveData()`,
`GutterSize` growth, D3D's `OnRestoreContent`) are not wired up on the GL side --
`m_loadRequested` never resets, so a `GLImage` that needs to be reloaded after its first
load won't be. Not needed for the "nothing renders at all" bug; flagging for whoever
implements dynamic image resizing/eviction next.

### Root cause 2 (independent, would still corrupt images after fixing #1): dangling pixel pointer in `ImageSharpBitmapInformation`

`UIX.RenderApi/Microsoft/Iris/Render/Bitmaps/ImageSharpBitmapInformation.cs`'s
`UpdateImageInfo(Image image)` called `image.CloneAs<Bgra32>(...)` and immediately
pinned pixel memory from that clone via `DangerousTryGetSinglePixelMemory(...).Pin()`,
without ever storing a reference to the clone anywhere. The clone was a bare expression
result (not even a local variable), so nothing rooted it for the GC; the JIT can
consider a reference dead as soon as its last IL use passes, which here is before the
pointer stashed in `ImageInfo.Data.rgData` is done being read by callers
(`ImageLoader.FromBuffer`/`FromFile` -> `IImage.LoadContent`, which reads it
synchronously but from several frames up the call stack). This is a classic
missing-GC-root/premature-collection bug: timing-dependent, so it would show up as
garbage pixels, a blank image, or an intermittent crash rather than something
deterministic -- consistent with "black window" even once bug #1 above is fixed.

Fix: `UpdateImageInfo` now stores the Bgra32-converted image in the `_image` field
(disposing whatever `_image` held before, if different) so it stays alive until the
next load or `Dispose()`, and stores the `MemoryHandle` from `.Pin()` in a new
`_pixelHandle` field so it's disposed explicitly rather than discarded. Also removed
`_LoadBuffer`'s now-redundant pre-emptive `.CloneAs<Bgra32>()` (the shared
`UpdateImageInfo` helper already converts to `Bgra32`), since keeping two separate
conversion sites made it easy to fix one and miss the other.

Verified: `dotnet build UIX.RenderApi.OpenGL/UIX.RenderApi.OpenGL.csproj` succeeds (0
errors; only pre-existing decompiled-code warnings, none in the touched files). Not
visually confirmed end-to-end in a real window yet -- same `DISPLAY`/lock-screen
limitation noted in the entry below; the fix is confirmed by code-path tracing against
the reference `Image`/`ImageCacheItem`/`ImageLoader` implementations, not a screenshot.

## 2026-07-27 — Blank/white window FIXED: solid-color "ColorElem.Color" effect fills were never rendered

User report: after the WaitForWork fix (below) got the app past the dispatcher stall
and rendering, the window is still all white.

### Diagnosis method

Same empirical approach as the 2026-07-26 entry: temporarily instrumented (again
`Console.Error.WriteLine`, all reverted afterward, verified via `git diff` showing zero
residual changes to the instrumented files) `GLRenderEngine`'s ctor/`OnLoad`/`OnRender`/
`RenderNow`, `GLRenderWindow.Initialize`/`RaiseLoad`, and `UIForm.OnLoad`. Built and ran
`ZuneHost` (`dotnet build ZuneHost/ZuneHost.csproj -f net8.0` then the produced apphost
directly) on the same Linux desktop as before.

Trace showed the full Load chain completing without any exception --
`Graphic.EnsureFallbackImages()` (the previously-suspected `UIXRender.dll` blocker) did
**not** throw on this path, `UIForm.OnLoad` ran to completion, and `OnRender` fired with
`rootChildren=1`, i.e. rendering genuinely happens. The window's actual clear color
printed as `R0.9529412,G0.9372549,B0.94509804,A1` -- RGB(243,239,241), a near-white gray
that reads as "white" to the eye but isn't the GL default; it's `GLRenderWindow`'s own
`BackgroundColor`. So the base canvas paints correctly -- the bug is that nothing else
ever paints on top of it.

(Tried to get an actual visual screenshot via `spectacle`/`import` on the real desktop
to corroborate, but the session's `DISPLAY` turned out to be a locked lock-screen, not
the desktop the app would show on -- `wmctrl -l` / `xwininfo` never listed an "Iris"
window either. Diagnosis relied on the code trace, not a visual capture.)

### Root cause

`Microsoft.Iris.UI.ViewItem.OnPaint` (`UIX/Microsoft/Iris/UI/ViewItem.cs:434`) paints
every view item's background the same way, whenever `_backgroundColor.A != 0`:
```
_backgroundSprite.Effect.SetProperty("ColorElem.Color", _backgroundColor.RenderConvert());
```
`"ColorElem.Color"` is `EffectManager.ColorEffectProperty`
(`UIX/Microsoft/Iris/Session/EffectManager.cs:15`), the fixed property key the built-in
solid-fill effect template is built around (`ColorEffectTemplate`/
`CreateColorFillEffect`, same file) -- this is the single most common paint path in the
whole UI (any panel/button/control background with a plain color, not an image).

`GLEffect` (`UIX.RenderApi.OpenGL/Scene/GLEffect.cs`) stores effect properties in an
untyped `Dictionary<string, object>` but only ever exposed `PrimaryImage` (scans for
`GLImage`/`IImage[]` values). `GLSprite.Render()` (`UIX.RenderApi.OpenGL/Scene/GLSprite.cs`)
used `PrimaryImage`, and falling back to `DebugColor` when absent -- but `DebugColor` is
a separate developer-debug-only property (see its doc comment) that markup/`ViewItem`
never sets. Net effect: every `"ColorElem.Color"` fill -- i.e. most of the visible
chrome -- was silently dropped, leaving only `GLRenderWindow.BackgroundColor` (the
near-white base clear color) visible. Images (`GLSprite` with a real `GLImage`) would
still have rendered fine; this bug is specific to the color-fill path.

### Fix

`GLEffect.cs`: added `internal ColorF? PrimaryColor` reading `"ColorElem.Color"` out of
the property dictionary, mirroring `PrimaryImage`'s pattern but keyed to the one
property name `ViewItem`/`EffectManager` actually use for solid fills (not a generic
"any ColorF property" scan -- other effects, e.g. `PointLight2D`/`Sepia`, hold `ColorF`
values for unrelated post-process purposes that don't belong on a sprite's own quad).

`GLSprite.cs`: `Render()` now tries, in order: textured quad (real image) -> colored
quad using `effect.PrimaryColor` (the `ColorElem.Color` fill) -> colored quad using
`DebugColor` (unchanged last-resort fallback, kept for whatever already relied on it).

Verified: `dotnet build ZuneHost/ZuneHost.csproj -f net8.0` clean (0 errors), ran the
apphost directly -- reaches the same `OnRender`-with-content state as before with no
new exceptions or trace regressions. Could not visually confirm actual on-screen pixels
in this environment (see above) -- if the window is still visually wrong after this,
next things to check: (1) whether `_backgroundColor.RenderConvert()`/`Color` values
coming out of `Styles.uix` are themselves wrong (the "Unreferenced namespace generic"
warning on that file every run might be masking a real style-resolution problem, not
just noise), (2) whether non-background sprite paints (borders, glyph/text rendering --
`SceneRenderer` only draws flat-colored/textured quads, no text path yet as far as this
pass checked) are similarly falling through a gap like this one.

## 2026-07-26 — Dispatcher stall FIXED: premature Load event + busy-spin WaitForWork

User report: `GLRenderEngine.WaitForWork` seems to block the message queue far more
than it should — `nTimeoutInMsecs` is often quite large (e.g. 113995ms), and waiting
for that whole period is not acceptable.

### Root cause

`TimeoutManager.NextTimeoutMillis` (`UIX/Microsoft/Iris/Session/TimeoutManager.cs:31`)
only returns `uint.MaxValue` when there is *no* pending timeout; a large-but-finite
value like 113995 is legitimate — it means some queue item really is scheduled ~114s
out. That much isn't a bug by itself: on real Windows, `WaitForWork`'s native
equivalent (`MsgWaitForMultipleObjectsEx`-style message wait) blocks for up to that
long too, but wakes immediately the instant *any* window message arrives, regardless
of how far away the scheduled timeout is.

`GLRenderEngine.WaitForWork` (as fixed 2026-07-26, see below) approximates that with a
poll loop: `Wait(15ms) → DoEvents() → repeat`, breaking early only when
`m_wakeRequested` is set by `InterThreadWake()` — which is exclusively a *cross-thread*
signal (`UIDispatcher.WakeDispatchThread` → `UISession.InterThreadWake()` →
`_engine.InterThreadWake()`). But `m_silkWindow.DoEvents()` inside that same loop pumps
Silk.NET's native event queue *synchronously on the same thread* — mouse/keyboard
input goes through `GLInputTranslator` → `IRawInputCallbacks.HandleRawMouseInput`/
`HandleRawKeyboardInput` (`UIX/Microsoft/Iris/Input/InputManager.cs:189` /`:178`), which
posts straight onto `InputManager`'s `_inputQueue` (wired into `UIDispatcher`'s master
queue at `queues[6]`, see `UIDispatcher.cs:45`) with no interthread marshaling needed,
since it's already on the UI thread. Window events (resize/move/close/focus) work the
same way via `GLRenderWindow.Raise*`. None of this ever set `m_wakeRequested`, so a
mouse click or keypress that arrived via `DoEvents()` sat queued but invisible to
`WaitForWork`'s loop condition — the loop kept sleeping in 15ms slices all the way to
the full timeout (or an unrelated cross-thread wake) before the dispatcher got a chance
to look at the queue again. Net effect: input could be delayed by up to
`nTimeoutInMsecs`, i.e. the UI looked hung whenever a distant timeout happened to be
pending.

### Fix

`GLRenderEngine.cs`: factored the wake logic out of `InterThreadWake()` into a private
`WakeWaitLoop()` (sets `m_wakeRequested` + signals `m_wakeEvent`, same as before).
`InterThreadWake()` now just calls it — no behavior change for the cross-thread path.
Additionally call `WakeWaitLoop()` from:
- the `Resize`/`Move`/`Closing`/`FocusChanged` Silk window-event lambdas in the
  constructor (previously just re-raised the Iris window event with no wake), and
- `GLInputTranslator`, via a new `Action onInputDelivered` constructor parameter
  (`GLInputTranslator.cs`) invoked right after each `cb.HandleRawKeyboardInput`/
  `HandleRawMouseInput` call (`OnKeyDown`, `OnKeyUp`, `OnKeyChar`, and the shared
  `DispatchMouse` used by move/down/up/scroll/double-click).

Since `WakeWaitLoop()` runs synchronously inside the `DoEvents()` call that's already
inside `WaitForWork`'s loop body, the very next loop-condition check
(`!m_wakeRequested`) sees it and exits immediately instead of waiting out the rest of
the poll slice, let alone the full timeout. Verified with
`dotnet build UIX.RenderApi.OpenGL.csproj` (0 errors, only pre-existing warnings) — no
runtime input trace was re-captured for this pass; if input still feels laggy after
this, check whether `IWindow.DoEvents()` itself is buffering/coalescing before invoking
Silk callbacks (Silk.NET/GLFW internals, not our code).

## 2026-07-26 — Dispatcher stall FIXED: premature Load event + busy-spin WaitForWork

User report (console ZuneHost on Linux): everything initializes with no errors, but
the Iris dispatcher only ever processes 2 items total, every other priority queue
stays empty forever, and the app spins doing nothing (high CPU, no crash, no window
visible via `wmctrl`).

### Diagnosis method

Static reading first (own pass, not just the earlier Explore-agent summary — verified
every claim against the actual source before trusting it): read `UIDispatcher`
(`UIX/Microsoft/Iris/Session/UIDispatcher.cs`), `PriorityQueue`
(`UIX/Microsoft/Iris/Queues/PriorityQueue.cs`), `UISession.cs`, `Form.cs`, `UIForm.cs`,
`GLRenderEngine.cs`, `GLRenderWindow.cs`. Then **empirically verified** by temporarily
instrumenting (`Console.Error.WriteLine`) `Dispatcher.MainLoop`'s per-item dispatch
(reusing the `debugString` already computed there for `Trace.WriteLine`),
`UISession.Schedule*`/`Process*`, and `GLRenderEngine.ProcessNativeEvents`/
`WaitForWork`, then building and running `ZuneHost` under `dotnet build -f net8.0` on
Linux (Wayland/XWayland desktop, `wmctrl` to check for a mapped window). All temporary
instrumentation was reverted before the real fix — `git diff` was checked file-by-file
against pre-existing WIP (`UISession.cs`/`Dispatcher.cs` were untouched otherwise;
`PriorityQueue.cs` had unrelated pre-existing WIP from a prior session, left alone).

Trace of the very first run confirmed the user's "2 items" exactly:
```
[TRACE] dispatch: Microsoft.Iris.UI.UIForm.InitializeWindow
[TRACE] dispatch: Microsoft.Zune.Shell.StandAlone+<>c.<Startup>b__1_0()
```
(the second is the `Windowing.ForceSetForegroundWindow` deferred call queued eagerly
at `DispatchPriority.Idle` in `StandAlone.Startup`, see [[Windowing]] in the ZuneDBApi
logs) — then nothing else, ever; `UISession.ScheduleInitialization`/`ScheduleLayout`
never fired even once.

### Root cause #1 (the actual stall): Load event raised before anyone can hear it

`Form`'s constructor (`UIX/Microsoft/Iris/Session/Form.cs:39`) subscribes
`InternalWindow.LoadEvent += OnRenderWindowLoad` — but `Form` can only be constructed
*after* `UISession`'s constructor has already built the render engine
(`Form(UISession session)` calls `session.GetRenderWindow()`, which needs `_engine` to
already exist). `GLRenderEngine`'s constructor eagerly did
`m_silkWindow.Initialize(); m_windowLoaded.Wait();` and its `OnLoad()` handler called
`m_window.RaiseLoad()` — i.e. the **one-shot** Iris `LoadEvent` was raised and consumed
during `UISession`'s own constructor, long before `Form` (which lives inside that same
call chain, but is constructed later, elsewhere in `ZuneApplication.Launch`) ever got a
chance to subscribe. `Form.InitializeWindow()` (the dispatched item we saw,
`Form.cs:60`) then called `GLRenderWindow.Initialize()`, which just called
`m_window.Initialize()` on the *already-initialized* Silk window — a no-op that does
not re-raise `Load` (Silk doesn't fire `Load` twice). Net effect: `Form.OnRenderWindowLoad`
→ `UIForm.OnLoad()` → `OnInitialize()` (Zone/markup construction, the thing that calls
`UISession.ScheduleUiTask(Initialization)` and kicks off Layout/Render) **never runs**.
No exception anywhere, matching "everything initializes with success."

Fix: stopped raising `LoadEvent` from `GLRenderEngine.OnLoad()` (still does all the real
GL/device/session setup there, just not the Iris-level notification). Moved the
`RaiseLoad()` call into `GLRenderWindow.Initialize()` (`UIX.RenderApi.OpenGL/Engine/GLRenderWindow.cs`),
which is what `Form.InitializeWindow` calls, i.e. exactly the point where `Form` has
already subscribed. Removed the now-redundant/harmful second `m_window.Initialize()`
call there (window+GL context already exist by construction time); `Initialize()` now
just applies the requested `InitialClientSize` and raises `Load`.

**Verified**: rebuilt, reran — the dispatch chain now reaches all the way into
`UIForm.OnLoad() → Graphic.EnsureFallbackImages() → ... → ExtensionsApi.SpBitmapLoadBuffer`
(full stack trace observed), i.e. far past where it used to permanently stall. It then
throws `DllNotFoundException` for `UIXRender.dll`/`libUIXRender.dll` — a **separate,
pre-existing, unrelated** gap: `Microsoft.Iris.Render.Extensions.ExtensionsApi` has
several unconditional `[DllImport("UIXRender.dll")]` declarations (bitmap
load/decode) left over from stage-1 decompilation that were never ported for
non-Windows (no `#if WINDOWS` gate, no managed fallback). Out of scope for this fix —
flagged here as the next blocker for anyone continuing this thread; needs a stage-2
managed image-decoding path (e.g. `System.Drawing`-free decoder or `SixLabors.ImageSharp`)
behind the same abstraction, TODO.

### Root cause #2 (independent, would matter once #1 is fixed further): busy-spin idle wait

`UIDispatcher`'s `Sleep`-priority drain hook (`WaitForWork`, `UIDispatcher.cs:279-297`)
always reports `didWork = true` (by design — it's supposed to represent "we did some
waiting") and always calls `UISession.WaitForWork(nextTimeoutMillis)` when there's no
pending timeout to process immediately. `TimeoutManager.NextTimeoutMillis` returns
`uint.MaxValue` as its "no pending timeout" sentinel (`TimeoutManager.cs:36`) — verified
this is exactly the value observed at runtime (`4294967295`). Because `didWork` is
always `true` here, `PriorityQueue.GetNextItemWorker` restarts its scan from the top
every single call (`PriorityQueue.cs`, drain-hook-did-work branch) — this is correct
*only if* the engine's `WaitForWork` actually blocks for a while first. `GLRenderEngine`'s
`WaitForWork(uint)` (`UIX.RenderApi.OpenGL/Engine/GLRenderEngine.cs`) had its real wait
loop commented out and returned instantly. Combined with `ProcessNativeEvents`'s
one-shot `m_didWork` latch (`true` exactly once, `false` forever after — never reset),
the loop bounces `RPC → Sleep → RPC → Sleep → …` at native speed: confirmed **~90% CPU
on the main thread** via `ps -T` before the fix (all other threads idle), with no
window ever appearing in `wmctrl -l`.

Fix: `WaitForWork` now does a real bounded wait using a `ManualResetEventSlim`
(`m_wakeEvent`), polling `m_silkWindow.DoEvents()` every ~15ms so window/input events
aren't starved during a long/indefinite wait, and treating `nTimeoutInMsecs >=
int.MaxValue` as "wait until `InterThreadWake` signals us" (matches the
`uint.MaxValue` sentinel). At the end of a wait it sets `m_didWork = true` again so the
next `ProcessNativeEvents` call reports "did work" once per wait cycle — restoring the
intended `SpPeekMessage`/`SpWaitMessage`-style rhythm (pump once, then actually sleep)
instead of a permanent one-shot. `InterThreadWake()` now signals the same event.

This part is code-reviewed but not empirically observed in a steady idle state in this
session — the run above hits the `UIXRender.dll` crash (root cause above) before the
dispatcher ever reaches genuine idle, so there was no window to watch spin at 0% CPU.
Logic re-verified by inspection (bounded `ManualResetEventSlim.Wait` per 15ms slice,
real blocking primitive, no remaining unconditional-instant-return path). Worth a
follow-up empirical check once the image-loading blocker above is resolved.

## 2026-07-25 — Animation curve formulas RECOVERED from native (Ghidra)

Resolves the "unverifiable — the real curves/formulas run in native code" caveat
from the entry below. The formulas are now **verified**, not assumed. Source:
`UIXrender.dll` (the native Splash render engine) in the `ZuneDesktop` Ghidra
project. All addresses below are in that image.

### How the pieces fit (verified end-to-end)
- Managed `KeyframeAnimation.SendInterpolation` (UIX.renderapi.dll) does NOT compute
  curves; it sends a distinct, parameterized message per curve type to native
  `RemoteAnimation`. Opcodes (from `RemoteAnimation` Msg structs):
  8=SetEaseOut, 9=SetEaseIn, 10=SetBezier, 11=SetCosine, 12=SetSine,
  13=SetSCurve, 14=SetLogarithmic, 15=SetExponential, 16=SetLinear.
  Params carried: Exp/Log/SCurve → `flWeight`; EaseIn/Out → `flWeight`+`flHandle`;
  Bezier → `flHandle1`+`flHandle2` (= ControlPoint1/2); Sine/Cosine/Linear → none.
  Every message also carries `fSpherical` (→ `UseSphericalCombination`).
- Native message dispatch table for the Animation class is at `0x311f9d90`
  (indexed by opcode). Handler[8..16] each allocate a small C++ "interpolation"
  object, store its params (weight @obj+0x10, handle/cp2 @obj+0x14), set a
  per-type vtable, and stash it in the keyframe array element (stride 0x18) at
  `keyframe+0x10`. Spherical flag → bit 0 of `obj+0xc`.
- Each interpolation object's vtable slot 1 is its **evaluate** method with
  signature `eval(obj, float t, uint channelCount, float* A, float* B, float* out)`.
  `t` is the already-normalized segment fraction [0,1]; A/B are the two keyframe
  endpoint value vectors (up to 4 floats). Evaluate computes an eased factor `f`
  then calls the **combine** routine: `FUN_310bb984` = linear `out = (1-f)·A + f·B`
  (per component), or `FUN_310bba70` = **slerp** `out = (sin((1-f)Ω)·A + sin(fΩ)·B)/sinΩ`,
  `Ω = acos(dot(Â,B̂))`, normalized per channel count, lerp fallback when Ω≈0
  (used when `UseSphericalCombination`).

### The core weighted-exponential ease (`FUN_310bbf90`)
```
ExpEase(x, w) = (w == 1) ? x : (pow(w, x) - 1) / (w - 1)
```
This single function underlies Exponential, Logarithmic, and SCurve.

### Per-type eased factor `f(t)` (verified)
- **Linear** (`FUN_310bbee0`):        f = t
- **Sine**   (`FUN_310bc128`):        f = sin(t · π/2)                       ← ease-out shape
- **Cosine** (`FUN_310bc1a4`):        f = sin((t−1)·π/2) + 1 = 1 − cos(t·π/2) ← ease-in shape
- **Exponential(w)** (`FUN_310bbf1c`): f = ExpEase(t, w)                     (w = Weight, >0)
- **Logarithmic(w)** (`FUN_310bbfdc`): f = ExpEase(t, 1/w)                   (reciprocal exponent)
- **SCurve(w)** (`FUN_310bc05c`):
    t < 0.5 : f = 0.5 · ExpEase(2t, w)
    t ≥ 0.5 : f = 0.5 + 0.5 · ExpEase(2(t−0.5), 1/w)                         (symmetric S)
- **Bezier(cp1, cp2)** (`FUN_310bc230`), u = 1−t — a **quintic Bézier** (Bernstein
  degree 5) easing with control values P0=0, P1=0, P2=cp1, P3=cp2, P4=1, P5=1:
    f = 10·cp1·u³·t² + 10·cp2·u²·t³ + 5·u·t⁴ + t⁵
  (`π` constant used by Sine/Cosine is the float `3.1415927`, not a double.)

### EaseIn / EaseOut are VALUE-SPACE, not scalar (`FUN_310bc3d0` / `0x310b8948`)
These do NOT remap `t` and lerp straight A→B. They split the segment at time
`handle` (h, 0<h<1) around a **computed intermediate control value** `mid`:
```
d      = ExpEase(0.99, w)
d      = (1 − d) · h
ctrl[] = (d / ((1−h)·0.01 + d)) · (B − A)      // per component
mid[]  = A + ctrl                               // intermediate control value

if (t >= h):  f = ExpEase((t−h)/(1−h), 1/w);  combine(mid, B, f)
else:         f = t / h;                       combine(A,   mid, f)
```
EaseOut (`0x310b8948`, vtable `0x3108b6f8`) is the mirror. Consequence for our
renderer: EaseIn/EaseOut **cannot** be expressed as a scalar `Ease(interp, t)`
fed to a plain A→B lerp — they need `mid` computed in value space and a
sub-segment choice. Flagged in code; the scalar path handles the other 7 types
exactly.

### Native vtables (for future reference)
Linear `0x3108b678`, SCurve `0x3108b6a8`, Sine `0x3108b6b8`, Cosine `0x3108b6c8`,
Bezier `0x3108b6d8`, Exponential `0x3108b688`, Logarithmic (shares ExpEase via
1/w), EaseIn `0x3108b6e8`, EaseOut `0x3108b6f8`.

### Also confirmed while here
- Keyframe time is **seconds** on the wire; native converts to ms via `×1000`
  then rounds to int (e.g. AddTimeEvent handler `0x310b853c`: `FUN_310e7d28(t*1000.0)`).
  Matches the existing time-unit assumption.
- Interpolation belongs to a **segment**, keyed by keyframe index
  (`idxKeyframe = keyframeIndex-1` when !BackCompat, else `keyframeIndex`). Our
  evaluator currently reads `b.Interpolation` (the segment's END keyframe); native
  stores per keyframe slot — the exact start-vs-end association for BackCompat is
  still worth a targeted check but does not affect the formulas above.

## 2026-07-25 — Real keyframe animation evaluation

Replaced the no-op animation stubs with a working evaluator. `GLKeyframeAnimation`
now advances a clock, finds the surrounding keyframes, eases + interpolates their
values and writes the result onto every target property. Repeat, auto-reset and
reset-behavior are implemented; Reference/Scale are applied as `ref + scale*value`.

New files: `Animation/AnimValue.cs` (resolve/lerp/slerp values),
`Animation/AnimationEasing.cs` (curve → eased t), `Animation/AnimationTargetApplier.cs`
(write value to a named property with channel masking). `GLAnimation` gained
protected play-state setters + an abstract `Advance`.

**One UIX.RenderApi change (approved by the human — "public accessors"):** added
`public virtual bool AnimationInput.TryGetConstantValue(out object)` (false by default)
and an override on `ConstantAnimationInput` returning its masked value. This is the
only supported way for a separate assembly to read keyframe values — the payload was
`internal`, and the original animation *engine* lives inside UIX.RenderApi so it never
needed a public accessor. `BinaryOperation` already exposes its operands publicly, so
expression inputs (relative keyframes) fold over `TryGetConstantValue` leaves; no
reflection is used anywhere.

Confirmed against the UIX consumer (`AnimationManager`, `KeyframeAnimation`):
- UIX sets `BackCompat = true`, so keyframe 0 is NOT auto-populated with the initial
  value (we honor the flag; the auto-keyframe only happens when BackCompat is false).
- UIX drives `PulseTimeAdvance` itself, so the render loop must NOT pulse animations.
- Keyframe 0 at t=0 = initial value (original behavior, used when !BackCompat).

Documented assumptions (unverifiable — the real curves/formulas run in native code;
logged per the CLAUDE.md unknowns procedure):
- Time units: pulse is milliseconds (`nAdvanceMs`), keyframe times / InstantAdvance are
  seconds. Our conversion matches both.
- RepeatCount: 0 = play once, N>0 = N extra loops (N+1 total), <0 = infinite.
- Easing shapes are standard curves matched to each interpolation class's name
  (Bezier falls back to smoothstep since its control points are internal).
- Reference/Scale combine as `reference + scale*value`.

Known gap (NOT done — needs a decision): stage/time/progress/value **event dispatch**.
`AnimationEvent`'s ctor hard-casts its target to the render-internal
`IActivatableObject`, which an external animation object cannot implement, so UIX's
`AnimationProxy` (which registers `AnimationEvent(anim, "AsyncNotify", …)` for
Complete/Reset) can't target our animations, and completion/reset notifications don't
flow back. Resolving this needs `IActivatableObject` made public + implemented on our
animation objects (+ an in-process activation dispatch), a larger change than the value
accessor. Events are stored today but not fired. Flagged to the human.

Validation: compiled the whole project against the prebuilt `UIX.RenderApi.dll` + a
one-method harness shim for the new `TryGetConstantValue` (the prebuilt DLL predates it)
— build succeeded.

## 2026-07-25 — Input-event translation (Silk.NET.Input)

Wired real keyboard/mouse input via `GLInputTranslator`, created by the engine on
window Load from `IWindow.CreateInput()`. It hooks every keyboard/mouse (and new
devices via `ConnectionChanged`) and dispatches to the registered
`IRawInputCallbacks`.

Message-id conventions were confirmed by reading the UIX consumer, not guessed:
- Keyboard (`KeyboardDevice.OnRawInput`): the `message` is the `KeyboardMessageId`
  ordinal — 0 Down, 1 Up, 2 Char, 3 SysDown, 4 SysUp, 5 SysChar. We emit Sys*
  variants while Alt is held. Character events put the char in
  `RawKeyboardData._virtualKey` (matches `OnRawKeyCharacter`'s `(char)_virtualKey`).
- Mouse (`MouseDevice.OnRawInput`): the `message` is the Win32 `WM_*` code
  (0x200 move, 0x201/0x202 L down/up, 0x204/5 R, 0x207/8 M, 0x20A wheel,
  0x20B-D X buttons, dblclk variants 0x203/6/9/20D). Wheel delta is `scroll.Y*120`.

`InputModifiers` is computed from live Silk key/button state each event. Silk
`Key` → Iris `Keys` (Win32 VK) mapping uses contiguous-range arithmetic for
A–Z / 0–9 / Keypad / F-keys plus an explicit table for the rest; unmapped → None.

Hit-testing: added `GLVisual.HitTest` (frontmost-first, honoring `Visible` and
`MouseOptions.Hittable`) so `RawMouseData._visNatural` is the visual under the
cursor. `_visCapture` is the `SetCapture` site when set (now stored on
`GLInputSystem.CaptureSite`), else the natural target. Screen<->local uses the
inverse of the visual's world matrix (same row-vector convention as rendering).

TODOs (still stage-3): HID/AppCommand (media/remote) and drag/drop translation are
not sourced from Silk yet; `_repCount` is a simple per-key press counter; keyboard
`_flags` is 0.

Validated the same way as before (compile against the prebuilt `UIX.RenderApi.dll`
+ Silk.NET, incl. Silk.NET.Input) — build succeeded.

## 2026-07-25 — Initial in-process OpenGL renderer

### Goal
Stage-3 enhancement: an in-process, OpenGL-backed implementation of the
`Microsoft.Iris.Render` interfaces defined in `UIX.RenderApi`, as an alternative
to the native/messaging (`Splash`) engine. New project only; `UIX.RenderApi` is
referenced, not modified. Targets net8.0 and net48. Uses Silk.NET (Windowing,
OpenGL, Input, Maths). PolySharp polyfills for the netfx target.

### Layout
`UIX.RenderApi.OpenGL/` (namespace `Microsoft.Iris.Render.OpenGL`):
- `OpenGLRenderApi` — public factory (mirrors `RenderApi.CreateEngine`, which we
  cannot reuse because it hard-codes the native `RenderEngine` and must not change).
- `Engine/` — `GLRenderEngine` (owns the Silk window + GL context, pumps events,
  renders each frame), `GLRenderSession` (object factory), `GLGraphicsDevice`,
  `GLRenderWindow`, `GLDisplayManager`/`GLDisplay`, `GLInputSystem`,
  `GLHwndHostWindow`.
- `Scene/` — `SharedRenderObject` (ISharedRenderObject usage counting), `GLVisual`
  (transform base), `GLVisualContainer`, `GLSprite`, `GLImage` (GL texture),
  `GLGradient`, `GLCamera`, `GLEffect`/`GLEffectTemplate`, `GLVideoStream`.
- `Animation/` — `GLAnimationSystem`, `GLAnimation`/`GLAnimationGroup`,
  `GLKeyframeAnimation`, `GLExternalAnimationInput`/`GLAnimationInputProvider`.
- `Sound/` — silent `GLSoundDevice`/`GLSoundBuffer`/`GLSound`.
- `Rendering/SceneRenderer` — GL shader program + quad drawing.

### Rendering approach (real logic)
Orthographic, top-left origin, pixel-space projection (`CreateOrthographicOffCenter(0,w,h,0,-1,1)`),
alpha blending, depth test off, draw order = tree order then by `Layer`. Sprites
draw as quads; content from the sprite's effect's first image (a lazily-uploaded
GL texture, BGRA), otherwise the sprite's DebugColor. Visual transform =
translate(-center) · scale · rotate(axis-angle) · translate(center) · translate(position),
accumulated down the tree with inherited alpha.

Matrix note: Silk.NET.Maths matrices are row-major; we upload with `transpose=false`
so GLSL reads the transpose and the shader uses column-vector order
`uProj * uModel * v`, which reproduces the row-vector composite. See
`SceneRenderer.UploadMatrix`.

### Documented stubs / TODOs (stage 3 follow-ups)
- Sound is silent (TODO: Silk.NET.OpenAL).
- Effects are typed property bags; no shader compilation. 9-slice, coord maps and
  gradient alpha ramps are recorded but not composited.
- Animations run the play-state machine and fire events but do not yet interpolate
  target properties.
- Video streams carry metadata only.
- `GLHwndHostWindow` tracks state only (native HWND embedding has no portable GL form).
- Back-buffer capture signals completion without writing a file.
- `GraphicsDeviceType` has no OpenGL member; we report `Direct3D9` as the
  hardware-accelerated stand-in (the UI branches on GDI-vs-accelerated).

### Multi-targeting decision
Task asked for net8.0 + net48. In this repo net48 only builds on Windows (the
referenced projects, incl. UIX.RenderApi, only emit their netfx output there — see
MicrosoftIris/Directory.Build.props and ZuneDBApi/CLAUDE.md "on Linux only net8.0
TFMs build"). So the csproj declares `net8.0` always and adds `net48` only under
`IsOsPlatform('Windows')`, mirroring the projects it depends on. PolySharp +
Silk.NET (netstandard2.0) cover the netfx target.

### Blocker observed (not caused by this project)
`UIX.RenderApi` does not currently build for net8.0 in the working tree: the
`MicrosoftIris` submodule is checked out on branch `exp/native-libs-impl` at
`651dcab "Empty UIXrender and UIXsup, retry impl"`, which emptied `UIXrender`.
`UIX.RenderApi/Protocol/EngineApi.cs` (non-netfx path) needs
`Microsoft.Iris.Render.Engine`/`.Interop` from `UIXrender`, so net8.0 compilation
fails until UIXrender is reimplemented. The superproject-recorded submodule commit
`7d96b4f` also fails to build UIXrender (missing Win32/HANDLE interop). `UIX.RenderApi`
itself is byte-identical between those two commits.

Because of this, the new project cannot be compiled through its `ProjectReference`
in the current tree. It was instead validated by compiling all of its sources
against the prebuilt `UIX.RenderApi.dll`
(`ZuneImpl/bin/x64/Debug/net8.0/UIX.RenderApi.dll`, 2026-07-22) plus the Silk.NET
packages — result: build succeeded (only CS0067 "unused event" warnings for the
not-yet-wired window/animation events). The user's submodule checkout was left
untouched (a temporary checkout of `7d96b4f` for inspection was reverted back to
`exp/native-libs-impl`).

Open question for the human: once UIXrender's managed engine is restored so
`UIX.RenderApi` builds for net8.0, this project should build via its ProjectReference
with no changes. Should `UIX.RenderApi.OpenGL` also be added to a solution
(ZuneUIXTools.sln)? Left out for now to avoid editing the submodule's tracked
solution during its WIP.
