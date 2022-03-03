// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IAnimationSystem
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public interface IAnimationSystem
    {
        int UpdatesPerSecond { get; set; }

        float SpeedAdjustment { get; set; }

        bool BackCompat { set; }

        IKeyframeAnimation CreateKeyframeAnimation(
          object objUser,
          AnimationInput initialValue);

        IAnimationGroup CreateAnimationGroup(object objUser);

        IExternalAnimationInput CreateExternalAnimationInput(
          object objUser,
          IAnimationPropertyMap propertyMap);

        void PulseTimeAdvance(int nAdvanceMs);

        void PauseAnimations();

        void StepAnimations(int nAdvanceMs);

        void ResumeAnimations();
    }
}
