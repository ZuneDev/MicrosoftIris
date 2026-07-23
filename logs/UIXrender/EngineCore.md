# UIXrender.dll engine core — implementation log

Append-only. Do not edit previous entries.

---

## 2026-07-22 — Wired the *remaining* EngineApi.cs methods managed-direct (all 9)

**Task:** the first managed-direct pass (entry below) converted only the 6 transport
methods `LocalChannel` uses. Now that the full surface is implemented, wire the other 9
`UIX.RenderApi/.../Protocol/EngineApi.cs` methods — `SpPeekMessage`, `SpWaitMessage`,
`SpInvoke`, the four `SpRemote*`, `SpDx9CompileEffect`, `SpObjectRelease` — to call
UIXrender's managed implementation instead of `[DllImport]`, keeping the net461 `#else`
branch on the native path exactly as the existing 6 do.

**Single source of truth, established deliberately.** Rather than duplicate logic between
UIX.RenderApi and UIXrender's `[UnmanagedCallersOnly]` shims, I made `EngineService` (the
public facade both sides already share) the one implementation and routed *both* the
native shims (`UIXrender/Interop/EngineApi.cs`) and UIX.RenderApi through it. So each
operation now has exactly one body: `EngineService.{PeekMessage,WaitMessage,Invoke,
RemoteCreateServerStreams,RemoteWaitServerStreamsConnected,RemoteServerInit,
RemoteServerUninit,ReleaseRemoteStream,Dx9CompileEffect}`. `EngineService`'s doc comment
was updated: it's no longer strictly pointer-free (opaque `IntPtr` handles pass through,
and `Invoke` has the one `unsafe` spot to call the function pointer a caller hands it),
and that's called out explicitly.

**Two things that would have been silent bugs, verified against the real call sites rather
than assumed:**

1. **`SpInvoke`'s only in-repo caller is `IRenderEngine.InterThreadWake`, which passes a
   *null* function pointer** (`SpInvoke(ctx, IntPtr.Zero, IntPtr.Zero, false)`) purely to
   wake a pump. The earlier native shim returned `E_INVALIDARG` for a null pointer — which
   would have been wrong for this caller. `EngineService.Invoke` now treats a null pointer
   as a well-defined `S_OK` no-op (there is no blocking pump to wake in this
   implementation), and both entry paths use that.

2. **The remote path had a handle-model mismatch that would have corrupted memory.**
   `RemoteChannel.Connect` calls `SpObjectRelease` on the two stream handles from
   `SpRemoteCreateServerStreams`. In managed-direct mode those handles are UIXrender
   `GCHandle`s, **not** COM vtable pointers — so the native shim's generic "release through
   the vtable slot 2" `SpObjectRelease` would have dereferenced garbage. Fixed by:
   - `RemoteServerConnection` now hands out **two distinct** `GCHandle`s (it previously
     aliased send == receive) with a **refcount of 2**; `ServerInit` adds a third (the
     session). Each release drops one reference and frees *its own* handle; the sockets
     are torn down exactly once, when the last reference goes. This matches
     `RemoteChannel.Connect`'s exact sequence (create → init → release send → release
     receive → later uninit session).
   - Managed-direct `SpObjectRelease` routes to `EngineService.ReleaseRemoteStream`
     (drop-a-reference), **not** the vtable path. The native shim keeps the vtable-release
     behaviour for genuine native COM callers — the two object models are distinct, like
     `SpWrapBufferProc` already is. Documented at both sites.

**Callback representation, handled consistently.** `RemoteServerConnection.ServerInit` was
refactored to take a pointer-free `BufferReceivedHandler` (like `RenderThread`), so its
`ReadLoop` no longer assumes a native function pointer. The native shim adapts the raw
`InitArgs.pfnProcessBuffer` function pointer into one; UIX.RenderApi's managed-direct path
resolves the `GCHandle`'d `MessageBufferEventHandler` and reuses the existing
`AdaptCallback` helper. In practice `RemoteChannel` connects with **no** receive callback
(it builds `InitArgs` with the 2-arg ctor → `pfnProcessBuffer == 0`), so the handler is
normally null — but the adaptation is correct for the non-null case too, rather than
mis-reading a `GCHandle` as a function pointer.

**`SpDx9CompileEffect`** routes to `EngineService.Dx9CompileEffect()` (`E_NOTIMPL`, per
FullSurface.md decision 1); the UIX.RenderApi side zeroes its out-params so a caller
ignoring the HRESULT still sees a well-defined "no blob". Note its caller
(`Dx9EffectResource`) wraps it in `IFC`, which throws on failure — same observable outcome
as before on net8.0, where there was no working native `SpDx9CompileEffect` to call
anyway; now it fails deterministically instead of at DLL-resolution time.

