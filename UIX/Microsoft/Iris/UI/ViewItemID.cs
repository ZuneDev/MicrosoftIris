// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.UI.ViewItemID
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.UI
{
    public struct ViewItemID
    {
        private const int USE_STRING_PART = -1;
        private int _id;
        private string _stringPart;

        public ViewItemID(int id)
        {
            _id = id;
            _stringPart = null;
        }

        public ViewItemID(string stringPart)
        {
            _id = -1;
            _stringPart = stringPart;
        }

        public ViewItemID(int id, string stringPart)
        {
            _id = id;
            _stringPart = stringPart;
        }

        public bool IDValid => _id != -1;

        public bool StringPartValid => _stringPart != null;

        public int ID => _id;

        public string StringPart => _stringPart;

        public override string ToString()
        {
            if (!StringPartValid)
                return _id.ToString();
            return !IDValid ? _stringPart : InvariantString.Format("{0} {1}", _stringPart, _id);
        }
    }
}
