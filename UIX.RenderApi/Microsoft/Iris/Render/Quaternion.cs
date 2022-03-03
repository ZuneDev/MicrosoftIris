// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Quaternion
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct Quaternion
    {
        private float m_x;
        private float m_y;
        private float m_z;
        private float m_w;
        public static readonly Quaternion Identity = new Quaternion(0.0f, 0.0f, 0.0f, 1f);

        public Quaternion(float x, float y, float z, float w)
        {
            this.m_x = x;
            this.m_y = y;
            this.m_z = z;
            this.m_w = w;
        }

        public Quaternion(Vector3 vAxis, float flAngle)
        {
            vAxis.Normalize();
            float num1 = (float)Math.Sin(0.5 * flAngle);
            float num2 = (float)Math.Cos(0.5 * flAngle);
            this.m_x = vAxis.X * num1;
            this.m_y = vAxis.Y * num1;
            this.m_z = vAxis.Z * num1;
            this.m_w = num2;
        }

        public Quaternion(AxisAngle axisAngle)
          : this(axisAngle.Axis, axisAngle.Angle)
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

        public float W
        {
            get => this.m_w;
            set => this.m_w = value;
        }

        public bool IsIdentity => Equals(this, Identity);

        public static Quaternion operator *(Quaternion q1, Quaternion q2) => new Quaternion()
        {
            m_x = (float)(q2.m_w * (double)q1.m_x + q2.m_x * (double)q1.m_w + q2.m_y * (double)q1.m_z - q2.m_z * (double)q1.m_y),
            m_y = (float)(q2.m_w * (double)q1.m_y - q2.m_x * (double)q1.m_z + q2.m_y * (double)q1.m_w + q2.m_z * (double)q1.m_x),
            m_z = (float)(q2.m_w * (double)q1.m_z + q2.m_x * (double)q1.m_y - q2.m_y * (double)q1.m_x + q2.m_z * (double)q1.m_w),
            m_w = (float)(q2.m_w * (double)q1.m_w - q2.m_x * (double)q1.m_x - q2.m_y * (double)q1.m_y - q2.m_z * (double)q1.m_z)
        };

        public static Quaternion Multiply(Quaternion qLeft, Quaternion qRight) => qLeft * qRight;

        public override bool Equals(object obj) => obj is Quaternion quaternion && this == quaternion;

        public static bool operator ==(Quaternion left, Quaternion right) => Math2.WithinEpsilon(left.X, right.X) && Math2.WithinEpsilon(left.Y, right.Y) && Math2.WithinEpsilon(left.Z, right.Z) && Math2.WithinEpsilon(left.W, right.W);

        public static bool operator !=(Quaternion left, Quaternion right) => !(left == right);

        public AxisAngle ToAxisAngle()
        {
            this.Normalize();
            Vector3 vAxis;
            float flAngle;
            this.ToAxisAngle(out vAxis, out flAngle);
            return vAxis.IsApproximate(Vector3.Zero) ? AxisAngle.Identity : new AxisAngle(vAxis, flAngle);
        }

        internal void Normalize()
        {
            float flValue1 = (float)(m_x * (double)this.m_x + m_y * (double)this.m_y + m_z * (double)this.m_z + m_w * (double)this.m_w);
            if (Math2.WithinEpsilon(flValue1, 1f))
                return;
            if (flValue1 > 1.40129846432482E-45)
            {
                float num = (float)Math.Sqrt(flValue1);
                this.m_x /= num;
                this.m_y /= num;
                this.m_z /= num;
                this.m_w /= num;
            }
            else
            {
                this.m_x = 0.0f;
                this.m_y = 0.0f;
                this.m_z = 0.0f;
                this.m_w = 0.0f;
            }
        }

        public override int GetHashCode() => this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode() ^ this.W.GetHashCode();

        public override string ToString() => base.ToString();

        public void ToAxisAngle(out Vector3 vAxis, out float flAngle)
        {
            vAxis = new Vector3(this.m_x, this.m_y, this.m_z);
            flAngle = (float)(2.0 * Math.Acos(m_w));
        }

        public bool IsApproximate(Quaternion q) => Math2.WithinEpsilon(this.m_x, q.m_x) && Math2.WithinEpsilon(this.m_y, q.m_y) && Math2.WithinEpsilon(this.m_z, q.m_z) && Math2.WithinEpsilon(this.m_w, q.m_w);
    }
}
