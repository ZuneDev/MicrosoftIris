// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.BlobInfo
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render.Protocol
{
    internal struct BlobInfo
    {
        private const int c_SizeOfChar = 2;
        private const int c_SizeOfUInt = 4;
        private Vector m_data;
        private unsafe Message* m_pmsgBlob;
        private uint m_totalSize;
        private uint m_blobsOffset;
        private bool m_foreignByteOrder;
        private static object s_msgBlobSentinal = new object();
        private static Vector s_cache;

        public unsafe BlobInfo(RenderPort port, uint origTotal)
        {
            this.m_foreignByteOrder = port.ForeignByteOrder;
            this.m_data = s_cache;
            s_cache = null;
            if (this.m_data == null)
                this.m_data = new Vector();
            this.m_totalSize = origTotal;
            this.m_blobsOffset = origTotal;
            this.m_pmsgBlob = null;
        }

        public uint AdjustedTotalSize => this.m_totalSize;

        public uint BlobDataSize => this.m_totalSize - this.m_blobsOffset;

        public BLOBREF Add(RENDERHANDLE[] param)
        {
            uint cbData = param == null ? 0U : (uint)(param.Length * 4);
            return this.Add(param, cbData);
        }

        public BLOBREF Add(string param)
        {
            if (param == null)
                param = "";
            uint cbData = (uint)((param.Length + 1) * 2);
            return this.Add(param, cbData);
        }

        public unsafe BLOBREF Add(Message* pmsg)
        {
            uint cbData = 0;
            if ((IntPtr)pmsg != IntPtr.Zero)
            {
                this.m_pmsgBlob = pmsg;
                cbData = pmsg->cbSize;
            }
            return this.Add(s_msgBlobSentinal, cbData);
        }

        private BLOBREF Add(object param, uint cbData)
        {
            Debug2.Validate(cbData < ushort.MaxValue, typeof(ArgumentException), "data blob too large", nameof(param));
            BLOBREF blobref = BLOBREF.FromUInt32(this.m_totalSize | cbData << 16);
            if (cbData != 0U)
            {
                if (this.m_totalSize + cbData > ushort.MaxValue)
                    throw new InvalidOperationException("insufficient space in message for data blob");
                this.m_totalSize += cbData;
                this.m_data.Add(param);
            }
            return blobref;
        }

        public unsafe void Attach(Message* pMessage)
        {
            byte* numPtr1 = (byte*)pMessage + this.m_blobsOffset;
            foreach (object obj in this.m_data)
            {
                if (obj is string)
                {
                    string str = (string)obj;
                    fixed (char* chPtr1 = str)
                    {
                        if (!this.m_foreignByteOrder)
                        {
                            char* chPtr2 = (char*)numPtr1;
                            char* chPtr3 = chPtr1;
                            char* chPtr4 = chPtr1 + (str.Length + 1);
                            while (chPtr3 < chPtr4)
                                *chPtr2++ = *chPtr3++;
                            numPtr1 = (byte*)chPtr2;
                        }
                        else
                        {
                            byte* numPtr2 = (byte*)chPtr1;
                            for (byte* numPtr3 = numPtr2 + ((long)(str.Length + 1) * 2); numPtr2 < numPtr3; numPtr2 += 2)
                            {
                                *numPtr1++ = numPtr2[1];
                                *numPtr1++ = *numPtr2;
                            }
                        }
                    }
                }
                else if (obj is RENDERHANDLE[])
                {
                    RENDERHANDLE[] renderhandleArray = (RENDERHANDLE[])obj;
                    fixed (RENDERHANDLE* renderhandlePtr = renderhandleArray)
                    {
                        if (!this.m_foreignByteOrder)
                        {
                            uint* numPtr2 = (uint*)numPtr1;
                            uint* numPtr3 = (uint*)renderhandlePtr;
                            uint* numPtr4 = numPtr3 + renderhandleArray.Length;
                            while (numPtr3 < numPtr4)
                                *numPtr2++ = *numPtr3++;
                            numPtr1 = (byte*)numPtr2;
                        }
                        else
                        {
                            byte* numPtr2 = (byte*)renderhandlePtr;
                            for (byte* numPtr3 = numPtr2 + ((long)renderhandleArray.Length * 4); numPtr2 < numPtr3; numPtr2 += 4)
                            {
                                *numPtr1++ = numPtr2[3];
                                *numPtr1++ = numPtr2[2];
                                *numPtr1++ = numPtr2[1];
                                *numPtr1++ = *numPtr2;
                            }
                        }
                    }
                }
                else if (obj == s_msgBlobSentinal)
                {
                    byte* pmsgBlob = (byte*)this.m_pmsgBlob;
                    byte* numPtr2 = pmsgBlob + (int)this.m_pmsgBlob->cbSize;
                    while (pmsgBlob < numPtr2)
                        *numPtr1++ = *pmsgBlob++;
                }
                else
                    Debug2.Throw(false, "Unexpected type.");
            }
            this.m_data.Clear();
            s_cache = this.m_data;
            this.m_data = null;
        }
    }
}
