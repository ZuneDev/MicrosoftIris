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
    public ParserYaccClass(SSYaccTable q_table, SSLex q_lex)
      : base(q_table, q_lex)
    {
    }

    public override SSYaccStackElement reduce(int q_prod, int q_size)
    {
        switch ((ParserYaccProd)q_prod)
        {
            case ParserYaccProd.StartCodeBlock:
                return ReturnObject(new ValidateCode(Owner, new ValidateStatementCompound(Owner, (ValidateStatement)FromProduction(1), CurrentLine, CurrentColumn), CurrentLine, CurrentColumn));
            case ParserYaccProd.StartInlineExpression:
                return ReturnObject(FromProduction(2));
            case ParserYaccProd.StartMethods:
                return ReturnObject(FromProduction(1));
            case ParserYaccProd.MethodsEmpty:
                return ReturnObject(new ValidateMethodList(Owner, CurrentLine, CurrentColumn));
            case ParserYaccProd.Methods:
                ValidateMethodList validateMethodList = (ValidateMethodList)FromProduction(0);
                ValidateMethod expression1 = (ValidateMethod)FromProduction(1);
                validateMethodList.AppendToEnd(expression1);
                return ReturnObject(validateMethodList);
            case ParserYaccProd.MethodSpecifiersEmpty:
                return ReturnObject(new Vector<MethodSpecifier>());
            case ParserYaccProd.MethodSpecifiers:
                Vector<MethodSpecifier> vector = (Vector<MethodSpecifier>)FromProduction(0);
                MethodSpecifier methodSpecifier = (MethodSpecifier)FromProduction(1);
                vector.Add(methodSpecifier);
                return ReturnObject(vector);
            case ParserYaccProd.MethodSpecifierVirtual:
                return ReturnObject(MethodSpecifier.Virtual);
            case ParserYaccProd.MethodSpecifierOverride:
                return ReturnObject(MethodSpecifier.Override);
            case ParserYaccProd.Method:
                return ReturnObject(ConstructValidateMethod(true));
            case ParserYaccProd.ParameterDefinitionsEmpty:
                return ReturnObject(new ValidateParameterDefinitionList(Owner, CurrentLine, CurrentColumn));
            case ParserYaccProd.ParameterDefinitions:
                return ReturnObject((ValidateParameterDefinitionList)FromProduction(0));
            case ParserYaccProd.ParameterDefinitionListSingle:
                ValidateParameterDefinition expression2 = (ValidateParameterDefinition)FromProduction(0);
                ValidateParameterDefinitionList parameterDefinitionList1 = new ValidateParameterDefinitionList(Owner, expression2.Line, expression2.Column);
                parameterDefinitionList1.AppendToEnd(expression2);
                return ReturnObject(parameterDefinitionList1);
            case ParserYaccProd.ParameterDefinitionListMulti:
                ValidateParameterDefinitionList parameterDefinitionList2 = (ValidateParameterDefinitionList)FromProduction(0);
                ValidateParameterDefinition expression3 = (ValidateParameterDefinition)FromProduction(2);
                parameterDefinitionList2.AppendToEnd(expression3);
                return ReturnObject(parameterDefinitionList2);
            case ParserYaccProd.ParameterDefinition:
                ValidateTypeIdentifier typeIdentifier1 = (ValidateTypeIdentifier)FromProduction(0);
                string name = FromTerminal(1);
                return ReturnObject(new ValidateParameterDefinition(Owner, Line(1), Column(1), name, typeIdentifier1));
            case ParserYaccProd.ExpressionListEmpty:
                return ReturnObject(new ValidateExpressionList(Owner, CurrentLine, CurrentColumn));
            case ParserYaccProd.ExpressionList:
                return ReturnObject(FromProduction(0));
            case ParserYaccProd.ExpressionsSingle:
                ValidateExpression expression4 = (ValidateExpression)FromProduction(0);
                ValidateExpressionList validateExpressionList1 = new ValidateExpressionList(Owner, expression4.Line, expression4.Column);
                validateExpressionList1.AppendToEnd(expression4);
                return ReturnObject(validateExpressionList1);
            case ParserYaccProd.ExpressionsMulti:
                ValidateExpressionList validateExpressionList2 = (ValidateExpressionList)FromProduction(0);
                ValidateExpression expression5 = (ValidateExpression)FromProduction(2);
                validateExpressionList2.AppendToEnd(expression5);
                return ReturnObject(validateExpressionList2);
            case ParserYaccProd.StatementLocalDeclaration:
                ValidateTypeIdentifier typeIdentifier2 = (ValidateTypeIdentifier)FromProduction(0);
                return ReturnObject(new ValidateStatementScopedLocal(Owner, FromTerminal(1), typeIdentifier2, Line(1), Column(1)));
            case ParserYaccProd.StatementLocalAssignment:
                ValidateTypeIdentifier typeIdentifier3 = (ValidateTypeIdentifier)FromProduction(0);
                string str = FromTerminal(1);
                return ReturnObject(new ValidateStatementAssignment(Owner, new ValidateStatementScopedLocal(Owner, str, typeIdentifier3, Line(1), Column(1)), new ValidateExpressionSymbol(Owner, str, Line(1), Column(1)), (ValidateExpression)FromProduction(3), Line(1), Column(1)));
            case ParserYaccProd.ForInitializerDecl:
                return ReturnObject(FromProduction(0));
            case ParserYaccProd.ForInitializerExprList:
                ValidateExpressionList validateExpressionList3 = (ValidateExpressionList)FromProduction(0);
                return ReturnObject(new ValidateStatementExpression(Owner, validateExpressionList3, validateExpressionList3.Line, validateExpressionList3.Column));
            case ParserYaccProd.StatementIf:
                return ReturnObject(new ValidateStatementIf(Owner, (ValidateExpression)FromProduction(2), ValidateStatementCompound.Encapsulate((ValidateStatement)FromProduction(4)), Line(0), Column(0)));
            case ParserYaccProd.StatementIfElse:
                ValidateExpression condition1 = (ValidateExpression)FromProduction(2);
                ValidateStatement statement1 = (ValidateStatement)FromProduction(4);
                ValidateStatement statement2 = (ValidateStatement)FromProduction(6);
                ValidateStatementCompound statementCompoundTrue = ValidateStatementCompound.Encapsulate(statement1);
                ValidateStatementCompound statementCompoundFalse = ValidateStatementCompound.Encapsulate(statement2);
                return ReturnObject(new ValidateStatementIfElse(Owner, condition1, statementCompoundTrue, statementCompoundFalse, Line(0), Column(0)));
            case ParserYaccProd.StatementForEach:
                ValidateTypeIdentifier typeIdentifier4 = (ValidateTypeIdentifier)FromProduction(2);
                return ReturnObject(new ValidateStatementForEach(Owner, new ValidateStatementScopedLocal(Owner, FromTerminal(3), typeIdentifier4, Line(3), Column(3)), (ValidateExpression)FromProduction(5), ValidateStatementCompound.Encapsulate((ValidateStatement)FromProduction(7)), Line(0), Column(0)));
            case ParserYaccProd.StatementWhile:
                ValidateExpression condition2 = (ValidateExpression)FromProduction(2);
                return ReturnObject(new ValidateStatementWhile(Owner, (ValidateStatement)FromProduction(4), condition2, false, Line(0), Column(0)));
            case ParserYaccProd.StatementDoWhile:
                return ReturnObject(new ValidateStatementWhile(Owner, (ValidateStatement)FromProduction(1), (ValidateExpression)FromProduction(4), true, Line(0), Column(0)));
            case ParserYaccProd.StatementFor:
                ValidateStatement statementList1 = (ValidateStatement)FromProduction(2);
                ValidateExpression condition3 = (ValidateExpression)FromProduction(4);
                ValidateExpression expression6 = (ValidateExpression)FromProduction(6);
                ValidateStatement statementList2 = (ValidateStatement)FromProduction(8);
                ValidateStatementExpression statementExpression = new ValidateStatementExpression(Owner, expression6, expression6.Line, expression6.Column);
                statementList2.AppendToEnd(statementExpression);
                ValidateStatementWhile validateStatementWhile = new ValidateStatementWhile(Owner, new ValidateStatementCompound(Owner, statementList2, statementList2.Line, statementList2.Column), condition3, false, Line(0), Column(0));
                statementList1.AppendToEnd(validateStatementWhile);
                return ReturnObject(new ValidateStatementCompound(Owner, statementList1, Line(0), Column(0)));
            case ParserYaccProd.StatementDecl:
                return ReturnObject(FromProduction(0));
            case ParserYaccProd.StatementExpr:
                ValidateExpression expression7 = (ValidateExpression)FromProduction(0);
                return ReturnObject(new ValidateStatementExpression(Owner, expression7, expression7.Line, expression7.Column));
            case ParserYaccProd.StatementReturn:
                return ReturnObject(new ValidateStatementReturn(Owner, null, Line(0), Column(0)));
            case ParserYaccProd.StatementReturnExpression:
                return ReturnObject(new ValidateStatementReturn(Owner, (ValidateExpression)FromProduction(1), Line(0), Column(0)));
            case ParserYaccProd.StatementBreak:
                return ReturnObject(new ValidateStatementBreak(Owner, false, Line(0), Column(0)));
            case ParserYaccProd.StatementContinue:
                return ReturnObject(new ValidateStatementBreak(Owner, true, Line(0), Column(0)));
            case ParserYaccProd.StatementAttribute:
                return ReturnObject(new ValidateStatementAttribute(Owner, FromTerminal(1), (ValidateParameter)FromProduction(3), Line(1), Column(1)));
            case ParserYaccProd.StatementCompound:
                return ReturnObject(new ValidateStatementCompound(Owner, (ValidateStatement)FromProduction(1), Line(0), Column(0)));
            case ParserYaccProd.StatementsMulti:
                ValidateStatement validateStatement1 = (ValidateStatement)FromProduction(0);
                ValidateStatement validateStatement2 = (ValidateStatement)FromProduction(1);
                if (validateStatement1 != null)
                    validateStatement1.AppendToEnd(validateStatement2);
                else
                    validateStatement1 = validateStatement2;
                return ReturnObject(validateStatement1);
            case ParserYaccProd.StatementsEmpty:
                return ReturnObject(null);
            case ParserYaccProd.ExpressionCallMethod:
                return ReturnObject(new ValidateExpressionCall(Owner, (ValidateExpression)FromProduction(0), FromTerminal(2), (ValidateParameter)FromProduction(4), Line(2), Column(2)));
            case ParserYaccProd.ExpressionCallThisMethod:
                string memberName = FromTerminal(0);
                ValidateParameter parameterList = (ValidateParameter)FromProduction(2);
                return ReturnObject(new ValidateExpressionCall(Owner, new ValidateExpressionThis(Owner, Line(0), Column(0)), memberName, parameterList, Line(0), Column(0)));
            case ParserYaccProd.ExpressionCallProperty:
                return ReturnObject(new ValidateExpressionCall(Owner, (ValidateExpression)FromProduction(0), FromTerminal(2), null, Line(2), Column(2)));
            case ParserYaccProd.ExpressionCallStaticProperty:
                return ReturnObject(new ValidateExpressionCall(Owner, new ValidateTypeIdentifier(Owner, FromTerminal(0), FromTerminal(2), Line(0), Column(0)), FromTerminal(4), null, Line(4), Column(4)));
            case ParserYaccProd.ExpressionCallStaticMethod:
                return ReturnObject(new ValidateExpressionCall(Owner, new ValidateTypeIdentifier(Owner, FromTerminal(0), FromTerminal(2), Line(0), Column(0)), FromTerminal(4), (ValidateParameter)FromProduction(6), Line(4), Column(4)));
            case ParserYaccProd.ExpressionIndex:
                return ReturnObject(new ValidateExpressionIndex(Owner, (ValidateExpression)FromProduction(0), (ValidateParameter)FromProduction(2), Line(1), Column(1)));
            case ParserYaccProd.ExpressionNew:
                return ReturnObject(new ValidateExpressionNew(Owner, (ValidateTypeIdentifier)FromProduction(1), (ValidateParameter)FromProduction(3), Line(0), Column(0)));
            case ParserYaccProd.ExpressionSymbol:
                return ReturnObject(new ValidateExpressionSymbol(Owner, FromTerminal(0), Line(0), Column(0)));
            case ParserYaccProd.ExpressionString:
                return ReturnObject(new ValidateExpressionConstant(Owner, FromTerminalTrim(0, 1, 1), ConstantType.String, Line(0), Column(0)));
            case ParserYaccProd.ExpressionStringLiteral:
                return ReturnObject(new ValidateExpressionConstant(Owner, FromTerminalTrim(0, 2, 1), ConstantType.StringLiteral, Line(0), Column(0)));
            case ParserYaccProd.ExpressionInteger:
                return ReturnObject(new ValidateExpressionConstant(Owner, FromTerminal(0), ConstantType.Integer, Line(0), Column(0)));
            case ParserYaccProd.ExpressionLongInteger:
                return ReturnObject(new ValidateExpressionConstant(Owner, FromTerminalTrim(0, 0, 1), ConstantType.LongInteger, Line(0), Column(0)));
            case ParserYaccProd.ExpressionFloat:
                return ReturnObject(new ValidateExpressionConstant(Owner, FromTerminal(0), ConstantType.Float, Line(0), Column(0)));
            case ParserYaccProd.ExpressionTrue:
                return ReturnObject(new ValidateExpressionConstant(Owner, true, Line(0), Column(0)));
            case ParserYaccProd.ExpressionFalse:
                return ReturnObject(new ValidateExpressionConstant(Owner, false, Line(0), Column(0)));
            case ParserYaccProd.ExpressionNull:
                return ReturnObject(new ValidateExpressionConstant(Owner, null, ConstantType.Null, Line(0), Column(0)));
            case ParserYaccProd.ExpressionThis:
                return ReturnObject(new ValidateExpressionThis(Owner, Line(0), Column(0)));
            case ParserYaccProd.ExpressionBase:
                return ReturnObject(new ValidateExpressionBaseClass(Owner, Line(0), Column(0)));
            case ParserYaccProd.ExpressionTypeOf:
                return ReturnObject(new ValidateExpressionTypeOf(Owner, (ValidateTypeIdentifier)FromProduction(2), Line(0), Column(0)));
            case ParserYaccProd.ExpressionGroup:
                return ReturnObject((ValidateExpression)FromProduction(1));
            case ParserYaccProd.ExpressionDeclareTrigger:
                return ReturnObject(new ValidateExpressionDeclareTrigger(Owner, (ValidateExpression)FromProduction(1), Line(0), Column(0)));
            case ParserYaccProd.ExpressionUnaryOperationLogicalNot:
                return ReturnObject(ConstructValidateExpressionUnaryOperation(OperationType.LogicalNot));
            case ParserYaccProd.ExpressionUnaryMinus:
                return ReturnObject(ConstructValidateExpressionUnaryOperation(OperationType.MathNegate));
            case ParserYaccProd.ExpressionPostIncrement:
                return ReturnObject(ConstructValidateExpressionPostUnaryOperation(OperationType.PostIncrement));
            case ParserYaccProd.ExpressionPostDecrement:
                return ReturnObject(ConstructValidateExpressionPostUnaryOperation(OperationType.PostDecrement));
            case ParserYaccProd.ExpressionCastPrefixed:
                string prefix = FromTerminal(1);
                string typeName = FromTerminal(3);
                ValidateExpression castee = (ValidateExpression)FromProduction(5);
                return ReturnObject(new ValidateExpressionCast(Owner, new ValidateTypeIdentifier(Owner, prefix, typeName, Line(1), Column(1)), castee, Line(0), Column(0)));
            case ParserYaccProd.ExpressionCast:
                return ReturnObject(new ValidateExpressionCast(Owner, (ValidateExpression)FromProduction(1), (ValidateExpression)FromProduction(3), Line(0), Column(0)));
            case ParserYaccProd.ExpressionOperationMathMultiply:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.MathMultiply));
            case ParserYaccProd.ExpressionOperationMathDivide:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.MathDivide));
            case ParserYaccProd.ExpressionOperationMathModulus:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.MathModulus));
            case ParserYaccProd.ExpressionOperationMathAdd:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.MathAdd));
            case ParserYaccProd.ExpressionOperationMathSubtract:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.MathSubtract));
            case ParserYaccProd.ExpressionOperationRelationalLessThan:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.RelationalLessThan));
            case ParserYaccProd.ExpressionOperationRelationalGreaterThan:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.RelationalGreaterThan));
            case ParserYaccProd.ExpressionOperationRelationalLessThanEquals:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.RelationalLessThanEquals));
            case ParserYaccProd.ExpressionOperationRelationalGreaterThanEquals:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.RelationalGreaterThanEquals));
            case ParserYaccProd.ExpressionOperationIs:
                return ReturnObject(new ValidateExpressionIsCheck(Owner, (ValidateExpression)FromProduction(0), (ValidateTypeIdentifier)FromProduction(2), Line(1), Column(1)));
            case ParserYaccProd.ExpressionOperationAs:
                return ReturnObject(new ValidateExpressionAs(Owner, (ValidateExpression)FromProduction(0), (ValidateTypeIdentifier)FromProduction(2), Line(1), Column(1)));
            case ParserYaccProd.ExpressionOperationRelationalEquals:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.RelationalEquals));
            case ParserYaccProd.ExpressionOperationRelationalNotEquals:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.RelationalNotEquals));
            case ParserYaccProd.ExpressionOperationLogicalAnd:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.LogicalAnd));
            case ParserYaccProd.ExpressionOperationLogicalOr:
                return ReturnObject(ConstructValidateExpressionOperation(OperationType.LogicalOr));
            case ParserYaccProd.ExpressionNullCoalescing:
                return ReturnObject(new ValidateExpressionNullCoalescing(Owner, (ValidateExpression)FromProduction(0), (ValidateExpression)FromProduction(2), Line(1), Column(1)));
            case ParserYaccProd.ExpressionTernary:
                return ReturnObject(new ValidateExpressionTernary(Owner, (ValidateExpression)FromProduction(0), (ValidateExpression)FromProduction(2), (ValidateExpression)FromProduction(4), Line(1), Column(1)));
            case ParserYaccProd.ExpressionAssignment:
                return ReturnObject(new ValidateExpressionAssignment(Owner, (ValidateExpression)FromProduction(0), (ValidateExpression)FromProduction(2), Line(1), Column(1)));
            case ParserYaccProd.ParametersEmpty:
                return ReturnObject(ValidateParameter.EmptyList);
            case ParserYaccProd.Parameters:
                return ReturnObject((ValidateParameter)FromProduction(0));
            case ParserYaccProd.ParameterListSingle:
                ValidateExpression expression8 = (ValidateExpression)FromProduction(0);
                return ReturnObject(new ValidateParameter(Owner, expression8, expression8.Line, expression8.Column));
            case ParserYaccProd.ParameterListMulti:
                ValidateParameter validateParameter1 = (ValidateParameter)FromProduction(0);
                ValidateExpression expression9 = (ValidateExpression)FromProduction(2);
                ValidateParameter validateParameter2 = new ValidateParameter(Owner, expression9, expression9.Line, expression9.Column);
                validateParameter1.AppendToEnd(validateParameter2);
                return ReturnObject(validateParameter1);
            case ParserYaccProd.TypeIdentifierNamespaced:
                return ReturnObject(new ValidateTypeIdentifier(Owner, FromTerminal(0), FromTerminal(2), Line(0), Column(0)));
            case ParserYaccProd.TypeIdentifier:
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
