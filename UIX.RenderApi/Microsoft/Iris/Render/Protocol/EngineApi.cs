// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.EngineApi
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;

namespace Microsoft.Iris.Render.Protocol
{
    [SuppressUnmanagedCodeSecurity]
    internal static class EngineApi
    {
        private const string s_stEhRenderDll = "UIXRender.dll";

        // SpInit/SpUninit/SpBufferOpen/SpWrapBufferProc/SpRenderThreadInit/
        // SpRenderThreadUninit below call directly into UIXrender's managed
        // Microsoft.Iris.Render.Engine.EngineService instead of P/Invoking into
        // UIXRender.dll -- no native marshaling when both assemblies are loaded in the
        // same process. Everything else in this file is untouched DllImport, since
        // UIXrender doesn't implement those exports yet. See
        // logs/UIXrender/EngineCore.md for the reasoning (first modification of
        // previously-decompiled code in this project) and why these 6 are exactly the
        // set RenderPort.cs/LocalChannel.cs actually use.

        public static void IFC(HRESULT hr)
        {
            if (hr.Int >= 0)
                return;
            var code = (RenderException.ErrorCode)hr.Int;
            switch (hr.Int)
            {
                case -2147221503:
                case -2147221502:
                    throw new OutOfMemoryException();
                case -2147221494:
                    throw new RenderException(code, "Generic failure.");
                case -2147221493:
                    throw new RenderException(code, "The object is in a \"busy\" state and is not available to process the request.");
                case -2147221492:
                    throw new RenderException(code, "The object is not in a usable state to process the request.");
                case -2147221484:
                    throw new RenderException(code, "The Context has not been initialized.");
                case -2147221474:
                    throw new RenderException(code, "The object was used in the incorrect context.");
                case -2147221473:
                    throw new RenderException(code, "The Context has been marked to only allow read-only operations.  For example, this may be in the middle of a read-only callback.");
                case -2147221472:
                    throw new RenderException(code, "The threading model has already be determined by a previous call to SpInit() and can no longer be changed.");
                case -2147221471:
                    throw new RenderException(code, "Unable to use the IGMM_STANDARD messaging model because it is either unsupported or cannot be installed.");
                case -2147221464:
                    throw new RenderException(code, "Can not mix an invalid coordinate mapping, for example having a non-relative child of a relative parent.");
                case -2147221454:
                    throw new RenderException(code, "Could not find a MSGID for one of the requested messages.  This will be represented by a '0' in the MSGID field for that message.");
                case -2147221444:
                    throw new RenderException(code, "The operation is not legal because the specified Gadget does not have a GS_BUFFERED style.");
                case -2147221434:
                    throw new RenderException(code, "The specific Gadget has started the destruction and can not be be modified in this manner.");
                case -2147221433:
                    throw new RenderException(code, "The specific object is locked and may not be modified.");
                case -2147221432:
                    throw new InvalidOperationException("The operation is not supported.");
                case -2147221424:
                    throw new RenderException(code, "The specified optional component has not yet been initialized with InitGadgetComponent().");
                case -2147221414:
                    throw new RenderException(code, "The specified object was not found.");
                case -2147221413:
                    throw new RenderException(code, "The ObjectID is already in use.");
                case -2147221412:
                    throw new FileNotFoundException("The specified file could not be found");
                case -2147221411:
                    throw new ArgumentOutOfRangeException("", "The argument is out of range");
                case -2147221404:
                    throw new RenderException(code, "The specified parmeters are mismatched for the current object state.");
                case -2147221394:
                    throw new RenderException(code, "GDI+ was unable to be loaded.  It may not be installed on the system or may not be properly initialized.");
                case -2147221393:
                    throw new RenderException(code, "Direct3D was unable to be loaded.  It may not be installed on the system or may not be properly initialized.");
                case -2147221384:
                    throw new RenderException(code, "The specified class was already registered.");
                case -2147221383:
                    throw new RenderException(code, "The specified message was not found during class registration.");
                case -2147221382:
                    throw new RenderException(code, "The specified message was not implemented during class registration.");
                case -2147221381:
                    throw new RenderException(code, "The implementation of the specific class has not yet been registered.");
                case -2147221380:
                    throw new RenderException(code, "Sending the message failed.");
                case -2147221379:
                    throw new RenderException(code, "The message data is too large.");
                case -2147221374:
                    throw new RenderException(code, "The specified object does not have any content.");
                case -2147221373:
                    throw new RenderException(code, "The specified object is not properly setup to store the data.");
                case -2147221364:
                    throw new RenderException(code, "Generic failure from Win32 that did not SetLastError().");
                case -2147221363:
                    throw new RenderException(code, "Generic failure from GDI+.");
                case -2147221362:
                    throw new RenderException(code, "Generic failure from driver or rendering.");
                case -2147221354:
                    throw new RenderException(code, "Unable to connect to remote renderer.");
                default:
                    Marshal.ThrowExceptionForHR(hr.Int);
                    break;
            }
        }

#if !NETFRAMEWORK
        public static HRESULT SpInit(ref InitArgs args) => new HRESULT(0);

