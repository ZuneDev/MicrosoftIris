// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Common.Debug2
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Iris.Render.Common
{
    internal static class Debug2
    {
        private static IDebug s_debug;

        internal static IDebug Module
        {
            get => s_debug;
            set => s_debug = value;
        }

        [Conditional("DEBUG")]
        internal static void Enter(string stBlock)
        {
            if (s_debug == null)
                return;
            s_debug.Enter(stBlock);
        }

        [Conditional("DEBUG")]
        internal static void Leave(string stBlock)
        {
            if (s_debug == null)
                return;
            s_debug.Leave(stBlock);
        }

        [Conditional("DEBUG")]
        internal static void Assert(bool condition, string stMessage)
        {
            if (condition || s_debug == null)
                return;
            s_debug.Assert(condition, stMessage);
        }

        [Conditional("DEBUG")]
        internal static void Assert(bool condition, string format, object arg1)
        {
            if (condition || s_debug == null)
                return;
            s_debug.Assert(condition, string.Format(format, arg1));
        }

        [Conditional("DEBUG")]
        internal static void Assert(bool condition, string format, object arg1, object arg2)
        {
            if (condition || s_debug == null)
                return;
            s_debug.Assert(condition, string.Format(format, arg1, arg2));
        }

        internal static void Throw(bool condition, string format)
        {
            if (!condition)
            {
                if (s_debug != null)
                    s_debug.Throw(condition, format);
                throw new InvalidOperationException(format);
            }
        }

        internal static void Throw(bool condition, string format, object arg1)
        {
            if (!condition)
            {
                if (s_debug != null)
                    s_debug.Throw(condition, string.Format(format, arg1));
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, format, arg1));
            }
        }

        internal static void Validate(bool condition, Type exceptionType, string arg1)
        {
            if (!condition)
            {
                if (exceptionType == null)
                    exceptionType = typeof(ArgumentException);
                throw (Exception)Activator.CreateInstance(exceptionType, (object)arg1);
            }
        }

        internal static void Validate(bool condition, Type exceptionType, object arg1, object arg2)
        {
            if (!condition)
            {
                if (exceptionType == null)
                    exceptionType = typeof(ArgumentException);
                throw (Exception)Activator.CreateInstance(exceptionType, arg1, arg2);
            }
        }

        [Conditional("DEBUG")]
        internal static void Write(DebugCategory category, int verbosityLevel, string message)
        {
            if (!IsCategoryEnabled(category, verbosityLevel))
                return;
            s_debug.Write(category, verbosityLevel, message);
        }

        [Conditional("DEBUG")]
        internal static void WriteLine(DebugCategory category, int verbosityLevel, string format)
        {
            if (!IsCategoryEnabled(category, verbosityLevel))
                return;
            s_debug.WriteLine(category, verbosityLevel, format);
        }

        [Conditional("DEBUG")]
        internal static void WriteLine(
          DebugCategory category,
          int verbosityLevel,
          string format,
          object arg1)
        {
            if (!IsCategoryEnabled(category, verbosityLevel))
                return;
            s_debug.WriteLine(category, verbosityLevel, string.Format(format, arg1));
        }

        [Conditional("DEBUG")]
        internal static void WriteLine(
          DebugCategory category,
          int verbosityLevel,
          string format,
          object arg1,
          object arg2)
        {
            if (!IsCategoryEnabled(category, verbosityLevel))
                return;
            s_debug.WriteLine(category, verbosityLevel, string.Format(format, arg1, arg2));
        }

        [Conditional("DEBUG")]
        internal static void WriteLine(
          DebugCategory category,
          int verbosityLevel,
          string format,
          object arg1,
          object arg2,
          object arg3)
        {
            if (!IsCategoryEnabled(category, verbosityLevel))
                return;
            s_debug.WriteLine(category, verbosityLevel, string.Format(format, arg1, arg2, arg3));
        }

        [Conditional("DEBUG")]
        internal static void WriteLine(
          DebugCategory category,
          int verbosityLevel,
          string format,
          object arg1,
          object arg2,
          object arg3,
          object arg4)
        {
            if (!IsCategoryEnabled(category, verbosityLevel))
                return;
            s_debug.WriteLine(category, verbosityLevel, string.Format(format, arg1, arg2, arg3, arg4));
        }

        [Conditional("DEBUG")]
        internal static void WriteLine(
          DebugCategory category,
          int verbosityLevel,
          string format,
          object arg1,
          object arg2,
          object arg3,
          object arg4,
          object arg5)
        {
            if (!IsCategoryEnabled(category, verbosityLevel))
                return;
            s_debug.WriteLine(category, verbosityLevel, string.Format(format, arg1, arg2, arg3, arg4, arg5));
        }

        [Conditional("DEBUG")]
        internal static void WriteLine(
          DebugCategory category,
          int verbosityLevel,
          string format,
          object arg1,
          object arg2,
          object arg3,
          object arg4,
          object arg5,
          object arg6)
        {
            if (!IsCategoryEnabled(category, verbosityLevel))
                return;
            s_debug.WriteLine(category, verbosityLevel, string.Format(format, arg1, arg2, arg3, arg4, arg5, arg6));
        }

        [Conditional("DEBUG")]
        internal static void WriteLine(
          DebugCategory category,
          int verbosityLevel,
          string format,
          object arg1,
          object arg2,
          object arg3,
          object arg4,
          object arg5,
          object arg6,
          object arg7)
        {
            if (!IsCategoryEnabled(category, verbosityLevel))
                return;
            s_debug.WriteLine(category, verbosityLevel, string.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7));
        }

        [Conditional("DEBUG")]
        internal static void Indent(DebugCategory category, int verbosityLevel)
        {
            if (!IsCategoryEnabled(category, verbosityLevel))
                return;
            s_debug.Indent(category, verbosityLevel);
        }

        [Conditional("DEBUG")]
        internal static void Unindent(DebugCategory category, int verbosityLevel)
        {
            if (!IsCategoryEnabled(category, verbosityLevel))
                return;
            s_debug.Unindent(category, verbosityLevel);
        }

        [Conditional("DEBUG")]
        internal static void OpenBrace(DebugCategory category, int verbosityLevel)
        {
        }

        [Conditional("DEBUG")]
        internal static void OpenBrace(DebugCategory category, int verbosityLevel, string format)
        {
        }

        [Conditional("DEBUG")]
        internal static void OpenBrace(
          DebugCategory category,
          int verbosityLevel,
          string format,
          object arg1)
        {
            if (!IsCategoryEnabled(category, verbosityLevel))
                ;
        }

        [Conditional("DEBUG")]
        internal static void OpenBrace(
          DebugCategory category,
          int verbosityLevel,
          string format,
          object arg1,
          object arg2)
        {
            if (!IsCategoryEnabled(category, verbosityLevel))
                ;
        }

        [Conditional("DEBUG")]
        internal static void CloseBrace(DebugCategory category, int verbosityLevel)
        {
        }

        internal static bool IsCategoryEnabled(DebugCategory category, int verbosityLevel) => s_debug != null && s_debug.IsCategoryEnabled(category, verbosityLevel);

        internal static void DumpException(Debug2.ExceptionType exceptionType, Exception e)
        {
            switch (exceptionType)
            {
                default:
                    Exception innerException = e.InnerException;
                    break;
            }
        }

        public enum ExceptionType
        {
            NotHandled,
            Recovered,
            Ignored,
        }
    }
}
