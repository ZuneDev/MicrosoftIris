// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Camera
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Animation;
using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Camera :
      SharedRenderObject,
      ICamera,
      IAnimatableObject,
      IAnimatable,
      ISharedRenderObject,
      IRenderHandleOwner
    {
        public static readonly string EyeProperty = "CameraEye";
        public static readonly string AtProperty = "CameraAt";
        public static readonly string UpProperty = "CameraUp";
        public static readonly string ZnProperty = "CameraZn";
        private RenderSession m_session;
        private RemoteCamera m_remoteCamera;
        private Vector3 m_vEye;
        private Vector3 m_vAt;
        private Vector3 m_vUp;
        private float m_flZn;
        private bool m_fPerspective;

        internal Camera(RenderSession session)
        {
            this.m_session = session;
            Debug2.Validate(this.m_session != null, typeof(ArgumentNullException), "Must have valid RenderSession");
            this.m_session.AssertOwningThread();
            this.m_remoteCamera = this.m_session.BuildRemoteCamera(this);
            this.SetEye(new Vector3(0.0, 0.0, -4.0));
            this.SetAt(new Vector3(0.0, 0.0, 0.0));
            this.SetUp(new Vector3(0.0, 1.0, 0.0));
            this.SetZn(2f);
            this.SetPerspective(true);
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (!fInDispose || this.m_remoteCamera == null)
                    return;
                this.m_remoteCamera.Dispose();
                this.m_remoteCamera = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteCamera.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteCamera = null;

        internal RemoteCamera RemoteStub => this.m_remoteCamera;

        Vector3 ICamera.Eye
        {
            get => this.m_vEye;
            set => this.SetEye(value);
        }

        Vector3 ICamera.At
        {
            get => this.m_vAt;
            set => this.SetAt(value);
        }

        Vector3 ICamera.Up
        {
            get => this.m_vUp;
            set => this.SetUp(value);
        }

        float ICamera.Zn
        {
            get => this.m_flZn;
            set => this.SetZn(value);
        }

        bool ICamera.Perspective
        {
            get => this.m_fPerspective;
            set => this.SetPerspective(value);
        }

        private void SetEye(Vector3 vEye)
        {
            if (!(this.m_vEye != vEye))
                return;
            this.m_vEye = vEye;
            if (!this.m_session.IsValid || !this.m_remoteCamera.IsValid)
                return;
            this.m_remoteCamera.SendSetEye(vEye);
        }

        private void SetAt(Vector3 vAt)
        {
            if (!(this.m_vAt != vAt))
                return;
            this.m_vAt = vAt;
            if (!this.m_session.IsValid || !this.m_remoteCamera.IsValid)
                return;
            this.m_remoteCamera.SendSetAt(vAt);
        }

        private void SetUp(Vector3 vUp)
        {
            if (!(this.m_vUp != vUp))
                return;
            this.m_vUp = vUp;
            if (!this.m_session.IsValid || !this.m_remoteCamera.IsValid)
                return;
            this.m_remoteCamera.SendSetUp(vUp);
        }

        private void SetZn(float flZn)
        {
            if (m_flZn == (double)flZn)
                return;
            this.m_flZn = flZn;
            if (!this.m_session.IsValid || !this.m_remoteCamera.IsValid)
                return;
            this.m_remoteCamera.SendSetZn(flZn);
        }

        private void SetPerspective(bool fPerspective)
        {
            if (this.m_fPerspective == fPerspective)
                return;
            this.m_fPerspective = fPerspective;
            if (!this.m_session.IsValid || !this.m_remoteCamera.IsValid)
                return;
            this.m_remoteCamera.SendSetPerspective(fPerspective);
        }

        RENDERHANDLE IAnimatableObject.GetObjectId() => ((IRenderHandleOwner)this).RenderHandle;

        uint IAnimatableObject.GetPropertyId(string propertyName)
        {
            if (propertyName == EyeProperty)
                return 1;
            if (propertyName == AtProperty)
                return 2;
            if (propertyName == UpProperty)
                return 3;
            if (propertyName == ZnProperty)
                return 4;
            Debug2.Validate(false, typeof(ArgumentException), "Unsupported property");
            return 0;
        }

        AnimationInputType IAnimatableObject.GetPropertyType(
          string propertyName)
        {
            if (propertyName == EyeProperty || propertyName == AtProperty || propertyName == UpProperty)
                return AnimationInputType.Vector3;
            if (propertyName == ZnProperty)
                return AnimationInputType.Float;
            Debug2.Validate(false, typeof(ArgumentException), "Unsupported property");
            return AnimationInputType.Float;
        }

        private enum AnimatableProperties : uint
        {
            Eye = 1,
            At = 2,
            Up = 3,
            Zn = 4,
        }
    }
}
