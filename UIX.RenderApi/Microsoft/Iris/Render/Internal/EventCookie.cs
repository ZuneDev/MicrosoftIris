// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.EventCookie
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render.Internal
{
    internal struct EventCookie
    {
        public static readonly EventCookie NULL = new EventCookie();
        private uint m_value;

        private EventCookie(uint value) => this.m_value = value;

        public static bool operator ==(EventCookie hl, EventCookie hr) => (int)hl.m_value == (int)hr.m_value;

        public static bool operator !=(EventCookie hl, EventCookie hr) => (int)hl.m_value != (int)hr.m_value;

        public override bool Equals(object oCompare)
        {
            uint num;

            if (oCompare is EventCookie eventCookie)
                num = eventCookie.m_value;
            else if (oCompare is uint value)
                num = value;
            else
                return false;

            return (int)m_value == (int)num;
        }

        public override int GetHashCode() => (int)this.m_value;

        internal static EventCookie FromUInt32(uint value) => new EventCookie(value);

        internal static uint ToUInt32(EventCookie handle) => handle.m_value;

        public static EventCookie ReserveSlot() => FromUInt32(KeyAllocator.ReserveSlot());
    }
}
