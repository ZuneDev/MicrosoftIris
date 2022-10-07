// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Library.DataCookie
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Library
{
    public struct DataCookie
    {
        public static readonly DataCookie NULL = new DataCookie();
        private uint m_value;

        private DataCookie(uint value) => m_value = value;

        public static bool operator ==(DataCookie hl, DataCookie hr) => (int)hl.m_value == (int)hr.m_value;

        public static bool operator !=(DataCookie hl, DataCookie hr) => (int)hl.m_value != (int)hr.m_value;

        public override bool Equals(object oCompare)
        {
            uint num;
            if (oCompare is DataCookie dataCookie)
                num = dataCookie.m_value;
            else if (oCompare is uint value)
                num = value;
            else
                return false;

            return (int)m_value == (int)num;
        }

        public override int GetHashCode() => (int)m_value;

        internal static DataCookie FromUInt32(uint value) => new DataCookie(value);

        internal static uint ToUInt32(DataCookie handle) => handle.m_value;

        public static DataCookie ReserveSlot() => FromUInt32(KeyAllocator.ReserveSlot());
    }
}
