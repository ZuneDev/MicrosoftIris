// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.UIXVariant
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using System;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal struct UIXVariant
    {
        private UIXVariant.VariantType _type;
        private long _union;

        public static object GetValue(UIXVariant inboundObject, LoadResult context)
        {
            switch (inboundObject._type)
            {
                case VariantType.Empty:
                    return null;
                case VariantType.Bool:
                    return inboundObject._integer != 0L;
                case VariantType.Byte:
                    return (byte)inboundObject._integer;
                case VariantType.Int32:
                    return (int)inboundObject._integer;
                case VariantType.Int64:
                    return inboundObject._integer;
                case VariantType.Single:
                    return inboundObject._float;
                case VariantType.Double:
                    return inboundObject._double;
                case VariantType.Enum:
                    return GetEnumValue(inboundObject._enum._value, inboundObject._enum._type);
                case VariantType.UIXObject:
                    return GetObjectValue(inboundObject._pointer, context);
                case VariantType.UIXString:
                    return DllProxyServices.GetString(inboundObject._pointer);
                case VariantType.UIXImage:
                    return DllProxyServices.GetImage(inboundObject._pointer);
                case VariantType.UIXDataQuery:
                    return DllProxyServices.GetDataQuery(inboundObject._pointer);
                case VariantType.UIXDataType:
                    return DllProxyServices.GetDataType(inboundObject._pointer);
                default:
                    return null;
            }
        }

        private static object GetEnumValue(int enumValue, uint typeID) => DllLoadResult.MapType(typeID) is DllEnumSchema dllEnumSchema ? dllEnumSchema.GetBoxedValue(enumValue) : null;

        private static object GetObjectValue(IntPtr value, LoadResult context)
        {
            object obj = null;
            DllLoadResult.PushContext(context);
            if (value != IntPtr.Zero)
                obj = DllProxyObject.Wrap(value);
            DllLoadResult.PopContext();
            return obj;
        }

        public static unsafe void MarshalObjectArray(object[] objects, UIXVariant* destination)
        {
            for (int index = 0; index < objects.Length; ++index)
                MarshalObject(objects[index], destination + index);
        }

        public static unsafe void MarshalObject(object o, UIXVariant* destination)
        {
            switch (o)
            {
                case null:
                    destination->SetToNull();
                    break;
                case bool flag:
                    destination->SetIntegerValue(flag ? 1L : 0L, VariantType.Bool);
                    break;
                case byte num:
                    destination->SetIntegerValue(num, VariantType.Byte);
                    break;
                case int num:
                    destination->SetIntegerValue(num, VariantType.Int32);
                    break;
                case long num:
                    destination->SetIntegerValue(num, VariantType.Int64);
                    break;
                case float num:
                    destination->SetFloatValue(num, VariantType.Single);
                    break;
                case double num:
                    destination->SetDoubleValue(num, VariantType.Double);
                    break;
                case string _:
                    IntPtr nativeObject1;
                    DllProxyServices.CreateNativeString((string)o, out nativeObject1);
                    destination->SetPointerValue(nativeObject1, VariantType.UIXString);
                    break;
                case DllProxyObject _:
                    destination->SetPointerValue(((DllProxyObject)o).NativeObject, VariantType.UIXObject);
                    NativeApi.SpAddRefExternalObject(destination->_pointer);
                    break;
                case DllEnumProxy _:
                    DllEnumProxy dllEnumProxy = (DllEnumProxy)o;
                    UIXVariant.EnumValue enumValue;
                    enumValue._value = dllEnumProxy.Value;
                    enumValue._type = dllEnumProxy.Type.ID;
                    destination->SetEnumValue(enumValue);
                    break;
                case UIImage _:
                    IntPtr nativeObject2;
                    DllProxyServices.CreateNativeImage((UIImage)o, out nativeObject2);
                    destination->SetPointerValue(nativeObject2, VariantType.UIXImage);
                    break;
                case MarkupDataQuery _:
                    MarkupDataQuery markupDataQuery = (MarkupDataQuery)o;
                    destination->SetPointerValue(markupDataQuery.ExternalNativeObject, VariantType.UIXDataQuery);
                    break;
                case MarkupDataType _:
                    MarkupDataType markupDataType = (MarkupDataType)o;
                    destination->SetPointerValue(markupDataType.ExternalNativeObject, VariantType.UIXDataType);
                    break;
            }
        }

        public static unsafe void CleanupMarshalledObjects(UIXVariant* source, int count)
        {
            for (int index = 0; index < count; ++index)
                CleanupMarshalledObject(source + index);
        }

        public static unsafe void CleanupMarshalledObject(UIXVariant* source)
        {
            if (source->_type != VariantType.UIXObject || !(source->_pointer != IntPtr.Zero))
                return;
            NativeApi.SpReleaseExternalObject(source->_pointer);
        }

        private void SetToNull()
        {
            _pointer = IntPtr.Zero;
            _type = VariantType.UIXObject;
        }

        private void SetPointerValue(IntPtr value, UIXVariant.VariantType type)
        {
            _pointer = value;
            _type = type;
        }

        private void SetIntegerValue(long value, UIXVariant.VariantType type)
        {
            _integer = value;
            _type = type;
        }

        private void SetFloatValue(float value, UIXVariant.VariantType type)
        {
            _float = value;
            _type = type;
        }

        private void SetDoubleValue(double value, UIXVariant.VariantType type)
        {
            _double = value;
            _type = type;
        }

        private void SetEnumValue(UIXVariant.EnumValue value)
        {
            _enum = value;
            _type = VariantType.Enum;
        }

        private unsafe IntPtr _pointer
        {
            get
            {
                var value = _union;
                return *(IntPtr*)&value;
            }
            set
            {
                long num;
                *(IntPtr*)&num = value;
                _union = num;
            }
        }

        private unsafe UIXVariant.EnumValue _enum
        {
            get
            {
                var value = _union;
                return *(UIXVariant.EnumValue*)&value;
            }
            set
            {
                long num;
                *(UIXVariant.EnumValue*)&num = value;
                _union = num;
            }
        }

        private long _integer
        {
            get => _union;
            set => _union = value;
        }

        private unsafe float _float
        {
            get
            {
                var value = _union;
                return *(float*)&value;
            }
            set
            {
                long num;
                *(float*)&num = value;
                _union = num;
            }
        }

        private unsafe double _double
        {
            get
            {
                var value = _union;
                return *(double*)&value;
            }
            set
            {
                long num;
                *(double*)&num = value;
                _union = num;
            }
        }

        internal enum VariantType
        {
            Empty = 0,
            Bool = 1,
            Byte = 2,
            Int32 = 3,
            Int64 = 4,
            Single = 5,
            Double = 6,
            Enum = 7,
            UIXObject = 128, // 0x00000080
            UIXString = 129, // 0x00000081
            UIXImage = 130, // 0x00000082
            UIXDataQuery = 131, // 0x00000083
            UIXDataType = 132, // 0x00000084
        }

        private struct EnumValue
        {
            public int _value;
            public uint _type;
        }
    }
}
