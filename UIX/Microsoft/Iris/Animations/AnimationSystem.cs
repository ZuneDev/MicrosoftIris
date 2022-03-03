// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.AnimationSystem
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Iris.Animations
{
    internal static class AnimationSystem
    {
        private static bool _enabledFlag = true;
        private static bool _overrideToFalseFlag;
        private static int _disableAnimationCount;
        private static Dictionary<string, AnimationTemplate> _sequences = new Dictionary<string, AnimationTemplate>();

        public static void ValidateAnimationType(AnimationType paramType)
        {
        }

        public static bool SequenceExists(string id) => _sequences.ContainsKey(id);

        public static AnimationTemplate GetSequenceByID(string id)
        {
            if (!Enabled)
                return null;
            return SequenceExists(id) ? (AnimationTemplate)_sequences[id].Clone() : null;
        }

        public static AnimationTemplate GetSequenceByIDAlways(string id) => SequenceExists(id) ? (AnimationTemplate)_sequences[id].Clone() : null;

        public static void AddSequenceByID(string id, AnimationTemplate seq)
        {
            if (SequenceExists(id))
                return;
            _sequences.Add(id, seq);
        }

        public static void ClearSequences() => _sequences = new Dictionary<string, AnimationTemplate>();

        public static ICollection GetAllSequences() => _sequences.Values;

        public static bool Enabled => _enabledFlag;

        public static void SetEnableState(bool value) => _enabledFlag = value;

        public static void OverrideAnimationState(bool overrideToFalseFlag)
        {
            if (_overrideToFalseFlag == overrideToFalseFlag)
                return;
            _overrideToFalseFlag = overrideToFalseFlag;
            UpdateAnimationState();
        }

        public static void UpdateAnimationState()
        {
            bool flag = true;
            if (_disableAnimationCount > 0 || _overrideToFalseFlag)
                flag = false;
            SetEnableState(flag);
        }

        public static void PushDisableAnimations()
        {
            ++_disableAnimationCount;
            if (_disableAnimationCount != 1)
                return;
            UpdateAnimationState();
        }

        public static void PopDisableAnimations()
        {
            --_disableAnimationCount;
            if (_disableAnimationCount != 0)
                return;
            UpdateAnimationState();
        }
    }
}
