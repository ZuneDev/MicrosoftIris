// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Animation.AnimationSystem
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
    internal class AnimationSystem : SharedRenderObject, IAnimationSystem, IRenderHandleOwner
    {
        private RenderSession m_ownerSession;
        private RemoteAnimationManager m_remoteObject;
        private int m_nUpdatesPerSecond;
        private float m_flSpeedAdjustment;
        private bool m_backCompat;

        internal AnimationSystem(RenderSession ownerSession)
        {
            this.m_ownerSession = ownerSession;
            this.m_remoteObject = ownerSession.BuildRemoteAnimationManager(this);
            this.m_flSpeedAdjustment = 1f;
            this.m_backCompat = false;
            this.RegisterUsage(ownerSession);
        }

        protected override void Dispose(bool inDispose)
        {
            try
            {
                if (inDispose && this.m_remoteObject != null)
                    this.m_remoteObject.Dispose();
                this.m_ownerSession = null;
                this.m_remoteObject = null;
            }
            finally
            {
                base.Dispose(inDispose);
            }
        }

        internal RenderSession Session => this.m_ownerSession;

        public bool BackCompat
        {
            get => this.m_backCompat;
            set => this.m_backCompat = value;
        }

        IKeyframeAnimation IAnimationSystem.CreateKeyframeAnimation(
          object objUser,
          AnimationInput initialValue)
        {
            Debug2.Validate(objUser != null, typeof(ArgumentNullException), nameof(objUser));
            Debug2.Validate(initialValue != null, typeof(ArgumentNullException), nameof(initialValue));
            KeyframeAnimation keyframeAnimation = new KeyframeAnimation(this, initialValue);
            keyframeAnimation.RegisterUsage(objUser);
            return keyframeAnimation;
        }

        IAnimationGroup IAnimationSystem.CreateAnimationGroup(
          object objUser)
        {
            Debug2.Validate(objUser != null, typeof(ArgumentNullException), nameof(objUser));
            AnimationGroup animationGroup = new AnimationGroup(this);
            animationGroup.RegisterUsage(objUser);
            return animationGroup;
        }

        void IAnimationSystem.PulseTimeAdvance(int nAdvanceMs) => this.PulseTimeAdvance(nAdvanceMs);

        int IAnimationSystem.UpdatesPerSecond
        {
            get => this.m_nUpdatesPerSecond;
            set
            {
                if (value == this.m_nUpdatesPerSecond)
                    return;
                this.m_nUpdatesPerSecond = value;
                this.m_remoteObject.SendSetAnimationRate(this.m_nUpdatesPerSecond);
            }
        }

        float IAnimationSystem.SpeedAdjustment
        {
            get => this.m_flSpeedAdjustment;
            set
            {
                if (Math2.WithinEpsilon(this.m_flSpeedAdjustment, value))
                    return;
                this.m_flSpeedAdjustment = value;
                this.m_remoteObject.SendSetGlobalSpeedAdjustment(this.m_flSpeedAdjustment);
            }
        }

        IExternalAnimationInput IAnimationSystem.CreateExternalAnimationInput(
          object objUser,
          IAnimationPropertyMap propertyMap)
        {
            return new ExternalAnimationInput(objUser, this.m_ownerSession, propertyMap);
        }

        void IAnimationSystem.PauseAnimations() => this.m_remoteObject.SendSetGlobalSpeedAdjustment(0.0f);

        void IAnimationSystem.StepAnimations(int nAdvanceMs) => this.PulseTimeAdvance(nAdvanceMs);

        void IAnimationSystem.ResumeAnimations() => this.m_remoteObject.SendSetGlobalSpeedAdjustment(this.m_flSpeedAdjustment);

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteObject.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteObject = null;

        protected override void Invariant()
        {
            Debug2.Validate(this.m_remoteObject.IsValid, typeof(InvalidOperationException), "RemoteAnimationManager must be connected");
            this.m_ownerSession.AssertOwningThread();
        }

        private void PulseTimeAdvance(int nAdvanceMs)
        {
            Debug2.Validate(nAdvanceMs > 0, typeof(ArgumentOutOfRangeException), "advanceTime");
            this.m_remoteObject.SendPulseTimeAdvance(nAdvanceMs);
        }
    }
}
