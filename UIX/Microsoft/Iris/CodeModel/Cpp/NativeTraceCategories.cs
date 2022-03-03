// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.NativeTraceCategories
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Runtime.InteropServices;

namespace Microsoft.Iris.CodeModel.Cpp
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    internal struct NativeTraceCategories
    {
        public const byte TypeLoading = 1;
        public const byte Handles = 2;
        public const byte PropertyChanges = 4;
        public const byte Invocations = 8;
    }
}
