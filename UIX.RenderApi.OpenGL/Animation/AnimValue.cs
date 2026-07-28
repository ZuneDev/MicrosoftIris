using System;

namespace Microsoft.Iris.Render.OpenGL.Animation
{
    /// <summary>
    /// A resolved animation value as up to four float channels plus its logical type.
    /// Reads keyframe inputs through the public <see cref="AnimationInput.TryGetConstantValue"/>
    /// accessor and folds <see cref="BinaryOperation"/> expressions over their public operands.
    /// Object/continuous inputs (whose value depends on a live render object) are not
    /// resolvable here and report failure so the caller can hold the previous value.
    /// </summary>
    internal readonly struct AnimValue
    {
        public readonly AnimationInputType Type;
        public readonly float X, Y, Z, W;

        public AnimValue(AnimationInputType type, float x, float y, float z, float w)
        {
            Type = type;
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static bool TryRead(AnimationInput input, out AnimValue value)
        {
            value = default;
            if (input == null)
                return false;

            if (input is BinaryOperation op)
            {
                if (!TryRead(op.LeftOperand, out AnimValue l) || !TryRead(op.RightOperand, out AnimValue r))
                    return false;
                value = op.Operation == BinaryOpCode.Multiply
                    ? new AnimValue(l.Type, l.X * r.X, l.Y * r.Y, l.Z * r.Z, l.W * r.W)
                    : new AnimValue(l.Type, l.X + r.X, l.Y + r.Y, l.Z + r.Z, l.W + r.W);
                return true;
            }

            if (!input.TryGetConstantValue(out object raw) || raw == null)
                return false;

            switch (raw)
            {
                case float f: value = new AnimValue(input.InputType, f, 0f, 0f, 0f); return true;
                case Vector2 v2: value = new AnimValue(input.InputType, v2.X, v2.Y, 0f, 0f); return true;
                case Vector3 v3: value = new AnimValue(input.InputType, v3.X, v3.Y, v3.Z, 0f); return true;
                case Vector4 v4: value = new AnimValue(input.InputType, v4.X, v4.Y, v4.Z, v4.W); return true;
                case Quaternion q: value = new AnimValue(input.InputType, q.X, q.Y, q.Z, q.W); return true;
                default: return false;
            }
        }

        /// <summary>Interpolate between two values by t∈[0,1]; quaternions use slerp.</summary>
        public static AnimValue Lerp(AnimValue a, AnimValue b, float t, bool spherical)
        {
            if (a.Type == AnimationInputType.Quaternion && (spherical || b.Type == AnimationInputType.Quaternion))
                return Slerp(a, b, t);
            return new AnimValue(
                a.Type,
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t,
                a.W + (b.W - a.W) * t);
        }

        private static AnimValue Slerp(AnimValue a, AnimValue b, float t)
        {
            float dot = a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
            float bx = b.X, by = b.Y, bz = b.Z, bw = b.W;
            if (dot < 0f)
            {
                dot = -dot;
                bx = -bx; by = -by; bz = -bz; bw = -bw;
            }

            float wa, wb;
            if (dot > 0.9995f)
            {
                wa = 1f - t;
                wb = t;
            }
            else
            {
                float theta = (float)Math.Acos(dot);
                float sin = (float)Math.Sin(theta);
                wa = (float)Math.Sin((1f - t) * theta) / sin;
                wb = (float)Math.Sin(t * theta) / sin;
            }
            return new AnimValue(
                AnimationInputType.Quaternion,
                a.X * wa + bx * wb,
                a.Y * wa + by * wb,
                a.Z * wa + bz * wb,
                a.W * wa + bw * wb);
        }

        public float AsFloat() => X;
        public Vector2 AsVector2() => new Vector2(X, Y);
        public Vector3 AsVector3() => new Vector3(X, Y, Z);
        public Vector4 AsVector4() => new Vector4(X, Y, Z, W);

        public float GetChannel(int index) => index switch { 0 => X, 1 => Y, 2 => Z, _ => W };
    }
}
