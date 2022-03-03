// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Common.ResourceTracker
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render.Common
{
    internal class ResourceTracker
    {
        private int m_cUsers;
        private static uint s_uNextUsage;
        private uint m_uLastUsage;
        private ResourceTracker.UsageChangeHandler m_handler;

        internal ResourceTracker(ResourceTracker.UsageChangeHandler handler) => this.m_handler = handler;

        internal bool InUse => this.m_cUsers > 0;

        internal uint LastUsage => this.m_uLastUsage;

        internal void AddActiveUser(object oUser)
        {
            ++this.m_cUsers;
            this.m_uLastUsage = uint.MaxValue;
            if (this.m_cUsers != 1)
                return;
            this.NotifyUsageChange(true);
        }

        internal void RemoveActiveUser(object oUser)
        {
            --this.m_cUsers;
            if (this.m_cUsers != 0)
                return;
            this.m_uLastUsage = s_uNextUsage++;
            this.NotifyUsageChange(false);
        }

        protected virtual void NotifyUsageChange(bool fInUse)
        {
            if (this.m_handler == null)
                return;
            this.m_handler(fInUse);
        }

        internal delegate void UsageChangeHandler(bool fInUse);
    }
}
