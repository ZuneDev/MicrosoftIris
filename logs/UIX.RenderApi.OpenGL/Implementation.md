# UIX.RenderApi.OpenGL

Reverse-chronological log (prepend new entries; never edit older ones).

## 2026-07-31 (final for now) — Added ZUNE_PERFTRACE instrumentation instead of guessing further

**Symptom (user report):** slightly better after the previous entry's fix, but still
awful. The user asked a fair, overdue question: every fix in this session so far was
found by reading code for patterns that *could* cause contention (many small lock
acquisitions, a lock held too long), never by actually measuring where time goes. Nothing
on the app/script side (`Microsoft.Iris.Markup`'s trigger/script execution, dispatched
through `Dispatcher.MainLoop`) had been looked at at all -- it's entirely possible the
real cost is there, not in anything this session touched.

**Added, not a fix:** env-gated (`ZUNE_PERFTRACE=1`) timing instrumentation, off by
default (zero cost unless opted into, same pattern as `GLVisual.TraceEnabled`/
`ZUNE_GLTRACE`), at the three places needed to tell app-side cost apart from
render/locking cost:
- `UIX/Microsoft/Iris/Queues/Dispatcher.cs` (`MainLoop`): times each `QueueItem.Dispatch()`
  call -- this is where UIX markup script/trigger execution actually runs, dispatched
  from the same queue as everything else. Logs `[PERFTRACE] Dispatcher: {ms} :: {debug
  string}` for anything >= 8ms.
- `UIX.RenderApi.OpenGL/Engine/GLRenderEngine.cs` (`DrawFrame`): times the locked
  tree-walk and the unlocked `SceneRenderer.Flush()` separately.
- `UIX.RenderApi.OpenGL/Engine/GLRenderWindow.cs` (`HitTest`): times the whole
  hit-test call.

All four log to stderr with an 8ms threshold, marked `TEMPORARY diagnostic tracing`, to
be removed once the actual bottleneck is confirmed rather than kept as permanent
overhead. **Next step is on the user's machine**, not here: run with `ZUNE_PERFTRACE=1`
set, reproduce the lag, and see which of the four categories (dispatcher/script,
locked walk, GL flush, hit-test) is actually large. That answers the user's question
directly instead of another round of pattern-matched locking fixes.

**Verification:** `MicrosoftIris.sln` and `ZuneHost` build clean (0 errors, same
pre-existing unrelated `SimpleDebugClient`/`SimpleIrisApp` failures only). Not
runtime-verified -- this sandbox still can't display the app's window, so even
`ZUNE_PERFTRACE`'s own output can't be observed from here.

## 2026-07-31 (yet still later) — Still laggy: hit-testing and the child-sort both did many separate lock acquisitions instead of one

**Symptom (user report), after the entry below's fix:** better, but Collection views
still really laggy.

**Diagnosis, two more instances of the same underlying mistake** (many small lock
acquisitions where one would do -- the exact pattern `GLRenderEngine.DrawFrame` already
avoided by locking once for the whole tree walk):

1. **Hit-testing had no outer lock at all.** `GLRenderWindow.HitTest` called straight
   into `GLVisualContainer`/`GLSprite`'s `HitTest`, each of which reads several
   individually-locked properties (`Visible`, `LocalMatrix`, `Alpha`, `MouseOptions`) at
   every level of the tree. For a list with several nested containers and many items, a
   single mouse move could trigger dozens-to-hundreds of *separate*
   `GLRenderSession.SyncRoot` acquisitions, each one an independent chance to collide
   with the render thread -- versus the render thread's own one-acquisition-per-frame
   pattern.
2. **`GLVisualContainer.BackToFrontOrder` read a locked property from inside its own sort
   comparer.** `snapshot.Select(...).OrderBy(t => t.v.Layer)...` reads `.Layer` (which
   locks internally) *during* the LINQ sort, i.e. after the method's own snapshot lock
   had already been released -- meaning every single comparison during the O(n log n)
   sort was its own separate lock acquisition. This method is called on every hit-test
   *and* every render frame, for every container in the tree -- so a list is exactly the
   shape that makes this expensive: many children per container, many containers.

**Fix:**
1. `GLRenderWindow.HitTest` now wraps the entire recursive hit-test walk in one
   `lock (m_session.SyncRoot)`. The nested per-property locks inside `GLVisual`/
   `GLVisualContainer`/`GLSprite` become cheap reentrant no-wait re-entries instead of
   independent contended acquisitions.
2. `BackToFrontOrder` now captures each child's `Layer` value *inside* its existing
   snapshot lock (one lock, reading `m_children[i]` and `.Layer` together while already
   holding it -- reentrant, cheap), then sorts an array of plain `(GLVisual, uint Layer,
   int Index)` tuples with `Array.Sort` and an explicit comparer, entirely outside any
   lock. Same ordering semantics (ascending Layer, ties broken by descending original
   index) as before, just without a lock acquisition per comparison. Also dropped the
   now-unused `System.Linq` using.

**Verification:** `UIX.RenderApi.OpenGL` and `ZuneHost` build clean (0 errors). Same
caveat as every entry in this session -- this sandbox can't display the app's window, so
this is verified by code review and the lock-acquisition-count argument, not by
re-measuring actual latency. Asked the user for more specific repro detail (is it laggy
with no hover/animation involved at all, e.g. just scrolling with the mouse held still?)
in case the remaining cost turns out to be algorithmic (tree-walk/hit-test CPU cost
itself) rather than lock-related -- worth checking before assuming another locking fix
will help.

## 2026-07-31 (still later) — Still unresponsive after the event-driven fix: the lock was still held across GL submission and first-time image decode

**Symptom (user report), after the entry below's fix:** the "Not Responding" window-manager
flag was gone, but Collection views (multiple, not just the one with the already-known
dual-tree bug from the 2026-07-30 entries) remained heavily laggy -- choppy, delayed hover
feedback -- and this was described as happening consistently, not just during active
animation/hover.

