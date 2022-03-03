// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateTypeIdentifier
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateTypeIdentifier : Microsoft.Iris.Markup.Validation.Validate
    {
        private string _prefix;
        private string _typeName;
        private TypeSchema _foundType;
        private int _foundTypeIndex = -1;

        public ValidateTypeIdentifier(
          SourceMarkupLoader owner,
          string prefix,
          string typeName,
          int line,
          int column)
          : base(owner, line, column)
        {
            _prefix = prefix;
            _typeName = typeName;
            if (_prefix == string.Empty)
                _prefix = null;
            owner.NotifyTypeIdentifierFound(_prefix, _typeName);
        }

        public ValidateTypeIdentifier(SourceMarkupLoader owner, string typeFQN, int line, int column)
          : base(owner, line, column)
        {
            int length = typeFQN.IndexOf(':');
            if (length >= 0)
            {
                _prefix = typeFQN.Substring(0, length);
                _typeName = typeFQN.Substring(length + 1);
            }
            else
                _typeName = typeFQN;
            owner.NotifyTypeIdentifierFound(_prefix, _typeName);
        }

        public static void PromoteSimplifiedTypeSyntax(ValidateProperty property)
        {
            if (property == null || !property.IsFromStringValue)
                return;
            string fromString = ((ValidateFromString)property.Value).FromString;
            ValidateTypeIdentifier typeIdentifier = new ValidateTypeIdentifier(property.Owner, fromString, property.Line, property.Column);
            property.Value = new ValidateExpressionTypeOf(property.Owner, typeIdentifier, property.Line, property.Column);
        }

        public string Prefix => _prefix;

        public string TypeName => _typeName;

        public void Validate()
        {
            LoadResult dependency = Owner.FindDependency(_prefix);
            if (dependency == null)
            {
                ReportError("Xmlns prefix '{0}' was not found", _prefix);
            }
            else
            {
                _foundType = dependency.FindType(_typeName);
                if (_foundType == null)
                    ReportError("Type '{0}' was not found within '{1}'", _typeName, dependency.Uri);
                else
                    _foundTypeIndex = Owner.TrackImportedType(_foundType);
            }
        }

        public TypeSchema FoundType => _foundType;

        public int FoundTypeIndex => _foundTypeIndex;

        public bool Validated => FoundType != null || HasErrors;

        public override string ToString() => _foundType != null ? _foundType.ToString() : _typeName + " (Unresolved)";
    }
}
