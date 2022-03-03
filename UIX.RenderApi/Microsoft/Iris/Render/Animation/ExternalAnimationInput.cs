// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Animation.ExternalAnimationInput
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;

namespace Microsoft.Iris.Render.Animation
{
    internal class ExternalAnimationInput :
      SharedRenderObject,
      IExternalAnimationInput,
      IAnimatableObject,
      IAnimatable,
      ISharedRenderObject,
      IRenderHandleOwner
    {
        private static uint s_uniqueIdSeed = 1;
        private uint m_uniqueId;
        private RemoteExternalAnimationInput m_remoteObject;
        private IAnimationPropertyMap m_propertyMap;
        private RenderSession m_session;

        public ExternalAnimationInput(
          object objUser,
          RenderSession session,
          IAnimationPropertyMap propertyMap)
        {
            Debug2.Validate(objUser != null, typeof(ArgumentNullException), nameof(objUser));
            Debug2.Validate(propertyMap != null, typeof(ArgumentNullException), nameof(propertyMap));
            this.m_uniqueId = s_uniqueIdSeed++;
            this.m_propertyMap = propertyMap;
            this.m_session = session;
            this.m_remoteObject = session.BuildRemoteExternalAnimationInput(this, this.m_uniqueId);
            this.RegisterUsage(objUser);
        }

        protected override void Dispose(bool inDispose)
        {
            try
            {
                if (inDispose && this.m_remoteObject != null)
                    this.m_remoteObject.Dispose();
                this.m_remoteObject = null;
                this.m_propertyMap = null;
                this.m_session = null;
            }
            finally
            {
                base.Dispose(inDispose);
            }
        }

        public uint UniqueId => this.m_uniqueId;

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteObject.RenderHandle;

        void IRenderHandleOwner.OnDisconnect()
        {
            this.m_uniqueId = 0U;
            this.m_remoteObject = null;
        }

        RENDERHANDLE IAnimatableObject.GetObjectId() => this.m_remoteObject.RenderHandle;

        uint IAnimatableObject.GetPropertyId(string propertyName) => this.m_propertyMap.GetPropertyId(propertyName);

        AnimationInputType IAnimatableObject.GetPropertyType(
          string propertyName)
        {
            return this.m_propertyMap.GetPropertyType(propertyName);
        }

        IAnimationInputProvider IExternalAnimationInput.CreateProvider(
          object objUser)
        {
            return new AnimationInputProvider(objUser, this.m_session, this);
        }
    }
}
