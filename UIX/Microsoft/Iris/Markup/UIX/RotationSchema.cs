// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.RotationSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using System;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class RotationSchema
    {
        public static UIXTypeSchema Type;

        private static object GetAxis(object instanceObj) => ((Rotation)instanceObj).Axis;

        private static void SetAxis(ref object instanceObj, object valueObj)
        {
            Rotation rotation = (Rotation)instanceObj;
            Vector3 vector3 = (Vector3)valueObj;
            rotation.Axis = vector3;
            instanceObj = rotation;
        }

        private static object GetAngleRadians(object instanceObj) => ((Rotation)instanceObj).AngleRadians;

        private static void SetAngleRadians(ref object instanceObj, object valueObj)
        {
            Rotation rotation = (Rotation)instanceObj;
            float num = (float)valueObj;
            rotation.AngleRadians = num;
            instanceObj = rotation;
        }

        private static object GetAngleDegrees(object instanceObj) => ((Rotation)instanceObj).AngleDegrees;

        private static void SetAngleDegrees(ref object instanceObj, object valueObj)
        {
            Rotation rotation = (Rotation)instanceObj;
            int num = (int)valueObj;
            rotation.AngleDegrees = num;
            instanceObj = rotation;
        }

        private static object Construct() => Rotation.Default;

        private static object ConstructAngleDegrees(object[] parameters)
        {
            object instanceObj = Construct();
            SetAngleDegrees(ref instanceObj, parameters[0]);
            return instanceObj;
        }

        private static object ConstructAngleRadians(object[] parameters)
        {
            object instanceObj = Construct();
            SetAngleRadians(ref instanceObj, parameters[0]);
            return instanceObj;
        }

        private static object ConstructAngleDegreesAxis(object[] parameters)
        {
            object instanceObj = Construct();
            SetAngleDegrees(ref instanceObj, parameters[0]);
            SetAxis(ref instanceObj, parameters[1]);
            return instanceObj;
        }

        private static object ConstructAngleRadiansAxis(object[] parameters)
        {
            object instanceObj = Construct();
            SetAngleRadians(ref instanceObj, parameters[0]);
            SetAxis(ref instanceObj, parameters[1]);
            return instanceObj;
        }

        private static object ConstructAxis(object[] parameters)
        {
            object instanceObj = Construct();
            SetAxis(ref instanceObj, parameters[0]);
            return instanceObj;
        }

        private static void EncodeBinary(ByteCodeWriter writer, object instanceObj)
        {
            Rotation rotation = (Rotation)instanceObj;
            Vector3Schema.Type.EncodeBinary(writer, rotation.Axis);
            writer.WriteSingle(rotation.AngleRadians);
        }

        private static object DecodeBinary(ByteCodeReader reader)
        {
            Vector3 axis = (Vector3)Vector3Schema.Type.DecodeBinary(reader);
            return new Rotation(reader.ReadSingle(), axis);
        }

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string str1 = (string)valueObj;
            instanceObj = null;
            Rotation rotation = Rotation.Default;
            string[] strArray = str1.Split(';');
            if (strArray.Length != 2)
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", str1, "Rotation");
            string str2 = strArray[1];
            object instance1;
            Result result1 = Vector3Schema.Type.TypeConverter(str2, StringSchema.Type, out instance1);
            if (result1.Failed)
                return Result.Fail("Problem converting '{0}' ({1})", "Rotation", result1.Error);
            rotation.Axis = (Vector3)instance1;
            string str3 = strArray[0];
            if (str3.EndsWith("rad", StringComparison.Ordinal))
            {
                string str4 = str3.Substring(0, str3.Length - 3);
                object instance2;
                Result result2 = SingleSchema.Type.TypeConverter(str4, StringSchema.Type, out instance2);
                if (result2.Failed)
                    return Result.Fail("Problem converting '{0}' ({1})", "Rotation", result2.Error);
                rotation.AngleRadians = (float)instance2;
            }
            else if (str3.EndsWith("deg", StringComparison.Ordinal))
            {
                string str4 = str3.Substring(0, str3.Length - 3);
                object instance2;
                Result result2 = Int32Schema.Type.TypeConverter(str4, StringSchema.Type, out instance2);
                if (result2.Failed)
                    return Result.Fail("Problem converting '{0}' ({1})", "Rotation", result2.Error);
                rotation.AngleDegrees = (int)instance2;
            }
            instanceObj = rotation;
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

        private static object CallTryParseStringRotation(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            Rotation parameter2 = (Rotation)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(176, "Rotation", null, 153, typeof(Rotation), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(176, "Axis", 234, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAxis), new SetValueHandler(SetAxis), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(176, "AngleRadians", 194, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAngleRadians), new SetValueHandler(SetAngleRadians), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(176, "AngleDegrees", 115, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetAngleDegrees), new SetValueHandler(SetAngleDegrees), false);
            UIXConstructorSchema constructorSchema1 = new UIXConstructorSchema(176, new short[1]
            {
         115
            }, new ConstructHandler(ConstructAngleDegrees));
            UIXConstructorSchema constructorSchema2 = new UIXConstructorSchema(176, new short[1]
            {
         194
            }, new ConstructHandler(ConstructAngleRadians));
            UIXConstructorSchema constructorSchema3 = new UIXConstructorSchema(176, new short[2]
            {
         115,
         234
            }, new ConstructHandler(ConstructAngleDegreesAxis));
            UIXConstructorSchema constructorSchema4 = new UIXConstructorSchema(176, new short[2]
            {
         194,
         234
            }, new ConstructHandler(ConstructAngleRadiansAxis));
            UIXConstructorSchema constructorSchema5 = new UIXConstructorSchema(176, new short[1]
            {
         234
            }, new ConstructHandler(ConstructAxis));
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(176, "TryParse", new short[2]
            {
         208,
         176
            }, 176, new InvokeHandler(CallTryParseStringRotation), true);
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[5]
            {
         constructorSchema1,
         constructorSchema2,
         constructorSchema3,
         constructorSchema4,
         constructorSchema5
            }, new PropertySchema[3]
            {
         uixPropertySchema3,
         uixPropertySchema2,
         uixPropertySchema1
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), new EncodeBinaryHandler(EncodeBinary), new DecodeBinaryHandler(DecodeBinary), null, null);
        }
    }
}
