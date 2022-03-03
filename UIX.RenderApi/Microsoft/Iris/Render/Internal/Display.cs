// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.Display
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Iris.Render.Internal
{
    internal class Display : IDisplay
    {
        private string m_stDeviceName;
        private Rectangle m_rcMonitor;
        private Rectangle m_rcWork;
        private bool m_fPrimaryMonitor;
        private DisplayManager m_owner;
        private RenderEngine m_engine;
        private uint m_nDisplayId;
        private TvFormat m_nTvFormat;
        private string m_stMonitorPnP;
        private DisplayMode m_modeDesktop;
        private DisplayMode m_modeCurrent;
        private DisplayModeFlags m_nCurrentValid;
        private DisplayMode[] m_arSupportedModes;
        private DisplayMode[] m_arExtraModes;
        private DisplayMode[] m_arAllModes;

        internal Display(
          DisplayManager owner,
          RenderEngine engine,
          string stDeviceName,
          uint nDisplayID)
        {
            this.m_stDeviceName = stDeviceName;
            this.m_owner = owner;
            this.m_engine = engine;
            this.m_nDisplayId = nDisplayID;
            this.m_arSupportedModes = DisplayMode.EmptyModes;
            this.m_arExtraModes = DisplayMode.EmptyModes;
            this.m_arAllModes = DisplayMode.EmptyModes;
            this.m_modeCurrent = new DisplayMode();
            this.m_nCurrentValid = 0;
        }

        public string DeviceName => this.m_stDeviceName;

        public Rectangle ScreenArea => this.m_rcMonitor;

        public Rectangle WorkArea => this.m_rcWork;

        public bool IsPrimary => this.m_fPrimaryMonitor;

        public TvFormat TvFormat => this.m_nTvFormat;

        public bool TvMode => this.m_modeCurrent.fTvMode;

        public Size LogicalFullScreenResolution => this.m_modeCurrent.sizeLogicalPxl.IsZero ? (this.m_modeCurrent.sizePhysicalPxl.IsZero ? this.m_rcMonitor.Size : this.m_modeCurrent.sizePhysicalPxl) : this.m_modeCurrent.sizeLogicalPxl;

        internal uint UniqueId => this.m_nDisplayId;

        public DisplayMode[] SupportedModes => this.m_arSupportedModes;

        public DisplayMode[] ExtraModes => this.m_arExtraModes;

        public DisplayMode[] AllModes => this.m_arAllModes;

        public DisplayMode CurrentMode => this.m_modeCurrent;

        public DisplayMode DesktopMode => this.m_modeDesktop;

        public string MonitorPnP => this.m_stMonitorPnP;

        public bool ValidateDisplayMode(
          DisplayMode modeDesired,
          DisplayModeFlags nCheck,
          bool fAllowAllModes,
          out DisplayMode modeComplete,
          out DisplayModeFlags nCompleteCheck)
        {
            if (fAllowAllModes)
            {
                if (this.m_arAllModes == null || this.m_arAllModes.Length == 0)
                {
                    modeComplete = modeDesired;
                    nCompleteCheck = nCheck;
                    return false;
                }
            }
            else if (this.m_arSupportedModes == null || this.m_arSupportedModes.Length == 0)
            {
                modeComplete = modeDesired;
                nCompleteCheck = nCheck;
                return false;
            }
            this.CompleteDisplayMode(ref modeDesired, ref nCheck);
            modeComplete = this.m_modeCurrent;
            nCompleteCheck = this.m_nCurrentValid;
            DisplayMode[] array = fAllowAllModes ? this.m_arAllModes : this.m_arExtraModes;
            DisplayMode.RenderModeComparer renderModeComparer = new DisplayMode.RenderModeComparer();
            if (Array.BinarySearch<DisplayMode>(array, modeDesired, renderModeComparer) >= 0)
            {
                modeComplete = modeDesired;
                nCompleteCheck = DisplayModeFlags.Size | DisplayModeFlags.Format | DisplayModeFlags.RefreshRate;
            }
            else
            {
                modeComplete = this.m_modeDesktop;
                int num = ComputeRank(this.m_modeDesktop, modeDesired, nCheck);
                nCompleteCheck = DisplayModeFlags.Size | DisplayModeFlags.Format | DisplayModeFlags.RefreshRate;
                foreach (DisplayMode arSupportedMode in this.m_arSupportedModes)
                {
                    int rank = ComputeRank(arSupportedMode, modeDesired, nCheck);
                    if (rank >= num)
                    {
                        modeComplete = arSupportedMode;
                        num = rank;
                        nCompleteCheck = DisplayModeFlags.Size | DisplayModeFlags.Format | DisplayModeFlags.RefreshRate;
                    }
                }
                if (modeDesired.sizeLogicalPxl.Width > 0 && modeDesired.sizeLogicalPxl.Height > 0)
                {
                    modeComplete.sizeLogicalPxl.Width = modeComplete.sizePhysicalPxl.Width;
                    modeComplete.sizeLogicalPxl.Height = modeComplete.sizePhysicalPxl.Width * modeDesired.sizeLogicalPxl.Height / modeDesired.sizeLogicalPxl.Width;
                }
                else
                    modeComplete.sizeLogicalPxl = modeComplete.sizePhysicalPxl;
            }
            if (Bits.TestFlag((int)nCheck, 8))
            {
                modeComplete.fTvMode = modeDesired.fTvMode;
                nCompleteCheck |= DisplayModeFlags.TvMode;
            }
            return true;
        }

        public bool ChangeFullScreenResolution(DisplayMode modeChanges, DisplayModeFlags nValid)
        {
            bool flag = false;
            Debug2.Validate(modeChanges.sizePhysicalPxl.Width == 0 && modeChanges.sizePhysicalPxl.Height == 0 || modeChanges.sizePhysicalPxl.Width > 0 && modeChanges.sizePhysicalPxl.Height > 0, typeof(ArgumentException), "Must specify physical 0 x 0 or a positive size", typeof(ArgumentOutOfRangeException));
            Debug2.Validate(modeChanges.sizeLogicalPxl.Width == 0 && modeChanges.sizeLogicalPxl.Height == 0 || modeChanges.sizeLogicalPxl.Width > 0 && modeChanges.sizeLogicalPxl.Height > 0, typeof(ArgumentException), "Must specify logical 0 x 0 or a positive size", typeof(ArgumentOutOfRangeException));
            Debug2.Validate(modeChanges.nRefreshRate >= 0 && modeChanges.nRefreshRate < 1000, typeof(ArgumentException), "Must have valid refresh rate");
            DisplayMode modeCurrent = this.m_modeCurrent;
            this.m_modeCurrent.ChangeFrom(modeChanges, nValid);
            this.m_nCurrentValid |= nValid;
            if (this.m_modeCurrent != modeCurrent)
            {
                this.m_owner.RemoteStub.SendChangeFullScreenMode(this.m_nDisplayId, new RenderDisplayMode()
                {
                    sizePhysicalPxl = this.m_modeCurrent.sizePhysicalPxl,
                    sizeLogicalPxl = this.m_modeCurrent.sizeLogicalPxl,
                    nFormat = this.m_modeCurrent.nFormat,
                    nRefreshRate = this.m_modeCurrent.nRefreshRate,
                    Interlaced = this.m_modeCurrent.fInterlaced
                }, this.m_modeCurrent.fTvMode);
                flag = true;
                this.m_engine.Window.NotifyDisplayReconfigured();
            }
            return flag;
        }

        private static int ComputeRank(
          DisplayMode modeValid,
          DisplayMode modeDesired,
          DisplayModeFlags nCheck)
        {
            int num1 = 0;
            if (Bits.TestFlag((int)nCheck, 2))
            {
                if (modeValid.nFormat == modeDesired.nFormat)
                {
                    num1 += 4000;
                }
                else
                {
                    int num2 = SurfaceFormatInfo.GetBitsPerPixel(modeValid.nFormat);
                    int num3 = SurfaceFormatInfo.GetBitsPerPixel(modeDesired.nFormat);
                    if (num2 == 24)
                        num2 = 32;
                    if (num3 == 24)
                        num3 = 32;
                    if (num2 == num3)
                        num1 += 3500;
                }
            }
            if (Bits.TestFlag((int)nCheck, 1) && !modeValid.sizePhysicalPxl.IsZero)
            {
                Size size = new Size(modeDesired.sizePhysicalPxl.Width - modeValid.sizePhysicalPxl.Width, modeDesired.sizePhysicalPxl.Height - modeValid.sizePhysicalPxl.Height);
                if (modeValid.sizePhysicalPxl.Width < 640 && modeValid.sizePhysicalPxl.Height < 480 || size.Width >= 0 && size.Height >= 0)
                {
                    float flValue1 = Math.Abs(size.Width / (float)modeValid.sizePhysicalPxl.Width);
                    float flValue2 = Math.Abs(size.Height / (float)modeValid.sizePhysicalPxl.Height);
                    int num2 = 2000 - (int)(1000.0 * Math2.Clamp(flValue1, 0.0f, 1f) + 1000.0 * Math2.Clamp(flValue2, 0.0f, 1f));
                    if (num2 < 0)
                        num2 = 0;
                    num1 += num2;
                }
            }
            if (Bits.TestFlag((int)nCheck, 4) && modeValid.fInterlaced == modeDesired.fInterlaced)
            {
                int num2 = Math.Abs(modeDesired.nRefreshRate - modeValid.nRefreshRate);
                if (num2 >= 15)
                    return -1;
                int num3 = 1000 * (15 - num2) / 15;
                num1 += num3;
            }
            return num1;
        }

        private void CompleteDisplayMode(ref DisplayMode modeDesired, ref DisplayModeFlags nCheck)
        {
            if (!Bits.TestFlag((int)nCheck, 1))
            {
                if (Bits.TestFlag((int)this.m_nCurrentValid, 1) && !this.m_modeCurrent.sizePhysicalPxl.IsZero)
                {
                    modeDesired.sizePhysicalPxl = this.m_modeCurrent.sizePhysicalPxl;
                    modeDesired.sizeLogicalPxl = this.m_modeCurrent.sizeLogicalPxl;
                    nCheck |= DisplayModeFlags.Size;
                }
                else
                {
                    modeDesired.sizePhysicalPxl.Width = this.m_rcMonitor.Width;
                    modeDesired.sizePhysicalPxl.Height = this.m_rcMonitor.Height;
                    modeDesired.sizeLogicalPxl.Width = this.m_rcMonitor.Width;
                    modeDesired.sizeLogicalPxl.Height = this.m_rcMonitor.Height;
                    nCheck |= DisplayModeFlags.Size;
                }
            }
            if (!Bits.TestFlag((int)nCheck, 2) && Bits.TestFlag((int)this.m_nCurrentValid, 2))
            {
                modeDesired.nFormat = this.m_modeCurrent.nFormat;
                nCheck |= DisplayModeFlags.Format;
            }
            if (!Bits.TestFlag((int)nCheck, 4))
            {
                if (Bits.TestFlag((int)this.m_nCurrentValid, 4) && this.m_modeCurrent.nRefreshRate > 0)
                {
                    modeDesired.nRefreshRate = this.m_modeCurrent.nRefreshRate;
                    modeDesired.fInterlaced = this.m_modeCurrent.fInterlaced;
                    nCheck |= DisplayModeFlags.RefreshRate;
                }
                else
                {
                    modeDesired.nRefreshRate = 60;
                    modeDesired.fInterlaced = false;
                    nCheck |= DisplayModeFlags.RefreshRate;
                }
            }
            if (Bits.TestFlag((int)nCheck, 8) || !Bits.TestFlag((int)this.m_nCurrentValid, 8))
                return;
            modeDesired.fTvMode = this.m_modeCurrent.fTvMode;
            nCheck |= DisplayModeFlags.TvMode;
        }

        internal void UpdateMonitorInfo(
          Rectangle rcMonitor,
          Rectangle rcWork,
          bool fPrimary,
          TvFormat nTvFormat,
          string stMonitorPnP,
          DisplayMode modeDesktop)
        {
            this.m_rcMonitor = rcMonitor;
            this.m_rcWork = rcWork;
            this.m_fPrimaryMonitor = fPrimary;
            this.m_nTvFormat = nTvFormat;
            this.m_modeDesktop = modeDesktop;
            this.m_stMonitorPnP = stMonitorPnP;
        }

        internal void UpdateDisplayModes(
          DisplayMode[] arNewSupportedModes,
          DisplayMode[] arNewExtraModes,
          DisplayMode[] arNewAllModes)
        {
            this.m_arSupportedModes = arNewSupportedModes;
            if (this.m_arSupportedModes == null)
                this.m_arSupportedModes = DisplayMode.EmptyModes;
            this.m_arExtraModes = arNewExtraModes;
            if (this.m_arExtraModes == null)
                this.m_arExtraModes = DisplayMode.EmptyModes;
            this.m_arAllModes = arNewAllModes;
            if (this.m_arAllModes != null)
                return;
            this.m_arAllModes = DisplayMode.EmptyModes;
        }
    }
}
