// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Animation.AnimationInputProvider
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
    internal class AnimationInputProvider :
      SharedRenderObject,
      IAnimationInputProvider,
      IAnimatable,
      ISharedRenderObject,
      IRenderHandleOwner
    {
        private RemoteAnimationInputProvider m_remoteObject;
        private ExternalAnimationInput m_externalInput;

        internal AnimationInputProvider(
          object objUser,
          RenderSession session,
          ExternalAnimationInput externalInput)
        {
            Debug2.Validate(externalInput != null, typeof(ArgumentNullException), nameof(externalInput));
            this.m_remoteObject = session.BuildRemoteAnimationInputProvider(this, externalInput.UniqueId);
            this.m_externalInput = externalInput;
            this.RegisterUsage(objUser);
        }

        protected override void Dispose(bool inDispose)
        {
            try
            {
                if (inDispose && this.m_remoteObject != null)
                    this.m_remoteObject.Dispose();
                this.m_remoteObject = null;
                this.m_externalInput = null;
            }
            finally
            {
                base.Dispose(inDispose);
            }
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteObject.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteObject = null;

        void IAnimationInputProvider.PublishFloat(
          string propertyName,
          float value)
        {
            this.m_remoteObject.SendPublishFloat(((IAnimatableObject)this.m_externalInput).GetPropertyId(propertyName), value);
        }

        void IAnimationInputProvider.PublishVector2(
          string propertyName,
          Vector2 value)
        {
            this.m_remoteObject.SendPublishVector2(((IAnimatableObject)this.m_externalInput).GetPropertyId(propertyName), value);
        }

        void IAnimationInputProvider.PublishVector3(
          string propertyName,
          Vector3 value)
        {
            this.m_remoteObject.SendPublishVector3(((IAnimatableObject)this.m_externalInput).GetPropertyId(propertyName), value);
        }

        void IAnimationInputProvider.PublishVector4(
          string propertyName,
          Vector4 value)
        {
            this.m_remoteObject.SendPublishVector4(((IAnimatableObject)this.m_externalInput).GetPropertyId(propertyName), value);
        }

        void IAnimationInputProvider.PublishQuaternion(
          string propertyName,
          Quaternion value)
        {
            this.m_remoteObject.SendPublishQuaternion(((IAnimatableObject)this.m_externalInput).GetPropertyId(propertyName), value);
        }

        void IAnimationInputProvider.RevokeFloat(string propertyName) => this.m_remoteObject.SendRevokeFloat(((IAnimatableObject)this.m_externalInput).GetPropertyId(propertyName));

        void IAnimationInputProvider.RevokeVector2(string propertyName) => this.m_remoteObject.SendRevokeVector2(((IAnimatableObject)this.m_externalInput).GetPropertyId(propertyName));

        void IAnimationInputProvider.RevokeVector3(string propertyName) => this.m_remoteObject.SendRevokeVector3(((IAnimatableObject)this.m_externalInput).GetPropertyId(propertyName));

        void IAnimationInputProvider.RevokeVector4(string propertyName) => this.m_remoteObject.SendRevokeVector4(((IAnimatableObject)this.m_externalInput).GetPropertyId(propertyName));

        void IAnimationInputProvider.RevokeQuaternion(string propertyName) => this.m_remoteObject.SendRevokeQuaternion(((IAnimatableObject)this.m_externalInput).GetPropertyId(propertyName));
    }
}
