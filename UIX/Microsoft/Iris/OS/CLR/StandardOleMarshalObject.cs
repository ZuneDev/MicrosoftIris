// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.CLR.StandardOleMarshalObject
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Microsoft.Iris.OS.CLR
{
    [ComVisible(true)]
    public class StandardOleMarshalObject : MarshalByRefObject, UnsafeNativeMethods.IMarshal
    {
        protected StandardOleMarshalObject()
        {
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private IntPtr GetStdMarshaller(ref Guid riid, int dwDestContext, int mshlflags)
        {
            IntPtr ppMarshal = IntPtr.Zero;
            IntPtr iunknownForObject = Marshal.GetIUnknownForObject(this);
            if (iunknownForObject != IntPtr.Zero)
            {
                try
                {
                    if (UnsafeNativeMethods.CoGetStandardMarshal(ref riid, iunknownForObject, dwDestContext, IntPtr.Zero, mshlflags, out ppMarshal) == 0)
                        return ppMarshal;
                }
                finally
                {
                    Marshal.Release(iunknownForObject);
                }
            }
            throw new InvalidOperationException();
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        int UnsafeNativeMethods.IMarshal.GetUnmarshalClass(
          ref Guid riid,
          IntPtr pv,
          int dwDestContext,
          IntPtr pvDestContext,
          int mshlflags,
          out Guid pCid)
        {
            pCid = typeof(UnsafeNativeMethods.IStdMarshal).GUID;
            return 0;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        int UnsafeNativeMethods.IMarshal.GetMarshalSizeMax(
          ref Guid riid,
          IntPtr pv,
          int dwDestContext,
          IntPtr pvDestContext,
          int mshlflags,
          out int pSize)
        {
            Guid riid1 = riid;
            IntPtr stdMarshaller = GetStdMarshaller(ref riid1, dwDestContext, mshlflags);
            try
            {
                return UnsafeNativeMethods.CoGetMarshalSizeMax(out pSize, ref riid1, stdMarshaller, dwDestContext, pvDestContext, mshlflags);
            }
            finally
            {
                Marshal.Release(stdMarshaller);
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        int UnsafeNativeMethods.IMarshal.MarshalInterface(
          object pStm,
          ref Guid riid,
          IntPtr pv,
          int dwDestContext,
          IntPtr pvDestContext,
          int mshlflags)
        {
            Guid riid1 = riid;
            IntPtr stdMarshaller = GetStdMarshaller(ref riid1, dwDestContext, mshlflags);
            try
            {
                return UnsafeNativeMethods.CoMarshalInterface(pStm, ref riid1, stdMarshaller, dwDestContext, pvDestContext, mshlflags);
            }
            finally
            {
                Marshal.Release(stdMarshaller);
                if (pStm != null)
                    Marshal.ReleaseComObject(pStm);
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        int UnsafeNativeMethods.IMarshal.UnmarshalInterface(
          object pStm,
          ref Guid riid,
          out IntPtr ppv)
        {
            ppv = IntPtr.Zero;
            if (pStm != null)
                Marshal.ReleaseComObject(pStm);
            return -2147467263;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        int UnsafeNativeMethods.IMarshal.ReleaseMarshalData(object pStm)
        {
            if (pStm != null)
                Marshal.ReleaseComObject(pStm);
            return -2147467263;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        int UnsafeNativeMethods.IMarshal.DisconnectObject(int dwReserved) => -2147467263;
    }
}
