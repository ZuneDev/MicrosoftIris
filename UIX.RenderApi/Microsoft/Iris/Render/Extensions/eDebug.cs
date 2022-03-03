// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.eDebug
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Internal;
using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Text;

namespace Microsoft.Iris.Render.Extensions
{
    [SuppressUnmanagedCodeSecurity]
    internal class eDebug
    {
        public const byte k_bDefaultTraceLevel = 1;
        private bool m_fTimedWriteLines;
        private bool m_fShowCategories;
        private bool m_fAlwaysShowBraces;
        private bool m_fOpenBracePending;
        private DateTime m_dtStart;
        private string m_stWriteLinePrefix;
        private string m_stRendererWriteLinePrefix;
        private string m_stIndent;
        private Stack m_stackIndentStrings;
        private byte m_bTraceLevelAdjustment;
        private string m_stPendingWrite;
        private int m_nMaxCategoryLength;
        private DebugCategory m_pendingBraceCategory;
        private string m_pendingBraceComment;

        internal eDebug()
        {
            this.m_stIndent = "    ";
            this.m_stackIndentStrings = new Stack();
            this.m_dtStart = DateTime.UtcNow;
            this.m_fShowCategories = true;
            this.m_bTraceLevelAdjustment = 0;
        }

        internal void Dispose()
        {
        }

        public static void Break()
        {
            if (Debugger.IsAttached || !Win32Api.IsDebuggerPresent())
                Debugger.Break();
            else
                Win32Api.DebugBreak();
        }

        public void Assert(bool fCondition)
        {
            if (!this.Assert(fCondition, "false", 2, null))
                return;
            Break();
        }

        public void Assert(bool fCondition, string stMessage)
        {
            if (!this.Assert(fCondition, stMessage, 2, null))
                return;
            Break();
        }

        public void Assert(bool fCondition, string stMessage, StackTrace stack)
        {
            if (!this.Assert(fCondition, stMessage, 0, stack))
                return;
            Break();
        }

        public void Assert(Type type, object obj, string stMessage)
        {
            this.Prompt(type != null, "must pass a valid type", typeof(ArgumentNullException));
            if (!this.Assert(type.IsInstanceOfType(obj), stMessage, 2, null))
                return;
            Break();
        }

        private bool Assert(bool fCondition, string stMessage, int cIgnoreFrames, StackTrace stack)
        {
            if (fCondition)
                return false;
            if (stack == null)
                stack = new StackTrace(true);
            return this.DisplayStackTrace("Iris Assert", stMessage, stack, cIgnoreFrames);
        }

        public void Prompt(string stMessage) => this.Prompt(false, stMessage, null, null, 1);

        public void Prompt(string stMessage, Type type) => this.Prompt(false, stMessage, type, null, 1);

        public void Prompt(bool fCondition, string stMessage) => this.Prompt(fCondition, stMessage, null, null, 1);

        public void Prompt(bool fCondition, string stMessage, Type type) => this.Prompt(fCondition, stMessage, type, null, 1);

        public void Prompt(bool fCondition, string stMessage, Type type, object objSubject) => this.Prompt(fCondition, stMessage, type, objSubject, 1);

        private void Prompt(
          bool fCondition,
          string stMessage,
          Type type,
          object objSubject,
          int cIgnoreFrames)
        {
            if (!fCondition)
            {
                StackTrace trace = new StackTrace(true);
                if (this.DisplayStackTrace("Iris API Validation Error (Application error)", stMessage, trace, cIgnoreFrames + 1))
                    Break();
                Exception exception;
                if (type == null)
                {
                    exception = new ArgumentException(stMessage);
                }
                else
                {
                    object[] objArray;
                    if (objSubject != null)
                        objArray = new object[2]
                        {
               stMessage,
              objSubject
                        };
                    else
                        objArray = new object[1] { stMessage };
                    exception = (Exception)Activator.CreateInstance(type, objArray);
                }
                throw exception;
            }
        }

