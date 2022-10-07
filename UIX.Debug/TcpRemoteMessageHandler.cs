#if OPENZUNE

using OwlCore.Remoting;
using OwlCore.Remoting.Transfer;
using OwlCore.Remoting.Transfer.MessageConverters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Iris.Debug
{
    internal class TcpRemoteMessageHandler : IRemoteMessageHandler, IDisposable
    {
        // https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example
        // ManualResetEvent instances signal completion.  
        private ManualResetEvent connectDone = new(false);
        private ManualResetEvent sendDone = new(false);
        private ManualResetEvent receiveDone = new(false);

        private Socket _socket;

        public RemotingMode Mode { get; set; }
        public MemberSignatureScope MemberSignatureScope { get; set; }

        public IRemoteMessageConverter MessageConverter { get; }

        public bool IsInitialized { get; private set; }

        public string IP { get; private set; }

        public int Port { get; private set; }

        public event EventHandler<IRemoteMessage> MessageReceived;

        public TcpRemoteMessageHandler(RemotingMode mode, string ip = "127.0.0.1", int port = 13000)
        {
            // The inbox newtonsoft converter. Tested with various types, structs, primitives, classes and more.
            // Provided by the lib for convenience, the underlying OwlCore.Remoting system doesn't use this property.
            MessageConverter = new NewtonsoftRemoteMessageConverter();

            Mode = mode;

            IP = ip;
            Port = port;
        }

        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            // Establish the remote endpoint for the socket.
            var ip = IPAddress.Parse(IP);

            // Create a TCP/IP socket.  
            _socket = new(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            if (Mode == RemotingMode.Client)
            {
                // Connect to the remote endpoint.
                _socket.BeginConnect(ip, Port, ConnectCallback, _socket);
            }

            Receive(_socket);

            IsInitialized = true;
        }

        public async Task SendMessageAsync(IRemoteMessage memberMessage, CancellationToken? cancellationToken = null)
        {
            // Serialize and send the message.
            byte[] data = await MessageConverter.SerializeAsync(memberMessage, cancellationToken);
            cancellationToken?.ThrowIfCancellationRequested();

            // Begin sending the data to the remote device.
            _socket.BeginSend(data, 0, data.Length, 0, SendCallback, _socket);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            // Retrieve the socket from the state object.  
            Socket client = (Socket)result.AsyncState;

            // Complete the connection.  
            client.EndConnect(result);

            Console.WriteLine($"Socket connected to {client.RemoteEndPoint}");

            // Signal that the connection has been made.  
            connectDone.Set();
        }

        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new()
                {
                    WorkSocket = client
                };

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private async void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.WorkSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.Data.AddRange(state.Buffer);

                    // Get the rest of the data.  
                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                }
                else
                {
                    // Signal that all bytes have been received.
                    receiveDone.Set();

                    // Deserialize message.
                    var msg = await MessageConverter.DeserializeAsync(state.Data.ToArray());
                    MessageReceived?.Invoke(this, msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine($"Sent {bytesSent} bytes to server.");

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Dispose()
        {
            _socket?.Shutdown(SocketShutdown.Both);
            _socket?.Close();
        }

        /// <summary>
        /// State object for receiving data from remote device.
        /// </summary>
        private class StateObject
        {
            /// <summary>
            /// Client socket.
            /// </summary>
            public Socket WorkSocket = null;

            /// <summary>
            /// Size of receive buffer.
            /// </summary>
            public const int BufferSize = 256;

            /// <summary>
            /// Receive buffer.
            /// </summary>
            public byte[] Buffer = new byte[BufferSize];

            /// <summary>
            /// Total recieved data.
            /// </summary>
            public List<byte> Data = new(BufferSize);
        }
    }
}

#endif
