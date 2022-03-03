// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.RawDragData
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render;
using System;

namespace Microsoft.Iris.Input
{
    [Serializable]
    public struct RawDragData
    {
        public IVisual _visCapture;
        public int _positionX;
        public int _positionY;
        public IntPtr _pDataStream;

        public RawDragData(IVisual visCapture, int positionX, int positionY, IntPtr pDataStream)
        {
            this._visCapture = visCapture;
            this._positionX = positionX;
            this._positionY = positionY;
            this._pDataStream = pDataStream;
        }
    }
}
