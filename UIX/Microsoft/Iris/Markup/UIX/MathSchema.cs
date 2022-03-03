// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.UIX.MathSchema
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Markup.UIX
{
    internal static class MathSchema
    {
        public static UIXTypeSchema Type;

        private static object CallMinInt32Int32(object instanceObj, object[] parameters) => (int)parameters[0] > (int)parameters[1] ? parameters[1] : parameters[0];

        private static object CallMinSingleSingle(object instanceObj, object[] parameters) => (float)parameters[0] > (double)(float)parameters[1] ? parameters[1] : parameters[0];

        private static object CallMinDoubleDouble(object instanceObj, object[] parameters) => (double)parameters[0] > (double)parameters[1] ? parameters[1] : parameters[0];

        private static object CallMaxInt32Int32(object instanceObj, object[] parameters) => (int)parameters[0] < (int)parameters[1] ? parameters[1] : parameters[0];

        private static object CallMaxSingleSingle(object instanceObj, object[] parameters) => (float)parameters[0] < (double)(float)parameters[1] ? parameters[1] : parameters[0];

        private static object CallMaxDoubleDouble(object instanceObj, object[] parameters) => (double)parameters[0] < (double)parameters[1] ? parameters[1] : parameters[0];

        private static object CallAbsInt32(object instanceObj, object[] parameters)
        {
            int parameter = (int)parameters[0];
            int num = Math.Abs(parameter);
            return num != parameter ? num : parameters[0];
        }

        private static object CallAbsSingle(object instanceObj, object[] parameters)
        {
            float parameter = (float)parameters[0];
            float num = Math.Abs(parameter);
            return num != (double)parameter ? num : parameters[0];
        }

        private static object CallAbsDouble(object instanceObj, object[] parameters)
        {
            double parameter = (double)parameters[0];
            double num = Math.Abs(parameter);
            return num != parameter ? num : parameters[0];
        }

        private static object CallRoundSingle(object instanceObj, object[] parameters)
        {
            float parameter = (float)parameters[0];
            float num = (float)Math.Round(parameter);
            return num != (double)parameter ? num : parameters[0];
        }

        private static object CallRoundDouble(object instanceObj, object[] parameters)
        {
            double parameter = (double)parameters[0];
            double num = Math.Round(parameter);
            return num != parameter ? num : parameters[0];
        }

        private static object CallFloorSingle(object instanceObj, object[] parameters)
        {
            float parameter = (float)parameters[0];
            float num = (float)Math.Floor(parameter);
            return num != (double)parameter ? num : parameters[0];
        }

        private static object CallFloorDouble(object instanceObj, object[] parameters)
        {
            double parameter = (double)parameters[0];
            double num = Math.Floor(parameter);
            return num != parameter ? num : parameters[0];
        }

        private static object CallCeilingSingle(object instanceObj, object[] parameters)
        {
            float parameter = (float)parameters[0];
            float num = (float)Math.Ceiling(parameter);
            return num != (double)parameter ? num : parameters[0];
        }

        private static object CallCeilingDouble(object instanceObj, object[] parameters)
        {
            double parameter = (double)parameters[0];
            double num = Math.Ceiling(parameter);
            return num != parameter ? num : parameters[0];
        }

        private static object CallAcosDouble(object instanceObj, object[] parameters) => Math.Acos((double)parameters[0]);

        private static object CallAsinDouble(object instanceObj, object[] parameters) => Math.Asin((double)parameters[0]);

        private static object CallAtanDouble(object instanceObj, object[] parameters) => Math.Atan((double)parameters[0]);

        private static object CallAtan2DoubleDouble(object instanceObj, object[] parameters) => Math.Atan2((double)parameters[0], (double)parameters[1]);

        private static object CallCosDouble(object instanceObj, object[] parameters) => Math.Cos((double)parameters[0]);

        private static object CallCoshDouble(object instanceObj, object[] parameters) => Math.Cosh((double)parameters[0]);

        private static object CallSinDouble(object instanceObj, object[] parameters) => Math.Sin((double)parameters[0]);

        private static object CallSinhDouble(object instanceObj, object[] parameters) => Math.Sinh((double)parameters[0]);

        private static object CallTanDouble(object instanceObj, object[] parameters) => Math.Tan((double)parameters[0]);

        private static object CallTanhDouble(object instanceObj, object[] parameters) => Math.Tanh((double)parameters[0]);

        private static object CallSqrtDouble(object instanceObj, object[] parameters) => Math.Sqrt((double)parameters[0]);

        private static object CallPowDoubleDouble(object instanceObj, object[] parameters) => Math.Pow((double)parameters[0], (double)parameters[1]);

        private static object CallLogDouble(object instanceObj, object[] parameters) => Math.Log((double)parameters[0]);

        private static object CallLogDoubleDouble(object instanceObj, object[] parameters) => Math.Log((double)parameters[0], (double)parameters[1]);

        private static object CallLog10Double(object instanceObj, object[] parameters) => Math.Log10((double)parameters[0]);

        public static void Pass1Initialize() => Type = new UIXTypeSchema(145, "Math", null, 153, typeof(object), UIXTypeFlags.Static);

        public static void Pass2Initialize()
        {
            UIXMethodSchema uixMethodSchema1 = new UIXMethodSchema(145, "Min", new short[2]
            {
         115,
         115
            }, 115, new InvokeHandler(CallMinInt32Int32), true);
            UIXMethodSchema uixMethodSchema2 = new UIXMethodSchema(145, "Min", new short[2]
            {
         194,
         194
            }, 194, new InvokeHandler(CallMinSingleSingle), true);
            UIXMethodSchema uixMethodSchema3 = new UIXMethodSchema(145, "Min", new short[2]
            {
         61,
         61
            }, 61, new InvokeHandler(CallMinDoubleDouble), true);
            UIXMethodSchema uixMethodSchema4 = new UIXMethodSchema(145, "Max", new short[2]
            {
         115,
         115
            }, 115, new InvokeHandler(CallMaxInt32Int32), true);
            UIXMethodSchema uixMethodSchema5 = new UIXMethodSchema(145, "Max", new short[2]
            {
         194,
         194
            }, 194, new InvokeHandler(CallMaxSingleSingle), true);
            UIXMethodSchema uixMethodSchema6 = new UIXMethodSchema(145, "Max", new short[2]
            {
         61,
         61
            }, 61, new InvokeHandler(CallMaxDoubleDouble), true);
            UIXMethodSchema uixMethodSchema7 = new UIXMethodSchema(145, "Abs", new short[1]
            {
         115
            }, 115, new InvokeHandler(CallAbsInt32), true);
            UIXMethodSchema uixMethodSchema8 = new UIXMethodSchema(145, "Abs", new short[1]
            {
         194
            }, 194, new InvokeHandler(CallAbsSingle), true);
            UIXMethodSchema uixMethodSchema9 = new UIXMethodSchema(145, "Abs", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallAbsDouble), true);
            UIXMethodSchema uixMethodSchema10 = new UIXMethodSchema(145, "Round", new short[1]
            {
         194
            }, 194, new InvokeHandler(CallRoundSingle), true);
            UIXMethodSchema uixMethodSchema11 = new UIXMethodSchema(145, "Round", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallRoundDouble), true);
            UIXMethodSchema uixMethodSchema12 = new UIXMethodSchema(145, "Floor", new short[1]
            {
         194
            }, 194, new InvokeHandler(CallFloorSingle), true);
            UIXMethodSchema uixMethodSchema13 = new UIXMethodSchema(145, "Floor", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallFloorDouble), true);
            UIXMethodSchema uixMethodSchema14 = new UIXMethodSchema(145, "Ceiling", new short[1]
            {
         194
            }, 194, new InvokeHandler(CallCeilingSingle), true);
            UIXMethodSchema uixMethodSchema15 = new UIXMethodSchema(145, "Ceiling", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallCeilingDouble), true);
            UIXMethodSchema uixMethodSchema16 = new UIXMethodSchema(145, "Acos", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallAcosDouble), true);
            UIXMethodSchema uixMethodSchema17 = new UIXMethodSchema(145, "Asin", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallAsinDouble), true);
            UIXMethodSchema uixMethodSchema18 = new UIXMethodSchema(145, "Atan", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallAtanDouble), true);
            UIXMethodSchema uixMethodSchema19 = new UIXMethodSchema(145, "Atan2", new short[2]
            {
         61,
         61
            }, 61, new InvokeHandler(CallAtan2DoubleDouble), true);
            UIXMethodSchema uixMethodSchema20 = new UIXMethodSchema(145, "Cos", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallCosDouble), true);
            UIXMethodSchema uixMethodSchema21 = new UIXMethodSchema(145, "Cosh", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallCoshDouble), true);
            UIXMethodSchema uixMethodSchema22 = new UIXMethodSchema(145, "Sin", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallSinDouble), true);
            UIXMethodSchema uixMethodSchema23 = new UIXMethodSchema(145, "Sinh", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallSinhDouble), true);
            UIXMethodSchema uixMethodSchema24 = new UIXMethodSchema(145, "Tan", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallTanDouble), true);
            UIXMethodSchema uixMethodSchema25 = new UIXMethodSchema(145, "Tanh", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallTanhDouble), true);
            UIXMethodSchema uixMethodSchema26 = new UIXMethodSchema(145, "Sqrt", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallSqrtDouble), true);
            UIXMethodSchema uixMethodSchema27 = new UIXMethodSchema(145, "Pow", new short[2]
            {
         61,
         61
            }, 61, new InvokeHandler(CallPowDoubleDouble), true);
            UIXMethodSchema uixMethodSchema28 = new UIXMethodSchema(145, "Log", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallLogDouble), true);
            UIXMethodSchema uixMethodSchema29 = new UIXMethodSchema(145, "Log", new short[2]
            {
         61,
         61
            }, 61, new InvokeHandler(CallLogDoubleDouble), true);
            UIXMethodSchema uixMethodSchema30 = new UIXMethodSchema(145, "Log10", new short[1]
            {
         61
            }, 61, new InvokeHandler(CallLog10Double), true);
            Type.Initialize(null, null, null, new MethodSchema[30]
            {
         uixMethodSchema1,
         uixMethodSchema2,
         uixMethodSchema3,
         uixMethodSchema4,
         uixMethodSchema5,
         uixMethodSchema6,
         uixMethodSchema7,
         uixMethodSchema8,
         uixMethodSchema9,
         uixMethodSchema10,
         uixMethodSchema11,
         uixMethodSchema12,
         uixMethodSchema13,
         uixMethodSchema14,
         uixMethodSchema15,
         uixMethodSchema16,
         uixMethodSchema17,
         uixMethodSchema18,
         uixMethodSchema19,
         uixMethodSchema20,
         uixMethodSchema21,
         uixMethodSchema22,
         uixMethodSchema23,
         uixMethodSchema24,
         uixMethodSchema25,
         uixMethodSchema26,
         uixMethodSchema27,
         uixMethodSchema28,
         uixMethodSchema29,
         uixMethodSchema30
            }, null, null, null, null, null, null, null, null);
        }
    }
}
