// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateParameterDefinition
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateParameterDefinition : Microsoft.Iris.Markup.Validation.Validate
    {
        private string _name;
        private ValidateTypeIdentifier _typeIdentifier;

        public ValidateParameterDefinition(
          SourceMarkupLoader owner,
          int line,
          int column,
          string name,
          ValidateTypeIdentifier typeIdentifier)
          : base(owner, line, column)
        {
            _name = name;
            _typeIdentifier = typeIdentifier;
        }

        public string Name => _name;

        public ValidateTypeIdentifier TypeIdentifier => _typeIdentifier;

        public TypeSchema FoundType => _typeIdentifier.FoundType;

        public void Validate(ValidateCode container, ValidateContext context)
        {
            if (context.CurrentPass < LoadPass.PopulatePublicModel || _typeIdentifier.Validated)
                return;
            _typeIdentifier.Validate();
            if (!_typeIdentifier.HasErrors)
                return;
            MarkHasErrors();
        }
    }
}
