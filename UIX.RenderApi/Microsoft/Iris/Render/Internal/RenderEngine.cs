// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.RenderEngine
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Sound;
using System;
using System.Collections;

namespace Microsoft.Iris.Render.Internal
{
    internal class RenderEngine : RenderObject, IRenderEngine, IRenderObject, IDisposable
    {
        private static BitArray s_contexts;
        private static uint s_contextIdSeed = 1;
        private static uint s_maxContextId = 254;
        private IrisEngineInfo m_engineInfo;
        private RenderToken m_primaryToken;
        private MessagingSession m_messagingSession;
        private RenderSession m_renderSession;
        private RenderCaps m_renderCaps;
        private GraphicsDeviceType m_typeGraphics;
        private SoundDeviceType m_typeSound;
        private RenderWindow m_renderWindow;
        private DisplayManager m_displayManager;
        private InputSystem m_inputSystem;
        private SoundDevice m_soundDevice;
        private ContextID m_localContextId;
        private ContextID m_engineContextId;

        static RenderEngine() => s_contexts = new BitArray((int)s_maxContextId + 1);

        internal RenderEngine(IrisEngineInfo engineInfo, IRenderHost renderHost)
        {
            Debug2.Validate(engineInfo != null, typeof(ArgumentNullException), nameof(engineInfo));
            Debug2.Validate(renderHost != null, typeof(ArgumentNullException), nameof(renderHost));
            this.m_engineInfo = engineInfo;
            this.m_engineContextId = AllocateContextId();
            this.m_localContextId = AllocateContextId();
            this.m_primaryToken = new RenderToken(engineInfo, this.m_localContextId, this.m_engineContextId, RENDERGROUP.NULL);
            this.m_messagingSession = new MessagingSession(renderHost, this.m_primaryToken);
            this.m_messagingSession.Connect();
            this.m_renderSession = new RenderSession(this);
            this.m_renderCaps = new RenderCaps(this.m_renderSession);
            this.m_renderCaps.RequestCaps();
            this.m_messagingSession.FlushBatch();
            while (!this.m_renderCaps.HasValidCaps)
                MessagingSession.DoEvents(50U);
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose)
                {
                    this.m_renderSession.ReleaseSharedResources();
                    if (this.m_renderWindow != null)
                        this.m_renderWindow.WindowCreatedEvent -= new EventHandler(this.OnRenderWindowCreated);
                    if (this.m_soundDevice != null)
                        this.m_soundDevice.Dispose();
                    if (this.m_renderWindow != null)
                        this.m_renderWindow.Dispose();
                    if (this.m_inputSystem != null)
                        this.m_inputSystem.Dispose();
                    if (this.m_displayManager != null)
                        this.m_displayManager.Dispose();
                    if (this.m_renderSession != null)
                        this.m_renderSession.Dispose();
                    if (this.m_messagingSession != null)
                    {
                        if (this.m_messagingSession.IsConnected)
                            this.m_messagingSession.Disconnect();
                        this.m_messagingSession.Dispose();
                    }
                    FreeContextId(this.m_localContextId);
                    FreeContextId(this.m_engineContextId);
                }
                this.m_soundDevice = null;
                this.m_displayManager = null;
                this.m_renderWindow = null;
                this.m_renderSession = null;
                this.m_messagingSession = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        void IRenderEngine.Initialize(
          GraphicsDeviceType typeGraphics,
          GraphicsRenderingQuality renderingQuality,
          SoundDeviceType typeSound)
        {
            Debug2.Validate(this.m_messagingSession.IsConnected, typeof(InvalidOperationException), "Must be connected before attempting to initialize");
            Debug2.Validate(this.m_renderSession != null, typeof(ArgumentNullException), "Session must exist before calling Initialize");
            Debug2.Validate(this.IsGraphicsDeviceAvailable(typeGraphics, false), typeof(InvalidOperationException), "Graphics device type not available");
            Debug2.Validate(this.IsSoundDeviceAvailable(typeSound), typeof(InvalidOperationException), "Sound device type not available");
            this.m_renderSession.AssertOwningThread();
            this.m_typeGraphics = typeGraphics;
            this.m_typeSound = typeSound;
            this.m_displayManager = DisplayManager.CreateDisplayManager(this.m_renderSession, this, false);
            this.m_renderWindow = new RenderWindow(this.m_renderSession, this.m_displayManager, typeGraphics, renderingQuality);
            this.m_renderWindow.WindowCreatedEvent += new EventHandler(this.OnRenderWindowCreated);
            this.m_renderSession.Initialize();
            this.m_inputSystem = new InputSystem(this.m_renderSession, this.m_renderWindow);
        }

        private MessagingSession MessagingSession => this.m_messagingSession;

        IRenderSession IRenderEngine.Session => m_renderSession;

        internal RenderSession Session => this.m_renderSession;

        IRenderWindow IRenderEngine.Window => m_renderWindow;

        internal RenderWindow Window => this.m_renderWindow;

        IDisplayManager IRenderEngine.DisplayManager => m_displayManager;

        public DisplayManager DisplayManager => this.m_displayManager;

        public InputSystem InputSystem => this.m_inputSystem;

        internal SoundDevice SoundDevice => this.m_soundDevice;

        internal Vector<Microsoft.Iris.Render.Internal.GraphicsCaps> GraphicsCaps => this.m_renderCaps.Graphics;

        internal Vector<Microsoft.Iris.Render.Internal.SoundCaps> SoundCaps => this.m_renderCaps.Sound;

