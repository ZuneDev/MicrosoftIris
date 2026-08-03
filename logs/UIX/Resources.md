# `Microsoft.Iris.OS` resource-loading — decompilation/implementation log

Append-only. Do not edit previous entries.

---

## 2026-08-01 — `FileResource` had no non-Windows path; traced the gap back to `NativeApi.MemAlloc`/`MemFree`

Requested task: implement a non-Windows path for `Microsoft.Iris.OS.FileResource`
(`UIX/Microsoft/Iris/OS/FileResource.cs`), which was still 100% stage-1 decompiled code —
`SynchronousDownload` called `Win32Api.CreateFile`/`GetFileSize`/`ReadFile`/`CloseHandle`
unconditionally (real Win32 P/Invokes, no `#if WINDOWS` gate), and `AsynchronousDownload`/
`OnFileDownloadComplete`/`CancelAcquisition` all went through `NativeApi.SpFileDownload`/
`DownloadGetBuffer`/`SpDownloadClose` — a native async-download queue implemented in
`UIXRender.dll`, Windows-only.

### Why real logic, not a stub

Per `ZuneDBApi/CLAUDE.md`'s stage-2 rule ("real logic preferred over stubs"): reading a
local file has no genuine Windows dependency — `System.IO` covers it on every target this
repo builds (`net8.0`, `net8.0-windows10.0.22000`, `net48`). This is unlike `DllResource`'s
non-Windows path (`res://` Win32-embedded-resource loading), which really has no
cross-platform equivalent and legitimately returns an error string. Wrote a real
`ReadFileToNativeBuffer` helper (sync path calls it directly; async path runs it on a
`Task.Run` background thread) instead of stubbing.

### Threading for the async path

The original `SpFileDownload` callback presumably gets marshaled back onto the calling
thread by the native download queue (same as `HttpResource`/native pattern elsewhere).
Found the existing in-repo mechanism for exactly this — `Microsoft.Iris.Session.DeferredCall.Post(Thread, DispatchPriority, SimpleCallback)`,
which posts a `QueueItem` onto a specific thread's `Dispatcher` (see
`UIX/Microsoft/Iris/Session/DeferredCall.cs`, `UIX/Microsoft/Iris/Queues/Dispatcher.cs`).
Non-Windows `AsynchronousDownload` now captures `Thread.CurrentThread` at request time and
posts the completion (`NotifyAcquisitionComplete`) back onto it via `DeferredCall.Post(...,
DispatchPriority.Normal, ...)`, so `ResourceAcquisitionCompleteHandler` subscribers still
only ever get called on the thread that started the acquisition, same as before.
`CancelAcquisition` on non-Windows cancels a `CancellationTokenSource` instead of closing a
native download handle; the background read still finishes, but the result buffer is freed
and the completion is dropped instead of being posted.

### Root cause found one layer down: `NativeApi.MemAlloc`/`MemFree`

`Resource.AllocNativeBuffer` (`UIX/Microsoft/Iris/Data/Resource.cs`) — used by
`FileResource`'s new non-Windows buffer path — called `NativeApi.MemAlloc` unconditionally,
which was an unconditional `[DllImport("UIXRender.dll")] SpMemAlloc` P/Invoke with **no**
`#if WINDOWS` gate at all — would throw `DllNotFoundException` on Linux the moment any
`Resource` subclass tried to allocate a buffer, regardless of this task. `FreeNativeBuffer`
right next to it already had a `#if WINDOWS ... NativeApi.MemFree(buffer) ... #else ...
Marshal.FreeHGlobal(buffer) ... #endif` split — evidence a previous session hit this same
gap from the free side but patched around it locally instead of fixing `NativeApi.MemFree`
itself.

`NativeApi.MemFree` is also called directly (bypassing `Resource.FreeNativeBuffer`) from
`Markup/ByteCodeReader.cs:OnDispose` (frees `_buffer` when `_ownsAllocation`), matching an
allocation via `NativeApi.MemAlloc` in `Markup/ByteCodeWriter.cs:ComposeFinalBuffer`
(`byte*)NativeApi.MemAlloc(_totalSize, false).ToPointer()`) — both call sites had the exact
same unconditional-native-P/Invoke problem, unrelated to `FileResource` but sharing the same
root cause.

Fixed at the root instead of duplicating platform branches per call site: gated
`SpMemAlloc`/`SpMemFree` themselves behind `#if WINDOWS` in `NativeApi.cs`, and gave
`MemAlloc`/`MemFree` real non-Windows bodies using `Marshal.AllocHGlobal`/`FreeHGlobal`
(`zeroMemory` handled via a `Span<byte>.Clear()` — `Marshal.AllocHGlobal` doesn't zero).
This transparently fixes `Resource.AllocNativeBuffer`, `ByteCodeReader.OnDispose`, and
`ByteCodeWriter.ComposeFinalBuffer` for non-Windows too, with zero changes to those three
call sites. Simplified `Resource.FreeNativeBuffer` back down to
`NativeApi.MemFree(buffer)` (removed its local `#if` split) now that `NativeApi.MemFree`
handles both platforms itself — one point of truth instead of two.

### Verification

`dotnet build UIX/UIX.csproj -f net8.0` (the non-Windows TFM, only one buildable in this
Linux sandbox) — clean, 0 errors, no new warnings from `FileResource.cs`, `NativeApi.cs`,
or `Resource.cs`. Windows TFM (`net8.0-windows10.0.22000`) not build-tested here — the
`#if WINDOWS` branch is untouched original decompiled code, so no behavior change expected
there, but not verified by an actual compile. Not runtime-tested (no scenario in this repo
currently drives `FileResource` end-to-end on Linux) — verified via code-path reasoning
against the original `Resource`/`FileResource`/`HttpResource` contracts only.
