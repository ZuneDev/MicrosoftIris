// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.RotateLayoutSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class RotateLayoutSchema
    {
        public static RangeValidator ValidateRightAngle = new RangeValidator(RangeValidateRightAngle);
        public static UIXTypeSchema Type;

        private static object GetAngleDegrees(object instanceObj) => ((RotateLayout)instanceObj).AngleDegrees;

        private static void SetAngleDegrees(ref object instanceObj, object valueObj)
        {
            RotateLayout rotateLayout = (RotateLayout)instanceObj;
            int num = (int)valueObj;
            Result result = ValidateRightAngle(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                rotateLayout.AngleDegrees = num;
        }

        private static object Construct() => new RotateLayout();

        private static Result RangeValidateRightAngle(object value)
        {
            int num = (int)value;
            switch (num)
            {
                case 0:
                case 90:
                case 180:
                case 270:
                    return Result.Success;
                default:
                    return Result.Fail("Expecting a value of 0, 90, 180, or 270, but got {0}", num.ToString());
            }
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(175, "RotateLayout", null, 132, typeof(RotateLayout), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema = new UIXPropertySchema(175, "AngleDegrees", 115, -1, ExpressionRestriction.None, false, ValidateRightAngle, false, new GetValueHandler(GetAngleDegrees), new SetValueHandler(SetAngleDegrees), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[1]
            {
         uixPropertySchema
            }, null, null, null, null, null, null, null, null, null);
        }
    }
}
