// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.ByteCodeWriter
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.OS;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Markup
{
    public class ByteCodeWriter
    {
        private const int BLOCK_SIZE = 4096;
        private byte[] _scratch = new byte[8];
        private uint _cbFreeInBlock;
        private uint _totalSize;
        private byte[] _currentBlock;
        private ArrayList _blockList = new ArrayList();

        public uint DataSize => _totalSize;

        public void WriteByte(byte value)
        {
            _scratch[0] = value;
            Write(_scratch, 1U);
        }

        public void WriteByte(OpCode value)
        {
            _scratch[0] = (byte)value;
            Write(_scratch, 1U);
        }

        public void WriteBool(bool value)
        {
            _scratch[0] = 0;
            if (value) _scratch[0] = 1;
            Write(_scratch, 1U);
        }

        public void WriteChar(char value)
        {
            _scratch[0] = (byte)value;
            _scratch[1] = (byte)((uint)value >> 8);
            Write(_scratch, 2U);
        }

        public void WriteUInt16(ushort value)
        {
            _scratch[0] = (byte)value;
            _scratch[1] = (byte)((uint)value >> 8);
            Write(_scratch, 2U);
        }

        public void WriteUInt16(int rawValue)
        {
            ushort num = (ushort)rawValue;
            _scratch[0] = (byte)num;
            _scratch[1] = (byte)((uint)num >> 8);
            Write(_scratch, 2U);
        }

        public void WriteUInt16(uint rawValue)
        {
            ushort num = (ushort)rawValue;
            _scratch[0] = (byte)num;
            _scratch[1] = (byte)((uint)num >> 8);
            Write(_scratch, 2U);
        }

        public void WriteInt32(int value)
        {
            _scratch[0] = (byte)value;
            _scratch[1] = (byte)(value >> 8);
            _scratch[2] = (byte)(value >> 16);
            _scratch[3] = (byte)(value >> 24);
            Write(_scratch, 4U);
        }

        public void WriteUInt32(uint value)
        {
            _scratch[0] = (byte)value;
            _scratch[1] = (byte)(value >> 8);
            _scratch[2] = (byte)(value >> 16);
            _scratch[3] = (byte)(value >> 24);
            Write(_scratch, 4U);
        }

        public void WriteInt64(long value) => WriteUInt64((ulong)value);

        public void WriteUInt64(ulong value)
        {
            _scratch[0] = (byte)value;
            _scratch[1] = (byte)(value >> 8);
            _scratch[2] = (byte)(value >> 16);
            _scratch[3] = (byte)(value >> 24);
            _scratch[4] = (byte)(value >> 32);
            _scratch[5] = (byte)(value >> 40);
            _scratch[6] = (byte)(value >> 48);
            _scratch[7] = (byte)(value >> 56);
            Write(_scratch, 8U);
        }

        public unsafe void WriteSingle(float value)
        {
            uint num = *(uint*)&value;
            _scratch[0] = (byte)num;
            _scratch[1] = (byte)(num >> 8);
            _scratch[2] = (byte)(num >> 16);
            _scratch[3] = (byte)(num >> 24);
            Write(_scratch, 4U);
        }

        public unsafe void WriteDouble(double value) => WriteInt64(*(long*)&value);

        public void WriteString(string value)
        {
            if (value == null)
            {
                WriteUInt16(ushort.MaxValue);
            }
            else
            {
                if (value.Length >= short.MaxValue)
                    throw new ArgumentException("String too long");
                bool flag = false;
                foreach (char ch in value)
                {
                    if (ch > 'ÿ')
                    {
                        flag = true;
                        break;
                    }
                }
                uint length = (uint)value.Length;
                if (!flag)
                    length |= 32768U;
                WriteUInt16((ushort)length);
                foreach (char ch in value)
                {
                    if (flag)
                        WriteChar(ch);
                    else
                        WriteByte((byte)ch);
                }
            }
        }

        public void Write(ByteCodeReader value) => Write(value, 0U);

        public unsafe void Write(ByteCodeReader value, uint offset)
        {
            IntPtr intPtr = value.ToIntPtr(out uint size);
            if (offset > size)
                throw new ArgumentOutOfRangeException(nameof(offset));
            Write(new IntPtr(intPtr.ToInt64() + offset), (uint)(size - offset));
        }

        public unsafe void Write(byte[] buffer, uint count)
        {
            fixed (byte* pbData = buffer)
                Write(pbData, count);
        }

        public unsafe void Write(IntPtr buffer, uint count) => Write((byte*)buffer.ToPointer(), count);

        private unsafe void Write(byte* pbData, uint cbData)
        {
            while (cbData > 0U)
            {
                if (_cbFreeInBlock == 0U)
                {
                    _currentBlock = new byte[4096];
                    _blockList.Add(_currentBlock);
                    _cbFreeInBlock = 4096U;
                }
                uint num1 = cbData <= _cbFreeInBlock ? cbData : _cbFreeInBlock;
                uint num2 = 4096U - _cbFreeInBlock;
                Marshal.Copy(new IntPtr(pbData), _currentBlock, (int)num2, (int)num1);
                pbData += (int)num1;
                cbData -= num1;
                _cbFreeInBlock -= num1;
                _totalSize += num1;
            }
        }

        public void Overwrite(uint offset, uint value)
        {
            if (offset + 4U > _totalSize)
                throw new ArgumentException("Invalid offset");
            OverwriteByte(offset, (byte)value);
            OverwriteByte(offset + 1U, (byte)(value >> 8));
            OverwriteByte(offset + 2U, (byte)(value >> 16));
            OverwriteByte(offset + 3U, (byte)(value >> 24));
        }

        private void OverwriteByte(uint offset, byte value) => ((byte[])_blockList[(int)(offset / 4096U)])[(offset % 4096U)] = value;

        private unsafe byte* ComposeFinalBuffer(out uint totalSize)
        {
            byte* pointer = (byte*)NativeApi.MemAlloc(_totalSize, false).ToPointer();
            byte* numPtr = pointer;
            for (int index = 0; index < _blockList.Count - 1; ++index)
            {
                Marshal.Copy((byte[])_blockList[index], 0, new IntPtr(numPtr), 4096);
                numPtr += 4096;
            }
            uint num = 4096U - _cbFreeInBlock;
            if (_currentBlock != null && num != 0U)
                Marshal.Copy(_currentBlock, 0, new IntPtr(numPtr), (int)num);
            totalSize = _totalSize;
            _blockList.Clear();
            _currentBlock = null;
            _cbFreeInBlock = 0U;
            _totalSize = 0U;
            return pointer;
        }

        public unsafe ByteCodeReader CreateReader()
        {
            uint totalSize;
            return new ByteCodeReader(new IntPtr(ComposeFinalBuffer(out totalSize)), totalSize, true);
        }
    }
}
