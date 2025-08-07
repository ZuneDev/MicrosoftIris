using Microsoft.Iris.Data;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

using ResourceManagerCore = System.Resources.ResourceManager;

namespace Microsoft.Iris.OS
{
    internal class ClrDllResource : Resource
    {
        private Assembly _assembly;
        private string _identifier;
        private string _specifier;
        private GCHandle _handle;
        private IntPtr _buffer;
        private uint _length;

        internal ClrDllResource(string uri, Assembly assembly, string identifier, string specifier)
          : base(uri, true)
        {
            _assembly = assembly;
            _identifier = identifier.ToUpperInvariant();
            _specifier = specifier ?? "RCDATA";
        }

        public override string Identifier => _identifier;

        protected override void StartAcquisition(bool forceSynchronous)
        {
            string errorDetails = null;
            if (_buffer == IntPtr.Zero)
            {
                try
                {
                    var rm = new ResourceManagerCore(_specifier, _assembly);

                    var data = (byte[])rm.GetObject(_identifier);

                    _handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                    _buffer = _handle.AddrOfPinnedObject();
                    _length = (uint)data.LongLength;
                }
                catch
                {
                    errorDetails = $"Resource '{_identifier}' not found in {_assembly}, resource '{_specifier}'";
                }
            }
            NotifyAcquisitionComplete(_buffer, _length, false, errorDetails);
        }

        protected override void CancelAcquisition()
        {
        }

        public override string ToString() => _assembly.GetName().Name + "|" + _identifier.ToLowerInvariant();
    }
}
