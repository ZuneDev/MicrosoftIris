using Microsoft.Iris.Data;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.OS
{
    internal class ClrDllResource : Resource
    {
        private Assembly _assembly;
        private string _identifier;
        private GCHandle _handle;
        private IntPtr _buffer;
        private uint _length;

        internal ClrDllResource(string uri, Assembly assembly, string identifier)
          : base(uri, true)
        {
            _assembly = assembly;
            _identifier = identifier;
        }

        public override string Identifier => _identifier;

        protected override void StartAcquisition(bool forceSynchronous)
        {
            string errorDetails = null;
            if (_buffer == IntPtr.Zero)
            {
                try
                {
                    var resName = _assembly.GetManifestResourceNames()
                        .First(s => s.EndsWith("." + _identifier, StringComparison.OrdinalIgnoreCase));

                    using System.IO.Stream fs = _assembly.GetManifestResourceStream(resName);
                    var data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);

                    _handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                    _buffer = _handle.AddrOfPinnedObject();
                    _length = (uint)data.LongLength;
                }
                catch
                {
                    errorDetails = $"Resource not found: {ClrDllResources.Scheme}://{_assembly.Location}!{_identifier}";
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
