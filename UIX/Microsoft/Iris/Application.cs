// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Application
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Data;
using Microsoft.Iris.Input;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Threading;

namespace Microsoft.Iris
{
    public static class Application
    {
        private static Application.InitializationState s_initializationState = InitializationState.NotInitialized;
        private static UISession s_session;
        private static Window s_mainWindow;
        private static bool s_isShuttingDown;
        private static RenderingType s_renderType = RenderingType.Default;
        private static SoundType s_soundType = SoundType.None;
        private static RenderingQuality s_renderingQuality = RenderingQuality.MaxQuality;
        private static bool s_EnableAnimations = true;
        private static bool s_IsRTL = false;
        private static Dictionary<int, IExternalAnimationInput> s_idToExternalAnimationInput;
        private static Dictionary<int, IAnimationInputProvider> s_animationProviders;

        public static event Action Initialized;

        public static string Name
        {
            get => UIApplication.ApplicationName;
            set => UIApplication.ApplicationName = value;
        }

        public static Window Window
        {
            get
            {
                if (s_initializationState != InitializationState.FullyInitialized)
                    throw new InvalidOperationException("Application.Initialize must be called prior Window query");
                if (s_mainWindow == null)
                    s_mainWindow = new Window((UIForm)s_session.Form);
                return s_mainWindow;
            }
        }

        public static RenderingType RenderingType
        {
            set
            {
                if (IsInitialized)
                    throw new InvalidOperationException("Property cannot be modified after application has been initialized");
                s_renderType = value;
            }
            get => s_renderType;
        }

        public static SoundType SoundType
        {
            set
            {
                if (IsInitialized)
                    throw new InvalidOperationException("Property cannot be modified after application has been initialized");
                s_soundType = value;
            }
            get => s_soundType;
        }

        public static RenderingQuality RenderingQuality
        {
            set
            {
                if (IsInitialized)
                    throw new InvalidOperationException("Property cannot be modified after application has been initialized");
                s_renderingQuality = value;
            }
            get => s_renderingQuality;
        }

        public static bool AnimationsEnabled
        {
            set
            {
                if (IsInitialized)
                    throw new InvalidOperationException("Property cannot be modified after application has been initialized");
                s_EnableAnimations = value;
            }
            get => s_EnableAnimations;
        }

        public static bool IsRTL
        {
            set
            {
                if (IsInitialized)
                    throw new InvalidOperationException("Property cannot be modified after application has been initialized");
                s_IsRTL = value;
            }
            get => s_IsRTL;
        }

        public static bool IsDx9AccelerationAvailable
        {
            get
            {
                if (s_initializationState != InitializationState.FullyInitialized)
                    throw new InvalidOperationException("Application.Initialize must be called prior to DX9 check");
                return s_session.IsGraphicsDeviceAvailable(GraphicsDeviceType.Direct3D9);
            }
        }

        private static bool IsInitialized => s_initializationState != InitializationState.NotInitialized;

        public static bool StaticDllResourcesOnly
        {
            set
            {
                if (IsInitialized)
                    throw new InvalidOperationException("Property cannot be modified after application has been initialized");
                DllResources.StaticDllResourcesOnly = value;
            }
            get => DllResources.StaticDllResourcesOnly;
        }

        public static Debug.DebugSettings DebugSettings { get; } = new();

        public static void Initialize()
        {
            if (IsInitialized)
                throw new InvalidOperationException("Application already initialized");
            VerifyTrustedEnvironment();
            s_session = new UISession();
            s_session.IsRtl = s_IsRTL;
            s_session.InputManager.KeyCoalescePolicy = new KeyCoalesceFilter(QueryKeyCoalesce);
            GraphicsDeviceType graphicsType = ChooseRenderingGraphicsDevice(s_renderType);
            switch (graphicsType)
            {
                case GraphicsDeviceType.Gdi:
                    s_renderType = RenderingType.GDI;
                    break;
                case GraphicsDeviceType.Direct3D9:
                    s_renderType = RenderingType.DX9;
                    break;
                default:
                    throw new ArgumentException(InvariantString.Format("Unknown graphics type {0}", graphicsType));
            }
            if (graphicsType == GraphicsDeviceType.Gdi)
                s_EnableAnimations = false;
            SoundDeviceType soundType = ChooseRendererSoundDevice(s_soundType);
            switch (soundType)
            {
                case SoundDeviceType.None:
                    s_soundType = SoundType.None;
                    break;
                case SoundDeviceType.DirectSound8:
                    s_soundType = SoundType.DirectSound;
                    break;
                default:
                    throw new ArgumentException(InvariantString.Format("Unknown sound type {0}", soundType));
            }
            s_session.InitializeRenderingDevices(graphicsType, (GraphicsRenderingQuality)s_renderingQuality, soundType);
            s_renderingQuality = (RenderingQuality)s_session.RenderSession.GraphicsDevice.RenderingQuality;
            InitializeCommon(true);
            if (!s_EnableAnimations)
                AnimationSystem.OverrideAnimationState(true);
            UIForm uiForm = new UIForm(s_session);

            s_initializationState = InitializationState.FullyInitialized;
            Initialized?.Invoke();
        }

