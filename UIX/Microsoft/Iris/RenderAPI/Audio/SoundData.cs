// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.RenderAPI.Audio.SoundData
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.Render.Extensions;
using System;

namespace Microsoft.Iris.RenderAPI.Audio
{
    internal class SoundData : ISoundData, IDisposable
    {
        private string _stSource;
        private Resource _soundResource;
        private uint _openCount;
        private bool _fStreamLoading;
        private bool _fStreamAvailable;
        private ExtensionsApi.HSpSound _soundHandle;
        private ExtensionsApi.SoundInformation _soundInfo;

        internal SoundData(string stSource, Resource soundResource)
        {
            _stSource = stSource;
            _soundResource = soundResource;
            _fStreamAvailable = false;
            _fStreamLoading = false;
        }

        ~SoundData() => Dispose(false);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool fInDispose)
        {
            int num = fInDispose ? 1 : 0;
            if (!(_soundHandle != ExtensionsApi.HSpSound.NULL))
                return;
            SoundLoader.DisposeData(_soundHandle, _soundInfo);
            _soundHandle = ExtensionsApi.HSpSound.NULL;
        }

        internal bool IsAvailable => _soundHandle != ExtensionsApi.HSpSound.NULL;

        public bool Load()
        {
            bool flag = false;
            if (_openCount == 0U && !_fStreamAvailable && !_fStreamLoading)
            {
                _fStreamLoading = true;
                _soundResource.Acquire(new ResourceAcquisitionCompleteHandler(OnContentLoadComplete));
            }
            if (_soundHandle != ExtensionsApi.HSpSound.NULL)
                flag = true;
            else if (_fStreamAvailable)
            {
                if (_soundResource.Status == ResourceStatus.Available)
                {
                    ExtensionsApi.HSpSound soundDataHandle;
                    ExtensionsApi.SoundInformation soundDataInfo;
                    SoundLoader.FromMemory(_soundResource.Buffer, (int)_soundResource.Length, out soundDataHandle, out soundDataInfo);
                    _soundHandle = soundDataHandle;
                    _soundInfo = soundDataInfo;
                    flag = true;
                }
            }
            else
                flag = false;
            ++_openCount;
            return flag;
        }

        public void Unload()
        {
            --_openCount;
            if (_openCount != 0U)
                return;
            if (_fStreamAvailable)
            {
                _soundResource.Free(new ResourceAcquisitionCompleteHandler(OnContentLoadComplete));
                _fStreamAvailable = false;
            }
            if (!(_soundHandle != ExtensionsApi.HSpSound.NULL))
                return;
            SoundLoader.DisposeData(_soundHandle, _soundInfo);
            _soundHandle = ExtensionsApi.HSpSound.NULL;
        }

        public static string GetCacheKey(string stSource) => InvariantString.Format("SND|{0}", stSource);

        SoundDataFormat ISoundData.Format => (SoundDataFormat)_soundInfo.Header.wFormatTag;

        uint ISoundData.ChannelCount => _soundInfo.Header.nChannels;

        uint ISoundData.SampleRate => _soundInfo.Header.nSamplesPerSec;

        uint ISoundData.SampleSize => _soundInfo.Header.wBitsPerSample;

        uint ISoundData.SampleCount => _soundInfo.Header.cbDataSize * 8U / _soundInfo.Header.wBitsPerSample;

        IntPtr ISoundData.AcquireContent()
        {
            if (Load())
                return _soundInfo.Data.rgData;
            throw new InvalidOperationException("Sound data isn't available");
        }

        void ISoundData.ReleaseContent() => Unload();

        private void OnContentLoadComplete(Resource resource)
        {
            _fStreamLoading = false;
            if (_soundResource.Status == ResourceStatus.Error)
                _fStreamAvailable = false;
            else
                _fStreamAvailable = true;
        }
    }
}
