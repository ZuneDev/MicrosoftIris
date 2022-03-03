// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.ImageRequirements
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Extensions
{
    [ComVisible(false)]
    [StructLayout(LayoutKind.Sequential)]
    public class ImageRequirements
    {
        private ImageRequirements.Fields m_nMask;
        private Size m_sizeMaximumPxl;
        private int m_nBorderPxl;
        private ColorF m_clrBorder;

        public Size MaximumSize
        {
            get => this.m_sizeMaximumPxl;
            set
            {
                this.m_sizeMaximumPxl = value;
                if (this.m_sizeMaximumPxl.IsZero)
                    this.m_nMask &= ~Fields.MaximumSize;
                else
                    this.m_nMask |= Fields.MaximumSize;
            }
        }

        public bool Flippable
        {
            get => (this.m_nMask & Fields.Flippable) != Fields.None;
            set
            {
                if (value)
                    this.m_nMask |= Fields.Flippable;
                else
                    this.m_nMask &= ~Fields.Flippable;
            }
        }

        public int BorderWidth
        {
            get => this.m_nBorderPxl;
            set
            {
                this.m_nBorderPxl = value;
                if (this.m_nBorderPxl > 0)
                    this.m_nMask |= Fields.Border;
                else
                    this.m_nMask &= ~Fields.Border;
            }
        }

        public ColorF BorderColor
        {
            get => this.m_clrBorder;
            set => this.m_clrBorder = value;
        }

        public bool AntialiasEdges
        {
            get => (this.m_nMask & Fields.AntialiasEdges) != Fields.None;
            set
            {
                if (value)
                    this.m_nMask |= Fields.AntialiasEdges;
                else
                    this.m_nMask &= ~Fields.AntialiasEdges;
            }
        }

        [Flags]
        internal enum Fields
        {
            None = 0,
            MaximumSize = 1,
            Border = 2,
            Flippable = 4,
            AntialiasEdges = 16, // 0x00000010
        }
    }
}
