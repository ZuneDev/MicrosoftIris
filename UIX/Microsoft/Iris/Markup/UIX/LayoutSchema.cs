// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.LayoutSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class LayoutSchema
    {
        public static UIXTypeSchema Type;

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string str = (string)valueObj;
            instanceObj = null;
            
            if (!PredefinedLayouts.TryGetFromName(str, out var layout))
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", str, "Layout");
            
            instanceObj = layout;
            return Result.Success;
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
                result = ConvertFromString(from, out instance);
                if (!result.Failed)
                    return result;
            }
            return result;
        }

        private static object CallTryParseStringLayout(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            ILayout parameter2 = (ILayout)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(132, "Layout", null, 153, typeof(ILayout), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(132, "TryParse", new short[2]
            {
         208,
         132
            }, 132, new InvokeHandler(CallTryParseStringLayout), true);
            Type.Initialize(null, null, null, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), null, null, null, null);
        }
    }
}