        public void PromptCompatibleArray(Array array, int indexStart, Type typeObject, int numItems)
        {
            this.Prompt(array != null, "Must have valid array", typeof(ArgumentNullException));
            this.Prompt(typeObject != null, "Must have valid Type", typeof(ArgumentNullException));
            this.Prompt(array.Rank == 1, "Array must have rank=1", typeof(RankException));
            this.Prompt(indexStart < array.Length || numItems == 0, "Must have valid index", typeof(ArgumentException));
            this.Prompt(indexStart >= 0, "Must have valid index", typeof(ArgumentOutOfRangeException));
            this.Prompt(indexStart + numItems <= array.Length, "Must have sufficient room in array", typeof(ArgumentOutOfRangeException));
            Type elementType = array.GetType().GetElementType();
            this.Prompt(typeObject == elementType || typeObject.IsSubclassOf(elementType), "Must be a compatible array type", typeof(ArrayTypeMismatchException));
        }

        public void PromptListContents(IList list, Type typeExpected)
        {
            this.Prompt(list != null, "Must have valid list", typeof(ArgumentNullException));
            this.Prompt(typeExpected != null, "Must have valid Type", typeof(ArgumentNullException));
            foreach (object obj in list)
                this.Prompt(obj != null && typeExpected.IsAssignableFrom(obj.GetType()), "List should only contain {0}.", typeExpected);
        }

        public void Write(DebugCategory cat, object oValue)
        {
            if (!this.IsCategoryEnabled(cat))
                return;
            this.WriteWorker(cat, oValue);
        }

        public void WriteLine(DebugCategory cat, object oValue)
        {
            if (!this.IsCategoryEnabled(cat))
                return;
            this.WriteLineWorker(cat, oValue);
        }

        public void Write(DebugCategory cat, byte bLevel, object oValue)
        {
            if (!this.IsCategoryEnabled(cat, bLevel))
                return;
            this.WriteWorker(cat, oValue);
        }

        public void WriteLine(DebugCategory cat, byte bLevel, object oValue)
        {
            if (!this.IsCategoryEnabled(cat, bLevel))
                return;
            this.WriteLineWorker(cat, oValue);
        }

        public void Write(DebugCategory cat, string stMessage)
        {
            if (!this.IsCategoryEnabled(cat))
                return;
            this.WriteWorker(cat, stMessage);
        }

        public void WriteLine(DebugCategory cat, string stMessage)
        {
            if (!this.IsCategoryEnabled(cat))
                return;
            this.WriteLineWorker(cat, stMessage);
        }

        public bool TimedWriteLines
        {
            get => this.m_fTimedWriteLines;
            set
            {
                if (this.m_fTimedWriteLines == value)
                    return;
                this.m_fTimedWriteLines = value;
                eDebugApi.DebugSetTimedWriteLines(value);
            }
        }

        public bool ShowCategories
        {
            get => this.m_fShowCategories;
            set
            {
                this.m_fShowCategories = value;
                if (value)
                    return;
                this.m_nMaxCategoryLength = 0;
            }
        }

        public bool AlwaysShowBraces
        {
            get => this.m_fAlwaysShowBraces;
            set
            {
                this.m_fAlwaysShowBraces = value;
                if (!value || !this.m_fOpenBracePending)
                    return;
                this.FlushBraces();
            }
        }

        public string WriteLinePrefix
        {
            get => this.m_stWriteLinePrefix;
            set => this.m_stWriteLinePrefix = value;
        }

        public string RendererWriteLinePrefix
        {
            get => this.m_stRendererWriteLinePrefix;
            set
            {
                this.m_stRendererWriteLinePrefix = value;
                eDebugApi.DebugSetWriteLinePrefix(value);
            }
        }

        public byte TraceLevelAdjustment
        {
            get => this.m_bTraceLevelAdjustment;
            set => this.m_bTraceLevelAdjustment = value;
        }

        public bool IsCategoryEnabled(DebugCategory cat) => false;

        public bool IsCategoryEnabled(DebugCategory cat, byte bLevel) => false;

        internal void EnableCategory(DebugCategory cat, bool fEnable)
        {
        }

        internal void EnableCategory(DebugCategory cat, byte bLevel)
        {
        }

        public string IndentString
        {
            get => this.m_stIndent;
            set
            {
                if (!(this.m_stIndent != value))
                    return;
                this.m_stIndent = value;
                int indentLevel = this.IndentLevel;
                this.IndentLevel = 0;
                this.IndentLevel = indentLevel;
            }
        }

        public void Indent(DebugCategory cat) => this.Indent(cat, 1);

