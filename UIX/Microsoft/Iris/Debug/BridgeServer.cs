using CoreRemoting;
using CoreRemoting.DependencyInjection;
using CoreRemoting.Serialization.Binary;
using System;

namespace Microsoft.Iris.Debug;

internal class BridgeServer : IBridge, IDisposable
{
    private static RemotingServer _server;
    
    public static IBridge Current { get; private set; }

    public event EventHandler<Data.InterpreterEntry> InterpreterStep;
    public event Action<string> DispatcherStep;

    public static void Start(ServerConfig config)
    {
        _server = new RemotingServer(new ServerConfig
        {
            HostName = "localhost",
            NetworkPort = 9090,
            MessageEncryption = false,
            Serializer = new BinarySerializerAdapter(),
            RegisterServicesAction = container =>
            {
                container.RegisterService<IBridge, BridgeServer>(ServiceLifetime.Singleton);
            }
        });

        Current = _server.ServiceRegistry.GetService<IBridge>();

        _server.Error += OnServerError;
        _server.BeforeCall += (_, ctx) =>
        {

        };

        _server.Start();
    }

    private static void OnServerError(object sender, Exception e)
    {

    }

    public void LogInterpreterOpCode(object context, Data.InterpreterEntry entry)
    {
        InterpreterStep?.Invoke(context, entry);
    }

    public void LogDispatcher(string message)
    {
        DispatcherStep?.Invoke(message);
    }

    public void Dispose() => _server.Dispose();
}
