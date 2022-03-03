// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Common.ByteBuilder
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Common
{
    internal class ByteBuilder
    {
        private byte[] m_data;
        private int m_nCurrentIndex;
        private byte m_nHashCode;

        internal ByteBuilder(int nMaxSize)
        {
            this.m_data = new byte[nMaxSize];
            this.m_nCurrentIndex = 0;
            this.m_nHashCode = 0;
        }

        internal int Capacity => this.m_data.Length;

        internal int Size => this.m_nCurrentIndex;

        internal void AppendByte(byte data)
        {
            this.m_data[this.m_nCurrentIndex] = data;
            ++this.m_nCurrentIndex;
        }

        internal unsafe void AppendInt(int nData)
        {
            Marshal.Copy(new IntPtr(&nData), this.m_data, this.m_nCurrentIndex, 4);
            this.m_nCurrentIndex += 4;
        }

        internal unsafe void AppendFloat(float fData)
        {
            Marshal.Copy(new IntPtr(&fData), this.m_data, this.m_nCurrentIndex, 4);
            this.m_nCurrentIndex += 4;
        }

        internal unsafe void AppendVector(Vector2 vData)
        {
            Marshal.Copy(new IntPtr(&vData), this.m_data, this.m_nCurrentIndex, sizeof(Vector2));
            this.m_nCurrentIndex += 8;
        }

        internal unsafe void AppendVector(Vector3 vData)
        {
            Marshal.Copy(new IntPtr(&vData), this.m_data, this.m_nCurrentIndex, sizeof(Vector3));
            this.m_nCurrentIndex += 12;
        }

        internal unsafe void AppendVector(Vector4 vData)
        {
            Marshal.Copy(new IntPtr(&vData), this.m_data, this.m_nCurrentIndex, sizeof(Vector4));
            this.m_nCurrentIndex += 16;
        }

        internal void Reset() => this.m_nCurrentIndex = 0;

        public override int GetHashCode()
        {
            if (this.m_nHashCode != 0)
                return m_nHashCode;
            int num = 0;
            for (int index = 0; index < this.m_nCurrentIndex; ++index)
                num += this.m_data[index];
            this.m_nHashCode = (byte)num;
            return m_nHashCode;
        }

        public override bool Equals(object obj)
        {
            ByteBuilder byteBuilder = obj as ByteBuilder;
            if (byteBuilder.Size != this.Size)
                return false;
            for (int index = 0; index < this.Size; ++index)
            {
                if (this.m_data[index] != byteBuilder.m_data[index])
                    return false;
            }
            return true;
        }
    }
}
