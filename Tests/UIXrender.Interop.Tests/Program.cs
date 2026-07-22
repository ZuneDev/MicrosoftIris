using System;
using System.Runtime.InteropServices;
using System.Threading;
using UIXrender.Interop.Tests;

// Phase 0 exit-criteria check (see the approved plan and logs/UIXrender/EngineCore.md):
// proves a native OS thread the CLR didn't create can call back into managed code via
// the exact SpWrapBufferProc/SpRenderThreadInit mechanism every Remote*/Local*Callback
// class in UIX.RenderApi depends on. Plain console app with explicit pass/fail output
// and a non-zero exit code on failure, so CI can gate on it without a test framework.

int failures = 0;

void Check(bool condition, string message)
{
    Console.WriteLine((condition ? "PASS: " : "FAIL: ") + message);
    if (!condition)
        failures++;
}

NativeMethods.SpInitializeTracing();
NativeMethods.SpUninitializeTracing();
Check(true, "SpInitializeTracing/SpUninitializeTracing did not throw");

var callbackInvoked = new ManualResetEventSlim(false);
uint observedContext = 0;

int Handler(IntPtr pData, uint hContext, IntPtr pBufferInfo, IntPtr pvBufferData)
{
    observedContext = hContext;
    callbackInvoked.Set();
    return 0;
}

// The delegate's real signature takes BufferInfo*/void* (unsafe); marshaled here as
// IntPtr in the local function above purely so it can be a normal (non-pointer-typed)
// C# local -- NativeMethods.MessageBufferEventHandler is the one the P/Invoke call
// actually binds against.
unsafe
{
    NativeMethods.MessageBufferEventHandler handler = (pData, hContext, pBufferInfo, pvBufferData) =>
        Handler(pData, hContext, (IntPtr)pBufferInfo, (IntPtr)pvBufferData);

    int hr = NativeMethods.SpWrapBufferProc(handler, out IntPtr nativeProc);
    Check(hr >= 0, $"SpWrapBufferProc succeeded (hr=0x{hr:X8})");

    var initArgs = new NativeMethods.InitArgs
    {
        cbSize = (uint)Marshal.SizeOf<NativeMethods.InitArgs>(),
        idContext = 42,
        cItemsPerGroupBits = 20,
        cGroupBits = 4,
        pfnProcessBuffer = nativeProc,
        pvProcessData = IntPtr.Zero,
        idObjectBrokerClass = 0,
        pfnTimeout = IntPtr.Zero,
        pvTimeoutData = IntPtr.Zero,
        nTimeOutSec = 0,
    };

    hr = NativeMethods.SpRenderThreadInit(ref initArgs, out IntPtr pThread);
    Check(hr >= 0, $"SpRenderThreadInit succeeded (hr=0x{hr:X8})");

    bool signaled = callbackInvoked.Wait(TimeSpan.FromSeconds(5));
    Check(signaled, "native thread invoked the managed callback within 5s");
    Check(observedContext == 42, $"callback observed the expected context id (got {observedContext})");

    hr = NativeMethods.SpRenderThreadUninit(pThread);
    Check(hr >= 0, $"SpRenderThreadUninit succeeded (hr=0x{hr:X8})");
}

Console.WriteLine(failures == 0 ? "ALL CHECKS PASSED" : $"{failures} CHECK(S) FAILED");
return failures == 0 ? 0 : 1;
