// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Common.SharedResourceArray
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;

namespace Microsoft.Iris.Render.Common
{
    internal class SharedResourceArray : SharedResource
    {
        private SharedResource[] m_rgResources;

        internal SharedResourceArray(SharedResource resource)
          : base(null)
        {
            this.m_rgResources = new SharedResource[1];
            this.m_rgResources[0] = resource;
        }

        internal SharedResourceArray(SharedResource[] rgResources)
          : base(null)
          => this.m_rgResources = rgResources;

        protected override void OnUsageChange(bool fUsed)
        {
            for (int index = 0; index < this.m_rgResources.Length; ++index)
            {
                if (this.m_rgResources[index] != null)
                {
                    if (fUsed)
                        this.m_rgResources[index].AddActiveUser(this);
                    else
                        this.m_rgResources[index].RemoveActiveUser(this);
                }
            }
        }

        internal override void RegisterUsage(object user)
        {
            base.RegisterUsage(user);
            foreach (SharedRenderObject rgResource in this.m_rgResources)
                rgResource.RegisterUsage(user);
        }

        internal override void UnregisterUsage(object user)
        {
            foreach (SharedRenderObject rgResource in this.m_rgResources)
                rgResource.UnregisterUsage(user);
            base.UnregisterUsage(user);
        }

        internal SharedResource[] Resources => this.m_rgResources;
    }
}
