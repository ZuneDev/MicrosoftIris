# UIXrender.dll — full public surface completion log

Append-only. Do not edit previous entries. Newest entries at the top (matching
`Architecture.md`/`EngineCore.md`'s actual observed order despite the header wording).

---

## 2026-07-22 — Complete: 193/193 exports implemented, verified against the built binary

**Result: the export surface is complete and mechanically verified.** All 193 `Sp*` entry
points that the four managed consumers P/Invoke for are implemented, and the
NativeAOT-published `UIXrender.so` exports **exactly** those 193 — no missing export (which
would be a runtime `EntryPointNotFoundException` waiting to happen) and no stray one.
Verified by diffing `nm -D --defined-only UIXrender.so` against the `extern` declarations
in `NativeApi.cs`/`ExtensionsApi.cs`/`FormApi.cs`/`Protocol/EngineApi.cs`; the diff is
empty. That check is now a CI step (new `linux` job) so it can't silently regress.

**Verification actually performed (not just "it compiles"):**
- `dotnet build MicrosoftIris.sln` — clean apart from the 5 pre-existing
  `SimpleDebugClient`/`SimpleIrisApp` errors already documented in `EngineCore.md`
  (`IDebuggerClient.InterpreterStep`, `DebugSettings.DebugConnectionUri` — debugger APIs,
  untouched by this work).
- `dotnet publish -r linux-x64 -p:PublishAot=true` — succeeds, **zero trim/AOT warnings**
  (see the AOT section below; they were fixed, not suppressed wholesale).
- `Tests/UIXrender.Engine.Tests` — extended from 8 checks to **49, all passing**, and they
  exercise real behaviour rather than construction: UIXList mutation/move/availability,
  `WaveParser` against a hand-built RIFF/WAVE buffer, `XmlLiteReader` walking elements/
  attributes/text/EOF, `RichTextObject` copy-paste/undo/redo/read-only/max-length,
  `TextMetrics` wrapping, `SchemaRegistration` reflecting over a probe type, and
  `UIXVariant` round-trips. Reached via a new `InternalsVisibleTo`, since the exports
  themselves are `[UnmanagedCallersOnly]` and uncallable from C#.

**AOT correctness fixes made while publishing (these were real, not cosmetic):**
- `Enum.GetValues(Type)` in `EnumSchema` was an **IL3050** — it constructs a `T[]` of the
  enum type at runtime, which can genuinely fail under NativeAOT, and this project
  publishes AOT. Replaced with `Enum.GetValuesAsUnderlyingType`, which needs no dynamic
  array construction.
- The reflection trim warnings (IL2026/IL2070/IL2075) were resolved by annotating the
  schema entry points with `[DynamicallyAccessedMembers(All)]` and, where the types
  genuinely come from a host-chosen assembly loaded at runtime (`SpLoadDll`), by
  `[UnconditionalSuppressMessage]` **with a written justification** — the trimmer cannot
  see into a runtime-chosen assembly by definition, so the host must root markup-visible
  assemblies itself. Suppressing with a reason keeps a future *real* warning from being
  lost in the noise.

**What is real vs. what is honestly not — the part worth reading before trusting this:**

Real, working logic: memory, tracing, engine transport (local + a genuinely functional
TCP/named-pipe/UDP remote channel), the ~48-export reflection/schema system, UIXList,
data-binding/query, XmlLite (`System.Xml`-backed), image decode (StbImageSharp,
cross-platform), WAV/PCM parsing, module + embedded-resource loading, HTTP/file download,
string & image handle marshaling, the `IRawUIXServices` callback bridge, IME
registration/dispatch, registry-change notification (Windows), and the entire rich-text
**editing** model (content, selection, clipboard, undo/redo, read-only, max length, wrap,
scale, scrollbars, timers, key forwarding) with real `IRichTextCallbacks` notifications.

**Not real, and deliberately failing loudly rather than faking success** — each returns
`E_NOTIMPL` (or `0`/null) with a `// TODO`, so a caller gets an honest error instead of
plausible-looking garbage:
- `SpDx9CompileEffect` — no cross-platform effect-compilation abstraction exists; binding
  `d3dcompiler_47.dll` would violate the dependency policy (decision 1 below).
- `SpRichTextRasterize` — needs a glyph rasterizer. Returning an empty bitmap would render
  as invisible text and read as a layout bug, so it fails instead.
- `SpBitmapLoadResource`, `SpLoadFontResource` — Win32 resource sections / platform font
  registration, no equivalent yet.
- `SpCreateNotifyWindow`, `SpExtractDroppedFileNames` — Win32 message-only window and
  shell drag-drop.
- `TransportProtocol.VC` — meaning never recovered (`Architecture.md` open question #2).

**Approximate, and flagged as the single biggest caveat: `TextMetrics`.** Measurement is
derived from font-height ratios typical of Latin UI faces, not from real font tables.
Line height and baseline are close to exact; **per-character advance is an average, so
measured text width is approximate and will not match a real rasterizer**. Word wrapping
itself is real logic — only the width feeding it is estimated. Practical consequence:
layout driven by these numbers will be plausible but not pixel-accurate. This is the top
open question below; it should be replaced wholesale by a font backend, not built upon.

**Documented assumption (per CLAUDE.md's uncertainty procedure):** `SpXmlLiteCreateXmlReader`
receives a byte buffer whose encoding is unstated — `NativeXmlReader` feeds it either a
pinned UTF-16 string (no BOM) *or* a raw file buffer (typically UTF-8). Real XmlLite
sniffs the encoding, so this does the same: BOM first, then the "every second byte is
zero" pattern that identifies BOM-less UTF-16 ASCII, else UTF-8.

**Things read off the decompiled sources rather than guessed** (each would have been a
silent, hard-to-find bug):
- `ListChanged`'s type codes are `Microsoft.Iris.Data.UIListContentsChangeType`, confirmed
  from `DllProxyList.ListChanged`'s direct cast — not invented.
- `SpXmlLiteRead`/`MoveToFirstAttribute`/`MoveToNextAttribute` must return a **failing**
  HRESULT at end-of-stream, because `NativeXmlReader` gates its loops on `SUCCEEDED(...)`
  — returning `S_FALSE` (the natural XmlLite idiom) would have produced an infinite loop.
- `SpXmlLiteCreateXmlReader`'s `length` is a **byte** count (`content.Length * 2` over a
  pinned string), not a character count.
- Every `bool`/`out bool` in the original `DllImport`s carries no `[MarshalAs]` override,
  so the CLR marshals it as a 4-byte Win32 `BOOL` — these are `int` on the export side,
  not `byte`. Getting this wrong would have corrupted the stack on every such call.
- The COM callback interfaces are all `ComInterfaceType.InterfaceIsIUnknown`, so their
  vtable slots start at 3; slot assignments follow each interface's declaration order,
  read from its own source file.

**Open questions carried forward (not guessed at):**
1. **Text measurement/rasterization needs a real font backend** — the largest remaining
   gap by far. Needs a decision on taking a text-shaping dependency (SixLabors.Fonts,
   HarfBuzz) versus platform-native text APIs behind an abstraction. Deliberately not
   decided unilaterally in this pass.
2. `TransportProtocol.VC` (value 1) — still unidentified.
3. The ~26 "export-only" natives with no managed caller anywhere in this repo
   (`SpCreateObject`, `SpFindClass`, `SpAttachWndProc`, `SpGetMessageA/W`, ...) — **not**
   implemented, because there is no signature to mirror and CLAUDE.md forbids guessing.
   They're absent from the built library; if a consumer surfaces, they can be added then.
4. Whether the schema subsystem's CLR-type projection matches what real `.uix` markup
   expects at runtime — it satisfies the *shape* of every accessor, but nothing in this
   repo exercises markup against it end-to-end yet.

---

## 2026-07-22 — Landed: shared infrastructure, memory, tracing, transport remainder, schema, assets, lists

Written and building clean (`dotnet build UIXrender -f net8.0`) as of this entry. Logging
mid-flight per the user's reminder, not at the end.

**Shared infrastructure (new, used by every subsystem below):**
- `Engine/HandleTable.cs` — the one place opaque `IntPtr` handles are minted. Every export
  that hands back a "pointer" (schema objects, bitmaps, XML readers, rich-text objects,
  data queries) returns a `GCHandle`, never a real object address. Safe because *nothing*
  outside `UIXrender` is permitted to dereference these — all four managed consumers type
  them as opaque `IntPtr`/`HANDLE`. Generalises the convention `SpRenderThreadInit`
  already used for its thread handle.
- `UIXInterop/NativeString.cs` extended with `InternUni`/`AllocUni`. Non-obvious detail
  worth recording: exports like `SpQueryTypeName`/`SpQueryPropertyName` hand out a `char*`
  that **the managed caller never frees**, so the callee must own it for the process
  lifetime. Interning by value bounds that — schema names come from a fixed registered set,
  so the pool stops growing after each name's first request, instead of leaking one
  allocation per call. Per-call values (query results, `ToString` output) use the
  non-interned `AllocUni` instead.
- `Interop/Com/ComVtable.cs` — callback interfaces (`IUIXListCallbacks`, `IRawUIXServices`,
  `IRichTextCallbacks`, `IImeCallbacks`) can't be received as `[MarshalAs(Interface)]`
  params by an `[UnmanagedCallersOnly]` method, so they arrive as `IntPtr` and are called
  through their vtable. Slot numbering starts at 3 because all four are
  `ComInterfaceType.InterfaceIsIUnknown` — verified from each interface's own attributes,
  not assumed.

**Subsystems landed:**
1. **Memory** — `SpMemAlloc`/`SpMemFree` real via `NativeMemory`. **Correction made while
   writing**: `SpFreeDib` initially routed to GDI's `DeleteObject`; removed. This
   implementation never *creates* DIB sections (bitmaps are our own unmanaged allocations),
   so calling GDI to free them would have been both wrong and a needless graphics-API
   dependency. Now frees through the same allocation path that produced it.
2. **Tracing** — `SpUpdateTraceSettings`/`SpLogTrace` now real (prefix/timestamp/category
   formatting, debugger + optional file sink), joining the existing init/uninit pair.
3. **Engine transport remainder** — `SpInvoke` (real: sync direct call / async via
   thread pool, through the supplied function pointer), `SpWaitMessage` (real timeout
   wait), `SpPeekMessage` (documented "no message available" — there is no Win32 message
   queue behind this yet and `LocalChannel` never calls it), `SpObjectRelease` (real
   `IUnknown::Release` through the object's own vtable), and the four `SpRemote*` entry
   points.
4. **Remote channel** (`Subsystems/Remote/RemoteServerConnection.cs`) — genuinely working
   out-of-process transport over `TcpListener`/`NamedPipeServerStream`/`UdpClient`, with a
   length-prefixed `BufferInfo` + payload framing and a reader thread that dispatches into
   the *same* `EngineService`/`ContextRegistry` the local channel uses — so a remote
   context is indistinguishable from a local one to the rest of the engine.
   `TransportProtocol.VC` returns `E_NOTIMPL`: its meaning was never recovered (open
   question #2 in `Architecture.md`) and CLAUDE.md forbids guessing.
5. **Native reflection / type-schema** (~48 exports, the single largest group) — real,
   projecting **CLR** types through the original export surface via `System.Reflection`.
   Recorded as a deliberate substitution, not a stub: the C++ gadget classes the original
   reflected over don't exist here and never will, but every accessor keeps its original
   signature and semantics, so the managed caller can't tell the difference. Type IDs are
   process-wide and stable (the managed side round-trips them across unrelated calls);
   IDs 0–15 are reserved for the primitives `UIXVariant` carries directly.
6. **UIXList** (17 exports) — real store, and it keeps the *virtualization* distinction
   real (per-slot availability, `FetchSlowData`, `WantSlowDataRequests`) rather than
   pretending every item is resident, because the managed ListBox's scrolling depends on
   it. **The `ListChanged` type codes are not guessed**: `DllProxyList.ListChanged`
   (`UIX/Microsoft/Iris/CodeModel/Cpp/DllProxyList.cs`) casts the argument straight to
   `Microsoft.Iris.Data.UIListContentsChangeType`, so the numbering was read off that
   decompiled enum.
7. **Assets** — `SpBitmapLoadFile`/`LoadBuffer` real cross-platform decode via
   StbImageSharp; `SpBitmapLoadRaw` real (pixel copy, no decoder involved);
   `SpSoundLoadBuffer` real (RIFF/WAVE chunk parser — no codec needed since the managed
   side's only declared format is `WAVE_FORMAT_PCM`). Non-obvious detail: stb decodes to
   RGBA byte order but Iris's `SurfaceFormat.ARGB32` is `0xAARRGGBB` as a little-endian
   uint, i.e. BGRA in memory — the channel swap in `BitmapStore` is required, not
   incidental. `SpBitmapLoadResource` is `E_NOTIMPL` + TODO: Win32 resource sections have
   no cross-platform equivalent and this project has no PE resource reader yet.

**Remaining for this session:** data-binding (`SpData*`), XmlLite, GDI+ init pair,
misc OS (DPI/download/DLL+resource loading/notify window/IME/dropped files/registry
notify/HTTP), native services callbacks + string/image handle marshaling, and rich
text/simple text.

---

## 2026-07-22 — Dependency policy: "Silk.NET abstractions, not specific graphics APIs"

**Trigger:** mid-implementation the user narrowed the earlier "prefer Silk.NET over stubs"
direction to: *use Silk.NET **abstractions** wherever possible, avoid **specific graphics
APIs***. That is a materially different instruction from "bind Silk.NET's D3D packages",
and it changes three decisions already in flight. Recording the reasoning before acting on
it, since two of the three outcomes are "no dependency added", which would otherwise look
like the instruction was ignored.

**Verified first, not assumed:** NuGet is reachable from this machine and
`Silk.NET.Core 2.23.0` restores cleanly (probe project in the scratchpad, not committed).
So "couldn't get the package" is *not* the reason for any decision below — each is a
deliberate fit judgement.

**Decision 1 — `SpDx9CompileEffect`: no `Silk.NET.Direct3D.Compilers`, returns
`E_NOTIMPL` with a `// TODO`.** `Silk.NET.Direct3D.Compilers` is a thin binding over
`d3dcompiler_47.dll` — i.e. exactly "a specific graphics API", Windows-only, and the
narrowest possible thing to hard-code into a project whose stated goal is cross-platform
revival. Silk.NET has **no** cross-platform shader/effect-compilation abstraction to use
instead (checked: `Silk.NET.Core`/`Maths`/`Windowing`/`Input` are the abstraction layer,
and none covers HLSL effect compilation; the D3D/OpenGL/Vulkan packages are all
API-specific bindings). Since there's no abstraction available and the instruction rules
out the specific API, the honest outcome is an explicit `E_NOTIMPL` + `TODO` rather than
a Windows-only binding smuggled in behind `#if WINDOWS`. Logged as an open question
below, **not** silently dropped.

**Decision 2 — no `Silk.NET.Windowing`/`Silk.NET.Input` for `SpCreateNotifyWindow`,
`SpGetMouseCursorInfo`, `SpGetDpi`.** These *look* like the right fit (they're genuine
cross-platform abstractions over exactly these concepts), and I nearly took them. They're
wrong here for a structural reason worth writing down: `Silk.NET.Windowing` owns a
window **and its event loop**, backed by a native GLFW/SDL binary loaded at runtime.
`UIXrender.dll` is a library P/Invoked *into an already-running host process* that already
owns its window and pump (Zune's own shell). A library that spins up a second windowing
backend to answer "what's the system DPI" would be both heavier and less correct than
asking the OS. So these stay platform-gated OS calls (`#if WINDOWS` + `// TODO` for other
platforms), which is what CLAUDE.md's platform rule prescribes anyway. Note these are
**OS** APIs (DPI, registry-change notification, IME), not graphics APIs — the user's
constraint doesn't bite here.

**Decision 3 — image decode uses `StbImageSharp`, not `System.Drawing.Common`.** This one
*does* add a dependency, and it's the case where the instruction changed the answer for
the better. The earlier plan (entry below) had `SpBitmapLoadFile`/`LoadBuffer`/
`LoadResource` decoding via `System.Drawing.Common` under `#if WINDOWS` with
cross-platform decode deferred — i.e. GDI+, a specific graphics API, Windows-only since
.NET 6, and a stub everywhere else. `StbImageSharp` is pure managed (no native
dependency), cross-platform, NativeAOT-safe (matters: this project is
`PublishAot`/`NativeLib=Shared`), and is the decoder Silk.NET's own tutorials pair with
its APIs. Net effect: PNG/JPEG/BMP/TGA decode becomes **real logic on every platform**
instead of Windows-only-plus-a-stub. Strictly better against both the project's
cross-platform goal and CLAUDE.md's "real logic preferred over stubs".

**Unchanged by this:** everything non-graphical (memory, tracing, transport, remote
channel, reflection/schema, UIXList, data-binding, XmlLite, sound) was never going to
touch a graphics API and keeps the real implementations described in the plan entry
below.

---

## 2026-07-22 — Plan: complete the entire native export surface this session

**Task, per explicit user direction:** implement stages 1 & 2 for the *entire* remaining
`UIXrender.dll` public surface (not just the `EngineApi.cs` transport methods worked on
so far), aiming for a complete library by the end of this session, the same bar applied
to `ZuneDBApi`. Real implementations are preferred over stubs (matching CLAUDE.md's stage
2 convention), with Silk.NET preferred over hand-rolled P/Invoke where a fitting binding
package exists (mainly Direct3D9 compile-effect). Existing `UIXsup`/`UIXrender` code may
be rewritten as needed as long as the public surface/signatures stay unchanged. Logging
before/while writing, not after, per this repo's established convention.

**Scope, counted from a full grep of every `[DllImport("UIXRender.dll")]` across the four
managed consumer files** (`UIX/Microsoft/Iris/OS/NativeApi.cs`,
`UIX.RenderApi/Microsoft/Iris/Render/Extensions/ExtensionsApi.cs`,
`UIX.RenderApi/Microsoft/Iris/Render/Internal/FormApi.cs`,
`UIX.RenderApi/Microsoft/Iris/Render/Protocol/EngineApi.cs`): every export that has a
concrete managed-side signature in this repo gets a matching `[UnmanagedCallersOnly]`
counterpart in `UIXrender`, organized into `UIXrender/Subsystems/<Group>/` folders
mirroring `Architecture.md`'s subsystem grouping. Exports flagged in `Architecture.md` as
"export-only" (no managed caller found in this repo, e.g. `SpAttachWndProc`,
`SpCreateObject`, `SpGetMessageA/W`) are **not** implemented here — there is no known
signature to mirror, and CLAUDE.md forbids guessing; logged as still-open, not silently
dropped.

**Subsystem plan and treatment (real logic vs. documented stub), decided before writing
any code:**

1. **Memory** (`SpMemAlloc`/`SpMemFree`/`SpFreeDib`) — real, via `NativeMemory`.
2. **Tracing** (`SpUpdateTraceSettings`/`SpLogTrace`, extending the existing
   `SpInitializeTracing`/`SpUninitializeTracing`) — real, `Console`/optional file sink.
3. **Engine transport remainder** (`SpPeekMessage`, `SpWaitMessage`, `SpInvoke`, the 4
   `SpRemote*` remote-channel entry points, `SpDx9CompileEffect`, `SpObjectRelease`) —
   real where the existing managed-direct `EngineService`/`ContextRegistry` design
   extends naturally (peek/wait/invoke against the per-context registry); `SpRemote*`
   implemented for real using `System.Net.Sockets`/named-pipe-equivalent streams since
   .NET's own stream types are a genuine, working transport, not a stub.
4. **Native reflection / type-schema system** (~50 `SpQuery*`/`SpGet*`/`SpInvoke*`
   exports) — real, backed by a new managed `SchemaRegistry` that projects real
   `System.Reflection` (`Type`/`PropertyInfo`/`MethodInfo`/`ConstructorInfo`/`EventInfo`)
   over registered CLR types through `GCHandle`-wrapped opaque `IntPtr` handles (the same
   opaque-handle convention already established for `RenderThread`). This is a genuine
   design substitution (real C++ types no longer exist to reflect over), not a stub —
   flagged here as a deliberate architecture decision, not an assumption to second-guess
   later.
5. **UIXList** (`SpUIXList*`, ~15 exports) — real, a `List<UIXVariant>`-backed store.
6. **Data binding / query** (`SpData*`, ~15 exports) — real, a managed registry mirroring
   the query lifecycle (construct/refresh/get-set property/notify).
7. **XmlLite** (`SpXmlLite*`, ~11 exports) — real, backed by `System.Xml.XmlReader`
   (cross-platform, not Windows-only XmlLite COM, but drop-in behaviorally).
8. **Asset loading** (`SpBitmap*`/`SpSound*`) — real where feasible: raw/buffer bitmap
   paths use direct pixel copy (no decode needed); file/resource decode paths use
   `System.Drawing.Common` on Windows (`#if WINDOWS`) since that's an in-box decoder,
   cross-platform decode deferred (stub + TODO, logged as an open question — no
   cross-platform image codec is referenced anywhere else in this repo yet). Sound
   buffer load is real (WAV header parse, no codec needed for PCM).
9. **Graphics** (`SpGdiplusInit`/`Uninit`, `SpGetStateCache`/`SpSetStateCache`,
   `SpDx9CompileEffect`) — GDI+ init/uninit are real (documented as legitimate no-ops:
   managed GDI+ has no manual startup step). `SpDx9CompileEffect` real via
   `Silk.NET.Direct3D.Compilers` (D3DCompiler), Windows-only (`#if WINDOWS`), since D3D9
   effect compilation is inherently a Windows/Direct3D concept — cross-platform has no
   equivalent, not guessed at.
10. **Misc OS/Win32** (`SpGetDpi`, `SpGetMouseCursorInfo`, `SpExtractDroppedFileNames`,
    `SpRegNotifyChangeKey`/`Revoke`, `SpCreateNotifyWindow`/`Destroy`,
    `SpRegisterImeCallbacks`/`Unregister`, `SpPostDeferredImeMessage`) — real on Windows
    (`#if WINDOWS`, direct Win32 P/Invoke, consistent with CLAUDE.md's platform-gating
    rule), stub + `// TODO` elsewhere since these are inherently Win32-shaped concepts.
11. **Rich text / Simple text** (~30 exports) — real editing model (content buffer,
    cut/copy/paste/delete/undo/redo, selection, timer/scroll/wrap/readonly/scale state)
    backed by an actual `StringBuilder` + undo stack; measurement real via
    `System.Drawing.Graphics.MeasureString` on Windows (`#if WINDOWS`), a documented
    fixed-width-estimate fallback elsewhere (logged assumption, not silently guessed);
    rasterization (`SpRichTextRasterize`) real ARGB bitmap production on Windows via
    `System.Drawing`, stub + TODO cross-platform (no font-shaping library referenced
    elsewhere in this repo to build on without adding a large new, unrequested
    dependency — flagged as an open question rather than guessed).

**Interface-typed callback parameters** (`IUIXListCallbacks`, `IRawUIXServices`,
`IRichTextCallbacks`, `IImeCallbacks`) can't be received as `[MarshalAs(UnmanagedType.
Interface)]` COM parameters in an `[UnmanagedCallersOnly]` method (not blittable) — native
callers pass a raw vtable pointer regardless of what the managed P/Invoke declares, so
`UIXrender`'s side receives `void*`/`IntPtr` and invokes through the vtable directly at
each interface's declared method slots (offset by the standard `IUnknown` 3:
QueryInterface/AddRef/Release), the same "reconstruct the interface, don't invent raw
pointer types" approach CLAUDE.md's *COM objects* section describes.

**Dependent-type mirrors needed** (independent `UIXrender/Interop/*.cs` copies, since
`UIXrender` cannot reference `UIX`/`UIX.RenderApi` — circular dependency, same reasoning
as the existing `ContextID`/`RENDERHANDLE`/`HRESULT`/`BufferInfo`/`InitArgs` mirrors):
`UIXVariant`, `Color`/`Size`/`Rectangle`/`RectangleF`/`ColorF`, `RawImageFormat`,
`SurfaceFormat`, `ImageRequirements`/`ImageInformation`/`ImageHeader`/`ImageData`/
`HSpBitmap`, `TextStyle.MarshalledData`, `TextMeasureParams.MarshalledData` (+
`FormattedRange`), `DataProviderQueryStatus`, `ShutdownReason`, `TransportProtocol`,
`NativeXmlNodeType`, `Win32Api.HANDLE`/`MSG`/`LOGFONTW_STRUCT` (the `Microsoft.Iris.OS`
variant, since that's what `NativeApi.cs` itself resolves to for its own P/Invokes).
Full field layouts confirmed via an Explore pass over the real decompiled sources before
writing any of these — not guessed.

**Not attempted, logged as genuinely out of scope rather than silently skipped:**
`UIXsup.dll`'s own surface beyond what's already there (separate DLL, separate stage-1
target — `logs/UIXsup.md` presumably covers it, not duplicated here); the ~26
"export-only" natives with no managed call site in this repo (no signature to mirror).

Proceeding subsystem-by-subsystem in the order above; each subsystem's own progress gets
logged as it lands, same as `EngineCore.md`'s existing entries.
