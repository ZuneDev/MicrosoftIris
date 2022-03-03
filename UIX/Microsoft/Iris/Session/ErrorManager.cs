// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Session.ErrorManager
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Iris.Session
{
    internal static class ErrorManager
    {
        private static uint s_totalErrorsReported;
        private static Stack<ErrorManager.Context> s_contextStack = new Stack<ErrorManager.Context>();
        private static uint s_ignoringErrorsDepth;
        private static bool s_errorBatchPending;
        private static IList s_errors;
        private static readonly SimpleCallback s_drainErrorQueueHandler = new SimpleCallback(OnDrainErrorQueue);

        public static ErrorWatermark Watermark => new ErrorWatermark(s_totalErrorsReported);

        public static void EnterContext(object contextObject) => EnterContext(contextObject, false);

        public static void EnterContext(object contextObject, bool ignoreErrors) => EnterContext(new ErrorManager.Context(contextObject, ignoreErrors));

        public static void EnterContext(IErrorContextSource contextSource) => EnterContext(new ErrorManager.Context(contextSource));

        private static void EnterContext(ErrorManager.Context context)
        {
            if (context.IgnoreErrors)
                ++s_ignoringErrorsDepth;
            s_contextStack.Push(context);
        }

        public static string CurrentContext
        {
            get
            {
                string str = null;
                if (s_contextStack.Count != 0)
                    str = s_contextStack.Peek().ToString();
                return str;
            }
        }

        public static bool IgnoringErrors => s_ignoringErrorsDepth > 0U;

        public static void ExitContext()
        {
            ErrorManager.Context context = s_contextStack.Peek();
            if (context.IgnoreErrors)
            {
                s_totalErrorsReported = context.TotalErrorsOnEnter;
                --s_ignoringErrorsDepth;
            }
            s_contextStack.Pop();
        }

        public static uint TotalErrorsReported => s_totalErrorsReported;

        public static event NotifyErrorBatch OnErrors;

        public static IList GetErrors()
        {
            IList errors = s_errors;
            s_errors = null;
            return errors;
        }

        public static void ReportError(string message) => TrackReportWorker(-1, -1, false, message);

        public static void ReportError(string format, object param) => TrackReport(-1, -1, false, format, param);

        public static void ReportError(string format, object param1, object param2) => TrackReport(-1, -1, false, format, param1, param2);

        public static void ReportError(string format, object param1, object param2, object param3) => TrackReport(-1, -1, false, format, param1, param2, param3);

        public static void ReportError(
          string format,
          object param1,
          object param2,
          object param3,
          object param4)
        {
            TrackReport(-1, -1, false, format, param1, param2, param3, param4);
        }

        public static void ReportError(
          string format,
          object param1,
          object param2,
          object param3,
          object param4,
          object param5)
        {
            TrackReport(-1, -1, false, format, param1, param2, param3, param4, param5);
        }

        public static void ReportError(int line, int column, string message) => TrackReportWorker(line, column, false, message);

        public static void ReportError(int line, int column, string format, object param) => TrackReport(line, column, false, format, param);

        public static void ReportError(
          int line,
          int column,
          string format,
          object param1,
          object param2)
        {
            TrackReport(line, column, false, format, param1, param2);
        }

        public static void ReportWarning(string message) => TrackReportWorker(-1, -1, true, message);

        public static void ReportWarning(string format, object param) => TrackReport(-1, -1, true, format, param);

        public static void ReportWarning(string format, object param1, object param2) => TrackReport(-1, -1, true, format, param1, param2);

        public static void ReportWarning(int line, int column, string message) => TrackReportWorker(line, column, true, message);

        public static void ReportWarning(int line, int column, string format, object param) => TrackReport(line, column, true, format, param);

        private static void TrackReportWorker(int line, int column, bool warning, string message)
        {
            if (!IgnoringErrors)
            {
                string str = null;
                if (s_contextStack.Count != 0)
                {
                    ErrorManager.Context context = s_contextStack.Peek();
                    str = context.Description;
                    if (line == -1 && column == -1)
                        context.GetErrorPosition(ref line, ref column);
                }
                ErrorRecord errorRecord = new ErrorRecord();
                errorRecord.Context = str;
                errorRecord.Line = line;
                errorRecord.Column = column;
                errorRecord.Warning = warning;
                errorRecord.Message = message;
                if (s_errors == null)
                    s_errors = new ArrayList();
                s_errors.Add(errorRecord);
                QueueNotify();
            }
            if (warning)
                return;
            ++s_totalErrorsReported;
        }

        public static void TrackReport(
          int line,
          int column,
          bool warning,
          string format,
          object param)
        {
            string message = null;
            if (!IgnoringErrors)
                message = string.Format(format, param);
            TrackReportWorker(line, column, warning, message);
        }

        public static void TrackReport(
          int line,
          int column,
          bool warning,
          string format,
          object param1,
          object param2)
        {
            string message = null;
            if (!IgnoringErrors)
                message = string.Format(format, param1, param2);
            TrackReportWorker(line, column, warning, message);
        }

        public static void TrackReport(
          int line,
          int column,
          bool warning,
          string format,
          object param1,
          object param2,
          object param3)
        {
            string message = null;
            if (!IgnoringErrors)
                message = string.Format(format, param1, param2, param3);
            TrackReportWorker(line, column, warning, message);
        }

        public static void TrackReport(
          int line,
          int column,
          bool warning,
          string format,
          object param1,
          object param2,
          object param3,
          object param4)
        {
            string message = null;
            if (!IgnoringErrors)
                message = string.Format(format, param1, param2, param3, param4);
            TrackReportWorker(line, column, warning, message);
        }

        public static void TrackReport(
          int line,
          int column,
          bool warning,
          string format,
          object param1,
          object param2,
          object param3,
          object param4,
          object param5)
        {
            string message = null;
            if (!IgnoringErrors)
                message = string.Format(format, param1, param2, param3, param4, param5);
            TrackReportWorker(line, column, warning, message);
        }

        private static void QueueNotify()
        {
            if (s_errorBatchPending)
                return;
            UIDispatcher currentDispatcher = UIDispatcher.CurrentDispatcher;
            if (currentDispatcher != null && currentDispatcher.UISession != null)
            {
                s_errorBatchPending = true;
                DeferredCall.Post(DispatchPriority.AppEventHigh, s_drainErrorQueueHandler);
            }
            else
                OnDrainErrorQueue();
        }

        private static void OnDrainErrorQueue()
        {
            s_errorBatchPending = false;
            IList errors = GetErrors();
            if (OnErrors == null)
                return;
            OnErrors(errors);
        }

        internal struct Context
        {
            private object _contextObject;
            private bool _ignoreErrors;
            private uint _errorCountOnEnter;
            private IErrorContextSource _callback;

            public Context(object contextObject, bool ignoreErrors)
            {
                _contextObject = contextObject;
                _callback = null;
                _ignoreErrors = ignoreErrors;
                _errorCountOnEnter = s_totalErrorsReported;
            }

            public Context(IErrorContextSource contextSource)
            {
                _callback = contextSource;
                _contextObject = null;
                _ignoreErrors = false;
                _errorCountOnEnter = s_totalErrorsReported;
            }

            public string Description
            {
                get
                {
                    string str = null;
                    if (_callback != null)
                        str = _callback.GetErrorContextDescription();
                    else if (_contextObject != null)
                    {
                        if (_contextObject is string)
                            str = (string)_contextObject;
                        else if (_contextObject is TypeSchema)
                            str = ((TypeSchema)_contextObject).ErrorContextDescription;
                    }
                    return str;
                }
            }

            public void GetErrorPosition(ref int line, ref int column)
            {
                if (_callback == null)
                    return;
                _callback.GetErrorPosition(ref line, ref column);
            }

            public bool IgnoreErrors => _ignoreErrors;

            public uint TotalErrorsOnEnter => _errorCountOnEnter;

            public override string ToString() => _callback != null ? _callback.ToString() : Description;
        }
    }
}
