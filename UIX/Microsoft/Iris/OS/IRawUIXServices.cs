// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.IRawUIXServices
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.CodeModel.Cpp;
using Microsoft.Iris.RenderAPI;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.OS
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("5DFE83AF-CE94-4f3b-96A5-8B076F4469A4")]
    internal interface IRawUIXServices
    {
        [MethodImpl(MethodImplOptions.PreserveSig)]
        void Crash([MarshalAs(UnmanagedType.LPWStr)] string message);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        HRESULT NotifyChangeForObject(IntPtr nativeObject, uint changedID);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        unsafe ulong AllocateString(char* value, out int length);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        unsafe void CopyString(ulong handle, char* target, uint targetSize);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        unsafe char* PinString(ulong handle);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void UnpinString(ulong handle);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void ReleaseString(ulong handle);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        unsafe ulong AllocateImageFromUri([MarshalAs(UnmanagedType.LPWStr)] string uri, NativeApi.IMAGE_DECODE_PARAMS* constructParams);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        unsafe ulong AllocateImageFromBits(
          [MarshalAs(UnmanagedType.LPWStr)] string ID,
          NativeApi.UIX_RAW_IMAGE_INFO* imageInfo,
          NativeApi.IMAGE_DECODE_PARAMS* decodeParams);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        unsafe void RemoveCachedImage([MarshalAs(UnmanagedType.LPWStr)] string ID, NativeApi.IMAGE_DECODE_PARAMS* decodeParams);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void ReleaseImage(ulong handle);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void RegisterDataProvider([MarshalAs(UnmanagedType.LPWStr)] string providerName, IntPtr nativeFactoryCallback);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void GetDataMapping([MarshalAs(UnmanagedType.LPWStr)] string providerName, ulong typeHandle, IntPtr nativeCallback);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void NotifyChangeForDataObject(
          ulong frameworkObjectHandle,
          [MarshalAs(UnmanagedType.LPWStr)] string propertyName,
          UIXVariant.VariantType variantType);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        IntPtr GetAppWindowHandle();

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void ReportError(bool isWarning, [MarshalAs(UnmanagedType.LPWStr)] string message);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        void LowPriorityDeferredInvoke(IntPtr pfnCallback, IntPtr pvCallbackData);
    }
}
