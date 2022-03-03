// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.ClassStateSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class ClassStateSchema
    {
        public static UIXTypeSchema Type;

        private static object CallDisposeOwnedObjectObject(object instanceObj, object[] parameters)
        {
            Class @class = (Class)instanceObj;
            object parameter = parameters[0];
            if (parameter == null)
                return null;
            if (!(parameter is IDisposableObject disposable))
            {
                ErrorManager.ReportError("Attempt to dispose an object '{0}' that isn't disposable", TypeSchema.NameFromInstance(parameter));
                return null;
            }
            if (!@class.UnregisterDisposable(ref disposable))
            {
                ErrorManager.ReportError("Attempt to dispose an object '{0}' that '{1}' doesn't own", TypeSchema.NameFromInstance(disposable), @class.TypeSchema.Name);
                return null;
            }
            disposable.Dispose(@class);
            return null;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(30, "ClassState", null, -1, typeof(Class), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXMethodSchema uixMethodSchema = new UIXMethodSchema(30, "DisposeOwnedObject", new short[1]
            {
         153
            }, 240, new InvokeHandler(CallDisposeOwnedObjectObject), false);
            Type.Initialize(null, null, null, new MethodSchema[1]
            {
         uixMethodSchema
            }, null, null, null, null, null, null, null, null);
        }
    }
}
