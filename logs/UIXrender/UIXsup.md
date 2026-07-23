# UIXsup.dll — implementation log

Append-only. Do not edit previous entries.

---

## 2026-07-22 — Phase 0 warm-up: project config + export catalog correction

**Task:** implement UIXsup.dll's real export surface as the lowest-risk NativeAOT
rehearsal before tackling UIXrender.dll's harder `SpBufferOpen`/`SpWrapBufferProc`
callback spike (per the approved plan, `/home/yoshiask/.claude/plans/dazzling-dreaming-waffle.md`).
Logging as I go per CLAUDE.md's *Dealing with unknowns and uncertainty* / *Logging*
sections, not after the fact.

**Correction to the plan's export count:** the plan (via a Plan sub-agent's earlier
research) listed 6 exports for `UIXsup.dll` including `DebugBreak`. Re-reading the
actual decompiled call sites (`UIX.RenderApi/Microsoft/Iris/Render/Extensions/eDebugApi.cs`,
`UIX/Microsoft/Iris/Debug/DebugHelpers.cs`) before implementing turned up only **5** real
`[DllImport("UIXsup.dll")]` exports:
- `DebugDisplayErrorStack(string, string, int, string, string) : bool`
- `DebugSetTimedWriteLines(bool)`
- `DebugSetWriteLinePrefix(string)`
- `DebugGetCategoryLevel(DebugCategory) : byte`
- `DebugSetCategoryLevel(DebugCategory, byte)`

`DebugHelpers.Break()` (which the agent's summary conflated with a `UIXsup.dll` export)
actually calls `Win32Api.DebugBreak()` — a *different* function, from `kernel32.dll`,
via `UIX/Microsoft/Iris/OS/Win32Api.cs`, not `UIXsup.dll` at all. Not implemented as
part of this project; it's the real Win32 API, already correctly targeted by the
existing managed code. This matches the original architecture-log entry from the first
research session in this effort, which had the correct 5-export list — the agent's later
research introduced the inaccuracy. Verified directly against source before writing any
code, per CLAUDE.md's requirement not to build on an unverified claim.

**Marshaling facts, read directly from source (not inferred):** none of the 5
`[DllImport("UIXsup.dll")]` declarations specify a `CharSet`. .NET's default
`CharSet` for `[DllImport]` when unspecified is `Ansi` — this is a fact about the
already-compiled, unchanged managed assemblies (baked into their P/Invoke metadata at
Microsoft's original compile time), not a guess, so every `string` parameter here
(`stMessage`, `filename`, `title`, `stackTrace`, `stPrefix`) is ANSI (`LPStr`), not
UTF-16. This matters because `[UnmanagedCallersOnly]` methods must use blittable
parameter types only — `string`/`bool` aren't blittable — so the native side takes
`byte*` (converted via `Marshal.PtrToStringAnsi`) and `int` (matching the default
4-byte Win32 `BOOL` marshaling of C# `bool`) instead.

**csproj/CI config, corrected after user feedback (see thread — recorded here for the
paper trail):**
1. First pass wrongly restricted `UIXrender.csproj`/`UIXsup.csproj` to
   `net8.0-windows10.0.22000` only, reasoning from the *original* binary's Windows-only
   surface. User corrected this: the whole point of the earlier Silk.NET decision is
   staying cross-platform, so platform-specific TFM locking contradicts that — reverted
   to plain `net8.0`, with the actual OS/arch chosen only at publish time via `-r <rid>`.
2. This surfaced a real, separate technical conflict: Directory.Build.props' default
   Windows-conditional TFM list included `net461`, which cannot use NativeAOT/
   `[UnmanagedCallersOnly]` at all (net7.0+ only). Flagged to the user rather than
   guessed at silently (two live options: override away from it locally, or fix the
   default). **User fixed it at the source** — `Directory.Build.props` now gates
   `net461` behind an opt-in `$(EnableNetFXTarget)` flag (`UIX`/`UIX.RenderApi` opt in,
   `UIXrender`/`UIXsup` don't), rather than each new project needing to route around it.
3. Found and fixed a genuine functional gap, not a style issue: `PublishAot=true` alone
   publishes a native **executable**, not an exporting shared library — NativeAOT needs
   `<NativeLib>Shared</NativeLib>` to actually emit a DLL with a real export table for
   `[UnmanagedCallersOnly]`-attributed methods. Added to both csproj. Without this, every
   `[UnmanagedCallersOnly]` method written for either project would silently fail to be
   callable via P/Invoke despite compiling cleanly — a bug in Phase 0's exit criteria
   that wouldn't have surfaced until CI's Windows publish step, so worth flagging clearly
   here even though it's a well-documented NativeAOT requirement (not a reverse-engineered
   unknown).
4. `.github/workflows/uixrender-ci.yml` added (`windows-latest`, since NativeAOT can't
   cross-compile Linux→Windows and today's managed consumers only run on Windows);
   publish steps pass `-f net8.0` explicitly since the project may carry a second TFM
   (`net8.0-windows10.0.22000`, from Directory.Build.props' Windows-conditional block)
   that isn't the one we ever intend to AOT-publish.
5. Verified locally (Linux): both `UIXrender.csproj` and `UIXsup.csproj` build clean
   (0 errors) as plain `net8.0`. Actual `PublishAot`/`NativeLib` publish behavior can
   only be verified on Windows (CI) — not checked yet as of this entry.

**Implementation complete for this phase**, all under `UIXsup/`:
- `DebugCategory.cs` — enum mirror, ordinal-exact.
- `DebugState.cs` — shared mutable state (timed-write-lines flag, prefix string,
  per-`DebugCategory` byte levels array, bounds-checked on both get/set since the enum
  value crosses an unmanaged boundary).
- `Interop/DebugApi.cs` — the 5 `[UnmanagedCallersOnly]` exports, `byte*`/`Marshal.PtrToStringAnsi`
  for the ANSI string params, `int` (not `bool`) for `DebugSetTimedWriteLines`'s
  parameter and `DebugDisplayErrorStack`'s return (matching the default 4-byte Win32
  `BOOL` marshaling `bool` gets on the managed P/Invoke side), operating on `DebugState`.

`dotnet build UIXsup/UIXsup.csproj` verified clean (0 warnings, 0 errors) on Linux —
this only proves compilation, not that NativeAOT publish actually produces a working
export table; that's still unverified until CI runs on Windows (next step).

**Known, deliberate scope gap (not a reverse-engineering unknown — a sequencing
decision):** `DebugDisplayErrorStack` almost certainly showed an interactive Win32
dialog in the original (title/message/file/line/stack, likely Abort/Retry/Ignore-style,
letting a developer break into the debugger) — no dialog UI exists yet in this
reimplementation. Real behavior implemented instead: format and write the message to
`Console.Error` (honoring the `TimedWriteLines`/`WriteLinePrefix` state the other 4
exports configure), always returning "don't break" (`0`/`false`). Marked with a `// TODO`
in the source pointing back to this entry.
