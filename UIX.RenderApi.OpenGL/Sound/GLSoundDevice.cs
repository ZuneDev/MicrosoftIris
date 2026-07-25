namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Silent sound device. Volume/mute state is tracked so callers behave, but no audio
    /// is produced. Stage-3 TODO: back this with Silk.NET.OpenAL for cross-platform audio.
    /// </summary>
    public sealed class GLSoundDevice : ISoundDevice
    {
        public GLSoundDevice(SoundDeviceType deviceType) => DeviceType = deviceType;

        public SoundDeviceType DeviceType { get; }
        public bool Mute { get; set; }
        public float Volume { get; set; } = 1f;

        public ISoundBuffer CreateSoundBuffer(object objUser, ISoundData soundData)
            => new GLSoundBuffer(soundData);
    }

    public sealed class GLSoundBuffer : SharedRenderObject, ISoundBuffer
    {
        private readonly ISoundData m_data;

        public GLSoundBuffer(ISoundData data) => m_data = data;

        public ISound CreateSound(object objUser) => new GLSound();
    }

    public sealed class GLSound : SharedRenderObject, ISound
    {
        // TODO(stage 3): drive an OpenAL source. No-op keeps the UI sound calls safe.
        public void Play() { }
        public void Stop() { }
    }
}
