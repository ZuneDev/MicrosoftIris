// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.CoordMap
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;
using System.Collections;

namespace Microsoft.Iris.Render
{
    public class CoordMap
    {
        private ArrayList m_alMaps;

        public CoordMap() => this.m_alMaps = new ArrayList();

        public void AddValue(float flPosition, float flValue, Orientation or)
        {
            Debug2.Validate(flPosition >= 0.0, typeof(ArgumentOutOfRangeException), "flPosition cannot be less than 0.0f");
            Debug2.Validate(flValue >= 0.0, typeof(ArgumentOutOfRangeException), "flValue cannot be less than 0.0f");
            Debug2.Validate(flPosition <= 1.0, typeof(ArgumentOutOfRangeException), "flPosition cannot exceed 1.0f");
            Debug2.Validate(flValue <= 1.0, typeof(ArgumentOutOfRangeException), "flValue cannot exceed 1.0f");
            foreach (CoordMap.CoordMapSample alMap in this.m_alMaps)
            {
                if (or == alMap.Orientation)
                    Debug2.Validate(flPosition != (double)alMap.flPosition, typeof(ArgumentException), "CoordMap cannot have two entries with the same flPosition and the same Orientation");
            }
            this.m_alMaps.Add(new CoordMap.CoordMapSample(flPosition, flValue, or));
        }

        public void Clear() => this.m_alMaps.Clear();

        internal ArrayList RampSamples => this.m_alMaps;

        internal struct CoordMapSample
        {
            internal float flPosition;
            internal float flValue;
            internal Orientation Orientation;

            internal CoordMapSample(float pos, float val, Orientation or)
            {
                this.flPosition = pos;
                this.flValue = val;
                this.Orientation = or;
            }
        }
    }
}
