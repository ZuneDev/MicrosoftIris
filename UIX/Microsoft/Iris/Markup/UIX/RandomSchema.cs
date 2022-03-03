// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.RandomSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class RandomSchema
    {
        public static UIXTypeSchema Type;

        private static object Construct() => new Random();

        private static object ConstructInt32(object[] parameters) => new Random((int)parameters[0]);

        private static object CallNext(object instanceObj, object[] parameters) => ((Random)instanceObj).Next();

        private static object CallNextInt32(object instanceObj, object[] parameters)
        {
            Random random = (Random)instanceObj;
            int parameter = (int)parameters[0];
            if (parameter >= 0)
                return random.Next(parameter);
            ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", parameter, "maxValue");
            return null;
        }

        private static object CallNextInt32Int32(object instanceObj, object[] parameters)
        {
            Random random = (Random)instanceObj;
            int parameter1 = (int)parameters[0];
            int parameter2 = (int)parameters[1];
            if (parameter1 <= parameter2)
                return random.Next(parameter1, parameter2);
            ErrorManager.ReportError("Script runtime failure: Invalid '{0}' value is out of range for '{1}'", parameter1, "minValue");
            return null;
        }

        private static object CallNextDouble(object instanceObj, object[] parameters) => ((Random)instanceObj).NextDouble();

        private static object CallNextDoubleDoubleDouble(object instanceObj, object[] parameters)
        {
            Random random = (Random)instanceObj;
            double parameter1 = (double)parameters[0];
            double parameter2 = (double)parameters[1];
            return random.NextDouble() * (parameter2 - parameter1) + parameter1;
        }

        private static object CallNextSingle(object instanceObj, object[] parameters) => (float)((Random)instanceObj).NextDouble();

        private static object CallNextSingleSingleSingle(object instanceObj, object[] parameters)
        {
            Random random = (Random)instanceObj;
            float parameter1 = (float)parameters[0];
            float parameter2 = (float)parameters[1];
            return (float)(random.NextDouble() * (parameter2 - (double)parameter1)) + parameter1;
        }

        public static void Pass1Initialize() => Type = new UIXTypeSchema(167, "Random", null, 153, typeof(Random), UIXTypeFlags.None);

        public static void Pass2Initialize()
        {
            UIXConstructorSchema constructorSchema = new UIXConstructorSchema(167, new short[1]
            {
         115
            }, new ConstructHandler(ConstructInt32));
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(167, "Next", null, 115, new InvokeHandler(CallNext), false);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(167, "Next", new short[1]
            {
         115
            }, 115, new InvokeHandler(CallNextInt32), false);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(167, "Next", new short[2]
            {
         115,
         115
            }, 115, new InvokeHandler(CallNextInt32Int32), false);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(167, "NextDouble", null, 61, new InvokeHandler(CallNextDouble), false);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(167, "NextDouble", new short[2]
            {
         61,
         61
            }, 61, new InvokeHandler(CallNextDoubleDoubleDouble), false);
            UIXMethodSchema uixMethodSchema6 = new UIXMethodSchema(167, "NextSingle", null, 194, new InvokeHandler(CallNextSingle), false);
            UIXMethodSchema uixMethodSchema7 = new UIXMethodSchema(167, "NextSingle", new short[2]
            {
         194,
         194
            }, 194, new InvokeHandler(CallNextSingleSingleSingle), false);
            Type.Initialize(new DefaultConstructHandler(Construct), new ConstructorSchema[1]
            {
         constructorSchema
            }, null, new MethodSchema[7]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4,
         uixMethodSchema5,
         uixMethodSchema6,
         uixMethodSchema7
            }, null, null, null, null, null, null, null, null);
        }
    }
}