        private static void InitializeCommon(bool fullInitialization)
        {
            ErrorManager.OnErrors += new NotifyErrorBatch(NotifyErrorBatchHandler);

            Debug.Trace.Initialize();
            Debug.BridgeServer.Start(null);

            MarkupSystem.Startup(!fullInitialization);
            StaticServices.Initialize();
        }

        public static void InitializeForToolOnly()
        {
            if (IsInitialized)
                throw new InvalidOperationException("Application has already been initialized");
            RenderApi.InitializeForToolOnly();
            UIDispatcher uiDispatcher = new UIDispatcher(true);
            InitializeCommon(false);
            s_initializationState = InitializationState.InitializedWithoutUI;
        }

        public static bool IsApplicationThread => UIDispatcher.IsUIThread;

        public static bool LoadMarkup(string uri)
        {
            UIDispatcher.VerifyOnApplicationThread();
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            return MarkupSystem.Load(uri, MarkupSystem.RootIslandId) != null;
        }

        public static bool CompileMarkup(CompilerInput[] compilands, CompilerInput dataTableCompiland)
        {
            UIDispatcher.VerifyOnApplicationThread();
            if (compilands == null)
                throw new ArgumentNullException(nameof(compilands));
            foreach (CompilerInput compiland in compilands)
            {
                if (compiland.SourceFileName == null)
                    throw new ArgumentNullException("SourceFileName");
                if (compiland.OutputFileName == null)
                    throw new ArgumentNullException("OutputFileName");
            }
            if (dataTableCompiland.SourceFileName == null && dataTableCompiland.OutputFileName != null || dataTableCompiland.SourceFileName != null && dataTableCompiland.OutputFileName == null)
                throw new ArgumentException(nameof(dataTableCompiland));
            return MarkupCompiler.Compile(compilands, dataTableCompiland);
        }

        public static void UnloadAllMarkup()
        {
            UIDispatcher.VerifyOnApplicationThread();
            MarkupSystem.UnloadAll();
        }

        public static void AddMarkupRedirect(string fromPrefix, string toPrefix)
        {
            UIDispatcher.VerifyOnApplicationThread();
            if (fromPrefix == null)
                throw new ArgumentNullException(nameof(fromPrefix));
            if (toPrefix == null)
                throw new ArgumentNullException(nameof(toPrefix));
            ResourceManager.Instance.AddUriRedirect(fromPrefix, toPrefix);
        }

        public static void AddImportRedirect(string fromPrefix, string toPrefix)
        {
            UIDispatcher.VerifyOnApplicationThread();
            if (fromPrefix == null)
                throw new ArgumentNullException(nameof(fromPrefix));
            if (toPrefix == null)
                throw new ArgumentNullException(nameof(toPrefix));
            MarkupSystem.AddImportRedirect(fromPrefix, toPrefix);
        }

        public static void RegisterDataProvider(string name, DataProviderQueryFactory factory)
        {
            UIDispatcher.VerifyOnApplicationThread();
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (MarkupDataProvider.GetDataProvider(name) != null)
                throw new ArgumentException("Provider is already registered");
            MarkupDataProvider.RegisterDataProvider(new AssemblyDataProviderWrapper(name, factory));
        }

        public static void Run(DeferredInvokeHandler initialLoadComplete)
        {
            UIDispatcher.VerifyOnApplicationThread();
            if (s_initializationState != InitializationState.FullyInitialized)
                throw new InvalidOperationException("Application not initialized for displaying UI");
            if (initialLoadComplete != null)
                ((UIForm)s_session.Form).SetInitialLoadCompleteCallback(DeferredInvokeProxy.Thunk(initialLoadComplete));
            UIApplication.Run();
        }

        public static void Run() => Run(null);

        public static event EventHandler ShuttingDown;

        public static bool IsShuttingDown => s_isShuttingDown;

