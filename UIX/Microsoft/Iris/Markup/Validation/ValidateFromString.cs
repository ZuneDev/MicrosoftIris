// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateFromString
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateFromString : ValidateObject
    {
        private string _fromString;
        private bool _expandEscapes;
        private TypeSchema _typeHint;
        private int _typeHintIndex;
        private object _fromStringInstance;

        public ValidateFromString(
          SourceMarkupLoader owner,
          string fromString,
          bool expandEscapes,
          int line,
          int column)
          : base(owner, line, column, ObjectSourceType.FromString)
        {
            _fromString = fromString;
            _expandEscapes = expandEscapes;
        }

        public string FromString => _fromString;

        public override TypeSchema ObjectType => _typeHint;

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            _typeHint = typeRestriction.Primary;
            if (!_typeHint.SupportsTypeConversion(StringSchema.Type))
            {
                ReportError("String conversion is not available for '{0}'", _typeHint.Name);
            }
            else
            {
                if (_expandEscapes)
                {
                    string invalidSequence;
                    _fromString = StringUtility.Unescape(_fromString, out int _, out invalidSequence);
                    if (_fromString == null)
                    {
                        ReportError("Invalid escape sequence '{0}' in string literal", invalidSequence);
                        return;
                    }
                }
                Result result = _typeHint.TypeConverter(_fromString, StringSchema.Type, out _fromStringInstance);
                if (result.Failed)
                    ReportError(result.Error);
                else
                    _typeHintIndex = Owner.TrackImportedType(_typeHint);
            }
        }

        public object FromStringInstance => _fromStringInstance;

        public int TypeHintIndex => _typeHintIndex;

        public override string ToString() => _typeHint != null ? string.Format("FromString : '{0}' {1}", _fromString, _typeHint) : "Unavailable";
    }
}
