// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ThreadSafetyBlock
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using System;
using System.Threading;

namespace Microsoft.Iris
{
    public struct ThreadSafetyBlock : IDisposable
    {
        private IThreadSafeObject _safe;
        private Thread _currentThread;
        private bool _error;

        public ThreadSafetyBlock(IThreadSafeObject safe)
        {
            _safe = safe != null ? safe : throw new ArgumentNullException(nameof(safe));
            _currentThread = Thread.CurrentThread;
            _error = false;
            if (_safe.Affinity == _currentThread)
                return;
            if (_currentThread == UIDispatcher.MainUIThread)
                _safe.Affinity = UIDispatcher.MainUIThread;
            else
                ThrowError();
        }

        public void Dispose()
        {
            if (_error || _safe.Affinity == _currentThread)
                return;
            ThrowError();
        }

        private void ThrowError()
        {
            _error = true;
            throw new InvalidOperationException("Access to object occurred on an invalid thread");
        }
    }
}
