// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.TextRunData
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.ViewItems
{
    internal class TextRunData
    {
        private TextRun _textRun;
        private Text _textViewItem;
        private Point _position;
        private Size _size;
        private bool _isOnLastLine;
        private int _lineNumber;

        public TextRunData(
          TextRun textRun,
          bool isOnLastLine,
          Text textViewItem,
          int lineAlignmentOffset)
        {
            _textRun = textRun;
            _size = textRun.Size;
            _position = textRun.Position;
            _position.X += lineAlignmentOffset;
            _lineNumber = textRun.Line;
            _isOnLastLine = isOnLastLine;
            _textViewItem = textViewItem;
        }

        public Point Position => _position;

        public Size Size => _size;

        public Color Color => _textRun == null ? new Color(0U) : _textRun.Color;

        public int LineNumber => _lineNumber;

        public TextRun Run => _textRun;

        public Text TextViewItem => _textViewItem;

        public bool IsOnLastLine => _isOnLastLine;

        public void NotifyPaintInvalid()
        {
            if (PaintInvalid == null)
                return;
            PaintInvalid();
        }

        public override string ToString() => string.Format("[ Text = {0}, Position = {1}, Size = {2} ]", _textRun.Content, _position, _size);

        public event PaintInvalidEventHandler PaintInvalid;
    }
}
