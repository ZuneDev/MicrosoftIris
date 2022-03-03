// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Animation.ObjectAnimationInput
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System;

namespace Microsoft.Iris.Render.Animation
{
    public abstract class ObjectAnimationInput : AnimationInput
    {
        private IAnimatableObject m_object;
        private string m_propertyName;

        protected ObjectAnimationInput(
          IAnimatable sourceObject,
          string sourcePropertyName,
          string sourceMaskSpec)
        {
            Debug2.Validate(sourceObject is IAnimatableObject, typeof(ArgumentException), nameof(sourceObject));
            Debug2.Validate(sourcePropertyName != null, typeof(ArgumentException), nameof(sourcePropertyName));
            AnimationTypeMask sourceMask = AnimationTypeMask.FromString(sourceMaskSpec);
            this.m_object = (IAnimatableObject)sourceObject;
            this.m_propertyName = sourcePropertyName;
            this.CommonCreate(this.m_object.GetPropertyType(this.m_propertyName), sourceMask);
        }

        protected ObjectAnimationInput(IAnimation sourceAnimation, string sourceMaskSpec)
        {
            Debug2.Validate(sourceAnimation is IAnimatableObject, typeof(ArgumentException), nameof(sourceAnimation));
            AnimationTypeMask sourceMask = AnimationTypeMask.FromString(sourceMaskSpec);
            this.m_object = (IAnimatableObject)sourceAnimation;
            this.m_propertyName = Animation.OutputProperty;
            this.CommonCreate(this.m_object.GetPropertyType(this.m_propertyName), sourceMask);
        }

        internal RENDERHANDLE ObjectId => this.m_object.GetObjectId();

        internal uint PropertyId => this.m_object.GetPropertyId(this.m_propertyName);

        internal override void RegisterUsage(object user)
        {
            Debug2.Validate(this.m_object != null, typeof(ObjectDisposedException), "Source object doesn't exist anymore");
            this.m_object.RegisterUsage(user);
            base.RegisterUsage(user);
        }

        internal override void UnregisterUsage(object user)
        {
            Debug2.Validate(this.m_object != null, typeof(ObjectDisposedException), "Source object doesn't exist anymore");
            bool flag = this.m_object.UsageCount == 1;
            this.m_object.UnregisterUsage(user);
            if (flag)
                this.m_object = null;
            base.UnregisterUsage(user);
        }
    }
}