        public static void Shutdown()
        {
            UIDispatcher.VerifyOnApplicationThread();
            s_isShuttingDown = true;
            if (ShuttingDown != null)
                ShuttingDown(null, EventArgs.Empty);
            MarkupSystem.Shutdown();
            if (s_initializationState == InitializationState.FullyInitialized)
            {
                s_session.Dispose();
                s_session = null;
            }
            if (s_initializationState == InitializationState.InitializedWithoutUI)
                RenderApi.ShutdownForToolOnly();
            StaticServices.Uninitialize();
            Debug.Trace.Shutdown();
            ErrorManager.OnErrors -= new NotifyErrorBatch(NotifyErrorBatchHandler);
            s_initializationState = InitializationState.NotInitialized;
        }

        public static void DeferredInvoke(DeferredInvokeHandler method, DeferredInvokePriority priority) => DeferredInvoke(method, null, priority);

        public static void DeferredInvoke(DeferredInvokeHandler method, object args) => DeferredInvoke(method, args, DeferredInvokePriority.Normal);

        public static void DeferredInvoke(
          DeferredInvokeHandler method,
          object args,
          DeferredInvokePriority priority)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            DispatchPriority priority1;
            switch (priority)
            {
                case DeferredInvokePriority.Normal:
                    priority1 = DispatchPriority.AppEvent;
                    break;
                case DeferredInvokePriority.Low:
                    priority1 = DispatchPriority.Idle;
                    break;
                default:
                    throw new ArgumentException(InvariantString.Format("Unknown DeferredInvokePriority {0}", priority));
            }
            DeferredCall.Post(priority1, DeferredInvokeProxy.Thunk(method), args);
        }

        public static void DeferredInvoke(DeferredInvokeHandler method, TimeSpan delay) => DeferredInvoke(method, null, delay);

