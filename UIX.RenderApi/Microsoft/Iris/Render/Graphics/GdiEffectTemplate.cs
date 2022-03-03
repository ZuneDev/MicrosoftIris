// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.GdiEffectTemplate
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using System;

namespace Microsoft.Iris.Render.Graphics
{
    internal class GdiEffectTemplate : EffectTemplate
    {
        private GdiEffectType m_type;

        internal GdiEffectTemplate(RenderSession session, GraphicsDevice device, string stName)
          : base(session, device, stName)
          => this.m_type = GdiEffectType.None;

        internal GdiEffectType Type => this.m_type;

        public override IEffect CreateInstance(object objUser)
        {
            Debug2.Validate(this.m_fBuilt, typeof(InvalidOperationException), "Cannot create instance if the template is not built");
            GdiEffect gdiEffect = null;
            if (this.m_cache != null)
                gdiEffect = (GdiEffect)this.m_cache.Remove(objUser);
            if (gdiEffect == null)
            {
                gdiEffect = new GdiEffect(this);
                gdiEffect.Cache = this.m_cache;
                gdiEffect.RegisterUsage(objUser);
                gdiEffect.RemoteEffect();
            }
            return gdiEffect;
        }

        protected override bool Build(EffectInput input)
        {
            if (!this.ProcessEffectElement(input))
                return false;
            int nPropCacheSize = 0;
            if (!this.GenerateProperties(input, this.m_dynamicProperties, out nPropCacheSize))
                return false;
            Map<string, EffectProperty>.Enumerator enumerator = this.m_dynamicProperties.GetEnumerator();
            while (enumerator.MoveNext())
            {
                EffectProperty effectProperty = enumerator.Current.Value;
                if (effectProperty == null || !effectProperty.IsDynamic)
                {
                    this.m_dynamicProperties.Remove(enumerator.Current.Key);
                    enumerator = this.m_dynamicProperties.GetEnumerator();
                }
            }
            return true;
        }

        private bool ProcessEffectElement(EffectInput element)
        {
            switch (element.Type)
            {
                case EffectInputType.Color:
                    this.m_type = GdiEffectType.LoadColor;
                    return true;
                case EffectInputType.Image:
                    this.m_type = GdiEffectType.LoadImage;
                    return true;
                default:
                    return false;
            }
        }

        protected override void ResetInternalState()
        {
            base.ResetInternalState();
            this.m_type = GdiEffectType.None;
        }
    }
}
