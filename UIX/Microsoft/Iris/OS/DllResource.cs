// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.DllResource
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using System;
using Microsoft.Iris.Debug;

namespace Microsoft.Iris.OS
{
    internal class DllResource : Resource
    {
        private readonly string _dll;
        private readonly string _identifier;
        private IntPtr _buffer;
        private uint _length;

        internal DllResource(string uri, string dll, string identifier)
          : base(uri, true)
        {
            _dll = dll;
            _identifier = identifier;
        }

        public override string Identifier => _identifier;

        protected override void StartAcquisition(bool forceSynchronous)
        {
            string errorDetails = null;
            if (_buffer == IntPtr.Zero)
            {
                var (error, buffer, length) = LoadBinaryResource(_dll, _identifier,
                    !DllResources.StaticDllResourcesOnly);
                if (error is not null)
                {
                    errorDetails = error;
                }
                else
                {
                    _buffer = buffer;
                    _length = length;
                }
            }

            NotifyAcquisitionComplete(_buffer, _length, false, errorDetails);
        }

        protected override void CancelAcquisition()
        {
        }

        public override string ToString() => $"{_dll}|{_identifier.ToLowerInvariant()}";
        
        private static (string error, nint buffer, uint length) LoadBinaryResource(string dllPath, string resourceName, bool allowDynamicResources)
        {
            #if WINDOWS
            
            if (NativeApi.SpLoadBinaryResource(dllPath, resourceName, allowDynamicResources, out var buffer, out var length))
                return (null, buffer, length);
            
            return ($"Resource not found: res://{dllPath}!{resourceName}", 0, 0);
            
            #else
            
            var error = $"Cannot load Win32 resource res://{dllPath}!{resourceName} on non-Windows platforms";
            Trace.WriteLine(TraceCategory.Resource, error);
            return (error, 0, 0);
            
            #endif
        }
    }
}
