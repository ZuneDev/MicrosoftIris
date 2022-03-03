// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Common.DeferredInvokeItem
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render.Common
{
    internal class DeferredInvokeItem : IDeferredInvokeItem
    {
        private Delegate m_method;
        private object m_args;
        private SharedRenderObject m_owner;

        internal DeferredInvokeItem(SharedRenderObject owner, Delegate method, object args)
        {
            this.m_method = method;
            this.m_args = args;
            this.m_owner = owner;
            if (this.m_owner == null)
                return;
            this.m_owner.RegisterUsage(this);
        }

        public void Dispatch()
        {
            ((DeferredHandler)this.m_method)(this.m_args);
            if (this.m_owner == null)
                return;
            this.m_owner.UnregisterUsage(this);
        }
    }
}
