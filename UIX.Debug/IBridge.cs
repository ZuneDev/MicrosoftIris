namespace Microsoft.Iris.Debug
{
    internal interface IBridge
    {
        /// <summary>
        /// Logs the context, opcode, and arguments of an instruction
        /// executed by <c>Microsoft.Iris.Markup.Interpreter</c>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entry"></param>
        public void LogInterpreterOpCode(object context, Data.InterpreterEntry entry);

        public void LogDispatcher(string message);
    }
}
