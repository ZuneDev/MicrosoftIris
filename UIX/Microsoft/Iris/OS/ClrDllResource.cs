using Microsoft.Iris.Data;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

using ResourceManagerCore = System.Resources.ResourceManager;

namespace Microsoft.Iris.OS
{
    internal class ClrDllResource : Resource
    {
        private ResourceManagerCore _rm;
        private Assembly _assembly;
        private string _identifier;
        private string _specifier;
        private GCHandle _handle;
        private IntPtr _buffer;
        private uint _length;

        internal ClrDllResource(string uri, ResourceManagerCore rm, Assembly assembly, string identifier, string specifier)
          : base(uri, true)
        {
            _rm = rm;
            _assembly = assembly;
            _identifier = identifier.ToUpperInvariant();
            _specifier = specifier;
        }

        public override string Identifier => _identifier;

        protected override void StartAcquisition(bool forceSynchronous)
        {
            string errorDetails = null;
            if (_buffer == IntPtr.Zero)
            {
                var error = true;
                try
                {
                    var data = _rm.GetObject(_identifier) as byte[];

                    if (data is not null)
                    {
                        _handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                        _buffer = _handle.AddrOfPinnedObject();
                        _length = (uint)data.LongLength;

                        error = false;
                    }
                }
                catch
                {
                    error = true;
                }

                if (error)
                    errorDetails = $"Resource '{_identifier}' not found in {_assembly}, resource '{_specifier}'";
            }
            NotifyAcquisitionComplete(_buffer, _length, false, errorDetails);
        }

        protected override void CancelAcquisition()
        {
        }

        public override string ToString() => _assembly.GetName().Name + "|" + _identifier.ToLowerInvariant();
    }
}
