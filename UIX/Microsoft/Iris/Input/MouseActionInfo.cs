// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.MouseActionInfo
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Input
{
    public abstract class MouseActionInfo : MouseInfo
    {
        private IRawInputSite _naturalHit;
        private ICookedInputSite _naturalTarget;
        private InputModifiers _modifiers;
        private int _screenX;
        private int _screenY;
        private int _wheelDelta;
        private uint _nativeMessageID;
        private MouseButtons _button;

        protected void Initialize(
          IRawInputSite rawSource,
          IRawInputSite rawNatural,
          int x,
          int y,
          int screenX,
          int screenY,
          InputModifiers modifiers,
          InputEventType eventType,
          uint messageID,
          MouseButtons button,
          int wheelDelta)
        {
            _naturalHit = rawNatural;
            _modifiers = modifiers;
            _screenX = screenX;
            _screenY = screenY;
            _wheelDelta = wheelDelta;
            _nativeMessageID = messageID;
            _button = button;
            Initialize(rawSource, x, y, eventType);
        }

        protected override void Zombie()
        {
            base.Zombie();
            _naturalHit = null;
            _naturalTarget = null;
        }

        public uint NativeMessageID => _nativeMessageID;

        public IRawInputSite NaturalHit => _naturalHit;

        public ICookedInputSite NaturalTarget => _naturalTarget;

        public InputModifiers Modifiers => _modifiers;

        public MouseButtons Button => _button;

        public int ScreenX => _screenX;

        public int ScreenY => _screenY;

        public int WheelDelta => _wheelDelta;

        public void SetMappedTargets(ICookedInputSite target, ICookedInputSite naturalTarget)
        {
            UpdateTarget(target);
            _naturalTarget = naturalTarget;
        }
    }
}
