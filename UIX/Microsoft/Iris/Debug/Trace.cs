// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Debug.Trace
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.OS;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Security;
using System.Text;

namespace Microsoft.Iris.Debug
{
    [SuppressUnmanagedCodeSecurity]
    internal static class Trace
    {
        public const byte c_defaultTraceLevelFlag = 1;
        private const string c_ehDebugDllFileName = "UIXsup.dll";
        private static StringBuilder s_safeFormatBuilder;
        private static object s_safeFormatLockObj;
        private static bool s_isInitialized;
        private static object[][] s_arrayCache = new object[7][];

        static Trace()
        {
            s_safeFormatLockObj = new object();
            s_safeFormatBuilder = new StringBuilder();
        }

        public static void Initialize()
        {
            if (s_isInitialized)
                return;
            s_isInitialized = true;
            NativeApi.SpInitializeTracing();
            TraceSettings.Current.Refresh();
            TraceSettings.Current.ListenForRegistryUpdates();
        }

        public static void Shutdown()
        {
            if (!s_isInitialized)
                return;
            TraceSettings.Current.StopListeningForRegistryUpdates();
            NativeApi.SpUninitializeTracing();
            s_isInitialized = false;
        }

        [Conditional("DEBUG")]
        public static void WriteLine(TraceCategory cat, object valueObject) => WriteLine(cat, 1, valueObject);

        [Conditional("DEBUG")]
        public static void WriteLine(TraceCategory cat, string format, object param) => WriteLine(cat, 1, format, param);

        [Conditional("DEBUG")]
        public static void WriteLine(TraceCategory cat, string format, object param1, object param2) => WriteLine(cat, 1, format, param1, param2);

