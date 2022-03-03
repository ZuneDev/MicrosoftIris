// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.RawMouseData
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render;
using System;

namespace Microsoft.Iris.Input
{
    [Serializable]
    public struct RawMouseData
    {
        public IVisual _visCapture;
        public IVisual _visNatural;
        public int _positionX;
        public int _positionY;
        public int _naturalX;
        public int _naturalY;
        public int _physicalX;
        public int _physicalY;
        public int _screenX;
        public int _screenY;
        public MouseButtons _button;
        public int _wheelDelta;

        public RawMouseData(
          IVisual visCapture,
          IVisual visNatural,
          int positionX,
          int positionY,
          int naturalX,
          int naturalY,
          int physicalX,
          int physicalY,
          int screenX,
          int screenY,
          MouseButtons button,
          int wheelDelta)
        {
            this._visCapture = visCapture;
            this._visNatural = visNatural;
            this._positionX = positionX;
            this._positionY = positionY;
            this._naturalX = naturalX;
            this._naturalY = naturalY;
            this._physicalX = physicalX;
            this._physicalY = physicalY;
            this._screenX = screenX;
            this._screenY = screenY;
            this._button = button;
            this._wheelDelta = wheelDelta;
        }
    }
}
