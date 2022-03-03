// Decompiled with JetBrains decompiler
// Type: ParserLexClass
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using SSVParseLib;

internal class ParserLexClass : SSLex
{
    public const int ParserLexExprCodeBlock = 0;
    public const int ParserLexExprCodeMLComment = 1;
    public const int ParserLexExprCodeSLComment = 2;
    public const int ParserLexTokenIdentifier = 54;
    public const int ParserLexTokenString = 55;
    public const int ParserLexTokenStringLiteral = 56;
    public const int ParserLexTokenInteger = 57;
    public const int ParserLexTokenLongInteger = 58;
    public const int ParserLexTokenFloat = 59;
    public const int ParserLexTokenTrue = 60;
    public const int ParserLexTokenFalse = 61;
    public const int ParserLexTokenBase = 62;
    public const int ParserLexTokenThis = 63;
    public const int ParserLexTokenReturn = 64;
    public const int ParserLexTokenIf = 65;
    public const int ParserLexTokenElse = 66;
    public const int ParserLexTokenFor = 67;
    public const int ParserLexTokenForEach = 68;
    public const int ParserLexTokenWhile = 69;
    public const int ParserLexTokenDo = 70;
    public const int ParserLexTokenBreak = 71;
    public const int ParserLexTokenContinue = 72;
    public const int ParserLexTokenIn = 73;
    public const int ParserLexTokenNull = 74;
    public const int ParserLexTokenNew = 75;
    public const int ParserLexTokenIs = 76;
    public const int ParserLexTokenTypeOf = 77;
    public const int ParserLexTokenAs = 78;
    public const int ParserLexTokenVirtual = 79;
    public const int ParserLexTokenOverride = 80;
    public const int ParserLexTokenParenOpen = 81;
    public const int ParserLexTokenParenClose = 82;
    public const int ParserLexTokenCurleyOpen = 83;
    public const int ParserLexTokenCurleyClose = 84;
    public const int ParserLexTokenSquareOpen = 85;
    public const int ParserLexTokenSquareClose = 86;
    public const int ParserLexTokenHash = 87;
    public const int ParserLexTokenPeriod = 88;
    public const int ParserLexTokenComma = 89;
    public const int ParserLexTokenColon = 90;
    public const int ParserLexTokenSemiColon = 91;
    public const int ParserLexTokenEquals = 92;
    public const int ParserLexTokenPlus = 93;
    public const int ParserLexTokenMinus = 94;
    public const int ParserLexTokenStar = 95;
    public const int ParserLexTokenSlash = 96;
    public const int ParserLexTokenPercent = 97;
    public const int ParserLexTokenAmpersand = 98;
    public const int ParserLexTokenPipe = 99;
    public const int ParserLexTokenDoubleEquals = 100;
    public const int ParserLexTokenBangEquals = 101;
    public const int ParserLexTokenLessThan = 102;
    public const int ParserLexTokenGreaterThan = 103;
    public const int ParserLexTokenLessThanEqual = 104;
    public const int ParserLexTokenGreaterThanEqual = 105;
    public const int ParserLexTokenAmpAmp = 106;
    public const int ParserLexTokenPipePipe = 107;
    public const int ParserLexTokenBang = 108;
    public const int ParserLexTokenPlusPlus = 109;
    public const int ParserLexTokenMinusMinus = 110;
    public const int ParserLexTokenQuestionMark = 111;
    public const int ParserLexTokenQuestionQuestion = 112;
    public const int ParserLexTokenColonSpace = 113;
    public const int ParserLexTokenMethodDisambiguator = 114;
    public const int ParserLexTokenExpressionDisambiguator = 115;
    public const int ParserLexTokenCodeBlockDisambiguator = 116;

    public ParserLexClass(SSLexTable q_table)
      : base(q_table, null)
    {
    }

    public ParserLexClass(SSLexTable q_table, SSLexConsumer q_consumer)
      : base(q_table, q_consumer)
    {
    }
}