        public static HRESULT SpUninit() => new HRESULT(0);

        public static unsafe HRESULT SpBufferOpen(
          BufferInfo* phdrData,
          void* pvData)
        {
            var src = new Iface.ContextID(ContextID.ToUInt32(phdrData->idContextSrc));
            var dest = new Iface.ContextID(ContextID.ToUInt32(phdrData->idContextDest));
            var bufferHandle = new Iface.RENDERHANDLE(RENDERHANDLE.ToUInt32(phdrData->idBuffer));
            var flags = (Iface.BufferFlags)phdrData->nFlags;
            var span = new ReadOnlySpan<byte>(pvData, (int)phdrData->cbSizeBuffer);

            Iface.HRESULT result = EngineService.SendBuffer(src, dest, bufferHandle, flags, span);
            return new HRESULT(result.hr);
        }

        public static unsafe HRESULT SpWrapBufferProc(
          MessageBufferEventHandler pfnProcessBufferProc,
          IntPtr* ppNativeProc)
        {
            if (ppNativeProc == null)
                return new HRESULT(unchecked((int)0x80070057));

            if (pfnProcessBufferProc == null)
            {
                *ppNativeProc = IntPtr.Zero;
                return new HRESULT(0);
            }

            // Managed-direct: store the delegate itself instead of marshaling to a
            // native function pointer -- SpRenderThreadInit resolves this straight back
            // to the delegate object and invokes it directly, no calli anywhere on this
            // path. See logs/UIXrender/EngineCore.md.
            GCHandle handle = GCHandle.Alloc(pfnProcessBufferProc, GCHandleType.Normal);
            *ppNativeProc = GCHandle.ToIntPtr(handle);
            return new HRESULT(0);
        }
#else
        // net461 (EnableNetFXTarget): UIXrender's managed API isn't referenceable from
        // .NET Framework, so this TFM keeps calling the real native UIXRender.dll.
        [DllImport(s_stEhRenderDll)]
        public static extern HRESULT SpInit(ref InitArgs args);

        [DllImport(s_stEhRenderDll)]
        public static extern HRESULT SpUninit();

        [DllImport(s_stEhRenderDll)]
        public static extern unsafe HRESULT SpBufferOpen(
          BufferInfo* phdrData,
          void* pvData);

        [DllImport(s_stEhRenderDll)]
        public static extern unsafe HRESULT SpWrapBufferProc(
          MessageBufferEventHandler pfnProcessBufferProc,
          IntPtr* ppNativeProc);
#endif

#if !NETFRAMEWORK
        // No Win32 message queue exists behind this reimplementation (see EngineService),
        // and LocalChannel -- the path Zune uses -- never peeks. Reports "no message".
        public static HRESULT SpPeekMessage(
          out Win32Api.MSG msg,
          HWND hwnd,
          uint nMsgFilterMin,
          uint nMsgFilterMax,
          uint wRemoveMsg,
          out WorkResult nResult)
        {
            msg = default;
            nResult = (WorkResult)EngineService.PeekMessage(nMsgFilterMin, nMsgFilterMax, wRemoveMsg);
            return new HRESULT(0);
        }

