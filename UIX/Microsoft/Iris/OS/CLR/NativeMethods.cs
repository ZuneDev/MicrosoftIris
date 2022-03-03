// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.CLR.NativeMethods
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Security.Permissions;

namespace Microsoft.Iris.OS.CLR
{
    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    internal static class NativeMethods
    {
        public const int S_OK = 0;
        public const int E_ABORT = -2147467260;
        public const int E_NOTIMPL = -2147467263;
    }
}
