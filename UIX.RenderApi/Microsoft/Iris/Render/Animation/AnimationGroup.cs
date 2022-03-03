// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Animation.AnimationGroup
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render.Animation
{
    internal class AnimationGroup : Microsoft.Iris.Render.Animation.Animation, IAnimationGroup, IAnimation, ISharedRenderObject
    {
        internal AnimationGroup(AnimationSystem owner) => throw new NotImplementedException("Not implemented yet");

        public override int RepeatCount
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override bool IsPlaying => throw new NotImplementedException();

        public override bool IsActive => throw new NotImplementedException();

        public override bool AutoReset
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override AnimationResetBehavior ResetBehavior
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override void Play() => throw new NotImplementedException();

        public override void Pause() => throw new NotImplementedException();

        public override void Reset() => throw new NotImplementedException();

        public override void InstantAdvance(float advanceTime) => throw new NotImplementedException();

        public override void InstantFinish() => throw new NotImplementedException();
    }
}
