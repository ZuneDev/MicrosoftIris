// Decompiled with JetBrains decompiler
// Type: ParserLexClass
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using SSVParseLib;

internal class ParserLexClass : SSLex
{
    public ParserLexClass(SSLexTable q_table)
      : base(q_table, null)
    {
    }

    public ParserLexClass(SSLexTable q_table, SSLexConsumer q_consumer)
      : base(q_table, q_consumer)
    {
    }
}

internal enum ParserLexExpr
{
    CodeBlock = 0,
    CodeMLComment = 1,
    CodeSLComment = 2,
}

internal enum ParserLexToken
{
    Identifier = 54,
    String = 55,
    StringLiteral = 56,
    Integer = 57,
    LongInteger = 58,
    Float = 59,
    True = 60,
    False = 61,
    Base = 62,
    This = 63,
    Return = 64,
    If = 65,
    Else = 66,
    For = 67,
    ForEach = 68,
    While = 69,
    Do = 70,
    Break = 71,
    Continue = 72,
    In = 73,
    Null = 74,
    New = 75,
    Is = 76,
    TypeOf = 77,
    As = 78,
    Virtual = 79,
    Override = 80,
    ParenOpen = 81,
    ParenClose = 82,
    CurleyOpen = 83,
    CurleyClose = 84,
    SquareOpen = 85,
    SquareClose = 86,
    Hash = 87,
    Period = 88,
    Comma = 89,
    Colon = 90,
    SemiColon = 91,
    Equals = 92,
    Plus = 93,
    Minus = 94,
    Star = 95,
    Slash = 96,
    Percent = 97,
    Ampersand = 98,
    Pipe = 99,
    DoubleEquals = 100,
    BangEquals = 101,
    LessThan = 102,
    GreaterThan = 103,
    LessThanEqual = 104,
    GreaterThanEqual = 105,
    AmpAmp = 106,
    PipePipe = 107,
    Bang = 108,
    PlusPlus = 109,
    MinusMinus = 110,
    QuestionMark = 111,
    QuestionQuestion = 112,
    ColonSpace = 113,
    MethodDisambiguator = 114,
    ExpressionDisambiguator = 115,
    CodeBlockDisambiguator = 116,
}
