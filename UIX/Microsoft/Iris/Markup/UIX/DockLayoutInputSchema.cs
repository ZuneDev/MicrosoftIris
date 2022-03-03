// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.DockLayoutInputSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class DockLayoutInputSchema
    {
        public static UIXTypeSchema Type;

        private static object Construct() => DockLayoutInput.Client;

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string str = (string)valueObj;
            instanceObj = null;
            DockLayoutInput instance = StringToInstance(str);
            if (instance == null)
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", str, "DockLayoutInput");
            instanceObj = instance;
            return Result.Success;
        }

        private static object FindCanonicalInstance(string name) => StringToInstance(name);

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

        private static object CallTryParseStringDockLayoutInput(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            DockLayoutInput parameter2 = (DockLayoutInput)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        private static DockLayoutInput StringToInstance(string value)
        {
            if (value == "Left")
                return DockLayoutInput.Left;
            if (value == "Top")
                return DockLayoutInput.Top;
            if (value == "Right")
                return DockLayoutInput.Right;
            if (value == "Bottom")
                return DockLayoutInput.Bottom;
            return value == "Client" ? DockLayoutInput.Client : null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(60, "DockLayoutInput", null, 133, typeof(DockLayoutInput), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(60, "TryParse", new short[2]
            {
         208,
         60
            }, 60, new InvokeHandler(CallTryParseStringDockLayoutInput), true);
            Type.Initialize(new DefaultConstructHandler(Construct), null, null, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, new FindCanonicalInstanceHandler(FindCanonicalInstance), new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), null, null, null, null);
        }
    }
}
