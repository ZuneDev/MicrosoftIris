// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.DisplayMode
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using System.Collections.Generic;

namespace Microsoft.Iris.Render
{
    public struct DisplayMode
    {
        public static readonly DisplayMode[] EmptyModes = new DisplayMode[0];
        public Size sizePhysicalPxl;
        public Size sizeLogicalPxl;
        public SurfaceFormat nFormat;
        public int nRefreshRate;
        public bool fInterlaced;
        public bool fTvMode;

        public void ChangeFrom(DisplayMode modeChanges, DisplayModeFlags nChanges)
        {
            if (Bits.TestFlag((int)nChanges, 1))
            {
                this.sizePhysicalPxl = modeChanges.sizePhysicalPxl;
                this.sizeLogicalPxl = modeChanges.sizeLogicalPxl;
            }
            if (Bits.TestFlag((int)nChanges, 2))
                this.nFormat = modeChanges.nFormat;
            if (Bits.TestFlag((int)nChanges, 4))
            {
                this.nRefreshRate = modeChanges.nRefreshRate;
                this.fInterlaced = modeChanges.fInterlaced;
            }
            if (!Bits.TestFlag((int)nChanges, 8))
                return;
            this.fTvMode = modeChanges.fTvMode;
        }

        public static bool operator ==(DisplayMode left, DisplayMode right) => left.sizePhysicalPxl == right.sizePhysicalPxl && left.sizeLogicalPxl == right.sizeLogicalPxl && (left.nFormat == right.nFormat && left.nRefreshRate == right.nRefreshRate) && left.fInterlaced == right.fInterlaced && left.fTvMode == right.fTvMode;

        public static bool operator !=(DisplayMode left, DisplayMode right) => left.sizePhysicalPxl != right.sizePhysicalPxl || left.sizeLogicalPxl != right.sizeLogicalPxl || (left.nFormat != right.nFormat || left.nRefreshRate != right.nRefreshRate) || left.fInterlaced != right.fInterlaced || left.fTvMode != right.fTvMode;

        public override bool Equals(object obj) => obj is DisplayMode displayMode && this == displayMode;

        public override int GetHashCode() => this.sizePhysicalPxl.GetHashCode() ^ this.sizeLogicalPxl.GetHashCode() ^ this.nFormat.GetHashCode() ^ this.nRefreshRate ^ this.fInterlaced.GetHashCode() ^ this.fTvMode.GetHashCode();

        public static int CompareRenderModes(DisplayMode dmA, DisplayMode dmB)
        {
            if (SurfaceFormatInfo.GetBitsPerPixel(dmA.nFormat) >= SurfaceFormatInfo.GetBitsPerPixel(dmB.nFormat))
            {
                if (SurfaceFormatInfo.GetBitsPerPixel(dmA.nFormat) <= SurfaceFormatInfo.GetBitsPerPixel(dmB.nFormat))
                {
                    if (dmA.sizePhysicalPxl.Width >= dmB.sizePhysicalPxl.Width)
                    {
                        if (dmA.sizePhysicalPxl.Width <= dmB.sizePhysicalPxl.Width)
                        {
                            if (dmA.sizePhysicalPxl.Height >= dmB.sizePhysicalPxl.Height)
                            {
                                if (dmA.sizePhysicalPxl.Height <= dmB.sizePhysicalPxl.Height)
                                {
                                    if (dmA.nRefreshRate >= dmB.nRefreshRate)
                                    {
                                        if (dmA.nRefreshRate <= dmB.nRefreshRate)
                                        {
                                            if (dmA.fInterlaced || !dmB.fInterlaced)
                                            {
                                                if (!dmA.fInterlaced || dmB.fInterlaced)
                                                    return 0;
                                            }
                                            else
                                                goto label_11;
                                        }
                                    }
                                    else
                                        goto label_11;
                                }
                            }
                            else
                                goto label_11;
                        }
                    }
                    else
                        goto label_11;
                }
                return 1;
            }
        label_11:
            return -1;
        }

        public static int CompareSimilarModes(DisplayMode dmA, DisplayMode dmB)
        {
            if (SurfaceFormatInfo.GetBitsPerPixel(dmA.nFormat) >= SurfaceFormatInfo.GetBitsPerPixel(dmB.nFormat))
            {
                if (SurfaceFormatInfo.GetBitsPerPixel(dmA.nFormat) <= SurfaceFormatInfo.GetBitsPerPixel(dmB.nFormat))
                {
                    if (dmA.sizePhysicalPxl.Width >= dmB.sizePhysicalPxl.Width)
                    {
                        if (dmA.sizePhysicalPxl.Width <= dmB.sizePhysicalPxl.Width)
                        {
                            if (dmA.sizePhysicalPxl.Height >= dmB.sizePhysicalPxl.Height)
                            {
                                if (dmA.sizePhysicalPxl.Height <= dmB.sizePhysicalPxl.Height)
                                    return 0;
                            }
                            else
                                goto label_7;
                        }
                    }
                    else
                        goto label_7;
                }
                return 1;
            }
        label_7:
            return -1;
        }

        public override string ToString() => base.ToString();

        public class RenderModeComparer : IComparer<DisplayMode>
        {
            public int Compare(DisplayMode dmA, DisplayMode dmB) => CompareRenderModes(dmA, dmB);
        }

        public class SimilarModeComparer : IComparer<DisplayMode>
        {
            public int Compare(DisplayMode dmA, DisplayMode dmB) => CompareSimilarModes(dmA, dmB);
        }
    }
}
