#if OPENZUNE

using OwlCore.Remoting;
using System;

namespace Microsoft.Iris.Debug
{
    [RemoteProperty]
    [RemoteMethod]
    [RemoteOptions(RemotingDirection.Bidirectional)]
    public class Bridge : IBridge, IDisposable
    {
        private readonly MemberRemote _memberRemote;

        public event EventHandler<Data.InterpreterEntry> InterpreterStep;
        public event Action<string> DispatcherStep;

        public Bridge(RemotingMode mode)
        {
            // Pass the instance you want to remote into MemberRemote() with an ID that is identical on both machines for that instance.
            // An instance will not receive member changes until you do this.
            // Optionally leave out the message handler. Uses the default set by MemberRemote.SetDefaultMessageHandler(handler);
            IrisDebugRemoteMessageHandler handler = new(mode);
            _memberRemote = new MemberRemote(this, typeof(Bridge).Assembly.FullName, handler);
        }

        [RemoteMethod, RemoteOptions(RemotingDirection.HostToClient)]
        public void LogInterpreterOpCode(object context, Data.InterpreterEntry entry)
        {
            InterpreterStep?.Invoke(context, new(entry));
        }

        [RemoteMethod, RemoteOptions(RemotingDirection.HostToClient)]
        public void LogDispatcher(string message)
        {
            DispatcherStep?.Invoke(message);
        }

        public void Dispose()
        {
            // Dispose of the MemberRemote when finished. Forgetting to do this WILL result in a memory leak.
            _memberRemote.Dispose();
        }
    }
}

#endif
