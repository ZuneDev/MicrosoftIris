// In-process, OpenGL-backed implementation of the UIX.RenderApi interfaces.
// See logs/MicrosoftIris/RenderApiOpenGL.md for design notes.

using System.Collections.Generic;

namespace Microsoft.Iris.Render.OpenGL.Scene
{
    /// <summary>
    /// Common base for render objects that participate in the reference-counted
    /// <see cref="ISharedRenderObject"/> lifetime protocol. The original renderer
    /// tracks a set of "users" per object; releasing the last user tears the object
    /// down. We reproduce that behaviour with a simple user set.
    /// </summary>
    public abstract class SharedRenderObject : ISharedRenderObject
    {
        // Shared with everything else reachable from a render frame (the owning
        // GLRenderSession's SyncRoot, for objects in the scene/animation graph the
        // render thread walks) so usage-count mutation and disposal are safe now
        // that the app thread and the render thread can both reach the same object
        // graph. Objects outside that graph (e.g. sound) don't need to share a lock
        // with anything, so they fall back to a private one via the parameterless
        // constructor.
        protected readonly object SyncRoot;

        private readonly HashSet<object> m_users = new HashSet<object>();
        private bool m_disposed;

        protected SharedRenderObject() : this(new object())
        {
        }

        protected SharedRenderObject(object syncRoot)
        {
            SyncRoot = syncRoot;
        }

        public int UsageCount { get { lock (SyncRoot) return m_users.Count; } }

        public void RegisterUsage(object user)
        {
            if (user == null)
                return;
            lock (SyncRoot)
                m_users.Add(user);
        }

        public void UnregisterUsage(object user)
        {
            if (user == null)
                return;
            // Dispose() re-enters this same lock -- fine, Monitor is reentrant on
            // the thread that already holds it.
            lock (SyncRoot)
            {
                if (m_users.Remove(user) && m_users.Count == 0)
                    Dispose();
            }
        }

        protected bool IsDisposed { get { lock (SyncRoot) return m_disposed; } }

        protected void Dispose()
        {
            lock (SyncRoot)
            {
                if (m_disposed)
                    return;
                m_disposed = true;
                DisposeCore();
            }
        }

        /// <summary>Release any GPU/native resources. Runs at most once.</summary>
        protected virtual void DisposeCore()
        {
        }
    }
}
