// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ImageSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ImageSchema
    {
        public static UIXTypeSchema Type;

        private static object GetSource(object instanceObj) => ((UIImage)instanceObj).Source;

        private static void SetSource(ref object instanceObj, object valueObj) => ((UIImage)instanceObj).Source = (string)valueObj;

        private static object GetNineGrid(object instanceObj) => ((UIImage)instanceObj).NineGrid;

        private static void SetNineGrid(ref object instanceObj, object valueObj) => ((UIImage)instanceObj).NineGrid = (Inset)valueObj;

        private static object GetMaximumSize(object instanceObj) => ((UIImage)instanceObj).MaximumSize;

        private static void SetMaximumSize(ref object instanceObj, object valueObj)
        {
            UIImage uiImage = (UIImage)instanceObj;
            Size size = (Size)valueObj;
            Result result = SizeSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                uiImage.MaximumSize = size;
        }

        private static object GetFlippable(object instanceObj) => BooleanBoxes.Box(((UIImage)instanceObj).Flippable);

        private static void SetFlippable(ref object instanceObj, object valueObj) => ((UIImage)instanceObj).Flippable = (bool)valueObj;

        private static object GetAntialiasEdges(object instanceObj) => BooleanBoxes.Box(((UIImage)instanceObj).AntialiasEdges);

        private static void SetAntialiasEdges(ref object instanceObj, object valueObj) => ((UIImage)instanceObj).AntialiasEdges = (bool)valueObj;

        private static object GetStatus(object instanceObj) => ((UIImage)instanceObj).Status;

        private static object GetWidth(object instanceObj) => ((UIImage)instanceObj).Width;

        private static object GetHeight(object instanceObj) => ((UIImage)instanceObj).Height;

        private static object Construct() => new UriImage();

        private static object CallLoad(object instanceObj, object[] parameters)
        {
            ((UIImage)instanceObj).Load();
            return null;
        }

        private static object ConstructSource(object[] parameters)
        {
            object instanceObj = Construct();
            SetSource(ref instanceObj, parameters[0]);
            return instanceObj;
        }

        private static Result ConvertFromStringSource(string[] splitString, out object instance)
        {
            instance = Construct();
            object valueObj;
            Result result = UIXLoadResult.ValidateStringAsValue(splitString[0], StringSchema.Type, null, out valueObj);
            if (result.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result.Error);
            SetSource(ref instance, valueObj);
            return result;
        }

        private static object ConstructSourceNineGrid(object[] parameters)
        {
            object instanceObj = Construct();
            SetSource(ref instanceObj, parameters[0]);
            SetNineGrid(ref instanceObj, parameters[1]);
            return instanceObj;
        }

        private static Result ConvertFromStringSourceNineGrid(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], StringSchema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result1.Error);
            SetSource(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], InsetSchema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result2.Error);
            SetNineGrid(ref instance, valueObj2);
            return result2;
        }

        private static object ConstructSourceNineGridMaximumSize(object[] parameters)
        {
            object instanceObj = Construct();
            SetSource(ref instanceObj, parameters[0]);
            SetNineGrid(ref instanceObj, parameters[1]);
            SetMaximumSize(ref instanceObj, parameters[2]);
            return instanceObj;
        }

        private static Result ConvertFromStringSourceNineGridMaximumSize(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], StringSchema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result1.Error);
            SetSource(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], InsetSchema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result2.Error);
            SetNineGrid(ref instance, valueObj2);
            object valueObj3;
            Result result3 = UIXLoadResult.ValidateStringAsValue(splitString[2], SizeSchema.Type, SizeSchema.ValidateNotNegative, out valueObj3);
            if (result3.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result3.Error);
            SetMaximumSize(ref instance, valueObj3);
            return result3;
        }

        private static object ConstructSourceNineGridMaximumSizeFlippable(object[] parameters)
        {
            object instanceObj = Construct();
            SetSource(ref instanceObj, parameters[0]);
            SetNineGrid(ref instanceObj, parameters[1]);
            SetMaximumSize(ref instanceObj, parameters[2]);
            SetFlippable(ref instanceObj, parameters[3]);
            return instanceObj;
        }

        private static Result ConvertFromStringSourceNineGridMaximumSizeFlippable(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], StringSchema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result1.Error);
            SetSource(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], InsetSchema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result2.Error);
            SetNineGrid(ref instance, valueObj2);
            object valueObj3;
            Result result3 = UIXLoadResult.ValidateStringAsValue(splitString[2], SizeSchema.Type, SizeSchema.ValidateNotNegative, out valueObj3);
            if (result3.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result3.Error);
            SetMaximumSize(ref instance, valueObj3);
            object valueObj4;
            Result result4 = UIXLoadResult.ValidateStringAsValue(splitString[3], BooleanSchema.Type, null, out valueObj4);
            if (result4.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result4.Error);
            SetFlippable(ref instance, valueObj4);
            return result4;
        }

        private static object ConstructSourceNineGridMaximumSizeFlippableAntialiasEdges(
          object[] parameters)
        {
            object instanceObj = Construct();
            SetSource(ref instanceObj, parameters[0]);
            SetNineGrid(ref instanceObj, parameters[1]);
            SetMaximumSize(ref instanceObj, parameters[2]);
            SetFlippable(ref instanceObj, parameters[3]);
            SetAntialiasEdges(ref instanceObj, parameters[4]);
            return instanceObj;
        }

        private static Result ConvertFromStringSourceNineGridMaximumSizeFlippableAntialiasEdges(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], StringSchema.Type, null, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result1.Error);
            SetSource(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], InsetSchema.Type, null, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result2.Error);
            SetNineGrid(ref instance, valueObj2);
            object valueObj3;
            Result result3 = UIXLoadResult.ValidateStringAsValue(splitString[2], SizeSchema.Type, SizeSchema.ValidateNotNegative, out valueObj3);
            if (result3.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result3.Error);
            SetMaximumSize(ref instance, valueObj3);
            object valueObj4;
            Result result4 = UIXLoadResult.ValidateStringAsValue(splitString[3], BooleanSchema.Type, null, out valueObj4);
            if (result4.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result4.Error);
            SetFlippable(ref instance, valueObj4);
            object valueObj5;
            Result result5 = UIXLoadResult.ValidateStringAsValue(splitString[4], BooleanSchema.Type, null, out valueObj5);
            if (result5.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Image", result5.Error);
            SetAntialiasEdges(ref instance, valueObj5);
            return result5;
        }

        private static bool IsConversionSupported(TypeSchema fromType) => StringSchema.Type.IsAssignableFrom(fromType);

        private static Result TryConvertFrom(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            Result result = Result.Fail("Unsupported");
            instance = null;
            if (StringSchema.Type.IsAssignableFrom(fromType))
            {
                string[] splitString = StringUtility.SplitAndTrim(',', (string)from);
                switch (splitString.Length)
                {
                    case 1:
                        result = ConvertFromStringSource(splitString, out instance);
                        if (!result.Failed)
                            return result;
                        break;
                    case 2:
                        result = ConvertFromStringSourceNineGrid(splitString, out instance);
                        if (!result.Failed)
                            return result;
                        break;
                    case 3:
                        result = ConvertFromStringSourceNineGridMaximumSize(splitString, out instance);
                        if (!result.Failed)
                            return result;
                        break;
                    case 4:
                        result = ConvertFromStringSourceNineGridMaximumSizeFlippable(splitString, out instance);
                        if (!result.Failed)
                            return result;
                        break;
                    case 5:
                        result = ConvertFromStringSourceNineGridMaximumSizeFlippableAntialiasEdges(splitString, out instance);
                        if (!result.Failed)
                            return result;
                        break;
                    default:
                        result = Result.Fail("Unable to convert \"{0}\" to type '{1}'", from.ToString(), "Image");
                        break;
                }
            }
            return result;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(105, "Image", null, 153, typeof(UIImage), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(105, "Source", 208, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetSource), new SetValueHandler(SetSource), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(105, "NineGrid", 114, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetNineGrid), new SetValueHandler(SetNineGrid), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(105, "MaximumSize", 195, -1, ExpressionRestriction.None, false, SizeSchema.ValidateNotNegative, false, new GetValueHandler(GetMaximumSize), new SetValueHandler(SetMaximumSize), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(105, "Flippable", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetFlippable), new SetValueHandler(SetFlippable), false);
            UIXPropertySchema uixPropertySchema5 = new UIXPropertySchema(105, "AntialiasEdges", 15, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAntialiasEdges), new SetValueHandler(SetAntialiasEdges), false);
            UIXPropertySchema uixPropertySchema6 = new UIXPropertySchema(105, "Status", 108, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetStatus), null, false);
            UIXPropertySchema uixPropertySchema7 = new UIXPropertySchema(105, "Width", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetWidth), null, false);
            UIXPropertySchema uixPropertySchema8 = new UIXPropertySchema(105, "Height", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetHeight), null, false);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(105, "Load", null, 240, new InvokeHandler(CallLoad), false);
            UIXConstructorSchema constructorSchema1 = new UIXConstructorSchema(105, new short[1]
            {
         208
            }, new ConstructHandler(ConstructSource));
            UIXConstructorSchema constructorSchema2 = new UIXConstructorSchema(105, new short[2]
            {
         208,
         114
            }, new ConstructHandler(ConstructSourceNineGrid));
            UIXConstructorSchema constructorSchema3 = new UIXConstructorSchema(105, new short[3]
            {
         208,
         114,
         195
            }, new ConstructHandler(ConstructSourceNineGridMaximumSize));
            UIXConstructorSchema constructorSchema4 = new UIXConstructorSchema(105, new short[4]
            {
         208,
         114,
         195,
         15
            }, new ConstructHandler(ConstructSourceNineGridMaximumSizeFlippable));
            UIXConstructorSchema constructorSchema5 = new UIXConstructorSchema(105, new short[5]
            {
         208,
         114,
         195,
         15,
         15
            }, new ConstructHandler(ConstructSourceNineGridMaximumSizeFlippableAntialiasEdges));
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[5]
            {
         constructorSchema1,
         constructorSchema2,
         constructorSchema3,
         constructorSchema4,
         constructorSchema5
            }, new PropertySchema[8]
            {
         uixPropertySchema5,
         uixPropertySchema4,
         uixPropertySchema8,
         uixPropertySchema3,
         uixPropertySchema2,
         uixPropertySchema1,
         uixPropertySchema6,
         uixPropertySchema7
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), null, null, null, null);
        }
    }
}
