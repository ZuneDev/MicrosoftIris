// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.SoundSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Audio;
using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.RenderAPI.Audio;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class SoundSchema
    {
        public static UIXTypeSchema Type;

        private static object GetSource(object instanceObj) => ((Sound)instanceObj).Source;

        private static void SetSource(ref object instanceObj, object valueObj) => ((Sound)instanceObj).Source = (string)valueObj;

        private static object GetSystemSoundEvent(object instanceObj) => ((Sound)instanceObj).SystemSoundEvent;

        private static void SetSystemSoundEvent(ref object instanceObj, object valueObj) => ((Sound)instanceObj).SystemSoundEvent = (SystemSoundEvent)valueObj;

        private static object Construct() => new Sound();

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
                return Result.Fail("Problem converting '{0}' ({1})", "Sound", result.Error);
            SetSource(ref instance, valueObj);
            return result;
        }

        private static object CallPlay(object instanceObj, object[] parameters)
        {
            ((Sound)instanceObj).Play();
            return null;
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
                if (splitString.Length == 1)
                {
                    result = ConvertFromStringSource(splitString, out instance);
                    if (!result.Failed)
                        return result;
                }
                else
                    result = Result.Fail("Unable to convert \"{0}\" to type '{1}'", from.ToString(), "Sound");
            }
            return result;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(201, "Sound", null, 153, typeof(Sound), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(201, "Source", 208, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetSource), new SetValueHandler(SetSource), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(201, "SystemSoundEvent", 211, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetSystemSoundEvent), new SetValueHandler(SetSystemSoundEvent), false);
            UIXConstructorSchema constructorSchema = new UIXConstructorSchema(201, new short[1]
            {
         208
            }, new ConstructHandler(ConstructSource));
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(201, "Play", null, 240, new InvokeHandler(CallPlay), false);
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[1]
            {
         constructorSchema
            }, new PropertySchema[2]
            {
         uixPropertySchema1,
         uixPropertySchema2
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), null, null, null, null);
        }
    }
}
