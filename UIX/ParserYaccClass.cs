// Decompiled with JetBrains decompiler
// Type: ParserYaccClass
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.Markup.Validation;
using SSVParseLib;

internal class ParserYaccClass : SSYacc
{
    public const int ParserYaccProdStartCodeBlock = 1;
    public const int ParserYaccProdStartInlineExpression = 2;
    public const int ParserYaccProdStartMethods = 3;
    public const int ParserYaccProdMethodsEmpty = 4;
    public const int ParserYaccProdMethods = 5;
    public const int ParserYaccProdMethodSpecifiersEmpty = 6;
    public const int ParserYaccProdMethodSpecifiers = 7;
    public const int ParserYaccProdMethodSpecifierVirtual = 8;
    public const int ParserYaccProdMethodSpecifierOverride = 9;
    public const int ParserYaccProdMethod = 10;
    public const int ParserYaccProdParameterDefinitionsEmpty = 11;
    public const int ParserYaccProdParameterDefinitions = 12;
    public const int ParserYaccProdParameterDefinitionListSingle = 13;
    public const int ParserYaccProdParameterDefinitionListMulti = 14;
    public const int ParserYaccProdParameterDefinition = 15;
    public const int ParserYaccProdExpressionListEmpty = 16;
    public const int ParserYaccProdExpressionList = 17;
    public const int ParserYaccProdExpressionsSingle = 18;
    public const int ParserYaccProdExpressionsMulti = 19;
    public const int ParserYaccProdStatementLocalDeclaration = 20;
    public const int ParserYaccProdStatementLocalAssignment = 21;
    public const int ParserYaccProdForInitializerDecl = 22;
    public const int ParserYaccProdForInitializerExprList = 23;
    public const int ParserYaccProdStatementIf = 24;
    public const int ParserYaccProdStatementIfElse = 25;
    public const int ParserYaccProdStatementForEach = 26;
    public const int ParserYaccProdStatementWhile = 27;
    public const int ParserYaccProdStatementDoWhile = 28;
    public const int ParserYaccProdStatementFor = 29;
    public const int ParserYaccProdStatementDecl = 30;
    public const int ParserYaccProdStatementExpr = 31;
    public const int ParserYaccProdStatementReturn = 32;
    public const int ParserYaccProdStatementReturnExpression = 33;
    public const int ParserYaccProdStatementBreak = 34;
    public const int ParserYaccProdStatementContinue = 35;
    public const int ParserYaccProdStatementAttribute = 36;
    public const int ParserYaccProdStatementCompound = 37;
    public const int ParserYaccProdStatementsMulti = 38;
    public const int ParserYaccProdStatementsEmpty = 39;
    public const int ParserYaccProdExpressionCallMethod = 40;
    public const int ParserYaccProdExpressionCallThisMethod = 41;
    public const int ParserYaccProdExpressionCallProperty = 42;
    public const int ParserYaccProdExpressionCallStaticProperty = 43;
    public const int ParserYaccProdExpressionCallStaticMethod = 44;
    public const int ParserYaccProdExpressionIndex = 45;
    public const int ParserYaccProdExpressionNew = 46;
    public const int ParserYaccProdExpressionSymbol = 47;
    public const int ParserYaccProdExpressionString = 48;
    public const int ParserYaccProdExpressionStringLiteral = 49;
    public const int ParserYaccProdExpressionInteger = 50;
    public const int ParserYaccProdExpressionLongInteger = 51;
    public const int ParserYaccProdExpressionFloat = 52;
    public const int ParserYaccProdExpressionTrue = 53;
    public const int ParserYaccProdExpressionFalse = 54;
    public const int ParserYaccProdExpressionNull = 55;
    public const int ParserYaccProdExpressionThis = 56;
    public const int ParserYaccProdExpressionBase = 57;
    public const int ParserYaccProdExpressionTypeOf = 58;
    public const int ParserYaccProdExpressionGroup = 59;
    public const int ParserYaccProdExpressionDeclareTrigger = 60;
    public const int ParserYaccProdExpressionUnaryOperationLogicalNot = 61;
    public const int ParserYaccProdExpressionUnaryMinus = 62;
    public const int ParserYaccProdExpressionPostIncrement = 63;
    public const int ParserYaccProdExpressionPostDecrement = 64;
    public const int ParserYaccProdExpressionCastPrefixed = 65;
    public const int ParserYaccProdExpressionCast = 66;
    public const int ParserYaccProdExpressionOperationMathMultiply = 67;
    public const int ParserYaccProdExpressionOperationMathDivide = 68;
    public const int ParserYaccProdExpressionOperationMathModulus = 69;
    public const int ParserYaccProdExpressionOperationMathAdd = 70;
    public const int ParserYaccProdExpressionOperationMathSubtract = 71;
    public const int ParserYaccProdExpressionOperationRelationalLessThan = 72;
    public const int ParserYaccProdExpressionOperationRelationalGreaterThan = 73;
    public const int ParserYaccProdExpressionOperationRelationalLessThanEquals = 74;
    public const int ParserYaccProdExpressionOperationRelationalGreaterThanEquals = 75;
    public const int ParserYaccProdExpressionOperationIs = 76;
    public const int ParserYaccProdExpressionOperationAs = 77;
    public const int ParserYaccProdExpressionOperationRelationalEquals = 78;
    public const int ParserYaccProdExpressionOperationRelationalNotEquals = 79;
    public const int ParserYaccProdExpressionOperationLogicalAnd = 80;
    public const int ParserYaccProdExpressionOperationLogicalOr = 81;
    public const int ParserYaccProdExpressionNullCoalescing = 82;
    public const int ParserYaccProdExpressionTernary = 83;
    public const int ParserYaccProdExpressionAssignment = 84;
    public const int ParserYaccProdParametersEmpty = 85;
    public const int ParserYaccProdParameters = 86;
    public const int ParserYaccProdParameterListSingle = 87;
    public const int ParserYaccProdParameterListMulti = 88;
    public const int ParserYaccProdTypeIdentifierNamespaced = 89;
    public const int ParserYaccProdTypeIdentifier = 90;

