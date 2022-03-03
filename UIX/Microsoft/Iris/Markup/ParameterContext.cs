// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.ParameterContext
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal struct ParameterContext
    {
        private string[] _parameterNames;
        private object[] _parameterValues;

        public ParameterContext(string[] parameterNames, object[] parameterValues)
        {
            _parameterNames = parameterNames;
            _parameterValues = parameterValues;
        }

        public object ReadParameter(string name)
        {
            int length = _parameterNames.Length;
            for (int index = 0; index < length; ++index)
            {
                if (_parameterNames[index] == name)
                    return _parameterValues[index];
            }
            return null;
        }

        public void WriteParameter(string name, object value)
        {
            int length = _parameterNames.Length;
            for (int index = 0; index < length; ++index)
            {
                if (_parameterNames[index] == name)
                {
                    _parameterValues[index] = value;
                    break;
                }
            }
        }
    }
}
