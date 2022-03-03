// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocols.Splash.Rendering.ProtocolSplashRendering
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Messaging;
using System;

namespace Microsoft.Iris.Render.Protocols.Splash.Rendering
{
    internal sealed class ProtocolSplashRendering : ProtocolInstance
    {
        private RENDERHANDLE _priv_remoteClass_Window;
        private RENDERHANDLE _priv_remoteClass_RenderCaps;
        private RENDERHANDLE _priv_remoteClass_Visual;
        private RENDERHANDLE _priv_remoteClass_Camera;
        private RENDERHANDLE _priv_remoteClass_VisualContainer;
        private RENDERHANDLE _priv_remoteClass_Device;
        private RENDERHANDLE _priv_remoteClass_Surface;
        private RENDERHANDLE _priv_remoteClass_SurfacePool;
        private RENDERHANDLE _priv_remoteClass_VideoPool;
        private RENDERHANDLE _priv_remoteClass_Rasterizer;
        private RENDERHANDLE _priv_remoteClass_Gradient;
        private RENDERHANDLE _priv_remoteClass_AnimationManager;
        private RENDERHANDLE _priv_remoteClass_Animation;
        private RENDERHANDLE _priv_remoteClass_ExternalAnimationInput;
        private RENDERHANDLE _priv_remoteClass_AnimationInputProvider;
        private RENDERHANDLE _priv_remoteClass_WaitCursor;
        private RENDERHANDLE _priv_remoteClass_NullDevice;
        private RENDERHANDLE _priv_remoteClass_Dx9Device;
        private RENDERHANDLE _priv_remoteClass_Dx9EffectResource;
        private RENDERHANDLE _priv_remoteClass_Effect;
        private RENDERHANDLE _priv_remoteClass_Dx9Effect;
        private RENDERHANDLE _priv_remoteClass_Sprite;
        private RENDERHANDLE _priv_remoteClass_Dx9Sprite;
        private RENDERHANDLE _priv_remoteClass_DynamicSurfaceFactory;
        private RENDERHANDLE _priv_remoteClass_SoundBuffer;
        private RENDERHANDLE _priv_remoteClass_Sound;
        private RENDERHANDLE _priv_remoteClass_SoundDevice;
        private RENDERHANDLE _priv_callbackInstance_RenderCapsCallback;
        private RENDERHANDLE _priv_callbackInstance_DeviceCallback;
        private RENDERHANDLE _priv_callbackInstance_VideoPoolCallback;
        private RENDERHANDLE _priv_callbackInstance_AnimationCallback;
        private RENDERHANDLE _priv_callbackInstance_SoundBufferCallback;

        public static ProtocolSplashRendering Bind(RenderPort port)
        {
            Debug2.Validate(port != null, typeof(ArgumentNullException), nameof(port));
            ProtocolInstance protocolInstance = port.LookUpProtocol("Splash::Rendering");
            if (protocolInstance != null)
            {
                ProtocolSplashRendering protocolSplashRendering = protocolInstance as ProtocolSplashRendering;
                Debug2.Validate(protocolSplashRendering != null, typeof(InvalidOperationException), "protocol name collision");
                return protocolSplashRendering;
            }
            ProtocolSplashRendering protocolSplashRendering1 = new ProtocolSplashRendering(port);
            port.BindProtocol(protocolSplashRendering1);
            return protocolSplashRendering1;
        }

        public LocalRenderCapsCallback LocalRenderCapsCallbackHandle => LocalRenderCapsCallback.CreateCallbackImpl(this.Port, this._priv_callbackInstance_RenderCapsCallback);

        public LocalDeviceCallback LocalDeviceCallbackHandle => LocalDeviceCallback.CreateCallbackImpl(this.Port, this._priv_callbackInstance_DeviceCallback);

        public LocalVideoPoolCallback LocalVideoPoolCallbackHandle => LocalVideoPoolCallback.CreateCallbackImpl(this.Port, this._priv_callbackInstance_VideoPoolCallback);

        public LocalAnimationCallback LocalAnimationCallbackHandle => LocalAnimationCallback.CreateCallbackImpl(this.Port, this._priv_callbackInstance_AnimationCallback);

        public LocalSoundBufferCallback LocalSoundBufferCallbackHandle => LocalSoundBufferCallback.CreateCallbackImpl(this.Port, this._priv_callbackInstance_SoundBufferCallback);

        private ProtocolSplashRendering(RenderPort port)
          : base(port, "Splash::Rendering")
        {
        }

