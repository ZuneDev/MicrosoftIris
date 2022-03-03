// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.ConstantAnimationInput
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;

namespace Microsoft.Iris.Render
{
    public sealed class ConstantAnimationInput : AnimationInput
    {
        private object m_inputValue;

        public ConstantAnimationInput(float sourceValue)
          : this(sourceValue, null)
        {
        }

        public ConstantAnimationInput(float sourceValue, string sourceMaskSpec)
        {
            AnimationTypeMask animationTypeMask = AnimationTypeMask.FromString(sourceMaskSpec);
            this.CommonCreate(AnimationInputType.Float, animationTypeMask);
            this.m_inputValue = this.ApplyMask(animationTypeMask, sourceValue);
        }

        public ConstantAnimationInput(Vector2 sourceValue)
          : this(sourceValue, null)
        {
        }

        public ConstantAnimationInput(Vector2 sourceValue, string sourceMaskSpec)
        {
            AnimationTypeMask animationTypeMask = AnimationTypeMask.FromString(sourceMaskSpec);
            this.CommonCreate(AnimationInputType.Vector2, animationTypeMask);
            this.m_inputValue = this.ApplyMask(animationTypeMask, sourceValue);
        }

        public ConstantAnimationInput(Vector3 sourceValue)
          : this(sourceValue, null)
        {
        }

        public ConstantAnimationInput(Vector3 sourceValue, string sourceMaskSpec)
        {
            AnimationTypeMask animationTypeMask = AnimationTypeMask.FromString(sourceMaskSpec);
            this.CommonCreate(AnimationInputType.Vector3, animationTypeMask);
            this.m_inputValue = this.ApplyMask(animationTypeMask, sourceValue);
        }

        public ConstantAnimationInput(Vector4 sourceValue)
          : this(sourceValue, null)
        {
        }

        public ConstantAnimationInput(Vector4 sourceValue, string sourceMaskSpec)
        {
            AnimationTypeMask animationTypeMask = AnimationTypeMask.FromString(sourceMaskSpec);
            this.CommonCreate(AnimationInputType.Vector4, animationTypeMask);
            this.m_inputValue = this.ApplyMask(animationTypeMask, sourceValue);
        }

        public ConstantAnimationInput(Quaternion sourceValue)
          : this(sourceValue, null)
        {
        }

        public ConstantAnimationInput(Quaternion sourceValue, string sourceMaskSpec)
        {
            AnimationTypeMask animationTypeMask = AnimationTypeMask.FromString(sourceMaskSpec);
            this.CommonCreate(AnimationInputType.Quaternion, animationTypeMask);
            this.m_inputValue = this.ApplyMask(animationTypeMask, sourceValue);
        }

        internal object RawValue => this.m_inputValue;

        private object ApplyMask(AnimationTypeMask mask, object value)
        {
            object obj = null;
            if (mask.ChannelCount == 0U)
            {
                obj = value;
            }
            else
            {
                float[] numArray = new float[4];
                switch (value)
                {
                    case float num:
                        numArray[0] = num;
                        numArray[1] = 0.0f;
                        numArray[2] = 0.0f;
                        numArray[3] = 0.0f;
                        break;
                    case Vector2 vector2:
                        numArray[0] = vector2.X;
                        numArray[1] = vector2.Y;
                        numArray[2] = 0.0f;
                        numArray[3] = 0.0f;
                        break;
                    case Vector3 vector3:
                        numArray[0] = vector3.X;
                        numArray[1] = vector3.Y;
                        numArray[2] = vector3.Z;
                        numArray[3] = 0.0f;
                        break;
                    case Vector4 vector4:
                        numArray[0] = vector4.X;
                        numArray[1] = vector4.Y;
                        numArray[2] = vector4.Z;
                        numArray[3] = vector4.W;
                        break;
                    case Quaternion quaternion:
                        numArray[0] = quaternion.X;
                        numArray[1] = quaternion.Y;
                        numArray[2] = quaternion.Z;
                        numArray[3] = quaternion.W;
                        break;
                    default:
                        Debug2.Throw(false, "Unsupported type: {0}", value.GetType());
                        break;
                }
                switch (mask.ChannelCount)
                {
                    case 1:
                        obj = (float)(mask[0] == AnimationTypeChannel.O ? 0.0 : numArray[(int)(mask[0] - 1)]);
                        break;
                    case 2:
                        obj = new Vector2()
                        {
                            X = (mask[0] == AnimationTypeChannel.O ? 0.0f : numArray[(int)(mask[0] - 1)]),
                            Y = (mask[1] == AnimationTypeChannel.O ? 0.0f : numArray[(int)(mask[1] - 1)])
                        };
                        break;
                    case 3:
                        obj = new Vector3()
                        {
                            X = (mask[0] == AnimationTypeChannel.O ? 0.0f : numArray[(int)(mask[0] - 1)]),
                            Y = (mask[1] == AnimationTypeChannel.O ? 0.0f : numArray[(int)(mask[1] - 1)]),
                            Z = (mask[2] == AnimationTypeChannel.O ? 0.0f : numArray[(int)(mask[2] - 1)])
                        };
                        break;
                    case 4:
                        obj = new Vector4()
                        {
                            X = (mask[0] == AnimationTypeChannel.O ? 0.0f : numArray[(int)(mask[0] - 1)]),
                            Y = (mask[1] == AnimationTypeChannel.O ? 0.0f : numArray[(int)(mask[1] - 1)]),
                            Z = (mask[2] == AnimationTypeChannel.O ? 0.0f : numArray[(int)(mask[2] - 1)]),
                            W = (mask[3] == AnimationTypeChannel.O ? 0.0f : numArray[(int)(mask[3] - 1)])
                        };
                        break;
                    default:
                        Debug2.Throw(false, "Only 4 channels are supported");
                        break;
                }
            }
            return obj;
        }
    }
}
