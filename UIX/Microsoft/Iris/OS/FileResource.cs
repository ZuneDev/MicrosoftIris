// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.FileResource
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using System;

namespace Microsoft.Iris.OS
{
    internal class FileResource : Resource
    {
        private string _filePath;
        private IntPtr _handle;
        private NativeApi.DownloadCompleteHandler _pendingCallback;

        public FileResource(string uri, string filePath, bool forceSynchronous)
          : base(uri, forceSynchronous)
          => _filePath = filePath;

        public override string Identifier => _filePath;

        protected override void StartAcquisition(bool forceSynchronous)
        {
            if (forceSynchronous)
                SynchronousDownload();
            else
                AsynchronousDownload();
        }

        private void AsynchronousDownload()
        {
            _pendingCallback = new NativeApi.DownloadCompleteHandler(OnFileDownloadComplete);
            int num = (int)NativeApi.SpFileDownload(_filePath, _pendingCallback, IntPtr.Zero, out _handle);
        }

        private void OnFileDownloadComplete(IntPtr handle, int error, uint length, IntPtr context)
        {
            IntPtr buffer = IntPtr.Zero;
            string errorDetails = null;
            if (error == 0)
                buffer = NativeApi.DownloadGetBuffer(_handle);
            else
                errorDetails = string.Format("Failed to complete download from '{0}'", _filePath);
            int num = (int)NativeApi.SpDownloadClose(_handle);
            _handle = IntPtr.Zero;
            _pendingCallback = null;
            NotifyAcquisitionComplete(buffer, length, true, errorDetails);
        }

        private void SynchronousDownload()
        {
            IntPtr num1 = IntPtr.Zero;
            uint num2 = 0;
            string errorDetails = null;
            IntPtr file = Win32Api.CreateFile(_filePath, 2147483648U, 1U, IntPtr.Zero, 3U, 0U, IntPtr.Zero);
            if (file == Win32Api.INVALID_HANDLE_VALUE)
            {
                errorDetails = string.Format("File not found: '{0}'", _filePath);
            }
            else
            {
                num2 = Win32Api.GetFileSize(file, IntPtr.Zero);
                if (num2 != uint.MaxValue)
                {
                    num1 = AllocNativeBuffer(num2);
                    uint lpNumberOfBytesRead;
                    if (!(num1 == IntPtr.Zero) && (!Win32Api.ReadFile(file, num1, num2, out lpNumberOfBytesRead, IntPtr.Zero) || (int)lpNumberOfBytesRead != (int)num2))
                    {
                        FreeNativeBuffer(num1);
                        num1 = IntPtr.Zero;
                    }
                }
            }
            if (file != IntPtr.Zero)
                Win32Api.CloseHandle(file);
            NotifyAcquisitionComplete(num1, num2, true, errorDetails);
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
