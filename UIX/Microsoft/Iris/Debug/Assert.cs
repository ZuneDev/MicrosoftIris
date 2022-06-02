// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Debug.Assert
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;
using System.Diagnostics;

namespace Microsoft.Iris.Debug
{
    // FIXME: Use WCT's Guard APIs
    internal static class Assert
    {
        [Conditional("DEBUG")]
        public static void IsNotNull<T>(T value, string valueName) where T : class
        {
            // ISSUE: variable of a boxed type
            var local = (object)value;
        }

        [Conditional("DEBUG")]
        public static void IsNotNullMessage<T>(T value, string message) where T : class
        {
            // ISSUE: variable of a boxed type
            var local = (object)value;
        }

        [Conditional("DEBUG")]
        public static void IsNotNullMessage<T>(T value, string message, object param) where T : class
        {
            // ISSUE: variable of a boxed type
            var local = (object)value;
        }

        [Conditional("DEBUG")]
        public static void IsNull<T>(T value, string valueName) where T : class
        {
            // ISSUE: variable of a boxed type
            var local = (object)value;
        }

        [Conditional("DEBUG")]
        public static void IsNullMessage<T>(T value, string message) where T : class
        {
            // ISSUE: variable of a boxed type
            var local = (object)value;
        }

        [Conditional("DEBUG")]
        public static void IsType(object value, Type type)
        {
            int num = value != null || type.IsValueType ? (value == null ? (false ? 1 : 0) : (type.IsAssignableFrom(value.GetType()) ? 1 : 0)) : (true ? 1 : 0);
        }

        [Conditional("DEBUG")]
        public static void IsInRange(int value, int min, int max, string valueName)
        {
            if (value < min)
                ;
        }

        [Conditional("DEBUG")]
        public static void IsInRange(float value, float min, float max, string valueName)
        {
            if (value < (double)min)
                ;
        }

        [Conditional("DEBUG")]
        public static void IsNotNegative(int value, string valueName)
        {
        }

        [Conditional("DEBUG")]
        public static void IsNotNegative(float value, string valueName)
        {
        }

        [Conditional("DEBUG")]
        public static void IsNotNegativeMessage(int value, string message)
        {
        }

        [Conditional("DEBUG")]
        public static void IsTrue(bool condition)
        {
            int num = condition ? 1 : 0;
        }

        [Conditional("DEBUG")]
        public static void IsTrue(bool condition, string message)
        {
            int num = condition ? 1 : 0;
        }

        [Conditional("DEBUG")]
        public static void IsTrue(bool condition, string message, object param)
        {
            int num = condition ? 1 : 0;
        }

        [Conditional("DEBUG")]
        public static void IsTrue(bool condition, string message, object param1, object param2)
        {
            int num = condition ? 1 : 0;
        }

        [Conditional("DEBUG")]
        public static void IsTrue(
          bool condition,
          string message,
          object param1,
          object param2,
          object param3)
        {
            int num = condition ? 1 : 0;
        }

        [Conditional("DEBUG")]
        public static void IsTrue(
          bool condition,
          string message,
          object param1,
          object param2,
          object param3,
          object param4)
        {
            int num = condition ? 1 : 0;
        }

        [Conditional("DEBUG")]
        public static void Fail(string message)
        {
        }

        [Conditional("DEBUG")]
        public static void Fail(string format, object param)
        {
        }

        [Conditional("DEBUG")]
        public static void Fail(string format, object param1, object param2)
        {
        }

        [Conditional("DEBUG")]
        public static void Fail(string format, object param1, object param2, object param3)
        {
        }

        [Conditional("DEBUG")]
        private static void ReportFailure(string message)
        {
            StackTrace trace = new StackTrace(true);
            if (!DebugHelpers.DisplayStackTrace("Iris Framework Assert", message, trace, 1))
                return;
            DebugHelpers.Break();
        }

        [Conditional("DEBUG")]
        private static void ReportFailure(string format, object param)
        {
        }

        [Conditional("DEBUG")]
        private static void ReportFailure(string format, object param1, object param2)
        {
        }

        [Conditional("DEBUG")]
        private static void ReportFailure(string format, object param1, object param2, object param3)
        {
        }

        [Conditional("DEBUG")]
        private static void ReportFailure(
          string format,
          object param1,
          object param2,
          object param3,
          object param4)
        {
        }

        [Conditional("DEBUG")]
        public static void IsApplicationThread()
        {
        }

        [Conditional("DEBUG")]
        public static void IsApplicationShuttingDown(string message)
        {
        }

        [Conditional("DEBUG")]
        public static void IsApplicationThreadOrShuttingDown()
        {
        }

        [Conditional("DEBUG")]
        public static void CompatibleArray(Array array, int indexStart, Type objectType, int numItems) => array.GetType().GetElementType();
    }
}
