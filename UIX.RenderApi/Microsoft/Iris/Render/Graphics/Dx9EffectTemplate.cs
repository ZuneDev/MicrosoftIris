// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Dx9EffectTemplate
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using System;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Dx9EffectTemplate : EffectTemplate
    {
        private Dx9EffectResource m_effectResource;

        internal Dx9EffectTemplate(RenderSession session, GraphicsDevice device, string stName)
          : base(session, device, stName)
        {
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (!fInDispose || this.m_effectResource == null)
                    return;
                this.m_effectResource.UnregisterUsage(this);
                this.m_effectResource = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        internal Dx9EffectManager EffectManager => ((Dx9GraphicsDevice)this.m_graphicsDevice).EffectManager;

        internal Dx9EffectResource EffectResource => this.m_effectResource;

        protected override bool Build(EffectInput input)
        {
            int nPropCacheSize = 0;
            if (!this.GenerateProperties(input, this.m_dynamicProperties, out nPropCacheSize) || !this.ProcessEffectElement(input, this.m_dynamicProperties, nPropCacheSize))
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

        public override IEffect CreateInstance(object objUser)
        {
            Debug2.Validate(this.m_fBuilt, typeof(InvalidOperationException), "Cannot create instance if the template is not built");
            Dx9Effect dx9Effect = null;
            if (this.m_cache != null)
                dx9Effect = (Dx9Effect)this.m_cache.Remove(objUser);
            if (dx9Effect == null)
            {
                dx9Effect = new Dx9Effect(this);
                dx9Effect.Cache = this.m_cache;
                dx9Effect.RegisterUsage(objUser);
                dx9Effect.RemoteEffect();
            }
            return dx9Effect;
        }

        private bool ProcessEffectElement(
          EffectInput element,
          Map<string, EffectProperty> allProps,
          int nPropCacheSize)
        {
            if (!(this.m_graphicsDevice as Dx9GraphicsDevice).EffectManager.CreateEffectResource(this.m_stName, element, nPropCacheSize, out this.m_effectResource))
                return false;
            this.m_effectResource.RegisterUsage(this);
            foreach (KeyValueEntry<string, EffectProperty> allProp in allProps)
            {
                EffectProperty effectProperty = allProp.Value;
                if (effectProperty.Type == EffectPropertyType.Image && effectProperty.Value != null)
                    ((Image)effectProperty.Value).GutterSize = this.m_effectResource.GutterSize;
            }
            return true;
        }
    }
}