**Verified:** `UIX.RenderApi` (net8.0) builds clean (only pre-existing decompiled-code
warnings); whole solution builds except the same pre-existing `SimpleDebugClient`/
`SimpleIrisApp` errors; NativeAOT publish still exports **exactly 193/193** (the wiring
changed no signatures); `Tests/UIXrender.Engine.Tests` extended to **53 checks, all
passing**, including new ones that exercise the managed-direct remote path
(`RemoteCreateServerStreams` VC→`E_NOTIMPL`, TCP→two distinct handles, and release of both
tearing the connection down once without a double-free).

---

## 2026-07-22 — Managed-direct calling: EngineService layer + first modification of UIX.RenderApi

**Task:** per user direction, restructure so `UIX.RenderApi` can call directly into
`UIXrender`'s managed implementation (no P/Invoke, no native marshaling) whenever both
are loaded in the same .NET process, while `Interop/EngineApi.cs`'s
`[UnmanagedCallersOnly]` exports remain for genuine native callers. Logging before/while
writing code, not after.

**Key fact driving the design:** `[UnmanagedCallersOnly]`-attributed methods cannot be
called directly from C# at all (compiler error CS8901) -- they can only be reached via a
function pointer, mimicking how native code calls them. So "call directly" necessarily
means a *separate* idiomatic layer beneath the shim, not calling the shim methods
themselves with fewer steps.

