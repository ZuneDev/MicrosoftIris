// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.ByteCodeReader
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.OS;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Markup
{
    internal class ManagedByteCodeReader : DisposableObject
    {
        private static char[] s_scratchCharArray = new char[100];
        private BinaryReader _reader;
        private IntPtr _buffer;
        private byte[] _bytes;
        private bool _inFixedMemory;

        public unsafe ManagedByteCodeReader(IntPtr buffer, long size, bool ownsAllocation)
        {
            byte[] bytes = new byte[size];
            Marshal.Copy(buffer, bytes, 0, (int)size);
            _bytes = bytes;

            _reader = new BinaryReader(new MemoryStream(_bytes));
            _buffer = buffer;
        }

        public byte[] Bytes => _bytes;

        public uint Size => (uint)_reader.BaseStream.Length;

        public uint CurrentOffset
        {
            get => (uint)_reader.BaseStream.Position;
            set => _reader.BaseStream.Seek(value <= Size ? value : throw new Exception("Invalid offset"), SeekOrigin.Begin);
        }

        public unsafe IntPtr CurrentAddress => new IntPtr(_buffer.ToInt64() + _reader.BaseStream.Position);

        public bool IsInFixedMemory => _inFixedMemory;

        public void MarkAsInFixedMemory() => _inFixedMemory = true;

        public bool ReadBool() => _reader.ReadBoolean();

        public unsafe byte ReadByte() => _reader.ReadByte();

        public char ReadChar() => _reader.ReadChar();

        public unsafe ushort ReadUInt16() => _reader.ReadUInt16();

        public int ReadInt32() => _reader.ReadInt32();

        public unsafe uint ReadUInt32() => _reader.ReadUInt32();

        public static unsafe ushort ReadUInt16(IntPtr pData)
        {
            var reader = new ByteCodeReader(pData, sizeof(ushort), false);
            return reader.ReadUInt16();
        }

        public static unsafe uint ReadUInt32(IntPtr pData)
        {
            var reader = new ByteCodeReader(pData, sizeof(uint), false);
            return reader.ReadUInt32();
        }

        public long ReadInt64() => _reader.ReadInt64();

        public unsafe ulong ReadUInt64() => _reader.ReadUInt64();

        public unsafe float ReadSingle() => _reader.ReadSingle();

        public unsafe double ReadDouble() => _reader.ReadDouble();

        public unsafe string ReadString()
        {
            uint num1 = ReadUInt16();
            if (num1 == ushort.MaxValue)
                return null;
            bool flag;
            uint num2;
            if (((int)num1 & 32768) != 0)
            {
                flag = true;
                num1 &= (uint)short.MaxValue;
                num2 = num1;
            }
            else
            {
                flag = false;
                num2 = num1 * 2U;
            }
            if (CurrentOffset + num2 > Size)
                ThrowReadError();
            char[] chArray = num1 >= s_scratchCharArray.Length ? new char[num1] : s_scratchCharArray;
            byte* numPtr1 = (byte*)(_buffer.ToInt64() + CurrentOffset);
            if (flag)
            {
                for (int index = 0; index < num1; ++index)
                    chArray[index] = (char)*numPtr1++;
            }
            else
            {
                for (int index = 0; index < num1; ++index)
                {
                    byte* numPtr2 = numPtr1;
                    byte* numPtr3 = numPtr2 + 1;
                    byte num3 = *numPtr2;
                    byte* numPtr4 = numPtr3;
                    numPtr1 = numPtr4 + 1;
                    byte num4 = *numPtr4;
                    chArray[index] = (char)(num3 | (uint)num4 << 8);
                }
            }
            _reader.BaseStream.Seek(num2, SeekOrigin.Current);
            return new string(chArray, 0, (int)num1);
        }

        public unsafe IntPtr ToIntPtr(out long size)
        {
            size = Size;
            return _buffer;
        }

        public unsafe IntPtr GetAddress(uint offset) => new IntPtr(_buffer.ToInt64() + offset);

        private void ThrowReadError() => throw new Exception("Attempted to read past the end of the buffer.");

        protected override unsafe void OnDispose()
        {
            base.OnDispose();
            _reader.Close();
            _buffer = IntPtr.Zero;
        }
    }

    public class ByteCodeReader : DisposableObject
    {
        private static char[] s_scratchCharArray = new char[100];
        private unsafe byte* _buffer;
        private uint _offset;
        private uint _size;
        private bool _ownsAllocation;
        private bool _inFixedMemory;

        public unsafe ByteCodeReader(IntPtr buffer, uint size, bool ownsAllocation)
        {
            _buffer = (byte*)buffer.ToPointer();
            _size = size;
            _offset = 0U;
            _ownsAllocation = ownsAllocation;
            int num = _ownsAllocation ? 1 : 0;
        }

        public uint Size => _size;

        public uint CurrentOffset
        {
            get => _offset;
            set => _offset = value <= _size ? value : throw new Exception("Invalid offset");
        }

        public unsafe IntPtr CurrentAddress => new IntPtr(_buffer + (int)_offset);

        public void MarkAsInFixedMemory() => _inFixedMemory = true;

        public bool IsInFixedMemory => _inFixedMemory;

        public bool ReadBool() => ReadByte() != 0;

        public unsafe byte ReadByte()
        {
            if (_offset >= _size)
                ThrowReadError();
            byte num = _buffer[(int)_offset];
            ++_offset;
            return num;
        }

        public char ReadChar() => (char)ReadUInt16();

        public unsafe ushort ReadUInt16()
        {
            if (_offset + 2U > _size)
                ThrowReadError();
            byte* numPtr = _buffer + (int)_offset;
            byte num1 = *numPtr;
            byte num2 = numPtr[1];
            _offset += 2U;
            return (ushort)(num1 | (uint)num2 << 8);
        }

        public int ReadInt32() => (int)ReadUInt32();

        public unsafe uint ReadUInt32()
        {
            if (_offset + 4U > _size)
                ThrowReadError();
            byte* numPtr = _buffer + (int)_offset;
            byte num1 = *numPtr;
            byte num2 = numPtr[1];
            byte num3 = numPtr[2];
            byte num4 = numPtr[3];
            _offset += 4U;
            return (uint)(num1 | num2 << 8 | num3 << 16 | num4 << 24);
        }

        public static unsafe ushort ReadUInt16(IntPtr pData)
        {
            byte* pointer = (byte*)pData.ToPointer();
            return (ushort)(*pointer | (uint)pointer[1] << 8);
        }

        public static unsafe uint ReadUInt32(IntPtr pData)
        {
            byte* pointer = (byte*)pData.ToPointer();
            return (uint)(*pointer | pointer[1] << 8 | pointer[2] << 16 | pointer[3] << 24);
        }

        public long ReadInt64() => (long)ReadUInt64();

        public unsafe ulong ReadUInt64()
        {
            if (_offset + 8U > _size)
                ThrowReadError();
            byte* numPtr = _buffer + (int)_offset;
            byte num1 = *numPtr;
            byte num2 = numPtr[1];
            byte num3 = numPtr[2];
            byte num4 = numPtr[3];
            byte num5 = numPtr[4];
            byte num6 = numPtr[5];
            byte num7 = numPtr[6];
            byte num8 = numPtr[7];
            _offset += 8U;
            uint num9 = (uint)(num1 | num2 << 8 | num3 << 16 | num4 << 24);
            return (ulong)(uint)(num5 | num6 << 8 | num7 << 16 | num8 << 24) << 32 | num9;
        }

        //public unsafe float ReadSingle() => *(float*)&ReadUInt32();
        public unsafe float ReadSingle() => BitConverter.ToSingle(BitConverter.GetBytes(ReadUInt32()), 0);

        //public unsafe double ReadDouble() => *(double*)&ReadInt64();
        public unsafe double ReadDouble() => BitConverter.ToSingle(BitConverter.GetBytes(ReadUInt64()), 0);

        public unsafe string ReadString()
        {
            uint num1 = ReadUInt16();
            if (num1 == ushort.MaxValue)
                return null;
            bool flag;
            uint num2;
            if (((int)num1 & 32768) != 0)
            {
                flag = true;
                num1 &= (uint)short.MaxValue;
                num2 = num1;
            }
            else
            {
                flag = false;
                num2 = num1 * 2U;
            }
            if (_offset + num2 > _size)
                ThrowReadError();
            char[] chArray = num1 >= s_scratchCharArray.Length ? new char[num1] : ByteCodeReader.s_scratchCharArray;
            byte* numPtr1 = _buffer + (int)_offset;
            if (flag)
            {
                for (int index = 0; index < num1; ++index)
                    chArray[index] = (char)*numPtr1++;
            }
            else
            {
                for (int index = 0; index < num1; ++index)
                {
                    byte* numPtr2 = numPtr1;
                    byte* numPtr3 = numPtr2 + 1;
                    byte num3 = *numPtr2;
                    byte* numPtr4 = numPtr3;
                    numPtr1 = numPtr4 + 1;
                    byte num4 = *numPtr4;
                    chArray[index] = (char)(num3 | (uint)num4 << 8);
                }
            }
            _offset += num2;
            return new string(chArray, 0, (int)num1);
        }

        public unsafe IntPtr ToIntPtr(out uint size)
        {
            size = _size;
            return new IntPtr(_buffer);
        }

        public unsafe IntPtr GetAddress(uint offset) => new IntPtr(_buffer + (int)offset);

        private void ThrowReadError() => throw new Exception("Attempted to read past the end of the buffer.");

        protected override unsafe void OnDispose()
        {
            base.OnDispose();
            if (_ownsAllocation)
                NativeApi.MemFree(new IntPtr(_buffer));
            _buffer = null;
        }
    }
}
