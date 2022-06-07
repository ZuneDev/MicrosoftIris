using Microsoft.Iris.Debug;

namespace SimpleDebugClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bridge bridge = new(OwlCore.Remoting.RemotingMode.Client);
            while (true) ;
        }
    }
}