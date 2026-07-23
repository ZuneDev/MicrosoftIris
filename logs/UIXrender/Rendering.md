# UIXrender.dll — rendering & message pump log

Append-only. Do not edit previous entries. Newest first.

---

## 2026-07-22 — Landed: real StbTrueType text rendering + backend-agnostic message pump

Built, AOT-published, and tested. Summary of what's now real vs. before.

**Text rendering (was: ratio-approximated measurement + `E_NOTIMPL` rasterization):**
- `Subsystems/Text/LoadedFont.cs` — a TrueType face over StbTrueTypeSharp (pure managed,
  zero GPU). Pins the font bytes (stb stores the data pointer) and exposes scaled
  advance/kerning/ascent/line-height and glyph coverage rasterization.
- `FontStore.cs` — resolves a face name to a font: runtime-registered fonts
  (`SpLoadFontResource`, now real — reads the embedded resource and registers it) →
  platform system-font discovery (Windows/Linux/macOS dirs) → first-available fallback.
  Cached, thread-safe. Returns null when nothing resolves, so callers degrade to the ratio
  metrics rather than faking.
- `TextLayout.cs` — real measurement over actual glyph advances + kerning, with the old
  ratio `TextMetrics` kept strictly as the no-font fallback. Word wrap is real either way.
- `GlyphRun.cs` — the object `hGlyphRunInfo` points at; `Rasterize(color)` composites stb
  coverage bitmaps into a straight-alpha ARGB32 buffer (BGRA-in-memory, matching
  `SurfaceFormat.ARGB32`). `TextBitmap` owns that buffer; `SpFreeDib` (repointed to
  `HandleTable.Free`) releases it.
- Wired: `SpSimpleTextMeasure` now produces a real glyph run + real geometry;
  `SpRichTextRasterize` (was `E_NOTIMPL`) composites it to a bitmap;
  `SpRichTextMeasure`/`SpRichTextGetNaturalBounds`/`SpSimpleTextMeasurePossible` use the
  real font. Outline/shadow passes and rich-text multi-run `rrcb` remain TODO.

**Message pump (was: `PeekMessage`→0, `WaitMessage`→`Thread.Sleep`, `Invoke(null)`→no-op):**
- `Subsystems/Os/MessagePump.cs` — a `ConcurrentQueue<Action>` + `ManualResetEventSlim`,
  with a lost-wakeup guard (reset only when empty, re-check after). No windowing/GPU
  dependency. `IWindowMessageSource` is the pluggable seam for a host/optional backend to
  inject OS messages during Peek.
