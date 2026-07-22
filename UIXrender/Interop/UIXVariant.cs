using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Interop;

// Bit-for-bit mirror of Microsoft.Iris.CodeModel.Cpp.UIXVariant
// (UIX/Microsoft/Iris/CodeModel/Cpp/UIXVariant.cs) -- a tagged union: a 4-byte type tag
// followed by an 8-byte payload reinterpreted depending on the tag. Managed-side helpers
// (GetValue/SetXxxValue) are reimplemented here as real logic rather than mirrored
// method-for-method, since this side never needs to talk to the CLR's own marshaller --
// it just needs the same wire layout and equivalent behavior.
public enum VariantType
{
    Empty = 0,
    Bool = 1,
    Byte = 2,
    Int32 = 3,
    Int64 = 4,
    Single = 5,
    Double = 6,
    Enum = 7,
    UIXObject = 128,
    UIXString = 129,
    UIXImage = 130,
    UIXDataQuery = 131,
    UIXDataType = 132,
}

[StructLayout(LayoutKind.Sequential)]
public struct EnumValue
{
    public int value;
    public uint type;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct UIXVariant
{
    public VariantType type;
    public long union;

    public static UIXVariant Empty => new() { type = VariantType.Empty };

    public IntPtr AsPointer
    {
        get => (IntPtr)union;
        set => union = (long)value;
    }

    public bool AsBool
    {
        get => union != 0;
        set => union = value ? 1 : 0;
    }

    public byte AsByte
    {
        get => (byte)union;
        set => union = value;
    }

    public int AsInt32
    {
        get => (int)union;
        set => union = value;
    }

    public long AsInt64
    {
        get => union;
        set => union = value;
    }

    public float AsSingle
    {
        get { long u = union; return *(float*)&u; }
        set { float f = value; union = *(int*)&f; }
    }

    public double AsDouble
    {
        get { long u = union; return *(double*)&u; }
        set => union = BitConverter.DoubleToInt64Bits(value);
    }

    public EnumValue AsEnum
    {
        get { long u = union; return *(EnumValue*)&u; }
        set { long u = 0; *(EnumValue*)&u = value; union = u; }
    }

    public static UIXVariant FromObject(object value) => value switch
    {
        null => Empty,
        bool b => new UIXVariant { type = VariantType.Bool, AsBool = b },
        byte b => new UIXVariant { type = VariantType.Byte, AsByte = b },
        int i => new UIXVariant { type = VariantType.Int32, AsInt32 = i },
        long l => new UIXVariant { type = VariantType.Int64, AsInt64 = l },
        float f => new UIXVariant { type = VariantType.Single, AsSingle = f },
        double d => new UIXVariant { type = VariantType.Double, AsDouble = d },
        _ => throw new NotSupportedException($"Cannot convert {value.GetType()} to UIXVariant"),
    };

    public object ToObject() => type switch
    {
        VariantType.Empty => null,
        VariantType.Bool => AsBool,
        VariantType.Byte => AsByte,
        VariantType.Int32 => AsInt32,
        VariantType.Int64 => AsInt64,
        VariantType.Single => AsSingle,
        VariantType.Double => AsDouble,
        _ => throw new NotSupportedException($"Cannot convert UIXVariant of type {type} to object"),
    };
}
