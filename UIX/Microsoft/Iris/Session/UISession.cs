// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Session.UISession
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Audio;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Input;
using Microsoft.Iris.Library;
using Microsoft.Iris.OS;
using Microsoft.Iris.Queues;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Audio;
using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.Session
{
    internal class UISession
    {
        private bool _rtl;
        private static UISession s_theOnlySession;
        private UIDispatcher _dispatcher;
        private InputManager _inputManager;
        private IRenderEngine _engine;
        private IRenderSession _session;
        private EffectManager _effectManager;
        private bool _syncWindowPending;
        private readonly SimpleCallback _syncWindowHandler;
        private bool _initRequestedFlag;
        private readonly SimpleCallback _processInitialization;
        private bool _layoutRequestedFlag;
        private readonly SimpleCallback _processLayout;
        private bool _applyLayoutRequestedFlag;
        private readonly SimpleCallback _applyLayout;
        private bool _paintRequestedFlag;
        private readonly SimpleCallback _processPaint;
        private bool _layingOut;
        private SimpleQueue _queueSyncLayoutComplete;
        private AnimationManager _animationManager;
        private SoundManager _soundManager;
        private Form _form;
        private static readonly DeferredHandler s_deferredPlaySound = new DeferredHandler(DeferredPlaySound);
        private static readonly DeferredHandler s_deferredPlaySystemSound = new DeferredHandler(DeferredPlaySystemSound);

        public UISession()
          : this(null, null, 0U)
        {
        }

        public UISession(
          EventHandler rendererConnectedCallback,
          TimeoutHandler handlerTimeout,
          uint timeoutSecValue)
        {
            _syncWindowHandler = new SimpleCallback(SyncWindowHandler);
            _queueSyncLayoutComplete = new SimpleQueue();
            _processInitialization = new SimpleCallback(ProcessInitialization);
            _processLayout = new SimpleCallback(ProcessLayout);
            _applyLayout = new SimpleCallback(ApplyLayout);
            _processPaint = new SimpleCallback(ProcessPaint);
            _inputManager = new InputManager(this);
            s_theOnlySession = this;
            _dispatcher = new UIDispatcher(this, handlerTimeout, timeoutSecValue, true);
            int pdwDefaultLayout;
            Win32Api.IFWIN32(Win32Api.GetProcessDefaultLayout(out pdwDefaultLayout));
            _rtl = pdwDefaultLayout == 1;
            _engine = RenderApi.CreateEngine(IrisEngineInfo.CreateLocal(), Dispatcher);
            _session = _engine.Session;
            TextImageCache.Initialize(this);
            ScavengeImageCache.Initialize(this);
        }

        internal void InitializeRenderingDevices(
          GraphicsDeviceType graphicsType,
          GraphicsRenderingQuality renderingQuality,
          SoundDeviceType soundType)
        {
            _engine.Initialize(graphicsType, renderingQuality, soundType);
            _effectManager = new EffectManager(_session);
            _soundManager = new SoundManager(this, _session);
            _animationManager = new AnimationManager(_session);
            _inputManager.ConnectToRenderer();
        }

        internal bool IsGraphicsDeviceRecommended(GraphicsDeviceType graphicsType) => _engine.IsGraphicsDeviceAvailable(graphicsType, true);

        internal bool IsGraphicsDeviceAvailable(GraphicsDeviceType graphicsType) => _engine.IsGraphicsDeviceAvailable(graphicsType, false);

        internal bool IsSoundDeviceAvailable(SoundDeviceType soundType) => _engine.IsSoundDeviceAvailable(soundType);

        internal bool ProcessNativeEvents() => _engine.ProcessNativeEvents();

        internal void WaitForWork(uint nTimeoutInMsecs) => _engine.WaitForWork(nTimeoutInMsecs);

        internal void InterThreadWake() => _engine.InterThreadWake();

        internal void FlushBatch() => _engine.FlushBatch();

        internal IRenderSession RenderSession => _session;

        internal AnimationManager AnimationManager => _animationManager;

        public void Dispose()
        {
            if (TextImageCache.Instance != null)
                TextImageCache.Instance.PrepareToShutdown();
            if (ScavengeImageCache.Instance != null)
                ScavengeImageCache.Instance.PrepareToShutdown();
            if (_soundManager != null)
            {
                _soundManager.Dispose();
                _soundManager = null;
            }
            if (_animationManager != null)
            {
                _animationManager.Dispose();
                _animationManager = null;
            }
            if (_form != null)
                _form.Visible = false;
            _inputManager.PrepareToShutDown();
            _queueSyncLayoutComplete = null;
            _form = null;
            _dispatcher.ShutDown(true);
            s_theOnlySession = null;
            _effectManager.Dispose();
            _effectManager = null;
            if (_engine != null)
            {
                _engine.Dispose();
                _engine = null;
                _session = null;
            }
            TextImageCache.Uninitialize(this);
            ScavengeImageCache.Uninitialize(this);
            _dispatcher.Dispose();
            if (RenderApi.DebugModule == null)
                return;
            RenderApi.DebugModule.Dispose();
            RenderApi.DebugModule = null;
        }

        internal bool IsValid => s_theOnlySession == this;

        public static void Validate(UISession session)
        {
        }

        public static UISession Default => s_theOnlySession;

        public bool IsRtl
        {
            get => _rtl;
            set => _rtl = value;
        }

        public UIDispatcher Dispatcher => _dispatcher;

        public InputManager InputManager => _inputManager;

        public EffectManager EffectManager => _effectManager;

        internal UIZone RootZone
        {
            get
            {
                UIZone uiZone = null;
                if (_form != null)
                    uiZone = _form.Zone;
                return uiZone;
            }
        }

        public bool InLayout => _layingOut;

        internal void ScheduleUiTask(UiTask task)
        {
            switch (task)
            {
                case UiTask.Initialization:
                    ScheduleInitialization();
                    break;
                case UiTask.LayoutComputation:
                    ScheduleLayout();
                    break;
                case UiTask.LayoutApplication:
                    ScheduleApplyLayout();
                    break;
                case UiTask.Painting:
                    SchedulePaint();
                    break;
            }
        }

        private void ScheduleLayout()
        {
            if (_layoutRequestedFlag)
                return;
            DeferredCall.Post(DispatchPriority.Layout, _processLayout);
            _layoutRequestedFlag = true;
        }

        private void ScheduleApplyLayout()
        {
            if (_applyLayoutRequestedFlag)
                return;
            DeferredCall.Post(DispatchPriority.LayoutApply, _applyLayout);
            _applyLayoutRequestedFlag = true;
        }

        private void SchedulePaint()
        {
            if (_paintRequestedFlag)
                return;
            DeferredCall.Post(DispatchPriority.Render, _processPaint);
            _paintRequestedFlag = true;
        }

        private void ScheduleInitialization()
        {
            if (_initRequestedFlag)
                return;
            DeferredCall.Post(DispatchPriority.High, _processInitialization);
            _initRequestedFlag = true;
        }

        public void RequestUpdateView(bool syncWindow)
        {
            InputManager.SuspendInputUntil(DispatchPriority.Idle);
            Dispatcher.TemporarilyBlockRPCs();
            if (!syncWindow || _syncWindowPending)
                return;
            _syncWindowPending = true;
            DeferredCall.Post(DispatchPriority.RenderSync, _syncWindowHandler);
        }

        private void SyncWindowHandler()
        {
            _syncWindowPending = false;
            _session.GraphicsDevice.RenderNowIfPossible();
        }

        internal void EnqueueSyncLayoutCompleteHandler(object snd, EventHandler eh) => _queueSyncLayoutComplete.PostItem(DeferredCall.Create(eh, snd, EventArgs.Empty));

        private void ProcessInitialization()
        {
            using (TaskReentrancyDetection.Enter("Initialization"))
            {
                if (!IsValid || !_initRequestedFlag)
                    return;
                _initRequestedFlag = false;
                RootZone?.ProcessUiTask(UiTask.Initialization, null);
            }
        }

        private void ProcessLayout()
        {
            using (TaskReentrancyDetection.Enter("Layout"))
            {
                if (!IsValid || !_layoutRequestedFlag)
                    return;
                _layoutRequestedFlag = false;
                UIZone rootZone = RootZone;
                if (rootZone == null)
                    return;
                _layingOut = true;
                rootZone.ProcessUiTask(UiTask.LayoutComputation, null);
                QueueItem nextItem;
                while ((nextItem = _queueSyncLayoutComplete.GetNextItem()) != null)
                    nextItem.Dispatch();
                _layingOut = false;
            }
        }

        private void ApplyLayout()
        {
            using (TaskReentrancyDetection.Enter(nameof(ApplyLayout)))
            {
                if (!IsValid || !_applyLayoutRequestedFlag)
                    return;
                _applyLayoutRequestedFlag = false;
                RootZone?.ProcessUiTask(UiTask.LayoutApplication, null);
            }
        }

        private void ProcessPaint()
        {
            using (TaskReentrancyDetection.Enter("Paint"))
            {
                if (!IsValid || !_paintRequestedFlag)
                    return;
                _paintRequestedFlag = false;
                RootZone?.ProcessUiTask(UiTask.Painting, null);
            }
        }

        internal void FireOnUnhandledException(object sender, Exception e)
        {
            UISession.UnhandledExceptionHandler unhandledException = OnUnhandledException;
            if (unhandledException == null)
                return;
            UISession.UnhandledExceptionArgs args = new UISession.UnhandledExceptionArgs(e);
            unhandledException(sender, args);
        }

        public event UISession.UnhandledExceptionHandler OnUnhandledException;

        public Form Form => _form;

        internal IRenderWindow GetRenderWindow() => _engine.Window;

        public SoundManager SoundManager => _soundManager;

        public void PlaySound(string stSoundSource)
        {
            UISession.PlaySoundArgs playSoundArgs = new UISession.PlaySoundArgs(this, stSoundSource);
            if (!UIDispatcher.IsUIThread)
                DeferredCall.Post(DispatchPriority.High, s_deferredPlaySound, playSoundArgs);
            else
                DeferredPlaySound(playSoundArgs);
        }

        private static void DeferredPlaySound(object argsObject)
        {
            UISession.PlaySoundArgs playSoundArgs = argsObject as UISession.PlaySoundArgs;
            if (!playSoundArgs.uiSession.IsValid || playSoundArgs.stSoundSource == null)
                return;
            new Sound() { Source = playSoundArgs.stSoundSource }.Play();
        }

        public void PlaySystemSound(SystemSoundEvent systemSoundEvent)
        {
            UISession.PlaySystemSoundArgs playSystemSoundArgs = new UISession.PlaySystemSoundArgs(this, systemSoundEvent);
            if (!UIDispatcher.IsUIThread)
                DeferredCall.Post(DispatchPriority.High, s_deferredPlaySystemSound, playSystemSoundArgs);
            else
                DeferredPlaySystemSound(playSystemSoundArgs);
        }

        private static void DeferredPlaySystemSound(object argsObject)
        {
            UISession.PlaySystemSoundArgs playSystemSoundArgs = argsObject as UISession.PlaySystemSoundArgs;
            if (!playSystemSoundArgs.uiSession.IsValid || playSystemSoundArgs.systemSoundEvent == SystemSoundEvent.None)
                return;
            new Sound()
            {
                SystemSoundEvent = playSystemSoundArgs.systemSoundEvent
            }.Play();
        }

        internal void RegisterHost(Form form) => _form = form;

        public class UnhandledExceptionArgs : EventArgs
        {
            private Exception _e;

            public UnhandledExceptionArgs(Exception e) => _e = e;

            public Exception Error => _e;
        }

        public delegate void UnhandledExceptionHandler(
          object sender,
          UISession.UnhandledExceptionArgs args);

        private class TaskReentrancyDetection : IDisposable
        {
            private static string s_currentTask;
            private static UISession.TaskReentrancyDetection s_currentTaskClearer = new UISession.TaskReentrancyDetection();

            public static IDisposable Enter(string task)
            {
                if (s_currentTask != null)
                    InvariantString.Format("REENTRANCY DETECTED! Attempt to process task '{0}' while already processing '{1}'.", s_currentTask, task);
                s_currentTask = task;
                return s_currentTaskClearer;
            }

            void IDisposable.Dispose() => s_currentTask = null;
        }

        private class PlaySoundArgs
        {
            public UISession uiSession;
            public string stSoundSource;

            public PlaySoundArgs(UISession session, string source)
            {
                uiSession = session;
                stSoundSource = source;
            }
        }

        private class PlaySystemSoundArgs
        {
            public UISession uiSession;
            public SystemSoundEvent systemSoundEvent;

            public PlaySystemSoundArgs(UISession session, SystemSoundEvent soundEventId)
            {
                uiSession = session;
                systemSoundEvent = soundEventId;
            }
        }
    }
}