        public void Indent(DebugCategory cat, byte bLevel)
        {
            if (!this.IsCategoryEnabled(cat, bLevel))
                return;
            this.Indent();
        }

        public void Unindent(DebugCategory cat) => this.Unindent(cat, 1);

        public void Unindent(DebugCategory cat, byte bLevel)
        {
            if (!this.IsCategoryEnabled(cat, bLevel))
                return;
            this.Unindent();
        }

        public int IndentLevel
        {
            get => this.m_stackIndentStrings.Count;
            set
            {
                while (this.IndentLevel < value)
                    this.Indent();
                while (this.IndentLevel > value)
                    this.Unindent();
            }
        }

        public void OpenBrace(DebugCategory cat) => this.OpenBrace(cat, null);

        public void OpenBrace(DebugCategory cat, string stComment) => this.OpenBrace(cat, 1, stComment);

        public void OpenBrace(DebugCategory cat, byte bLevel, string stComment)
        {
            if (!this.IsCategoryEnabled(cat, bLevel))
                return;
            if (this.m_fOpenBracePending)
                this.FlushBraces();
            if (stComment != null)
                this.WriteLineWorker(cat, stComment);
            if (!this.m_fAlwaysShowBraces)
            {
                this.m_fOpenBracePending = true;
                this.m_pendingBraceCategory = cat;
                this.m_pendingBraceComment = null;
            }
            else
            {
                this.WriteLineWorker(cat, "{");
                this.Indent();
            }
        }

        public void DeferOpenBrace(DebugCategory cat) => this.DeferOpenBrace(cat, null);

        public void DeferOpenBrace(DebugCategory cat, string stComment) => this.DeferOpenBrace(cat, 1, stComment);

        public void DeferOpenBrace(DebugCategory cat, byte bLevel, string stComment)
        {
            if (!this.IsCategoryEnabled(cat, bLevel))
                return;
            if (this.m_fOpenBracePending)
                this.FlushBraces();
            this.m_fOpenBracePending = true;
            this.m_pendingBraceCategory = cat;
            this.m_pendingBraceComment = stComment;
        }

        public void FlushBraces()
        {
            if (!this.m_fOpenBracePending)
                return;
            this.m_fOpenBracePending = false;
            bool alwaysShowBraces = this.m_fAlwaysShowBraces;
            this.m_fAlwaysShowBraces = true;
            this.OpenBrace(this.m_pendingBraceCategory, this.m_pendingBraceComment);
            this.m_fAlwaysShowBraces = alwaysShowBraces;
        }

        public void CloseBrace(DebugCategory cat) => this.CloseBrace(cat, 1);

        public void CloseBrace(DebugCategory cat, byte bLevel)
        {
            if (!this.IsCategoryEnabled(cat, bLevel))
                return;
            if (this.m_fOpenBracePending && cat == this.m_pendingBraceCategory)
            {
                this.m_fOpenBracePending = false;
                this.m_pendingBraceComment = null;
            }
            else
            {
                this.Unindent();
                this.WriteLineWorker(cat, "}");
            }
        }

