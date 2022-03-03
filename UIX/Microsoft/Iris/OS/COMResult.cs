// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.COMResult
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Security;

namespace Microsoft.Iris.OS
{
    [SuppressUnmanagedCodeSecurity]
    internal static class COMResult
    {
        public const int S_OK = 0;
        public const int S_FALSE = 1;
        public const int E_NOTIMPL = -2147467263;
        public const int E_FAIL = -2147467259;
        public const int E_INVALIDARG = -2147024809;
        public const int STG_E_INVALIDFUNCTION = -2147287039;
    }
}