        protected override void Init()
        {
            RenderPort port = this.Port;
            this._priv_remoteClass_Window = port.InitRemoteClass("Splash::Rendering::Window");
            this._priv_remoteClass_RenderCaps = port.InitRemoteClass("Splash::Rendering::RenderCaps");
            this._priv_remoteClass_Visual = port.InitRemoteClass("Splash::Rendering::Visual");
            this._priv_remoteClass_Camera = port.InitRemoteClass("Splash::Rendering::Camera");
            this._priv_remoteClass_VisualContainer = port.InitRemoteClass("Splash::Rendering::VisualContainer");
            this._priv_remoteClass_Device = port.InitRemoteClass("Splash::Rendering::Device");
            this._priv_remoteClass_Surface = port.InitRemoteClass("Splash::Rendering::Surface");
            this._priv_remoteClass_SurfacePool = port.InitRemoteClass("Splash::Rendering::SurfacePool");
            this._priv_remoteClass_VideoPool = port.InitRemoteClass("Splash::Rendering::VideoPool");
            this._priv_remoteClass_Rasterizer = port.InitRemoteClass("Splash::Rendering::Rasterizer");
            this._priv_remoteClass_Gradient = port.InitRemoteClass("Splash::Rendering::Gradient");
            this._priv_remoteClass_AnimationManager = port.InitRemoteClass("Splash::Rendering::AnimationManager");
            this._priv_remoteClass_Animation = port.InitRemoteClass("Splash::Rendering::Animation");
            this._priv_remoteClass_ExternalAnimationInput = port.InitRemoteClass("Splash::Rendering::ExternalAnimationInput");
            this._priv_remoteClass_AnimationInputProvider = port.InitRemoteClass("Splash::Rendering::AnimationInputProvider");
            this._priv_remoteClass_WaitCursor = port.InitRemoteClass("Splash::Rendering::WaitCursor");
            this._priv_remoteClass_NullDevice = port.InitRemoteClass("Splash::Rendering::NullDevice");
            this._priv_remoteClass_Dx9Device = port.InitRemoteClass("Splash::Rendering::Dx9Device");
            this._priv_remoteClass_Dx9EffectResource = port.InitRemoteClass("Splash::Rendering::Dx9EffectResource");
            this._priv_remoteClass_Effect = port.InitRemoteClass("Splash::Rendering::Effect");
            this._priv_remoteClass_Dx9Effect = port.InitRemoteClass("Splash::Rendering::Dx9Effect");
            this._priv_remoteClass_Sprite = port.InitRemoteClass("Splash::Rendering::Sprite");
            this._priv_remoteClass_Dx9Sprite = port.InitRemoteClass("Splash::Rendering::Dx9Sprite");
            this._priv_remoteClass_DynamicSurfaceFactory = port.InitRemoteClass("Splash::Rendering::DynamicSurfaceFactory");
            this._priv_remoteClass_SoundBuffer = port.InitRemoteClass("Splash::Rendering::SoundBuffer");
            this._priv_remoteClass_Sound = port.InitRemoteClass("Splash::Rendering::Sound");
            this._priv_remoteClass_SoundDevice = port.InitRemoteClass("Splash::Rendering::SoundDevice");
            this._priv_callbackInstance_RenderCapsCallback = LocalRenderCapsCallback.BindCallback(port);
            this._priv_callbackInstance_DeviceCallback = LocalDeviceCallback.BindCallback(port);
            this._priv_callbackInstance_VideoPoolCallback = LocalVideoPoolCallback.BindCallback(port);
            this._priv_callbackInstance_AnimationCallback = LocalAnimationCallback.BindCallback(port);
            this._priv_callbackInstance_SoundBufferCallback = LocalSoundBufferCallback.BindCallback(port);
        }

        internal RENDERHANDLE Window_ClassHandle => this._priv_remoteClass_Window;

        internal RENDERHANDLE RenderCaps_ClassHandle => this._priv_remoteClass_RenderCaps;

        internal RENDERHANDLE Visual_ClassHandle => this._priv_remoteClass_Visual;

        internal RENDERHANDLE Camera_ClassHandle => this._priv_remoteClass_Camera;

        internal RENDERHANDLE VisualContainer_ClassHandle => this._priv_remoteClass_VisualContainer;

        internal RENDERHANDLE Device_ClassHandle => this._priv_remoteClass_Device;

        internal RENDERHANDLE Surface_ClassHandle => this._priv_remoteClass_Surface;

        internal RENDERHANDLE SurfacePool_ClassHandle => this._priv_remoteClass_SurfacePool;

        internal RENDERHANDLE VideoPool_ClassHandle => this._priv_remoteClass_VideoPool;

        internal RENDERHANDLE Rasterizer_ClassHandle => this._priv_remoteClass_Rasterizer;

        internal RENDERHANDLE Gradient_ClassHandle => this._priv_remoteClass_Gradient;

        internal RENDERHANDLE AnimationManager_ClassHandle => this._priv_remoteClass_AnimationManager;

        internal RENDERHANDLE Animation_ClassHandle => this._priv_remoteClass_Animation;

