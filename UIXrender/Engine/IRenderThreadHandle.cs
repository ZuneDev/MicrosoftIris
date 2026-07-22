using System;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Engine;

public interface IRenderThreadHandle : IDisposable
{
    ContextID ContextId { get; }
}
