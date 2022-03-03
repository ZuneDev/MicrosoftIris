// Decompiled with JetBrains decompiler
// Type: SSVParseLib.SSYacc
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.Session;

namespace SSVParseLib
{
    internal class SSYacc
    {
        private const int SSYaccActionShift = 0;
        private const int SSYaccActionError = 1;
        private const int SSYaccActionReduce = 2;
        private const int SSYaccActionAccept = 3;
        private const int SSYaccActionConflict = 4;
        public const int m_eofToken = -1;
        private const int m_errorToken = -2;
        private const int SSYaccLexemeCacheMax = -1;
        private int m_cache;
        private int m_state;
        protected SSLex m_lex;
        private int m_action;
        private int m_leftside;
        private bool m_error;
        private bool m_abort;
        private int m_production;
        private SSYaccStack m_stack;
        private SSYaccTable m_table;
        private int m_productionSize;
        private bool m_endOfInput;
        private SSLexLexeme m_endLexeme;
        private SSLexLexeme m_lookahead;
        private SSLexLexeme m_larLookahead;
        private SSLexSubtable m_lexSubtable;
        private SSYaccStackElement m_element;
        private SSYaccStackElement m_treeRoot;
        private SSYaccCache m_lexemeCache;
        private SourceMarkupLoader m_owner;
        private bool m_hasErrors;

        public string FromTerminal(int position) => elementFromProduction(position).lexeme().GetValue(m_lex);

        public string FromTerminalTrim(int position, int trimLeft, int trimRight) => elementFromProduction(position).lexeme().GetTrimmedValue(m_lex, trimLeft, trimRight);

        public object FromProduction(int position) => elementFromProduction(position).Object;

        public SSYaccStackElement ReturnObject(object value)
        {
            SSYaccStackElement yaccStackElement = stackElement();
            yaccStackElement.Object = value;
            return yaccStackElement;
        }

        public int Line(int position) => elementFromProduction(position).lexeme().line();

        public int Column(int position) => elementFromProduction(position).lexeme().offset();

        public SSYacc(SSYaccTable q_table, SSLex q_lex)
        {
            m_lex = q_lex;
            m_table = q_table;
            m_stack = new SSYaccStack(5, 5);
            m_lexemeCache = new SSYaccCache();
            Reset(null);
        }

        public void Reset(SourceMarkupLoader owner)
        {
            m_cache = 0;
            m_abort = false;
            m_error = false;
            m_endOfInput = false;
            m_owner = owner;
            m_action = 0;
            m_endOfInput = false;
            m_hasErrors = false;
            m_larLookahead = null;
            m_leftside = 0;
            m_lexSubtable = null;
            m_lookahead = null;
            m_production = 0;
            m_productionSize = 0;
            m_state = 0;
            m_endLexeme = null;
            m_element = new SSYaccStackElement();
            m_treeRoot = new SSYaccStackElement();
            m_stack.Clear();
            m_lexemeCache.Clear();
            if (owner == null)
                return;
            m_endLexeme = SSLexLexeme.CreateEOFLexeme(m_lex);
            m_element = stackElement();
            push();
        }

        public virtual SSYaccStackElement reduce(int q_prod, int q_length) => stackElement();

        public virtual SSLexLexeme nextLexeme() => m_lex.next();

        public virtual SSYaccStackElement stackElement() => new SSYaccStackElement();

        public virtual SSYaccStackElement shift(SSLexLexeme q_lexeme) => stackElement();

        public bool larLookahead(SSLexLexeme q_lex) => false;

        public virtual bool error(int q_state, SSLexLexeme q_look)
        {
            m_hasErrors = true;
            if (!m_lex.HasErrors)
            {
                string str = q_look.GetValue(m_lex);
                int line = q_look.line();
                int column = q_look.offset();
                string message = string.Format("Syntax Error: Unexpected character encountered: '{0}'", str);
                if (str == "eof")
                {
                    message = string.Format("Unexpected end of script (script beginning at line {0}, column {1})", line, column);
                    line = m_lex.consumer().line();
                    column = m_lex.consumer().offset();
                }
                ErrorManager.ReportError(line, column, message);
            }
            return true;
        }

        public SourceMarkupLoader Owner => m_owner;

        public bool HasErrors => m_hasErrors;

        public bool larError(int q_state, SSLexLexeme q_look, SSLexLexeme q_larLook) => error(q_state, q_look);

        public bool parse()
        {
            if (doGetLexeme(true))
                return true;
            while (!m_abort)
            {
                switch (m_action)
                {
                    case 0:
                        if (doShift())
                            return true;
                        continue;
                    case 1:
                        if (doError())
                            return true;
                        continue;
                    case 2:
                        if (doReduce())
                            return true;
                        continue;
                    case 3:
                        m_treeRoot = m_element;
                        return m_error;
                    case 4:
                        if (doConflict())
                            return true;
                        continue;
                    default:
                        return true;
                }
            }
            return true;
        }

