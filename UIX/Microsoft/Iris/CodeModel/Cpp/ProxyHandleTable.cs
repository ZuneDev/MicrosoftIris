// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.ProxyHandleTable
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Diagnostics;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class ProxyHandleTable
    {
        public const ulong c_InvalidHandle = 0;
        private uint _tableIndex;
        private static uint s_tableCount;

        protected ProxyHandleTable() => _tableIndex = s_tableCount++;

        protected uint TableIndex => _tableIndex;

        [Conditional("DEBUG")]
        public static void DEBUG_AssertValidHandle(ulong handle)
        {
        }

        public static string DEBUG_FormatHandle(ulong handle) => string.Empty;
    }
}
