using Microsoft.Iris.Debug;

namespace SimpleDebugClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bridge bridge = new(OwlCore.Remoting.RemotingMode.Client);

            Console.WriteLine("Listening for debug messages. Press ENTER to exit.");
            Console.ReadLine();
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