// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.ReferenceAnimation
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Animations
{
    internal abstract class ReferenceAnimation : IAnimationProvider
    {
        private IAnimationProvider _sourceAnimation;

        public IAnimationProvider Source
        {
            get => _sourceAnimation;
            set
            {
                if (_sourceAnimation == value)
                    return;
                _sourceAnimation = value;
                OnSourceChanged();
            }
        }

        protected virtual void OnSourceChanged()
        {
        }

        public AnimationEventType Type => _sourceAnimation != null ? _sourceAnimation.Type : AnimationEventType.Idle;

        public AnimationTemplate Build(ref AnimationArgs args) => BuildWorker(ref args);

        protected virtual AnimationTemplate BuildWorker(ref AnimationArgs args) => _sourceAnimation.Build(ref args);

        public override string ToString() => InvariantString.Format("{0}({1})", GetType().Name, Source);

        public virtual bool CanCache => _sourceAnimation == null || _sourceAnimation.CanCache;
    }
}
