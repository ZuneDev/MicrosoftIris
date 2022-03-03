// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.DisplayManager
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Internal
{
    internal class DisplayManager :
      RenderObject,
      IDisplayManager,
      IDesktopManagerCallback,
      IRenderHandleOwner,
      IEnumerable
    {
        private const uint MONITORINFOF_PRIMARY = 1;
        private static ushort[] s_ByteOrder_MonitorInfoCallbackMsg;
        private RenderSession m_session;
        private RenderEngine m_engine;
        private RemoteDesktopManager m_remoteManager;
        private DisplayMode[] m_arExtraModes;
        private DisplayManager.DisplayInfo m_info;
        private DisplayManager.DisplayInfo m_infoRebuilding;
        private Display m_dispRebuildLastAdded;
        private ArrayList m_alRebuildSupportedModes;
        private ArrayList m_alRebuildAllModes;
        private bool m_fPendingChangeNotification;

        private DisplayManager(RenderSession session, RenderEngine engine, bool fEnumDisplayModes)
        {
            this.m_session = session;
            this.m_engine = engine;
            this.m_info = new DisplayManager.DisplayInfo();
            this.m_arExtraModes = DisplayMode.EmptyModes;
            this.m_remoteManager = this.m_session.BuildRemoteDesktopManager(this, fEnumDisplayModes);
            this.m_remoteManager.SendRebuildMonitorCache();
        }

        internal static DisplayManager CreateDisplayManager(
          RenderSession session,
          RenderEngine engine,
          bool fEnumDisplayModes)
        {
            return new DisplayManager(session, engine, fEnumDisplayModes);
        }

        protected override void Dispose(bool fInDispose)
        {
            if (fInDispose && this.m_remoteManager != null)
                this.m_remoteManager.Dispose();
            this.m_remoteManager = null;
            base.Dispose(fInDispose);
        }

        public IDisplay PrimaryDisplay => m_info.primaryDisplay;

        public DisplayMode[] ExtraModes
        {
            get => this.m_arExtraModes;
            set
            {
                if (value == null)
                    value = DisplayMode.EmptyModes;
                this.m_arExtraModes = value;
            }
        }

        internal RemoteDesktopManager RemoteStub => this.m_remoteManager;

        public IDisplay DisplayFromDeviceName(string stDeviceName) => this.DisplayFromName(stDeviceName);

        private Display DisplayFromName(string stDeviceName)
        {
            Display display1 = null;
            if (!string.IsNullOrEmpty(stDeviceName))
            {
                foreach (Display display2 in this.m_info.displays)
                {
                    if (string.Compare(display2.DeviceName, stDeviceName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        display1 = display2;
                        break;
                    }
                }
            }
            return display1;
        }

        internal Display DisplayFromUniqueId(uint idDisplay)
        {
            foreach (Display display in this.m_info.displays)
            {
                if ((int)idDisplay == (int)display.UniqueId)
                    return display;
            }
            return null;
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteManager.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteManager = null;

        void IDesktopManagerCallback.OnBeginEnumMonitorInfo(RENDERHANDLE target)
        {
            this.m_infoRebuilding = new DisplayManager.DisplayInfo();
            this.m_dispRebuildLastAdded = null;
            this.m_alRebuildSupportedModes = new ArrayList();
            this.m_alRebuildAllModes = new ArrayList();
        }

        void IDesktopManagerCallback.OnEndEnumMonitorInfo(RENDERHANDLE target)
        {
            foreach (Display display in this.m_infoRebuilding.displays)
            {
                if (display.IsPrimary)
                    this.m_infoRebuilding.primaryDisplay = display;
            }
            this.m_info = this.m_infoRebuilding;
            this.m_infoRebuilding = null;
            this.m_dispRebuildLastAdded = null;
            this.m_alRebuildSupportedModes = null;
            this.m_alRebuildAllModes = null;
            if (this.DisplayChange == null || this.m_fPendingChangeNotification)
                return;
            this.m_session.DeferredInvoke(new DeferredHandler(this.DeliverDisplayChangeNotification), null, DeferredInvokePriority.Normal);
            this.m_fPendingChangeNotification = true;
        }

        unsafe void IDesktopManagerCallback.OnMonitorInfo(
          RENDERHANDLE target,
          Message* pmsgRaw)
        {
            if (this.m_session.IsForeignByteOrderOnWindowing)
                MarshalHelper.SwapByteOrder((byte*)pmsgRaw, ref s_ByteOrder_MonitorInfoCallbackMsg, typeof(DisplayManager.MonitorInfoCallbackMsg), 0, 0);
            Display display = this.ConvertMonitorInfo((DisplayManager.MonitorInfoCallbackMsg*)pmsgRaw);
            this.m_dispRebuildLastAdded = display;
            this.m_alRebuildSupportedModes.Clear();
            this.m_alRebuildAllModes.Clear();
            this.m_infoRebuilding.displays.Add(display);
        }

        void IDesktopManagerCallback.OnBeginDisplayModes(RENDERHANDLE target)
        {
        }

        void IDesktopManagerCallback.OnDisplayMode(
          RENDERHANDLE target,
          RenderDisplayMode rmodeAdd,
          bool fSupported)
        {
            DisplayMode displayMode = new DisplayMode();
            displayMode.sizePhysicalPxl = rmodeAdd.sizePhysicalPxl;
            displayMode.sizeLogicalPxl = rmodeAdd.sizeLogicalPxl;
            displayMode.nFormat = rmodeAdd.nFormat;
            displayMode.nRefreshRate = rmodeAdd.nRefreshRate;
            displayMode.fInterlaced = rmodeAdd.Interlaced;
            displayMode.fTvMode = false;
            if (fSupported)
                this.m_alRebuildSupportedModes.Add(displayMode);
            else
                this.m_alRebuildAllModes.Add(displayMode);
        }

        void IDesktopManagerCallback.OnEndDisplayModes(RENDERHANDLE target)
        {
            DisplayMode[] arSupportedModes;
            DisplayMode[] arExtraModes;
            DisplayMode[] arAllModes;
            this.BuildModeLists(out arSupportedModes, out arExtraModes, out arAllModes);
            if (arSupportedModes.Length <= 0)
                return;
            this.m_dispRebuildLastAdded.UpdateDisplayModes(arSupportedModes, arExtraModes, arAllModes);
        }

        public IEnumerator GetEnumerator() => this.m_info != null ? this.m_info.displays.GetEnumerator() : new Display[0].GetEnumerator();

        public event EventHandler DisplayChange;

        private void DeliverDisplayChangeNotification(object param)
        {
            if (!this.m_fPendingChangeNotification)
                return;
            this.m_fPendingChangeNotification = false;
            if (this.DisplayChange == null)
                return;
            this.DisplayChange(this, EventArgs.Empty);
        }

        private unsafe Display ConvertMonitorInfo(DisplayManager.MonitorInfoCallbackMsg* pmsg)
        {
            string stringUni1 = Marshal.PtrToStringUni(new IntPtr(&pmsg->pad_szDevice1));
            Display display = null;
            if (this.m_info != null)
                display = this.DisplayFromName(stringUni1);
            if (display == null)
                display = new Display(this, this.m_engine, stringUni1, pmsg->idDisplay);
            Rectangle rcMonitor = new Rectangle(pmsg->rcMonLeft, pmsg->rcMonTop, pmsg->rcMonRight - pmsg->rcMonLeft, pmsg->rcMonBottom - pmsg->rcMonTop);
            Rectangle rcWork = new Rectangle(pmsg->rcWrkLeft, pmsg->rcWrkTop, pmsg->rcWrkRight - pmsg->rcWrkLeft, pmsg->rcWrkBottom - pmsg->rcWrkTop);
            bool fPrimary = ((int)pmsg->dwFlags & 1) != 0;
            TvFormat nTvFormat = (TvFormat)pmsg->nTvFormat;
            DisplayMode modeDesktop = new DisplayMode();
            modeDesktop.sizePhysicalPxl.Width = pmsg->sizeScreenWidth;
            modeDesktop.sizePhysicalPxl.Height = pmsg->sizeScreenHeight;
            modeDesktop.sizeLogicalPxl.Width = pmsg->sizeScreenWidth;
            modeDesktop.sizeLogicalPxl.Height = pmsg->sizeScreenHeight;
            modeDesktop.nFormat = (SurfaceFormat)pmsg->nFormat;
            modeDesktop.nRefreshRate = pmsg->nRefreshRate;
            modeDesktop.fInterlaced = pmsg->fInterlaced != 0U;
            modeDesktop.fTvMode = false;
            string stringUni2 = Marshal.PtrToStringUni(new IntPtr(&pmsg->pad_szMonitorPnP1));
            display.UpdateMonitorInfo(rcMonitor, rcWork, fPrimary, nTvFormat, stringUni2, modeDesktop);
            return display;
        }

        private void BuildModeLists(
          out DisplayMode[] arSupportedModes,
          out DisplayMode[] arExtraModes,
          out DisplayMode[] arAllModes)
        {
            DisplayMode.RenderModeComparer renderModeComparer = new DisplayMode.RenderModeComparer();
            arSupportedModes = (DisplayMode[])this.m_alRebuildSupportedModes.ToArray(typeof(DisplayMode));
            Array.Sort<DisplayMode>(arSupportedModes, renderModeComparer);
            arAllModes = (DisplayMode[])this.m_alRebuildAllModes.ToArray(typeof(DisplayMode));
            Array.Sort<DisplayMode>(arAllModes, renderModeComparer);
            Vector<DisplayMode> vector = new Vector<DisplayMode>();
            int length = this.m_arExtraModes.Length;
            List<int>[] intListArray = new List<int>[length];
            for (int index = 0; index < length; ++index)
                intListArray[index] = new List<int>();
            for (int index1 = 0; index1 < this.m_alRebuildAllModes.Count; ++index1)
            {
                DisplayMode alRebuildAllMode = (DisplayMode)this.m_alRebuildAllModes[index1];
                if (Array.BinarySearch<DisplayMode>(arSupportedModes, alRebuildAllMode, renderModeComparer) < 0)
                {
                    for (int index2 = 0; index2 < this.m_arExtraModes.Length; ++index2)
                    {
                        DisplayMode arExtraMode = this.m_arExtraModes[index2];
                        if (DisplayMode.CompareRenderModes(alRebuildAllMode, arExtraMode) == 0)
                        {
                            vector.Add(alRebuildAllMode);
                            intListArray[index2] = null;
                        }
                        else if (DisplayMode.CompareSimilarModes(alRebuildAllMode, arExtraMode) == 0)
                            intListArray[index2]?.Add(index1);
                    }
                }
            }
            foreach (List<int> intList in intListArray)
            {
                if (intList != null)
                {
                    foreach (int index in intList)
                        vector.Add((DisplayMode)this.m_alRebuildAllModes[index]);
                }
            }
            arExtraModes = vector.ToArray();
            Array.Sort<DisplayMode>(arExtraModes, renderModeComparer);
        }

        private class DisplayInfo
        {
            public Display primaryDisplay;
            public ArrayList displays = new ArrayList();
        }

        [ComVisible(false)]
        private struct MonitorInfoCallbackMsg
        {
            public uint cbSize;
            public int nMsg;
            public RENDERHANDLE idObjectSubject;
            public uint idDisplay;
            public uint nTvFormat;
            public int sizeScreenWidth;
            public int sizeScreenHeight;
            public uint nFormat;
            public int nRefreshRate;
            public uint fInterlaced;
            public uint cbMiSize;
            public int rcMonLeft;
            public int rcMonTop;
            public int rcMonRight;
            public int rcMonBottom;
            public int rcWrkLeft;
            public int rcWrkTop;
            public int rcWrkRight;
            public int rcWrkBottom;
            public uint dwFlags;
            public long pad_szDevice1;
            public long pad_szDevice2;
            public long pad_szDevice3;
            public long pad_szDevice4;
            public long pad_szDevice5;
            public long pad_szDevice6;
            public long pad_szDevice7;
            public long pad_szDevice8;
            public long pad_szMonitorPnP1;
            public long pad_szMonitorPnP2;
            public long pad_szMonitorPnP3;
            public long pad_szMonitorPnP4;
            public long pad_szMonitorPnP5;
            public long pad_szMonitorPnP6;
            public long pad_szMonitorPnP7;
            public long pad_szMonitorPnP8;
            public long pad_szMonitorPnP9;
            public long pad_szMonitorPnP10;
            public long pad_szMonitorPnP11;
            public long pad_szMonitorPnP12;
            public long pad_szMonitorPnP13;
            public long pad_szMonitorPnP14;
            public long pad_szMonitorPnP15;
            public long pad_szMonitorPnP16;
            public long pad_szMonitorPnP17;
            public long pad_szMonitorPnP18;
            public long pad_szMonitorPnP19;
            public long pad_szMonitorPnP20;
            public long pad_szMonitorPnP21;
            public long pad_szMonitorPnP22;
            public long pad_szMonitorPnP23;
            public long pad_szMonitorPnP24;
            public long pad_szMonitorPnP25;
            public long pad_szMonitorPnP26;
            public long pad_szMonitorPnP27;
            public long pad_szMonitorPnP28;
            public long pad_szMonitorPnP29;
            public long pad_szMonitorPnP30;
            public long pad_szMonitorPnP31;
            public long pad_szMonitorPnP32;
        }
    }
}
