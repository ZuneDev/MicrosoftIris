// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.EffectTemplate
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using System;

namespace Microsoft.Iris.Render.Graphics
{
    internal abstract class EffectTemplate : SharedRenderObject, IEffectTemplate, ISharedRenderObject
    {
        protected string m_stName;
        protected GraphicsDevice m_graphicsDevice;
        protected RenderSession m_session;
        protected Map<string, EffectProperty> m_dynamicProperties;
        protected bool m_fBuilt;
        protected ObjectCache m_cache;
        private bool m_fReleasingCache;

        internal EffectTemplate(RenderSession session, GraphicsDevice device, string stName)
        {
            this.m_dynamicProperties = new Map<string, EffectProperty>(10);
            this.m_graphicsDevice = device;
            this.m_session = session;
            this.m_fBuilt = false;
            this.m_stName = stName;
            ObjectCacheManager cacheManager = session.CacheManager;
            if (cacheManager == null)
                return;
            this.m_cache = new ObjectCache(100, 300, 100);
            this.m_cache.DebugDescription = stName;
            cacheManager.RegisterCache(this.m_cache);
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (!fInDispose)
                    return;
                if (this.m_dynamicProperties != null)
                {
                    foreach (KeyValueEntry<string, EffectProperty> dynamicProperty in this.m_dynamicProperties)
                    {
                        EffectProperty effectProperty = dynamicProperty.Value;
                        if (effectProperty != null && effectProperty.Value != null && effectProperty.IsSharedResource)
                            ((SharedRenderObject)effectProperty.Value).UnregisterUsage(this);
                        effectProperty.Dispose();
                    }
                    this.m_dynamicProperties = null;
                }
                if (this.m_cache == null)
                    return;
                this.m_session.CacheManager.UnregisterCache(this.m_cache);
                this.m_cache.Dispose();
                this.m_cache = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        internal override void UnregisterUsage(object objUser)
        {
            if (!this.m_fReleasingCache && this.m_cache != null && this.UsageCount == this.m_cache.Size + 1)
            {
                this.m_fReleasingCache = true;
                this.m_cache.RemoveAll();
                this.m_fReleasingCache = false;
            }
            base.UnregisterUsage(objUser);
        }

        internal GraphicsDevice Device => this.m_graphicsDevice;

        internal RenderSession Session => this.m_session;

        internal Map<string, EffectProperty> DynamicProperties => this.m_dynamicProperties;

        public bool IsBuilt => this.m_fBuilt;

        public string Name => this.m_stName;

        void IEffectTemplate.AddEffectProperty(string stProp) => this.m_dynamicProperties.Add(stProp, null);

        bool IEffectTemplate.Build(EffectInput input)
        {
            Debug2.Validate(input != null, typeof(ArgumentNullException), "IEffectTemplate.Build null argument");
            Debug2.Validate(!this.m_fBuilt, typeof(ArgumentNullException), "IEffectTemplate.Build called twice");
            if (!this.MeetQualityBar(input))
            {
                this.ResetInternalState();
                return false;
            }
            if (!this.Build(input))
            {
                this.ResetInternalState();
                return false;
            }
            foreach (KeyValueEntry<string, EffectProperty> dynamicProperty in this.m_dynamicProperties)
            {
                EffectProperty effectProperty = dynamicProperty.Value;
                if (effectProperty != null && effectProperty.Value != null && effectProperty.IsSharedResource)
                    ((SharedRenderObject)effectProperty.Value).RegisterUsage(this);
            }
            this.m_fBuilt = true;
            return true;
        }

        protected virtual void ResetInternalState()
        {
            foreach (KeyValueEntry<string, EffectProperty> dynamicProperty in this.m_dynamicProperties)
                dynamicProperty.Value?.Dispose();
            this.m_dynamicProperties.Clear();
            this.m_fBuilt = false;
        }

        protected bool GenerateProperties(
          EffectInput input,
          Map<string, EffectProperty> dictionary,
          out int nPropCacheSize)
        {
            byte nNextUniqueID = 0;
            nPropCacheSize = input.PreProcessProperties(dictionary, ref nNextUniqueID);
            return input.Process(dictionary);
        }

        private bool MeetQualityBar(EffectInput input) => this.m_graphicsDevice.RenderingQuality != GraphicsRenderingQuality.MinQuality || input.Type == EffectInputType.Color || (input.Type == EffectInputType.Image || input.Type == EffectInputType.Video);

        protected abstract bool Build(EffectInput element);

        public abstract IEffect CreateInstance(object objUser);
    }
}
