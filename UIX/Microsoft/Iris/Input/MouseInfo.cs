// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.MouseInfo
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Input
{
    internal abstract class MouseInfo : InputInfo
    {
        private IRawInputSite _rawSource;
        private int _x;
        private int _y;

        protected void Initialize(IRawInputSite rawSource, int x, int y, InputEventType eventType)
        {
            _rawSource = rawSource;
            _x = x;
            _y = y;
            Initialize(eventType);
        }

        protected override void Zombie()
        {
            base.Zombie();
            _rawSource = null;
        }

        public IRawInputSite RawSource => _rawSource;

        public int X => _x;

        public int Y => _y;
    }
}
