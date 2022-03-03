// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.ValueTransformer
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Animations
{
    internal class ValueTransformer
    {
        private float _add;
        private float _subtract;
        private float _multiply;
        private float _divide;
        private float _mod;
        private bool _absolute;

        public ValueTransformer()
        {
            _multiply = 1f;
            _divide = 1f;
            _mod = float.MaxValue;
        }

        public float Transform(float value)
        {
            value *= _multiply;
            value /= _divide;
            value += _add;
            value -= _subtract;
            if (_mod != 3.40282346638529E+38)
                value %= _mod;
            if (_absolute)
                value = Math.Abs(value);
            return value;
        }

        public float Add
        {
            get => _add;
            set => _add = value;
        }

        public float Subtract
        {
            get => _subtract;
            set => _subtract = value;
        }

        public float Multiply
        {
            get => _multiply;
            set => _multiply = value;
        }

        public float Divide
        {
            get => _divide;
            set => _divide = value;
        }

        public float Mod
        {
            get => _mod;
            set => _mod = value;
        }

        public bool Absolute
        {
            get => _absolute;
            set => _absolute = value;
        }
    }
}
