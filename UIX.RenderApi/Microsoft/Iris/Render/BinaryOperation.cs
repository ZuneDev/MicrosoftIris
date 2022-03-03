// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.BinaryOperation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render
{
    public class BinaryOperation : AnimationInput
    {
        private AnimationInput m_leftInput;
        private AnimationInput m_rightInput;
        private BinaryOpCode m_opCode;

        public BinaryOperation(
          BinaryOpCode opCode,
          AnimationInput leftInput,
          AnimationInput rightInput)
        {
            Debug2.Validate(leftInput != null, typeof(ArgumentNullException), nameof(leftInput));
            Debug2.Validate(rightInput != null, typeof(ArgumentNullException), nameof(rightInput));
            this.m_leftInput = leftInput;
            this.m_rightInput = rightInput;
            this.m_opCode = opCode;
            this.CommonCreate(leftInput.InputType, AnimationTypeMask.Default);
        }

        public BinaryOpCode Operation => this.m_opCode;

        public AnimationInput LeftOperand => this.m_leftInput;

        public AnimationInput RightOperand => this.m_rightInput;
    }
}