    public ParserYaccClass(SSYaccTable q_table, SSLex q_lex)
      : base(q_table, q_lex)
    {
    }

    public override SSYaccStackElement reduce(int q_prod, int q_size)
    {
        switch (q_prod)
        {
            case 1:
                return ReturnObject(new ValidateCode(Owner, new ValidateStatementCompound(Owner, (ValidateStatement)FromProduction(1), CurrentLine, CurrentColumn), CurrentLine, CurrentColumn));
            case 2:
                return ReturnObject(FromProduction(2));
            case 3:
                return ReturnObject(FromProduction(1));
            case 4:
                return ReturnObject(new ValidateMethodList(Owner, CurrentLine, CurrentColumn));
            case 5:
                ValidateMethodList validateMethodList = (ValidateMethodList)FromProduction(0);
                ValidateMethod expression1 = (ValidateMethod)FromProduction(1);
                validateMethodList.AppendToEnd(expression1);
                return ReturnObject(validateMethodList);
            case 6:
                return ReturnObject(new Vector<MethodSpecifier>());
            case 7:
                Vector<MethodSpecifier> vector = (Vector<MethodSpecifier>)FromProduction(0);
                MethodSpecifier methodSpecifier = (MethodSpecifier)FromProduction(1);
                vector.Add(methodSpecifier);
                return ReturnObject(vector);
            case 8:
                return ReturnObject(MethodSpecifier.Virtual);
            case 9:
                return ReturnObject(MethodSpecifier.Override);
            case 10:
                return ReturnObject(ConstructValidateMethod(true));
            case 11:
                return ReturnObject(new ValidateParameterDefinitionList(Owner, CurrentLine, CurrentColumn));
            case 12:
                return ReturnObject((ValidateParameterDefinitionList)FromProduction(0));
            case 13:
                ValidateParameterDefinition expression2 = (ValidateParameterDefinition)FromProduction(0);
                ValidateParameterDefinitionList parameterDefinitionList1 = new ValidateParameterDefinitionList(Owner, expression2.Line, expression2.Column);
                parameterDefinitionList1.AppendToEnd(expression2);
                return ReturnObject(parameterDefinitionList1);
            case 14:
                ValidateParameterDefinitionList parameterDefinitionList2 = (ValidateParameterDefinitionList)FromProduction(0);
                ValidateParameterDefinition expression3 = (ValidateParameterDefinition)FromProduction(2);
                parameterDefinitionList2.AppendToEnd(expression3);
                return ReturnObject(parameterDefinitionList2);
            case 15:
                ValidateTypeIdentifier typeIdentifier1 = (ValidateTypeIdentifier)FromProduction(0);
                string name = FromTerminal(1);
                return ReturnObject(new ValidateParameterDefinition(Owner, Line(1), Column(1), name, typeIdentifier1));
            case 16:
                return ReturnObject(new ValidateExpressionList(Owner, CurrentLine, CurrentColumn));
            case 17:
                return ReturnObject(FromProduction(0));
            case 18:
                ValidateExpression expression4 = (ValidateExpression)FromProduction(0);
                ValidateExpressionList validateExpressionList1 = new ValidateExpressionList(Owner, expression4.Line, expression4.Column);
                validateExpressionList1.AppendToEnd(expression4);
                return ReturnObject(validateExpressionList1);
            case 19:
                ValidateExpressionList validateExpressionList2 = (ValidateExpressionList)FromProduction(0);
                ValidateExpression expression5 = (ValidateExpression)FromProduction(2);
                validateExpressionList2.AppendToEnd(expression5);
                return ReturnObject(validateExpressionList2);
            case 20:
                ValidateTypeIdentifier typeIdentifier2 = (ValidateTypeIdentifier)FromProduction(0);
                return ReturnObject(new ValidateStatementScopedLocal(Owner, FromTerminal(1), typeIdentifier2, Line(1), Column(1)));
            case 21:
                ValidateTypeIdentifier typeIdentifier3 = (ValidateTypeIdentifier)FromProduction(0);
                string str = FromTerminal(1);
                return ReturnObject(new ValidateStatementAssignment(Owner, new ValidateStatementScopedLocal(Owner, str, typeIdentifier3, Line(1), Column(1)), new ValidateExpressionSymbol(Owner, str, Line(1), Column(1)), (ValidateExpression)FromProduction(3), Line(1), Column(1)));
            case 22:
                return ReturnObject(FromProduction(0));
            case 23:
                ValidateExpressionList validateExpressionList3 = (ValidateExpressionList)FromProduction(0);
                return ReturnObject(new ValidateStatementExpression(Owner, validateExpressionList3, validateExpressionList3.Line, validateExpressionList3.Column));
            case 24:
                return ReturnObject(new ValidateStatementIf(Owner, (ValidateExpression)FromProduction(2), ValidateStatementCompound.Encapsulate((ValidateStatement)FromProduction(4)), Line(0), Column(0)));
            case 25:
                ValidateExpression condition1 = (ValidateExpression)FromProduction(2);
                ValidateStatement statement1 = (ValidateStatement)FromProduction(4);
                ValidateStatement statement2 = (ValidateStatement)FromProduction(6);
                ValidateStatementCompound statementCompoundTrue = ValidateStatementCompound.Encapsulate(statement1);
                ValidateStatementCompound statementCompoundFalse = ValidateStatementCompound.Encapsulate(statement2);
                return ReturnObject(new ValidateStatementIfElse(Owner, condition1, statementCompoundTrue, statementCompoundFalse, Line(0), Column(0)));
            case 26:
                ValidateTypeIdentifier typeIdentifier4 = (ValidateTypeIdentifier)FromProduction(2);
                return ReturnObject(new ValidateStatementForEach(Owner, new ValidateStatementScopedLocal(Owner, FromTerminal(3), typeIdentifier4, Line(3), Column(3)), (ValidateExpression)FromProduction(5), ValidateStatementCompound.Encapsulate((ValidateStatement)FromProduction(7)), Line(0), Column(0)));
            case 27:
                ValidateExpression condition2 = (ValidateExpression)FromProduction(2);
                return ReturnObject(new ValidateStatementWhile(Owner, (ValidateStatement)FromProduction(4), condition2, false, Line(0), Column(0)));
            case 28:
                return ReturnObject(new ValidateStatementWhile(Owner, (ValidateStatement)FromProduction(1), (ValidateExpression)FromProduction(4), true, Line(0), Column(0)));
            case 29:
                ValidateStatement statementList1 = (ValidateStatement)FromProduction(2);
                ValidateExpression condition3 = (ValidateExpression)FromProduction(4);
                ValidateExpression expression6 = (ValidateExpression)FromProduction(6);
                ValidateStatement statementList2 = (ValidateStatement)FromProduction(8);
                ValidateStatementExpression statementExpression = new ValidateStatementExpression(Owner, expression6, expression6.Line, expression6.Column);
                statementList2.AppendToEnd(statementExpression);
                ValidateStatementWhile validateStatementWhile = new ValidateStatementWhile(Owner, new ValidateStatementCompound(Owner, statementList2, statementList2.Line, statementList2.Column), condition3, false, Line(0), Column(0));
                statementList1.AppendToEnd(validateStatementWhile);
                return ReturnObject(new ValidateStatementCompound(Owner, statementList1, Line(0), Column(0)));
            case 30:
                return ReturnObject(FromProduction(0));
            case 31:
                ValidateExpression expression7 = (ValidateExpression)FromProduction(0);
                return ReturnObject(new ValidateStatementExpression(Owner, expression7, expression7.Line, expression7.Column));
            case 32:
                return ReturnObject(new ValidateStatementReturn(Owner, null, Line(0), Column(0)));
            case 33:
                return ReturnObject(new ValidateStatementReturn(Owner, (ValidateExpression)FromProduction(1), Line(0), Column(0)));
            case 34:
                return ReturnObject(new ValidateStatementBreak(Owner, false, Line(0), Column(0)));
            case 35:
                return ReturnObject(new ValidateStatementBreak(Owner, true, Line(0), Column(0)));
            case 36:
                return ReturnObject(new ValidateStatementAttribute(Owner, FromTerminal(1), (ValidateParameter)FromProduction(3), Line(1), Column(1)));
            case 37:
                return ReturnObject(new ValidateStatementCompound(Owner, (ValidateStatement)FromProduction(1), Line(0), Column(0)));
            case 38:
                ValidateStatement validateStatement1 = (ValidateStatement)FromProduction(0);
                ValidateStatement validateStatement2 = (ValidateStatement)FromProduction(1);
                if (validateStatement1 != null)
                    validateStatement1.AppendToEnd(validateStatement2);
                else
                    validateStatement1 = validateStatement2;
                return ReturnObject(validateStatement1);
            case 39:
                return ReturnObject(null);
            case 40:
                return ReturnObject(new ValidateExpressionCall(Owner, (ValidateExpression)FromProduction(0), FromTerminal(2), (ValidateParameter)FromProduction(4), Line(2), Column(2)));
            case 41:
                string memberName = FromTerminal(0);
                ValidateParameter parameterList = (ValidateParameter)FromProduction(2);
                return ReturnObject(new ValidateExpressionCall(Owner, new ValidateExpressionThis(Owner, Line(0), Column(0)), memberName, parameterList, Line(0), Column(0)));
            case 42:
                return ReturnObject(new ValidateExpressionCall(Owner, (ValidateExpression)FromProduction(0), FromTerminal(2), null, Line(2), Column(2)));
            case 43:
                return ReturnObject(new ValidateExpressionCall(Owner, new ValidateTypeIdentifier(Owner, FromTerminal(0), FromTerminal(2), Line(0), Column(0)), FromTerminal(4), null, Line(4), Column(4)));
            case 44:
                return ReturnObject(new ValidateExpressionCall(Owner, new ValidateTypeIdentifier(Owner, FromTerminal(0), FromTerminal(2), Line(0), Column(0)), FromTerminal(4), (ValidateParameter)FromProduction(6), Line(4), Column(4)));
            case 45:
                return ReturnObject(new ValidateExpressionIndex(Owner, (ValidateExpression)FromProduction(0), (ValidateParameter)FromProduction(2), Line(1), Column(1)));
            case 46:
                return ReturnObject(new ValidateExpressionNew(Owner, (ValidateTypeIdentifier)FromProduction(1), (ValidateParameter)FromProduction(3), Line(0), Column(0)));
            case 47:
                return ReturnObject(new ValidateExpressionSymbol(Owner, FromTerminal(0), Line(0), Column(0)));
            case 48:
                return ReturnObject(new ValidateExpressionConstant(Owner, FromTerminalTrim(0, 1, 1), ConstantType.String, Line(0), Column(0)));
            case 49:
                return ReturnObject(new ValidateExpressionConstant(Owner, FromTerminalTrim(0, 2, 1), ConstantType.StringLiteral, Line(0), Column(0)));
            case 50:
                return ReturnObject(new ValidateExpressionConstant(Owner, FromTerminal(0), ConstantType.Integer, Line(0), Column(0)));
            case 51:
                return ReturnObject(new ValidateExpressionConstant(Owner, FromTerminalTrim(0, 0, 1), ConstantType.LongInteger, Line(0), Column(0)));
            case 52:
                return ReturnObject(new ValidateExpressionConstant(Owner, FromTerminal(0), ConstantType.Float, Line(0), Column(0)));
            case 53:
                return ReturnObject(new ValidateExpressionConstant(Owner, true, Line(0), Column(0)));
            case 54:
                return ReturnObject(new ValidateExpressionConstant(Owner, false, Line(0), Column(0)));
            case 55:
                return ReturnObject(new ValidateExpressionConstant(Owner, null, ConstantType.Null, Line(0), Column(0)));
            case 56:
                return ReturnObject(new ValidateExpressionThis(Owner, Line(0), Column(0)));
            case 57:
                return ReturnObject(new ValidateExpressionBaseClass(Owner, Line(0), Column(0)));
            case 58:
                return ReturnObject(new ValidateExpressionTypeOf(Owner, (ValidateTypeIdentifier)FromProduction(2), Line(0), Column(0)));
            case 59:
                return ReturnObject((ValidateExpression)FromProduction(1));
            case 60:
                return ReturnObject(new ValidateExpressionDeclareTrigger(Owner, (ValidateExpression)FromProduction(1), Line(0), Column(0)));
            case 61:
                return ReturnObject(ConstructValidateExpressionUnaryOperation(OperationType.LogicalNot));
            case 62:
                return ReturnObject(ConstructValidateExpressionUnaryOperation(OperationType.MathNegate));
            case 63:
                return ReturnObject(ConstructValidateExpressionPostUnaryOperation(OperationType.PostIncrement));
            case 64:
                return ReturnObject(ConstructValidateExpressionPostUnaryOperation(OperationType.PostDecrement));
            case 65:
                string prefix = FromTerminal(1);
                string typeName = FromTerminal(3);
                ValidateExpression castee = (ValidateExpression)FromProduction(5);
                return ReturnObject(new ValidateExpressionCast(Owner, new ValidateTypeIdentifier(Owner, prefix, typeName, Line(1), Column(1)), castee, Line(0), Column(0)));
            case 66:
                return ReturnObject(new ValidateExpressionCast(Owner, (ValidateExpression)FromProduction(1), (ValidateExpression)FromProduction(3), Line(0), Column(0)));
            case 67:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.MathMultiply));
            case 68:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.MathDivide));
            case 69:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.MathModulus));
            case 70:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.MathAdd));
            case 71:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.MathSubtract));
            case 72:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.RelationalLessThan));
            case 73:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.RelationalGreaterThan));
            case 74:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.RelationalLessThanEquals));
            case 75:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.RelationalGreaterThanEquals));
            case 76:
                return ReturnObject(new ValidateExpressionIsCheck(Owner, (ValidateExpression)FromProduction(0), (ValidateTypeIdentifier)FromProduction(2), Line(1), Column(1)));
            case 77:
                return ReturnObject(new ValidateExpressionAs(Owner, (ValidateExpression)FromProduction(0), (ValidateTypeIdentifier)FromProduction(2), Line(1), Column(1)));
            case 78:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.RelationalEquals));
            case 79:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.RelationalNotEquals));
            case 80:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.LogicalAnd));
            case 81:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.LogicalOr));
            case 82:
                return ReturnObject(new ValidateExpressionNullCoalescing(Owner, (ValidateExpression)FromProduction(0), (ValidateExpression)FromProduction(2), Line(1), Column(1)));
            case 83:
                return ReturnObject(new ValidateExpressionTernary(Owner, (ValidateExpression)FromProduction(0), (ValidateExpression)FromProduction(2), (ValidateExpression)FromProduction(4), Line(1), Column(1)));
            case 84:
                return ReturnObject(new ValidateExpressionAssignment(Owner, (ValidateExpression)FromProduction(0), (ValidateExpression)FromProduction(2), Line(1), Column(1)));
            case 85:
                return ReturnObject(ValidateParameter.EmptyList);
            case 86:
                return ReturnObject((ValidateParameter)FromProduction(0));
            case 87:
                ValidateExpression expression8 = (ValidateExpression)FromProduction(0);
                return ReturnObject(new ValidateParameter(Owner, expression8, expression8.Line, expression8.Column));
            case 88:
                ValidateParameter validateParameter1 = (ValidateParameter)FromProduction(0);
                ValidateExpression expression9 = (ValidateExpression)FromProduction(2);
                ValidateParameter validateParameter2 = new ValidateParameter(Owner, expression9, expression9.Line, expression9.Column);
                validateParameter1.AppendToEnd(validateParameter2);
                return ReturnObject(validateParameter1);
            case 89:
                return ReturnObject(new ValidateTypeIdentifier(Owner, FromTerminal(0), FromTerminal(2), Line(0), Column(0)));
            case 90:
                return ReturnObject(new ValidateTypeIdentifier(Owner, null, FromTerminal(0), Line(0), Column(0)));
            default:
                return stackElement();
        }
    }

    private ValidateExpressionOperation ConstructValidateExpressionOperation(
      OperationType op)
    {
        ValidateExpression leftSide = (ValidateExpression)FromProduction(0);
        ValidateExpression rightSide = (ValidateExpression)FromProduction(2);
        return new ValidateExpressionOperation(Owner, leftSide, op, rightSide, Line(1), Column(1));
    }

    private ValidateExpressionOperation ConstructValidateExpressionUnaryOperation(
      OperationType op)
    {
        return new ValidateExpressionOperation(Owner, (ValidateExpression)FromProduction(1), op, null, Line(0), Column(0));
    }

    private ValidateExpressionOperation ConstructValidateExpressionPostUnaryOperation(
      OperationType op)
    {
        return new ValidateExpressionOperation(Owner, (ValidateExpression)FromProduction(0), op, null, Line(1), Column(1));
    }

    private ValidateMethod ConstructValidateMethod(bool hasBody)
    {
        string methodName = FromTerminal(2);
        ValidateTypeIdentifier returnType = (ValidateTypeIdentifier)FromProduction(1);
        Vector<MethodSpecifier> specifiers = (Vector<MethodSpecifier>)FromProduction(0);
        ValidateParameterDefinitionList paramList = (ValidateParameterDefinitionList)FromProduction(4);
        ValidateCode body = new ValidateCode(Owner, new ValidateStatementCompound(Owner, hasBody ? (ValidateStatement)FromProduction(7) : null, Line(6), Column(6)), Line(6), Column(6));
        return new ValidateMethod(Owner, Line(2), Column(2), methodName, returnType, specifiers, paramList, body);
    }

    private int CurrentLine => m_lex.consumer().line();

    private int CurrentColumn => m_lex.consumer().offset();
}
