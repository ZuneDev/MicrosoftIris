// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.DebugDefault
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Collections;
using System.Globalization;
using System.Threading;

namespace Microsoft.Iris.Render.Extensions
{
    public class DebugDefault : IDebug, IDisposable
    {
        private eDebug m_eDebug;
        private static ReaderWriterLock s_contextLock = new ReaderWriterLock();
        private static Map<Thread, Stack> s_contextDict = new Map<Thread, Stack>();

        public DebugDefault() => this.m_eDebug = new eDebug();

        void IDisposable.Dispose()
        {
            if (this.m_eDebug == null)
                return;
            this.m_eDebug.Dispose();
            this.m_eDebug = null;
        }

        public static Stack CurrentContextStack
        {
            get
            {
                Stack stack = null;
                s_contextLock.AcquireReaderLock(-1);
                try
                {
                    stack = s_contextDict[Thread.CurrentThread];
                }
                finally
                {
                    s_contextLock.ReleaseReaderLock();
                }
                if (stack == null)
                {
                    s_contextLock.AcquireWriterLock(-1);
                    try
                    {
                        stack = s_contextDict[Thread.CurrentThread];
                        if (stack == null)
                        {
                            stack = new Stack();
                            stack.Push("<top>");
                            s_contextDict[Thread.CurrentThread] = stack;
                        }
                    }
                    finally
                    {
                        s_contextLock.ReleaseWriterLock();
                    }
                }
                return stack;
            }
        }

        void IDebug.Enter(string context)
        {
            if (context == null)
                context = "<null>";
            CurrentContextStack.Push(context);
            this.m_eDebug.WriteLinePrefix = string.Format(CultureInfo.InvariantCulture, "[{0}]", context);
        }

        void IDebug.Leave(string context)
        {
            Stack currentContextStack = CurrentContextStack;
            this.m_eDebug.Assert(currentContextStack.Count > 0, "Context stack is empty");
            if (currentContextStack.Count > 0)
                this.m_eDebug.WriteLinePrefix = string.Format(CultureInfo.InvariantCulture, "[{0}]", (string)currentContextStack.Peek());
            else
                this.m_eDebug.WriteLinePrefix = null;
        }

        void IDebug.Assert(bool condition, string message) => this.m_eDebug.Assert(condition, message);

        void IDebug.Throw(bool condition, string message) => this.m_eDebug.Prompt(condition, message, typeof(InvalidOperationException));

        void IDebug.Write(DebugCategory category, int level, string message) => this.m_eDebug.Write(category, (byte)level, message);

        void IDebug.WriteLine(DebugCategory category, int level, string message) => this.m_eDebug.WriteLine(category, (byte)level, message);

        void IDebug.Indent(DebugCategory category, int level) => this.m_eDebug.Indent(category, (byte)level);

        void IDebug.Unindent(DebugCategory category, int level) => this.m_eDebug.Unindent(category, (byte)level);

        bool IDebug.IsCategoryEnabled(DebugCategory category, int level) => this.m_eDebug.IsCategoryEnabled(category, (byte)level);
    }
}
