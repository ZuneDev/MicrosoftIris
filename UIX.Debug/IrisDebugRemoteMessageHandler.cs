#if ZUNE5

using OwlCore.Remoting;
using OwlCore.Remoting.Transfer;
using OwlCore.Remoting.Transfer.MessageConverters;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Iris.Debug
{
    internal class IrisDebugRemoteMessageHandler : IRemoteMessageHandler
    {
        public RemotingMode Mode { get; set; }
        public MemberSignatureScope MemberSignatureScope { get; set; }

        public IRemoteMessageConverter MessageConverter { get; }

        public bool IsInitialized { get; private set; }

        public event EventHandler<IRemoteMessage> MessageReceived;

        public IrisDebugRemoteMessageHandler(RemotingMode mode)
        {
            // The inbox newtonsoft converter. Tested with various types, structs, primitives, classes and more.
            // Provided by the lib for convenience, the underlying OwlCore.Remoting system doesn't use this property.
            MessageConverter = new NewtonsoftRemoteMessageConverter();

            Mode = mode;
        }

        public Task InitAsync(CancellationToken cancellationToken = default)
        {
            IsInitialized = true;
            return Task.CompletedTask;
        }

        public Task SendMessageAsync(IRemoteMessage memberMessage, CancellationToken? cancellationToken = null)
        {
            // Serialize and send the message.
            return Task.CompletedTask;
        }
    }
}

#endif
