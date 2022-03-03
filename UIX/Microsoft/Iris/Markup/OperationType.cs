// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.OperationType
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal enum OperationType
    {
        MathAdd = 1,
        MathSubtract = 2,
        MathMultiply = 3,
        MathDivide = 4,
        MathModulus = 5,
        LogicalAnd = 6,
        LogicalOr = 7,
        RelationalEquals = 8,
        RelationalNotEquals = 9,
        RelationalLessThan = 10, // 0x0000000A
        RelationalGreaterThan = 11, // 0x0000000B
        RelationalLessThanEquals = 12, // 0x0000000C
        RelationalGreaterThanEquals = 13, // 0x0000000D
        RelationalIs = 14, // 0x0000000E
        LogicalNot = 15, // 0x0000000F
        MathNegate = 16, // 0x00000010
        PostIncrement = 17, // 0x00000011
        PostDecrement = 18, // 0x00000012
    }
}
