// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateAlias
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateAlias : ValidateObjectTag
    {
        private string _aliasName;
        private ValidateTypeIdentifier _typeIdentifier;
        private LoadPass _currentValidationPass;

        public ValidateAlias(
          SourceMarkupLoader owner,
          ValidateTypeIdentifier typeIdentifier,
          int line,
          int offset)
          : base(owner, typeIdentifier, line, offset)
        {
        }

        public void Validate(LoadPass currentPass)
        {
            if (_currentValidationPass >= currentPass)
                return;
            _currentValidationPass = currentPass;
            if (_currentValidationPass == LoadPass.DeclareTypes)
            {
                ValidateContext context = new ValidateContext(null, null, _currentValidationPass);
                Validate(TypeRestriction.None, context);
                _aliasName = GetInlinePropertyValueNoValidate("Name");
                string propertyValueNoValidate = GetInlinePropertyValueNoValidate("Type");
                if (propertyValueNoValidate != null)
                {
                    _typeIdentifier = new ValidateTypeIdentifier(Owner, propertyValueNoValidate, Line, Column);
                    Owner.NotifyTypeIdentifierFound(_typeIdentifier.Prefix, _typeIdentifier.TypeName);
                    string typeName = _typeIdentifier.TypeName;
                    if (_aliasName == null)
                        _aliasName = typeName;
                    LoadResult dependency = Owner.FindDependency(_typeIdentifier.Prefix);
                    if (dependency == null)
                        ReportError("Xmlns prefix '{0}' was not found", _typeIdentifier.Prefix);
                    else if (!ValidateContext.IsValidSymbolName(_aliasName))
                    {
                        ReportError("Invalid name \"{0}\".  Valid names must begin with either an alphabetic character or an underscore and can otherwise contain only alphabetic, numeric, or underscore characters", _aliasName);
                    }
                    else
                    {
                        if (Owner.IsTypeNameTaken(_aliasName))
                            ReportError("Type '{0}' was specified more than once", _aliasName);
                        Owner.RegisterAlias(_aliasName, dependency, typeName);
                    }
                }
                else
                    ReportError("Alias Type property must be provided");
            }
            else
            {
                if (_currentValidationPass != LoadPass.Full || Owner.LoadResultTarget.FindType(_aliasName) != null)
                    return;
                ReportError("Target type for alias '{0}' could not be found", _aliasName);
            }
        }
    }
}
