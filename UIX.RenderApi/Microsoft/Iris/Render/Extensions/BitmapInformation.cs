// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.BitmapInformation
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Protocol;
using System;

namespace Microsoft.Iris.Render.Extensions
{
    public class BitmapInformation : IDisposable
    {
        public ImageInformation imageInfo;
        internal HSpBitmap hBitmap;

        public void Dispose() => this.ReleaseData();

        private void ReleaseData()
        {
            if (!(this.hBitmap != HSpBitmap.NULL))
                return;
            EngineApi.IFC(ExtensionsApi.SpBitmapDelete(this.hBitmap));
            this.hBitmap = HSpBitmap.NULL;
        }

        public override string ToString() => base.ToString();
    }
}
