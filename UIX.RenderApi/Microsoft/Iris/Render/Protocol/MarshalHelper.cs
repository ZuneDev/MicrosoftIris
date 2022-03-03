// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.MarshalHelper
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Iris.Render.Protocol
{
    internal class MarshalHelper
    {
        private static RENDERHANDLE[] s_EmptyRenderHandleArray = new RENDERHANDLE[0];

        public static unsafe void Decode(
          RenderPort port,
          Message* pmsg,
          BLOBREF refParam,
          out string param)
        {
            uint uint32 = BLOBREF.ToUInt32(refParam);
            uint num1 = uint32 & ushort.MaxValue;
            uint num2 = (uint32 & 4294901760U) >> 16;
            byte* numPtr = (byte*)((int)pmsg + (int)num1);
            int length = (int)num2 - 2;
            byte[] numArray = new byte[length];
            Marshal.Copy(new IntPtr(numPtr), numArray, 0, length);
            Encoding encoding = Encoding.Unicode;
            if (port.AlwaysBigEndian)
                encoding = Encoding.BigEndianUnicode;
            param = encoding.GetString(numArray);
        }

        public static unsafe void Decode(
          RenderPort port,
          Message* pmsgOuter,
          BLOBREF refMsgInner,
          out Message* pmsgInner)
        {
            byte* numPtr = (byte*)pmsgOuter;
            uint num = ushort.MaxValue & BLOBREF.ToUInt32(refMsgInner);
            pmsgInner = (Message*)(numPtr + (int)num);
        }

        private static ushort[] ComputeByteOrderMap(Type targetType, bool fCompact)
        {
            ushort[] fieldMap = new ushort[10];
            ComputeByteOrderMapWorker(ref fieldMap, targetType, 0);
            if (fCompact)
            {
                int length = fieldMap[0] + 1;
                if (fieldMap.Length > length)
                {
                    ushort[] numArray = new ushort[length];
                    Array.Copy(fieldMap, numArray, length);
                    fieldMap = numArray;
                }
            }
            return fieldMap;
        }

        public static unsafe void SwapByteOrder(
          byte* pMem,
          ref ushort[] fieldMapByRef,
          Type type,
          int startOffset,
          int stopOffset)
        {
            if (fieldMapByRef == null)
                fieldMapByRef = ComputeByteOrderMap(type, true);
            ushort[] numArray = fieldMapByRef;
            if (stopOffset <= 0)
                stopOffset = int.MaxValue;
            int num1 = numArray[0] + 1;
            for (int index1 = 1; index1 < num1; ++index1)
            {
                ushort num2 = numArray[index1];
                int num3 = num2 >> 2;
                if (num3 >= startOffset)
                {
                    if (num3 >= stopOffset)
                        break;
                    int num4 = 1 << (num2 & 3);
                    byte* numPtr = pMem + num3;
                    int num5 = num4 / 2;
                    for (int index2 = 0; index2 < num5; ++index2)
                    {
                        int index3 = num4 - (index2 + 1);
                        byte num6 = numPtr[index2];
                        numPtr[index2] = numPtr[index3];
                        numPtr[index3] = num6;
                    }
                }
            }
        }

        public static object MakeDumpable(string value) => value;

        public static object MakeDumpable(object value) => value;

        public static unsafe object MakeDumpable(void* value) => new IntPtr(value);

        private static void ComputeByteOrderMapWorker(
          ref ushort[] fieldMap,
          Type targetType,
          int parentOffset)
        {
            if (targetType.IsPrimitive)
                return;
            int packingForType = GetPackingForType(targetType);
            int num1 = 0;
            foreach (FieldInfo field in targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                Type type = field.FieldType;
                if (type.IsEnum)
                    type = Enum.GetUnderlyingType(type);
                int size = Marshal.SizeOf(type);
                int num2 = size;
                if (type.IsPrimitive)
                {
                    if (num2 > packingForType)
                        num2 = packingForType;
                }
                else if (num2 > 4)
                    num2 = 4;
                int num3 = num2 - 1;
                int num4 = num1 + num3 & ~num3;
                int num5 = num4 + parentOffset;
                if (type.IsPrimitive)
                    AddByteOrderEntry(ref fieldMap, num5, size);
                else
                    ComputeByteOrderMapWorker(ref fieldMap, type, num5);
                num1 = num4 + size;
            }
        }

        private static int GetPackingForType(Type t) => 8;

        private static void AddByteOrderEntry(ref ushort[] fieldMap, int offset, int size)
        {
            ushort num;
            switch (size)
            {
                case 1:
                    return;
                case 2:
                    num = 1;
                    break;
                case 3:
                    return;
                case 4:
                    num = 2;
                    break;
                case 8:
                    num = 3;
                    break;
                default:
                    return;
            }
            int index = fieldMap[0] + 1;
            if (index >= fieldMap.Length)
            {
                ushort[] numArray = new ushort[2 * fieldMap.Length];
                Array.Copy(fieldMap, numArray, fieldMap.Length);
                fieldMap = numArray;
            }
            fieldMap[0] = (ushort)index;
            fieldMap[index] = (ushort)((uint)(offset << 2) | num);
        }

        public static RENDERHANDLE[] CreateRenderHandleArray(int length) => length == 0 ? s_EmptyRenderHandleArray : new RENDERHANDLE[length];
    }
}