**Design:** split `UIXrender/Engine/` into a public, pointer-free API
(`EngineService`/`IRenderThreadHandle`/`BufferReceivedHandler`, using
`ReadOnlySpan<byte>` and ordinary C# delegates) that `ContextRegistry`/`RenderThread`
are refactored to use internally -- no `IntPtr`/raw function pointers anywhere in
`Engine/` after this change. All pointer/unsafe marshaling concentrates in
`Interop/EngineApi.cs` (adapting native `BufferInfo*`/`void*`/raw function pointers to
and from the idiomatic API), which is exactly the "backwards-compatible entrypoint for
native callers" the user asked to keep.

**Scope of the `UIX.RenderApi` change, decided deliberately, not implicitly:** only the
6 `EngineApi.cs` methods `UIXrender` actually implements today get their `[DllImport]`
bodies replaced with real calls into `EngineService`: `SpInit`, `SpUninit`,
`SpWrapBufferProc`, `SpRenderThreadInit`, `SpRenderThreadUninit`, `SpBufferOpen`. These
happen to be the *entire* set `RenderPort.cs`/`LocalChannel.cs` (the real send/receive
path) use -- confirmed by re-checking call sites, not assumed. Everything else in
`EngineApi.cs` (`SpPeekMessage`, `SpWaitMessage`, `SpInvoke`, the `SpRemote*` family,
`SpDx9CompileEffect`, `SpObjectRelease`) stays untouched `[DllImport]` since `UIXrender`
doesn't implement those yet -- converting them now would break, not improve, anything
that still needs them. **This is the first modification of a previously-decompiled,
until-now-untouched file in this whole effort** (`UIX.RenderApi/Microsoft/Iris/Render/Protocol/EngineApi.cs`),
worth flagging explicitly: only method *bodies* change, not signatures or any other
file's call sites, and it's an internal-only class (nothing outside `UIX.RenderApi.dll`
depends on how it's implemented), so this doesn't touch the drop-in-replacement contract
CLAUDE.md cares about for this project's actual public surface.

**A real, additional simplification found along the way, not just pointer-hiding:**
`SpWrapBufferProc`'s original job was handing a native caller a real function pointer to
invoke later. In the managed-direct path, the delegate the caller passes
(`MessageBufferEventHandler`) doesn't need to become a function pointer at all -- it can
be stored via a `GCHandle` and resolved straight back to the original delegate object,
invoked as a normal C# delegate call. No `delegate* unmanaged<...>`/`calli` anywhere on
this path. A `BufferInfo*`/`void*` pointer still gets reconstructed at the point of
calling the stored `MessageBufferEventHandler` delegate, because that delegate's own
signature (already-decompiled, unchanged) is pointer-shaped -- not something avoidable
without touching `RenderPort.cs`, which is out of scope here.

**Implementation, extending this entry as each piece lands:**

1. `UIXrender/Engine/` refactored to be entirely pointer-free: `BufferReceivedHandler.cs`
   (custom delegate, not `Action<>`, specifically so `ReadOnlySpan<byte>` is legal as a
   parameter -- `Span<T>`/`ReadOnlySpan<T>` are ref structs and can't be generic type
   arguments, but a hand-declared delegate can take one directly), `IRenderThreadHandle.cs`,
   `ContextRegistry.cs` (now `ConcurrentDictionary<uint, BufferReceivedHandler>`, no more
   `IntPtr`), `RenderThread.cs` (`IDisposable`/`IRenderThreadHandle`, holds the delegate
   directly, no function pointer casts), and the new public `EngineService.cs` facade
   (`StartRenderThread`, `SendBuffer`) -- the one type meant to be called by anything
   that wants managed-direct access.
2. `UIXrender/Interop/EngineApi.cs` rewritten to route through `EngineService` --
   `SpRenderThreadInit` builds a `BufferReceivedHandler` closure that reconstructs a
   `BufferInfo*`/`void*` call only at the point of invoking the raw native function
   pointer it received; `SpBufferOpen` converts its `void*`/`cbSizeBuffer` into a
   `ReadOnlySpan<byte>` and calls `EngineService.SendBuffer`. Verified this still builds
   clean on Linux.
3. `UIX.RenderApi/Microsoft/Iris/Render/Protocol/EngineApi.cs` -- the 6 in-scope
   methods' `[DllImport]` bodies replaced with real implementations calling
   `EngineService` directly, converting between `UIX.RenderApi`'s own (unchanged,
   pre-existing) `Protocol.ContextID`/`RENDERHANDLE`/`Internal.HRESULT`/nested
   `EngineApi.BufferInfo` types and `UIXrender`'s `Interop.*` equivalents at the
   boundary via the `ToUInt32`/constructor accessors those types already expose -- kept
   as two separate struct sets rather than unifying them, since `UIXrender` must stay
   independent of `UIX.RenderApi` (it's also P/Invoked directly by `UIX.dll`'s
   `NativeApi.cs`, a different assembly entirely) and a reference in the other direction
   would be circular. **`net461` handled explicitly, not overlooked**: `UIX.RenderApi.csproj`
   has `EnableNetFXTarget=true` (still targets .NET Framework 4.6.1 on Windows), which
   can't reference a net8.0-only project at all -- the new `ProjectReference` to
   `UIXrender.csproj` is conditioned out for `net461`
   (`Condition="'$(TargetFramework)' != 'net461'"` in the csproj), and all 6 modified
   methods are wrapped `#if !NETFRAMEWORK` with the original, unmodified `[DllImport]`
   declarations preserved in the `#else` branch -- net461 keeps calling the real native
   `UIXRender.dll` exactly as before, nothing regresses for that target.
4. `Tests/UIXrender.Engine.Tests` (new) -- exercises `EngineService` directly via a
   `ProjectReference` to `UIXrender.csproj`, no P/Invoke or published DLL involved. Since
   `Engine/` is now fully pointer-free, this is the first thing in this whole effort that
   could actually be **run and observed**, not just compiled, from this (Linux) machine.
   Ran it: `StartRenderThread`'s on-start synthetic invocation fires, `SendBuffer`
   correctly routes to the registered handler with the right source context/buffer
   handle/payload bytes, and `SendBuffer` to a disposed/unregistered context fails
   cleanly (`E_FAIL`) rather than hanging or throwing. All 8 checks passed. Wired into
   CI (runs in the same `windows-latest` job today for simplicity, but noted in the
   workflow as cross-platform-capable if a Linux job gets added later).

**Verified on Linux:** whole solution builds clean except the same pre-existing,
unrelated `SimpleDebugClient`/`SimpleIrisApp` errors; `Tests/UIXrender.Engine.Tests`
actually **runs** and all 8 checks pass. **Not verified here, needs Windows CI:** the
`#else`/net461 branches in `EngineApi.cs` (Directory.Build.props only adds `net461` when
`IsOsPlatform('Windows')`, so this machine never compiles that branch at all -- a real
gap, flagged rather than assumed fine); the Phase 0 native-ABI spike from the entry
below, still pending its first CI run.

**Follow-up cleanup, prompted by the user asking why `SpRenderThreadInit` had grown so
much logic:** worth recording the answer, not just the fix. Three sources, sorted by
whether they're removable:

- **Not removable** -- inherent to preserving the original API exactly: (1)
  `SpWrapBufferProc`/`SpRenderThreadInit` are two separate calls in the original API,
  and the only channel between them matching that shape is `InitArgs.pfnProcessBuffer`
  as a plain `IntPtr`, which forces the GCHandle-store/GCHandle-resolve round trip
  rather than passing the delegate straight through; (2) `MessageBufferEventHandler`'s
  signature is still pointer-shaped (`BufferInfo* pBufferInfo, void* pvBufferData`),
  unchanged since `RenderPort.cs` still expects to receive calls that way, so
  *something* has to reconstruct a pointer-based call no matter how clean
  `EngineService`'s own API is; (3) `UIX.RenderApi`'s own `ContextID`/`RENDERHANDLE`/
  `BufferInfo` and `UIXrender`'s `Interop.*` equivalents are two genuinely separate type
  systems (by design, to avoid the circular-reference problem noted earlier), so
  crossing that boundary needs explicit conversion.
- **Removable, and fixed**: the `BufferInfo`-reconstruction closure was inlined directly
  inside `SpRenderThreadInit`, making the method read as one undifferentiated block
  instead of clear steps -- and it's structurally the *same* pattern (build a
  `BufferInfo`, get a pointer to the span, invoke) as `UIXrender/Interop/EngineApi.cs`'s
  own `SpRenderThreadInit` uses for native callers, just invoking a stored C# delegate
  at the end instead of a raw function pointer. Extracted into a new private
  `AdaptCallback(MessageBufferEventHandler, ContextID) : BufferReceivedHandler` helper,
  so `SpRenderThreadInit` itself now reads as four named steps (resolve the stored
  delegate → adapt it → start the thread → wrap the handle) instead of one block with an
  inline lambda in the middle. Same behavior, verified: `UIX.RenderApi.csproj` still
  builds clean and `Tests/UIXrender.Engine.Tests` still passes all 8 checks after the
  change.

