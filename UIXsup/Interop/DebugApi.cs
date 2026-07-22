using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Support.Interop;

// [UnmanagedCallersOnly] exports matching the 5 DllImport("UIXsup.dll") declarations in
// the already-shipped managed callers (eDebugApi.cs / DebugHelpers.cs) field-for-field.
// See logs/UIXrender/UIXsup.md for why parameters are byte*/int here instead of
// string/bool (ANSI marshaling, blittability) and for the DebugDisplayErrorStack scope
// gap noted below.
public static unsafe class DebugApi
{
    [UnmanagedCallersOnly(EntryPoint = "DebugDisplayErrorStack")]
    public static int DebugDisplayErrorStack(byte* stMessage, byte* filename, int line, byte* title, byte* stackTrace)
    {
        string timestamp = DebugState.TimedWriteLines ? $"[{DateTime.Now:HH:mm:ss.fff}] " : string.Empty;
        Console.Error.WriteLine(
            $"{timestamp}{DebugState.WriteLinePrefix}{PtrToString(title)}: {PtrToString(stMessage)} ({PtrToString(filename)}:{line})\n{PtrToString(stackTrace)}");

        // TODO: no interactive dialog UI yet (see logs/UIXrender/UIXsup.md) -- always
        // reports "don't break".
        return 0;
    }

    [UnmanagedCallersOnly(EntryPoint = "DebugSetTimedWriteLines")]
    public static void DebugSetTimedWriteLines(int fEnabled) => DebugState.TimedWriteLines = fEnabled != 0;

    [UnmanagedCallersOnly(EntryPoint = "DebugSetWriteLinePrefix")]
    public static void DebugSetWriteLinePrefix(byte* stPrefix) => DebugState.WriteLinePrefix = PtrToString(stPrefix) ?? string.Empty;

    [UnmanagedCallersOnly(EntryPoint = "DebugGetCategoryLevel")]
    public static byte DebugGetCategoryLevel(DebugCategory cat) => DebugState.GetCategoryLevel(cat);

    [UnmanagedCallersOnly(EntryPoint = "DebugSetCategoryLevel")]
    public static void DebugSetCategoryLevel(DebugCategory cat, byte level) => DebugState.SetCategoryLevel(cat, level);

    private static string PtrToString(byte* p) => p == null ? null : Marshal.PtrToStringAnsi((IntPtr)p);
}
