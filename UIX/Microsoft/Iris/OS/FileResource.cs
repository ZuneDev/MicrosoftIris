// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.FileResource
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using System;
#if !WINDOWS
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Iris.Session;
#endif

namespace Microsoft.Iris.OS
{
    internal class FileResource : Resource
    {
        private string _filePath;
#if WINDOWS
        private IntPtr _handle;
        private NativeApi.DownloadCompleteHandler _pendingCallback;
#else
        private CancellationTokenSource _cancellation;
#endif

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
#if WINDOWS
            _pendingCallback = new NativeApi.DownloadCompleteHandler(OnFileDownloadComplete);
            int num = (int)NativeApi.SpFileDownload(_filePath, _pendingCallback, IntPtr.Zero, out _handle);
#else
            // UIXRender.dll's async download queue is Windows-only; there's no
            // platform-specific reason a file read needs native help, so this reads
            // in the background via managed I/O and posts the result back to the
            // thread that requested it, matching the callback's original threading.
            var cancellation = new CancellationTokenSource();
            _cancellation = cancellation;
            Thread callingThread = Thread.CurrentThread;
            Task.Run(() =>
            {
                var (buffer, length, errorDetails) = ReadFileToNativeBuffer(_filePath);
                if (cancellation.IsCancellationRequested)
                {
                    if (buffer != IntPtr.Zero)
                        FreeNativeBuffer(buffer);
                    return;
                }
                DeferredCall.Post(callingThread, DispatchPriority.Normal, () =>
                {
                    _cancellation = null;
                    NotifyAcquisitionComplete(buffer, length, true, errorDetails);
                });
            }, cancellation.Token);
#endif
        }

#if WINDOWS
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
#endif

        private void SynchronousDownload()
        {
#if WINDOWS
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
#else
            var (buffer, length, errorDetails) = ReadFileToNativeBuffer(_filePath);
            NotifyAcquisitionComplete(buffer, length, true, errorDetails);
#endif
        }

#if !WINDOWS
        private static (IntPtr buffer, uint length, string errorDetails) ReadFileToNativeBuffer(string filePath)
        {
            if (!File.Exists(filePath))
                return (IntPtr.Zero, 0, string.Format("File not found: '{0}'", filePath));

            try
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                uint length = (uint)bytes.Length;
                IntPtr buffer = AllocNativeBuffer(length);
                Marshal.Copy(bytes, 0, buffer, bytes.Length);
                return (buffer, length, null);
            }
            catch (IOException ex)
            {
                return (IntPtr.Zero, 0, string.Format("Failed to read file '{0}': {1}", filePath, ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return (IntPtr.Zero, 0, string.Format("Failed to read file '{0}': {1}", filePath, ex.Message));
            }
        }
#endif

        protected override void CancelAcquisition()
        {
#if WINDOWS
            if (!(_handle != IntPtr.Zero))
                return;
            int num = (int)NativeApi.SpDownloadClose(_handle);
            _handle = IntPtr.Zero;
            _pendingCallback = null;
#else
            _cancellation?.Cancel();
            _cancellation = null;
#endif
        }
    }
}
