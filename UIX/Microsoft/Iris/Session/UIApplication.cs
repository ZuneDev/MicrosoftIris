// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Session.UIApplication
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using System;
using System.Threading;

namespace Microsoft.Iris.Session
{
    internal class UIApplication
    {
        private static string s_applicationName = "UIX Client";

        public static string ApplicationName
        {
            get => s_applicationName;
            set => s_applicationName = value;
        }

        public static void Run() => UIDispatcher.CurrentDispatcher.Run(null);

        public static void DoEvents(LoopCondition loop) => UIDispatcher.CurrentDispatcher.Run(loop);

        public static void DoEvents(LoopCondition loop, UISession session, bool focusCanBeNullFlag)
        {
            InputManager inputManager = session.InputManager;
            bool keyFocusCanBeNull = inputManager.KeyFocusCanBeNull;
            inputManager.KeyFocusCanBeNull = focusCanBeNullFlag;
            try
            {
                DoEvents(loop);
            }
            finally
            {
                inputManager.KeyFocusCanBeNull = keyFocusCanBeNull;
            }
        }

        public static void DoWin32Events(LoopCondition loop) => UIDispatcher.CurrentDispatcher.RPCYield(loop);

        public static void DeferredInvokeOnWorkerThread(
          DeferredHandler workerMethod,
          DeferredHandler notifyMethod,
          object args)
        {
            new UIApplication.AsyncInvokeHelper(workerMethod, notifyMethod, args).BeginInvoke();
        }

        public static Thread StartThreadWithDispatcher(string threadName)
        {
            ManualResetEvent registeredEvent = new ManualResetEvent(false);
            Thread thread = new Thread(new ParameterizedThreadStart(StartDispatcher));
            thread.Name = threadName;
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start(new UIApplication.StartArgs(registeredEvent));
            registeredEvent.WaitOne();
            registeredEvent.Close();
            return thread;
        }

        public static void StartDispatcher(object argsObject)
        {
            UIApplication.StartArgs startArgs = (UIApplication.StartArgs)argsObject;
            Thread currentThread = Thread.CurrentThread;
            using (UIDispatcher uiDispatcher = new UIDispatcher(false))
            {
                if (startArgs.registeredEvent != null)
                {
                    startArgs.registeredEvent.Set();
                    startArgs.registeredEvent = null;
                }
                if (startArgs.initialWork != null)
                {
                    DeferredCall.Post(currentThread, DispatchPriority.AppEvent, startArgs.initialWork, startArgs.initialWorkArgs);
                    startArgs.initialWork = null;
                    startArgs.initialWorkArgs = null;
                }
                uiDispatcher.Run(null);
            }
        }

        private class AsyncInvokeHelper
        {
            private object _args;
            private DeferredHandler _worker;
            private DeferredHandler _notify;

            public AsyncInvokeHelper(
              DeferredHandler workerMethod,
              DeferredHandler notifyMethod,
              object args)
            {
                _args = args;
                _notify = notifyMethod;
                _worker = workerMethod;
            }

            public void BeginInvoke() => _worker.BeginInvoke(_args, new AsyncCallback(AsyncInvokeCompleted), null);

            private void AsyncInvokeCompleted(IAsyncResult result)
            {
                _worker.EndInvoke(result);
                DeferredCall.Post(DispatchPriority.Idle, _notify, _args);
            }
        }

        internal class StartArgs
        {
            public ManualResetEvent registeredEvent;
            public DeferredHandler initialWork;
            public object initialWorkArgs;

            public StartArgs(ManualResetEvent registeredEvent) => this.registeredEvent = registeredEvent;

            public StartArgs(DeferredHandler initialWork, object initialWorkArgs)
            {
                this.initialWork = initialWork;
                this.initialWorkArgs = initialWorkArgs;
            }
        }
    }
}