        public static HRESULT SpWaitMessage(uint nTimeOutMs, IntPtr _unused)
        {
            EngineService.WaitMessage(nTimeOutMs);
            return new HRESULT(0);
        }

        public static HRESULT SpInvoke(
          ContextID idContext,
          IntPtr pfnInvoke,
          IntPtr pvArgs,
          bool synchronous)
        {
            Iface.HRESULT result = EngineService.Invoke(
              new Iface.ContextID(ContextID.ToUInt32(idContext)), pfnInvoke, pvArgs, synchronous);
            return new HRESULT(result.hr);
        }
#else
        [DllImport(s_stEhRenderDll, CharSet = CharSet.Auto)]
        public static extern HRESULT SpPeekMessage(
          out Win32Api.MSG msg,
          HWND hwnd,
          uint nMsgFilterMin,
          uint nMsgFilterMax,
          uint wRemoveMsg,
          out WorkResult nResult);

        [DllImport(s_stEhRenderDll)]
        public static extern HRESULT SpWaitMessage(uint nTimeOutMs, IntPtr _unused);

        [DllImport(s_stEhRenderDll)]
        public static extern HRESULT SpInvoke(
          ContextID idContext,
          IntPtr pfnInvoke,
          IntPtr pvArgs,
          bool synchronous);
#endif

#if !NETFRAMEWORK
        public static HRESULT SpRenderThreadInit(
          ref InitArgs argsRender,
          out IntPtr pThread)
        {
            var contextId = new Iface.ContextID(ContextID.ToUInt32(argsRender.idContext));

            MessageBufferEventHandler managedCallback = argsRender.pfnProcessBuffer != IntPtr.Zero
              ? GCHandle.FromIntPtr(argsRender.pfnProcessBuffer).Target as MessageBufferEventHandler
              : null;

            BufferReceivedHandler handler = managedCallback != null
              ? AdaptCallback(managedCallback, argsRender.idContext)
              : delegate { };

            IRenderThreadHandle threadHandle = EngineService.StartRenderThread(contextId, handler);
            pThread = GCHandle.ToIntPtr(GCHandle.Alloc(threadHandle, GCHandleType.Normal));
            return new HRESULT(0);
        }

        // Adapts a stored MessageBufferEventHandler (already-decompiled, still
        // pointer-shaped -- see logs/UIXrender/EngineCore.md) into the idiomatic
        // BufferReceivedHandler shape EngineService deals in. Factored out of
        // SpRenderThreadInit so that method reads as resolve -> adapt -> start -> wrap,
        // and named to make clear it's the same kind of adaptation
        // UIXrender/Interop/EngineApi.cs's own SpRenderThreadInit does for native
        // callers (there: raw function pointer -> BufferReceivedHandler; here: managed
        // delegate -> BufferReceivedHandler -- same shape, different invocation
        // mechanism at the end).
        private static unsafe BufferReceivedHandler AdaptCallback(MessageBufferEventHandler callback, ContextID destContext) =>
            (source, bufferHandle, flags, data) =>
            {
                fixed (byte* pData = data)
                {
                    var info = new BufferInfo
                    {
                        idContextSrc = ContextID.FromUInt32(source.value),
                        idContextDest = destContext,
                        idBuffer = RENDERHANDLE.FromUInt32(bufferHandle.value),
                        nFlags = (BufferFlags)flags,
                        cbSizeBuffer = (uint)data.Length,
                    };
                    callback(IntPtr.Zero, source.value, &info, pData);
                }
            };