**Diagnosis:** the entry below only fixed *how often* the render thread took
`GLRenderSession.SyncRoot` (event-driven instead of every vsync tick); it didn't fix *how
long* it held the lock for each frame it did draw. `DrawFrame` still wrapped the entire
frame -- tree walk *and* actual GL submission -- in one lock, and GL submission includes
`GLImage.EnsureUploaded`, which on first use of an image synchronously calls into
`ImageCacheItem.ReloadImage` -> `ImageLoader.FromBuffer`/`FromFile`, a real, potentially
tens-of-milliseconds CPU image decode. Collection views load new album art constantly as
you scroll or hover between items, so this path is hit constantly there specifically
(unlike Settings, which is mostly text) -- and every single decode blocked every other
app-thread scene mutation (property sets, hit-testing) for its full duration, regardless
of whether the render thread was looping continuously or not.

**Fix, two parts:**
1. `SceneRenderer` (`UIX.RenderApi.OpenGL/Rendering/SceneRenderer.cs`) split from
   immediate-mode into record-then-execute: `BeginFrame`/`DrawColoredQuad`/
   `DrawTexturedQuad` now only append lightweight `DrawCommand` structs to a list (pure
   CPU, no GL calls, no image work) -- call sites in `GLVisual`/`GLVisualContainer`/
   `GLSprite`'s `Render` methods are unchanged. A new `Flush()` does the actual GL work
   (viewport/clear/program setup, then each command's real draw call including
   `EnsureUploaded`).
2. `GLRenderEngine.DrawFrame` now only holds `SyncRoot` around the animation pulse and
   the tree walk (building the command list -- matrix math and property reads, no GL, no
   decode); `SceneRenderer.Flush()` is called *after* releasing that lock. `GLImage` also
   stopped sharing `GLRenderSession.SyncRoot` entirely and went back to its own private
   lock (dropped the `syncRoot` constructor parameter added in the entry two below,
   updated `GLRenderSession.CreateImage` accordingly) -- nothing outside a `GLImage` ever
   needed its pixel/texture state to be atomic with the rest of the scene graph, so
   sharing the global lock there was pure cost with no correctness benefit.

Net effect: the lock the app thread contends with is now held only for CPU-cheap tree
traversal, never for GL calls or image decode. A slow-to-load image's decode cost is
still real (still happens on the render thread, still user-visible as that specific
image popping in late), but no longer blocks anything unrelated to it.

**Verification:** `UIX.RenderApi.OpenGL` and `ZuneHost` both build clean (0 errors).
Same caveat as every entry in this session: this sandbox cannot display the app's window
(Wayland/XWayland capture issue, unrelated to any of this), so this is verified by code
review and the lock-scope argument, not by re-measuring actual interaction latency. If
Collection views are still laggy after this, the tree-walk/hit-test cost itself (not
locking) becomes the next suspect -- worth profiling rather than guessing further.

## 2026-07-31 (later) — Render thread regression: continuous vsync loop caused a lock convoy, badly regressing responsiveness

