// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.SwitchAnimation
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ModelItems;
using System.Collections.Generic;

namespace Microsoft.Iris.Animations
{
    internal class SwitchAnimation : IAnimationProvider
    {
        private IUIValueRange _expressionObject;
        private Dictionary<string, IAnimationProvider> _optionsList;
        private AnimationEventType _type;

        public IUIValueRange Expression
        {
            get => _expressionObject;
            set => _expressionObject = value;
        }

        public Dictionary<string, IAnimationProvider> Options
        {
            get
            {
                if (_optionsList == null)
                    _optionsList = new Dictionary<string, IAnimationProvider>();
                return _optionsList;
            }
            set => _optionsList = value;
        }

        public AnimationEventType Type
        {
            get => _type;
            set => _type = value;
        }

        public AnimationTemplate Build(ref AnimationArgs args)
        {
            AnimationTemplate animationTemplate = null;
            if (_optionsList != null)
            {
                object obj = null;
                if (_expressionObject != null)
                    obj = _expressionObject.ObjectValue;
                string key = null;
                if (obj != null)
                    key = obj.ToString();
                IAnimationProvider animationProvider = null;
                if (key != null && _optionsList.ContainsKey(key))
                    animationProvider = _optionsList[key];
                if (animationProvider != null)
                    animationTemplate = animationProvider.Build(ref args);
            }
            return animationTemplate;
        }

        public bool CanCache => false;
    }
}
