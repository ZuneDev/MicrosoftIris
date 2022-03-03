// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.GdiEffect
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt;

namespace Microsoft.Iris.Render.Graphics
{
    internal class GdiEffect : Effect
    {
        internal GdiEffect(GdiEffectTemplate effectTemplate)
          : base(effectTemplate)
          => this.m_remoteEffect = this.m_effectTemplate.Session.BuildRemoteGdiEffect(this);

        internal override void RemoteEffect()
        {
            ((RemoteGdiEffect)this.m_remoteEffect).SendSetType((int)((GdiEffectTemplate)this.m_effectTemplate).Type);
            this.Initialize();
        }
    }
}