## 2026-07-22 — Phase 0 spike: SpBufferOpen/SpWrapBufferProc/SpRenderThreadInit

**Task:** per the approved plan, implement `UIXrender.dll`'s core transport primitives
well enough to prove the riskiest interop mechanism works: a native OS thread the CLR
didn't create calling back into a managed delegate. Logging as I go, per the user's
explicit reminder not to wait until done.

**Structural decision, before writing code:** creating a new shared project,
`UIXInterop` (`Microsoft.Iris.Interop` namespace), for native-marshaling helpers used by
both `UIXsup` and `UIXrender` (currently just ANSI/Unicode `byte*`/`char*` ↔ `string`
conversion — `UIXsup/Interop/DebugApi.cs` already has a private `PtrToString` that would
otherwise get copy-pasted here). Per the user's explicit go-ahead to split into
additional projects to cut duplication. `UIXsup` will be refactored to use it too, so
there's a single implementation instead of two copies drifting apart. This is narrower
than fully splitting `UIXrender` into per-subsystem projects (Engine, Graphics, ...) —
that's still a folder-level split inside `UIXrender/`, per the plan's project-structure
section; only genuinely cross-project shared code moves to its own assembly.

**Scoping `SpInit`/`SpUninit` out of this spike, logged rather than guessed:**
`EngineApi.SpUninit()` takes **no parameters at all** — no context id, nothing — which
is hard to reconcile with `SpInit(ref InitArgs args)` registering a specific
`args.idContext`. Checked whether `LocalChannel.Connect()` (the code path real Zune
actually uses) calls `SpInit`/`SpUninit` at all: it doesn't — it only calls
`SpRenderThreadInit`/`SpRenderThreadUninit` (`UIX.RenderApi/Microsoft/Iris/Render/Protocol/LocalChannel.cs`).
`SpInit`/`SpUninit` must serve some other call path not yet located (possibly the
alternate "IGMM_STANDARD messaging model" mentioned in one of `EngineApi.IFC`'s HRESULT
error strings, implying a selectable threading model distinct from the dedicated-thread
one `SpRenderThreadInit` provides) — no confident answer, so no guess: implementing both
as plain `HRESULT.S_OK`-returning stubs for now (the explicitly-allowed stub convention),
real logic deferred until the actual caller/semantics are found. Not blocking this
session's work since the spike's own critical path (`LocalChannel`'s path) never calls
either.

**Design note, not an unknown — a legitimate simplification:** `SpWrapBufferProc`'s
purpose in the original was presumably to hand the CLR-marshaled delegate pointer to
native code in a form the original C++ implementation's internal calling convention
needed (possibly a real trampoline/thunk). Since this reimplementation *is* the native
side now, and controls both ends of the call, there's nothing to adapt — the pointer the
CLR hands us when marshaling `MessageBufferEventHandler` is already directly callable
from a `delegate* unmanaged<...>` field. `SpWrapBufferProc` becomes a real (not fake)
but simple implementation: validate, store/echo the pointer, return `S_OK`. Recording
this so a future reader doesn't wonder why it's "too simple" compared to what the
original probably did internally — the simplification is deliberate, not a missed spot.

**Implementation complete for this phase:**
1. `UIXInterop` project (`UIXInterop/NativeString.cs`) — `AnsiToString`/`UniToString`
   pointer conversion, shared by `UIXsup` (refactored to use it, removing its local
   copy) and `UIXrender`.
2. `UIXrender/Interop/` wire structs — `HRESULT.cs`, `ContextID.cs`, `RENDERHANDLE.cs`,
   `BufferInfo.cs` (+ `BufferFlags`), `InitArgs.cs` — bit-for-bit mirrors of the
   decompiled `UIX.RenderApi/Microsoft/Iris/Render/{Internal,Protocol}/*.cs` structs
   read in the first architecture session. `InitArgs.pfnTimeout` is `IntPtr` here, not a
   delegate type (see file comment: the CLR marshals the managed side's delegate field
   to a plain function pointer before the struct crosses over, so that's the actual wire
   representation).
3. `UIXrender/Engine/ContextRegistry.cs` — `ContextID -> (callback pointer, callback
   data)` registry, real logic (`ConcurrentDictionary`), the smallest coherent slice of
   "Engine core" (the plan's subsystem 1) needed for `SpRenderThreadInit`/`SpBufferOpen`
   to mean anything together. Callback stored as raw `IntPtr`, cast to a
   `delegate* unmanaged<...>` only at the actual invocation site, to avoid putting a
   function-pointer-typed field into a generic collection's value type.
4. `UIXrender/Engine/RenderThread.cs` — real (not mocked) thread lifecycle: `Start`
   spins up a genuine `System.Threading.Thread`, which invokes the registered callback
   once with synthetic `BufferInfo` data then blocks on a `ManualResetEventSlim` until
   `Stop` signals it, joins, and unregisters. Not the full message-pump/dispatch loop
   (that's later, larger "Engine core" work) — this proves the interop mechanism itself.
5. `UIXrender/Subsystems/Tracing/` — `TracingState.cs` (minimal real init-flag state)
   and `TracingApi.cs` (`SpInitializeTracing`/`SpUninitializeTracing` exports). Full
   tracing (`SpUpdateTraceSettings`/`SpLogTrace`) deferred to when the Tracing subsystem
   gets its own pass per the plan's sequencing.
6. `UIXrender/Interop/EngineApi.cs` — the remaining `[UnmanagedCallersOnly]` exports:
   `SpInit`/`SpUninit` (S_OK stubs, per the open question above), `SpWrapBufferProc`
   (pass-through, per the design note above), `SpRenderThreadInit`/`SpRenderThreadUninit`
   (wrap `RenderThread` behind a `GCHandle`-backed opaque `IntPtr` handle, matching what
   the managed side's `out IntPtr pThread` expects to receive and later pass back),
   `SpBufferOpen` (looks up the destination context in `ContextRegistry`, invokes its
   callback synchronously).
7. `Tests/UIXrender.Interop.Tests` — the Phase 0 exit-criteria harness: a plain console
   app (no test framework dependency added) with its own P/Invoke declarations (can't
   reference `EngineApi.cs` directly — it's `internal` in `UIX.RenderApi.dll`, and this
   harness is meant to exercise the published `UIXrender.dll` the way an arbitrary
   external caller would anyway). Registers a callback via `SpWrapBufferProc`, starts a
   thread via `SpRenderThreadInit`, waits up to 5s for the callback to fire with the
   expected context id, then tears down via `SpRenderThreadUninit`. Explicit pass/fail
   per check, non-zero exit code on any failure, wired into
   `.github/workflows/uixrender-ci.yml` after the publish steps (locates the published
   DLL and the harness's build output via `Get-ChildItem`, copies the DLL alongside the
   harness exe, runs it, fails the job on a non-zero exit code).

**Verified on Linux:** the whole solution builds clean except the same pre-existing,
unrelated `Tests/SimpleDebugClient`/`SimpleIrisApp` errors noted in `UIXsup.md` (not
touched by this work). **Not yet verified:** actual execution — the interop spike can
only run on Windows (P/Invoking a real NativeAOT-published `UIXrender.dll`), which
requires the CI workflow to actually run, not checked yet as of this entry.
