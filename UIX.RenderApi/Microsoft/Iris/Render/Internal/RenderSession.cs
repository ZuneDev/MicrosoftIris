// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.RenderSession
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Animation;
using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Desktop;
using Microsoft.Iris.Render.Protocols.Splash.Desktop.Nt;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using Microsoft.Iris.Render.Protocols.Splash.Rendering.Nt;
using Microsoft.Iris.Render.Remote;
using Microsoft.Iris.Render.Sound;
using System;

namespace Microsoft.Iris.Render.Internal
{
    internal class RenderSession : RenderObject, IRenderSession, IRenderObject, IDisposable
    {
        private RenderEngine m_renderEngine;
        private MessagingSession m_owningSession;
        private VisualPropertyManager m_visPropManager;
        private ProtocolSplashDesktop m_protocolDesktop;
        private ProtocolSplashDesktopNt m_protocolDesktopNt;
        private ProtocolSplashRendering m_protocolRendering;
        private ProtocolSplashRenderingNt m_protocolRenderingNt;
        private ProtocolSplashRendering m_protocolRenderingLocal;
        private AnimationSystem m_animationSystem;
        private ObjectCacheManager m_cacheManager;
        private Microsoft.Iris.Render.Common.ObjectCache m_cacheSprite;
        private Microsoft.Iris.Render.Common.ObjectCache m_cacheContainer;

        internal RenderSession(RenderEngine renderEngine)
        {
            Debug2.Validate(renderEngine != null, typeof(ArgumentNullException), nameof(renderEngine));
            this.RegisterOwningThread();
            this.m_protocolRendering = ProtocolSplashRendering.Bind(this.Messaging.RenderingPort);
            this.m_protocolDesktop = ProtocolSplashDesktop.Bind(this.Messaging.WindowingPort);
            this.m_protocolRenderingNt = ProtocolSplashRenderingNt.Bind(this.Messaging.RenderingPort);
            this.m_protocolDesktopNt = ProtocolSplashDesktopNt.Bind(this.Messaging.WindowingPort);
            if (this.Messaging.WindowingPort != this.Messaging.RenderingPort)
                this.m_protocolRenderingLocal = ProtocolSplashRendering.Bind(this.Messaging.WindowingPort);
            this.m_renderEngine = renderEngine;
            this.m_visPropManager = new VisualPropertyManager();
            this.m_cacheManager = null;
        }

        internal void Initialize() => this.m_animationSystem = new AnimationSystem(this);

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose && this.m_animationSystem != null)
                    this.m_animationSystem.UnregisterUsage(this);
                this.UnregisterOwningThread();
                this.m_animationSystem = null;
                this.m_visPropManager.Dispose();
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        IGraphicsDevice IRenderSession.GraphicsDevice => m_renderEngine.Window.GraphicsDevice;

        ISoundDevice IRenderSession.SoundDevice => m_renderEngine.SoundDevice;

        internal SoundDevice SoundDevice => this.m_renderEngine.SoundDevice;

        IAnimationSystem IRenderSession.AnimationSystem => m_animationSystem;

        IInputSystem IRenderSession.InputSystem => m_renderEngine.InputSystem;

        internal MessagingSession Messaging => this.m_owningSession;

        internal RenderPort RenderingPort => this.m_owningSession.RenderingPort;

        internal RenderPort WindowingPort => this.m_owningSession.WindowingPort;

        internal ProtocolSplashDesktop DesktopProtocol => this.m_protocolDesktop;

        internal ProtocolSplashDesktopNt NtDesktopProtocol => this.m_protocolDesktopNt;

        internal ProtocolSplashRendering RenderingProtocol => this.m_protocolRendering;

        internal ProtocolSplashRenderingNt NtRenderingProtocol => this.m_protocolRenderingNt;

        internal ProtocolSplashRendering LocalRenderingProtocol => this.m_protocolRenderingLocal;

        internal bool IsValid => this.m_owningSession.IsConnected;

        internal bool IsForeignByteOrderOnWindowing => this.WindowingPort.ForeignByteOrder;

        internal VisualPropertyManager VisualPropertyManager => this.m_visPropManager;

