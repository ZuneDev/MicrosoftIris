// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateUI
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;
using System;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateUI : ValidateClass
    {
        private Vector<ValidateProperty> _foundNamedContentProperties;
        private ValidateProperty _foundContentValidateProperty;
        private int _inlineContentIndex;
        private NamedContentRecord[] _namedContentTable;
        private static Map<string, TypeSchema> s_uiReservedSymbols = new Map<string, TypeSchema>(1);

        public new static void InitializeStatics() => s_uiReservedSymbols["UI"] = UIStateSchema.Type;

        public ValidateUI(
          SourceMarkupLoader owner,
          ValidateTypeIdentifier typeIdentifier,
          int line,
          int offset)
          : base(owner, typeIdentifier, line, offset)
        {
        }

        public override void RearrangePropertiesForSymbols()
        {
            MovePropertyToFront("Content");
            MovePropertyToFront("Input");
            base.RearrangePropertiesForSymbols();
        }

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            if (context.CurrentPass == LoadPass.Full)
                context.DeclareReservedSymbols(s_uiReservedSymbols);
            if (context.CurrentPass == LoadPass.DeclareTypes)
                RemoveNamedContentProperties();
            base.Validate(typeRestriction, context);
        }

        protected override void FullValidation(ValidateContext context)
        {
            base.FullValidation(context);
            ValidateProperty property = FindProperty("Input");
            if (property != null)
            {
                if (property.Value != null && !property.IsObjectTagValue)
                    ReportError("Property '{0}' values must be in expanded form", property.PropertyName);
                else
                    property.ReversePropertyValues();
            }
            if (_foundNamedContentProperties != null)
            {
                context.NotifyScopedLocalFrameEnter();
                context.NotifyScopedLocal("RepeatedItem", ObjectSchema.Type, true, SymbolOrigin.Parameter);
                context.NotifyScopedLocal("RepeatedItemIndex", IndexSchema.Type, true, SymbolOrigin.Parameter);
                _namedContentTable = new NamedContentRecord[_foundNamedContentProperties.Count];
                for (int index = 0; index < _foundNamedContentProperties.Count; ++index)
                {
                    if (index >= _namedContentTable.Length)
                    {
                        NamedContentRecord[] namedContentRecordArray = new NamedContentRecord[_foundNamedContentProperties.Count];
                        _namedContentTable.CopyTo(namedContentRecordArray, 0);
                        _namedContentTable = namedContentRecordArray;
                    }
                    ValidateNamedContent(_foundNamedContentProperties[index], context, index);
                }
                context.NotifyScopedLocalFrameExit();
                if (HasErrors)
                    return;
                ((UIClassTypeSchema)TypeExport).SetNamedContentTable(_namedContentTable);
            }
            _foundContentValidateProperty = FindProperty("Content");
            if (_foundContentValidateProperty == null)
                return;
            bool flag = false;
            for (MarkupTypeSchema markupTypeSchema = FoundBaseType; markupTypeSchema != null; markupTypeSchema = markupTypeSchema.MarkupTypeBase)
            {
                SymbolRecord[] inheritableSymbolsTable = markupTypeSchema.InheritableSymbolsTable;
                for (int index = 0; index < inheritableSymbolsTable.Length; ++index)
                {
                    if (inheritableSymbolsTable[index].SymbolOrigin == SymbolOrigin.Content)
                    {
                        flag = true;
                        string name = inheritableSymbolsTable[index].Name;
                        TypeSchema type = inheritableSymbolsTable[index].Type;
                        SymbolOrigin origin;
                        TypeSchema checkSchema = context.ResolveSymbolNoBaseTypes(name, out origin);
                        if (origin != SymbolOrigin.Content)
                            ReportError("Missing content name declaration: base type '{0}' has declared content name '{1}' of type '{2}'", markupTypeSchema.Name, name, type.Name);
                        else if (!type.IsAssignableFrom(checkSchema))
                            ReportError("Content Name '{0}' exists in base class '{1}' with type '{2}' and type override '{3}' does not match", inheritableSymbolsTable[index].Name, markupTypeSchema.Name, type.Name, checkSchema.Name);
                    }
                }
                if (flag)
                    break;
            }
        }

        public override void NotifyDiscoveredObjectTag(ValidateObjectTag objectTag)
        {
            base.NotifyDiscoveredObjectTag(objectTag);
            if (!RepeaterSchema.Type.IsAssignableFrom(objectTag.FoundType))
                return;
            ExtractInlineContentOnRepeater(objectTag, "Content", "ContentName");
            ExtractInlineContentOnRepeater(objectTag, "Divider", "DividerName");
        }

        private void ExtractInlineContentOnRepeater(
          ValidateObjectTag repeater,
          string contentAttribute,
          string contentNameAttribute)
        {
            ValidateProperty property = repeater.FindProperty(contentAttribute, true);
            if (property == null)
                return;
            string str1 = FoundBaseType == null ? "0" : FoundBaseType.LocallyUniqueId;
            string str2 = string.Format("#Inline{0}{1}.{2}", contentAttribute, str1, _inlineContentIndex++);
            property.RepurposeProperty("Content", new PropertyAttribute("Name", str2));
            if (repeater.FindProperty(contentNameAttribute) == null)
                repeater.AddStringProperty(contentNameAttribute, str2);
            if (_foundNamedContentProperties == null)
                _foundNamedContentProperties = new Vector<ValidateProperty>();
            _foundNamedContentProperties.Add(property);
        }

        private void RemoveNamedContentProperties()
        {
            for (ValidateProperty validateProperty = PropertyList; validateProperty != null; validateProperty = validateProperty.Next)
            {
                if (validateProperty.PropertyName == "Content" && validateProperty.PropertyAttributeList != null)
                {
                    if (_foundNamedContentProperties == null)
                        _foundNamedContentProperties = new Vector<ValidateProperty>();
                    _foundNamedContentProperties.Add(validateProperty);
                }
            }
            if (_foundNamedContentProperties == null)
                return;
            foreach (ValidateProperty namedContentProperty in _foundNamedContentProperties)
                RemoveProperty(namedContentProperty);
        }

        private void ValidateNamedContent(
          ValidateProperty namedContentProperty,
          ValidateContext context,
          int index)
        {
            PropertyAttribute propertyAttributeList = namedContentProperty.PropertyAttributeList;
            if (propertyAttributeList.Name != "Name" || propertyAttributeList.Next != null)
            {
                ReportError("Named Content must have a single 'Name' attribute");
            }
            else
            {
                namedContentProperty.AllowPropertyAttributes();
                namedContentProperty.Validate(this, context);
                if (namedContentProperty.HasErrors)
                    MarkHasErrors();
                NamedContentRecord namedContentRecord = new NamedContentRecord(propertyAttributeList.Value);
                _namedContentTable[index] = namedContentRecord;
            }
        }

        public Vector<ValidateProperty> FoundNamedContentProperties => _foundNamedContentProperties;

        public ValidateProperty FoundContentValidateProperty => _foundContentValidateProperty;
    }
}