        public static void DeferredInvoke(DeferredInvokeHandler method, object args, TimeSpan delay)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            DeferredCall.Post(delay, DeferredInvokeProxy.Thunk(method), args);
        }

        public static void DeferredInvoke(Thread thread, DeferredInvokeHandler method) => DeferredInvoke(thread, method, null);

        public static void DeferredInvoke(Thread thread, DeferredInvokeHandler method, object args)
        {
            if (thread == null)
                throw new ArgumentNullException(nameof(thread));
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            DeferredCall.Post(thread, DispatchPriority.AppEvent, DeferredInvokeProxy.Thunk(method), args);
        }

        public static void DeferredInvokeOnWorkerThread(
          DeferredInvokeHandler workerMethod,
          DeferredInvokeHandler notifyMethod,
          object args)
        {
            if (workerMethod == null)
                throw new ArgumentNullException(nameof(workerMethod));
            if (notifyMethod == null)
                throw new ArgumentNullException(nameof(notifyMethod));
            UIApplication.DeferredInvokeOnWorkerThread(DeferredInvokeProxy.Thunk(workerMethod), DeferredInvokeProxy.Thunk(notifyMethod), args);
        }

        public static void RunWorkerMessagePump(
          DeferredInvokeHandler initialWork,
          object initialWorkArgs)
        {
            if (UIDispatcher.CurrentDispatcher != null)
                throw new InvalidOperationException("Thread already has a dispatcher running");
            if (initialWork == null)
                throw new ArgumentNullException(nameof(initialWork));
            UIApplication.StartArgs startArgs = null;
            if (initialWork != null)
                startArgs = new UIApplication.StartArgs(DeferredInvokeProxy.Thunk(initialWork), initialWorkArgs);
            UIApplication.StartDispatcher(startArgs);
        }

        public static Thread StartWorkerThreadWithMessagePump(string threadName) => UIApplication.StartThreadWithDispatcher(threadName);

        public static void ShutdownWorkerMessagePump(Thread thread)
        {
            if (thread == UIDispatcher.MainUIThread)
                throw new InvalidOperationException("Use Application.Shutdown to shut down the application dispatcher");
            UIDispatcher.StopCurrentMessageLoop(thread);
        }

        public static int CreateExternalAnimationInput(IDictionary<string, int> propertyNameToId)
        {
            UIDispatcher.VerifyOnApplicationThread();
            if (s_idToExternalAnimationInput == null)
                s_idToExternalAnimationInput = new Dictionary<int, IExternalAnimationInput>();
            SimpleAnimationPropertyMap animationPropertyMap = new SimpleAnimationPropertyMap(propertyNameToId);
            IExternalAnimationInput externalAnimationInput = s_session.RenderSession.AnimationSystem.CreateExternalAnimationInput(s_idToExternalAnimationInput, animationPropertyMap);
            s_idToExternalAnimationInput.Add((int)externalAnimationInput.UniqueId, externalAnimationInput);
            return (int)externalAnimationInput.UniqueId;
        }

        public static void DisposeExternalAnimationInput(int animationId)
        {
            UIDispatcher.VerifyOnApplicationThread();
            IExternalAnimationInput externalAnimationInput;
            if (s_idToExternalAnimationInput == null || !s_idToExternalAnimationInput.TryGetValue(animationId, out externalAnimationInput))
                return;
            s_idToExternalAnimationInput.Remove(animationId);
            externalAnimationInput.UnregisterUsage(s_idToExternalAnimationInput);
            IAnimationInputProvider animationInputProvider;
            if (s_animationProviders == null || !s_animationProviders.TryGetValue(animationId, out animationInputProvider))
                return;
            s_animationProviders.Remove(animationId);
            animationInputProvider.UnregisterUsage(s_idToExternalAnimationInput);
        }

        internal static IExternalAnimationInput MapExternalAnimationInput(
          int animationId)
        {
            UIDispatcher.VerifyOnApplicationThread();
            IExternalAnimationInput externalAnimationInput = null;
            if (s_idToExternalAnimationInput != null)
                s_idToExternalAnimationInput.TryGetValue(animationId, out externalAnimationInput);
            return externalAnimationInput;
        }

        public static void SetExternalAnimationInputProperty(
          int animationId,
          string property,
          float value)
        {
            UIDispatcher.VerifyOnApplicationThread();
            IAnimationInputProvider provider;
            if (s_animationProviders == null || !s_animationProviders.TryGetValue(animationId, out provider))
            {
                provider = MapExternalAnimationInput(animationId).CreateProvider(s_idToExternalAnimationInput);
                if (s_animationProviders == null)
                    s_animationProviders = new Dictionary<int, IAnimationInputProvider>();
                s_animationProviders.Add(animationId, provider);
            }
            provider.PublishFloat(property, value);
        }

        public static event ErrorReportHandler ErrorReport;

        private static void NotifyErrorBatchHandler(IList records)
        {
            if (ErrorReport == null)
                return;
            Error[] errors = new Error[records.Count];
            for (int index = 0; index < records.Count; ++index)
            {
                ErrorRecord record = (ErrorRecord)records[index];
                errors[index] = new Error()
                {
                    Context = record.Context,
                    Message = record.Message,
                    Warning = record.Warning,
                    Line = record.Line,
                    Column = record.Column
                };
            }
            ErrorReport(errors);
        }

        private static GraphicsDeviceType ChooseRenderingGraphicsDevice(
          RenderingType type)
        {
            GraphicsDeviceType graphicsType;
            switch (type)
            {
                case RenderingType.GDI:
                    graphicsType = GraphicsDeviceType.Gdi;
                    break;
                case RenderingType.DX9:
                case RenderingType.Default:
                    graphicsType = GraphicsDeviceType.Direct3D9;
                    break;
                default:
                    throw new ArgumentException(InvariantString.Format("Unknown rendering type {0}", type));
            }
            if (type == RenderingType.Default && !s_session.IsGraphicsDeviceRecommended(graphicsType) || !s_session.IsGraphicsDeviceAvailable(graphicsType))
                graphicsType = GraphicsDeviceType.Gdi;
            return graphicsType;
        }

        private static SoundDeviceType ChooseRendererSoundDevice(SoundType typeRequested)
        {
            if (typeRequested == SoundType.None)
                return SoundDeviceType.None;
            if (typeRequested != SoundType.DirectSound)
                throw new ArgumentException(InvariantString.Format("Unknown sound type {0}", typeRequested));
            return s_session.IsSoundDeviceAvailable(SoundDeviceType.DirectSound8) ? SoundDeviceType.DirectSound8 : SoundDeviceType.None;
        }

        private static bool QueryKeyCoalesce(Keys key) => true;

        private static void VerifyTrustedEnvironment()
        {
            Assembly assembly1 = new StackTrace().GetFrame(2).GetMethod().DeclaringType.Assembly;
            Assembly assembly2 = typeof(Application).Assembly;
            byte[] publicKey1 = assembly1.GetName().GetPublicKey();
            byte[] publicKey2 = assembly2.GetName().GetPublicKey();
            bool flag = true;
            if (publicKey1.Length == publicKey2.Length)
            {
                for (int index = 0; index < publicKey2.Length; ++index)
                {
                    if (publicKey1[index] != publicKey2[index])
                    {
                        flag = false;
                        break;
                    }
                }
            }
            else
                flag = false;
            //if (!flag)
            //    throw new SecurityException("Attempt to activate system within an untrusted environment");
        }

        private enum InitializationState
        {
            NotInitialized,
            FullyInitialized,
            InitializedWithoutUI,
        }
    }
}
