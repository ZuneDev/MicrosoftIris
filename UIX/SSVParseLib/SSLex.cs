// Decompiled with JetBrains decompiler
// Type: SSVParseLib.SSLex
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;

namespace SSVParseLib
{
    internal class SSLex
    {
        private int m_state;
        private char[] m_currentChar;
        private SSLexTable m_table;
        private SSLexConsumer m_consumer;
        public bool m_hasErrors;

        public SSLex(SSLexTable q_table, SSLexConsumer q_consumer)
        {
            q_table.Reset();
            m_table = q_table;
            m_consumer = q_consumer;
            m_currentChar = new char[1];
        }

        public void Reset(SSLexConsumer consumer)
        {
            m_table.Reset();
            m_consumer = consumer;
            m_currentChar[0] = char.MinValue;
            m_state = 0;
            m_hasErrors = false;
        }

        public SSLexConsumer consumer() => m_consumer;

        public virtual bool error(SSLexLexeme q_lexeme)
        {
            m_hasErrors = true;
            string message = string.Format("Syntax Error: Unexpected character encountered: '{0}'", m_currentChar[0]);
            ErrorManager.ReportError(q_lexeme.line(), q_lexeme.offset() + q_lexeme.length() - 1, message);
            return true;
        }

        public bool HasErrors => m_hasErrors;

        public virtual bool complete(SSLexLexeme q_lexeme) => true;

        public SSLexLexeme next()
        {
            SSLexLexeme ssLexLexeme = null;
            while (true)
            {
                SSLexMark? q_mark;
                SSLexFinalState q_final;
                do
                {
                    m_state = 0;
                    bool flag = false;
                    q_mark = new SSLexMark?();
                    q_final = m_table.lookupFinal(m_state);
                    if (q_final.isFinal())
                        m_consumer.mark();
                    while (m_consumer.next())
                    {
                        flag = true;
                        m_currentChar[0] = m_consumer.getCurrent();
                        m_state = m_table.lookup(m_state, m_currentChar[0]);
                        if (m_state != -1)
                        {
                            SSLexFinalState ssLexFinalState = m_table.lookupFinal(m_state);
                            if (ssLexFinalState.isFinal())
                            {
                                q_mark = new SSLexMark?(m_consumer.mark());
                                q_final = ssLexFinalState;
                            }
                            if (ssLexFinalState.isContextStart())
                            {
                                SSLexMark? nullable = new SSLexMark?(m_consumer.mark());
                            }
                        }
                        else
                            break;
                    }
                    if (flag)
                    {
                        if (q_final.isContextEnd() && q_mark.HasValue)
                            m_consumer.flushEndOfLine(ref q_mark);
                        if (q_final.isIgnore() && q_mark.HasValue)
                        {
                            m_consumer.flushLexeme(q_mark.Value);
                            if (q_final.isPop() && q_final.isPush())
                                m_table.gotoSubtable(q_final.pushIndex());
                            else if (q_final.isPop())
                                m_table.popSubtable();
                        }
                        else
                            goto label_19;
                    }
                    else
                        goto label_34;
                }
                while (!q_final.isPush());
                m_table.pushSubtable(q_final.pushIndex());
                continue;
            label_19:
                if (!q_final.isFinal() || !q_mark.HasValue)
                {
                    ssLexLexeme = new SSLexLexeme(m_consumer);
                    if (!error(ssLexLexeme))
                    {
                        m_consumer.flushLexeme();
                        ssLexLexeme = null;
                    }
                    else
                        break;
                }
                else
                {
                    if (q_final.isPop() && q_final.isPush())
                        m_table.gotoSubtable(q_final.pushIndex());
                    else if (q_final.isPop())
                        m_table.popSubtable();
                    else if (q_final.isPush())
                        m_table.pushSubtable(q_final.pushIndex());
                    if (q_final.isStartOfLine() && m_consumer.line() != 0 && m_consumer.offset() != 0)
                        m_consumer.flushStartOfLine(ref q_mark);
                    ssLexLexeme = new SSLexLexeme(m_consumer, q_final, q_mark.Value);
                    if (q_final.isKeyword())
                        m_table.findKeyword(ssLexLexeme);
                    m_consumer.flushLexeme(q_mark.Value);
                    if (!complete(ssLexLexeme))
                        ssLexLexeme = null;
                    else
                        break;
                }
            }
        label_34:
            return ssLexLexeme;
        }
    }
}
