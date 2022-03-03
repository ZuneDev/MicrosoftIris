// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.Animation
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using System;
using System.Collections.Specialized;
using System.Text;

namespace Microsoft.Iris.Animations
{
    internal class Animation : AnimationTemplate, IAnimationProvider
    {
        private static readonly DataCookie s_rotationAxisProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_centerPointScaleProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_centerPointOffsetProperty = DataCookie.ReserveSlot();
        private BitVector32 _bitsBitVector;
        private AnimationEventType _type;
        private DynamicData _dataMap;

        public Animation()
        {
            _type = AnimationEventType.Idle;
            _bitsBitVector = new BitVector32();
            _dataMap = new DynamicData();
        }

        public override object Clone()
        {
            Animation animation = new Animation();
            CloneWorker(animation);
            return animation;
        }

        protected override void CloneWorker(AnimationTemplate rawAnimation)
        {
            base.CloneWorker(rawAnimation);
            Animation animation = (Animation)rawAnimation;
            animation.Type = Type;
            if (GetBit(Bits.CenterPointScale))
                animation.CenterPointPercent = CenterPointPercent;
            if (GetBit(Bits.RotationAxis))
                animation.RotationAxis = RotationAxis;
            animation.DisableMouseInput = DisableMouseInput;
        }

        private void PrepareToPlay(ref AnimationArgs args)
        {
            if (GetBit(Bits.CenterPointScale))
                args.ViewItem.VisualCenterPoint = CenterPointPercent;
            if (!GetBit(Bits.RotationAxis))
                return;
            Rotation visualRotation = args.ViewItem.VisualRotation;
            args.ViewItem.VisualRotation = new Rotation(visualRotation.AngleRadians, RotationAxis);
        }

        public AnimationEventType Type
        {
            get => _type;
            set => _type = value;
        }

        public Vector3 CenterPointPercent
        {
            get => !GetBit(Bits.CenterPointScale) ? Vector3.Zero : (Vector3)GetData(s_centerPointScaleProperty);
            set
            {
                if (!(CenterPointPercent != value))
                    return;
                SetData(s_centerPointScaleProperty, value);
                SetBit(Bits.CenterPointScale, true);
            }
        }

        public Vector3 RotationAxis
        {
            get => !GetBit(Bits.RotationAxis) ? Rotation.Default.Axis : (Vector3)GetData(s_rotationAxisProperty);
            set
            {
                if (!(RotationAxis != value))
                    return;
                SetData(s_rotationAxisProperty, value);
                SetBit(Bits.RotationAxis, true);
            }
        }

        public bool DisableMouseInput
        {
            get => GetBit(Bits.DisableMouseInput);
            set => SetBit(Bits.DisableMouseInput, value);
        }

        AnimationTemplate IAnimationProvider.Build(
          ref AnimationArgs args)
        {
            PrepareToPlay(ref args);
            return this;
        }

        public bool CanCache => true;

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{AnimationTemplate ID=");
            stringBuilder.Append(DebugID);
            stringBuilder.Append(", Loop=");
            stringBuilder.Append(Loop);
            stringBuilder.Append(", KeyframeCount=");
            stringBuilder.Append(Keyframes.Count);
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        private bool GetBit(Animation.Bits bit) => _bitsBitVector[(int)bit];

        private void SetBit(Animation.Bits bit, bool value) => _bitsBitVector[(int)bit] = value;

        private bool ChangeBit(Animation.Bits bit, bool value)
        {
            if (_bitsBitVector[(int)bit] == value)
                return false;
            _bitsBitVector[(int)bit] = value;
            return true;
        }

        protected object GetData(DataCookie cookie) => _dataMap.GetData(cookie);

        protected void SetData(DataCookie cookie, object value) => _dataMap.SetData(cookie, value);

        protected Delegate GetEventHandler(EventCookie cookie) => _dataMap.GetEventHandler(cookie);

        protected void AddEventHandler(EventCookie cookie, Delegate handlerToAdd) => _dataMap.AddEventHandler(cookie, handlerToAdd);

        protected void RemoveEventHandler(EventCookie cookie, Delegate handlerToRemove) => _dataMap.RemoveEventHandler(cookie, handlerToRemove);

        protected void RemoveEventHandlers(EventCookie cookie) => _dataMap.RemoveEventHandlers(cookie);

        private static uint GetKey(EventCookie cookie) => EventCookie.ToUInt32(cookie);

        private enum Bits
        {
            CenterPointScale = 1,
            RotationAxis = 2,
            DisableMouseInput = 4,
        }
    }
}
