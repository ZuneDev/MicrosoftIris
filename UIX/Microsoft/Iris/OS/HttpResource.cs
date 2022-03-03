// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.HttpResource
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using System;

namespace Microsoft.Iris.OS
{
    internal class HttpResource : Resource
    {
        private IntPtr _handle;
        private NativeApi.DownloadCompleteHandler _pendingCallback;

        public HttpResource(string uri, bool forceSynchronous)
          : base(uri, forceSynchronous)
        {
        }

        public override string Identifier => _uri;

        protected override void StartAcquisition(bool forceSynchronous)
        {
            _pendingCallback = new NativeApi.DownloadCompleteHandler(OnHttpDownloadComplete);
            int num = (int)NativeApi.SpHttpDownload(_uri, _pendingCallback, IntPtr.Zero, out _handle);
        }

        private void OnHttpDownloadComplete(IntPtr handle, int error, uint length, IntPtr context)
        {
            IntPtr buffer = IntPtr.Zero;
            string errorDetails = null;
            switch (error)
            {
                case 0:
                    buffer = NativeApi.DownloadGetBuffer(_handle);
                    break;
                case 1:
                    errorDetails = string.Format("Invalid URI: '{0}'", _uri);
                    break;
                case 2:
                    errorDetails = string.Format("Unable to connect to web host: '{0}'", _uri);
                    break;
                default:
                    errorDetails = string.Format("Failed to complete download from '{0}'", _uri);
                    break;
            }
            int num = (int)NativeApi.SpDownloadClose(_handle);
            _handle = IntPtr.Zero;
            _pendingCallback = null;
            NotifyAcquisitionComplete(buffer, length, true, errorDetails);
        }

        protected override void CancelAcquisition()
        {
            if (!(_handle != IntPtr.Zero))
                return;
            int num = (int)NativeApi.SpDownloadClose(_handle);
            _handle = IntPtr.Zero;
            _pendingCallback = null;
        }
    }
}
