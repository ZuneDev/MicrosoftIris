// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.OpCode
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

namespace Microsoft.Iris.Markup
{
    internal enum OpCode : byte
    {
        ConstructObject = 1,
        ConstructObjectIndirect = 2,
        ConstructObjectParam = 3,
        ConstructFromString = 4,
        ConstructFromBinary = 5,
        InitializeInstance = 6,
        InitializeInstanceIndirect = 7,
        LookupSymbol = 8,
        WriteSymbol = 9,
        WriteSymbolPeek = 10, // 0x0A
        ClearSymbol = 11, // 0x0B
        PropertyInitialize = 12, // 0x0C
        PropertyInitializeIndirect = 13, // 0x0D
        PropertyListAdd = 14, // 0x0E
        PropertyDictionaryAdd = 15, // 0x0F
        PropertyAssign = 16, // 0x10
        PropertyAssignStatic = 17, // 0x11
        PropertyGet = 18, // 0x12
        PropertyGetPeek = 19, // 0x13
        PropertyGetStatic = 20, // 0x14
        MethodInvoke = 21, // 0x15
        MethodInvokePeek = 22, // 0x16
        MethodInvokeStatic = 23, // 0x17
        MethodInvokePushLastParam = 24, // 0x18
        MethodInvokeStaticPushLastParam = 25, // 0x19
        VerifyTypeCast = 26, // 0x1A
        ConvertType = 27, // 0x1B
        Operation = 28, // 0x1C
        IsCheck = 29, // 0x1D
        As = 30, // 0x1E
        TypeOf = 31, // 0x1F
        PushNull = 32, // 0x20
        PushConstant = 33, // 0x21
        PushThis = 34, // 0x22
        DiscardValue = 35, // 0x23
        ReturnValue = 36, // 0x24
        ReturnVoid = 37, // 0x25
        JumpIfFalse = 38, // 0x26
        JumpIfFalsePeek = 39, // 0x27
        JumpIfTruePeek = 40, // 0x28
        JumpIfDictionaryContains = 41, // 0x29
        JumpIfNullPeek = 42, // 0x2A
        Jump = 43, // 0x2B
        ConstructListenerStorage = 44, // 0x2C
        Listen = 45, // 0x2D
        DestructiveListen = 46, // 0x2E
        EnterDebugState = 47, // 0x2F
    }
}
