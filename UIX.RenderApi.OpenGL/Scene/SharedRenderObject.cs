// In-process, OpenGL-backed implementation of the UIX.RenderApi interfaces.
// See logs/MicrosoftIris/RenderApiOpenGL.md for design notes.

using System.Collections.Generic;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Common base for render objects that participate in the reference-counted
    /// <see cref="ISharedRenderObject"/> lifetime protocol. The original renderer
    /// tracks a set of "users" per object; releasing the last user tears the object
    /// down. We reproduce that behaviour with a simple user set.
    /// </summary>
    public abstract class SharedRenderObject : ISharedRenderObject
    {
        private readonly HashSet<object> m_users = new HashSet<object>();
        private bool m_disposed;

        public int UsageCount => m_users.Count;

        public void RegisterUsage(object user)
        {
            if (user != null)
                m_users.Add(user);
        }

        public void UnregisterUsage(object user)
        {
            if (user != null && m_users.Remove(user) && m_users.Count == 0)
                Dispose();
        }

        protected bool IsDisposed => m_disposed;

        protected void Dispose()
        {
            if (m_disposed)
                return;
            m_disposed = true;
            DisposeCore();
        }

        /// <summary>Release any GPU/native resources. Runs at most once.</summary>
        protected virtual void DisposeCore()
        {
        }
    }
}