        public static HRESULT SpRenderThreadUninit(IntPtr pThread)
        {
            if (pThread == IntPtr.Zero)
                return new HRESULT(unchecked((int)0x80070057));

            GCHandle handle = GCHandle.FromIntPtr(pThread);
            (handle.Target as IDisposable)?.Dispose();
            handle.Free();
            return new HRESULT(0);
        }
#else
        [DllImport(s_stEhRenderDll)]
        public static extern HRESULT SpRenderThreadInit(
          ref InitArgs argsRender,
          out IntPtr pThread);

        [DllImport(s_stEhRenderDll)]
        public static extern HRESULT SpRenderThreadUninit(IntPtr pThread);
#endif

#if !NETFRAMEWORK
        public static HRESULT SpRemoteCreateServerStreams(
          string stSession,
          TransportProtocol nProtocol,
          out IntPtr pSendStream,
          out IntPtr pReceiveStream)
        {
            Iface.HRESULT result = EngineService.RemoteCreateServerStreams(
              stSession, (Iface.Protocol.TransportProtocol)(int)nProtocol, out pSendStream, out pReceiveStream);
            return new HRESULT(result.hr);
        }

        public static HRESULT SpRemoteWaitServerStreamsConnected(
          TransportProtocol nProtocol,
          IntPtr pSendStream,
          IntPtr pReceiveStream)
        {
            Iface.HRESULT result = EngineService.RemoteWaitServerStreamsConnected(
              (Iface.Protocol.TransportProtocol)(int)nProtocol, pSendStream);
            return new HRESULT(result.hr);
        }

        public static HRESULT SpRemoteServerInit(
          IntPtr pSendStream,
          IntPtr pReceiveStream,
          InitArgs argsSend,
          out IntPtr pSession)
        {
            var context = new Iface.ContextID(ContextID.ToUInt32(argsSend.idContext));

            // Same delegate-behind-a-GCHandle representation SpRenderThreadInit resolves;
            // RemoteChannel connects without a receive callback (pfnProcessBuffer == 0),
            // so this is normally null.
            MessageBufferEventHandler managedCallback = argsSend.pfnProcessBuffer != IntPtr.Zero
              ? GCHandle.FromIntPtr(argsSend.pfnProcessBuffer).Target as MessageBufferEventHandler
              : null;
            BufferReceivedHandler handler = managedCallback != null
              ? AdaptCallback(managedCallback, argsSend.idContext)
              : null;

            Iface.HRESULT result = EngineService.RemoteServerInit(pSendStream, context, handler, out pSession);
            return new HRESULT(result.hr);
        }

        public static HRESULT SpRemoteServerUninit(
          IntPtr pSession,
          bool fForceShutdown,
          out ShutdownReason nShutdownReason)
        {
            Iface.HRESULT result = EngineService.RemoteServerUninit(
              pSession, fForceShutdown, out Iface.Protocol.ShutdownReason reason);
            nShutdownReason = (ShutdownReason)(int)reason;
            return new HRESULT(result.hr);
        }

        public static HRESULT SpDx9CompileEffect(
          string stEffect,
          string stDefines,
          out IntPtr pErrorString,
          out IntPtr pErrorBuffer,
          out IntPtr pEffectBlob,
          out uint EffectBlobSize,
          out IntPtr pEffectBlobBuffer)
        {
            pErrorString = IntPtr.Zero;
            pErrorBuffer = IntPtr.Zero;
            pEffectBlob = IntPtr.Zero;
            EffectBlobSize = 0U;
            pEffectBlobBuffer = IntPtr.Zero;
            return new HRESULT(EngineService.Dx9CompileEffect().hr);
        }

        // SpObjectRelease's only callers (RemoteChannel) release the stream handles from
        // SpRemoteCreateServerStreams, which in the managed-direct path are UIXrender
        // handles, not COM pointers -- so this drops the handle's reference rather than
        // calling through a vtable.
        public static void SpObjectRelease(IntPtr pUnknown) => EngineService.ReleaseRemoteStream(pUnknown);
#else
        [DllImport(s_stEhRenderDll, CharSet = CharSet.Unicode)]
        public static extern HRESULT SpRemoteCreateServerStreams(
          string stSession,
          TransportProtocol nProtocol,
          out IntPtr pSendStream,
          out IntPtr pReceiveStream);