        private bool DisplayStackTrace(
          string title,
          string stMessage,
          StackTrace trace,
          int cIgnoreFrames)
        {
            this.Prompt(trace != null, "Must pass a valid StackTrace", typeof(ArgumentNullException));
            StackFrame frame1 = trace.GetFrame(cIgnoreFrames);
            string filename = frame1.GetFileName();
            int fileLineNumber = frame1.GetFileLineNumber();
            if (filename == null)
                filename = "<no filename info>";
            StringBuilder stringBuilder = new StringBuilder(10240);
            for (int index = cIgnoreFrames; index < trace.FrameCount; ++index)
            {
                StackFrame frame2 = trace.GetFrame(index);
                stringBuilder.Append(frame2.GetILOffset());
                stringBuilder.Append("\t");
                string fileName = frame2.GetFileName();
                if (!string.IsNullOrEmpty(fileName))
                {
                    stringBuilder.Append(fileName);
                    stringBuilder.Append(",");
                    stringBuilder.Append(frame2.GetFileLineNumber());
                }
                else
                    stringBuilder.Append("<unknown>");
                stringBuilder.Append("\t");
                MethodBase method = frame2.GetMethod();
                Type reflectedType = method.ReflectedType;
                if (reflectedType != null)
                {
                    stringBuilder.Append(reflectedType.Name);
                    stringBuilder.Append(".");
                }
                else
                    stringBuilder.Append("::");
                stringBuilder.Append(method.Name);
                stringBuilder.Append("\t");
                if (reflectedType != null)
                {
                    AssemblyName assemblyName = new AssemblyName(reflectedType.Assembly.FullName);
                    stringBuilder.Append(assemblyName.Name);
                }
                else
                {
                    Assembly entryAssembly = Assembly.GetEntryAssembly();
                    if (entryAssembly != null)
                    {
                        AssemblyName assemblyName = new AssemblyName(entryAssembly.FullName);
                        stringBuilder.Append(assemblyName.Name);
                    }
                    else
                        stringBuilder.Append("<unknown>");
                }
                stringBuilder.Append("\n");
                stringBuilder.Append("\t");
            }
            string str = stringBuilder.ToString();
            this.OpenBrace(DebugCategory.Failure, string.Format("Assert: {0}", stMessage));
            this.WriteLine(DebugCategory.Failure, string.Format("Filename: {0}", filename));
            this.WriteLine(DebugCategory.Failure, string.Format("Line:     {0}", fileLineNumber));
            this.OpenBrace(DebugCategory.Failure, string.Format("Stack trace:"));
            this.WriteLine(DebugCategory.Failure, string.Format("Frame\tFile Name\tMethod Name\tAssembly", fileLineNumber));
            this.WriteLine(DebugCategory.Failure, str);
            this.CloseBrace(DebugCategory.Failure);
            this.CloseBrace(DebugCategory.Failure);
            return eDebugApi.DebugDisplayErrorStack(stMessage, filename, fileLineNumber, title, str);
        }

        private string CurrentIndentString => this.m_stackIndentStrings.Count == 0 ? string.Empty : (string)this.m_stackIndentStrings.Peek();

        private void WriteWorker(DebugCategory cat, object oValue)
        {
            string str1 = null;
            if (oValue != null)
                str1 = oValue.ToString();
            this.m_stPendingWrite += str1;
            if (this.m_stPendingWrite == null)
                return;
            int length1 = this.m_stPendingWrite.LastIndexOf('\n');
            if (length1 <= 0)
                return;
            int length2 = this.m_stPendingWrite.Length - length1 - 1;
            if (length2 > 0)
            {
                string str2 = this.m_stPendingWrite.Substring(0, length1);
                string str3 = this.m_stPendingWrite.Substring(length1 + 1, length2);
                this.m_stPendingWrite = null;
                this.WriteLineWorker(cat, str2);
                this.m_stPendingWrite = str3;
            }
            else
            {
                string str2 = this.m_stPendingWrite.Substring(0, this.m_stPendingWrite.Length - 1);
                this.m_stPendingWrite = null;
                this.WriteLineWorker(cat, str2);
            }
        }

        private void WriteLineWorker(DebugCategory cat, object oValue)
        {
            if (this.m_fOpenBracePending)
                this.FlushBraces();
            string str1 = null;
            if (this.ShowCategories)
            {
                str1 = string.Format("[{0}] ", cat);
                int length = str1.Length;
                if (this.m_nMaxCategoryLength > length)
                    str1 = str1.PadLeft(this.m_nMaxCategoryLength);
                else if (this.m_nMaxCategoryLength < length)
                    this.m_nMaxCategoryLength = length;
            }
            string str2 = this.m_stPendingWrite + oValue;
            this.m_stPendingWrite = null;
            string str3 = str2;
            char[] chArray = new char[1] { '\n' };
            foreach (string str4 in str3.Split(chArray))
            {
                string lpOutputString = this.WriteLinePrefix + str1 + this.CurrentIndentString + str4 + "\n";
                if (this.TimedWriteLines)
                    lpOutputString = this.PrefixCurrentTime(lpOutputString);
                Win32Api.OutputDebugString(lpOutputString);
            }
        }

        private void Indent() => this.m_stackIndentStrings.Push(this.CurrentIndentString + this.m_stIndent);

        private void Unindent()
        {
            if (this.m_stackIndentStrings.Count <= 0)
                return;
            this.m_stackIndentStrings.Pop();
        }

        private string PrefixCurrentTime(object oValue) => oValue.ToString();
    }
}
