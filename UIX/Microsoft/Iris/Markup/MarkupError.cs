// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.MarkupError
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup
{
    internal class MarkupError
    {
        private ErrorRecord _error;
        private string _context;

        internal MarkupError(ErrorRecord error)
        {
            _error = error;
            _context = error.Context;
            if (_context == null || error.Line == -1)
                return;
            if (error.Column != -1)
                _context = string.Format("{0} ({1},{2})", _context, error.Line, error.Column);
            else
                _context = string.Format("{0} ({1})", _context, error.Line);
        }

        public bool IsError => !_error.Warning;

        public string Message => _error.Message;

        public string Context => _context;

        public string Uri => _error.Context;

        public int Line => _error.Line;

        public int Column => _error.Column;
    }
}