        [DllImport(s_stEhRenderDll)]
        public static extern HRESULT SpRemoteWaitServerStreamsConnected(
          TransportProtocol nProtocol,
          IntPtr pSendStream,
          IntPtr pReceiveStream);

        [DllImport(s_stEhRenderDll)]
        public static extern HRESULT SpRemoteServerInit(
          IntPtr pSendStream,
          IntPtr pReceiveStream,
          InitArgs argsSend,
          out IntPtr pSession);

        [DllImport(s_stEhRenderDll)]
        public static extern HRESULT SpRemoteServerUninit(
          IntPtr pSession,
          bool fForceShutdown,
          out ShutdownReason nShutdownReason);

        [DllImport(s_stEhRenderDll, CharSet = CharSet.Ansi)]
        public static extern HRESULT SpDx9CompileEffect(
          string stEffect,
          string stDefines,
          out IntPtr pErrorString,
          out IntPtr pErrorBuffer,
          out IntPtr pEffectBlob,
          out uint EffectBlobSize,
          out IntPtr pEffectBlobBuffer);

        [DllImport(s_stEhRenderDll)]
        public static extern void SpObjectRelease(IntPtr pUnknown);
#endif

        [Flags]
        public enum BufferFlags
        {
            IsBatch = 1,
            CopyData = 2,
            Valid = CopyData | IsBatch, // 0x00000003
        }

        [ComVisible(false)]
        public struct BufferInfo
        {
            public ContextID idContextSrc;
            public ContextID idContextDest;
            public RENDERHANDLE idBuffer;
            public BufferFlags nFlags;
            public uint cbSizeBuffer;
        }

        internal delegate void TimeoutEventHandler(IntPtr pData);

        [ComVisible(false)]
        internal struct InitArgs
        {
            public uint cbSize;
            public ContextID idContext;
            public int cItemsPerGroupBits;
            public int cGroupBits;
            public IntPtr pfnProcessBuffer;
            public IntPtr pvProcessData;
            public RENDERHANDLE idObjectBrokerClass;
            public TimeoutEventHandler pfnTimeout;
            public IntPtr pvTimeoutData;
            public uint nTimeOutSec;

            public InitArgs(MessageCookieLayout layout, ContextID idContextNew)
            {
                Debug2.Validate(idContextNew != ContextID.NULL, typeof(ArgumentNullException), nameof(idContextNew));
                cbSize = (uint)Marshal.SizeOf(typeof(InitArgs));
                idContext = idContextNew;
                cItemsPerGroupBits = layout.numberOfObjectBits;
                cGroupBits = layout.numberOfGroupBits;
                pfnProcessBuffer = IntPtr.Zero;
                pvProcessData = IntPtr.Zero;
                idObjectBrokerClass = RENDERHANDLE.FromUInt32(0U);
                pfnTimeout = null;
                pvTimeoutData = IntPtr.Zero;
                nTimeOutSec = 0U;
            }

            public unsafe InitArgs(
              MessageCookieLayout layout,
              ContextID idContextNew,
              MessageBufferEventHandler pfnProcessBufferProc)
              : this(layout, idContextNew)
            {
                if (pfnProcessBufferProc == null)
                    return;
                IntPtr num;
                IFC(SpWrapBufferProc(pfnProcessBufferProc, &num));
                pfnProcessBuffer = num;
            }
        }

        internal unsafe delegate int MessageBufferEventHandler(
          IntPtr pData,
          uint hContext,
          BufferInfo* pBufferInfo,
          void* pvBufferData);

        [Flags]
        public enum WorkResult : uint
        {
            ProcessedMessage = 1,
            NewUserMessage = 2,
        }
    }
}