- Wired through `EngineService`: `PeekMessage` → drain+run queued work, report
  ProcessedMessage(1)/None(0); `WaitMessage` → real bounded wait that returns early on a
  post; `Invoke(null ptr)` → `PostWake` (**the InterThreadWake fix** — a blocked
  `WaitMessage` now actually wakes); async `Invoke(real ptr)` → deferred onto the pump
  thread; `SetWindowMessageSource` exposes the seam. `SpPostDeferredImeMessage` now posts
  its dispatch onto the pump (genuinely deferred). `SpCreateNotifyWindow` now returns
  `S_OK` + a token (was `E_NOTIMPL`, which threw inside `UIForm.Initialize`'s `IFC`).

**Verified:** `dotnet build UIXrender -f net8.0` clean; whole solution builds except the
pre-existing `SimpleDebugClient`/`SimpleIrisApp` errors; NativeAOT `linux-x64` publish
succeeds with **zero trim/AOT warnings** (StbTrueTypeSharp is pure-managed, AOT-safe) and
exports **exactly 193/193** (surface unchanged). `Tests/UIXrender.Engine.Tests` extended
to **66 checks, all passing**, exercising real behaviour: proportional metrics ('WWW' >
'iii', which the ratio path could never produce), word-wrap growth, rasterized ink
(non-zero alpha pixels — glyphs actually drawn), deferred-work execution on Peek,
`InterThreadWake` unblocking a blocked `WaitMessage` in ~50ms (not the 5s timeout),
`WaitMessage` honouring its timeout, and `PeekMessage` reporting the right WorkResult.

**Still open / not done (flagged, not hidden):**
- **No on-screen compositing / real window.** By design (backend-agnostic): text
  measurement + CPU rasterization is complete, but presenting pixels to a screen needs a
  window+GPU backend, which stays out of core. `IWindowMessageSource` is the seam for a
  later optional backend package. `SpPeekMessage` therefore never returns
  `NewUserMessage(2)` (no HWND to Win32-dispatch).
- Text: DPI scaling (fontHeightPts treated as pixels for now), complex-script shaping
  (stb is Latin-oriented), and outline/shadow/underline rendering passes.
- Font family matching is filename-heuristic, not the TTF `name` table / fontconfig — a
  bold/italic style request currently resolves the family's plain face.

## 2026-07-22 — Course-correction: no GLFW/SDL; backend-agnostic pump + pluggable window seam

**What happened:** I asked the user (AskUserQuestion) how to back the message pump and
they picked "Silk.NET.Windowing real window". While setting that up the user pushed back:
*"why are you adding a dependency on GLFW? That's not backend-agnostic."* Correct.

**The constraint, stated plainly:** `Silk.NET.Windowing` is an abstraction over `IWindow`,
but there is **no backend-agnostic way to create a real OS window** — `Window.Create()`
needs a concrete platform provider (GLFW *or* SDL) registered via a backend package at
runtime, or it throws "no platform registered". So "real window" unavoidably pulls in
GLFW or SDL, which violates the backend-agnostic requirement that has governed this whole
effort (and matches FullSurface.md decision 2: UIXrender is loaded into a host that
already owns its window). Nothing windowing-related had been added to the real project
yet — only a throwaway scratchpad probe — so there was nothing to revert.

**Revised design (what's actually being built):**
- **Message pump = a backend-agnostic message queue** owned by the render-session thread.
  This is exactly and only what the render engine's loop uses the pump for, verified from
  the call sites (`UIX.RenderApi/.../Internal/RenderEngine.cs`):
  `IRenderEngine.WaitForWork` → `SpWaitMessage(timeout)` (block until work or timeout);
  `IRenderEngine.ProcessNativeEvents` → `SpPeekMessage(drain)` (drain + report a
  WorkResult); `IRenderEngine.InterThreadWake` → `SpInvoke(ctx, NULL, NULL, false)` (wake
  a blocked wait from another thread). A `ConcurrentQueue` + a wait handle implements all
  three correctly with no windowing/GPU dependency at all.
- **`ProcessNativeEvents`'s `NewUserMessage` (2) path is deliberately never taken.** That
  branch forwards a Win32 `MSG` to `RenderWindow.ForwardWindowMessage` then calls
  `Win32Api.TranslateMessage`/`DispatchMessage` — genuine Win32/HWND dispatch that only
  exists on Windows with a real window. The backend-agnostic pump has no HWND, so
  `SpPeekMessage` returns `ProcessedMessage` (1) when it ran queued work, else `0` (none),
  and leaves `out msg` default — which routes `ProcessNativeEvents` down its non-Win32
  branch (`return TestFlag(nResult, 1)`), exactly right for a windowless pump.
- **Pluggable window seam, no dependency:** a small `IWindowMessageSource` interface plus
  `EngineService.SetWindowMessageSource(...)` lets a host that owns a window (or an
  *optional, separate* Silk.NET.Windowing backend package that never ships in UIXrender
  core) inject OS window/input messages into the pump later. The pump harvests from the
  source during Peek if one is registered. This preserves the "windowing can plug in"
  intent of the user's original menu choice while keeping UIXrender core free of GLFW/SDL.

**Deferred-invoke semantics fixed at the same time:** `SpInvoke` with a **null** function
pointer is `InterThreadWake` → post a wake to the pump (this is *the* fix the user's "must
implement a pump" requirement was about: a blocked `SpWaitMessage` now actually wakes). A
**non-null** async `SpInvoke` enqueues the callback to run *on the pump thread* during the
next Peek (real "deferred invoke to the render thread" semantics), not on a random
threadpool thread as the interim wiring did; a non-null sync `SpInvoke` runs inline.

---

## 2026-07-22 — Plan: text rendering (StbTrueTypeSharp) + message pump

**Task (user):** implement rendering, specifically text rendering, and — mandatory — a
message pump so `SpPeekMessage`/`SpWaitMessage` work correctly. Backend-agnostic; pause
and ask if no suitable abstraction exists (which is why the font backend was an
AskUserQuestion — Silk.NET has no font/text abstraction, and the community option
SilkyNvg was rejected for being OpenGL-only).

**Font backend chosen (user):** StbTrueTypeSharp — pure-managed stb_truetype port,
zero-GPU, NativeAOT-clean, pairs with the StbImageSharp decoder already referenced. Gives
glyph metrics (real measurement) and CPU glyph rasterization (real `SpRichTextRasterize`).
No complex-script shaping — line/word layout is built here (the ratio-based `TextMetrics`
already did the layout; only the per-glyph *width* becomes real).

**Text subsystem shape (`Subsystems/Rendering/Text/`):**
- `FontStore` — resolves a TextStyle font-face name to a loaded font: registered fonts
  (via `SpLoadFontResource`), then platform system-font discovery (Windows `Fonts`, Linux
  `/usr/share/fonts` & friends, macOS), then a first-available fallback. Cached,
  thread-safe. When nothing resolves, callers fall back to the existing ratio metrics and
  rasterization reports `E_NOTIMPL` — honest degradation, not a fake.
- `LoadedFont` — wraps `stbtt_fontinfo` + v-metrics; scaled advance/kerning/ascent/line
  height, and glyph coverage-bitmap rasterization.
- `TextLayout` — real measurement (word wrap over real advances) and a measured run.
- `GlyphRun` — a measured run stored behind a `HandleTable` handle (this becomes the
  `hGlyphRunInfo` the managed side round-trips from measure to rasterize).

**Rasterize lifecycle (read off the managed consumers, not guessed):**
`SpRichTextRasterize` returns `phTextBitmap` + `ppvBits` + size, which
`Microsoft.Iris.Drawing.RichText.Rasterize` wraps in a `Dib`
(`UIX/.../RenderAPI/Drawing/Dib.cs`) that frees `phTextBitmap` via **`SpFreeDib`**. So the
rasterizer allocates one unmanaged ARGB buffer, returns a `HandleTable` handle as
`phTextBitmap` and the buffer pointer as `ppvBits`, and `SpFreeDib` is repointed to
`HandleTable.Free` (its previous body referenced a since-removed GDI helper and only ever
mattered on Windows; the only producer of "DIBs" in this reimplementation is this
rasterizer). `SpRichTextDestroyGlyphRunInfo` already frees the glyph-run handle.
