// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Animation.AnimationTarget
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System.Threading;

namespace Microsoft.Iris.Render.Animation
{
    internal class AnimationTarget : RenderObject
    {
        private static int s_targetIdSeed;
        private int m_targetId;
        private IAnimatableObject m_object;
        private string m_property;

        internal AnimationTarget(IAnimatableObject targetObject, string targetPropertyName)
        {
            int propertyType = (int)targetObject.GetPropertyType(targetPropertyName);
            targetObject.RegisterUsage(this);
            this.m_targetId = Interlocked.Increment(ref s_targetIdSeed);
            this.m_object = targetObject;
            this.m_property = targetPropertyName;
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose)
                    this.m_object.UnregisterUsage(this);
                this.m_object = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        internal int TargetId => this.m_targetId;

        internal IAnimatableObject TargetObject => this.m_object;

        internal string TargetPropertyName => this.m_property;

        internal AnimationInputType TargetPropertyType => this.m_object.GetPropertyType(this.m_property);
    }
}
