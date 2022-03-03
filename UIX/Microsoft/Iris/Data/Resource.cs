// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Data.Resource
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.OS;
using System;

namespace Microsoft.Iris.Data
{
    internal abstract class Resource
    {
        protected string _uri;
        private bool _forceSynchronous;
        private IntPtr _buffer;
        private uint _length;
        private bool _requiresMemoryFree;
        private ResourceStatus _status;
        private string _errorDetails;
        private int _acquisitions;
        private ResourceAcquisitionCompleteHandler _completeHandlers;

        public Resource(string uri, bool forceSynchronous)
        {
            _uri = uri;
            _forceSynchronous = forceSynchronous;
            _status = ResourceStatus.NeedsAcquire;
        }

        public string Uri => _uri;

        public abstract string Identifier { get; }

        public void Acquire() => Acquire(null);

        public void Acquire(ResourceAcquisitionCompleteHandler completeHandler)
        {
            ++_acquisitions;
            if (completeHandler != null)
                _completeHandlers += completeHandler;
            if (_status == ResourceStatus.Acquiring)
                return;
            if (_status != ResourceStatus.Available)
            {
                _status = ResourceStatus.Acquiring;
                _errorDetails = null;
                StartAcquisition(_forceSynchronous);
            }
            else
            {
                if (completeHandler == null)
                    return;
                completeHandler(this);
            }
        }

        public void Free() => Free(null);

        public void Free(ResourceAcquisitionCompleteHandler completeHandler)
        {
            --_acquisitions;
            if (completeHandler != null)
                _completeHandlers -= completeHandler;
            if (_acquisitions != 0)
                return;
            if (_status == ResourceStatus.Acquiring)
                CancelAcquisition();
            else if (_buffer != IntPtr.Zero)
            {
                if (_requiresMemoryFree)
                    FreeNativeBuffer(_buffer);
                _buffer = IntPtr.Zero;
            }
            _status = ResourceStatus.NeedsAcquire;
        }

        public ResourceStatus Status
        {
            get => _status;
            set => _status = value;
        }

        public string ErrorDetails => _errorDetails;

        public IntPtr Buffer => _buffer;

        public uint Length => _length;

        public bool ForceSynchronous => _forceSynchronous;

        protected abstract void StartAcquisition(bool forceSynchronous);

        protected abstract void CancelAcquisition();

        protected void NotifyAcquisitionComplete(
          IntPtr buffer,
          uint length,
          bool requiresMemoryFree,
          string errorDetails)
        {
            _buffer = buffer;
            _length = length;
            _requiresMemoryFree = requiresMemoryFree;
            if (buffer != IntPtr.Zero)
            {
                _status = ResourceStatus.Available;
            }
            else
            {
                _status = ResourceStatus.Error;
                if (errorDetails == null)
                    errorDetails = string.Format("Failed to acquire resource '{0}'", Identifier);
                _errorDetails = errorDetails;
            }
            if (_completeHandlers == null)
                return;
            _completeHandlers(this);
        }

        protected static IntPtr AllocNativeBuffer(uint length) => NativeApi.MemAlloc(length, false);

        protected static void FreeNativeBuffer(IntPtr buffer) => NativeApi.MemFree(buffer);

        private void FireAcquisitionCompleteHandlers()
        {
            if (_completeHandlers == null)
                return;
            _completeHandlers(this);
            _completeHandlers = null;
        }

        public override string ToString() => _uri;
    }
}
