// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.eDebugApi
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Iris.Render.Extensions
{
    [SuppressUnmanagedCodeSecurity]
    internal static class eDebugApi
    {
        private const string k_strEhDebugDllFileName = "UIXsup.dll";

        static eDebugApi()
        {
            IntPtr h = Win32Api.LoadLibraryEx("UIXsup.dll", Win32Api.HANDLE.NULL, 0U).h;
        }

        [DllImport("UIXsup.dll")]
        internal static extern bool DebugDisplayErrorStack(
          string stMessage,
          string filename,
          int line,
          string title,
          string stackTrace);

        [DllImport("UIXsup.dll")]
        internal static extern void DebugSetTimedWriteLines(bool fEnabled);

        [DllImport("UIXsup.dll")]
        internal static extern void DebugSetWriteLinePrefix(string stPrefix);

        [DllImport("UIXsup.dll")]
        internal static extern byte DebugGetCategoryLevel(DebugCategory cat);

        [DllImport("UIXsup.dll")]
        internal static extern void DebugSetCategoryLevel(DebugCategory cat, byte level);
    }
}
