#if OPENZUNE

using System;

namespace Microsoft.Iris.Debug
{
    public class Bridge : IBridge
    {
        public event EventHandler<Data.InterpreterEntry> InterpreterStep;
        public event Action<string> DispatcherStep;

        public void LogInterpreterOpCode(object context, Data.InterpreterEntry entry)
        {
            InterpreterStep?.Invoke(context, entry);
        }

        public void LogDispatcher(string message)
        {
            DispatcherStep?.Invoke(message);
        }
    }
}

#endif
