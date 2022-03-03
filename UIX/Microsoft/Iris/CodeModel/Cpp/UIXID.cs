// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.UIXID
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal static class UIXID
    {
        public const uint IntrinsicSchemaComponent = 65535;
        public const uint IntrinsicSchemaID = 4294901760;
        public const uint Invalid = 4294967295;
        public const uint Bool = 4294967294;
        public const uint Byte = 4294967293;
        public const uint Double = 4294967292;
        public const uint IBooleanChoice = 4294967291;
        public const uint IByteRangedValue = 4294967290;
        public const uint IChoice = 4294967289;
        public const uint ICommand = 4294967288;
        public const uint IDictionary = 4294967287;
        public const uint IIntRangedValue = 4294967286;
        public const uint IList = 4294967285;
        public const uint Image = 4294967284;
        public const uint Int32 = 4294967283;
        public const uint Int64 = 4294967282;
        public const uint IRangedValue = 4294967281;
        public const uint Object = 4294967280;
        public const uint Single = 4294967279;
        public const uint String = 4294967278;
        public const uint Void = 4294967277;
        public const uint DataQuery = 4294967276;
        public const uint DataType = 4294967275;
        private const int SCHEMA_SHIFT = 16;
        private const uint SCHEMA_MASK = 4294901760;
        private const uint LOCAL_MASK = 65535;

        public static uint GetSchemaID(uint ID) => ID & 4294901760U;

        public static uint GetSchemaComponent(uint ID) => (ID & 4294901760U) >> 16;

        public static uint GetLocalComponent(uint ID) => ID & ushort.MaxValue;
    }
}
