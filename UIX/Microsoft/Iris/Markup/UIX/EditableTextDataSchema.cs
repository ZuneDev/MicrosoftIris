// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.EditableTextDataSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class EditableTextDataSchema
    {
        public static UIXTypeSchema Type;

        private static object GetValue(object instanceObj) => ((EditableTextData)instanceObj).Value;

        private static void SetValue(ref object instanceObj, object valueObj) => ((EditableTextData)instanceObj).Value = (string)valueObj;

        private static object GetMaxLength(object instanceObj) => ((EditableTextData)instanceObj).MaxLength;

        private static void SetMaxLength(ref object instanceObj, object valueObj)
        {
            EditableTextData editableTextData = (EditableTextData)instanceObj;
            int num = (int)valueObj;
            Result result = Int32Schema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                editableTextData.MaxLength = num;
        }

        private static object GetReadOnly(object instanceObj) => BooleanBoxes.Box(((EditableTextData)instanceObj).ReadOnly);

        private static void SetReadOnly(ref object instanceObj, object valueObj) => ((EditableTextData)instanceObj).ReadOnly = (bool)valueObj;

        private static object Construct() => new EditableTextData();

        private static object CallSubmit(object instanceObj, object[] parameters)
        {
            ((EditableTextData)instanceObj).Submit();
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(68, "EditableTextData", null, 153, typeof(EditableTextData), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(68, "Value", 208, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetValue), new SetValueHandler(SetValue), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(68, "MaxLength", 115, -1, ExpressionRestriction.None, false, Int32Schema.ValidateNotNegative, true, new GetValueHandler(GetMaxLength), new SetValueHandler(SetMaxLength), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(68, "ReadOnly", 15, -1, ExpressionRestriction.None, false, null, true, new GetValueHandler(GetReadOnly), new SetValueHandler(SetReadOnly), false);
            UIXEventSchema uixEventSchema = new UIXEventSchema(68, "Submitted");
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(68, "Submit", null, 240, new InvokeHandler(CallSubmit), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[3]
            {
         uixPropertySchema2,
         uixPropertySchema3,
         uixPropertySchema1
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, new EventSchema[1] { uixEventSchema }, null, null, null, null, null, null, null);
        }
    }
}
