using Microsoft.Iris.Debug;

namespace SimpleDebugClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bridge bridge = new(OwlCore.Remoting.RemotingMode.Client);
            bridge.DispatcherStep += Bridge_DispatcherStep;
            bridge.InterpreterStep += Bridge_InterpreterStep;
        }

        private static void Bridge_DispatcherStep(string obj)
        {
            Console.WriteLine(obj);
        }

        private static void Bridge_InterpreterStep(object? sender, Microsoft.Iris.Debug.Data.InterpreterEntry e)
        {
            Console.WriteLine(e);
        }
    }
}