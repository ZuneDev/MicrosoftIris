// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.Validate
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup.Validation
{
    internal class Validate : DisposableObject
    {
        private SourceMarkupLoader _owner;
        private int _line;
        private int _column;
        private ValidateMetadata _metadata;
        private bool _hasErrors;

        public Validate(SourceMarkupLoader owner, int line, int column)
        {
            _owner = owner;
            _line = line;
            _column = column;
            _owner.TrackValidateObject(this);
        }

        protected Validate()
        {
        }

        public SourceMarkupLoader Owner => _owner;

        public int Line => _line;

        public int Column => _column;

        public ValidateMetadata Metadata
        {
            get
            {
                if (_metadata == null)
                    _metadata = new ValidateMetadata();
                return _metadata;
            }
        }

        public bool HasErrors => _hasErrors;

        public void MarkHasErrors()
        {
            _owner.MarkHasErrors();
            _hasErrors = true;
        }

        public void ReportError(
          string error,
          string param0,
          string param1,
          string param2,
          string param3)
        {
            ReportError(string.Format(error, param0, param1, param2, param3));
        }

        public void ReportError(string error, string param0, string param1, string param2) => ReportError(string.Format(error, param0, param1, param2));

        public void ReportError(string error, string param0, string param1) => ReportError(string.Format(error, param0, param1));

        public void ReportError(string error, string param0) => ReportError(string.Format(error, param0));

        public void ReportError(string error)
        {
            MarkHasErrors();
            _owner.ReportError(error, _line, _column);
        }

        public void ReportErrorWithAdjustedPosition(string error, int lineOffset, int columnOffset)
        {
            MarkHasErrors();
            _owner.ReportError(error, _line + lineOffset, _column + columnOffset);
        }
    }
}
