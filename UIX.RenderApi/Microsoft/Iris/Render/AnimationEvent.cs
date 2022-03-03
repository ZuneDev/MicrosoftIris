// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.AnimationEvent
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Animation;
using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System;

namespace Microsoft.Iris.Render
{
    public class AnimationEvent
    {
        private static int s_eventIdSeed = 1;
        private uint m_eventId;
        private IActivatableObject m_eventTarget;
        private string m_eventMethodName;
        private uint m_eventMethodArg;
        private bool m_allowRepeat;
        private bool m_initialActivation;

        public AnimationEvent(IActivatable eventTarget, string eventMethodName)
          : this(eventTarget, eventMethodName, 0U)
        {
        }

        public AnimationEvent(IActivatable eventTarget, string eventMethodName, uint eventMethodArg)
        {
            Debug2.Validate(eventTarget != null, typeof(ArgumentNullException), nameof(eventTarget));
            Debug2.Validate(eventTarget is IActivatableObject, typeof(ArgumentException), nameof(eventTarget));
            Debug2.Validate(eventMethodName != null, typeof(ArgumentNullException), nameof(eventMethodName));
            this.m_eventId = AllocateEventId();
            this.m_eventTarget = (IActivatableObject)eventTarget;
            this.m_eventMethodName = eventMethodName;
            this.m_eventMethodArg = eventMethodArg;
            this.m_allowRepeat = false;
        }

        ~AnimationEvent() => this.m_eventTarget = null;

        internal uint EventId => this.m_eventId;

        internal RENDERHANDLE TargetObjectId => this.m_eventTarget.GetObjectId();

        internal uint TargetMethodId => this.m_eventTarget.GetMethodId(this.m_eventMethodName);

        internal uint TargetMethodArg => this.m_eventMethodArg;

        public bool AllowRepeat
        {
            get => this.m_allowRepeat;
            set => this.m_allowRepeat = value;
        }

        public bool InitialActivation
        {
            get => this.m_initialActivation;
            set => this.m_initialActivation = value;
        }

        private static uint AllocateEventId() => (uint)s_eventIdSeed++;
    }
}
