// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.TextImageItem
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using Microsoft.Iris.Render.Extensions;
using Microsoft.Iris.RenderAPI.Drawing;

namespace Microsoft.Iris.Drawing
{
    internal class TextImageItem : ImageCacheItem
    {
        private TextRun _run;
        private Dib _dib;
        private string _samplingModeName;
        private Color _textColor;
        private bool _outlineFlag;

        internal TextImageItem(
          IRenderSession renderSession,
          TextRun run,
          string samplingModeName,
          bool outlineFlag,
          Color textColor)
          : base(renderSession, run.Content)
        {
            _run = run;
            _run.RegisterUsage(this);
            _dib = null;
            _samplingModeName = samplingModeName;
            _outlineFlag = outlineFlag;
            _textColor = textColor;
        }

        protected override void OnDispose()
        {
            _run.UnregisterUsage(this);
            ReleaseDib();
            base.OnDispose();
        }

        internal TextRun Run => _run;

        protected override bool EnsureBuffer()
        {
            if (_dib == null)
                _dib = _run.Rasterize(_samplingModeName, _textColor, _outlineFlag);
            return true;
        }

        protected override bool DoImageLoad() => RenderImage.LoadContent(_dib.ImageFormat, _dib.ContentSize, _dib.Stride, _dib.Data);

        protected override void OnImageLoadComplete()
        {
            if (HasLoadsInProgress)
                return;
            ReleaseDib();
        }

        public override string ToString() => _run.Content;

        private void ReleaseDib()
        {
            if (_dib == null)
                return;
            _dib.Dispose();
            _dib = null;
        }
    }
}
