// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TextStyleSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TextStyleSchema
    {
        public static RangeValidator ValidateFontFace = new RangeValidator(RangeValidateFontFace);
        public static UIXTypeSchema Type;

        private static object GetFontFace(object instanceObj) => ((TextStyle)instanceObj).FontFace;

        private static void SetFontFace(ref object instanceObj, object valueObj)
        {
            TextStyle textStyle = (TextStyle)instanceObj;
            string str = (string)valueObj;
            Result result = ValidateFontFace(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                textStyle.FontFace = str;
        }

        private static object GetFontSize(object instanceObj) => ((TextStyle)instanceObj).FontSize;

        private static void SetFontSize(ref object instanceObj, object valueObj)
        {
            TextStyle textStyle = (TextStyle)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                textStyle.FontSize = num;
        }

        private static object GetBold(object instanceObj) => BooleanBoxes.Box(((TextStyle)instanceObj).Bold);

        private static void SetBold(ref object instanceObj, object valueObj) => ((TextStyle)instanceObj).Bold = (bool)valueObj;

        private static object GetItalic(object instanceObj) => BooleanBoxes.Box(((TextStyle)instanceObj).Italic);

        private static void SetItalic(ref object instanceObj, object valueObj) => ((TextStyle)instanceObj).Italic = (bool)valueObj;

        private static object GetUnderline(object instanceObj) => BooleanBoxes.Box(((TextStyle)instanceObj).Underline);

        private static void SetUnderline(ref object instanceObj, object valueObj) => ((TextStyle)instanceObj).Underline = (bool)valueObj;

        private static object GetColor(object instanceObj) => ((TextStyle)instanceObj).Color;

        private static void SetColor(ref object instanceObj, object valueObj) => ((TextStyle)instanceObj).Color = (Color)valueObj;

        private static object GetLineSpacing(object instanceObj) => ((TextStyle)instanceObj).LineSpacing;

        private static void SetLineSpacing(ref object instanceObj, object valueObj)
        {
            TextStyle textStyle = (TextStyle)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                textStyle.LineSpacing = num;
        }

        private static object GetEnableKerning(object instanceObj) => BooleanBoxes.Box(((TextStyle)instanceObj).EnableKerning);

        private static void SetEnableKerning(ref object instanceObj, object valueObj) => ((TextStyle)instanceObj).EnableKerning = (bool)valueObj;

        private static object GetCharacterSpacing(object instanceObj) => ((TextStyle)instanceObj).CharacterSpacing;

        private static void SetCharacterSpacing(ref object instanceObj, object valueObj) => ((TextStyle)instanceObj).CharacterSpacing = (float)valueObj;

        private static object GetFragment(object instanceObj) => BooleanBoxes.Box(((TextStyle)instanceObj).Fragment);

        private static void SetFragment(ref object instanceObj, object valueObj) => ((TextStyle)instanceObj).Fragment = (bool)valueObj;

        private static object Construct() => new TextStyle();

        private static Result RangeValidateFontFace(object value) => Result.Success;

        public static void Pass1Initialize() => Type = new UIXTypeSchema(220, "TextStyle", null, 153, typeof(TextStyle), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(220, "FontFace", 208, -1, ExpressionRestriction.None, false, ValidateFontFace, true, new GetValueHandler(GetFontFace), new SetValueHandler(SetFontFace), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(220, "FontSize", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, true, new GetValueHandler(GetFontSize), new SetValueHandler(SetFontSize), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(220, "Bold", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetBold), new SetValueHandler(SetBold), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(220, "Italic", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetItalic), new SetValueHandler(SetItalic), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(220, "Underline", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetUnderline), new SetValueHandler(SetUnderline), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(220, "Color", 35, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetColor), new SetValueHandler(SetColor), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(220, "LineSpacing", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, true, new GetValueHandler(GetLineSpacing), new SetValueHandler(SetLineSpacing), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(220, "EnableKerning", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEnableKerning), new SetValueHandler(SetEnableKerning), false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(220, "CharacterSpacing", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCharacterSpacing), new SetValueHandler(SetCharacterSpacing), false);
            UIXPropertySchema uixPropertySchema10 = new UIXPropertySchema(220, "Fragment", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetFragment), new SetValueHandler(SetFragment), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[10]
            {
         uixPropertySchema3,
         uixPropertySchema9,
         uixPropertySchema6,
         uixPropertySchema8,
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema10,
         uixPropertySchema4,
         uixPropertySchema7,
         uixPropertySchema5
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
