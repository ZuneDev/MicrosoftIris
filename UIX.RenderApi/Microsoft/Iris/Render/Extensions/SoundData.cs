// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.SoundData
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render.Extensions
{
    public class SoundData : ISoundData, IDisposable
    {
        private string m_moduleName;
        private string m_resourceId;
        private int m_usageCount;
        private bool m_hasContent;
        private ExtensionsApi.HSpSound m_soundHandle;
        private ExtensionsApi.SoundInformation m_soundInfo;

        public SoundData(string moduleName, string resourceId)
        {
            Debug2.Validate(moduleName != null, typeof(ArgumentNullException), nameof(moduleName));
            Debug2.Validate(resourceId != null, typeof(ArgumentNullException), nameof(resourceId));
            this.m_moduleName = moduleName;
            this.m_resourceId = resourceId;
            this.m_usageCount = 0;
            this.m_hasContent = false;
            this.LoadSoundData();
        }

        ~SoundData() => this.Dispose(false);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        protected virtual void Dispose(bool fInDispose)
        {
            if (!this.m_hasContent)
                return;
            this.UnloadSoundData();
        }

        SoundDataFormat ISoundData.Format => SoundDataFormat.PCM;

        uint ISoundData.ChannelCount => m_soundInfo.Header.nChannels;

        uint ISoundData.SampleRate => this.m_soundInfo.Header.nSamplesPerSec;

        uint ISoundData.SampleSize => m_soundInfo.Header.wBitsPerSample;

        uint ISoundData.SampleCount => (uint)(m_soundInfo.Header.cbDataSize / (ulong)(m_soundInfo.Header.wBitsPerSample / 8));

        IntPtr ISoundData.AcquireContent()
        {
            ++this.m_usageCount;
            return this.m_soundInfo.Data.rgData;
        }

        void ISoundData.ReleaseContent() => --this.m_usageCount;

        private void LoadSoundData()
        {
            if (this.m_hasContent)
                return;
            ExtensionsApi.HSpSound soundDataHandle;
            ExtensionsApi.SoundInformation soundDataInfo;
            SoundLoader.FromResource(this.m_moduleName, this.m_resourceId, out soundDataHandle, out soundDataInfo);
            Debug2.Validate(soundDataInfo.Data.rgData != IntPtr.Zero, typeof(InvalidOperationException), "Failed to load data and no exception was thrown");
            Debug2.Validate(soundDataInfo.Header.wFormatTag == 1, typeof(ArgumentException), "Only PCM sound data is currently supported");
            Debug2.Validate(soundDataInfo.Header.nChannels >= 1 && soundDataInfo.Header.nChannels <= 2, typeof(ArgumentException), "Only mono/stereo sound data is currently supported");
            Debug2.Validate(soundDataInfo.Header.wBitsPerSample == 8 || soundDataInfo.Header.wBitsPerSample == 16, typeof(ArgumentException), "Only 8bps/16bps sound data is currently supported");
            Debug2.Validate((uint)(soundDataInfo.Header.cbDataSize / (ulong)(soundDataInfo.Header.wBitsPerSample / 8)) % soundDataInfo.Header.nChannels == 0U, typeof(ArgumentException), "Insufficient sample data");
            this.m_soundHandle = soundDataHandle;
            this.m_soundInfo = soundDataInfo;
            this.m_hasContent = true;
        }

        private void UnloadSoundData()
        {
            if (!this.m_hasContent)
                return;
            this.m_hasContent = false;
            SoundLoader.DisposeData(this.m_soundHandle, this.m_soundInfo);
        }
    }
}
