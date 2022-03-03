// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Common.SharedResource
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;

namespace Microsoft.Iris.Render.Common
{
    internal abstract class SharedResource : SharedRenderObject
    {
        protected RenderSession m_session;
        private ResourceTracker m_resourceTracker;

        internal SharedResource(RenderSession session) => this.m_session = session;

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (!fInDispose)
                    return;
                ResourceTracker resourceTracker1 = this.m_resourceTracker;
                if (this.m_session == null || !this.m_session.IsValid)
                    return;
                ResourceTracker resourceTracker2 = this.m_resourceTracker;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        public bool InUse => this.m_resourceTracker != null && this.m_resourceTracker.InUse;

        public uint LastUsage => this.m_resourceTracker == null ? 0U : this.m_resourceTracker.LastUsage;

        public void AddActiveUser(object oUser)
        {
            if (this.m_resourceTracker == null)
                this.m_resourceTracker = new ResourceTracker(new ResourceTracker.UsageChangeHandler(this.OnUsageChange));
            this.m_resourceTracker.AddActiveUser(oUser);
        }

        public void RemoveActiveUser(object oUser)
        {
            this.m_resourceTracker.RemoveActiveUser(oUser);
            if (this.m_resourceTracker.InUse)
                return;
            this.m_resourceTracker = null;
        }

        protected abstract void OnUsageChange(bool fUsed);
    }
}
