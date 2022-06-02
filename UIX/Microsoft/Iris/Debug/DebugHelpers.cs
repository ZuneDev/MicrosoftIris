// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Debug.DebugHelpers
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.OS;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Iris.Debug
{
    [SuppressUnmanagedCodeSecurity]
    internal class DebugHelpers
    {
        public static void Break()
        {
            if (Debugger.IsAttached || !Win32Api.IsDebuggerPresent())
                Debugger.Break();
            else
                Win32Api.DebugBreak();
        }

        public static bool DisplayStackTrace(
          string title,
          string messageName,
          StackTrace trace,
          int ignoreFramesCount)
        {
            return false;
        }

        [DllImport("UIXsup.dll")]
        private static extern bool DebugDisplayErrorStack(
          string messageName,
          string filename,
          int line,
          string title,
          string traceStack);

        public static object DEBUG_ObjectToString(object obj)
        {
            if (obj == null)
                return "null";
            if ((object)(obj as Delegate) != null)
                obj = DEBUG_DelegateToString((Delegate)obj, null);
            else if (obj is Type)
                obj = ((Type)obj).Name;
            return obj;
        }

        public static string DEBUG_DelegateToString(Delegate d, object args)
        {
            if ((object)d == null)
                return "null";
            Delegate[] invocationList = d.GetInvocationList();
            string str = "ERROR DISPLAYING DELEGATE";
            if (invocationList != null && invocationList.Length > 0)
            {
                if (invocationList.Length == 1)
                {
                    str = DEBUG_UnicastToString(invocationList[0], args);
                }
                else
                {
                    string[] strArray = new string[invocationList.Length];
                    for (int index = 0; index < invocationList.Length; ++index)
                        strArray[index] = "\r\n" + DEBUG_UnicastToString(invocationList[index], args);
                    str = " MULTICAST:" + string.Concat(strArray);
                }
            }
            return str;
        }

        private static string DEBUG_UnicastToString(Delegate d, object args)
        {
            if ((object)d == null)
                return "null";
            object target = d.Target;
            MethodInfo method = d.Method;
            return target == null ? Trace.DEBUG_SafeFormat("{0}.{1}({2})", method.DeclaringType.Name, method.Name, args) : Trace.DEBUG_SafeFormat("{0}.{1}({2}) -> {3}", method.DeclaringType.Name, method.Name, args, target);
        }

        public static string DEBUG_EscapeString(string text) => text;
    }
}