        internal ObjectCacheManager CacheManager => this.m_cacheManager;

        public IEffectTemplate CreateEffectTemplate(object objUser, string stName)
        {
            Debug2.Validate(objUser != null, typeof(ArgumentNullException), nameof(objUser));
            EffectTemplate effectTemplate = this.m_renderEngine.Window.GraphicsDevice.CreateEffectTemplate(stName);
            effectTemplate.RegisterUsage(objUser);
            return effectTemplate;
        }

        public IVideoStream CreateVideoStream(object objUser)
        {
            Debug2.Validate(objUser != null, typeof(ArgumentNullException), nameof(objUser));
            VideoStream videoStream = new VideoStream(this, this.m_renderEngine.Window.GraphicsDevice);
            videoStream.RegisterUsage(objUser);
            return videoStream;
        }

        public IVisualContainer CreateVisualContainer(
          object objUser,
          object objOwnerData)
        {
            Debug2.Validate(objUser != null, typeof(ArgumentNullException), nameof(objUser));
            VisualContainer visualContainer = null;
            if (this.m_cacheContainer != null)
            {
                visualContainer = (VisualContainer)this.m_cacheContainer.Remove(objUser);
                if (visualContainer != null)
                    visualContainer.OwnerData = objOwnerData;
            }
            if (visualContainer == null)
            {
                RemoteVisual remoteVisual;
                visualContainer = new VisualContainer(false, this, this.m_renderEngine.Window, objOwnerData, out remoteVisual);
                visualContainer.RegisterUsage(objUser);
                visualContainer.Cache = this.m_cacheContainer;
            }
            return visualContainer;
        }

        public ICamera CreateCamera(object objUser)
        {
            Debug2.Validate(objUser != null, typeof(ArgumentNullException), nameof(objUser));
            Camera camera = new Camera(this);
            camera.RegisterUsage(objUser);
            return camera;
        }

        public IGradient CreateGradient(object objUser)
        {
            Debug2.Validate(objUser != null, typeof(ArgumentNullException), nameof(objUser));
            Gradient gradient = new Gradient(this, this.m_renderEngine.Window.GraphicsDevice.RemoteDevice);
            gradient.RegisterUsage(objUser);
            return gradient;
        }

        public ISprite CreateSprite(object objUser, object objOwnerData)
        {
            Debug2.Validate(objUser != null, typeof(ArgumentNullException), nameof(objUser));
            Sprite sprite = null;
            if (this.m_cacheSprite != null)
            {
                sprite = (Sprite)this.m_cacheSprite.Remove(objUser);
                if (sprite != null)
                    sprite.OwnerData = objOwnerData;
            }
            if (sprite == null)
            {
                sprite = new Sprite(this, this.m_renderEngine.Window, objOwnerData);
                sprite.RegisterUsage(objUser);
                sprite.Cache = this.m_cacheSprite;
            }
            return sprite;
        }

        public IImage CreateImage(
          object objUser,
          string identifier,
          ContentNotifyHandler handler)
        {
            Debug2.Validate(objUser != null, typeof(ArgumentNullException), nameof(objUser));
            Debug2.Validate(handler != null, typeof(InvalidOperationException), "Invalid handler");
            Image image = this.m_renderEngine.Window.GraphicsDevice.CreateImage(identifier, handler);
            image.RegisterUsage(objUser);
            return image;
        }

        internal GraphicsCaps GetGraphicsCaps(GraphicsDeviceType type)
        {
            foreach (GraphicsCaps graphicsCap in this.m_renderEngine.GraphicsCaps)
            {
                if ((GraphicsDeviceType)graphicsCap.DeviceType == type)
                    return graphicsCap;
            }
            return new GraphicsCaps();
        }

        private void RegisterOwningThread() => this.m_owningSession = MessagingSession.Current;

        private void UnregisterOwningThread() => this.m_owningSession = null;

        internal void AssertOwningThread()
        {
        }

        internal bool IsOwningThread() => this.m_owningSession == MessagingSession.Current;

        public void DeferredInvoke(Delegate method, object args, DeferredInvokePriority priority)
        {
            if (this.m_owningSession == null)
                return;
            this.m_owningSession.DeferredInvoke(method, args, priority, TimeSpan.FromMilliseconds(0.0));
        }

