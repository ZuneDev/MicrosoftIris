// Decompiled with JetBrains decompiler
// Type: ZuneUI.MonitorDetector
// Assembly: ZuneShell, Version=4.7.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: FC8028F3-A47B-4FB4-B35B-11D1752D8264
// Assembly location: C:\Program Files\Zune\ZuneShell.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Iris.Render;
using Microsoft.Iris.Render.Monitors;

namespace Microsoft.Iris.OS
{
    public class Win32MonitorSystem : IMonitorSystem
    {
        private List<MonitorSize> _listInProgress;

        public List<MonitorSize> DetectMonitors()
        {
            List<MonitorSize> monitorSizeList = [];
            _listInProgress = monitorSizeList;
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumerated, IntPtr.Zero);
            _listInProgress = null;
            return monitorSizeList;
        }

        public float GetDpi() => NativeApi.SpGetDpi();

        public float GetDisplayScale() => GetDpi() / 96f;

        private bool MonitorEnumerated(
          IntPtr hMonitor,
          IntPtr hdcMonitor,
          [In] ref RECT lprcMonitor,
          IntPtr dwData)
        {
            MONITORINFO lpmi = new MONITORINFO();
            lpmi.cbSize = Marshal.SizeOf(lpmi);
            if (GetMonitorInfo(hMonitor, ref lpmi))
            {
                --lpmi.rcMonitor.Right;
                --lpmi.rcMonitor.Bottom;
                --lpmi.rcWorkArea.Right;
                --lpmi.rcWorkArea.Bottom;
                
                Rectangle totalRect = new(lpmi.rcMonitor.Left, lpmi.rcMonitor.Top, lpmi.rcMonitor.Right, lpmi.rcMonitor.Bottom);
                Rectangle workRect = new(lpmi.rcWorkArea.Left, lpmi.rcWorkArea.Top, lpmi.rcWorkArea.Right, lpmi.rcWorkArea.Bottom);
                _listInProgress.Add(new MonitorSize(totalRect, workRect));
            }
            return true;
        }

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(
          IntPtr hdc,
          IntPtr lprcClip,
          MonitorEnumProc lpfnEnum,
          IntPtr dwData);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        private delegate bool MonitorEnumProc(
          IntPtr hMonitor,
          IntPtr hdcMonitor,
          [In] ref RECT lprcMonitor,
          IntPtr dwData);

        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWorkArea;
            public int dwFlags;
        }
        
        internal struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
