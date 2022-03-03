// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.AnimationInput
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render
{
    public abstract class AnimationInput
    {
        private AnimationInputType m_sourceType;
        private AnimationInputType m_inputType;
        private AnimationTypeMask m_sourceMask;

        protected void CommonCreate(AnimationInputType sourceType, AnimationTypeMask sourceMask)
        {
            Debug2.Validate(sourceMask.CanMapFromType(sourceType), typeof(ArgumentException), nameof(sourceMask));
            AnimationInputType animationInputType;
            switch (sourceMask.ChannelCount)
            {
                case 0:
                    animationInputType = sourceType;
                    break;
                case 1:
                    animationInputType = AnimationInputType.Float;
                    break;
                case 2:
                    animationInputType = AnimationInputType.Vector2;
                    break;
                case 3:
                    animationInputType = AnimationInputType.Vector3;
                    break;
                case 4:
                    animationInputType = AnimationInputType.Vector4;
                    break;
                default:
                    Debug2.Throw(false, "Masking is limited to four channels");
                    animationInputType = sourceType;
                    break;
            }
            this.m_sourceType = sourceType;
            this.m_sourceMask = sourceMask;
            this.m_inputType = animationInputType;
        }

        public AnimationInputType InputType => this.m_inputType;

        internal AnimationInputType SourceType => this.m_sourceType;

        internal AnimationTypeMask SourceMask => this.m_sourceMask;

        internal virtual void RegisterUsage(object user)
        {
        }

        internal virtual void UnregisterUsage(object user)
        {
        }

        public static AnimationInput operator +(AnimationInput left, AnimationInput right)
        {
            Debug2.Validate(left != null, typeof(ArgumentNullException), nameof(left));
            Debug2.Validate(right != null, typeof(ArgumentNullException), nameof(right));
            return new BinaryOperation(BinaryOpCode.Add, left, right);
        }

        public static AnimationInput operator *(AnimationInput left, AnimationInput right)
        {
            Debug2.Validate(left != null, typeof(ArgumentNullException), nameof(left));
            Debug2.Validate(right != null, typeof(ArgumentNullException), nameof(right));
            return new BinaryOperation(BinaryOpCode.Multiply, left, right);
        }
    }
}
