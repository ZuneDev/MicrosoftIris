// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.DataObject
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.RenderAPI;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.OS
{
    internal class DataObject : IDisposable
    {
        private IntPtr _dataStream;
        private string[] _data;

        public DataObject(IntPtr dataStream) => _dataStream = dataStream;

        public object GetExternalData()
        {
            object obj = null;
            if (_dataStream != IntPtr.Zero)
            {
                try
                {
                    _data = null;
                    RendererApi.IFC(new HRESULT(NativeApi.SpExtractDroppedFileNames(_dataStream, new NativeApi.ExtractDroppedFileNamesCallback(ExtractDroppedFileNamesCallback))));
                    obj = _data;
                }
                catch (COMException ex)
                {
                }
            }
            _data = null;
            return obj;
        }

        private void ExtractDroppedFileNamesCallback(
          uint totalCount,
          uint currentIndex,
          string filename)
        {
            if (currentIndex == 0U)
                _data = new string[totalCount];
            _data[currentIndex] = filename;
        }

        public void Dispose()
        {
            if (!(_dataStream != IntPtr.Zero))
                return;
            Marshal.Release(_dataStream);
            _dataStream = IntPtr.Zero;
        }
    }
}
