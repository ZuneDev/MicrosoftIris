# UIX.RenderApi.OpenGL

Reverse-chronological log (prepend new entries; never edit older ones).

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
