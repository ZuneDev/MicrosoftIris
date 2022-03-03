// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9GraphicsDevice
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;

namespace Microsoft.Iris.Render.Graphics
{
    internal abstract class Dx9GraphicsDevice : GraphicsDevice
    {
        private Dx9EffectManager m_effectManager;

        protected Dx9GraphicsDevice(
          RenderSession session,
          string stDisplayName,
          GraphicsDeviceType type,
          GraphicsRenderingQuality renderingQuality)
          : base(session, stDisplayName)
        {
            this.m_deviceType = type;
            this.m_graphicsCaps = session.GetGraphicsCaps(type);
            this.m_renderingQuality = renderingQuality;
            if (this.m_renderingQuality != GraphicsRenderingQuality.MinQuality)
            {
                Dx9GraphicsDevice.PixelShaderProfileType pixelShaderProfile = (Dx9GraphicsDevice.PixelShaderProfileType)this.m_graphicsCaps.PixelShaderProfile;
                if ((byte)this.m_graphicsCaps.VertexShaderProfile < 1 || pixelShaderProfile < PixelShaderProfileType.PS_2_0)
                    this.m_renderingQuality = GraphicsRenderingQuality.MinQuality;
            }
            this.m_sizeGutterPxl = new Size(1, 1);
            this.m_effectManager = new Dx9EffectManager(session, this);
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (!fInDispose || this.m_effectManager == null)
                    return;
                this.m_effectManager.Dispose();
                this.m_effectManager = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        public override bool IsVideoComposited => true;

        public override bool CanPlayAnimations => true;

        internal Dx9EffectManager EffectManager => this.m_effectManager;

        internal string PixelShaderProfile
        {
            get
            {
                switch ((byte)this.m_graphicsCaps.PixelShaderProfile)
                {
                    case 0:
                        return "ps_1_1";
                    case 1:
                        return "ps_1_2";
                    case 2:
                        return "ps_1_3";
                    case 3:
                        return "ps_1_4";
                    case 4:
                        return "ps_2_0";
                    case 5:
                        return "ps_2_a";
                    case 6:
                        return "ps_2_b";
                    case 7:
                        return "ps_3_0";
                    default:
                        return "ps_1_1";
                }
            }
        }

        internal string VertexShaderProfile
        {
            get
            {
                switch ((byte)this.m_graphicsCaps.VertexShaderProfile)
                {
                    case 0:
                        return "vs_1_1";
                    case 1:
                        return "vs_2_0";
                    case 2:
                        return "vs_2_a";
                    case 3:
                        return "vs_3_0";
                    default:
                        return "vs_1_1";
                }
            }
        }

        public override bool CanPlayAnimationType(AnimationInputType type) => true;

        internal override EffectTemplate CreateEffectTemplate(string stName) => new Dx9EffectTemplate(this.m_session, this, stName);

        private enum PixelShaderProfileType : byte
        {
            PS_1_1,
            PS_1_2,
            PS_1_3,
            PS_1_4,
            PS_2_0,
            PS_2_A,
            PS_2_B,
            PS_3_0,
        }

        private enum VertexShaderProfileType : byte
        {
            VS_1_1,
            VS_2_0,
            VS_2_A,
            VS_3_0,
        }
    }
}
