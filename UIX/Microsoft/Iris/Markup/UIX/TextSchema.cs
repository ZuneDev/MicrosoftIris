// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.TextSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using Microsoft.Iris.ViewItems;
using System.Collections;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class TextSchema
    {
        public static UIXTypeSchema Type;

        private static object GetContent(object instanceObj) => ((Text)instanceObj).Content;

        private static void SetContent(ref object instanceObj, object valueObj) => ((Text)instanceObj).Content = (string)valueObj;

        private static object GetFont(object instanceObj) => ((Text)instanceObj).Font;

        private static void SetFont(ref object instanceObj, object valueObj) => ((Text)instanceObj).Font = (Font)valueObj;

        private static object GetColor(object instanceObj) => ((Text)instanceObj).Color;

        private static void SetColor(ref object instanceObj, object valueObj) => ((Text)instanceObj).Color = (Color)valueObj;

        private static object GetWordWrap(object instanceObj) => BooleanBoxes.Box(((Text)instanceObj).WordWrap);

        private static void SetWordWrap(ref object instanceObj, object valueObj) => ((Text)instanceObj).WordWrap = (bool)valueObj;

        private static object GetMaximumLines(object instanceObj) => ((Text)instanceObj).MaximumLines;

        private static void SetMaximumLines(ref object instanceObj, object valueObj)
        {
            Text text = (Text)instanceObj;
            int num = (int)valueObj;
            Result result = Int32Schema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                text.MaximumLines = num;
        }

        private static object GetLineAlignment(object instanceObj) => ((Text)instanceObj).LineAlignment;

        private static void SetLineAlignment(ref object instanceObj, object valueObj) => ((Text)instanceObj).LineAlignment = (LineAlignment)valueObj;

        private static object GetLineSpacing(object instanceObj) => ((Text)instanceObj).LineSpacing;

        private static void SetLineSpacing(ref object instanceObj, object valueObj) => ((Text)instanceObj).LineSpacing = (float)valueObj;

        private static object GetCharacterSpacing(object instanceObj) => ((Text)instanceObj).CharacterSpacing;

        private static void SetCharacterSpacing(ref object instanceObj, object valueObj) => ((Text)instanceObj).CharacterSpacing = (float)valueObj;

        private static object GetEnableKerning(object instanceObj) => BooleanBoxes.Box(((Text)instanceObj).EnableKerning);

        private static void SetEnableKerning(ref object instanceObj, object valueObj) => ((Text)instanceObj).EnableKerning = (bool)valueObj;

        private static object GetLastLineBounds(object instanceObj) => ((Text)instanceObj).LastLineBounds;

        private static object GetFadeSize(object instanceObj) => ((Text)instanceObj).FadeSize;

        private static void SetFadeSize(ref object instanceObj, object valueObj) => ((Text)instanceObj).FadeSize = (float)valueObj;

        private static object GetStyle(object instanceObj) => ((Text)instanceObj).Style;

        private static void SetStyle(ref object instanceObj, object valueObj) => ((Text)instanceObj).Style = (TextStyle)valueObj;

        private static object GetNamedStyles(object instanceObj) => ((Text)instanceObj).NamedStyles;

        private static void SetNamedStyles(ref object instanceObj, object valueObj) => ((Text)instanceObj).NamedStyles = (IDictionary)valueObj;

        private static object GetFragments(object instanceObj) => ((Text)instanceObj).Fragments;

        private static object GetTextSharpness(object instanceObj) => ((Text)instanceObj).TextSharpness;

        private static void SetTextSharpness(ref object instanceObj, object valueObj) => ((Text)instanceObj).TextSharpness = (TextSharpness)valueObj;

        private static object GetClipped(object instanceObj) => BooleanBoxes.Box(((Text)instanceObj).Clipped);

        private static object GetContributesToWidth(object instanceObj) => BooleanBoxes.Box(((Text)instanceObj).ContributesToWidth);

        private static void SetContributesToWidth(ref object instanceObj, object valueObj) => ((Text)instanceObj).ContributesToWidth = (bool)valueObj;

        private static object GetBoundsType(object instanceObj) => ((Text)instanceObj).BoundsType;

        private static void SetBoundsType(ref object instanceObj, object valueObj) => ((Text)instanceObj).BoundsType = (TextBounds)valueObj;

        private static object GetEffect(object instanceObj) => ((ViewItem)instanceObj).Effect;

        private static void SetEffect(ref object instanceObj, object valueObj) => ((ViewItem)instanceObj).Effect = (EffectClass)valueObj;

        private static object GetDisableIme(object instanceObj) => BooleanBoxes.Box(((Text)instanceObj).DisableIme);

        private static void SetDisableIme(ref object instanceObj, object valueObj) => ((Text)instanceObj).DisableIme = (bool)valueObj;

        private static object GetHighlightColor(object instanceObj) => ((Text)instanceObj).HighlightColor;

        private static void SetHighlightColor(ref object instanceObj, object valueObj) => ((Text)instanceObj).HighlightColor = (Color)valueObj;

        private static object GetTextHighlightColor(object instanceObj) => ((Text)instanceObj).TextHighlightColor;

        private static void SetTextHighlightColor(ref object instanceObj, object valueObj) => ((Text)instanceObj).TextHighlightColor = (Color)valueObj;

        private static object GetUsePasswordMask(object instanceObj) => BooleanBoxes.Box(((Text)instanceObj).UsePasswordMask);

        private static void SetUsePasswordMask(ref object instanceObj, object valueObj) => ((Text)instanceObj).UsePasswordMask = (bool)valueObj;

        private static object GetPasswordMask(object instanceObj) => ((Text)instanceObj).PasswordMask;

        private static void SetPasswordMask(ref object instanceObj, object valueObj) => ((Text)instanceObj).PasswordMask = (char)valueObj;

        private static object Construct() => new Text();

        public static void Pass1Initialize() => Type = new UIXTypeSchema(212, "Text", null, 239, typeof(Text), UIXTypeFlags.Disposable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(212, "Content", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetContent), new SetValueHandler(SetContent), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(212, "Font", 93, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetFont), new SetValueHandler(SetFont), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(212, "Color", 35, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetColor), new SetValueHandler(SetColor), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(212, "WordWrap", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetWordWrap), new SetValueHandler(SetWordWrap), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(212, "MaximumLines", 115, -1, ExpressionRestriction.None, false, Int32Schema.ValidateNotNegative, true, new GetValueHandler(GetMaximumLines), new SetValueHandler(SetMaximumLines), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(212, "LineAlignment", 137, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLineAlignment), new SetValueHandler(SetLineAlignment), false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(212, "LineSpacing", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetLineSpacing), new SetValueHandler(SetLineSpacing), false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(212, "CharacterSpacing", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetCharacterSpacing), new SetValueHandler(SetCharacterSpacing), false);
            UIXPropertySchema uixPropertySchema9 = new UIXPropertySchema(212, "EnableKerning", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEnableKerning), new SetValueHandler(SetEnableKerning), false);
            UIXPropertySchema uixPropertySchema10 = new UIXPropertySchema(212, "LastLineBounds", 169, -1, ExpressionRestriction.ReadOnly, false, null, true, new GetValueHandler(GetLastLineBounds), null, false);
            UIXPropertySchema uixPropertySchema11 = new UIXPropertySchema(212, "FadeSize", 194, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetFadeSize), new SetValueHandler(SetFadeSize), false);
            UIXPropertySchema uixPropertySchema12 = new UIXPropertySchema(212, "Style", 220, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetStyle), new SetValueHandler(SetStyle), false);
            UIXPropertySchema uixPropertySchema13 = new UIXPropertySchema(212, "NamedStyles", 58, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetNamedStyles), new SetValueHandler(SetNamedStyles), false);
            UIXPropertySchema uixPropertySchema14 = new UIXPropertySchema(212, "Fragments", 138, 215, ExpressionRestriction.ReadOnly, false, null, true, new GetValueHandler(GetFragments), null, false);
            UIXPropertySchema uixPropertySchema15 = new UIXPropertySchema(212, "TextSharpness", 219, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetTextSharpness), new SetValueHandler(SetTextSharpness), false);
            UIXPropertySchema uixPropertySchema16 = new UIXPropertySchema(212, "Clipped", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetClipped), null, false);
            UIXPropertySchema uixPropertySchema17 = new UIXPropertySchema(212, "ContributesToWidth", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetContributesToWidth), new SetValueHandler(SetContributesToWidth), false);
            UIXPropertySchema uixPropertySchema18 = new UIXPropertySchema(212, "BoundsType", 213, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetBoundsType), new SetValueHandler(SetBoundsType), false);
            UIXPropertySchema uixPropertySchema19 = new UIXPropertySchema(212, "Effect", 78, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetEffect), new SetValueHandler(SetEffect), false);
            UIXPropertySchema uixPropertySchema20 = new UIXPropertySchema(212, "DisableIme", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetDisableIme), new SetValueHandler(SetDisableIme), false);
            UIXPropertySchema uixPropertySchema21 = new UIXPropertySchema(212, "HighlightColor", 35, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetHighlightColor), new SetValueHandler(SetHighlightColor), false);
            UIXPropertySchema uixPropertySchema22 = new UIXPropertySchema(212, "TextHighlightColor", 35, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetTextHighlightColor), new SetValueHandler(SetTextHighlightColor), false);
            UIXPropertySchema uixPropertySchema23 = new UIXPropertySchema(212, "UsePasswordMask", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetUsePasswordMask), new SetValueHandler(SetUsePasswordMask), false);
            UIXPropertySchema uixPropertySchema24 = new UIXPropertySchema(212, "PasswordMask", 27, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetPasswordMask), new SetValueHandler(SetPasswordMask), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[24]
            {
         uixPropertySchema18,
         uixPropertySchema8,
         uixPropertySchema16,
         uixPropertySchema3,
         uixPropertySchema1,
         uixPropertySchema17,
         uixPropertySchema20,
         uixPropertySchema19,
         uixPropertySchema9,
         uixPropertySchema11,
         uixPropertySchema2,
         uixPropertySchema14,
         uixPropertySchema21,
         uixPropertySchema10,
         uixPropertySchema6,
         uixPropertySchema7,
         uixPropertySchema5,
         uixPropertySchema13,
         uixPropertySchema24,
         uixPropertySchema12,
         uixPropertySchema22,
         uixPropertySchema15,
         uixPropertySchema23,
         uixPropertySchema4
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