        public bool doShift()
        {
            m_element = shift(m_lookahead);
            m_element.setLexeme(m_lookahead);
            m_element.setState(m_state);
            push();
            return doGetLexeme(true);
        }

        public bool doReduce()
        {
            m_element = reduce(m_production, m_productionSize);
            pop(m_productionSize);
            return goTo(m_leftside);
        }

        public bool doError()
        {
            m_error = true;
            return error(m_state, m_lookahead);
        }

        public bool doLarError()
        {
            m_error = true;
            return larError(m_state, m_lookahead, m_larLookahead);
        }

        public SSLexLexeme getLexemeCache()
        {
            SSLexLexeme ssLexLexeme = null;
            if (m_cache != -1 && m_lexemeCache.hasElements())
                ssLexLexeme = (SSLexLexeme)m_lexemeCache.Dequeue();
            if (ssLexLexeme == null)
            {
                m_cache = -1;
                ssLexLexeme = nextLexeme() ?? m_endLexeme;
                m_lexemeCache.Enqueue(ssLexLexeme);
            }
            return ssLexLexeme;
        }

        public bool doConflict()
        {
            m_cache = 0;
            int q_state = m_lexSubtable.lookup(0, m_lookahead.token());
            while ((m_larLookahead = getLexemeCache()) != null)
            {
                q_state = m_lexSubtable.lookup(q_state, m_larLookahead.token());
                if (q_state != -1)
                {
                    SSLexFinalState ssLexFinalState = m_lexSubtable.lookupFinal(q_state);
                    if (ssLexFinalState.isFinal())
                    {
                        if (ssLexFinalState.isReduce())
                        {
                            m_production = ssLexFinalState.token();
                            SSYaccTableProd ssYaccTableProd = m_table.lookupProd(m_production);
                            m_leftside = ssYaccTableProd.leftside();
                            m_productionSize = ssYaccTableProd.size();
                            return doReduce();
                        }
                        m_state = ssLexFinalState.token();
                        return doShift();
                    }
                }
                else
                    break;
            }
            return doLarError();
        }

        public bool doGetLexeme(bool q_look)
        {
            if ((m_lookahead = m_lexemeCache.remove()) == null)
                return getLexeme(q_look);
            if (larLookahead(m_lookahead))
                return true;
            if (q_look)
                lookupAction(m_state, m_lookahead.token());
            return false;
        }

        public bool getLexeme(bool q_look)
        {
            if (m_endOfInput)
                return true;
            m_lookahead = nextLexeme();
            if (m_lookahead == null)
            {
                m_endOfInput = true;
                m_lookahead = m_endLexeme;
            }
            if (q_look)
                lookupAction(m_state, m_lookahead.token());
            return false;
        }

        public bool goTo(int q_goto)
        {
            if (lookupGoto(m_state, m_leftside))
                return true;
            m_element.setState(m_state);
            push();
            lookupAction(m_state, m_lookahead.token());
            return false;
        }

        public void lookupAction(int q_state, int q_token)
        {
            SSYaccTableRowEntry yaccTableRowEntry = m_table.lookupRow(q_state).lookupAction(q_token);
            if (yaccTableRowEntry == null)
            {
                m_action = 1;
            }
            else
            {
                switch (m_action = yaccTableRowEntry.action())
                {
                    case 0:
                        m_state = yaccTableRowEntry.entry();
                        break;
                    case 2:
                        SSYaccTableProd ssYaccTableProd = m_table.lookupProd(yaccTableRowEntry.entry());
                        m_production = yaccTableRowEntry.entry();
                        m_leftside = ssYaccTableProd.leftside();
                        m_productionSize = ssYaccTableProd.size();
                        break;
                    case 4:
                        m_lexSubtable = m_table.larTable(yaccTableRowEntry.entry());
                        break;
                }
            }
        }

        public bool lookupGoto(int q_state, int q_token)
        {
            SSYaccTableRowEntry yaccTableRowEntry = m_table.lookupRow(q_state).lookupGoto(q_token);
            if (yaccTableRowEntry == null)
                return true;
            m_state = yaccTableRowEntry.entry();
            return false;
        }

        public bool push()
        {
            m_stack.push(m_element);
            return true;
        }

        public bool pop(int q_pop)
        {
            for (int index = 0; index < q_pop; ++index)
                m_stack.pop();
            m_state = m_stack.peek().state();
            return false;
        }

        public SSYaccStackElement elementFromProduction(int q_index) => m_stack.elementAt(m_stack.getSize() - m_productionSize + q_index);

        public SSYaccStackElement treeRoot() => m_treeRoot;
    }
}
