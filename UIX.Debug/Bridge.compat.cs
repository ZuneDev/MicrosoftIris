#if !ZUNE5

using Microsoft.Iris.Debug.Data;

namespace Microsoft.Iris.Debug
{
    /// <summary>
    /// A dummy implementation of <see cref="IBridge"/>, used to avoid conditional
    /// code in consuming libraries.
    /// </summary>
    public class Bridge : IBridge
    {
        public void LogDispatcher(string message)
        {
            
        }

        public void LogInterpreterOpCode(object context, InterpreterEntry entry)
        {
            
        }
    }
}

#endif
