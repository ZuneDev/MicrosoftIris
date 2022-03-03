// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.MouseMoveInfo
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Input
{
    internal class MouseMoveInfo : MouseActionInfo
    {
        private static InputInfo.InfoType s_poolType = InfoType.MouseMove;

        static MouseMoveInfo() => SetPoolLimitMode(s_poolType, true);

        private MouseMoveInfo()
        {
        }

        public static MouseMoveInfo Create(
          IRawInputSite rawSource,
          IRawInputSite rawNatural,
          int x,
          int y,
          int screenX,
          int screenY,
          InputModifiers modifiers)
        {
            MouseMoveInfo mouseMoveInfo = (MouseMoveInfo)GetFromPool(s_poolType) ?? new MouseMoveInfo();
            mouseMoveInfo.Initialize(rawSource, rawNatural, x, y, screenX, screenY, modifiers, InputEventType.MouseMove, 512U, MouseButtons.None, 0);
            return mouseMoveInfo;
        }

        protected override InputInfo.InfoType PoolType => s_poolType;
    }
}
