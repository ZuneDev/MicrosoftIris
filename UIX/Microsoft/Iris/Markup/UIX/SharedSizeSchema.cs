// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.SharedSizeSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class SharedSizeSchema
    {
        public static UIXTypeSchema Type;

        private static object GetMaximumSize(object instanceObj) => ((SharedSize)instanceObj).MaximumSize;

        private static void SetMaximumSize(ref object instanceObj, object valueObj)
        {
            SharedSize sharedSize = (SharedSize)instanceObj;
            Size size = (Size)valueObj;
            Result result = SizeSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                sharedSize.MaximumSize = size;
        }

        private static object GetMinimumSize(object instanceObj) => ((SharedSize)instanceObj).MinimumSize;

        private static void SetMinimumSize(ref object instanceObj, object valueObj)
        {
            SharedSize sharedSize = (SharedSize)instanceObj;
            Size size = (Size)valueObj;
            Result result = SizeSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                sharedSize.MinimumSize = size;
        }

        private static object GetSize(object instanceObj) => ((SharedSize)instanceObj).Size;

        private static void SetSize(ref object instanceObj, object valueObj)
        {
            SharedSize sharedSize = (SharedSize)instanceObj;
            Size size = (Size)valueObj;
            Result result = SizeSchema.ValidateNotNegative(valueObj);
            if (result.Failed)
                ErrorManager.ReportError(result.Error);
            else
                sharedSize.Size = size;
        }

        private static object Construct() => new SharedSize();

        private static object CallAutoSize(object instanceObj, object[] parameters)
        {
            ((SharedSize)instanceObj).AutoSize();
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(190, "SharedSize", null, 153, typeof(SharedSize), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXPropertySchema uixPropertySchema1 = new UIXPropertySchema(190, "MaximumSize", 195, -1, ExpressionRestriction.None, false, SizeSchema.ValidateNotNegative, true, new GetValueHandler(GetMaximumSize), new SetValueHandler(SetMaximumSize), false);
            UIXPropertySchema uixPropertySchema2 = new UIXPropertySchema(190, "MinimumSize", 195, -1, ExpressionRestriction.None, false, SizeSchema.ValidateNotNegative, true, new GetValueHandler(GetMinimumSize), new SetValueHandler(SetMinimumSize), false);
            UIXPropertySchema uixPropertySchema3 = new UIXPropertySchema(190, "Size", 195, -1, ExpressionRestriction.None, false, SizeSchema.ValidateNotNegative, true, new GetValueHandler(GetSize), new SetValueHandler(SetSize), false);
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(190, "AutoSize", null, 240, new InvokeHandler(CallAutoSize), false);
            Type.Initialize(new DefaultConstructHandler(Construct), null, new PropertySchema[3]
            {
         uixPropertySchema1,
         uixPropertySchema2,
         uixPropertySchema3
            }, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, null, null, null, null, null, null);
        }
    }
}
