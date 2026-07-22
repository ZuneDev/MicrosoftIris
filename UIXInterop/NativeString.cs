using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Interop;

// [UnmanagedCallersOnly] methods can't take `string` params (not blittable) -- callers
// take raw pointers and go through here instead, so the "which encoding" decision (read
// from each P/Invoke call site's CharSet, see per-module logs) is made in one place.
public static unsafe class NativeString
{
    public static string AnsiToString(byte* p) => p == null ? null : Marshal.PtrToStringAnsi((IntPtr)p);

    public static string UniToString(char* p) => p == null ? null : Marshal.PtrToStringUni((IntPtr)p);

    // Interned, caller-does-not-free native copies. Several exports hand a `char*` back
    // out (SpQueryTypeName, SpQueryPropertyName, SpQueryMethodName, SpQueryEnumName, ...)
    // and their managed declarations never free what they receive -- so the callee has to
    // own the memory for the process lifetime. Interning by value keeps that bounded:
    // these are schema names drawn from a fixed set of registered types, so the pool
    // stops growing once every name has been asked for once, instead of leaking a fresh
    // allocation per call.
    private static readonly ConcurrentDictionary<string, IntPtr> s_interned = new(StringComparer.Ordinal);

    public static char* InternUni(string value)
    {
        if (value == null)
            return null;

        return (char*)s_interned.GetOrAdd(value, static v => Marshal.StringToHGlobalUni(v));
    }

    // Non-interned copy, for values that are genuinely per-call (query results, converted
    // strings) rather than fixed schema names. The caller frees these via SpMemFree.
    public static char* AllocUni(string value) =>
        value == null ? null : (char*)Marshal.StringToHGlobalUni(value);
}
