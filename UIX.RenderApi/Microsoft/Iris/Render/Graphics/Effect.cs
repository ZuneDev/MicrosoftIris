// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Effect
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Animation;
using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;

namespace Microsoft.Iris.Render.Graphics
{
    internal abstract class Effect :
      CachedRenderObject,
      IEffect,
      IAnimatableObject,
      IAnimatable,
      ISharedRenderObject,
      IRenderHandleOwner
    {
        protected Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteEffect m_remoteEffect;
        protected EffectTemplate m_effectTemplate;
        protected Vector<EffectProperty> m_propertyCache;
        private static RemoteSurface[] s_EmptySurfaceArray = new RemoteSurface[0];

        internal Effect(EffectTemplate effectTemplate)
        {
            this.m_effectTemplate = effectTemplate;
            this.m_effectTemplate.RegisterUsage(this);
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (!fInDispose)
                    return;
                if (this.m_remoteEffect != null)
                    this.m_remoteEffect.Dispose();
                if (this.m_propertyCache != null)
                {
                    foreach (EffectProperty effectProperty in this.m_propertyCache)
                    {
                        switch (effectProperty.Type)
                        {
                            case EffectPropertyType.Image:
                                this.FreeResource((Image)effectProperty.Value);
                                break;
                            case EffectPropertyType.ImageArray:
                                this.FreeResource((SharedResourceArray)effectProperty.Value);
                                break;
                            case EffectPropertyType.Video:
                                this.FreeResource((VideoStream)effectProperty.Value);
                                break;
                        }
                        effectProperty.Dispose();
                    }
                }
                this.m_effectTemplate.UnregisterUsage(this);
                this.m_effectTemplate = null;
                this.m_propertyCache = null;
                this.m_remoteEffect = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteEffect = null;

        internal Microsoft.Iris.Render.Protocols.Splash.Rendering.RemoteEffect RemoteStub => this.m_remoteEffect;

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteEffect.RenderHandle;

        public string Name => this.m_effectTemplate.Name;

        public IEffectTemplate Template => m_effectTemplate;

        void IEffect.SetProperty(string stPropertyName, int nValue) => this.SetProperty(stPropertyName, nValue);

        void IEffect.SetProperty(string stPropertyName, float flValue) => this.SetProperty(stPropertyName, flValue);

        void IEffect.SetProperty(string stPropertyName, Vector2 vValue) => this.SetProperty(stPropertyName, vValue);

        void IEffect.SetProperty(string stPropertyName, Vector3 vValue) => this.SetProperty(stPropertyName, vValue);

        void IEffect.SetProperty(string stPropertyName, Vector4 vValue) => this.SetProperty(stPropertyName, vValue);

        void IEffect.SetProperty(string stPropertyName, IImage image) => this.SetProperty(stPropertyName, (Image)image);

        void IEffect.SetProperty(string stPropertyName, IImage[] images) => this.SetProperty(stPropertyName, images);

        void IEffect.SetProperty(string stPropertyName, ColorF color) => this.SetProperty(stPropertyName, color);

        void IEffect.SetProperty(string stPropertyName, IVideoStream videoStream) => this.SetProperty(stPropertyName, videoStream as VideoStream);

        RENDERHANDLE IAnimatableObject.GetObjectId() => this.RemoteStub.RenderHandle;

        uint IAnimatableObject.GetPropertyId(string propertyName)
        {
            Debug2.Validate(propertyName != null, typeof(ArgumentNullException), nameof(propertyName));
            Map<string, EffectProperty> dynamicProperties = this.m_effectTemplate.DynamicProperties;
            if (dynamicProperties.ContainsKey(propertyName))
                return this.GenerateAnimationPropertyId(dynamicProperties[propertyName]);
            Debug2.Validate(false, typeof(ArgumentException), "Unsupported property: ", propertyName);
            return 0;
        }

        AnimationInputType IAnimatableObject.GetPropertyType(
          string propertyName)
        {
            Debug2.Validate(propertyName != null, typeof(ArgumentNullException), nameof(propertyName));
            Map<string, EffectProperty> dynamicProperties = this.m_effectTemplate.DynamicProperties;
            if (dynamicProperties.ContainsKey(propertyName))
                return this.MapEffectPropertyTypeToAnimationType(dynamicProperties[propertyName]);
            Debug2.Validate(false, typeof(ArgumentException), "Unsupported property: ", propertyName);
            return AnimationInputType.Float;
        }

        private uint GenerateAnimationPropertyId(EffectProperty effectProperty)
        {
            uint num1 = 7;
            uint num2 = 16777215;
            Debug2.Validate((effectProperty.Type & (EffectPropertyType)~(int)num1) == EffectPropertyType.Integer, typeof(ArgumentException), "Unsupported effect property type: {0}", effectProperty.Type);
            Debug2.Validate((effectProperty.ID & ~(int)num2) == 0, typeof(ArgumentException), "Only 24-bits of effect property IDs are supported");
            return (uint)(effectProperty.Type & (EffectPropertyType)num1) << 28 | effectProperty.ID & num2;
        }

        private AnimationInputType MapEffectPropertyTypeToAnimationType(
          EffectProperty effectProperty)
        {
            AnimationInputType animationInputType;
            switch (effectProperty.Type)
            {
                case EffectPropertyType.Integer:
                case EffectPropertyType.Float:
                    animationInputType = AnimationInputType.Float;
                    break;
                case EffectPropertyType.Vector2:
                    animationInputType = AnimationInputType.Vector2;
                    break;
                case EffectPropertyType.Vector3:
                    animationInputType = AnimationInputType.Vector3;
                    break;
                case EffectPropertyType.Vector4:
                    animationInputType = AnimationInputType.Vector4;
                    break;
                case EffectPropertyType.Color:
                    animationInputType = AnimationInputType.Vector4;
                    break;
                default:
                    Debug2.Throw(false, "{0} is not supported as animatable property", effectProperty.Type);
                    animationInputType = AnimationInputType.Float;
                    break;
            }
            return animationInputType;
        }

        internal void SetProperty(string stPropertyName, int nValue)
        {
            EffectProperty effectProperty;
            if (!this.IsValidProperty(stPropertyName, EffectPropertyType.Integer, out effectProperty))
                return;
            this.RemoteProperty(effectProperty, nValue);
        }

        internal void SetProperty(string stPropertyName, float flValue)
        {
            EffectProperty effectProperty;
            if (!this.IsValidProperty(stPropertyName, EffectPropertyType.Float, out effectProperty))
                return;
            this.RemoteProperty(effectProperty, flValue);
        }

        internal void SetProperty(string stPropertyName, Vector2 vValue)
        {
            EffectProperty effectProperty;
            if (!this.IsValidProperty(stPropertyName, EffectPropertyType.Vector2, out effectProperty))
                return;
            this.RemoteProperty(effectProperty, vValue);
        }

        internal void SetProperty(string stPropertyName, Vector3 vValue)
        {
            EffectProperty effectProperty;
            if (!this.IsValidProperty(stPropertyName, EffectPropertyType.Vector3, out effectProperty))
                return;
            this.RemoteProperty(effectProperty, vValue);
        }

        internal void SetProperty(string stPropertyName, Vector4 vValue)
        {
            EffectProperty effectProperty;
            if (!this.IsValidProperty(stPropertyName, EffectPropertyType.Vector4, out effectProperty))
                return;
            this.RemoteProperty(effectProperty, vValue);
        }

        internal void SetProperty(string stPropertyName, ColorF color)
        {
            EffectProperty effectProperty;
            if (!this.IsValidProperty(stPropertyName, EffectPropertyType.Color, out effectProperty))
                return;
            this.RemoteProperty(effectProperty, color);
        }

        internal virtual void SetProperty(string stPropertyName, Image image)
        {
            EffectProperty propertyFromCache = this.GetPropertyFromCache(stPropertyName);
            Debug2.Validate(propertyFromCache != null, typeof(InvalidOperationException), "Invalid property name");
            Debug2.Validate(propertyFromCache.Type == EffectPropertyType.Image || propertyFromCache.Type == EffectPropertyType.ImageArray, typeof(InvalidOperationException), "Invalid property type");
            if (propertyFromCache.Type == EffectPropertyType.Image)
            {
                if (propertyFromCache.HasIdenticalValue(image))
                    return;
                this.SetSharedResourceProperty(propertyFromCache, image);
            }
            else
            {
                if (propertyFromCache.Value != null && (Image)((SharedResourceArray)propertyFromCache.Value).Resources[0] == image)
                    return;
                this.SetSharedResourceProperty(propertyFromCache, new SharedResourceArray(image));
            }
        }

        internal virtual void SetProperty(string stPropertyName, IImage[] images)
        {
            Debug2.Validate(images != null, typeof(ArgumentNullException), nameof(images));
            Debug2.Validate(images.Length > 0, typeof(ArgumentException), "images.Length");
            EffectProperty propertyFromCache = this.GetPropertyFromCache(stPropertyName);
            Debug2.Validate(propertyFromCache != null, typeof(InvalidOperationException), "Invalid property name");
            Debug2.Validate(propertyFromCache.Type == EffectPropertyType.ImageArray, typeof(InvalidOperationException), "Invalid property type");
            SharedResource[] rgResources = new SharedResource[images.Length];
            for (int index = 0; index < images.Length; ++index)
                rgResources[index] = (SharedResource)images[index];
            SharedResourceArray oValue = new SharedResourceArray(rgResources);
            oValue.RegisterUsage(this);
            if (propertyFromCache.HasIdenticalValue(oValue))
            {
                oValue.UnregisterUsage(this);
            }
            else
            {
                this.SetSharedResourceProperty(propertyFromCache, oValue);
                oValue.UnregisterUsage(this);
            }
        }

        internal void SetProperty(string stPropertyName, VideoStream videoStream)
        {
            Debug2.Validate(videoStream != null, typeof(ArgumentNullException), "VideoStream");
            Debug2.Validate(videoStream.Surface != null, typeof(ArgumentException), "VideoStream.Surface");
            EffectProperty propertyFromCache = this.GetPropertyFromCache(stPropertyName);
            Debug2.Validate(propertyFromCache != null, typeof(InvalidOperationException), "Invalid property name");
            Debug2.Validate(propertyFromCache.Type == EffectPropertyType.Video, typeof(InvalidOperationException), "Invalid property type");
            if (propertyFromCache.HasIdenticalValue(videoStream))
                return;
            this.SetSharedResourceProperty(propertyFromCache, videoStream);
        }

        private bool IsValidProperty(
          string stPropertyName,
          EffectPropertyType type,
          out EffectProperty effectProperty)
        {
            EffectProperty dynamicProperty = this.m_effectTemplate.DynamicProperties[stPropertyName];
            Debug2.Validate(dynamicProperty != null, typeof(InvalidOperationException), "Invalid property name");
            Debug2.Validate(dynamicProperty.Type == type, typeof(InvalidOperationException), "Invalid property type");
            effectProperty = dynamicProperty;
            return true;
        }

        private void SetSharedResourceProperty(EffectProperty property, SharedResourceArray oValue)
        {
            Debug2.Validate(property.Type == EffectPropertyType.ImageArray || property.Type == EffectPropertyType.Image, typeof(InvalidOperationException), "Not an Image type");
            this.FreeResource(property.Value as SharedResourceArray);
            property.Value = oValue;
            this.AcquireNewResource(oValue);
            this.RemoteProperty(property, oValue);
        }

        private void SetSharedResourceProperty(EffectProperty property, Image oValue)
        {
            Debug2.Validate(property.Type == EffectPropertyType.Image, typeof(InvalidOperationException), "Not an Image type");
            this.FreeResource(property.Value as Image);
            property.Value = oValue;
            this.AcquireNewResource(oValue);
            this.RemoteProperty(property, oValue);
        }

        private void SetSharedResourceProperty(EffectProperty property, VideoStream oValue)
        {
            Debug2.Validate(property.Type == EffectPropertyType.Video, typeof(InvalidOperationException), "Not a Video type");
            this.FreeResource(property.Value as VideoStream);
            property.Value = oValue;
            this.AcquireNewResource(oValue);
            this.RemoteProperty(property, oValue);
        }

        private EffectProperty GetPropertyFromCache(string propertyName)
        {
            Map<string, EffectProperty> dynamicProperties = this.m_effectTemplate.DynamicProperties;
            if (!dynamicProperties.ContainsKey(propertyName))
                Debug2.Validate(false, typeof(InvalidOperationException), "Invalid property name", propertyName);
            EffectProperty effectProperty = dynamicProperties[propertyName];
            Debug2.Validate(effectProperty.IsSharedResource, typeof(InvalidOperationException), "Invalid property type");
            return this.GetPropertyFromCache(effectProperty.ID);
        }

        private EffectProperty GetPropertyFromCache(int nPropertyId)
        {
            foreach (EffectProperty effectProperty in this.m_propertyCache)
            {
                if (effectProperty.ID == nPropertyId)
                    return effectProperty;
            }
            return null;
        }

        private void RemoteProperty(EffectProperty property, object oValue)
        {
            switch (property.Type)
            {
                case EffectPropertyType.Integer:
                    this.m_remoteEffect.SendSetProperty(property.ID, (int)oValue);
                    break;
                case EffectPropertyType.Float:
                    this.m_remoteEffect.SendSetProperty(property.ID, (float)oValue);
                    break;
                case EffectPropertyType.Vector2:
                    this.m_remoteEffect.SendSetProperty(property.ID, (Vector2)oValue);
                    break;
                case EffectPropertyType.Vector3:
                    this.m_remoteEffect.SendSetProperty(property.ID, (Vector3)oValue);
                    break;
                case EffectPropertyType.Vector4:
                    this.m_remoteEffect.SendSetProperty(property.ID, (Vector4)oValue);
                    break;
                case EffectPropertyType.Image:
                    Image image = (Image)oValue;
                    RemoteSurface surface1 = null;
                    if (image != null && image.Surface != null)
                        surface1 = image.Surface.RemoteStub;
                    this.m_remoteEffect.SendSetProperty(property.ID, surface1);
                    break;
                case EffectPropertyType.Color:
                    this.m_remoteEffect.SendSetProperty(property.ID, ((ColorF)oValue).ToVector4());
                    break;
                case EffectPropertyType.ImageArray:
                    SharedResource[] resources = ((SharedResourceArray)oValue).Resources;
                    RemoteSurface[] surfaceArray = new RemoteSurface[resources.Length];
                    bool flag = true;
                    for (int index = 0; index < resources.Length; ++index)
                    {
                        if (resources[index] == null || ((Image)resources[index]).Surface == null)
                        {
                            flag = false;
                            break;
                        }
                        surfaceArray[index] = ((Image)resources[index]).Surface.RemoteStub;
                    }
                    if (flag)
                    {
                        this.m_remoteEffect.SendSetProperty(property.ID, surfaceArray);
                        break;
                    }
                    this.m_remoteEffect.SendSetProperty(property.ID, s_EmptySurfaceArray);
                    break;
                case EffectPropertyType.Video:
                    VideoStream videoStream = (VideoStream)oValue;
                    RemoteSurface surface2 = null;
                    if (videoStream != null && videoStream.Surface != null)
                        surface2 = videoStream.Surface.RemoteStub;
                    this.m_remoteEffect.SendSetProperty(property.ID, surface2);
                    break;
                default:
                    Debug2.Throw(false, "Unknown property type");
                    break;
            }
        }

        private void RemoteProperty(EffectProperty property, float flValue) => this.m_remoteEffect.SendSetProperty(property.ID, flValue);

        private void RemoteProperty(EffectProperty property, int nValue) => this.m_remoteEffect.SendSetProperty(property.ID, nValue);

        private void RemoteProperty(EffectProperty property, Vector2 vValue) => this.m_remoteEffect.SendSetProperty(property.ID, vValue);

        private void RemoteProperty(EffectProperty property, Vector3 vValue) => this.m_remoteEffect.SendSetProperty(property.ID, vValue);

        private void RemoteProperty(EffectProperty property, Vector4 vValue) => this.m_remoteEffect.SendSetProperty(property.ID, vValue);

        private void RemoteProperty(EffectProperty property, ColorF color) => this.m_remoteEffect.SendSetProperty(property.ID, color.ToVector4());

        private void AddSharedResourcesToCache()
        {
            foreach (EffectProperty property in this.m_effectTemplate.DynamicProperties.Values)
            {
                switch (property.Type)
                {
                    case EffectPropertyType.Image:
                        this.AcquireNewResource(property.Value as Image);
                        this.AddPropertyToCache(property);
                        continue;
                    case EffectPropertyType.ImageArray:
                        this.AcquireNewResource(property.Value as SharedResourceArray);
                        this.AddPropertyToCache(property);
                        continue;
                    case EffectPropertyType.Video:
                        this.AcquireNewResource(property.Value as VideoStream);
                        this.AddPropertyToCache(property);
                        continue;
                    default:
                        continue;
                }
            }
        }

        private void AddPropertyToCache(EffectProperty property)
        {
            if (this.m_propertyCache == null)
                this.m_propertyCache = new Vector<EffectProperty>(3);
            this.m_propertyCache.Add(property.Clone());
        }

        protected void Initialize()
        {
            Debug2.Validate(this.m_propertyCache == null, typeof(InvalidOperationException), "Initialize() should be called once");
            this.AddSharedResourcesToCache();
            if (this.m_propertyCache != null)
                this.m_propertyCache.TrimExcess();
            this.CommitTemplateDefaults(false);
        }

        private void CommitTemplateDefaults(bool fUpdateCache)
        {
            foreach (EffectProperty property in this.m_effectTemplate.DynamicProperties.Values)
            {
                if (fUpdateCache && property.IsSharedResource)
                {
                    EffectProperty propertyFromCache = this.GetPropertyFromCache(property.ID);
                    switch (property.Type)
                    {
                        case EffectPropertyType.Image:
                            this.SetSharedResourceProperty(propertyFromCache, (Image)property.Value);
                            break;
                        case EffectPropertyType.ImageArray:
                            this.SetSharedResourceProperty(propertyFromCache, (SharedResourceArray)property.Value);
                            break;
                        case EffectPropertyType.Video:
                            this.SetSharedResourceProperty(propertyFromCache, (VideoStream)property.Value);
                            break;
                    }
                }
                this.RemoteProperty(property, property.Value);
            }
        }

        private void AcquireNewResource(Image image)
        {
            if (image == null)
                return;
            image.RegisterUsage(this);
            image.ContentReloadedEvent += new Image.ContentReloadedHandler(this.ImageContentReloadedHandler);
            image.AddActiveUser(this);
        }

        private void AcquireNewResource(SharedResourceArray imageArray)
        {
            if (imageArray == null)
                return;
            imageArray.RegisterUsage(this);
            foreach (SharedResource resource in imageArray.Resources)
                (resource as Image).ContentReloadedEvent += new Image.ContentReloadedHandler(this.ImageContentReloadedHandler);
            imageArray.AddActiveUser(this);
        }

        private void AcquireNewResource(VideoStream videoStream)
        {
            if (videoStream == null)
                return;
            videoStream.RegisterUsage(this);
            videoStream.AddActiveUser(this);
        }

        internal void FreeResource(Image image)
        {
            if (image == null)
                return;
            image.ContentReloadedEvent -= new Image.ContentReloadedHandler(this.ImageContentReloadedHandler);
            image.RemoveActiveUser(this);
            image.UnregisterUsage(this);
        }

        internal void FreeResource(SharedResourceArray imageArray)
        {
            if (imageArray == null)
                return;
            foreach (SharedResource resource in imageArray.Resources)
                (resource as Image).ContentReloadedEvent -= new Image.ContentReloadedHandler(this.ImageContentReloadedHandler);
            imageArray.RemoveActiveUser(this);
            imageArray.UnregisterUsage(this);
        }

        internal void FreeResource(VideoStream videoStream)
        {
            if (videoStream == null)
                return;
            videoStream.RemoveActiveUser(this);
            videoStream.UnregisterUsage(this);
        }

        internal void ImageContentReloadedHandler(Image image)
        {
            bool condition = false;
            foreach (EffectProperty property in this.m_propertyCache)
            {
                if (property.Type == EffectPropertyType.Image)
                {
                    if ((Image)property.Value == image)
                    {
                        condition = true;
                        this.RemoteProperty(property, property.Value);
                    }
                }
                else if (property.Type == EffectPropertyType.ImageArray)
                {
                    SharedResource[] resources = ((SharedResourceArray)property.Value).Resources;
                    for (int index = 0; index < resources.Length; ++index)
                    {
                        if (resources[index] != null && (Image)resources[index] == image)
                        {
                            condition = true;
                            this.RemoteProperty(property, property.Value);
                        }
                    }
                }
            }
            Debug2.Validate(condition, typeof(ArgumentException), "Could not find an Image property");
        }

        internal abstract void RemoteEffect();

        internal override void Reset() => this.CommitTemplateDefaults(true);
    }
}
