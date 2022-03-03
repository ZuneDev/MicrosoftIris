// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Library.DisposableObject
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Diagnostics;

namespace Microsoft.Iris.Library
{
    internal class DisposableObject : IDisposableObject
    {
        private bool _isDisposed;

        protected DisposableObject()
        {
        }

        public bool IsDisposed => _isDisposed;

        public void DeclareOwner(object owner) => OnOwnerDeclared(owner);

        public void TransferOwnership(object owner) => OnOwnerDeclared(owner);

        protected virtual void OnOwnerDeclared(object owner)
        {
        }

        public void Dispose(object owner) => OnDispose();

        protected virtual void OnDispose() => _isDisposed = true;

        [Conditional("DEBUG")]
        public void DEBUG_DeclareDisposeUnnecessary()
        {
        }
    }
}
