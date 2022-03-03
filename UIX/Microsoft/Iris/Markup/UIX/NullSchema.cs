// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.NullSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup.UIX
{
    internal static class NullSchema
    {
        public static UIXTypeSchema Type;

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
            object obj1 = leftObj;
            object obj2 = rightObj;
            switch (op)
            {
                case OperationType.RelationalEquals:
                    return BooleanBoxes.Box(obj1 == obj2);
                case OperationType.RelationalNotEquals:
                    return BooleanBoxes.Box(obj1 != obj2);
                default:
                    return null;
            }
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(152, "Null", null, -1, typeof(object), UIXTypeFlags.None);

        public static void Pass2Initialize() => Type.Initialize(null, null, null, null, null, null, null, null, null, null, new PerformOperationHandler(ExecuteOperation), new SupportsOperationHandler(IsOperationSupported));
    }
}
