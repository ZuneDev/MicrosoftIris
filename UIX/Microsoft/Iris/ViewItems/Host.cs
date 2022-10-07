// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.Host
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Markup.UIX;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.ViewItems
{
    public class Host : ViewItem, ISchemaInfo
    {
        private const string DefaultUI = "Default";
        private UIClassTypeSchema _typeRestriction;
        private UIClassTypeSchema _typeCurrent;
        private UIClass _childUI;
        private string _lastRequestedSource;
        private HostStatus _status;
        private bool _inputEnabled;
        private bool _dynamicHost;
        private bool _unloadable;
        private bool _newContentOnTop;
        private Vector<UIPropertyRecord> _heldUIProperties;
        private Vector<IDisposableObject> _heldInitialUIDisposables;
        private ChildFaultedInDelegate _loadNotify;
        private string _loadNotifyURI;
        private HostRequestPacket _pendingHostRequest;
        private uint _islandId;
        private static DeferredHandler s_startRequestHandler = new DeferredHandler(StartSourceRequest);

        public Host()
          : this(null, null)
        {
        }

        public Host(UIClassTypeSchema typeSchema)
          : this(typeSchema, typeSchema)
        {
        }

        public Host(UIClassTypeSchema typeRestriction, UIClassTypeSchema typeCurrent)
        {
            _status = HostStatus.Normal;
            _inputEnabled = true;
            _typeRestriction = typeRestriction;
            _typeCurrent = typeCurrent;
            if (_typeCurrent == null)
                return;
            SetChildUI(_typeCurrent.ConstructUI());
            _lastRequestedSource = SourceFromType(_typeCurrent);
        }

        private string SourceFromType(TypeSchema type) => type.Owner.Uri + "#" + type.Name;

        public TypeSchema TypeSchema => _typeCurrent;

        protected override void OnDispose()
        {
            base.OnDispose();
            Cancel();
            if (_childUI != null)
                SetChildUI(null);
            if (_heldInitialUIDisposables != null)
            {
                foreach (DisposableObject initialUiDisposable in _heldInitialUIDisposables)
                    initialUiDisposable.Dispose(this);
            }
            if (!Unloadable)
                return;
            UnloadAll();
            MarkupSystem.FreeIslandId(_islandId);
            _islandId = 0U;
        }

        protected override void OnOwnerDeclared(object owner)
        {
            base.OnOwnerDeclared(owner);
            UIClass uiClass = (UIClass)owner;
            if (ChildUI == null)
                return;
            uiClass.Children.Add(ChildUI);
        }

        public void RequestSource(string source, Vector<UIPropertyRecord> properties) => RequestSource(source, null, properties);

        public void RequestSource(TypeSchema type, Vector<UIPropertyRecord> properties) => RequestSource(null, type, properties);

        public void RequestSource(string source, TypeSchema type, Vector<UIPropertyRecord> properties)
        {
            if (type != null)
            {
                if (!HostSchema.Type.IsAssignableFrom(type))
                {
                    ErrorManager.ReportError("RequestSource failed: Referrenced type '{0}' is not a UI", type.Name);
                    return;
                }
                source = SourceFromType(type);
            }
            Cancel();
            HostRequestPacket hostRequestPacket = new HostRequestPacket();
            hostRequestPacket.Host = this;
            hostRequestPacket.Source = source;
            hostRequestPacket.Type = (UIClassTypeSchema)type;
            hostRequestPacket.Properties = properties;
            _pendingHostRequest = hostRequestPacket;
            _lastRequestedSource = source;
            DeferredCall.Post(DispatchPriority.High, s_startRequestHandler, hostRequestPacket);
        }

        public void Cancel()
        {
            if (_pendingHostRequest != null)
            {
                _pendingHostRequest.Clear();
                _pendingHostRequest = null;
            }
            RevokePendingLoadNotification();
        }

        private static void StartSourceRequest(object args)
        {
            HostRequestPacket hostRequestPacket = (HostRequestPacket)args;
            if (hostRequestPacket.Host == null)
                return;
            Host host = hostRequestPacket.Host;
            string source = hostRequestPacket.Source;
            UIClassTypeSchema type = hostRequestPacket.Type;
            Vector<UIPropertyRecord> properties = hostRequestPacket.Properties;
            host.Cancel();
            ErrorManager.EnterContext(source);
            try
            {
                host.SetStatus(HostStatus.LoadingSource);
                LoadResult loadResult = null;
                string uiToCreate = null;
                if (type == null && source != null)
                    loadResult = MarkupSystem.Load(CrackSourceUri(source, out uiToCreate), host.InheritedIslandId);
                host.CompleteSourceRequest(source, type, properties, loadResult, uiToCreate);
            }
            finally
            {
                ErrorManager.ExitContext();
            }
        }

        private void CompleteSourceRequest(
          string requestedSource,
          UIClassTypeSchema requestedType,
          Vector<UIPropertyRecord> properties,
          LoadResult loadResult,
          string uiToCreate)
        {
            ErrorManager.EnterContext(requestedSource);
            ErrorWatermark watermark = ErrorManager.Watermark;
            bool flag = true;
            ForceContentChange();
            try
            {
                if (_childUI != null)
                {
                    if (_typeRestriction != null)
                        HoldChildUIPropertyValues();
                    SetChildUI(null);
                    _typeCurrent = _typeRestriction;
                    FireNotification(NotificationID.SourceType);
                }
                _dynamicHost = true;
                UIClassTypeSchema uiClassTypeSchema = null;
                Vector<UIPropertyRecord> vector = null;
                if (requestedSource != null)
                {
                    if (requestedType == null)
                    {
                        if (loadResult == null || loadResult.Status == LoadResultStatus.Error)
                        {
                            ErrorManager.ReportError("RequestSource failed: Unable to load '{0}'", requestedSource);
                        }
                        else
                        {
                            TypeSchema type = loadResult.FindType(uiToCreate);
                            if (type == null)
                                ErrorManager.ReportError("RequestSource failed: Unable to find '{0}' within '{1}'", uiToCreate, requestedSource);
                            else if (!HostSchema.Type.IsAssignableFrom(type))
                                ErrorManager.ReportError("RequestSource failed: Referrenced type '{0}' is not a UI", uiToCreate);
                            else
                                requestedType = (UIClassTypeSchema)type;
                        }
                    }
                    if (requestedType != null)
                    {
                        uiClassTypeSchema = requestedType;
                        if (_typeRestriction != null && !_typeRestriction.IsAssignableFrom(uiClassTypeSchema))
                            ErrorManager.ReportError("RequestSource failed: Found '{0}' within '{1}', but, it is not a '{2}'", uiToCreate, requestedSource, _typeRestriction.Name);
                        vector = NegotiateNewChildUIPropertyValues(uiClassTypeSchema, properties);
                    }
                }
                if (watermark.ErrorsDetected)
                {
                    SetStatus(HostStatus.FailureLoadingSource);
                }
                else
                {
                    if (uiClassTypeSchema != null)
                    {
                        Host host = (Host)uiClassTypeSchema.ConstructDefault();
                        UIClass childUi = host.ChildUI;
                        foreach (UIPropertyRecord uiPropertyRecord in vector)
                        {
                            object instance = host;
                            uiPropertyRecord.Schema.SetValue(ref instance, uiPropertyRecord.Value);
                        }
                        _heldUIProperties = null;
                        SetChildUI(childUi);
                        object instance1 = this;
                        uiClassTypeSchema.InitializeInstance(ref instance1);
                        UI.Children.Add(childUi);
                        _typeCurrent = uiClassTypeSchema;
                        FireNotification(NotificationID.Source);
                        FireNotification(NotificationID.SourceType);
                    }
                    SetStatus(HostStatus.Normal);
                    if (!NotifyForLatestLoad())
                        return;
                    DeferredCall.Post(DispatchPriority.LayoutSync, new SimpleCallback(DeliverLoadCompleteNotification));
                    flag = false;
                }
            }
            finally
            {
                if (flag)
                    RevokePendingLoadNotification();
                ErrorManager.ExitContext();
            }
        }

        private bool NotifyForLatestLoad() => _loadNotify != null && InvariantString.Equals(_loadNotifyURI, Source);

        private void DeliverLoadCompleteNotification()
        {
            if (NotifyForLatestLoad() && ChildUI != null && ChildUI.RootItem != null)
                _loadNotify(this, ChildUI.RootItem);
            RevokePendingLoadNotification();
        }

        private void RevokePendingLoadNotification()
        {
            _loadNotify = null;
            _loadNotifyURI = null;
        }

        private void HoldChildUIPropertyValues()
        {
            if (_typeRestriction == null)
                return;
            _heldUIProperties = new Vector<UIPropertyRecord>();
            for (TypeSchema typeRestriction = _typeRestriction; typeRestriction != HostSchema.Type; typeRestriction = typeRestriction.Base)
            {
                foreach (PropertySchema property1 in typeRestriction.Properties)
                {
                    string name = property1.Name;
                    if (_childUI.Storage.ContainsKey(name) && !UIPropertyRecord.IsInList(_heldUIProperties, name))
                    {
                        object property2 = _childUI.GetProperty(name);
                        UIPropertyRecord.AddToList(_heldUIProperties, name, property2);
                        if (!_dynamicHost && property2 != null && property1.PropertyType.Disposable)
                        {
                            IDisposableObject disposable = (IDisposableObject)property2;
                            if (_childUI.UnregisterDisposable(ref disposable))
                            {
                                if (_heldInitialUIDisposables == null)
                                    _heldInitialUIDisposables = new Vector<IDisposableObject>();
                                _heldInitialUIDisposables.Add(disposable);
                                disposable.TransferOwnership(this);
                            }
                        }
                    }
                }
            }
        }

        private Vector<UIPropertyRecord> NegotiateNewChildUIPropertyValues(
          TypeSchema replacementType,
          Vector<UIPropertyRecord> specifiedProperties)
        {
            Vector<UIPropertyRecord> list = specifiedProperties ?? new Vector<UIPropertyRecord>();
            if (_typeRestriction != null)
            {
                foreach (UIPropertyRecord heldUiProperty in _heldUIProperties)
                {
                    if (!UIPropertyRecord.IsInList(list, heldUiProperty.Name))
                        list.Add(heldUiProperty);
                }
            }
            foreach (UIPropertyRecord uiPropertyRecord in list)
            {
                uiPropertyRecord.Schema = replacementType.FindPropertyDeep(uiPropertyRecord.Name);
                if (uiPropertyRecord.Schema == null)
                    ErrorManager.ReportError("Runtime UI replacement to '{0}' failed since a property named '{1}' was specified but doesn't exist on '{0}'", replacementType.Name, uiPropertyRecord.Name);
                else if (!uiPropertyRecord.Schema.PropertyType.IsAssignableFrom(uiPropertyRecord.Value))
                    ErrorManager.ReportError("Runtime UI replacement to '{0}' failed since the value specified ({1}) for the '{2}' property is incompatible (type expected is '{3}')", replacementType.Name, uiPropertyRecord.Value, uiPropertyRecord.Name, uiPropertyRecord.Schema.PropertyType.Name);
            }
            foreach (string name in replacementType.FindRequiredPropertyNamesDeep())
            {
                if (!UIPropertyRecord.IsInList(list, name))
                    ErrorManager.ReportError("Runtime UI replacement to '{0}' failed since required property '{1}' was never provided a value", replacementType.Name, name);
            }
            return list;
        }

        protected override ViewItemID IDForChild(ViewItem childItem) => new ViewItemID(Source);

        protected override FindChildResult ChildForID(
          ViewItemID part,
          out ViewItem resultItem)
        {
            resultItem = null;
            FindChildResult findChildResult = FindChildResult.Failure;
            if (part.StringPartValid && !part.IDValid)
            {
                if (InvariantString.Equals(Source, part.StringPart) && ChildUI != null && ChildUI.RootItem != null)
                {
                    resultItem = ChildUI.RootItem;
                    findChildResult = FindChildResult.Success;
                }
                else if (_status == HostStatus.LoadingSource)
                    findChildResult = FindChildResult.PotentiallyFaultIn;
            }
            return findChildResult;
        }

        internal override void FaultInChild(ViewItemID childID, ChildFaultedInDelegate handler)
        {
            _loadNotify = handler;
            _loadNotifyURI = childID.StringPart;
        }

        public string Source => _lastRequestedSource;

        public TypeSchema SourceType => _typeCurrent;

        public bool Unloadable
        {
            get => _unloadable;
            set
            {
                if (_unloadable == value)
                    return;
                _unloadable = value;
                if (!_unloadable)
                    return;
                _islandId = MarkupSystem.AllocateIslandId();
                if (_islandId != 0U)
                    return;
                ErrorManager.ReportError("Maximum number of Unloadable Hosts reached");
            }
        }

        public bool UnloadAll() => UnloadAll(true);

        public bool UnloadAll(bool requestSourceToNull)
        {
            if (Unloadable)
            {
                MarkupSystem.UnloadIsland(_islandId);
                if (requestSourceToNull)
                    RequestSource(null, null, null);
                return true;
            }
            ErrorManager.ReportError("UnloadAll may only be called on Unloadable hosts");
            return false;
        }

        protected uint InheritedIslandId
        {
            get
            {
                if (_islandId == 0U)
                    _islandId = UI == null || UI.Host == null ? MarkupSystem.RootIslandId : UI.Host.InheritedIslandId;
                return _islandId;
            }
        }

        public void ForceRefresh() => ForceRefresh(false);

        public void ForceRefresh(bool unloadMarkup)
        {
            if (unloadMarkup)
            {
                string lastRequestedSource = _lastRequestedSource;
                RequestSource(null, null, null);
                DeferredCall.Post(DispatchPriority.High, new SimpleCallback(DeferredUnloadAll));
                DeferredCall.Post(DispatchPriority.High, new DeferredHandler(DeferredRequestSource), lastRequestedSource);
            }
            else
                RequestSource(_lastRequestedSource, null);
        }

        private void DeferredUnloadAll() => UnloadAll(false);

        private void DeferredRequestSource(object objLastRequestedSource) => RequestSource((string)objLastRequestedSource, null);

        public HostStatus Status => _status;

        private void SetStatus(HostStatus status)
        {
            if (_status == status)
                return;
            _status = status;
            FireNotification(NotificationID.Status);
        }

        public bool InputEnabled
        {
            get => _inputEnabled;
            set
            {
                if (_inputEnabled == value)
                    return;
                _inputEnabled = value;
                if (_childUI != null)
                    _childUI.OnInputEnabledChanged();
                FireNotification(NotificationID.InputEnabled);
            }
        }

        public UIClass ChildUI => _childUI;

        private void SetChildUI(UIClass childUI)
        {
            LoadResult loadResult = null;
            if (_childUI != null)
            {
                loadResult = _childUI.TypeSchema.Owner;
                _childUI.Dispose(this);
            }
            if (_dynamicHost)
            {
                loadResult?.UnregisterUsage(this);
                childUI?.TypeSchema.Owner.RegisterUsage(this);
            }
            _childUI = childUI;
        }

        protected override void CreateVisualContainer(IRenderSession renderSession)
        {
            base.CreateVisualContainer(renderSession);
            if (_childUI == null)
                return;
            _childUI.OnHostVisibilityChanged();
        }

        protected static string CrackSourceUri(string source, out string uiToCreate)
        {
            string str = source;
            uiToCreate = "Default";
            int length = source.LastIndexOf('#');
            if (length != -1)
            {
                str = source.Substring(0, length);
                uiToCreate = source.Substring(length + 1, source.Length - length - 1);
            }
            return str;
        }

        public void NotifyChildUIScriptErrors() => SetStatus(HostStatus.FailureRunningScript);

        public object GetChildUIProperty(string name) => _childUI == null ? UIPropertyRecord.FindInList(_heldUIProperties, name).Value : _childUI.GetProperty(name);

        public void SetChildUIProperty(string name, object value)
        {
            if (_childUI != null)
            {
                _childUI.SetProperty(name, value);
            }
            else
            {
                UIPropertyRecord inList = UIPropertyRecord.FindInList(_heldUIProperties, name);
                if (Utility.IsEqual(inList.Value, value))
                    return;
                inList.Value = value;
                FireNotification(name);
            }
        }

        public void FireChildUINotification(string id) => FireNotification(id);

        public bool NewContentOnTop
        {
            get => _newContentOnTop;
            set
            {
                if (_newContentOnTop == value)
                    return;
                _newContentOnTop = value;
                FireNotification(NotificationID.NewContentOnTop);
            }
        }

        protected override VisualOrder GetVisualOrder() => !NewContentOnTop ? VisualOrder.Last : VisualOrder.First;

        public override string ToString() => base.ToString() + " ('" + _lastRequestedSource + "')";
    }
}
