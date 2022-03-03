// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.PlacementModeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;
using System.Collections;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class PlacementModeSchema
    {
        public static UIXTypeSchema Type;

        private static object GetPopupPositions(object instanceObj) => new ArrayList(((PlacementMode)instanceObj).PopupPositions);

        private static void SetPopupPositions(ref object instanceObj, object valueObj)
        {
            PlacementMode placementMode = (PlacementMode)instanceObj;
            IList list = (IList)valueObj;
            PopupPosition[] popupPositionArray = new PopupPosition[list.Count];
            for (int index = 0; index < list.Count; ++index)
                popupPositionArray[index] = (PopupPosition)list[index];
            placementMode.PopupPositions = popupPositionArray;
        }

        private static object GetMouseTarget(object instanceObj) => ((PlacementMode)instanceObj).MouseTarget;

        private static void SetMouseTarget(ref object instanceObj, object valueObj) => ((PlacementMode)instanceObj).MouseTarget = (MouseTarget)valueObj;

        private static object Construct() => new PlacementMode();

        private static Result ConvertFromString(object valueObj, out object instanceObj)
        {
            string str = (string)valueObj;
            instanceObj = null;
            PlacementMode instance = StringToInstance(str);
            if (instance == null)
                return Result.Fail("Unable to convert \"{0}\" to type '{1}'", str, "PlacementMode");
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

        private static object CallTryParseStringPlacementMode(object instanceObj, object[] parameters)
        {
            string parameter1 = (string)parameters[0];
            PlacementMode parameter2 = (PlacementMode)parameters[1];
            object instanceObj1;
            return ConvertFromString(parameter1, out instanceObj1).Failed ? parameter2 : instanceObj1;
        }

        private static PlacementMode StringToInstance(string value)
        {
            if (value == "Origin")
                return PlacementMode.Origin;
            if (value == "Left")
                return PlacementMode.Left;
            if (value == "Right")
                return PlacementMode.Right;
            if (value == "Top")
                return PlacementMode.Top;
            if (value == "Bottom")
                return PlacementMode.Bottom;
            if (value == "Center")
                return PlacementMode.Center;
            if (value == "MouseOrigin")
                return PlacementMode.MouseOrigin;
            if (value == "MouseBottom")
                return PlacementMode.MouseBottom;
            if (value == "FollowMouseOrigin")
                return PlacementMode.FollowMouseOrigin;
            return value == "FollowMouseBottom" ? PlacementMode.FollowMouseBottom : null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(157, "PlacementMode", null, 153, typeof(PlacementMode), UIXTypeFlags.Immutable);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(157, "PopupPositions", 138, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetPopupPositions), new SetValueHandler(SetPopupPositions), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(157, "MouseTarget", 149, -1, ExpressionRestriction.None, false, null, false, new GetValueHandler(GetMouseTarget), new SetValueHandler(SetMouseTarget), false);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(157, "TryParse", new short[2]
            {
         208,
         157
            }, 157, new InvokeHandler(CallTryParseStringPlacementMode), true);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[2]
            {
         uixPropertySchema2,
         uixPropertySchema1
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, new FindCanonicalInstanceHandler(FindCanonicalInstance), new TypeConverterHandler(TryConvertFrom), new SupportsTypeConversionHandler(IsConversionSupported), null, null, null, null);
        }
    }
}
