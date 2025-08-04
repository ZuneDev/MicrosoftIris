// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.FontSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.OS;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class FontSchema
    {
        public static RangeValidator ValidateFontName = new RangeValidator(RangeValidateFontName);
        public static UIXTypeSchema Type;

        private static object GetFontName(object instanceObj) => ((Font)instanceObj).FontName;

        private static void SetFontName(ref object instanceObj, object valueObj)
        {
            Font font = (Font)instanceObj;
            string str = (string)valueObj;
            Result result = ValidateFontName(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                font.FontName = str;
        }

        private static object GetFontSize(object instanceObj) => ((Font)instanceObj).FontSize;

        private static void SetFontSize(ref object instanceObj, object valueObj)
        {
            Font font = (Font)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                font.FontSize = num;
        }

        private static object GetAltFontSize(object instanceObj) => ((Font)instanceObj).AltFontSize;

        private static void SetAltFontSize(ref object instanceObj, object valueObj)
        {
            Font font = (Font)instanceObj;
            float num = (float)valueObj;
            Result result = SingleSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                font.AltFontSize = num;
        }

        private static object GetFontStyle(object instanceObj) => ((Font)instanceObj).FontStyle;

        private static void SetFontStyle(ref object instanceObj, object valueObj) => ((Font)instanceObj).FontStyle = (FontStyles)valueObj;

        private static object Construct() => new Font();

        private static object ConstructFontName(object[] parameters)
        {
            object instanceObj = Construct();
            SetFontName(ref instanceObj, parameters[0]);
            return instanceObj;
        }

        private static Result ConvertFromStringFontName(string[] splitString, out object instance)
        {
            instance = Construct();
            object valueObj;
            Result result = UIXLoadResult.ValidateStringAsValue(splitString[0], StringSchema.Type, ValidateFontName, out valueObj);
            if (result.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Font", result.Error);
            SetFontName(ref instance, valueObj);
            return result;
        }

        private static object ConstructFontNameFontSize(object[] parameters)
        {
            object instanceObj = Construct();
            SetFontName(ref instanceObj, parameters[0]);
            SetFontSize(ref instanceObj, parameters[1]);
            return instanceObj;
        }

        private static Result ConvertFromStringFontNameFontSize(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], StringSchema.Type, ValidateFontName, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Font", result1.Error);
            SetFontName(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], SingleSchema.Type, SingleSchema.ValidateNotNegative, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Font", result2.Error);
            SetFontSize(ref instance, valueObj2);
            return result2;
        }

        private static object ConstructFontNameFontSizeAltFontSize(object[] parameters)
        {
            object instanceObj = Construct();
            SetFontName(ref instanceObj, parameters[0]);
            SetFontSize(ref instanceObj, parameters[1]);
            SetAltFontSize(ref instanceObj, parameters[2]);
            return instanceObj;
        }

        private static Result ConvertFromStringFontNameFontSizeAltFontSize(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], StringSchema.Type, ValidateFontName, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Font", result1.Error);
            SetFontName(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], SingleSchema.Type, SingleSchema.ValidateNotNegative, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Font", result2.Error);
            SetFontSize(ref instance, valueObj2);
            object valueObj3;
            Result result3 = UIXLoadResult.ValidateStringAsValue(splitString[2], SingleSchema.Type, SingleSchema.ValidateNotNegative, out valueObj3);
            if (result3.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Font", result3.Error);
            SetAltFontSize(ref instance, valueObj3);
            return result3;
        }

        private static object ConstructFontNameFontSizeFontStyle(object[] parameters)
        {
            object instanceObj = Construct();
            SetFontName(ref instanceObj, parameters[0]);
            SetFontSize(ref instanceObj, parameters[1]);
            SetFontStyle(ref instanceObj, parameters[2]);
            return instanceObj;
        }

        private static Result ConvertFromStringFontNameFontSizeFontStyle(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], StringSchema.Type, ValidateFontName, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Font", result1.Error);
            SetFontName(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], SingleSchema.Type, SingleSchema.ValidateNotNegative, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Font", result2.Error);
            SetFontSize(ref instance, valueObj2);
            object valueObj3;
            Result result3 = UIXLoadResult.ValidateStringAsValue(splitString[2], UIXLoadResultExports.FontStylesType, null, out valueObj3);
            if (result3.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Font", result3.Error);
            SetFontStyle(ref instance, valueObj3);
            return result3;
        }

        private static object ConstructFontNameFontSizeAltFontSizeFontStyle(object[] parameters)
        {
            object instanceObj = Construct();
            SetFontName(ref instanceObj, parameters[0]);
            SetFontSize(ref instanceObj, parameters[1]);
            SetAltFontSize(ref instanceObj, parameters[2]);
            SetFontStyle(ref instanceObj, parameters[3]);
            return instanceObj;
        }

        private static Result ConvertFromStringFontNameFontSizeAltFontSizeFontStyle(
          string[] splitString,
          out object instance)
        {
            instance = Construct();
            object valueObj1;
            Result result1 = UIXLoadResult.ValidateStringAsValue(splitString[0], StringSchema.Type, ValidateFontName, out valueObj1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Font", result1.Error);
            SetFontName(ref instance, valueObj1);
            object valueObj2;
            Result result2 = UIXLoadResult.ValidateStringAsValue(splitString[1], SingleSchema.Type, SingleSchema.ValidateNotNegative, out valueObj2);
            if (result2.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Font", result2.Error);
            SetFontSize(ref instance, valueObj2);
            object valueObj3;
            Result result3 = UIXLoadResult.ValidateStringAsValue(splitString[2], SingleSchema.Type, SingleSchema.ValidateNotNegative, out valueObj3);
            if (result3.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Font", result3.Error);
            SetAltFontSize(ref instance, valueObj3);
            object valueObj4;
            Result result4 = UIXLoadResult.ValidateStringAsValue(splitString[3], UIXLoadResultExports.FontStylesType, null, out valueObj4);
            if (result4.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Font", result4.Error);
            SetFontStyle(ref instance, valueObj4);
            return result4;
        }

        private static object CallLoadFontResourceStringString(object instanceObj, object[] parameters)
        {
            string moduleName = (string)parameters[0];
            string resourceName = (string)parameters[1];
            if (string.IsNullOrEmpty(moduleName))
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "moduleName");
            if (string.IsNullOrEmpty(resourceName))
                ErrorManager.ReportError("Script runtime failure: Invalid 'null' value for '{0}'", "resourceName");

            bool error;

            // UIXRender doesn't know how to load resources from .NET assemblies, so we'll
            // call the relevant GDI API manually for CLR DLLs.
            var assemblyName = System.IO.Path.GetFileNameWithoutExtension(moduleName);

            if (ClrDllResources.Instance.TryGetResource($"{assemblyName}!{resourceName}", $"clr-res://{assemblyName}", true, out var resource))
            {
                resource.Acquire();

                uint cFonts = 0;
                var hFont = Win32Api.AddFontMemResourceEx(resource.Buffer, resource.Length, System.IntPtr.Zero, ref cFonts);

                error = hFont == System.IntPtr.Zero;
            }
            else
            {
                error = !NativeApi.SpLoadFontResource(moduleName, resourceName);
            }

            if (error)
                ErrorManager.ReportError("Font Resource {1} not found in module {0}", moduleName, resourceName);

            return null;
        }

        private static bool IsConversionSupported(TypeSchema fromType) => StringSchema.Type.IsAssignableFrom(fromType);

        private static Result TryConvertFrom(
          object from,
          TypeSchema fromType,
          out object instance)
        {
            Result result1 = Result.Fail("Unsupported");
            instance = null;
            if (StringSchema.Type.IsAssignableFrom(fromType))
            {
                string[] splitString = StringUtility.SplitAndTrim(',', (string)from);
                switch (splitString.Length)
                {
                    case 1:
                        result1 = ConvertFromStringFontName(splitString, out instance);
                        if (!result1.Failed)
                            return result1;
                        break;
                    case 2:
                        result1 = ConvertFromStringFontNameFontSize(splitString, out instance);
                        if (!result1.Failed)
                            return result1;
                        break;
                    case 3:
                        Result result2 = ConvertFromStringFontNameFontSizeAltFontSize(splitString, out instance);
                        if (!result2.Failed)
                            return result2;
                        result1 = ConvertFromStringFontNameFontSizeFontStyle(splitString, out instance);
                        if (!result1.Failed)
                            return result1;
                        break;
                    case 4:
                        result1 = ConvertFromStringFontNameFontSizeAltFontSizeFontStyle(splitString, out instance);
                        if (!result1.Failed)
                            return result1;
                        break;
                    default:
                        result1 = Result.Fail("Unable to convert \"{0}\" to type '{1}'", from.ToString(), "Font");
                        break;
                }
            }
            return result1;
        }

        private static Result RangeValidateFontName(object value)
        {
            return value is not string
                ? Result.Fail("Script runtime failure: Invalid '{0}' value for '{1}'", value ?? "null", "FontName")
                : Result.Success;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(93, "Font", null, 153, typeof(Font), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(93, "FontName", 208, -1, ExpressionRestriction.None, false, ValidateFontName, false, new GetValueHandler(GetFontName), new SetValueHandler(SetFontName), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(93, "FontSize", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, false, new GetValueHandler(GetFontSize), new SetValueHandler(SetFontSize), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(93, "AltFontSize", 194, -1, ExpressionRestriction.None, false, SingleSchema.ValidateNotNegative, false, new GetValueHandler(GetAltFontSize), new SetValueHandler(SetAltFontSize), false);
            UIXPropertySchema uixPropertySchema4 = new UIXPropertySchema(93, "FontStyle", 94, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetFontStyle), new SetValueHandler(SetFontStyle), false);
            UIXConstructorSchema constructorSchema1 = new UIXConstructorSchema(93, new short[1]
            {
         208
            }, new ConstructHandler(ConstructFontName));
            UIXConstructorSchema constructorSchema2 = new UIXConstructorSchema(93, new short[2]
            {
         208,
         194
            }, new ConstructHandler(ConstructFontNameFontSize));
            UIXConstructorSchema constructorSchema3 = new UIXConstructorSchema(93, new short[3]
            {
         208,
         194,
         194
            }, new ConstructHandler(ConstructFontNameFontSizeAltFontSize));
            UIXConstructorSchema constructorSchema4 = new UIXConstructorSchema(93, new short[3]
            {
         208,
         194,
         94
            }, new ConstructHandler(ConstructFontNameFontSizeFontStyle));
            UIXConstructorSchema constructorSchema5 = new UIXConstructorSchema(93, new short[4]
            {
         208,
         194,
         194,
         94
            }, new ConstructHandler(ConstructFontNameFontSizeAltFontSizeFontStyle));
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(93, "LoadFontResource", new short[2]
            {
         208,
         208
            }, 240, new InvokeHandler(CallLoadFontResourceStringString), true);
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[5]
            {
         constructorSchema1,
         constructorSchema2,
         constructorSchema3,
         constructorSchema4,
         constructorSchema5
            }, new PropertySchema[4]
            {
         uixPropertySchema3,
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema4
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), null, null, null, null);
        }
    }
}