        public void DeferredInvoke(
          SharedRenderObject owner,
          Delegate method,
          object args,
          DeferredInvokePriority priority)
        {
            if (this.m_owningSession == null)
                return;
            this.m_owningSession.DeferredInvoke(owner, method, args, priority, TimeSpan.FromMilliseconds(0.0));
        }

        internal void DeferredInvoke(
          Delegate method,
          object args,
          DeferredInvokePriority priority,
          TimeSpan delay)
        {
            if (this.m_owningSession == null)
                return;
            this.m_owningSession.DeferredInvoke(method, args, priority, delay);
        }

        internal void ReleaseSharedResources()
        {
            if (this.m_cacheSprite != null)
            {
                this.m_cacheManager.UnregisterCache(this.m_cacheSprite);
                this.m_cacheSprite.Dispose();
                this.m_cacheSprite = null;
            }
            if (this.m_cacheContainer == null)
                return;
            this.m_cacheManager.UnregisterCache(this.m_cacheContainer);
            this.m_cacheContainer.Dispose();
            this.m_cacheContainer = null;
        }

        internal RENDERHANDLE AllocateRenderHandle(IRenderHandleOwner owner) => this.RenderingPort.AllocHandle(owner);

        internal void FreeRenderHandle(RENDERHANDLE handle) => this.RenderingPort.FreeHandle(handle);

        internal IVisual TryGetHandleOwner(uint dwHandleData) => this.WindowingPort.TryGetHandleOwner(RENDERHANDLE.FromUInt32(dwHandleData)) as IVisual;

        public RemoteDesktopManager BuildRemoteDesktopManager(
          DisplayManager owner,
          bool fEnumDisplayModes)
        {
            Debug2.Validate(this.IsOwningThread(), typeof(InvalidOperationException), "Must be called on the session's owning client thread");
            return this.NtDesktopProtocol.BuildRemoteDesktopManager(owner, this.NtDesktopProtocol.LocalDesktopManagerCallbackHandle, fEnumDisplayModes);
        }

        internal RemoteFormWindow BuildRemoteFormWindow(
          RenderWindow window,
          DisplayManager displayManager)
        {
            Debug2.Validate(this.IsOwningThread(), typeof(InvalidOperationException), "Must be called on the session's owning client thread");
            return this.NtDesktopProtocol.BuildRemoteFormWindow(window, displayManager.RemoteStub, this.DesktopProtocol.LocalFormWindowCallbackHandle);
        }

        internal RemoteInputRouter BuildRemoteInputRouter(
          InputSystem inputSystem,
          RenderWindow window)
        {
            Debug2.Validate(this.IsOwningThread(), typeof(InvalidOperationException), "Must be called on the session's owning client thread");
            RemoteInputRouter remoteInputRouter = this.DesktopProtocol.BuildRemoteInputRouter(inputSystem, this.DesktopProtocol.LocalInputCallbackHandle);
            remoteInputRouter?.SendRegisterWithInputSource(this.WindowingPort.DefaultGroupID, ((IRenderHandleOwner)window).RenderHandle);
            return remoteInputRouter;
        }

        internal RemoteNtDevice BuildRemoteNtDevice(NtGraphicsDevice device) => this.NtRenderingProtocol.BuildRemoteNtDevice(device, this.RenderingProtocol.LocalDeviceCallbackHandle);

        internal RemoteGdiDevice BuildRemoteGdiDevice(GdiGraphicsDevice device) => this.NtRenderingProtocol.BuildRemoteGdiDevice(device, this.RenderingProtocol.LocalDeviceCallbackHandle);

        internal RemoteNullDevice BuildRemoteNullDevice(
          NullGraphicsDevice device,
          bool fUseSplitWindowingPort)
        {
            return fUseSplitWindowingPort ? this.LocalRenderingProtocol.BuildRemoteNullDevice(device, this.LocalRenderingProtocol.LocalDeviceCallbackHandle) : this.RenderingProtocol.BuildRemoteNullDevice(device, this.RenderingProtocol.LocalDeviceCallbackHandle);
        }

