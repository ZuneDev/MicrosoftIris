# `Microsoft.Iris.Markup` compiler — decompilation/implementation log

Append-only. Do not edit previous entries.

---

## 2026-08-01 — `MarkupCompiler.SaveCompiledOutput` had no non-Windows path

Same class of gap as `FileResource` (see `logs/UIX/Resources.md`, same day): this method
wrote the compiled `.uib` bytecode buffer to disk using `Win32Api.CreateFile`/`WriteFile`/
`CloseHandle` unconditionally, no `#if WINDOWS` gate. Writing a file has no genuine
Windows-only dependency, so — per `ZuneDBApi/CLAUDE.md`'s stage-2 preference for real logic
over stubs — wrote a real non-Windows body instead of a stub, using `System.IO.File.Create`
+ `FileStream.Write`.

### Preserving the original's error-flow quirks

The original has a slightly odd control flow worth calling out, since the non-Windows branch
mirrors it deliberately rather than "fixing" it:
- It always attempts `CreateFile`/`File.Create`, even if `ErrorManager.Watermark` already has
  errors recorded from earlier compilation stages (i.e. it still tries to open/truncate the
  output file even when it knows it won't write anything meaningful to it).
- The actual write only happens `if (!watermark.ErrorsDetected)` — a snapshot of the
  watermark taken at the *top* of the method, before the open attempt, so a failed open does
  NOT set `ErrorsDetected` in time to skip the write attempt on the Windows side (`WriteFile`
  is called with `file == INVALID_HANDLE_VALUE` and simply fails again, reported as a second,
  redundant error). Kept this as-is on the non-Windows side too, gating on `file != null` in
  addition to `!watermark.ErrorsDetected` (equivalent effect, no redundant second error, but
  same "still opens the file even if we already know the compile failed" behavior) rather
  than restructuring the flow — not this task's scope, and changing decompiled-code
  semantics without a concrete reason is against the stage-1/2 rules in
  `ZuneDBApi/CLAUDE.md`.
- `reader?.Dispose(typeof(MarkupSystem))` still runs unconditionally at the end on both
  platforms, same as the original.

### Buffer transfer

`ByteCodeReader.ToIntPtr(out uint size)` still hands back a raw native pointer (unaffected by
the `NativeApi.MemAlloc`/`MemFree` fix from the `FileResource` work earlier today — that
fixed *allocation*, not this call site, which just reads out the existing pointer/size). Non-
Windows path copies it into a managed `byte[]` via `Marshal.Copy` before handing it to
`FileStream.Write` — no unsafe code needed, `MarkupCompiler` wasn't an `unsafe` class and
there was no reason to make it one for this.

### Verification

`dotnet build UIX/UIX.csproj -f net8.0` — clean, 0 errors, no new warnings (only pre-existing
warning at `MarkupCompiler.cs:29`, an unrelated unused field). Windows TFM not build-tested
(Linux sandbox); `#if WINDOWS` branch is untouched original code. Not runtime-tested — no
markup-compilation scenario currently exercised end-to-end on Linux in this repo; verified via
code-path reasoning against the original control flow only.
