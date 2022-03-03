// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.Validation.ValidateEffectElement
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup.UIX;

namespace Microsoft.Iris.Markup.Validation
{
    internal class ValidateEffectElement : ValidateObjectTag
    {
        public ValidateEffectElement(
          SourceMarkupLoader owner,
          ValidateTypeIdentifier typeIdentifier,
          int line,
          int offset)
          : base(owner, typeIdentifier, line, offset)
        {
        }

        public override void Validate(TypeRestriction typeRestriction, ValidateContext context)
        {
            base.Validate(typeRestriction, context);
            if (context.CurrentPass != LoadPass.Full || !(context.Owner is ValidateEffect owner))
                return;
            ValidateProperty validateProperty1 = PropertyList;
            while (validateProperty1 != null)
            {
                ValidateProperty validateProperty2 = validateProperty1;
                validateProperty1 = validateProperty1.Next;
                if (!validateProperty2.HasErrors && !validateProperty2.IsFromStringValue && !EffectElementSchema.Type.IsAssignableFrom(validateProperty2.FoundProperty.PropertyType) && (!ListSchema.Type.IsAssignableFrom(validateProperty2.FoundProperty.PropertyType) || !EffectElementSchema.Type.IsAssignableFrom(validateProperty2.FoundProperty.AlternateType)))
                {
                    RemoveProperty(validateProperty2);
                    owner.TrackInstanceProperty(context, Name, validateProperty2);
                }
            }
        }
    }
}