        internal unsafe DataBuffer BuildDataBuffer(IntPtr pData, uint cbSize) => new DataBuffer(this.RenderingPort, pData.ToPointer(), cbSize);

        internal void LoadSurface(DataBuffer buffer, Surface surface, ImageHeader header) => RemoteRasterizer.SendLoadRawImage(this.RenderingProtocol, surface.RemoteStub, buffer.RemoteStub, header);

        internal RemoteDx9EffectResource BuildRemoteDx9EffectResource(
          Dx9EffectResource effectResource,
          Dx9GraphicsDevice dx9Device,
          DataBuffer dataBuffer)
        {
            return this.RenderingProtocol.BuildRemoteDx9EffectResource(effectResource, dx9Device.RemoteDevice, dataBuffer.RemoteStub);
        }

        internal RemoteDx9Effect BuildRemoteDx9Effect(Dx9Effect effect) => RemoteDx9Effect.Create(this.RenderingProtocol, effect);

        internal RemoteGdiEffect BuildRemoteGdiEffect(GdiEffect effect) => RemoteGdiEffect.Create(this.NtRenderingProtocol, effect);

        internal RemoteCamera BuildRemoteCamera(Camera camera) => this.RenderingProtocol.BuildRemoteCamera(camera);

        internal RemoteVisualContainer BuildRemoteVisualContainer(Visual visual) => this.RenderingProtocol.BuildRemoteVisualContainer(visual);

        internal RemoteSprite BuildRemoteSprite(Sprite sprite, RenderWindow window)
        {
            switch (window.GraphicsDeviceType)
            {
                case GraphicsDeviceType.Gdi:
                    return this.NtRenderingProtocol.BuildRemoteGdiSprite(sprite);
                case GraphicsDeviceType.Direct3D9:
                case GraphicsDeviceType.XeDirectX9:
                    return this.RenderingProtocol.BuildRemoteDx9Sprite(sprite);
                default:
                    Debug2.Throw(false, "Unrecognized device type: {0}", window.GraphicsDeviceType);
                    return null;
            }
        }

        internal RemoteSoundBuffer BuildSoundBufferFromHandle(RENDERHANDLE handle) => RemoteSoundBuffer.CreateFromHandle(this.RenderingPort, handle);

        internal RemoteSound BuildSoundFromHandle(RENDERHANDLE handle) => RemoteSound.CreateFromHandle(this.RenderingPort, handle);

        public RemoteAnimationManager BuildRemoteAnimationManager(
          AnimationSystem owner)
        {
            Debug2.Validate(this.IsOwningThread(), typeof(InvalidOperationException), "Must be called on the session's owning client thread");
            return this.RenderingProtocol.BuildRemoteAnimationManager(owner);
        }

        public RemoteAnimation BuildRemoteAnimation(
          KeyframeAnimation owner,
          AnimationInputType animationType)
        {
            Debug2.Validate(this.IsOwningThread(), typeof(InvalidOperationException), "Must be called on the session's owning client thread");
            return this.RenderingProtocol.BuildRemoteAnimation(owner, animationType, this.RenderingProtocol.LocalAnimationCallbackHandle);
        }

        public RemoteExternalAnimationInput BuildRemoteExternalAnimationInput(
          ExternalAnimationInput owner,
          uint uniqueId)
        {
            Debug2.Validate(this.IsOwningThread(), typeof(InvalidOperationException), "Must be called on the session's owning client thread");
            return this.RenderingProtocol.BuildRemoteExternalAnimationInput(owner, uniqueId);
        }

        public RemoteAnimationInputProvider BuildRemoteAnimationInputProvider(
          AnimationInputProvider owner,
          uint externalAnimationInputId)
        {
            Debug2.Validate(this.IsOwningThread(), typeof(InvalidOperationException), "Must be called on the session's owning client thread");
            return this.RenderingProtocol.BuildRemoteAnimationInputProvider(owner, externalAnimationInputId);
        }

        protected override void Invariant()
        {
            this.AssertOwningThread();
            Debug2.Validate(this.m_owningSession != null, typeof(InvalidOperationException), this.GetType().ToString());
        }
    }
}