        internal RENDERHANDLE ExternalAnimationInput_ClassHandle => this._priv_remoteClass_ExternalAnimationInput;

        internal RENDERHANDLE AnimationInputProvider_ClassHandle => this._priv_remoteClass_AnimationInputProvider;

        internal RENDERHANDLE WaitCursor_ClassHandle => this._priv_remoteClass_WaitCursor;

        internal RENDERHANDLE NullDevice_ClassHandle => this._priv_remoteClass_NullDevice;

        internal RENDERHANDLE Dx9Device_ClassHandle => this._priv_remoteClass_Dx9Device;

        internal RENDERHANDLE Dx9EffectResource_ClassHandle => this._priv_remoteClass_Dx9EffectResource;

        internal RENDERHANDLE Effect_ClassHandle => this._priv_remoteClass_Effect;

        internal RENDERHANDLE Dx9Effect_ClassHandle => this._priv_remoteClass_Dx9Effect;

        internal RENDERHANDLE Sprite_ClassHandle => this._priv_remoteClass_Sprite;

        internal RENDERHANDLE Dx9Sprite_ClassHandle => this._priv_remoteClass_Dx9Sprite;

        internal RENDERHANDLE DynamicSurfaceFactory_ClassHandle => this._priv_remoteClass_DynamicSurfaceFactory;

        internal RENDERHANDLE SoundBuffer_ClassHandle => this._priv_remoteClass_SoundBuffer;

        internal RENDERHANDLE Sound_ClassHandle => this._priv_remoteClass_Sound;

        internal RENDERHANDLE SoundDevice_ClassHandle => this._priv_remoteClass_SoundDevice;

        internal RENDERHANDLE RenderCapsCallback_CallbackInstance => this._priv_callbackInstance_RenderCapsCallback;

        internal RENDERHANDLE DeviceCallback_CallbackInstance => this._priv_callbackInstance_DeviceCallback;

        internal RENDERHANDLE VideoPoolCallback_CallbackInstance => this._priv_callbackInstance_VideoPoolCallback;

        internal RENDERHANDLE AnimationCallback_CallbackInstance => this._priv_callbackInstance_AnimationCallback;

        internal RENDERHANDLE SoundBufferCallback_CallbackInstance => this._priv_callbackInstance_SoundBufferCallback;

        internal RemoteRenderCaps BuildRemoteRenderCaps(
          IRenderHandleOwner _priv_owner,
          LocalRenderCapsCallback callback)
        {
            return RemoteRenderCaps.Create(this, _priv_owner, callback);
        }

        internal RemoteCamera BuildRemoteCamera(IRenderHandleOwner _priv_owner) => RemoteCamera.Create(this, _priv_owner);

        internal RemoteVisualContainer BuildRemoteVisualContainer(
          IRenderHandleOwner _priv_owner)
        {
            return RemoteVisualContainer.Create(this, _priv_owner);
        }

        internal RemoteAnimationManager BuildRemoteAnimationManager(
          IRenderHandleOwner _priv_owner)
        {
            return RemoteAnimationManager.Create(this, _priv_owner);
        }

        internal RemoteAnimation BuildRemoteAnimation(
          IRenderHandleOwner _priv_owner,
          AnimationInputType animationType,
          LocalAnimationCallback cb)
        {
            return RemoteAnimation.Create(this, _priv_owner, animationType, cb);
        }

        internal RemoteExternalAnimationInput BuildRemoteExternalAnimationInput(
          IRenderHandleOwner _priv_owner,
          uint nUniqueId)
        {
            return RemoteExternalAnimationInput.Create(this, _priv_owner, nUniqueId);
        }

        internal RemoteAnimationInputProvider BuildRemoteAnimationInputProvider(
          IRenderHandleOwner _priv_owner,
          uint nExternalAnimationInputId)
        {
            return RemoteAnimationInputProvider.Create(this, _priv_owner, nExternalAnimationInputId);
        }

        internal RemoteWaitCursor BuildRemoteWaitCursor(IRenderHandleOwner _priv_owner) => RemoteWaitCursor.Create(this, _priv_owner);

        internal RemoteNullDevice BuildRemoteNullDevice(
          IRenderHandleOwner _priv_owner,
          LocalDeviceCallback cb)
        {
            return RemoteNullDevice.Create(this, _priv_owner, cb);
        }

        internal RemoteDx9EffectResource BuildRemoteDx9EffectResource(
          IRenderHandleOwner _priv_owner,
          RemoteDevice devOwner,
          RemoteDataBuffer dataBuffer)
        {
            return RemoteDx9EffectResource.Create(this, _priv_owner, devOwner, dataBuffer);
        }

        internal RemoteDx9Sprite BuildRemoteDx9Sprite(IRenderHandleOwner _priv_owner) => RemoteDx9Sprite.Create(this, _priv_owner);
    }
}