**Symptom (user report), on a real machine (not this sandbox, where the app's window
couldn't be observed at all -- see the entry below):** noticeably slower page loads in
Settings ("takes many milliseconds"), and clicking "start" made the window-manager mark
the window "Not Responding" for several seconds.

**Root cause:** the entry below's `RenderThreadMain` ran an *unconditional* loop --
`DrawFrame` (under `GLRenderSession.SyncRoot`) then `SwapBuffers()`, forever, regardless
of whether anything on screen had changed -- on the theory that `SwapBuffers()` blocking
for vsync would give the app thread enough breathing room between frames. It does, in
isolation, but the practical effect was the opposite of the intent: the render thread was
re-acquiring the one lock shared by the *entire* scene/animation graph roughly 60 times a
second, permanently, even looking at a completely static settings page. Every single
app-thread property set or tree mutation -- and a page load or a `RepeatCount`/animation
setup on "start" easily does hundreds of them in a row -- now had to contend with a
background thread that was trying to reacquire the same lock on a tight, regular cadence.
That's a textbook lock convoy: individually each wait might be small, but a sequence of
many small mutations each paying a lock-contention tax adds up to the reported
many-millisecond page loads, and under heavier contention (a bigger transition, a slow
first-time image decode inside `GLImage.EnsureUploaded` while still holding the lock) it
compounds into the reported multi-second freeze. `FlushBatch`/`RenderNowIfPossible` had
also been turned into no-ops on the assumption that "the next vsync frame picks it up
anyway" -- true only because the loop never stopped running, which was itself the bug.

**Fix:** made the render thread event-driven instead of a continuous loop. It now blocks
on a `ManualResetEventSlim` (`m_renderRequested`) and only draws when something actually
asks it to: `FlushBatch`/`RenderNowIfPossible` (restored to real wakes, not no-ops --
these are the same two invalidation hooks the original single-threaded engine used,
traced via `UISession.SyncWindowHandler`/`UIDispatcher.DoBatchFlush`), a handful of
window events (resize, focus), or the loop itself re-arming when the frame it just drew
still has a playing animation (`GLAnimationSystem.HasPlayingAnimations`, re-added --
computed from the same snapshot pattern `StepAnimations` already used, still O(1) lock
acquisitions per frame, not per-animation). The wait has a 250ms fallback timeout as a
low-cost safety net (self-heals any invalidation path this design missed, without
reintroducing a tight loop -- 4Hz idle polling is not remotely comparable to 60Hz lock
contention). Net effect: while the app is idle, the render thread touches the lock zero
times; while an animation is actually playing, it behaves the same as the entry below
(contending during the animation's actual duration, which is expected and bounded, not
permanent).

**Verification:** rebuilt `UIX.RenderApi.OpenGL` and `ZuneHost` clean (0 errors). Could
not re-run the responsiveness test myself -- this sandbox's display/screenshot tooling
still can't show the window (see the entry below) -- so this fix is verified by code
review and the lock-acquisition-frequency argument above, not by re-measuring the actual
page-load/click latency the user reported. That measurement should happen on a machine
that can actually show the window before considering this closed.

## 2026-07-31 — Dedicated render thread: painting/animation split off the app/UI thread

**Task (user):** the app/UI thread (`UIDispatcher`'s single owning thread) did window
event pumping, animation pulsing, and GL painting all synchronously in one loop
(`GLRenderEngine.OnRender`, driven by Silk's `Render` event / `DoRender`). A busy
dispatcher callback stalled painting and animation with it -- exactly the bug class
already worked around by `WaitForWork`'s 15ms poll loop. User asked to move as much of
that onto its own thread as feasible to keep the app responsive, without necessarily
moving everything.

**Design (not everything moved, by design):** window creation, native event pumping
(`ProcessNativeEvents`/`WaitForWork`'s `DoEvents` calls), input translation, and
hit-testing all stay on the app/UI thread -- the project references
`Silk.NET.Windowing.Sdl`/`Silk.NET.Input.Sdl`, and SDL's documented threading rule
(OS-enforced on macOS specifically) is that window creation and event pumping must stay
on the thread that initialized the video subsystem. **Not verified on an actual Mac from
this environment** -- flagged rather than silently assumed. Only painting, animation
pulsing, and GPU submission move to a new dedicated render thread
(`GLRenderEngine.RenderThreadMain`), which owns the GL context after a one-time handoff
(`IWindow.ClearContext()` on the main thread right after `OnLoad`, `IWindow.MakeCurrent()`
as the render thread's first action -- both confirmed via ilspy against
`Silk.NET.Core.Contexts.IGLContext`/`Silk.NET.Windowing.WindowExtensions` in the actual
referenced package version, 2.23.0) and loops continuously, paced by the window's vsync
(`WindowOptions.Default.VSync == true`, confirmed via ilspy), replacing Silk's own
`Render` event/`DoRender` pump entirely.

**Synchronization:** one coarse lock (`GLRenderSession.SyncRoot`), not per-object locking
or message-passing snapshotting. Unlike the original native engine (`SpRenderThreadInit`
ran the real Splash engine on its own thread too, per
`UIX.RenderApi/.../Protocol/LocalChannel.cs`, but decoupled via async message-passing so
the two sides never shared memory), this reimplementation's scene graph
(`GLVisual`/`GLVisualContainer`/`GLSprite`/`GLEffect`/`GLImage`/animation objects) is
directly shared, same-process mutable state -- and it's a genuine two-writer situation,
not just reader/writer: `GLKeyframeAnimation.Advance` (now render-thread-only) writes
straight back onto `GLVisual`/`GLEffect` properties every frame via
`AnimationTargetApplier`, the same properties app-side markup/session code sets directly.
A single lock held by the render thread for a whole frame (pulse + draw) and by every
app-side mutator for the duration of its call is deliberately the simplest correct
answer -- GPU submission time dwarfs lock hold time, and render code freely walks from
one object into another's state (e.g. a sprite reading its parent container's size),
which would make per-object locking deadlock-prone for no real benefit.

**Two reentrancy fixes, not just locking:** `GLGraphicsDevice.RenderNowIfPossible`
(reached from app-thread `UISession.RenderNowIfPossible`) and `GLRenderEngine.FlushBatch`
both used to draw a frame inline on whichever thread called them -- illegal now that the
GL context lives only on the render thread. Both became effective no-ops: the render
thread's continuous vsync-paced loop already picks up whatever change prompted the call
on its very next frame (well under 16ms later), so there's nothing to nudge. This is
also a better match for the original protocol's own semantics than the previous inline
draw was: `RemoteNtDevice.SendRenderNowIfPossible` (`UIX.RenderApi/.../RemoteNtDevice.cs`)
fires an async, no-reply message at the separately-threaded native engine and never waits
for a frame. `WaitForWork`'s `HasPlayingAnimations`-triggered `RenderNow()` poll-render
hack was removed for the same reason -- the render thread no longer needs prompting to
keep animating.

**Locking footprint, scoped to where real contention exists:** `SharedRenderObject`
(base for nearly every render object) gained a `SyncRoot` field -- a parameterless
constructor overload gives objects outside the scene/animation graph (sound: `GLSound`/
`GLSoundBuffer`) a private lock with zero call-site changes, while `GLVisual`/
`GLVisualContainer`/`GLSprite`/`GLImage`/`GLEffect`/`GLEffectTemplate`/
`GLAnimationSystem`/`GLAnimation`/`GLAnimationGroup`/`GLKeyframeAnimation` thread the
shared `GLRenderSession.SyncRoot` through their constructors and lock every mutator (and,
where the render thread reads them outside its own per-frame lock -- e.g. hit-testing on
the main thread -- every getter too). `GLCamera`/`GLGradient`/`GLVideoStream` were
deliberately left on the default private lock: confirmed by direct reading that nothing
in the current render path applies camera perspective or evaluates gradients (gradient
compositing was implemented and reverted two entries below, still unused), so there's no
live cross-thread contention on them today.

**Build verification:** `UIX.RenderApi.OpenGL` alone, the whole `MicrosoftIris.sln`
(clean except the pre-existing, unrelated `SimpleDebugClient`/`SimpleIrisApp` errors
noted in earlier entries), and the actual consuming app (`ZuneHost/ZuneHost.csproj`,
`ZuneShell.dll` repo) all build with 0 errors on Linux.

**Runtime verification, honestly reported:** could not visually confirm the running app
in this environment. `dotnet run`'d `ZuneHost` and it stayed alive with no exceptions/
crashes, but its window was never enumerable via `wmctrl`/`xdotool`, and screen captures
(`ffmpeg -f x11grab`) came back solid black with no window content from *any* running
app, not just this one -- consistent with the Wayland/XWayland disconnect already noted
in the 2026-07-29 entry (native Wayland surfaces aren't visible to X11-based capture
tools). To rule out a regression rather than just an environment limitation: `git stash`d
every change in this entry, rebuilt, and ran the unmodified code as a control -- identical
symptoms (process alive, flat/idle CPU, no enumerable window) with the original
single-thread code too. Same behavior with and without this change is evidence against a
new hang/deadlock, but it is not the same as watching the app actually run smoothly --
flagged rather than claimed as a full pass. Whoever has a working display/screenshot
path in this environment (or a Windows/macOS machine) should watch a page transition or
similar animation while deliberately stalling the dispatcher (e.g. a long synchronous
callback) to confirm painting keeps going -- that's the actual regression test this
change is for.

## 2026-07-30 (still later) — Collection "Gallery" hero/grid bleed-through ROOT-CAUSED: not a render-backend bug at all -- two sibling Scroller/Repeater subtrees both `Visible=True` simultaneously, same class of bug as the ModalLayer entry below

Follow-up to the two entries below (gradient compositing implemented, then reverted as
an unrelated regression). With gradients back to their pre-session no-op state, the
user confirmed the *original* reported bug -- the enlarged "now selected" album cover in
the Collection view showing other grid content bleeding through/around it -- was
**still present**, proving gradients were never the actual cause.

### Diagnosis method

Same live-tracing pattern used throughout this log. Added temporary instrumentation to
`GLSprite.Render` (reverted after, confirmed via `git diff` showing only this log file
and the entries below's already-landed changes remain): for each drawn sprite, walked
the full `ParentContainer` chain via reflection (type name + markup `Name`, deduped where
container/sprite share `OwnerData` per the ViewItem pattern established in the
2026-07-27 ModalLayer entry) and printed it alongside the sprite's resolved world-space
rect/alpha/content. Iterated three times, rebuilding and asking the user to
navigate back to the Vulfmon/"Dot" album view each time (this sandbox has a live `DISPLAY`
this session, confirmed via `wmctrl`/`xdpyinfo`, but the ZuneHost window isn't
enumerable via `wmctrl`/`xdotool` -- consistent with the Wayland/XWayland quirk noted in
the 2026-07-29 entry -- so forcing a repaint required the user to hover/interact rather
than a scripted nudge); the last pass added object-identity hash codes and per-ancestor
local `Position`/`Size` to disambiguate structurally-identical `Repeater`-generated
template instances (type-name-only chains looked identical between genuinely different
items because none of the repeated items' intermediate `Host`/`Panel` wrappers carry a
markup `Name`).

Also tried decompiling `ALBUMSPANEL.UIX` and `GALLERYPANEL.UIX` via `UIXC` to read the
real markup/script driving this, per the project's normal "read the source before
guessing" procedure -- both throw the same
`System.ArgumentNullException: Value cannot be null. (Parameter 'source')` inside
`Decompiler.CreateTree`/`DecompileScript` (`UIX.DecompXml/Decompiler.Script.cs:607`/`:28`).
This is a pre-existing bug in the `UIXC` decompiler itself (unrelated to anything in this
session's changes), not something fixed here -- noted as a **TODO** for whoever next
needs to read one of these two files' real markup source, since static reading wasn't an
option this session and everything below was recovered from live object-identity tracing
instead.

### Root cause

The traced ancestor chain for both the big (159x159) hero image and a normal-sized
(86x86) grid thumbnail is identical up through a `Host` element named `"Gallery"` (same
object-identity hash both times, `#C2C9F`), confirming both come from the same logical
Gallery control instance, not two different controls. Immediately below that shared
`Host"Gallery"`, the chain **splits into two different sibling `Panel` children** (distinct
hashes each pass, e.g. `Panel#36873DD` vs `Panel#12AE185`), each hosting its own separate
`Scroller"Scroller"`/`Panel"Background"`/`Repeater"Repeater"` subtree -- one Repeater
generating 159-sized `Host"Button"` items (the "focused/enlarged" mode), the other
generating 86-sized ones (the "small grid" mode). Every level of both subtrees reported
`Visible=True` in the trace. Both subtrees are positioned `pos=(0,0)` within the shared
`Gallery` host with near-identical sizes (`330x356`ish vs `317x356`ish), so they paint
into the same screen rectangle -- explaining the exact reported symptom: the enlarged
"Dot" cover (and its shadow/white-backing/hover-overlay stack, all independently verified
compositing *correctly*, fully opaque, in the right paint order) sits directly on top of
whatever the small grid happened to place in that same cell, with neither subtree hidden.

This is architecturally the same bug class as the already-diagnosed
**2026-07-27 "ModalLayer.Visible never re-evaluates" entry** below: a markup-authored
`Visible` (or equivalent mode-switch) binding on one of these two Panels is supposed to be
mutually exclusive with the other (single-focused-item "Gallery" layout vs. multi-item
grid layout -- plausibly toggled by how many albums the selected artist has, matching
"1 ALBUM" vs. a full grid in the original screenshot) and isn't being kept in sync,
leaving both branches permanently visible. Like ModalLayer, this lives entirely in
Iris's markup script-execution/data-binding engine (`UIX/Microsoft/Iris/Markup/
ScriptRunScheduler.cs` and the surrounding `Script`/trigger infrastructure) --
**predates the OpenGL port, is not a rendering-backend bug, and needs the same
not-yet-written fix flagged (and deliberately not attempted) in that earlier entry.**
No code change was made in this pass beyond reverting the diagnostic tracing; the
`GLGradient`/`GLVisual` evaluator infrastructure from the entry below remains in place,
unused, since it's unrelated to this bug and still a reasonable starting point for a
future gradient-compositing attempt.

`dotnet build ZuneHost/ZuneHost.csproj -f net8.0` clean (0 errors) after reverting the
tracing; `git diff --stat` on this submodule confirms `GLSprite.cs` is back to its
pre-session state (only `GLGradient.cs`/`GLVisual.cs`/`GLVisualContainer.cs` from the
entry below, plus this log, remain changed).

## 2026-07-30 (yet even later) — Gradient alpha compositing (previous entry) REVERTED: widespread invisible-but-hittable elements, unrelated to the original bug

**Symptom (user report), immediately after the previous entry's fix:** on the FUE
welcome wizard, many elements across the page render invisible (the black `DialogBody`
panel, both `ActionButton.Pink*.png` buttons, associated captions) while still being
interactive/hittable -- confirmed by the user clicking through blind. "WELCOME TO ZUNE"
and its orange/pink underline bar remained visible. No correlation found between
element type (image vs. text) and which ones went invisible -- ruling out a bug scoped
to only one draw path.

This is a regression from the previous entry's `EvaluateOwnGradients`/
`EvaluateChildGradients` wiring, not related to the originally-reported Collection
hero-panel bleed-through (which remains unconfirmed/unfixed).

### Investigation before reverting

Checked whether `Clip`/`EdgeFade` (the container-gradient path, previous entry's primary
hypothesis) could be responsible: `grep -rl "<Clip" ZuneShell/Resources/RCDATA/*.UIX`
across every checked-in UIX source in the outer repo returned **zero matches** -- no
`<Clip>` element is used anywhere in this resource tree. So the `EdgeFade`/container-
gradient path can't be what's hiding content on this specific page; the previous entry's
"container gradient with wrong extent" theory doesn't explain this regression by itself.

Remaining live suspect, not yet confirmed: `Text.cs`'s own self-attached gradients
(`gradientClipLeftRight`/`gradientMultiLine`, `CreateFadeGradientsHelper`) are created
automatically as part of text rendering -- not markup-configured via `<Clip>` -- so they
would explain text going dark without any `<Clip>` element anywhere. Doesn't by itself
explain non-text elements (button background images) also going dark unless those share
a gradient-bearing ancestor container with a text sibling, which hasn't been traced yet.

### Action taken

Given the severity (real, currently-shipped UI content going invisible, more disruptive
than the bug being fixed) and no display/interactive access in this sandbox to iterate
live, reverted the *effect* without discarding the investigation: `GLSprite.Render` and
`GLVisualContainer.Render` no longer multiply alpha by `EvaluateOwnGradients()`/
`EvaluateChildGradients()` -- both call sites restored to their pre-fix behavior
(`inheritedAlpha * Alpha` only). `GLGradient.Evaluate`/`GLVisual.EvaluateOwnGradients`/
`GLVisual.EvaluateChildGradients` are left in place, unused, as a starting point for the
next attempt -- the evaluator's piecewise-linear-ramp math may well be correct; the bug
is more likely in *what position gets sampled against what extent* (per-visual center
sampling, or a units/coordinate-frame mismatch between the gradient's authored stops and
this backend's `GLVisual.Size`), not necessarily in `Evaluate` itself.

`dotnet build UIX.RenderApi.OpenGL/UIX.RenderApi.OpenGL.csproj` clean (0 errors).

**Not yet done:** live trace instrumentation (the pattern used successfully for every
other bug in this log) to print, per draw call, which gradients are attached, their
resolved stops, and the sampled position/extent/factor -- needed to find the real cause
before re-attempting. Deferred pending the user's direction on whether to pursue that now
or leave gradients off for the time being. The original Collection hero-panel
bleed-through report is still open and unexplained.

## 2026-07-30 (even later) — Gradients recorded but never composited: implemented alpha-ramp evaluation for edge/line fades

**Symptom (user report):** in the Collection view, the enlarged "now selected" album
cover (a mostly-black image, "Dot" by Vulfmon) showed other grid content -- other album
thumbnails, unrelated track/album text -- faintly visible through/around it, instead of
the grid being cleanly obscured. User explicitly ruled out a UIX markup/style issue
(confirmed nothing relevant in the outer repo's uncommitted `STYLES.UIX` diff) and asked
to look at rendering/resource management instead.

### Diagnosis method

Delegated an initial broad sweep (missing/partial back-buffer clear, dirty-rect logic,
stale texture/render-target caching, double-buffer swap ordering) to a research agent,
which ruled all of those out by reading `SceneRenderer.BeginFrame` (full-viewport
`gl.Clear` every frame, no dirty rects anywhere) and confirmed zero FBOs/render targets
exist in this backend at all. It flagged `GLVisual.Gradients` as recorded but never read
anywhere in the backend, matching this same log's own prior TODO
(`GLGradient`'s doc comment before this fix: "applying them as an alpha ramp during
compositing is left as a stage-3 TODO").

Independently re-verified by reading `GLSprite.Render` directly: it only ever draws a
textured quad, a flat `PrimaryColor` quad, or a `DebugColor` fallback -- `Gradients` is
never consulted. `grep -rn Gradient` across the whole `UIX.RenderApi.OpenGL` project
confirmed the property (`GLVisual.cs:97`, at the time) had no reader anywhere in the
project.

Then read the decompiled call sites to recover the real semantics before implementing
anything (per the project's "don't guess API" rule):
- `UIX/Microsoft/Iris/RenderAPI/Drawing/EdgeFade.cs` -- the classic list/scroll edge
  fade. Creates two gradients (`_minFadeGradient`/`_maxFadeGradient`), each with two
  `AddValue` stops: one full-alpha (`1f`) stop positioned `FadeSize` pixels in from the
  relevant edge, and one `(1 - FadeAmount)` stop right at the edge -- i.e. a literal
  piecewise-linear **alpha** ramp (not a color blend) from full opacity to reduced
  opacity as position approaches a container edge. Applied via `IVisualContainer.AddGradient`.
- `UIX/Microsoft/Iris/ViewItems/Text.cs`/`TextFlowRenderingHelper.cs` -- text's own
  left/right clip fade and multi-line fade, added directly to a `GLSprite`-equivalent
  (`sprite1.AddGradient(gradientMultiLine)`) as well as to a container (`topVisual`).
  Confirms gradients are a general `GLVisual` feature, not container-only.

`RelativeSpace.Min`/`.Max` stop positions are edge-relative offsets (`Max` measured
backward from the far edge, per `EdgeFade.UpdateFades`'s `flPosition2 - FadeSize`
pattern); `RelativeSpace.Global` has no call site that gives it distinct meaning from
`Min` in anything decompiled so far.

### Root cause

Real gap, not stub-vs-real ambiguity: gradients are a genuine, actively-used Iris
feature (list edge-fades, text clip fades) whose OpenGL backend implementation was never
written. `AddGradient`/`RemoveAllGradients` recorded state correctly; nothing ever
turned that state into reduced alpha at render time. For the reported screenshot
specifically: if the Collection hero panel's backdrop (or the grid it sits over) relies
on an edge-fade-style gradient to visually separate the enlarged cover from the grid
underneath, that fade silently no-opped, leaving the grid at full alpha behind it.

### Fix and documented assumptions

`UIX.RenderApi.OpenGL/Scene/GLGradient.cs`: added `internal float Evaluate(float
axisPos, float extent)`, converting stops to absolute local-space coordinates
(`Max` -> `extent - Position`) and piecewise-linearly interpolating `Value` between the
two nearest stops (clamping outside the recorded range). Two assumptions made and
documented in the type's doc comment rather than guessed at silently, per the "dealing
with unknowns" procedure:
- `Value` is used as a pure alpha multiplier. `ColorMask` (recorded, stored, has a
  setter) is **not** applied -- no decompiled call site gives enough signal to tell
  whether it tints the faded region toward a color or is otherwise-unused API surface,
  and blending toward `ColorMask`'s default (opaque black) without evidence risked
  visibly wrong output (e.g. edges fading to black instead of transparent) rather than
  the "no worse than before" bar a documented gap should meet. Left as a `// TODO`
  requiring a native Ghidra cross-check before implementing.
- `RelativeSpace.Global` is treated identically to `Min` (no distinguishing call site
  found).

`UIX.RenderApi.OpenGL/Scene/GLVisual.cs`: added `EvaluateOwnGradients()` (this visual's
own directly-attached gradients, sampled at its own local center -- covers Text's
self-attached sprite gradients) and `EvaluateChildGradients(GLVisual child)` (this
container's gradients, sampled at a child's center within the container's local space --
covers EdgeFade's container-attached pattern). Both multiply together every gradient's
`Evaluate()` result (orientation picks the X or Y axis).

**Known limitation, documented in both methods' doc comments rather than silently
shipped:** these sample at a single center point per visual/child, producing one scalar
alpha factor per draw call -- not a true per-pixel ramp. For a visual small relative to
the fade zone (the common case: list rows, text runs) this is a close approximation; a
single large sprite spanning an entire fade zone (or the hero-panel backdrop itself, if
that's what turns out to be gradient-driven here) would only get one averaged-ish alpha
value rather than visibly fading across its own extent. True per-pixel fidelity would
need gradient stops threaded into the fragment shader as uniforms (`SceneRenderer`
currently has no per-fragment gradient support at all, and only a single scalar `uAlpha`
uniform, not per-vertex/per-fragment alpha) -- a materially bigger change than this fix,
left as a follow-up if the coarser approximation turns out insufficient once visually
verified.

`UIX.RenderApi.OpenGL/Scene/GLSprite.cs`: `Render()`'s alpha computation now multiplies
in `EvaluateOwnGradients()`. `UIX.RenderApi.OpenGL/Scene/GLVisualContainer.cs`:
`Render()` multiplies its own alpha by `EvaluateOwnGradients()` and each child's alpha by
`EvaluateChildGradients(child)` before recursing.

`dotnet build UIX.RenderApi.OpenGL/UIX.RenderApi.OpenGL.csproj` succeeds (0 errors; only
pre-existing warnings in untouched files, none in the four files changed here). Not yet
visually confirmed against the original reported screenshot -- next step is to run
`ZuneHost` and re-check the Collection hero-panel view; if the bleed-through persists,
the single-center-sample limitation above (or an entirely different gradient/backdrop
not yet identified) is the most likely next place to look.

## 2026-07-30 (later) — Nine-sliced images (e.g. wizard buttons) never fade out: fragment shader's nine-slice branch never applied `uAlpha`

**Symptom (user report):** after clicking "Start" on the FUE welcome wizard, the wizard's
page transitions away (the Collection page renders correctly underneath), but the two
pink `ActionButton.Pink.png`/`ActionButton.Pink.Pressed.png` button backgrounds ("Start"/
"change the default settings") stay fully visible on top of the new page indefinitely.
Text captions and plain-color panels on the same wizard page correctly disappeared;
only these two loaded-image sprites persisted.

### Reproduction and diagnosis method

Found `UIX.RenderApi.OpenGL/Scene/GLSprite.cs`/`GLVisualContainer.cs` already had
uncommitted, mid-flight `Debug.WriteLine` tracing from a prior, unfinished session on
this same bug (per `git status` on this submodule at session start) -- switched those to
`Console.Error.WriteLine` (`Debug.WriteLine`'s default listener doesn't reliably surface
on Linux console) and extended the trace in `GLSprite.Render` to print real numeric
`Size`/`EffectiveSize`/`RelativeSize`/resolved world position/resolved `alpha` (the
struct fields have no `ToString()` override, so the original trace was printing bare
type names, not values) plus a per-frame counter in `GLRenderEngine.OnRender`.

Built and ran `ZuneHost` (`dotnet build ZuneHost/ZuneHost.csproj -f net8.0`) with
`DISPLAY=:0`, piping stderr to a log file. The user drove the actual "Start" click (no
input-simulation permission in this sandbox, consistent with the 2026-07-29 FUE entry
below -- an `xdotool`-via-Xwayland-bridge click worked once by luck earlier in the
session but was unreliable enough that self-driving further repro steps wasn't trustworthy,
so subsequent verification went through the user directly). Captured the trace for the
frame after the click settled and grepped it for the wizard page's sprites specifically.

Every wizard-page sprite reported `alpha=0` in the trace -- `DialogBody`'s black panel,
`WELCOME TO ZUNE`, `Wizard.Background.Highlight.png`, both `ActionButton.Pink*.png`
button backgrounds, all of it -- confirming the page's fade-out animation actually ran
and completed correctly (the `PulseTimeAdvance` driver from the entry below is working
as intended here). Yet the user confirmed the two pink button rectangles were still
fully opaque on screen at that same moment. So the bug isn't a stuck `Visible` binding
(ruled out the `ModalLayer`-class bug from the entry below -- the scene graph data here
is correct) and isn't sprites left in the tree (also correct -- they're being redrawn
every frame with the right, fully-transparent alpha value); the GPU side is simply not
honoring that alpha for these two sprites specifically.

### Root cause

`UIX.RenderApi.OpenGL/Shaders/FragmentShader.glsl`'s nine-slice branch (taken whenever
`GLSprite`'s `m_nineSlice` is set, i.e. whenever `ISprite.SetNineGrid` was called --
true for resizable button/panel chrome like `ActionButton.Pink.png`, never for plain
text-caption or flat-color quads):
```glsl
fragColor = texture(uTex, newUV);
```
This copies the sampled texture color straight to output, completely skipping the
`t.a * uAlpha` multiply that both sibling paths -- the plain textured-quad branch
(`fragColor = vec4(t.rgb, t.a * uAlpha)`) and the solid-color branch
(`fragColor = vec4(uColor.rgb, uColor.a * uAlpha)`) -- already do correctly. `uAlpha` is
uploaded correctly from `SceneRenderer.DrawTexturedQuad` every draw (verified: the C#
side's computed `alpha` matches the trace's `alpha=0`); the shader itself just never
reads the uniform on this one code path. Net effect: any nine-sliced image is
permanently drawn at full texture alpha regardless of the sprite's own `Alpha`,
parent-inherited alpha, or any fade animation driving either -- explains both why only
*images* were affected (nine-slicing is an image-only feature; `GLSprite`'s
`PrimaryColor` fill path was never in this branch) and why only the *button backgrounds*
specifically were affected among all the wizard's images (they're the only nine-sliced
sprites on that page -- the plain `Graphic`s like `WizardDropShadow.left.png` and
`Wizard.Background.Highlight.png` use the ordinary textured-quad path and faded
correctly, matching the trace).

### Fix

`UIX.RenderApi.OpenGL/Shaders/FragmentShader.glsl`: changed the nine-slice branch to
sample into a local `vec4 t` and apply the same `vec4(t.rgb, t.a * uAlpha)` alpha
multiply the other two branches already use. Purely a shader fix, no C#/API surface
touched.

Reverted all temporary trace instrumentation added this session (confirmed via
`git diff --stat` showing only `Shaders/FragmentShader.glsl` changed) -- including the
prior session's uncommitted `Debug.WriteLine` calls that were sitting in the working
tree before this session started, since they'd served their purpose. Verified end-to-end
with the user: rebuilt, relaunched, user clicked "Start" again, confirmed the Collection
page now renders cleanly with no leftover wizard content.

## 2026-07-30 — Gated `PopupLayout.GetMouseRect`'s `SpGetMouseCursorInfo` call behind `#if WINDOWS`

Follow-up to the `SpGetMouseCursorInfo`/`UIXRender.dll` crash flagged in the 2026-07-29
entry below (same class of gap as `ExtensionsApi.SpBitmapLoadBuffer`: an unconditional,
un-gated `[DllImport("UIXRender.dll")]` in stage-1 decompiled code, no `#if WINDOWS`, no
managed fallback).

Decompiled `SpGetMouseCursorInfo` itself via the ZuneDesktop Ghidra project
(`UIXrender.dll @ 0x310c0c54`, ordinal 77): it calls Win32 `GetCursorInfo`/`GetIconInfo`/
`GetBitmapBits` on the current system cursor icon, then scans the mask bitmap to trim
blank rows and returns the cursor's effective visible height plus a hotspot Y adjusted to
that trimmed region. Its only caller is
`UIX/Microsoft/Iris/Layouts/PopupLayout.cs:GetMouseRect`, which uses it only when
`PlacementMode.UsesTargetSize` is set, to build a rectangle spanning the visible cursor
glyph (so target-sized popups don't render on top of the pointer). The mouse *position*
itself (`UISession.Default.InputManager.MostRecentPhysicalMousePos`) doesn't touch this
API at all — it already comes from the per-platform input translator.

Fixed by wrapping the native call in `#if WINDOWS` and falling back, on other platforms,
to the same zero-height point rect the method already returns for
`!placement.UsesTargetSize`. No new behavior invented: worst case on non-Windows is a
target-sized popup rendering slightly closer to/over the cursor glyph, a minor visual
detail, not a functional regression. Left a `// TODO` noting the real per-platform story:
X11 has a genuine equivalent (`XFixesGetCursorImage`, which returns the same
width/height/hotspot/pixels info for whatever cursor is currently displayed system-wide),
but Wayland compositors deliberately don't expose another surface's cursor image at all —
there is no general implementation possible there, not just a "not yet ported" gap.

`dotnet build UIX/UIX.csproj -f net8.0` succeeds (0 errors, pre-existing warnings only).
Windows TFM (`net8.0-windows10.0.22000`/`net48`) not build-tested in this environment
(Linux sandbox) but the change is a pure `#if` split around the pre-existing call, matching
the same pattern used throughout `RichText.cs`/`Win32Api.cs`/etc.

## 2026-07-29 — FUE "Start" button freeze: page transitions never animated because nothing ever pulsed `GLAnimationSystem`'s clock

**Symptom (user report):** clicking "Start" on the FUE welcome screen made the window
appear to freeze -- no re-render on further clicks, hover, or resize -- but with no
crash, `SceneRenderer`'s `Draw*` methods still firing every frame as expected, and
dispatcher traces showing hover/input events completing normally.

### Reproduction

Needed a live `ZuneHost` run with the user driving the actual "Start" click (I have no
input-simulation permission in this sandbox by default -- an `xdotool` control-input
permission prompt came up when I tried, and I deliberately left that decision to the
user rather than granting it myself). Two unrelated, pre-existing bugs blocked even
getting a reproduction running, fixed first and logged separately:
- Linux `net8.0` build break (`NativeMessageBox` package mis-gated to Windows-only) --
  `logs/ZuneUI/BuildFixes.md` (outer repo), 2026-07-29.
- Startup crash in `SixLaborsTextDocument.BuildFormattedGlyphRuns` (0-based vs. 1-based
  line numbering) -- `logs/text-abstraction.md`, 2026-07-29.

With those out of the way, temporarily instrumented `SceneRenderer` (a `DrawCallCount`
counter, reset each `BeginFrame`) and `GLRenderEngine.OnRender` (a per-frame trace of
frame number/window size/draw-call count/GL error, all reverted after) and had the user
click Start while the trace ran. Result: before the click, steady `drawCalls=33` per
frame (the static welcome screen). After the click, `drawCalls` jumped to `68`-`70` and
kept rendering every frame with `glErr=NoError` throughout -- i.e. **real new content
(the next page) was being submitted and drawn correctly, every frame, with no errors** --
yet a screenshot taken at the same moment showed the exact same "WELCOME TO ZUNE" pixels
as before the click, including no hover-highlight on the Start button when the mouse sat
over it. This immediately ruled out the two obvious suspects: a `SwapBuffers`/present bug
(new frames clearly *were* being generated) and a GL error swallowing the new draws.

### Root cause

Zune's page/panel transitions (and, apparently, basic hover highlighting too) are driven
by `Microsoft.Iris.Render.Animation.IAnimatableObject`/keyframe animations. On this GL
backend that's `GLAnimationSystem`/`GLKeyframeAnimation`
(`UIX.RenderApi.OpenGL/Animation/`), a from-scratch, correctly-written, purely in-process
reimplementation (interpolation, easing, repeat/reset behavior -- none of that is
suspect). But something has to call `IAnimationSystem.PulseTimeAdvance(int nAdvanceMs)`
every frame with real elapsed time to actually advance any playing animation's clock.

Traced every call site of `PulseTimeAdvance`/`AnimationManager.PulseTimeAdvance`/
`Environment.AnimationAdvance` across `UIX` and `UIX.RenderApi.OpenGL`: **nothing calls
it.** `AnimationManager.PulseTimeAdvance` (`UIX/Microsoft/Iris/Animations/AnimationManager.cs:67`)
exists and forwards to `_session.AnimationSystem.PulseTimeAdvance`, but has zero callers
anywhere in reachable code. In the original architecture this pulse was almost certainly
driven by the native render engine's own internal timer, delivered to managed code via
the remote/message protocol (`RemoteAnimationManager.SendPulseTimeAdvance`,
`UIX.RenderApi/.../Protocols/Splash/Rendering/RemoteAnimationManager.cs`) -- machinery
this in-process GL backend deliberately doesn't use. Nobody wrote an equivalent driver
for it when the GL backend's own `GLAnimationSystem` was built, so every
`GLKeyframeAnimation` that gets `Play()`'d sits forever at its `t=0` keyframe (i.e. its
*initial* value -- literally the state it started in) and is evaluated fresh, at t=0,
every single frame: nothing throws, GL renders it correctly and repeatedly, and it just
never visibly changes. This also explains why hover highlighting looked dead too, since
ordinary hover-state visual feedback is animation-driven the same way -- separate from
the *dispatcher* successfully completing the underlying hover *input* event, which is
unrelated to whether anything animated in response.

### Fix

`UIX.RenderApi.OpenGL/Engine/GLRenderEngine.cs`:
- `OnRender(double deltaSeconds)` now calls
  `((GLAnimationSystem)m_session.AnimationSystem).PulseTimeAdvance(...)` before rendering,
  using Silk's own real per-frame `deltaSeconds` (clamped to 100ms so a pulse following a
  long idle gap -- e.g. the user takes a while to actually click something -- can't jump a
  freshly-started animation straight to its completed state in one step).
- `WaitForWork`'s existing ~15ms poll loop now also calls `RenderNow()` whenever
  `GLAnimationSystem.HasActiveAnimations` (new, backend-internal-only property, added
  purely additively -- doesn't touch the decompiled `IAnimationSystem` contract other
  render backends implement) is true, so a playing animation keeps getting fresh, small
  pulses and keeps rendering every frame until it finishes, instead of only advancing
  whenever something *else* happens to trigger a repaint.

Both changes are additive/backend-internal to the GL engine; no decompiled/stage-1 API
surface changed. Verified end-to-end by having the user click Start against the live,
cleaned-up build: the FUE welcome screen now transitions correctly to the next page (the
main library/Collection view) instead of freezing. All temporary trace instrumentation
(`SceneRenderer.DrawCallCount`, the `OnRender` `Console.Error.WriteLine` trace) was
reverted after diagnosis -- confirmed via `git diff` that only the real fix (this entry's
`GLRenderEngine.cs`/`GLAnimationSystem.cs` changes, plus the unrelated
`SixLaborsTextDocument.cs` line-numbering fix from the same session) remains.

**Not fixed, out of scope:** a second, non-deterministic startup/layout crash surfaced
once during a retry run --
`System.DllNotFoundException: ... UIXRender.dll ...` from
`Microsoft.Iris.OS.NativeApi.SpGetMouseCursorInfo`, called by
`Microsoft.Iris.Layouts.PopupLayout.GetMouseRect` while arranging a popup-placed element.
Same class of gap as the previously-logged `ExtensionsApi.SpBitmapLoadBuffer` one (an
unconditional, un-gated `[DllImport("UIXRender.dll")]` in stage-1 decompiled code, no
`#if WINDOWS`, no managed fallback) -- didn't reproduce on the other two runs this
session (mouse-position-dependent, since it's on `PopupLayout`'s placement-rect
computation), so left alone rather than guessing at a fix under time pressure. Worth a
dedicated look if popups/tooltips turn out to be unreliable on Linux.

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
