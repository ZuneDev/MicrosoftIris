// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Vector3
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using System;
using System.Text;

namespace Microsoft.Iris.Render
{
    [Serializable]
    public struct Vector3
    {
        private float m_x;
        private float m_y;
        private float m_z;
        public static readonly Vector3 Zero = new Vector3(0.0f, 0.0f, 0.0f);
        public static readonly Vector3 UnitVector = new Vector3(1f, 1f, 1f);

        public Vector3(float x, float y, float z)
        {
            this.m_x = x;
            this.m_y = y;
            this.m_z = z;
        }

        public Vector3(Vector3 value)
        {
            this.m_x = value.m_x;
            this.m_y = value.m_y;
            this.m_z = value.m_z;
        }

        internal Vector3(double x, double y, double z)
          : this((float)x, (float)y, (float)z)
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

        public float Z
        {
            get => this.m_z;
            set => this.m_z = value;
        }

        public static Vector3 operator +(Vector3 left, Vector3 right) => new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

        public static Vector3 operator +(Vector3 vector, float scalar) => new Vector3(vector.X + scalar, vector.Y + scalar, vector.Z + scalar);

        public static Vector3 Add(Vector3 left, Vector3 right) => left + right;

        public static Vector3 operator -(Vector3 left, Vector3 right) => new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        public static Vector3 operator -(Vector3 vector, float scalar) => new Vector3(vector.X - scalar, vector.Y - scalar, vector.Z - scalar);

        public static Vector3 operator -(Vector3 left) => new Vector3(-left.X, -left.Y, -left.Z);

        public static Vector3 Subtract(Vector3 left, Vector3 right) => left - right;

        public static Vector3 operator *(Vector3 left, Vector3 right) => new Vector3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

        public static Vector3 Multiply(Vector3 left, Vector3 right) => left * right;

        public static Vector3 operator *(Vector3 vector, float scalar) => new Vector3(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);

        public static Vector3 Multiply(Vector3 vector, float scalar) => vector * scalar;

        public static Vector3 operator /(Vector3 left, Vector3 right) => new Vector3(left.X / right.X, left.Y / right.Y, left.Z / right.Z);

        public static Vector3 Divide(Vector3 left, Vector3 right) => left / right;

        public static Vector3 operator /(Vector3 vector, float scalar) => new Vector3(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);

        public static Vector3 Divide(Vector3 vector, float scalar) => vector / scalar;

        internal void Normalize()
        {
            float flValue1 = (float)(m_x * (double)this.m_x + m_y * (double)this.m_y + m_z * (double)this.m_z);
            if (Math2.WithinEpsilon(flValue1, 1f))
                return;
            if (flValue1 > 1.40129846432482E-45)
            {
                float num = (float)Math.Sqrt(flValue1);
                this.m_x /= num;
                this.m_y /= num;
                this.m_z /= num;
            }
            else
            {
                this.m_x = 0.0f;
                this.m_y = 0.0f;
                this.m_z = 0.0f;
            }
        }

        public override bool Equals(object obj) => obj is Vector3 vector3 && this == vector3;

        public static bool operator ==(Vector3 left, Vector3 right) => left.X == (double)right.X && left.Y == (double)right.Y && left.Z == (double)right.Z;

        public static bool operator !=(Vector3 left, Vector3 right) => !(left == right);

        public override int GetHashCode() => this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();

        public override string ToString() => base.ToString();

        internal string ToDxShaderString()
        {
            StringBuilder stringBuilder = new StringBuilder(128);
            stringBuilder.Append("{");
            stringBuilder.Append(this.X);
            stringBuilder.Append(", ");
            stringBuilder.Append(this.Y);
            stringBuilder.Append(", ");
            stringBuilder.Append(this.Z);
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        public bool IsApproximate(Vector3 vector) => Math2.WithinEpsilon(this.m_x, vector.m_x) && Math2.WithinEpsilon(this.m_y, vector.m_y) && Math2.WithinEpsilon(this.m_z, vector.m_z);
    }
}
