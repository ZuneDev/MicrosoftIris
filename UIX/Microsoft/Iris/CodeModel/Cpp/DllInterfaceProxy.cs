// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllInterfaceProxy
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal abstract class DllInterfaceProxy : DllProxyObject
    {
        protected IntPtr _interface;

        ~DllInterfaceProxy() => RegisterAppThreadRelease(new DllProxyObject.AppThreadReleaseEntry(_interface));

        protected override void OnDispose() => new DllProxyObject.AppThreadReleaseEntry(_interface).Release();

        protected override void LoadWorker(IntPtr nativeObject, IntPtr nativeMarshalAs)
        {
            _interface = nativeMarshalAs;
            base.LoadWorker(nativeObject, nativeMarshalAs);
        }
    }
}