        [Conditional("DEBUG")]
        public static void WriteLine(TraceCategory cat, string format, object param1, object param2, object param3)
        {
            WriteLine(cat, 1, format, param1, param2, param3);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(TraceCategory cat, string format, params object[] pars)
        {
            WriteLine(cat, 1, format, pars);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(TraceCategory cat, byte levelFlag, object valueObject)
        {
            if (IsCategoryEnabled(cat, levelFlag))
                WriteLine(BuildLine(valueObject.ToString()));
        }

        [Conditional("DEBUG")]
        public static void WriteLine(TraceCategory cat, byte levelFlag, string format, object param)
        {
            if (IsCategoryEnabled(cat, levelFlag))
            {
                string content = string.Format(format, param);
                WriteLine(BuildLine(content));
            }
        }

        [Conditional("DEBUG")]
        public static void WriteLine(TraceCategory cat, byte levelFlag, string format, object param1, object param2)
        {
            if (IsCategoryEnabled(cat, levelFlag))
            {
                string content = string.Format(format, param1, param2);
                WriteLine(BuildLine(content));
            }
        }

        [Conditional("DEBUG")]
        public static void WriteLine(TraceCategory cat, byte levelFlag, string format, object param1, object param2, object param3)
        {
            if (IsCategoryEnabled(cat, levelFlag))
            {
                string content = string.Format(format, param1, param2, param3);
                WriteLine(BuildLine(content));
            }
        }

        [Conditional("DEBUG")]
        public static void WriteLine(TraceCategory cat, byte levelFlag, string format, params object[] pars)
        {
            if (IsCategoryEnabled(cat, levelFlag))
            {
                string content = string.Format(format, pars);
                var line = BuildLine(content);

                WriteLine(line);
            }
        }

        public static int IndentLevel { get; set; }

        public static bool IsCategoryEnabled(TraceCategory cat) => IsCategoryEnabled(cat, 1);

        public static bool IsCategoryEnabled(TraceCategory cat, byte requestLevel)
        {
            return TraceSettings.Current.GetCategoryLevel(cat) >= requestLevel;
        }

        public static void EnableCategory(TraceCategory cat, bool enabled)
        {
            TraceSettings.Current.SetCategoryLevel(cat, (byte)(enabled ? 1 : 0));
        }

        public static void EnableAllCategories(bool enabled)
        {
            foreach (Enum e in Enum.GetValues(typeof(TraceCategory)))
                EnableCategory((TraceCategory)e, enabled);
        }

        [Conditional("DEBUG")]
        public static void Indent(TraceCategory cat)
        {
        }

        [Conditional("DEBUG")]
        public static void Indent(TraceCategory cat, byte levelFlag) => IsCategoryEnabled(cat, levelFlag);

        [Conditional("DEBUG")]
        public static void Unindent(TraceCategory cat)
        {
        }

        [Conditional("DEBUG")]
        public static void Unindent(TraceCategory cat, byte levelFlag) => IsCategoryEnabled(cat, levelFlag);

        [Conditional("DEBUG")]
        public static void OpenBrace(TraceCategory cat) => IsCategoryEnabled(cat, 1);

        [Conditional("DEBUG")]
        public static void OpenBrace(TraceCategory cat, string message) => IsCategoryEnabled(cat, 1);

        [Conditional("DEBUG")]
        public static void OpenBrace(TraceCategory cat, string format, object param) => IsCategoryEnabled(cat, 1);

        [Conditional("DEBUG")]
        public static void OpenBrace(TraceCategory cat, string format, object param, object param2) => IsCategoryEnabled(cat, 1);

        [Conditional("DEBUG")]
        public static void OpenBrace(TraceCategory cat, string format, object param, object param2, object param3)
        {
            IsCategoryEnabled(cat, 1);
        }

        [Conditional("DEBUG")]
        public static void OpenBrace(TraceCategory cat, string format, object param, object param2, object param3, object param4)
        {
            IsCategoryEnabled(cat, 1);
        }

        [Conditional("DEBUG")]
        public static void OpenBrace(TraceCategory cat, byte levelFlag, string message) => IsCategoryEnabled(cat, levelFlag);

        [Conditional("DEBUG")]
        public static void OpenBrace(TraceCategory cat, byte levelFlag, string format, object param) => IsCategoryEnabled(cat, levelFlag);

        [Conditional("DEBUG")]
        public static void OpenBrace(TraceCategory cat, byte levelFlag, string format, object param, object param2)
        {
            IsCategoryEnabled(cat, levelFlag);
        }

        [Conditional("DEBUG")]
        public static void OpenBrace(
          TraceCategory cat,
          byte levelFlag,
          string format,
          object param,
          object param2,
          object param3)
        {
            IsCategoryEnabled(cat, levelFlag);
        }

        [Conditional("DEBUG")]
        public static void OpenBrace(
          TraceCategory cat,
          byte levelFlag,
          string format,
          object param,
          object param2,
          object param3,
          object param4)
        {
            IsCategoryEnabled(cat, levelFlag);
        }

        [Conditional("DEBUG")]
        private static void OpenBraceWorker(TraceCategory cat, string message)
        {
        }

        [Conditional("DEBUG")]
        public static void DeferOpenBrace(TraceCategory cat) => IsCategoryEnabled(cat, 1);

        [Conditional("DEBUG")]
        public static void DeferOpenBrace(TraceCategory cat, string message) => IsCategoryEnabled(cat, 1);

        [Conditional("DEBUG")]
        public static void DeferOpenBrace(TraceCategory cat, string format, object param) => IsCategoryEnabled(cat, 1);

        [Conditional("DEBUG")]
        public static void DeferOpenBrace(
          TraceCategory cat,
          string format,
          object param,
          object param2)
        {
            IsCategoryEnabled(cat, 1);
        }

        [Conditional("DEBUG")]
        public static void DeferOpenBrace(
          TraceCategory cat,
          string format,
          object param,
          object param2,
          object param3)
        {
            IsCategoryEnabled(cat, 1);
        }

        [Conditional("DEBUG")]
        public static void DeferOpenBrace(TraceCategory cat, byte levelFlag, string message) => IsCategoryEnabled(cat, levelFlag);

        [Conditional("DEBUG")]
        public static void DeferOpenBrace(
          TraceCategory cat,
          byte levelFlag,
          string format,
          object param)
        {
            IsCategoryEnabled(cat, levelFlag);
        }

        [Conditional("DEBUG")]
        public static void DeferOpenBrace(
          TraceCategory cat,
          byte levelFlag,
          string format,
          object param,
          object param2)
        {
            IsCategoryEnabled(cat, levelFlag);
        }

        [Conditional("DEBUG")]
        public static void DeferOpenBrace(
          TraceCategory cat,
          byte levelFlag,
          string format,
          object param,
          object param2,
          object param3)
        {
            IsCategoryEnabled(cat, levelFlag);
        }

        [Conditional("DEBUG")]
        private static void DeferOpenBraceWorker(TraceCategory cat, string message)
        {
        }

        [Conditional("DEBUG")]
        public static void FlushBraces()
        {
        }

        [Conditional("DEBUG")]
        public static void CloseBrace(TraceCategory cat)
        {
        }

        [Conditional("DEBUG")]
        public static void CloseBrace(TraceCategory cat, byte levelFlag)
        {
        }

        internal static byte GetTraceLevelForEvent(InputInfo info) => 0;

        [Conditional("DEBUG")]
        private static void WriteLineWorker(TraceCategory cat, object value)
        {
        }

        [Conditional("DEBUG")]
        private static void Indent()
        {
            IndentLevel++;
        }

        [Conditional("DEBUG")]
        private static void Unindent()
        {
            IndentLevel--;
        }

        private static string BuildLine(string content)
        {
            return new string('\t', IndentLevel) + content;
        }

        internal static string DEBUG_SafeFormat(string format, object param)
        {
            object[] cachedObjectArray = GetCachedObjectArray(1);
            cachedObjectArray[0] = param;
            return DEBUG_SafeFormatWorker(format, cachedObjectArray);
        }

        internal static string DEBUG_SafeFormat(string format, object param1, object param2)
        {
            object[] cachedObjectArray = GetCachedObjectArray(2);
            cachedObjectArray[0] = param1;
            cachedObjectArray[1] = param2;
            return DEBUG_SafeFormatWorker(format, cachedObjectArray);
        }

        internal static string DEBUG_SafeFormat(
          string format,
          object param1,
          object param2,
          object param3)
        {
            object[] cachedObjectArray = GetCachedObjectArray(3);
            cachedObjectArray[0] = param1;
            cachedObjectArray[1] = param2;
            cachedObjectArray[2] = param3;
            return DEBUG_SafeFormatWorker(format, cachedObjectArray);
        }

        internal static string DEBUG_SafeFormat(
          string format,
          object param1,
          object param2,
          object param3,
          object param4)
        {
            object[] cachedObjectArray = GetCachedObjectArray(4);
            cachedObjectArray[0] = param1;
            cachedObjectArray[1] = param2;
            cachedObjectArray[2] = param3;
            cachedObjectArray[3] = param4;
            return DEBUG_SafeFormatWorker(format, cachedObjectArray);
        }

        internal static string DEBUG_SafeFormat(
          string format,
          object param1,
          object param2,
          object param3,
          object param4,
          object param5)
        {
            object[] cachedObjectArray = GetCachedObjectArray(5);
            cachedObjectArray[0] = param1;
            cachedObjectArray[1] = param2;
            cachedObjectArray[2] = param3;
            cachedObjectArray[3] = param4;
            cachedObjectArray[4] = param5;
            return DEBUG_SafeFormatWorker(format, cachedObjectArray);
        }

        internal static string DEBUG_SafeFormat(
          string format,
          object param1,
          object param2,
          object param3,
          object param4,
          object param5,
          object param6)
        {
            object[] cachedObjectArray = GetCachedObjectArray(6);
            cachedObjectArray[0] = param1;
            cachedObjectArray[1] = param2;
            cachedObjectArray[2] = param3;
            cachedObjectArray[3] = param4;
            cachedObjectArray[4] = param5;
            cachedObjectArray[5] = param6;
            return DEBUG_SafeFormatWorker(format, cachedObjectArray);
        }

        private static string DEBUG_SafeFormatWorker(string format, object[] args)
        {
            lock (s_safeFormatLockObj)
            {
                s_safeFormatBuilder.Length = 0;
                s_safeFormatBuilder.AppendFormat(CultureInfo.InvariantCulture, format, args);
                Array.Clear(args, 0, args.Length);
                s_arrayCache[args.Length] = args;
                return s_safeFormatBuilder.ToString();
            }
        }

        private static object[] GetCachedObjectArray(int length)
        {
            object[] objArray = s_arrayCache[length];
            if (objArray != null)
                s_arrayCache[length] = null;
            else
                objArray = new object[length];
            return objArray;
        }

        private static void WriteLine(string line)
        {
            System.Diagnostics.Debug.WriteLine(line);
            TraceSettings.Current.FireWriteCallbacks(line);
        }
    }
}