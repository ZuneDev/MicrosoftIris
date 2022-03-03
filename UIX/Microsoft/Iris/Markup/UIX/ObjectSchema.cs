// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ObjectSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ObjectSchema
    {
        public static UIXTypeSchema Type;

        private static object CallToString(object instanceObj, object[] parameters) => instanceObj.ToString();

        private static bool IsOperationSupported(OperationType op)
        {
            switch (op)
            {
                case OperationType.RelationalEquals:
                case OperationType.RelationalNotEquals:
                    return true;
                default:
                    return false;
            }
        }

        private static object ExecuteOperation(object leftObj, object rightObj, OperationType op)
        {
            object objA = leftObj;
            object objB = rightObj;
            switch (op)
            {
                case OperationType.RelationalEquals:
                    return BooleanBoxes.Box(Equals(objA, objB));
                case OperationType.RelationalNotEquals:
                    return BooleanBoxes.Box(!Equals(objA, objB));
                default:
                    return null;
            }
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(153, "Object", "object", -1, typeof(object), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(153, "ToString", null, 208, new InvokeHandler(CallToString), false);
            Type.Initialize(null, null, null, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, null, null, null, null, new PerformOperationHandler(ExecuteOperation), new SupportsOperationHandler(IsOperationSupported));
        }
    }
}