        private void DoEvents(uint nTimeoutInMsecs)
        {
            this.m_messagingSession.AssertOwningThread();
            MessagingSession.DoEvents(nTimeoutInMsecs);
        }

        bool IRenderEngine.ProcessNativeEvents()
        {
            this.m_renderSession.AssertOwningThread();
            Win32Api.MSG msg;
            EngineApi.WorkResult nResult;
            EngineApi.SpPeekMessage(out msg, HWND.NULL, 0U, 0U, 1U, out nResult);
            if (!Bits.TestFlag((uint)nResult, 2U))
                return Bits.TestFlag((uint)nResult, 1U);
            if (this.m_renderWindow != null)
                this.m_renderWindow.ForwardWindowMessage(ref msg);
            Win32Api.TranslateMessage(ref msg);
            Win32Api.DispatchMessage(ref msg);
            return true;
        }

        void IRenderEngine.WaitForWork(uint nTimeoutInMsecs)
        {
            this.m_renderSession.AssertOwningThread();
            EngineApi.SpWaitMessage(nTimeoutInMsecs, IntPtr.Zero);
        }

        void IRenderEngine.InterThreadWake() => EngineApi.SpInvoke(this.m_localContextId, IntPtr.Zero, IntPtr.Zero, false);

        void IRenderEngine.FlushBatch()
        {
            this.m_renderSession.AssertOwningThread();
            this.m_messagingSession.FlushBatch();
        }

        bool IRenderEngine.IsGraphicsDeviceAvailable(
          GraphicsDeviceType type,
          bool fFilterRecommended)
        {
            Debug2.Validate(this.m_renderCaps != null, typeof(ArgumentNullException), "RenderCaps must be initialized prior to calling");
            this.m_renderSession.AssertOwningThread();
            return this.IsGraphicsDeviceAvailable(type, fFilterRecommended);
        }

        private bool IsGraphicsDeviceAvailable(GraphicsDeviceType type, bool fFilterRecommended)
        {
            foreach (Microsoft.Iris.Render.Internal.GraphicsCaps graphic in this.m_renderCaps.Graphics)
            {
                if ((GraphicsDeviceType)graphic.DeviceType == type)
                    return !fFilterRecommended || graphic.DriverWarning == 0;
            }
            return false;
        }

        bool IRenderEngine.IsSoundDeviceAvailable(SoundDeviceType type)
        {
            Debug2.Validate(this.m_renderCaps != null, typeof(ArgumentNullException), "RenderCaps must be initialized prior to calling");
            this.m_renderSession.AssertOwningThread();
            return this.IsSoundDeviceAvailable(type);
        }

        private bool IsSoundDeviceAvailable(SoundDeviceType type)
        {
            if (type == SoundDeviceType.None)
                return true;
            foreach (Microsoft.Iris.Render.Internal.SoundCaps soundCaps in this.m_renderCaps.Sound)
            {
                if ((SoundDeviceType)soundCaps.DeviceType == type)
                    return true;
            }
            return false;
        }

        private void OnRenderWindowCreated(object sender, EventArgs args)
        {
            SoundDevice soundDevice = null;
            switch (this.m_typeSound)
            {
                case SoundDeviceType.None:
                    this.m_soundDevice = soundDevice;
                    break;
                case SoundDeviceType.DirectSound8:
                    soundDevice = new Ds8SoundDevice(this.m_renderSession, m_renderWindow);
                    goto case SoundDeviceType.None;
                case SoundDeviceType.WaveAudio:
                case SoundDeviceType.XAudio:
                    Debug2.Validate(false, typeof(NotImplementedException), "{0} is not yet supported");
                    goto case SoundDeviceType.None;
                default:
                    Debug2.Validate(false, typeof(ArgumentException), "typeSound");
                    goto case SoundDeviceType.None;
            }
        }

        internal static ContextID AllocateContextId()
        {
            uint num1 = 0;
            lock (s_contexts)
            {
                for (int index = 0; index < s_maxContextId; ++index)
                {
                    uint num2 = s_contextIdSeed++;
                    if (s_contextIdSeed > s_maxContextId)
                        s_contextIdSeed = 1U;
                    if (!s_contexts[(int)num2])
                    {
                        num1 = num2;
                        s_contexts[(int)num1] = true;
                        break;
                    }
                }
            }
            Debug2.Validate(num1 != 0U, typeof(InvalidOperationException), "Failed to allocate a context ID");
            return ContextID.FromUInt32(num1);
        }

        private static void FreeContextId(ContextID contextId)
        {
            uint uint32 = ContextID.ToUInt32(contextId);
            Debug2.Validate(s_contexts[(int)uint32], typeof(InvalidOperationException), "ContextID is not in use");
            lock (s_contexts)
                s_contexts[(int)uint32] = false;
        }

        protected override void Invariant()
        {
            Debug2.Validate(this.m_localContextId != ContextID.NULL, typeof(InvalidOperationException), "local ContextId should not be NULL while running");
            Debug2.Validate(this.m_engineContextId != ContextID.NULL, typeof(InvalidOperationException), "engine ContextId should not be NULL while running");
            Debug2.Validate(this.MessagingSession != null, typeof(InvalidOperationException), "MessagingSession should not be null while running");
            Debug2.Validate(this.MessagingSession.IsConnected, typeof(InvalidOperationException), "MessagingSession should be connected while running");
        }
    }
}
