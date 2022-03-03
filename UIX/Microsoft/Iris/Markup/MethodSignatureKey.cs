// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MethodSignatureKey
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal struct MethodSignatureKey
    {
        private string _name;
        private TypeSchema[] _parameters;

        public MethodSignatureKey(TypeSchema[] parameters)
          : this(null, parameters)
        {
        }

        public MethodSignatureKey(string name, TypeSchema[] parameters)
        {
            _name = name;
            _parameters = parameters;
        }

        public override bool Equals(object obj)
        {
            MethodSignatureKey methodSignatureKey = (MethodSignatureKey)obj;
            if (_name != methodSignatureKey._name || _parameters.Length != methodSignatureKey._parameters.Length)
                return false;
            for (int index = 0; index < _parameters.Length; ++index)
            {
                if (!_parameters[index].IsAssignableFrom(methodSignatureKey._parameters[index]))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int num = 0;
            if (_name != null)
                num ^= _name.GetHashCode();
            return num ^ _parameters.Length;
        }
    }
}
