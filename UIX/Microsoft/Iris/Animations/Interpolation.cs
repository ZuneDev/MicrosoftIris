// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.Interpolation
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Text;

namespace Microsoft.Iris.Animations
{
    public class Interpolation : IStringEncodable
    {
        public static readonly Interpolation Default = new Interpolation(InterpolationType.Linear);
        private InterpolationType _type;
        private float _weight = 1f;
        private float _bezierHandle1;
        private float _bezierHandle2 = 1f;
        private float _easePercent = 0.5f;

        public Interpolation()
          : this(InterpolationType.Linear)
        {
        }

        public Interpolation(InterpolationType type) => _type = type;

        public InterpolationType Type
        {
            get => _type;
            set => _type = value;
        }

        public float Weight
        {
            get => _weight;
            set => _weight = value;
        }

        public float BezierHandle1
        {
            get => _bezierHandle1;
            set => _bezierHandle1 = value;
        }

        public float BezierHandle2
        {
            get => _bezierHandle2;
            set => _bezierHandle2 = value;
        }

        public float EasePercent
        {
            get => _easePercent;
            set => _easePercent = value;
        }

        public override string ToString()
        {
            string str;
            switch (Type)
            {
                case InterpolationType.SCurve:
                    str = "SCurve";
                    break;
                case InterpolationType.Exp:
                    str = "Exp";
                    break;
                case InterpolationType.Log:
                    str = "Log";
                    break;
                case InterpolationType.Sine:
                    str = "Sine";
                    break;
                case InterpolationType.Cosine:
                    str = "Cosine";
                    break;
                case InterpolationType.Bezier:
                    str = "Bezier, " + BezierHandle1 + ", " + BezierHandle2;
                    break;
                case InterpolationType.EaseIn:
                    str = "EaseIn, " + Weight + ", " + EasePercent;
                    break;
                case InterpolationType.EaseOut:
                    str = "EaseOut, " + Weight + ", " + EasePercent;
                    break;
                default:
                    str = "Linear";
                    break;
            }
            if (Weight != 1.0)
                str = str + ", " + Weight;
            return str;
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            if (obj is Interpolation)
            {
                Interpolation interpolation = (Interpolation)obj;
                flag = _type == interpolation._type && _weight == (double)interpolation._weight && (_bezierHandle1 == (double)interpolation._bezierHandle1 && _bezierHandle2 == (double)interpolation._bezierHandle2) && _easePercent == (double)interpolation._easePercent;
            }
            return flag;
        }

        public override int GetHashCode() => _type.GetHashCode() ^ _weight.GetHashCode() ^ _bezierHandle1.GetHashCode() ^ _bezierHandle2.GetHashCode() ^ _easePercent.GetHashCode();

        public string EncodeString()
        {
            StringBuilder sb = new(Type.ToString());

            if (Type is InterpolationType.Bezier)
                sb.Append($", {BezierHandle1}, {BezierHandle2}");
            else if (Type is InterpolationType.EaseOut or InterpolationType.EaseIn)
                sb.Append($", {Weight}, {EasePercent}");
            else if (Weight != 1.0)
                sb.Append($", {Weight}");

            return sb.ToString();
        }
    }
}
