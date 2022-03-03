// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.RelativeTo
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris.Animations
{
    internal class RelativeTo
    {
        private IAnimatable _sourceObject;
        private int _sourceId;
        private string _sourceProperty;
        private SnapshotPolicy _snapshot;
        private int _power = 1;
        private float _multiply = 1f;
        private float _add;
        private static RelativeTo s_absolute = new RelativeTo();
        private static RelativeTo s_current = new RelativeTo(SnapshotPolicy.Once);
        private static RelativeTo s_currentSnapshotOnLoop = new RelativeTo(SnapshotPolicy.OnLoop);
        private static RelativeTo s_final = new RelativeTo();

        public RelativeTo() => _snapshot = SnapshotPolicy.Continuous;

        public RelativeTo(SnapshotPolicy snapshot) => _snapshot = snapshot;

        public bool IsRelativeToObject => _sourceObject != null || _sourceId != 0 || this == s_current || this == s_currentSnapshotOnLoop;

        public IAnimatable Source
        {
            get => _sourceObject;
            set => _sourceObject = value;
        }

        public int SourceId
        {
            get => _sourceId;
            set => _sourceId = value;
        }

        public string Property
        {
            get => _sourceProperty;
            set => _sourceProperty = value;
        }

        public SnapshotPolicy Snapshot
        {
            get => _snapshot;
            set => _snapshot = value;
        }

        public int Power
        {
            get => _power;
            set => _power = value;
        }

        public float Multiply
        {
            get => _multiply;
            set => _multiply = value;
        }

        public float Add
        {
            get => _add;
            set => _add = value;
        }

        public static RelativeTo Absolute => s_absolute;

        public static RelativeTo Current => s_current;

        public static RelativeTo CurrentSnapshotOnLoop => s_currentSnapshotOnLoop;

        public static RelativeTo Final => s_final;

        public AnimationInput CreateAnimationInput(
          IAnimatable defaultSource,
          string defaultSourceProperty,
          string defaultSourcePropertyMask)
        {
            IAnimatable sourceObject = null;
            string sourcePropertyName = null;
            string sourceMaskSpec = null;
            bool flag = true;
            if (_sourceProperty != null)
            {
                sourceObject = _sourceId == 0 ? _sourceObject : Application.MapExternalAnimationInput(_sourceId);
                if (sourceObject != null)
                {
                    sourcePropertyName = _sourceProperty;
                    flag = false;
                }
            }
            if (flag)
            {
                sourceObject = defaultSource;
                sourcePropertyName = defaultSourceProperty;
                sourceMaskSpec = defaultSourcePropertyMask;
            }
            AnimationInput animationInput1;
            if (Snapshot == SnapshotPolicy.Continuous)
            {
                animationInput1 = new ContinuousAnimationInput(sourceObject, sourcePropertyName, sourceMaskSpec);
            }
            else
            {
                CapturedAnimationInput capturedAnimationInput = new CapturedAnimationInput(sourceObject, sourcePropertyName, sourceMaskSpec);
                if (Snapshot == SnapshotPolicy.OnLoop)
                    capturedAnimationInput.RefreshOnRepeat = true;
                animationInput1 = capturedAnimationInput;
            }
            AnimationInput animationInput2 = animationInput1;
            for (int index = 1; index < _power; ++index)
                animationInput2 *= animationInput1;
            if (_multiply != 1.0)
                animationInput2 *= new ConstantAnimationInput(_multiply);
            if (_add != 0.0)
                animationInput2 += new ConstantAnimationInput(_add);
            return animationInput2;
        }

        public override string ToString()
        {
            if (this == s_absolute)
                return "Absolute";
            if (this == s_current)
                return "Current";
            if (this == s_currentSnapshotOnLoop)
                return "CurrentSnapshotOnLoop";
            return this == s_final ? "Final" : string.Format("[Object = {0}, Property = {1}]", _sourceObject != null ? _sourceObject : (object)_sourceId, _sourceProperty);
        }
    }
}
