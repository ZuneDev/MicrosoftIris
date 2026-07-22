using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Iris.Interop;
using Microsoft.Iris.Render.Engine;

namespace Microsoft.Iris.Render.Subsystems.Os;

// [UnmanagedCallersOnly] exports for the download/HTTP family in
// UIX/Microsoft/Iris/OS/NativeApi.cs.
//
// Real and cross-platform: HttpClient for the network path, FileStream for the local one.
// Both are genuinely asynchronous like the original (the managed caller passes a
// DownloadCompleteHandler and gets called back on completion), and both hand the
// completed bytes over as an unmanaged buffer the caller reads via SpDownloadGetBuffer
// and releases via SpDownloadClose.
//
// The error codes are not invented -- NativeApi.cs declares them as public constants:
// DOWNLOAD_ERROR_NONE = 0, DOWNLOAD_ERROR_GENERALFAILURE = -1,
// HTTPDOWNLOAD_ERROR_INVALIDURI = 1, HTTPDOWNLOAD_ERROR_HOSTCONNECTIONFAILED = 2.
//
// Note the class is not `unsafe` as a whole: C# forbids `await` inside an unsafe context,
// so the async download body lives in ordinary code and only the final callback
// invocation (which needs a function pointer) is marked unsafe.
public static class DownloadApi
{
    private const int DOWNLOAD_ERROR_NONE = 0;
    private const int DOWNLOAD_ERROR_GENERALFAILURE = -1;
    private const int HTTPDOWNLOAD_ERROR_INVALIDURI = 1;
    private const int HTTPDOWNLOAD_ERROR_HOSTCONNECTIONFAILED = 2;

    private static readonly Lazy<HttpClient> s_httpClient = new(() => new HttpClient());
    private static int s_httpStarted;

    private sealed class Download : IDisposable
    {
        public IntPtr Buffer;
        public uint Length;

        public void Dispose()
        {
            if (Buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(Buffer);
                Buffer = IntPtr.Zero;
            }
        }
    }

    [UnmanagedCallersOnly(EntryPoint = "SpHttpStartup")]
    public static void SpHttpStartup() => Interlocked.Exchange(ref s_httpStarted, 1);

    [UnmanagedCallersOnly(EntryPoint = "SpHttpShutdown")]
    public static void SpHttpShutdown() => Interlocked.Exchange(ref s_httpStarted, 0);

    // HttpClient manages its own connection pool and exposes no proxy-cache flush, so
    // there is nothing to invalidate here. Kept as a real (empty) implementation rather
    // than a failure return: the original's callers treat this as advisory.
    [UnmanagedCallersOnly(EntryPoint = "SpHttpFlushProxyCache")]
    public static void SpHttpFlushProxyCache() { }

    [UnmanagedCallersOnly(EntryPoint = "SpFileDownload")]
    public static unsafe uint SpFileDownload(char* path, IntPtr handler, IntPtr context, IntPtr* handle)
    {
        if (handle == null)
            return unchecked((uint)DOWNLOAD_ERROR_GENERALFAILURE);

        string filePath = NativeString.UniToString(path);
        var download = new Download();
        IntPtr downloadHandle = HandleTable.Alloc(download);
        *handle = downloadHandle;

        RunDownload(() => File.ReadAllBytesAsync(filePath), download, downloadHandle, handler, context, static _ => DOWNLOAD_ERROR_GENERALFAILURE);
        return DOWNLOAD_ERROR_NONE;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpHttpDownload")]
    public static unsafe uint SpHttpDownload(char* url, IntPtr handler, IntPtr context, IntPtr* handle)
    {
        if (handle == null)
            return unchecked((uint)DOWNLOAD_ERROR_GENERALFAILURE);

        *handle = IntPtr.Zero;
        string address = NativeString.UniToString(url);
        if (!Uri.TryCreate(address, UriKind.Absolute, out Uri uri))
            return HTTPDOWNLOAD_ERROR_INVALIDURI;

        var download = new Download();
        IntPtr downloadHandle = HandleTable.Alloc(download);
        *handle = downloadHandle;

        RunDownload(() => s_httpClient.Value.GetByteArrayAsync(uri), download, downloadHandle, handler, context,
            static e => e is HttpRequestException ? HTTPDOWNLOAD_ERROR_HOSTCONNECTIONFAILED : DOWNLOAD_ERROR_GENERALFAILURE);

        return DOWNLOAD_ERROR_NONE;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpDownloadGetBuffer")]
    public static IntPtr SpDownloadGetBuffer(IntPtr handle) =>
        HandleTable.TryGet(handle, out Download download) ? download.Buffer : IntPtr.Zero;

    [UnmanagedCallersOnly(EntryPoint = "SpDownloadClose")]
    public static uint SpDownloadClose(IntPtr handle)
    {
        HandleTable.Free(handle);
        return DOWNLOAD_ERROR_NONE;
    }

    // Shared completion path for both download flavours: run the fetch off-thread, copy
    // the result into an unmanaged buffer the caller can read, then invoke the caller's
    // DownloadCompleteHandler(handle, error, length, context).
    private static void RunDownload(Func<Task<byte[]>> fetch, Download download, IntPtr downloadHandle, IntPtr handler, IntPtr context, Func<Exception, int> classify)
    {
        _ = Task.Run(async () =>
        {
            int error = DOWNLOAD_ERROR_NONE;
            try
            {
                byte[] bytes = await fetch().ConfigureAwait(false);
                IntPtr buffer = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, buffer, bytes.Length);
                download.Buffer = buffer;
                download.Length = (uint)bytes.Length;
            }
            catch (Exception e)
            {
                error = classify(e);
            }

            InvokeCompletionHandler(handler, downloadHandle, error, download.Length, context);
        });
    }

    private static unsafe void InvokeCompletionHandler(IntPtr handler, IntPtr downloadHandle, int error, uint length, IntPtr context)
    {
        if (handler == IntPtr.Zero)
            return;

        var callback = (delegate* unmanaged<IntPtr, int, uint, IntPtr, void>)handler;
        callback(downloadHandle, error, length, context);
    }
}
