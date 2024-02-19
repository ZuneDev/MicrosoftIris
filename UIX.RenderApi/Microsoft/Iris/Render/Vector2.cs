// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Vector2
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using System;
using System.Text;

namespace Microsoft.Iris.Render
{
    [Serializable]
    public struct Vector2 : IStringEncodable
    {
        private float m_x;
        private float m_y;
        public static readonly Vector2 Zero = new Vector2(0.0f, 0.0f);
        public static readonly Vector2 UnitVector = new Vector2(1f, 1f);

        public Vector2(float x, float y)
        {
            this.m_x = x;
            this.m_y = y;
        }

        internal Vector2(double x, double y)
          : this((float)x, (float)y)
        {
        }

        public float X
        {
            get => this.m_x;
            set => this.m_x = value;
        }

        public float Y
        {
            get => this.m_y;
            set => this.m_y = value;
        }

        public static Vector2 operator +(Vector2 left, Vector2 right) => new Vector2(left.X + right.X, left.Y + right.Y);

        public static Vector2 Add(Vector2 left, Vector2 right) => left + right;

        public static Vector2 operator -(Vector2 left, Vector2 right) => new Vector2(left.X - right.X, left.Y - right.Y);

        public static Vector2 operator -(Vector2 left) => new Vector2(-left.X, -left.Y);

        public static Vector2 Subtract(Vector2 left, Vector2 right) => left - right;

        public static Vector2 operator *(Vector2 left, Vector2 right) => new Vector2(left.X * right.X, left.Y * right.Y);

        public static Vector2 Multiply(Vector2 left, Vector2 right) => left * right;

        public static Vector2 operator *(Vector2 vector, float scalar) => new Vector2(vector.X * scalar, vector.Y * scalar);

        public static Vector2 Multiply(Vector2 vector, float scalar) => vector * scalar;

        public static Vector2 operator /(Vector2 left, Vector2 right) => new Vector2(left.X / right.X, left.Y / right.Y);

        public static Vector2 Divide(Vector2 left, Vector2 right) => left / right;

        public static Vector2 operator /(Vector2 vector, float scalar) => new Vector2(vector.X / scalar, vector.Y / scalar);

        public static Vector2 Divide(Vector2 vector, float scalar) => vector / scalar;

        internal void Normalize()
        {
            float flValue1 = (float)(m_x * (double)this.m_x + m_y * (double)this.m_y);
            if (Math2.WithinEpsilon(flValue1, 1f))
                return;
            if (flValue1 > 1.40129846432482E-45)
            {
                float num = (float)Math.Sqrt(flValue1);
                this.m_x /= num;
                this.m_y /= num;
            }
            else
            {
                this.m_x = 0.0f;
                this.m_y = 0.0f;
            }
        }

        public override bool Equals(object obj) => obj is Vector2 vector2 && this == vector2;

        public static bool operator ==(Vector2 left, Vector2 right) => left.X == (double)right.X && left.Y == (double)right.Y;

        public static bool operator !=(Vector2 left, Vector2 right) => !(left == right);

        public override int GetHashCode() => this.X.GetHashCode() ^ this.Y.GetHashCode();

        public override string ToString() => base.ToString();

        internal string ToDxShaderString()
        {
            StringBuilder stringBuilder = new StringBuilder(128);
            stringBuilder.Append("{");
            stringBuilder.Append(this.X);
            stringBuilder.Append(", ");
            stringBuilder.Append(this.Y);
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        public bool IsApproximate(Vector2 vector) => Math2.WithinEpsilon(this.m_x, vector.m_x) && Math2.WithinEpsilon(this.m_y, vector.m_y);

        public string EncodeString() => $"{X}, {Y}";
    }
}
