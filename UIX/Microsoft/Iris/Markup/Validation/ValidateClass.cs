// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateClass
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;
using System;
using System.Collections;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateClass : ValidateObjectTag
    {
        private ValidateScripts _scripts;
        private ValidateTypeIdentifier _baseTypeIdentifier;
        private ValidateProperty _foundPropertiesValidateProperty;
        private ValidateProperty _foundScriptsProperty;
        private ValidateMethodList _foundMethods;
        private Vector<MarkupMethodSchema> _foundVirtualMethods;
        private Vector<MarkupMethodSchema> _foundVirtualBaseMethods;
        protected MarkupTypeSchema _typeExport;
        private int _typeExportIndex;
        private MarkupTypeSchema _foundBaseType;
        private int _foundBaseTypeIndex = -1;
        private Vector<TriggerRecord> _triggerList;
        private Vector<ValidateCode> _actionList;
        private LoadPass _currentValidationPass;
        private string _previewName;
        private static Map<string, TypeSchema> s_classReservedSymbols = new Map<string, TypeSchema>(1);

        public static void InitializeStatics() => s_classReservedSymbols["Class"] = ClassStateSchema.Type;

        public ValidateClass(
          SourceMarkupLoader owner,
          ValidateTypeIdentifier typeIdentifier,
          int line,
          int offset)
          : base(owner, typeIdentifier, line, offset)
        {
        }

        public void NotifyFoundMethodList(ValidateMethodList methodList)
        {
            if (_foundMethods != null)
                ReportError("Methods may only be specified once");
            else
                _foundMethods = methodList;
        }

        public override void NotifyParseComplete()
        {
            _previewName = GetInlinePropertyValueNoValidate("Name");
            string propertyValueNoValidate = GetInlinePropertyValueNoValidate("Base");
            if (propertyValueNoValidate != null && propertyValueNoValidate.IndexOf(':') != -1)
                _baseTypeIdentifier = new ValidateTypeIdentifier(Owner, propertyValueNoValidate, Line, Column);
            Owner.NotifyClassParseComplete(_previewName);
        }

        public void Validate(LoadPass currentPass)
        {
            if (_currentValidationPass >= currentPass)
                return;
            _currentValidationPass = currentPass;
            if (_foundBaseType != null && ((MarkupLoadResult)_foundBaseType.Owner).IsSource)
                ((SourceMarkupLoadResult)_foundBaseType.Owner).ValidateType(_foundBaseType, _currentValidationPass);
            ValidateContext context = new ValidateContext(this, _foundBaseType, _currentValidationPass);
            Validate(TypeRestriction.None, context);
            int num = HasErrors ? 1 : 0;
            if (_currentValidationPass == LoadPass.DeclareTypes)
                DeclareTypes(context);
            else if (_currentValidationPass == LoadPass.PopulatePublicModel)
            {
                PopulatePublicModel(context);
            }
            else
            {
                FullValidation(context);
                TransferToTypeExport(context);
            }
        }

        private void DeclareTypes(ValidateContext context)
        {
            RearrangePropertiesForSymbols();
            _foundScriptsProperty = FindProperty("Scripts", true);
            if (PreviewName == null)
                ReportError("'Name' property is required and must be provided");
            else if (!ValidateContext.IsValidSymbolName(PreviewName))
            {
                ReportError("Invalid name \"{0}\".  Valid names must begin with either an alphabetic character or an underscore and can otherwise contain only alphabetic, numeric, or underscore characters", PreviewName);
            }
            else
            {
                if (Owner.IsTypeNameTaken(PreviewName))
                    ReportError("Type '{0}' was specified more than once", PreviewName);
                _typeExport = MarkupTypeSchema.Build(ObjectType, Owner.LoadResultTarget, PreviewName);
                _typeExportIndex = Owner.RegisterExportedType(_typeExport);
                _typeExport.LoadData = this;
            }
        }

        protected virtual void PopulatePublicModel(ValidateContext context)
        {
            if (_baseTypeIdentifier != null)
            {
                _baseTypeIdentifier.Validate();
                if (_baseTypeIdentifier.HasErrors)
                    MarkHasErrors();
                else if (!(_baseTypeIdentifier.FoundType is MarkupTypeSchema foundType))
                {
                    _baseTypeIdentifier.ReportError("Base type must be a markup-defined type");
                    MarkHasErrors();
                }
                else
                {
                    if (this is ValidateUI)
                    {
                        if (!(foundType is UIClassTypeSchema))
                        {
                            _baseTypeIdentifier.ReportError("Base type must be a markup-defined UI type");
                            MarkHasErrors();
                        }
                    }
                    else if (!(foundType is ClassTypeSchema))
                    {
                        _baseTypeIdentifier.ReportError("Base type must be a markup-defined Class type");
                        MarkHasErrors();
                    }
                    if (!HasErrors)
                    {
                        bool flag = false;
                        for (TypeSchema typeSchema = foundType; typeSchema is MarkupTypeSchema; typeSchema = typeSchema.Base)
                        {
                            if (typeSchema == _typeExport)
                            {
                                _baseTypeIdentifier.ReportError("Circular base class dependency involving '{0}' and '{1}'", _typeExport.Name, foundType.Name);
                                MarkHasErrors();
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            _foundBaseType = foundType;
                            _foundBaseTypeIndex = _baseTypeIdentifier.FoundTypeIndex;
                        }
                    }
                }
            }
            ValidateProperty property1 = FindProperty("Base", true);
            if (property1 != null)
            {
                if (property1.IsFromStringValue)
                {
                    if (_baseTypeIdentifier == null)
                    {
                        property1.ReportError("Base must be a type identifier (prefix:Type)");
                        MarkHasErrors();
                    }
                }
                else
                {
                    property1.ReportError("Property '{0}' does not support expanded value syntax", property1.PropertyName);
                    MarkHasErrors();
                }
            }
            if (_typeExport != null)
                _typeExport.SetBaseType(_foundBaseType);
            bool flag1 = false;
            ValidateProperty property2 = FindProperty("Shared", true);
            if (property2 != null)
            {
                property2.Validate(this, context);
                if (property2.HasErrors)
                    MarkHasErrors();
                else if (property2.IsFromStringValue)
                {
                    flag1 = (bool)((ValidateFromString)property2.Value).FromStringInstance;
                    if (flag1 && this is ValidateUI)
                    {
                        flag1 = false;
                        ReportError("UI cannot be marked as shared");
                    }
                }
                else
                {
                    property2.ReportError("Property '{0}' does not support expanded value syntax", property2.PropertyName);
                    MarkHasErrors();
                }
                if (flag1 && _typeExport != null)
                    ((ClassTypeSchema)_typeExport).MarkShareable();
            }
            ValidateProperty property3 = FindProperty("Properties");
            if (property3 != null)
            {
                if (property3.Value == null || property3.IsObjectTagValue)
                {
                    _foundPropertiesValidateProperty = property3;
                    int length1 = 0;
                    for (ValidateObjectTag next = (ValidateObjectTag)property3.Value; next != null; next = next.Next)
                        ++length1;
                    PropertySchema[] properties = new PropertySchema[length1];
                    int length2 = 0;
                    for (ValidateObjectTag next = (ValidateObjectTag)property3.Value; next != null; next = next.Next)
                    {
                        next.Validate(TypeRestriction.None, context);
                        if (next.ObjectType != null)
                        {
                            string propertyValueNoValidate = next.GetInlinePropertyValueNoValidate("Name");
                            if (propertyValueNoValidate == null)
                            {
                                if (!next.HasErrors)
                                {
                                    next.ReportError("'Name' property is required and must be provided");
                                    MarkHasErrors();
                                }
                            }
                            else if (_typeExport != null)
                            {
                                MarkupPropertySchema propertyExport = MarkupPropertySchema.Build(ObjectType, _typeExport, propertyValueNoValidate, next.ObjectType);
                                properties[length2] = propertyExport;
                                ++length2;
                                next.PropertySchemaExport = propertyExport;
                                propertyExport.SetRequiredForCreation(next.PropertyIsRequiredForCreation);
                                AnnotateProperty(next, context, propertyExport);
                            }
                        }
                    }
                    if (length2 != properties.Length)
                    {
                        PropertySchema[] propertySchemaArray = new PropertySchema[length2];
                        Array.Copy(properties, propertySchemaArray, length2);
                        properties = propertySchemaArray;
                    }
                    if (_typeExport != null)
                        _typeExport.SetPropertyList(properties);
                }
                else
                {
                    property3.ReportError("Property '{0}' values must be in expanded form", property3.PropertyName);
                    MarkHasErrors();
                }
            }
            if (_foundMethods == null)
                return;
            _foundMethods.Validate(this, context);
            if (_foundMethods.FoundMethods == null)
                return;
            _typeExport.SetMethodList(_foundMethods.FoundMethods);
        }

        protected virtual void AnnotateProperty(
          ValidateObjectTag objectTag,
          ValidateContext context,
          MarkupPropertySchema propertyExport)
        {
        }

        protected virtual void FullValidation(ValidateContext context)
        {
            if (_foundPropertiesValidateProperty != null)
                _foundPropertiesValidateProperty.ShouldSkipDictionaryAddIfContains = true;
            if (_foundBaseType is ClassTypeSchema foundBaseType && foundBaseType.IsShared)
                ReportError("Base type cannot be a shared Class");
            ValidateProperty property = FindProperty("Locals");
            if (property != null)
            {
                if (property.Value == null || property.IsObjectTagValue)
                {
                    for (ValidateObjectTag next = (ValidateObjectTag)property.Value; next != null; next = next.Next)
                    {
                        if (next.Name == null && !next.HasErrors)
                        {
                            next.ReportError("'Name' property is required and must be provided");
                            MarkHasErrors();
                        }
                    }
                }
                else
                {
                    property.ReportError("Property '{0}' values must be in expanded form", property.PropertyName);
                    MarkHasErrors();
                }
            }
            if (_foundMethods != null)
                _foundMethods.Validate(this, context);
            if (_foundScriptsProperty != null)
            {
                _scripts = new ValidateScripts(_foundScriptsProperty);
                _scripts.Validate(context);
                if (_scripts.HasErrors)
                    MarkHasErrors();
            }
            if (_foundVirtualMethods == null)
                return;
            MethodSchema[] virtualMethods = new MethodSchema[_foundVirtualMethods.Count];
            for (int index = 0; index < virtualMethods.Length; ++index)
                virtualMethods[index] = _foundVirtualMethods[index];
            _typeExport.SetVirtualMethodList(virtualMethods);
        }

        private void TransferToTypeExport(ValidateContext context)
        {
            if (_typeExport != null)
            {
                _typeExport.SetSymbolReferenceTable(context.SymbolReferenceTable);
                _typeExport.SetInheritableSymbolsTable(context.InheritableSymbolsTable);
                _typeExport.SetListenerCount(context.NotifierCount);
            }
            _triggerList = context.TriggerList;
            _actionList = context.ActionList;
            Map map = new Map();
            for (MarkupTypeSchema typeExport = TypeExport; typeExport != null; typeExport = typeExport.Base as MarkupTypeSchema)
            {
                if (typeExport.InheritableSymbolsTable != null)
                {
                    foreach (SymbolRecord symbolRecord in typeExport.InheritableSymbolsTable)
                    {
                        if (symbolRecord.SymbolOrigin == SymbolOrigin.Locals || symbolRecord.SymbolOrigin == SymbolOrigin.Properties)
                            map[symbolRecord.Name] = null;
                    }
                }
            }
            _typeExport.SetTotalPropertiesAndLocalsCount(map.Count);
        }

        public virtual void RearrangePropertiesForSymbols()
        {
            MovePropertyToFront("Locals");
            MovePropertyToFront("Properties");
        }

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (context.CurrentPass == LoadPass.Full)
                context.DeclareReservedSymbols(s_classReservedSymbols);
            base.Validate(typeRestriction, context);
        }

        public virtual void NotifyDiscoveredObjectTag(ValidateObjectTag objectTag)
        {
        }

        public virtual void NotifyFoundPropertyAssignment(ValidateExpressionCall call)
        {
        }

        public virtual void NotifyFoundMethodCall(ValidateExpressionCall call)
        {
        }

        public void AddVirtualMethod(
          MarkupMethodSchema methodSchema,
          MarkupMethodSchema baseVirtualMethodSchema)
        {
            if (_foundVirtualMethods == null)
            {
                _foundVirtualMethods = new Vector<MarkupMethodSchema>();
                _foundVirtualBaseMethods = new Vector<MarkupMethodSchema>();
            }
            _foundVirtualMethods.Add(methodSchema);
            _foundVirtualBaseMethods.Add(baseVirtualMethodSchema);
        }

        protected override bool ForceAbstractAsConcrete => true;

        public ValidateScripts Scripts => _scripts;

        internal ValidateProperty FoundScripts => _foundScriptsProperty;

        public MarkupTypeSchema TypeExport => _typeExport;

        public int TypeExportIndex => _typeExportIndex;

        public Vector<TriggerRecord> TriggerList => _triggerList;

        public Vector<ValidateCode> ActionList => _actionList;

        public ArrayList MethodList => _foundMethods == null ? null : _foundMethods.Methods;

        public string PreviewName => _previewName;

        public MarkupTypeSchema FoundBaseType => _foundBaseType;

        public int FoundBaseTypeIndex => _foundBaseTypeIndex;

        public ValidateProperty FoundPropertiesValidateProperty => _foundPropertiesValidateProperty;

        public override string ToString() => Name != null ? string.Format("ClassTag : {0}", Name) : "Not validated :" + _previewName;
    }
}
