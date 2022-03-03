// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.AxisAngle
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render
{
    [Serializable]
    public struct AxisAngle
    {
        public static readonly AxisAngle Zero = new AxisAngle(new Vector3(0.0f, 0.0f, 0.0f), 0.0f);
        public static readonly AxisAngle Identity = new AxisAngle(new Vector3(0.0f, 0.0f, 1f), 0.0f);
        private Vector3 m_axis;
        private float m_angle;

        public AxisAngle(Vector3 axis, float angle)
        {
            this.m_axis = axis;
            this.m_angle = angle;
        }

        public Vector3 Axis
        {
            get => this.m_axis;
            set => this.m_axis = value;
        }

        public float Angle
        {
            get => this.m_angle;
            set => this.m_angle = value;
        }

        public Quaternion ToQuaternion() => new Quaternion(this.m_axis, this.m_angle);

        public Vector4 ToVector4() => new Vector4(this.m_axis.X, this.m_axis.Y, this.m_axis.Z, this.m_angle);

        public override bool Equals(object obj) => obj is AxisAngle axisAngle && this == axisAngle;

        public static bool operator ==(AxisAngle left, AxisAngle right) => left.Axis == right.Axis && left.Angle == (double)right.Angle;

        public static bool operator !=(AxisAngle left, AxisAngle right) => !(left == right);

        public override int GetHashCode() => this.Axis.GetHashCode() ^ this.Angle.GetHashCode();

        public override string ToString() => base.ToString();
    }
}
