// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.CLR.UnsafeNativeMethods
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Iris.OS.CLR
{
    [SuppressUnmanagedCodeSecurity]
    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    internal static class UnsafeNativeMethods
    {
        [DllImport("Ole32.dll")]
        internal static extern int CoMarshalInterface(
          [MarshalAs(UnmanagedType.Interface)] object pStm,
          ref Guid riid,
          IntPtr pv,
          int dwDestContext,
          IntPtr pvDestContext,
          int mshlflags);

        [DllImport("Ole32.dll")]
        internal static extern int CoGetStandardMarshal(
          ref Guid riid,
          IntPtr pv,
          int dwDestContext,
          IntPtr pvDestContext,
          int mshlflags,
          out IntPtr ppMarshal);

        [DllImport("Ole32.dll")]
        internal static extern int CoGetMarshalSizeMax(
          out int pulSize,
          ref Guid riid,
          IntPtr pv,
          int dwDestContext,
          IntPtr pvDestContext,
          int mshlflags);

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("00000017-0000-0000-c000-000000000046")]
        [ComImport]
        internal interface IStdMarshal
        {
        }

        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("00000003-0000-0000-C000-000000000046")]
        [ComImport]
        internal interface IMarshal
        {
            [MethodImpl(MethodImplOptions.PreserveSig)]
            int GetUnmarshalClass(
              ref Guid riid,
              IntPtr pv,
              int dwDestContext,
              IntPtr pvDestContext,
              int mshlflags,
              out Guid pCid);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int GetMarshalSizeMax(
              ref Guid riid,
              IntPtr pv,
              int dwDestContext,
              IntPtr pvDestContext,
              int mshlflags,
              out int pSize);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int MarshalInterface(
              [MarshalAs(UnmanagedType.Interface)] object pStm,
              ref Guid riid,
              IntPtr pv,
              int dwDestContext,
              IntPtr pvDestContext,
              int mshlflags);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int UnmarshalInterface([MarshalAs(UnmanagedType.Interface)] object pStm, ref Guid riid, out IntPtr ppv);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int ReleaseMarshalData([MarshalAs(UnmanagedType.Interface)] object pStm);

            [MethodImpl(MethodImplOptions.PreserveSig)]
            int DisconnectObject(int dwReserved);
        }
    }
}
