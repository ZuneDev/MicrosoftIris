// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateStatementScopedLocal
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateStatementScopedLocal : ValidateStatement
    {
        private string _name;
        private ValidateTypeIdentifier _typeIdentifier;
        private bool _hasInitialAssignment;
        private TypeSchema _foundType;
        private int _foundTypeIndex;
        private int _foundSymbolIndex;

        public ValidateStatementScopedLocal(
          SourceMarkupLoader owner,
          string name,
          ValidateTypeIdentifier typeIdentifier,
          int line,
          int column)
          : base(owner, line, column, StatementType.ScopedLocal)
        {
            _name = name;
            _typeIdentifier = typeIdentifier;
        }

        public string Name => _name;

        public ValidateTypeIdentifier TypeIdentifier => _typeIdentifier;

        public override void Validate(ValidateCode container, ValidateContext context)
        {
            if (context.CurrentPass >= LoadPass.PopulatePublicModel && !_typeIdentifier.Validated)
            {
                _typeIdentifier.Validate();
                if (_typeIdentifier.HasErrors)
                {
                    MarkHasErrors();
                    return;
                }
                _foundType = _typeIdentifier.FoundType;
                _foundTypeIndex = _typeIdentifier.FoundTypeIndex;
            }
            if (context.CurrentPass != LoadPass.Full)
                return;
            Result result = context.NotifyScopedLocal(_name, _foundType);
            if (result.Failed)
                ReportError(result.Error);
            else
                _foundSymbolIndex = context.TrackSymbolUsage(_name, SymbolOrigin.ScopedLocal);
        }

        public bool HasInitialAssignment
        {
            get => _hasInitialAssignment;
            set => _hasInitialAssignment = value;
        }

        public TypeSchema FoundType => _foundType;

        public int FoundTypeIndex => _foundTypeIndex;

        public int FoundSymbolIndex => _foundSymbolIndex;
    }
}
