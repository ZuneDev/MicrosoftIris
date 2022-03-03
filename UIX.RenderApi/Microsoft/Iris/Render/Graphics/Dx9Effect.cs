// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9Effect
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocols.Splash.Rendering;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9Effect : Effect
    {
        internal Dx9Effect(Dx9EffectTemplate effectTemplate)
          : base(effectTemplate)
          => this.m_remoteEffect = this.m_effectTemplate.Session.BuildRemoteDx9Effect(this);

        internal Dx9EffectManager EffectManager => ((Dx9GraphicsDevice)this.m_effectTemplate.Device).EffectManager;

        internal override void RemoteEffect()
        {
            ((RemoteDx9Effect)this.m_remoteEffect).SendLoadEffectResource(((Dx9EffectTemplate)this.m_effectTemplate).EffectResource.RemoteStub);
            this.Initialize();
        }

        internal override void SetProperty(string stPropertyName, Image image)
        {
            if (image != null)
            {
                Dx9EffectResource effectResource = ((Dx9EffectTemplate)this.m_effectTemplate).EffectResource;
                image.GutterSize = effectResource.GutterSize;
            }
            base.SetProperty(stPropertyName, image);
        }

        internal override void SetProperty(string stPropertyName, IImage[] images)
        {
            Dx9EffectResource effectResource = ((Dx9EffectTemplate)this.m_effectTemplate).EffectResource;
            for (int index = 0; index < images.Length; ++index)
            {
                if (images[index] is Image image)
                    image.GutterSize = effectResource.GutterSize;
            }
            base.SetProperty(stPropertyName, images);
        }
    }
}
