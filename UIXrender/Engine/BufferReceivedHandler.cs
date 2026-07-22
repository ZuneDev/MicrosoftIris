using System;
using Microsoft.Iris.Render.Interop;

namespace Microsoft.Iris.Render.Engine;

// A custom (non-generic) delegate so ReadOnlySpan<byte> is legal here -- Span<T> can't
// be an Action<T>/Func<T> type argument (ref structs aren't valid generic arguments),
// but a hand-declared delegate type can take one directly.
public delegate void BufferReceivedHandler(ContextID sourceContext, RENDERHANDLE bufferHandle, BufferFlags flags, ReadOnlySpan<byte> data);
