// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.UI.ViewItem
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Animations;
using Microsoft.Iris.Data;
using Microsoft.Iris.Debug;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Input;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Layouts;
using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.Navigation;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.ViewItems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Iris.UI
{
    public abstract class ViewItem :
      Microsoft.Iris.Library.TreeNode,
      INotifyObject,
      INavigationSite,
      IZoneDisplayChild,
      ITrackableUIElement,
      ITrackableUIElementEvents,
      IAnimatableOwner,
      ILayoutNode
    {
        private static readonly DataCookie s_maxSizeProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_minSizeProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_marginsProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_paddingProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_alignmentProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_childAlignmentProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_sharedSizeProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_sharedSizePolicyProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_layoutOutputProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_scaleProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_alphaProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_rotationProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_centerPointPercentProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_layerProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_activeAnimationsProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_animationBuildersProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_idleAnimationsProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_animationHandlesProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_navModeProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_navCacheProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_focusOrderProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_nameProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_effectProperty = DataCookie.ReserveSlot();
        protected static readonly DataCookie s_scaleNoSendProperty = DataCookie.ReserveSlot();
        protected static readonly DataCookie s_alphaNoSendProperty = DataCookie.ReserveSlot();
        protected static readonly DataCookie s_rotationNoSendProperty = DataCookie.ReserveSlot();
        protected static readonly DataCookie s_positionNoSendProperty = DataCookie.ReserveSlot();
        protected static readonly DataCookie s_sizeNoSendProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_cameraProperty = DataCookie.ReserveSlot();
        private static readonly DataCookie s_debugOutlineProperty = DataCookie.ReserveSlot();
        private static SingleArrayListCache s_pathListCache;
        private static EventCookie s_layoutCompleteEvent = EventCookie.ReserveSlot();
        private static EventCookie s_paintEvent = EventCookie.ReserveSlot();
        private static EventCookie s_paintInvalidEvent = EventCookie.ReserveSlot();
        private static EventCookie s_propertyChangedEvent = EventCookie.ReserveSlot();
        private static EventCookie s_deepLayoutChangeEvent = EventCookie.ReserveSlot();
        private UIClass _ownerUI;
        private ILayout _layout;
        private int _uiID;
        private BitVector32 _bits;
        private BitVector32 _bits2;
        private NotifyService _notifier = new NotifyService();
        private IVisualContainer _container;
        private ISprite _backgroundSprite;
        private Color _backgroundColor;
        private int _visibleChildCount;
        private Size _constraint;
        private Size _desiredSize;
        private object _measureData;
        private Size _alignedSize;
        private Point _measureAlignment;
        private LayoutSlot _slot;
        private Rectangle _bounds;
        private Vector3 _scale = Vector3.UnitVector;
        private Rotation _rotation = Rotation.Default;
        private Rectangle _location;
        private Visibility _visible;
        private Point _alignedOffset;
        private Vector<AreaOfInterest> _externallySetAreasOfInterest;
        private AreaOfInterestID _ownedAreasOfInterest;
        private AreaOfInterestID _containedAreasOfInterest;
        private int _requestedCount;
        private Vector<int> _requestedIndices;
        private ExtendedLayoutOutput _extendedOutputs;
        private static DeferredHandler s_scrollIntoViewCleanup = new DeferredHandler(CleanUpAfterScrollIntoView);

        public ViewItem()
        {
            _bits = new BitVector32();
            _bits2 = new BitVector32();
            SetBit(Bits.LayoutInputVisible, true);
            _layout = DefaultLayout.Instance;
            _backgroundColor = Color.Transparent;
        }

        protected override void OnOwnerDeclared(object owner)
        {
            _ownerUI = (UIClass)owner;
            _uiID = _ownerUI.AllocateUIID(this);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            RemoveEventHandlers(s_layoutCompleteEvent);
            RemoveEventHandlers(s_paintEvent);
            RemoveEventHandlers(s_paintInvalidEvent);
            RemoveEventHandlers(s_propertyChangedEvent);
            RemoveEventHandlers(s_deepLayoutChangeEvent);
            SharedSize?.Unregister(this);
            Vector<ActiveSequence> activeAnimations = GetActiveAnimations(false);
            if (activeAnimations != null)
            {
                foreach (DisposableObject disposableObject in activeAnimations)
                    disposableObject.Dispose(this);
            }
            Vector<ActiveSequence> idleAnimations = GetIdleAnimations(false);
            if (idleAnimations != null)
            {
                foreach (DisposableObject disposableObject in idleAnimations)
                    disposableObject.Dispose(this);
            }
            Effect?.DoneWithRenderEffects(this);
            _ownerUI = null;
            _layout = null;
            if (!GetBit(Bits2.HasCamera))
                return;
            Camera data = (Camera)GetData(s_cameraProperty);
            if (data == null)
                return;
            SetData(s_cameraProperty, null);
            data.UnregisterUsage(this);
        }

        protected void FireNotification(string id) => _notifier.Fire(id);

        public void AddListener(Listener listener) => _notifier.AddListener(listener);

        public bool HasVisual => _container != null;

        public IVisual RendererVisual => _container;

        IAnimatable IAnimatableOwner.AnimationTarget => _container;

        public virtual Color Background
        {
            get => _backgroundColor;
            set
            {
                if (!(_backgroundColor != value))
                    return;
                ForceContentChange();
                _backgroundColor = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.Background);
            }
        }

        protected virtual ISprite ContentVisual => _backgroundSprite;

        public virtual void OrphanVisuals(OrphanedVisualCollection orphans)
        {
            if (_container != null)
            {
                orphans.AddOrphan(_container);
                _container.UnregisterUsage(this);
                _container = null;
            }
            DisposeBackgroundContent(false);
        }

        public Vector2 VisualSize
        {
            get => GetBit(Bits2.HasSizeNoSend) ? (Vector2)GetData(s_sizeNoSendProperty) : _container.Size;
            set
            {
                bool bit = GetBit(Bits2.HasSizeNoSend);
                _container.SetSize(value, bit);
                if (bit)
                    SetDynamicValue(value, true, Bits2.HasSizeNoSend, s_sizeNoSendProperty, nameof(VisualSize));
                MarkPaintInvalid();
            }
        }

        private Vector2 VisualSizeNoSend
        {
            set => SetDynamicValue(value, false, Bits2.HasSizeNoSend, s_sizeNoSendProperty, "VisualSize");
        }

        public Vector3 VisualPosition
        {
            get => GetBit(Bits2.HasPositionNoSend) ? (Vector3)GetData(s_positionNoSendProperty) : _container.Position;
            set
            {
                bool bit = GetBit(Bits2.HasPositionNoSend);
                _container.SetPosition(value, bit);
                if (!bit)
                    return;
                SetDynamicValue(value, true, Bits2.HasPositionNoSend, s_positionNoSendProperty, nameof(VisualPosition));
            }
        }

        private Vector3 VisualPositionNoSend
        {
            set => SetDynamicValue(value, false, Bits2.HasPositionNoSend, s_positionNoSendProperty, "VisualPosition");
        }

        public Vector3 VisualScale
        {
            get => GetBit(Bits2.HasScaleNoSend) ? (Vector3)GetData(s_scaleNoSendProperty) : _container.Scale;
            set
            {
                bool bit = GetBit(Bits2.HasScaleNoSend);
                _container.SetScale(value, bit);
                if (!bit)
                    return;
                SetDynamicValue(value, true, Bits2.HasScaleNoSend, s_scaleNoSendProperty, nameof(VisualScale));
            }
        }

        private Vector3 VisualScaleNoSend
        {
            set => SetDynamicValue(value, false, Bits2.HasScaleNoSend, s_scaleNoSendProperty, "VisualScale");
        }

        public Rotation VisualRotation
        {
            get
            {
                if (GetBit(Bits2.HasRotationNoSend))
                    return (Rotation)GetData(s_rotationNoSendProperty);
                AxisAngle rotation = _container.Rotation;
                return new Rotation(rotation.Angle, rotation.Axis);
            }
            set
            {
                bool bit = GetBit(Bits2.HasRotationNoSend);
                _container.SetRotation(new AxisAngle(value.Axis, value.AngleRadians), bit);
                if (!bit)
                    return;
                SetDynamicValue(value, true, Bits2.HasRotationNoSend, s_rotationNoSendProperty, nameof(VisualRotation));
            }
        }

        private Rotation VisualRotationNoSend
        {
            set => SetDynamicValue(value, false, Bits2.HasRotationNoSend, s_rotationNoSendProperty, "VisualRotation");
        }

        public float VisualAlpha
        {
            get => GetBit(Bits2.HasAlphaNoSend) ? (float)GetData(s_alphaNoSendProperty) : _container.Alpha;
            set
            {
                bool fullyVisible = FullyVisible;
                bool bit = GetBit(Bits2.HasAlphaNoSend);
                _container.SetAlpha(value, bit);
                if (bit)
                    SetDynamicValue(value, true, Bits2.HasAlphaNoSend, s_alphaNoSendProperty, nameof(VisualAlpha));
                if (fullyVisible == FullyVisible)
                    return;
                OnVisibilityChange();
            }
        }

        private float VisualAlphaNoSend
        {
            set
            {
                bool fullyVisible = FullyVisible;
                SetDynamicValue(value, false, Bits2.HasAlphaNoSend, s_alphaNoSendProperty, "VisualAlpha");
                if (fullyVisible == FullyVisible)
                    return;
                OnVisibilityChange();
            }
        }

        public Vector3 VisualCenterPoint
        {
            get => _container.CenterPoint;
            set => _container.CenterPoint = value;
        }

        public uint VisualLayer
        {
            get => _container.Layer;
            set => _container.Layer = value;
        }

        private void SetDynamicValue(
          object value,
          bool fValueIsDefault,
          ViewItem.Bits2 bit,
          DataCookie cookie,
          string stTracePropertyName)
        {
            if (fValueIsDefault)
                SetData(cookie, null);
            else
                SetData(cookie, value);
            SetBit(bit, !fValueIsDefault);
        }

        public override bool IsRoot => IsZoned && Zone.RootViewItem == this;

        public ViewItem Parent => (ViewItem)base.Parent;

        public virtual bool HideNamedChildren => false;

        public UIClass UI => _ownerUI;

        protected override void OnZoneAttached()
        {
            UI.UpdateMouseHandling(this);
            MarkLayoutInvalid();
            if (GetBit(Bits.OutputSelfDirty))
                MarkLayoutOutputDirty(true);
            OnEffectiveScaleChange();
        }

        protected override void OnChildrenChanged()
        {
            base.OnChildrenChanged();
            MarkLayoutInvalid();
        }

        public void MarkPaintInvalid()
        {
            if (!IsZoned || GetBit(Bits.PaintInvalid) || !HasVisual)
                return;
            SetBit(Bits.PaintInvalid, true);
            if (!IsRoot)
                Parent.MarkPaintChildrenInvalid();
            Zone.ScheduleUiTask(UiTask.Painting);
            if (!(GetEventHandler(s_paintInvalidEvent) is EventHandler eventHandler))
                return;
            eventHandler(this, EventArgs.Empty);
        }

        private void MarkPaintChildrenInvalid()
        {
            if (ChildrenPaintInvalid)
                return;
            SetBit(Bits2.PaintChildrenInvalid, true);
            if (IsRoot)
                return;
            Parent.MarkPaintChildrenInvalid();
        }

        public bool ChildrenPaintInvalid => GetBit(Bits2.PaintChildrenInvalid);

        [Conditional("DEBUG")]
        private static void DEBUG_CountPaintInvalid()
        {
        }

        [Conditional("DEBUG")]
        internal static void DEBUG_ClearPaintInvalid()
        {
        }

        public void ResendExistingContentTree()
        {
            if (HasVisual)
                MarkPaintInvalid();
            foreach (ViewItem child in Children)
                child.ResendExistingContentTree();
        }

        public void OnVisibilityChange() => ResendExistingContentTree();

        public bool FullyVisible
        {
            get
            {
                if (!HasVisual || !Visible)
                    return false;
                return VisualAlpha > 0.0 || GetBit(Bits2.IsAlphaAnimationPlaying);
            }
        }

        public void PaintTree(bool visible)
        {
            visible &= FullyVisible;
            PaintSelf(visible);
            if (!ChildrenPaintInvalid)
                return;
            SetBit(Bits2.PaintChildrenInvalid, false);
            foreach (ViewItem child in Children)
                child.PaintTree(visible);
        }

        private void PaintSelf(bool visible)
        {
            if (!GetBit(Bits.PaintInvalid))
                return;
            SetBit(Bits.PaintInvalid, false);
            if (!HasVisual)
                return;
            OnPaint(visible);
        }

        protected virtual void OnPaint(bool visible)
        {
            if (GetEventHandler(s_paintEvent) is ViewItem.PaintHandler eventHandler)
                eventHandler(this);
            bool flag = visible && _backgroundColor.A != 0;
            if (flag && _backgroundSprite == null)
                CreateBackgroundContent();
            else if (!flag && _backgroundSprite != null)
                DisposeBackgroundContent(true);
            if (_backgroundSprite == null)
                return;
            _backgroundSprite.Effect.SetProperty("ColorElem.Color", _backgroundColor.RenderConvert());
        }

        public event ViewItem.PaintHandler Paint
        {
            add => AddEventHandler(s_paintEvent, value);
            remove => RemoveEventHandler(s_paintEvent, value);
        }

        public event EventHandler PaintInvalid
        {
            add => AddEventHandler(s_paintInvalidEvent, value);
            remove => RemoveEventHandler(s_paintInvalidEvent, value);
        }

        protected virtual void DisposeAllContent() => DisposeBackgroundContent(true);

        private void DisposeBackgroundContent(bool removeFromTree)
        {
            if (_backgroundSprite == null)
                return;
            if (removeFromTree)
                _backgroundSprite.Remove();
            _backgroundSprite.UnregisterUsage(this);
            _backgroundSprite = null;
        }

        private void CreateBackgroundContent()
        {
            _backgroundSprite = UISession.Default.RenderSession.CreateSprite(this, this);
            VisualContainer.AddChild(_backgroundSprite, null, VisualOrder.Last);
            IEffect colorFillEffect = EffectManager.CreateColorFillEffect(this, _backgroundColor);
            _backgroundSprite.Effect = colorFillEffect;
            _backgroundSprite.RelativeSize = true;
            _backgroundSprite.Size = Vector2.UnitVector;
            colorFillEffect.UnregisterUsage(this);
        }

        public Vector3 Scale
        {
            get
            {
                Vector3 vector3 = Vector3.UnitVector;
                if (GetBit(Bits.HasScale))
                {
                    object data = GetData(s_scaleProperty);
                    if (data != null)
                        vector3 = (Vector3)data;
                }
                return vector3;
            }
            set
            {
                if (!(value != Scale))
                    return;
                if (value != Vector3.UnitVector)
                {
                    SetData(s_scaleProperty, value);
                    SetBit(Bits.HasScale, true);
                }
                else if (GetBit(Bits.HasScale))
                {
                    SetData(s_scaleProperty, null);
                    SetBit(Bits.HasScale, false);
                }
                if (HasVisual)
                {
                    var args = new AnimationArgs(this)
                    {
                        NewScale = _scale * value
                    };
                    ApplyAnimatableValue(AnimationEventType.Scale, ref args);
                }
                else
                    MarkLayoutInvalid();
                NotifyEffectiveScaleChange(false);
                FireNotification(NotificationID.Scale);
            }
        }

        public float Alpha
        {
            get
            {
                float num = 1f;
                if (GetBit(Bits2.HasAlpha))
                    num = (float)GetData(s_alphaProperty);
                return num;
            }
            set
            {
                float alpha = Alpha;
                if (value == (double)alpha)
                    return;
                bool flag = Math2.WithinEpsilon(value, 1f);
                object obj = flag ? null : (object)value;
                SetData(s_alphaProperty, obj);
                SetBit(Bits2.HasAlpha, !flag);
                if (HasVisual)
                {
                    var args = new AnimationArgs(this)
                    {
                        OldAlpha = VisualAlpha,
                        NewAlpha = value
                    };
                    ApplyAnimatableValue(AnimationEventType.Alpha, ref args);
                    if (alpha == 0.0 || value == 0.0)
                        OnVisibilityChange();
                }
                FireNotification(NotificationID.Alpha);
            }
        }

        public Camera Camera
        {
            get
            {
                Camera camera = null;
                if (GetBit(Bits2.HasCamera))
                    camera = (Camera)GetData(s_cameraProperty);
                return camera;
            }
            set
            {
                if (value == Camera)
                    return;
                bool flag = value == null;
                object obj = flag ? null : (object)value;
                SetData(s_cameraProperty, obj);
                SetBit(Bits2.HasCamera, !flag);
                value?.RegisterUsage(this);
                if (_container != null)
                    _container.Camera = value == null ? null : value.APICamera;
                FireNotification(NotificationID.Camera);
            }
        }

        public Rotation Rotation
        {
            get
            {
                Rotation data = Rotation.Default;
                if (GetBit(Bits2.HasRotation))
                    data = (Rotation)GetData(s_rotationProperty);
                return data;
            }
            set
            {
                if (!(value != Rotation))
                    return;
                bool flag = value == Rotation.Default;
                object obj = flag ? null : (object)value;
                SetData(s_rotationProperty, obj);
                SetBit(Bits2.HasRotation, !flag);
                FireNotification(NotificationID.Rotation);
                MarkLayoutInvalid();
            }
        }

        public Vector3 CenterPointPercent
        {
            get
            {
                Vector3 vector3 = new Vector3();
                if (GetBit(Bits2.HasCenterPointPercent))
                    vector3 = (Vector3)GetData(s_centerPointPercentProperty);
                return vector3;
            }
            set
            {
                if (!(value != CenterPointPercent))
                    return;
                bool flag = value == new Vector3();
                object obj = flag ? null : (object)value;
                SetData(s_centerPointPercentProperty, obj);
                SetBit(Bits2.HasCenterPointPercent, !flag);
                if (HasVisual)
                    VisualCenterPoint = value;
                FireNotification(NotificationID.CenterPointPercent);
            }
        }

        public uint Layer
        {
            get
            {
                uint num = 0;
                if (GetBit(Bits2.HasLayer))
                    num = (uint)GetData(s_layerProperty);
                return num;
            }
            set
            {
                if ((int)value == (int)Layer)
                    return;
                bool flag = value == 0U;
                object obj = flag ? null : (object)value;
                SetData(s_layerProperty, obj);
                SetBit(Bits2.HasLayer, !flag);
                if (!HasVisual)
                    return;
                VisualLayer = value;
            }
        }

        public EffectClass Effect
        {
            get => GetBit(Bits2.HasEffect) ? (EffectClass)GetData(s_effectProperty) : null;
            set
            {
                EffectClass effect = Effect;
                if (effect == value)
                    return;
                effect?.DoneWithRenderEffects(this);
                SetData(s_effectProperty, value);
                SetBit(Bits2.HasEffect, value != null);
                OnEffectChanged();
                MarkPaintInvalid();
                FireNotification(NotificationID.Effect);
            }
        }

        protected virtual void OnEffectChanged()
        {
        }

        public Vector3 ComputeEffectiveScale()
        {
            Vector3 hostDisplayScale = Zone.HostDisplayScale;
            ViewItem viewItem = this;
            do
            {
                if (viewItem.HasVisual)
                {
                    Vector3 visualScale = viewItem.VisualScale;
                    hostDisplayScale *= new Vector3(visualScale.X, visualScale.Y, visualScale.Z);
                }
                else if (viewItem.GetBit(Bits.HasScale))
                    hostDisplayScale *= viewItem.Scale;
                viewItem = viewItem.Parent;
            }
            while (viewItem != null);
            return hostDisplayScale;
        }

        protected virtual void OnEffectiveScaleChange()
        {
        }

        internal void NotifyEffectiveScaleChange(bool forceFlag)
        {
            if (!IsZoned || !ChangeBit(Bits.ScaleChanged, true) && !forceFlag)
                return;
            Zone.ScheduleScaleChangeNotifications();
        }

        internal void DeliverEffectiveScaleChange(bool parentChangedFlag)
        {
            if (parentChangedFlag)
                SetBit(Bits.ScaleChanged, false);
            else
                parentChangedFlag = ChangeBit(Bits.ScaleChanged, false);
            if (parentChangedFlag)
                OnEffectiveScaleChange();
            foreach (ViewItem child in Children)
                child.DeliverEffectiveScaleChange(parentChangedFlag);
        }

        public bool Visible
        {
            get => GetBit(Bits.LayoutInputVisible);
            set
            {
                bool fullyVisible = FullyVisible;
                if (!ChangeBit(Bits.LayoutInputVisible, value))
                    return;
                MarkLayoutInvalid();
                if (fullyVisible != FullyVisible)
                    OnVisibilityChange();
                FireNotification(NotificationID.Visible);
            }
        }

        internal Size MaximumSize => (Size)MaximumSizeObject;

        public object MaximumSizeObject
        {
            get => GetBit(Bits.LayoutInputMaxSize) ? GetData(s_maxSizeProperty) : Size.ZeroBox;
            set
            {
                if (!(MaximumSize != (Size)value))
                    return;
                SetLayoutData(s_maxSizeProperty, Bits.LayoutInputMaxSize, value, Size.ZeroBox);
                FireNotification(NotificationID.MaximumSize);
            }
        }

        internal Size MinimumSize => (Size)MinimumSizeObject;

        public object MinimumSizeObject
        {
            get => GetBit(Bits.LayoutInputMinSize) ? GetData(s_minSizeProperty) : Size.ZeroBox;
            set
            {
                if (!(MinimumSize != (Size)value))
                    return;
                SetLayoutData(s_minSizeProperty, Bits.LayoutInputMinSize, value, Size.ZeroBox);
                FireNotification(NotificationID.MinimumSize);
                OnMinimumSizeChanged();
            }
        }

        protected virtual void OnMinimumSizeChanged()
        {
        }

        public ItemAlignment Alignment
        {
            get => GetBit(Bits.LayoutAlignment) ? (ItemAlignment)GetData(s_alignmentProperty) : ItemAlignment.Default;
            set
            {
                if (!(Alignment != value))
                    return;
                SetLayoutData(s_alignmentProperty, Bits.LayoutAlignment, value, ItemAlignment.Default);
                FireNotification(NotificationID.Alignment);
            }
        }

        public ItemAlignment ChildAlignment
        {
            get => GetBit(Bits.LayoutChildAlignment) ? (ItemAlignment)GetData(s_childAlignmentProperty) : ItemAlignment.Default;
            set
            {
                if (!(ChildAlignment != value))
                    return;
                SetLayoutData(s_childAlignmentProperty, Bits.LayoutChildAlignment, value, ItemAlignment.Default);
                FireNotification(NotificationID.ChildAlignment);
            }
        }

        internal ItemAlignment GetEffectiveAlignment()
        {
            ItemAlignment alignment = Alignment;
            if (Parent != null && (alignment.Horizontal == Iris.Layout.Alignment.Unspecified || alignment.Vertical == Iris.Layout.Alignment.Unspecified))
            {
                alignment = ItemAlignment.Merge(alignment, Parent.ChildAlignment);
                if (Parent.Layout != null)
                    alignment = ItemAlignment.Merge(alignment, Parent.Layout.DefaultChildAlignment);
            }
            return alignment;
        }

        public SharedSize SharedSize
        {
            get => GetBit(Bits.LayoutInputSharedSize) ? (SharedSize)GetData(s_sharedSizeProperty) : null;
            set
            {
                SharedSize sharedSize = SharedSize;
                if (sharedSize == value)
                    return;
                sharedSize?.Unregister(this);
                SetLayoutData(s_sharedSizeProperty, Bits.LayoutInputSharedSize, value, null);
                value?.Register(this);
                FireNotification(NotificationID.SharedSize);
            }
        }

        public SharedSizePolicy SharedSizePolicy
        {
            get => GetBit(Bits.LayoutInputSharedSizePolicy) ? (SharedSizePolicy)GetData(s_sharedSizePolicyProperty) : SharedSizePolicy.Default;
            set
            {
                if (SharedSizePolicy == value)
                    return;
                SetLayoutData(s_sharedSizePolicyProperty, Bits.LayoutInputSharedSizePolicy, value, SharedSizePolicy.Default);
                FireNotification(NotificationID.SharedSizePolicy);
            }
        }

        public Inset Margins
        {
            get => GetInset(s_marginsProperty, Bits.LayoutInputMargins);
            set
            {
                if (!(Margins != value))
                    return;
                SetLayoutData(s_marginsProperty, Bits.LayoutInputMargins, value, Inset.Zero);
                FireNotification(NotificationID.Margins);
            }
        }

        public Inset Padding
        {
            get => GetInset(s_paddingProperty, Bits.LayoutInputPadding);
            set
            {
                if (!(Padding != value))
                    return;
                SetLayoutData(s_paddingProperty, Bits.LayoutInputPadding, value, Inset.Zero);
                FireNotification(NotificationID.Padding);
            }
        }

        private Inset GetInset(DataCookie changeProperty, ViewItem.Bits changeBit) => GetBit(changeBit) ? (Inset)GetData(changeProperty) : Inset.Zero;

        private void SetLayoutData(
          DataCookie changeProperty,
          ViewItem.Bits changeBit,
          object newValue,
          object emptyValue)
        {
            uint bitAsUint = GetBitAsUInt(changeBit);
            uint num = newValue.Equals(emptyValue) ? 0U : 1U;
            if (num == 0U && (int)bitAsUint == (int)num)
                return;
            switch (bitAsUint << 1 | num)
            {
                case 0:
                    return;
                case 1:
                    SetData(changeProperty, newValue);
                    SetBit(changeBit, true);
                    break;
                case 2:
                    SetData(changeProperty, null);
                    SetBit(changeBit, false);
                    break;
                case 3:
                    if (GetData(changeProperty) == newValue)
                        return;
                    SetData(changeProperty, newValue);
                    SetBit(changeBit, true);
                    break;
            }
            MarkLayoutInvalid();
        }

        public ILayout Layout
        {
            get => _layout;
            set
            {
                if (_layout == value)
                    return;
                _layout = value;
                MarkLayoutInvalid();
                FireNotification(NotificationID.Layout);
            }
        }

        public ILayoutInput GetLayoutInput(DataCookie inputID) => (ILayoutInput)GetData(inputID);

        public void SetLayoutInput(ILayoutInput newValue) => SetLayoutInput(newValue.Data, newValue, true);

        public void SetLayoutInput(ILayoutInput newValue, bool invalidateLayout) => SetLayoutInput(newValue.Data, newValue, invalidateLayout);

        public void SetLayoutInput(DataCookie inputID, ILayoutInput newValue) => SetLayoutInput(inputID, newValue, true);

        public void SetLayoutInput(DataCookie inputID, ILayoutInput newValue, bool invalidateLayout)
        {
            if (GetData(inputID) == newValue)
                return;
            SetData(inputID, newValue);
            if (!invalidateLayout)
                return;
            MarkLayoutInvalid();
        }

        public LayoutOutput LayoutOutput
        {
            get
            {
                if (ChangeBit(Bits2.HasLayoutOutput, true))
                    SetData(s_layoutOutputProperty, new LayoutOutput(LayoutSize));
                return (LayoutOutput)GetData(s_layoutOutputProperty);
            }
        }

        public Rectangle LayoutBounds => _location;

        public Size LayoutSize => LayoutBounds.Size;

        public Point LayoutPosition => LayoutBounds.Location;

        public Vector3 LayoutScale => _scale;

        public Rotation LayoutRotation => _rotation;

        public bool LayoutOffscreen
        {
            get => GetBit(Bits2.LayoutOffscreen);
            private set => SetBit(Bits2.LayoutOffscreen, value);
        }

        public int LayoutRequestedCount => _requestedCount;

        public Vector<int> LayoutRequestedIndices => _requestedIndices;

        public bool LayoutVisible => _visible == Visibility.Visible;

        Size ILayoutNode.DesiredSize => DesiredSize;

        private Size DesiredSize
        {
            get
            {
                Size desiredSize = _desiredSize;
                if (!LayoutContributesToWidth)
                    desiredSize.Width = 0;
                return desiredSize;
            }
        }

        bool ILayoutNode.LayoutContributesToWidth
        {
            get => LayoutContributesToWidth;
            set => LayoutContributesToWidth = value;
        }

        private bool LayoutContributesToWidth
        {
            get => GetBit(Bits2.ContributesToWidth);
            set => SetBit(Bits2.ContributesToWidth, value);
        }

        public Size LayoutMaximumSize
        {
            get
            {
                Size maximumSize = MaximumSize;
                if (!LayoutContributesToWidth && (maximumSize.Width == 0 || maximumSize.Width > _desiredSize.Width))
                    maximumSize.Width = _desiredSize.Width;
                return maximumSize;
            }
        }

        object ILayoutNode.MeasureData
        {
            get => _measureData;
            set => _measureData = value;
        }

        Point ILayoutNode.AlignmentOffset => _measureAlignment;

        Size ILayoutNode.AlignedSize => _alignedSize;

        internal RectangleF GetDescendantFocusRect()
        {
            RectangleF rectangleF = RectangleF.Zero;
            if (UISession.InputManager.Queue.PendingKeyFocus is UIClass pendingKeyFocus)
            {
                ViewItem rootItem = pendingKeyFocus.RootItem;
                if (HasDescendant(rootItem))
                {
                    Vector3 positionPxlVector;
                    Vector3 sizePxlVector;
                    ((INavigationSite)rootItem).ComputeBounds(out positionPxlVector, out sizePxlVector);
                    rectangleF = new RectangleF(positionPxlVector.X, positionPxlVector.Y, sizePxlVector.X, sizePxlVector.Y);
                }
            }
            return rectangleF;
        }

        public bool IsOffscreen
        {
            get => GetBit(Bits2.IsOffscreen);
            set => SetBit(Bits2.IsOffscreen, value);
        }

        public ILayoutInput LayoutInput
        {
            set => SetLayoutInput(value.Data, value);
        }

        public bool LayoutInvalid => GetBit(Bits.LayoutInvalid);

        public void MarkLayoutInvalid()
        {
            if (!IsZoned || Zone.InfiniteLayoutLoopDetected)
                return;
            ViewItem viewItem = this;
            while (!viewItem.LayoutInvalid)
            {
                viewItem.ClearLayoutInfo();
                viewItem.SetBit(Bits.LayoutInvalid, true);
                viewItem = viewItem.Parent;
                if (viewItem == null)
                {
                    Zone.ScheduleUiTask(UiTask.LayoutComputation);
                    if (!DebugOutlines.Enabled)
                        break;
                    DebugOutlines.NotifyLayoutChange(this);
                    break;
                }
            }
        }

        private void ClearLayoutInfo()
        {
            SetBit(Bits2.BuiltLayoutChildren, false);
            _visibleChildCount = 0;
            Measured = false;
            _constraint = Size.Zero;
            Arranged = false;
            _slot = new LayoutSlot();
            _bounds = Rectangle.Zero;
        }

        public void ResetLayoutInvalid()
        {
            if (!ChangeBit(Bits.LayoutInvalid, false))
                return;
            foreach (ViewItem child in Children)
                child.ResetLayoutInvalid();
        }

        public event EventHandler DeepLayoutChange
        {
            add
            {
                if (!AddEventHandler(s_deepLayoutChangeEvent, value))
                    return;
                EnableDeepLayoutNotifications(true);
            }
            remove
            {
                if (!RemoveEventHandler(s_deepLayoutChangeEvent, value))
                    return;
                EnableDeepLayoutNotifications(false);
            }
        }

        private void EnableDeepLayoutNotifications(bool enableFlag)
        {
            if (!enableFlag)
            {
                SetBit(Bits.DeepLayoutNotifySelf, false);
            }
            else
            {
                if (GetBit(Bits.DeepLayoutNotifySelf))
                    return;
                SetBit(Bits.DeepLayoutNotifySelf, true);
                ViewItem viewItem = this;
                do
                {
                    viewItem.SetBit(Bits.DeepLayoutNotifyTree, true);
                    ViewItem parent = viewItem.Parent;
                    if (parent == null)
                        break;
                    viewItem = parent;
                }
                while (!viewItem.GetBit(Bits.DeepLayoutNotifyTree));
            }
        }

        private void BuildLayoutChildren()
        {
            if (GetBit(Bits2.BuiltLayoutChildren))
                return;
            SetBit(Bits2.BuiltLayoutChildren, true);
            _visibleChildCount = 0;
            foreach (ILayoutNode child in Children)
            {
                if (child.Visible)
                    ++_visibleChildCount;
                else
                    child.MarkHidden();
            }
        }

        int ILayoutNode.LayoutChildrenCount => LayoutChildrenCount;

        private int LayoutChildrenCount
        {
            get
            {
                BuildLayoutChildren();
                return _visibleChildCount;
            }
        }

        LayoutNodeEnumerator ILayoutNode.LayoutChildren => LayoutChildren;

        private LayoutNodeEnumerator LayoutChildren
        {
            get
            {
                BuildLayoutChildren();
                ILayoutNode start = null;
                if (_visibleChildCount > 0)
                {
                    start = (ILayoutNode)FirstChild;
                    if (!start.Visible)
                        start = start.NextVisibleSibling;
                }
                return new LayoutNodeEnumerator(start);
            }
        }

        ILayoutNode ILayoutNode.NextVisibleSibling
        {
            get
            {
                ILayoutNode nextSibling = (ILayoutNode)NextSibling;
                while (nextSibling != null && !nextSibling.Visible)
                    nextSibling = nextSibling.NextSibling;
                return nextSibling;
            }
        }

        ILayoutNode ILayoutNode.NextSibling => (ILayoutNode)NextSibling;

        Vector<int> ILayoutNode.GetSpecificChildrenRequestList()
        {
            Vector<int> vector = _requestedIndices;
            if (vector == null)
                vector = new Vector<int>();
            else
                vector.Clear();
            return vector;
        }

        void ILayoutNode.RequestSpecificChildren(Vector<int> requestedIndices) => _requestedIndices = requestedIndices;

        void ILayoutNode.RequestMoreChildren(int childrenCount) => _requestedCount = childrenCount;

        public ExtendedLayoutOutput GetExtendedLayoutOutput(DataCookie outputID)
        {
            ExtendedLayoutOutput extendedLayoutOutput = _extendedOutputs;
            while (extendedLayoutOutput != null && !(extendedLayoutOutput.OutputID == outputID))
                extendedLayoutOutput = extendedLayoutOutput.nextOutput;
            return extendedLayoutOutput;
        }

        void ILayoutNode.SetExtendedLayoutOutput(ExtendedLayoutOutput newDataOutput)
        {
            DataCookie outputId = newDataOutput.OutputID;
            ExtendedLayoutOutput extendedLayoutOutput1 = _extendedOutputs;
            ExtendedLayoutOutput extendedLayoutOutput2 = null;
            ExtendedLayoutOutput extendedLayoutOutput3 = null;
            for (; extendedLayoutOutput1 != null; extendedLayoutOutput1 = extendedLayoutOutput3)
            {
                extendedLayoutOutput3 = extendedLayoutOutput1.nextOutput;
                if (!(extendedLayoutOutput1.OutputID == outputId))
                    extendedLayoutOutput2 = extendedLayoutOutput1;
                else
                    break;
            }
            if (extendedLayoutOutput2 != null)
                extendedLayoutOutput2.nextOutput = newDataOutput;
            else
                _extendedOutputs = newDataOutput;
            newDataOutput.nextOutput = extendedLayoutOutput3;
            if (extendedLayoutOutput1 == null)
                return;
            extendedLayoutOutput1.nextOutput = null;
        }

        void ILayoutNode.AddAreaOfInterest(AreaOfInterest interest) => AreaOfInterest.AddAreaOfInterest(interest, ref _externallySetAreasOfInterest);

        void ILayoutNode.SetVisibleIndexRange(
          int beginVisible,
          int endVisible,
          int beginVisibleOffscreen,
          int endVisibleOffscreen,
          int? focusedItem)
        {
            if (!(GetExtendedLayoutOutput(VisibleIndexRangeLayoutOutput.DataCookie) is VisibleIndexRangeLayoutOutput rangeLayoutOutput))
            {
                rangeLayoutOutput = new VisibleIndexRangeLayoutOutput();
                ((ILayoutNode)this).SetExtendedLayoutOutput(rangeLayoutOutput);
            }
            rangeLayoutOutput.Initialize(beginVisible, endVisible, beginVisibleOffscreen, endVisibleOffscreen, focusedItem);
        }

        void ILayoutNode.MarkHidden() => MarkHidden();

        private void MarkHidden()
        {
            _visible = Visibility.ExplicitlyHidden;
            _desiredSize = Size.Zero;
            Measured = true;
            Arranged = true;
            Committed = false;
        }

        Size ILayoutNode.Measure(Size constraint)
        {
            if (Measured && _constraint == constraint)
                return _desiredSize;
            ResetMeasureInfo();
            _constraint = constraint;
            Size size1 = Margins.Size;
            constraint.Deflate(size1);
            Size layoutMaximumSize = LayoutMaximumSize;
            if (layoutMaximumSize.Width != 0)
                constraint.Width = Math.Min(layoutMaximumSize.Width, constraint.Width);
            if (layoutMaximumSize.Height != 0)
                constraint.Height = Math.Min(layoutMaximumSize.Height, constraint.Height);
            Size minimumSize = MinimumSize;
            SharedSize sharedSize = SharedSize;
            SharedSizePolicy sharedSizePolicy = SharedSizePolicy;
            sharedSize?.AdjustConstraint(ref constraint, ref minimumSize, sharedSizePolicy);
            Inset padding = Padding;
            Size size2 = padding.Size;
            constraint.Deflate(size2);
            minimumSize.Deflate(size2);
            if (constraint.IsEmpty || constraint.Width < minimumSize.Width || constraint.Height < minimumSize.Height)
            {
                MarkHidden();
                return Size.Zero;
            }
            Size sz2 = Layout.Measure(this, constraint);
            Size size3 = Size.Max(Size.Min(constraint, sz2), minimumSize);
            Size size4 = size3;
            if (!size3.IsZero)
            {
                Size sz1 = new Size(1, 1);
                Size size5 = Size.Max(sz1, size4 + padding.Size);
                sharedSize?.AccumulateSize(size5, sharedSizePolicy);
                size4 = Size.Max(sz1, size5 + size1);
            }
            _desiredSize = size4;
            Measured = true;
            ItemAlignment effectiveAlignment = GetEffectiveAlignment();
            _alignedSize.Width = Align(effectiveAlignment, Orientation.Horizontal, _constraint, ref _measureAlignment);
            _alignedSize.Height = Align(effectiveAlignment, Orientation.Vertical, _constraint, ref _measureAlignment);
            return DesiredSize;
        }

        private void ResetMeasureInfo()
        {
            _desiredSize = Size.Zero;
            LayoutContributesToWidth = true;
            _alignedSize = Size.Zero;
            _measureAlignment = Point.Zero;
            _visible = Visibility.Visible;
            _externallySetAreasOfInterest = null;
            _ownedAreasOfInterest = 0;
            _containedAreasOfInterest = 0;
            _extendedOutputs = null;
            _requestedCount = 0;
            _requestedIndices = null;
            ResetArrangeInfo();
        }

        private bool Measured
        {
            get => GetBit(Bits2.Measured);
            set => SetBit(Bits2.Measured, value);
        }

        private bool Arranged
        {
            get => GetBit(Bits2.Arranged);
            set => SetBit(Bits2.Arranged, value);
        }

        private bool Committed
        {
            get => GetBit(Bits2.Committed);
            set => SetBit(Bits2.Committed, value);
        }

        [Conditional("DEBUG")]
        private void CheckMeasured()
        {
        }

        void ILayoutNode.Arrange(LayoutSlot parentSlot) => ((ILayoutNode)this).Arrange(parentSlot, new Rectangle(Point.Zero, parentSlot.Bounds));

        void ILayoutNode.Arrange(LayoutSlot parentSlot, Rectangle bounds) => ((ILayoutNode)this).Arrange(parentSlot, bounds, Vector3.UnitVector, Rotation.Default);

        void ILayoutNode.Arrange(
          LayoutSlot parentSlot,
          Rectangle bounds,
          Vector3 scale,
          Rotation rotation)
        {
            if (bounds.Width == 0 || bounds.Height == 0)
            {
                MarkHidden();
            }
            else
            {
                if (_visible == Visibility.ExplicitlyHidden)
                    return;
                rotation = rotation != Rotation.Default ? rotation : Rotation;
                if (Arranged && _visible == Visibility.Visible && (parentSlot == _slot && bounds == _bounds) && (scale == _scale && rotation == _rotation))
                    return;
                ResetArrangeInfo();
                ItemAlignment effectiveAlignment = GetEffectiveAlignment();
                int num1 = Align(effectiveAlignment, Orientation.Horizontal, bounds.Size, ref _alignedOffset);
                int num2 = Align(effectiveAlignment, Orientation.Vertical, bounds.Size, ref _alignedOffset);
                Inset margins = Margins;
                int num3 = bounds.X + _alignedOffset.X + margins.Left;
                int num4 = bounds.Y + _alignedOffset.Y + margins.Top;
                int x = parentSlot.Offset.X + num3;
                int y = parentSlot.Offset.Y + num4;
                int width = num1 - (margins.Left + margins.Right);
                int height = num2 - (margins.Top + margins.Bottom);
                if (width <= 0 || height <= 0)
                {
                    _location = Rectangle.Zero;
                    _visible = Visibility.ImplicitlyHidden;
                    LayoutOffscreen = false;
                }
                else
                {
                    _location = new Rectangle(x, y, width, height);
                    bool flag = KeepAliveLayoutInput.ShouldKeepVisible(this);
                    if (!flag && Parent != null)
                    {
                        ViewItem parent = Parent;
                        flag = parent.GetBit(Bits2.KeepAlive) && !parent.DiscardOffscreenVisuals;
                    }
                    SetBit(Bits2.KeepAlive, flag);
                    SharedSize?.AccumulateSize(_location.Size, SharedSizePolicy);
                    Inset padding = Padding;
                    Size size = padding.Size;
                    Size extent = new Size(width, height);
                    extent.Width -= size.Width;
                    extent.Height -= size.Height;
                    bool processChildren = extent.Width > 0 && extent.Height > 0;
                    if (processChildren)
                    {
                        Point offset = new Point(padding.Left, padding.Top);
                        Point pos = new Point(-(num3 + offset.X), -(num4 + offset.Y));
                        Rectangle viewBounds = Rectangle.Offset(parentSlot.View, pos);
                        Rectangle viewPeripheralBounds = Rectangle.Offset(parentSlot.PeripheralView, pos);
                        Layout.Arrange(this, new LayoutSlot(extent, offset, viewBounds, viewPeripheralBounds));
                    }
                    ProcessAreasOfInterest(processChildren);
                    Rectangle location = _location;
                    AreaOfInterest area;
                    if (((ILayoutNode)this).TryGetAreaOfInterest(AreaOfInterestID.ScrollableRange, out area))
                        location.Union(area.Rectangle);
                    _visible = location.IntersectsWith(parentSlot.PeripheralView) || flag ? Visibility.Visible : Visibility.ImplicitlyHidden;
                    SetBit(Bits2.LayoutOffscreen, !location.IntersectsWith(parentSlot.View));
                }
                _slot = parentSlot;
                _bounds = bounds;
                _scale = scale;
                _rotation = rotation;
                Arranged = true;
            }
        }

        private void ResetArrangeInfo()
        {
            Arranged = false;
            _externallySetAreasOfInterest = null;
            _ownedAreasOfInterest = 0;
            _containedAreasOfInterest = 0;
            Committed = false;
        }

        private int Align(
          ItemAlignment alignment,
          Orientation orientation,
          Size slotSize,
          ref Point alignmentOffset)
        {
            int dimension1 = slotSize.GetDimension(orientation);
            int num = DesiredSize.GetDimension(orientation);
            switch (alignment.GetAlignment(orientation))
            {
                case Iris.Layout.Alignment.Unspecified:
                case Iris.Layout.Alignment.Fill:
                    int dimension2 = LayoutMaximumSize.GetDimension(orientation);
                    if (dimension2 > 0)
                    {
                        int val2 = dimension2 + Margins.Size.GetDimension(orientation);
                        num = Math.Min(dimension1, val2);
                        goto case Iris.Layout.Alignment.Near;
                    }
                    else
                    {
                        num = dimension1;
                        goto case Iris.Layout.Alignment.Near;
                    }
                case Iris.Layout.Alignment.Near:
                    alignmentOffset.SetDimension(orientation, 0);
                    break;
                case Iris.Layout.Alignment.Center:
                    alignmentOffset.SetDimension(orientation, (dimension1 - num) / 2);
                    break;
                case Iris.Layout.Alignment.Far:
                    alignmentOffset.SetDimension(orientation, dimension1 - num);
                    break;
            }
            return num;
        }

        private void ProcessAreasOfInterest(bool processChildren)
        {
            _ownedAreasOfInterest = 0;
            AreaOfInterestLayoutInput layoutInput = (AreaOfInterestLayoutInput)GetLayoutInput(AreaOfInterestLayoutInput.Data);
            if (layoutInput != null)
                _ownedAreasOfInterest = layoutInput.Id;
            if (PendingScrollIntoView)
                _ownedAreasOfInterest |= AreaOfInterestID.ScrollIntoViewRequest;
            if (_externallySetAreasOfInterest != null)
            {
                foreach (AreaOfInterest areaOfInterest in _externallySetAreasOfInterest)
                    _ownedAreasOfInterest |= areaOfInterest.Id;
            }
            _containedAreasOfInterest = 0;
            if (!processChildren)
                return;
            foreach (ViewItem layoutChild in LayoutChildren)
                _containedAreasOfInterest |= layoutChild._ownedAreasOfInterest | layoutChild._containedAreasOfInterest;
        }

        bool ILayoutNode.ContainsAreaOfInterest(AreaOfInterestID id) => (_ownedAreasOfInterest & id) != 0 || (_containedAreasOfInterest & id) != 0;

        bool ILayoutNode.TryGetAreaOfInterest(AreaOfInterestID id, out AreaOfInterest area)
        {
            if ((_ownedAreasOfInterest & id) != 0)
            {
                AreaOfInterestLayoutInput layoutInput = (AreaOfInterestLayoutInput)GetLayoutInput(AreaOfInterestLayoutInput.Data);
                if (layoutInput != null && layoutInput.Id == id)
                {
                    area = new AreaOfInterest(new Rectangle(Point.Zero, _location.Size), layoutInput.Margins, id);
                    return true;
                }
                if (id == AreaOfInterestID.ScrollIntoViewRequest)
                {
                    area = new AreaOfInterest(new Rectangle(Point.Zero, _location.Size), Inset.Zero, AreaOfInterestID.ScrollIntoViewRequest);
                    return true;
                }
                if (_externallySetAreasOfInterest != null)
                {
                    foreach (AreaOfInterest areaOfInterest in _externallySetAreasOfInterest)
                    {
                        if (areaOfInterest.Id == id)
                        {
                            area = areaOfInterest;
                            return true;
                        }
                    }
                }
            }
            else if ((_containedAreasOfInterest & id) != 0)
            {
                foreach (ILayoutNode layoutChild in LayoutChildren)
                {
                    AreaOfInterest area1;
                    if (layoutChild.TryGetAreaOfInterest(id, out area1))
                    {
                        Point layoutPosition = layoutChild.LayoutPosition;
                        AreaOfInterest interest = area1.Transform(layoutPosition);
                        ClipAreaOfInterest(ref interest, _location.Size);
                        area = interest;
                        return true;
                    }
                }
            }
            area = new AreaOfInterest();
            return false;
        }

        void ILayoutNode.Commit()
        {
            if (Committed)
                return;
            Committed = true;
            MarkLayoutOutputDirty(false);
            OnCommit();
            foreach (ILayoutNode child in Children)
                child.Commit();
        }

        public virtual void OnCommit()
        {
        }

        public void MarkLayoutOutputDirty(bool forceFlag)
        {
            if (!forceFlag && GetBit(Bits.OutputSelfDirty))
                return;
            SetBit(Bits.OutputSelfDirty, true);
            ViewItem viewItem = this;
            do
            {
                viewItem.SetBit(Bits.OutputTreeDirty, true);
                ViewItem parent = viewItem.Parent;
                if (parent == null)
                {
                    Zone.ScheduleUiTask(UiTask.LayoutApplication);
                    break;
                }
                viewItem = parent;
            }
            while (!viewItem.GetBit(Bits.OutputTreeDirty));
        }

        [Conditional("DEBUG")]
        public void DEBUG_ValidatePostLayoutState()
        {
            foreach (ViewItem child in Children)
                ;
        }

        public void ApplyLayoutOutputs(bool visibilityChanging)
        {
            if (!visibilityChanging && !GetBit(Bits.OutputTreeDirty))
                return;
            var args = new LayoutApplyParams()
            {
                fullyVisible = Zone.ZonePhysicalVisible,
                parentOffscreen = false,
                visibilityChanging = visibilityChanging,
                allowAnimations = true
            };
            ApplyLayoutOutputWorker(ref args);
        }

        private void ApplyLayoutOutputWorker(ref ViewItem.LayoutApplyParams selfApplyParams)
        {
            SetBit(Bits.OutputTreeDirty, false);
            bool flag1 = false;
            if (selfApplyParams.visibilityChanging || selfApplyParams.offscreenChanging || GetBit(Bits.OutputSelfDirty))
            {
                SetBit(Bits.OutputSelfDirty, false);
                bool flag2 = false;
                bool offscreenChange = false;
                if (_ownerUI == null)
                {
                    if (this is RootViewItem rootViewItem)
                    {
                        rootViewItem.ApplyRootLayoutOutput(selfApplyParams.fullyVisible, out flag2);
                        flag1 = true;
                        if (flag2)
                            selfApplyParams.allowAnimations = false;
                    }
                }
                else
                {
                    _ownerUI.ApplyLayoutOutput(this, selfApplyParams.fullyVisible, selfApplyParams.parentOffscreen, selfApplyParams.allowAnimations, out flag2, out offscreenChange);
                    flag1 = true;
                }
                if (flag2 && !selfApplyParams.visibilityChanging)
                    selfApplyParams.visibilityChanging = true;
                if (offscreenChange && !selfApplyParams.offscreenChanging)
                    selfApplyParams.offscreenChanging = true;
            }
            selfApplyParams.deepLayoutChanged |= flag1;
            if (!LayoutVisible)
                selfApplyParams.fullyVisible = false;
            if (LayoutOffscreen)
                selfApplyParams.parentOffscreen = true;
            if (HasChildren)
            {
                foreach (ViewItem child in Children)
                {
                    if (selfApplyParams.visibilityChanging || selfApplyParams.offscreenChanging || child.GetBit(Bits.OutputTreeDirty) || selfApplyParams.deepLayoutChanged && child.GetBit(Bits.DeepLayoutNotifyTree))
                    {
                        ViewItem.LayoutApplyParams selfApplyParams1 = new ViewItem.LayoutApplyParams();
                        selfApplyParams1.fullyVisible = selfApplyParams.fullyVisible;
                        selfApplyParams1.visibilityChanging = selfApplyParams.visibilityChanging;
                        selfApplyParams1.parentOffscreen = selfApplyParams.parentOffscreen;
                        selfApplyParams1.offscreenChanging = selfApplyParams.offscreenChanging;
                        selfApplyParams1.allowAnimations = selfApplyParams.allowAnimations;
                        selfApplyParams1.deepLayoutChanged = selfApplyParams.deepLayoutChanged;
                        child.ApplyLayoutOutputWorker(ref selfApplyParams1);
                        selfApplyParams.anyDeepChangesDelivered |= selfApplyParams1.anyDeepChangesDelivered;
                    }
                }
            }
            if (selfApplyParams.deepLayoutChanged && GetBit(Bits.DeepLayoutNotifyTree))
            {
                if (GetBit(Bits.DeepLayoutNotifySelf))
                {
                    if (GetEventHandler(s_deepLayoutChangeEvent) is EventHandler eventHandler)
                    {
                        eventHandler(this, EventArgs.Empty);
                        selfApplyParams.anyDeepChangesDelivered = true;
                    }
                    else
                        SetBit(Bits.DeepLayoutNotifySelf, false);
                }
                if (!selfApplyParams.anyDeepChangesDelivered)
                    SetBit(Bits.DeepLayoutNotifyTree, false);
            }
            if (!flag1)
                return;
            OnLayoutComplete(this);
        }

        public void OnScaleChange(Vector3 oldScaleVector, Vector3 newScaleVector) => NotifyEffectiveScaleChange(false);

        public void SetAreaOfInterest(AreaOfInterestID id, Inset margins) => SetLayoutInput(new AreaOfInterestLayoutInput(id, margins));

        public void ClearAreaOfInterest(AreaOfInterestID id)
        {
            if (!(GetLayoutInput(AreaOfInterestLayoutInput.Data) is AreaOfInterestLayoutInput layoutInput) || layoutInput.Id != id)
                return;
            SetLayoutInput(AreaOfInterestLayoutInput.Data, null);
        }

        bool ITrackableUIElement.IsUIVisible => IsVisibleToRenderer;

        Rectangle ITrackableUIElement.EstimatePosition(
          IZoneDisplayChild ancestor)
        {
            Vector3 positionPxlVector;
            Vector3 sizePxlVector;
            return ComputeBounds(ancestor, out positionPxlVector, out sizePxlVector) ? new Rectangle(positionPxlVector.X, positionPxlVector.Y, sizePxlVector.X, sizePxlVector.Y) : Rectangle.Zero;
        }

        event EventHandler ITrackableUIElementEvents.UIChange
        {
            add
            {
                DeepParentChange += value;
                DeepLayoutChange += value;
            }
            remove
            {
                DeepParentChange -= value;
                DeepLayoutChange -= value;
            }
        }

        public bool PlayAnimation(AnimationEventType type)
        {
            IAnimationProvider animation = GetAnimation(type);
            return animation != null && PlayAnimation(animation, GetAnimationHandle(type));
        }

        public bool PlayAnimation(IAnimationProvider ab, AnimationHandle animationHandle)
        {
            if (!HasVisual)
                return false;
            AnimationArgs args = new AnimationArgs(this);
            return PlayAnimation(ab, ref args, UIClass.ShouldPlayAnimation(ab), animationHandle);
        }

        public void ApplyAnimatableValue(AnimationEventType type, ref AnimationArgs args)
        {
            bool flag = false;
            IAnimationProvider animation = GetAnimation(type);
            if (animation != null)
                flag = PlayAnimation(animation, ref args, UIClass.ShouldPlayAnimation(animation), GetAnimationHandle(type));
            bool applyNow = !flag;
            if (applyNow)
                StopOverlappingAnimations(null, ActiveSequence.ConvertToActiveTransition(type));
            SetVisualValue(type, ref args, applyNow);
        }

        private void SetVisualValue(AnimationEventType type, ref AnimationArgs args, bool applyNow)
        {
            switch (type)
            {
                case AnimationEventType.Move:
                    if (!applyNow)
                    {
                        VisualPositionNoSend = args.NewPosition;
                        break;
                    }
                    VisualPosition = args.NewPosition;
                    break;
                case AnimationEventType.Size:
                    if (!applyNow)
                    {
                        VisualSizeNoSend = args.NewSize;
                        break;
                    }
                    VisualSize = args.NewSize;
                    break;
                case AnimationEventType.Scale:
                    if (!applyNow)
                    {
                        VisualScaleNoSend = args.NewScale;
                        break;
                    }
                    VisualScale = args.NewScale;
                    break;
                case AnimationEventType.Rotate:
                    if (!applyNow)
                    {
                        VisualRotationNoSend = args.NewRotation;
                        break;
                    }
                    VisualRotation = args.NewRotation;
                    break;
                case AnimationEventType.Alpha:
                    if (!applyNow)
                    {
                        VisualAlphaNoSend = args.NewAlpha;
                        break;
                    }
                    VisualAlpha = args.NewAlpha;
                    break;
            }
        }

        [Conditional("DEBUG")]
        private void DEBUG_ValidateSetVisualValue(
          AnimationEventType type,
          object oldValue,
          object newValue)
        {
        }

        private bool PlayAnimation(
          IAnimationProvider ab,
          ref AnimationArgs args,
          bool shouldPlayAnimation,
          AnimationHandle animationHandle)
        {
            AnimationTemplate anim = BuildAnimation(ab, ref args);
            if (anim == null)
                return false;
            if (shouldPlayAnimation)
            {
                PlayAnimation(anim, ref args, null, animationHandle);
            }
            else
            {
                ApplyFinalAnimationState(anim, ref args);
                animationHandle?.FireCompleted();
            }
            return true;
        }

        private void PlayAnimation(
          AnimationTemplate anim,
          ref AnimationArgs args,
          EventHandler onCompleteHandler,
          AnimationHandle animationHandle)
        {
            ActiveSequence instance = anim.CreateInstance(RendererVisual, ref args);
            if (instance == null)
                return;
            instance.DeclareOwner(this);
            if (onCompleteHandler != null)
                instance.AnimationCompleted += onCompleteHandler;
            animationHandle?.AssociateWithAnimationInstance(instance);
            instance.AnimationCompleted += new EventHandler(OnAnimationComplete);
            PlayAnimationWorker(instance, true);
            if (!(anim is Animation animation) || !animation.DisableMouseInput)
                return;
            UI.UpdateMouseHandling(this);
        }

        public void PlayShowAnimation()
        {
            IAnimationProvider ab = null;
            if (GetBit(Bits2.InsideContentChange))
            {
                ab = GetAnimation(AnimationEventType.ContentChangeShow);
                SetBit(Bits2.InsideContentChange, false);
            }
            if (ab == null)
                ab = GetAnimation(AnimationEventType.Show);
            if (ab != null)
                PlayAnimation(ab, GetAnimationHandle(AnimationEventType.Show));
            else
                TryToPlayIdleAnimation();
        }

        public void PlayHideAnimation(OrphanedVisualCollection orphans)
        {
            IAnimationProvider animationProvider = null;
            if (GetBit(Bits2.InsideContentChange))
                animationProvider = GetAnimation(AnimationEventType.ContentChangeHide);
            if (animationProvider == null)
                animationProvider = GetAnimation(AnimationEventType.Hide);
            if (animationProvider == null || !UIClass.ShouldPlayAnimation(animationProvider))
                return;
            AnimationArgs args = new AnimationArgs(this);
            AnimationTemplate animationTemplate = BuildAnimation(animationProvider, ref args);
            if (animationTemplate == null)
                return;
            if (animationTemplate.Loop == -1)
                animationTemplate.Loop = 0;
            ActiveSequence instance = animationTemplate.CreateInstance(RendererVisual, ref args);
            if (instance == null)
                return;
            orphans.RegisterWaitForAnimation(instance, false);
            PlayAnimationWorker(instance, false);
            TransferActiveAnimations(orphans);
        }

        private void TransferActiveAnimations(OrphanedVisualCollection orphans)
        {
            Vector<ActiveSequence> activeAnimations = GetActiveAnimations(false);
            TransferAnimationsList(orphans, activeAnimations, new EventHandler(OnAnimationComplete));
            SetData(s_activeAnimationsProperty, null);
            SetBit(Bits.ActiveAnimations, false);
            Vector<ActiveSequence> idleAnimations = GetIdleAnimations(false);
            TransferAnimationsList(orphans, idleAnimations, new EventHandler(OnIdleAnimationComplete));
            SetData(s_idleAnimationsProperty, null);
            SetBit(Bits.IdleAnimations, false);
            OnAnimationListChanged();
        }

        private void TransferAnimationsList(
          OrphanedVisualCollection orphans,
          Vector<ActiveSequence> animationsList,
          EventHandler handler)
        {
            if (animationsList == null)
                return;
            foreach (ActiveSequence animations in animationsList)
            {
                orphans.RegisterAnimation(animations, true);
                if (handler != null)
                    animations.AnimationCompleted -= handler;
                if (animations.Template.Loop == -1)
                    animations.Stop();
            }
        }

        public Dictionary<AnimationEventType, IAnimationProvider> GetAnimationSet() => !GetBit(Bits.AnimationBuilders) ? null : (Dictionary<AnimationEventType, IAnimationProvider>)GetData(s_animationBuildersProperty);

        public IAnimationProvider GetAnimation(AnimationEventType type)
        {
            Dictionary<AnimationEventType, IAnimationProvider> animationSet = GetAnimationSet();
            if (animationSet == null)
                return null;
            IAnimationProvider animationProvider;
            animationSet.TryGetValue(type, out animationProvider);
            return animationProvider;
        }

        public RelativeTo SnapshotPosition() => new SnapshotRelativeTo(BoundsRelativeToAncestor(null));

        public void AttachAnimation(IAnimationProvider animation) => SetAnimationData(animation.Type, animation);

        public void AttachAnimation(IAnimationProvider animation, AnimationHandle animationHandle)
        {
            AttachAnimation(animation);
            SetAnimationHandle(animation.Type, animationHandle);
        }

        public void DetachAnimation(AnimationEventType type)
        {
            SetAnimationData(type, null);
            SetAnimationHandle(type, null);
        }

        private Dictionary<AnimationEventType, AnimationHandle> GetAnimationHandleSet() => !GetBit(Bits2.AnimationHandles) ? null : (Dictionary<AnimationEventType, AnimationHandle>)GetData(s_animationHandlesProperty);

        public AnimationHandle GetAnimationHandle(AnimationEventType type)
        {
            Dictionary<AnimationEventType, AnimationHandle> animationHandleSet = GetAnimationHandleSet();
            if (animationHandleSet == null)
                return null;
            AnimationHandle animationHandle;
            animationHandleSet.TryGetValue(type, out animationHandle);
            return animationHandle;
        }

        public void SetAnimationHandle(AnimationEventType type, AnimationHandle animationHandle)
        {
            bool flag = animationHandle != null;
            Dictionary<AnimationEventType, AnimationHandle> dictionary = GetAnimationHandleSet();
            if (dictionary == null && flag)
            {
                dictionary = new Dictionary<AnimationEventType, AnimationHandle>();
                SetData(s_animationHandlesProperty, dictionary);
                SetBit(Bits2.AnimationHandles, true);
            }
            if (dictionary == null)
                return;
            if (flag)
            {
                dictionary[type] = animationHandle;
            }
            else
            {
                dictionary.Remove(type);
                if (dictionary.Count != 0)
                    return;
                SetData(s_animationHandlesProperty, null);
                SetBit(Bits2.AnimationHandles, false);
            }
        }

        private void PlayAnimationWorker(ActiveSequence newSequence, bool addToActiveAnimationList)
        {
            StopIdleAnimation();
            StopOverlappingAnimations(newSequence, newSequence.GetActiveTransitions());
            if (addToActiveAnimationList)
            {
                GetActiveAnimations(true).Add(newSequence);
                OnAnimationListChanged();
            }
            newSequence.Play();
        }

        private AnimationTemplate BuildAnimation(
          IAnimationProvider ab,
          ref AnimationArgs args)
        {
            return ab.Build(ref args);
        }

        private void StopOverlappingAnimations(
          ActiveSequence newSequence,
          ActiveTransitions newTransitions)
        {
            Vector<ActiveSequence> activeAnimations = GetActiveAnimations(false);
            if (activeAnimations == null)
                return;
            StopCommandSet stopCommand = null;
            foreach (ActiveSequence playingSequence in activeAnimations)
                StopAnimationIfOverlapping(playingSequence, newSequence, newTransitions, ref stopCommand);
        }

        private bool StopAnimationIfOverlapping(
          ActiveSequence playingSequence,
          ActiveSequence newSequence,
          ActiveTransitions newTransitions,
          ref StopCommandSet stopCommand)
        {
            IAnimatable animatable = VisualContainer;
            if (newSequence != null)
                animatable = newSequence.Target;
            if (playingSequence.Target == animatable)
            {
                ActiveTransitions activeTransitions = playingSequence.GetActiveTransitions();
                if ((newTransitions & activeTransitions) != ActiveTransitions.None)
                {
                    if (stopCommand == null && newSequence != null)
                    {
                        stopCommand = newSequence.GetStopCommandSet();
                        StopCommandSet stopCommandSet = stopCommand;
                    }
                    playingSequence.Stop(stopCommand);
                    return true;
                }
            }
            return false;
        }

        private void OnAnimationComplete(object sender, EventArgs args)
        {
            ActiveSequence activeSequence = sender as ActiveSequence;
            activeSequence.AnimationCompleted -= new EventHandler(OnAnimationComplete);
            Vector<ActiveSequence> activeAnimations = GetActiveAnimations(false);
            activeAnimations.Remove(activeSequence);
            OnAnimationListChanged();
            if (activeAnimations.Count == 0)
            {
                SetData(s_activeAnimationsProperty, null);
                SetBit(Bits.ActiveAnimations, false);
                TryToPlayIdleAnimation();
            }
            if (activeSequence.Template is Animation template && template.DisableMouseInput)
                UI.UpdateMouseHandling(this);
            activeSequence.Dispose(this);
        }

        private void OnIdleAnimationComplete(object sender, EventArgs args)
        {
            ActiveSequence activeSequence = sender as ActiveSequence;
            activeSequence.AnimationCompleted -= new EventHandler(OnIdleAnimationComplete);
            Vector<ActiveSequence> idleAnimations = GetIdleAnimations(false);
            idleAnimations.Remove(activeSequence);
            OnAnimationListChanged();
            if (idleAnimations.Count == 0)
            {
                SetData(s_idleAnimationsProperty, null);
                SetBit(Bits.IdleAnimations, false);
            }
            activeSequence.Dispose(this);
        }

        private bool TryToPlayIdleAnimation()
        {
            if (!HasVisual)
                return false;
            Vector<ActiveSequence> idleAnimations = GetIdleAnimations(false);
            ActiveSequence playingSequence = null;
            if (idleAnimations != null)
                playingSequence = idleAnimations[idleAnimations.Count - 1];
            IAnimationProvider animation = GetAnimation(AnimationEventType.Idle);
            if (animation == null)
                return false;
            AnimationArgs args = new AnimationArgs(this);
            AnimationTemplate anim = BuildAnimation(animation, ref args);
            if (anim == null)
                return false;
            if (!UIClass.ShouldPlayAnimation(animation))
            {
                ApplyFinalAnimationState(anim, ref args);
                return false;
            }
            ActiveSequence instance = anim.CreateInstance(RendererVisual, ref args);
            if (instance == null)
                return false;
            instance.DeclareOwner(this);
            if (playingSequence != null && playingSequence.Playing)
            {
                ActiveTransitions activeTransitions = instance.GetActiveTransitions();
                StopCommandSet stopCommand = null;
                if (StopAnimationIfOverlapping(playingSequence, instance, activeTransitions, ref stopCommand))
                    playingSequence = null;
            }
            instance.Play();
            if (playingSequence != null && playingSequence.Template.Loop == -1)
                playingSequence.Stop();
            if (instance != null)
            {
                GetIdleAnimations(true).Add(instance);
                OnAnimationListChanged();
                instance.AnimationCompleted += new EventHandler(OnIdleAnimationComplete);
            }
            return instance != null;
        }

        private void StopIdleAnimation()
        {
            Vector<ActiveSequence> idleAnimations = GetIdleAnimations(false);
            idleAnimations?[idleAnimations.Count - 1].Stop();
        }

        private void StopActiveAnimations()
        {
            if (!GetBit(Bits.ActiveAnimations))
                return;
            foreach (ActiveSequence activeAnimation in GetActiveAnimations(true))
                activeAnimation.Stop();
        }

        internal void ApplyFinalAnimationState(AnimationTemplate anim, ref AnimationArgs args)
        {
            if (!HasVisual)
                return;
            BaseKeyframe[] baseKeyframeArray = new BaseKeyframe[20];
            foreach (BaseKeyframe keyframe in anim.Keyframes)
            {
                BaseKeyframe baseKeyframe = baseKeyframeArray[(uint)keyframe.Type];
                if (baseKeyframe == null || baseKeyframe.Time <= (double)keyframe.Time)
                    baseKeyframeArray[(uint)keyframe.Type] = keyframe;
            }
            foreach (BaseKeyframe baseKeyframe in baseKeyframeArray)
                baseKeyframe?.Apply(this, ref args);
        }

        private void SetAnimationData(AnimationEventType type, IAnimationProvider anim)
        {
            bool flag = anim != null;
            Dictionary<AnimationEventType, IAnimationProvider> dictionary = GetAnimationSet();
            if (dictionary == null && flag)
            {
                dictionary = new Dictionary<AnimationEventType, IAnimationProvider>();
                SetData(s_animationBuildersProperty, dictionary);
                SetBit(Bits.AnimationBuilders, true);
            }
            if (dictionary != null)
            {
                if (flag)
                {
                    dictionary[type] = anim;
                }
                else
                {
                    dictionary.Remove(type);
                    if (dictionary.Count == 0)
                    {
                        SetData(s_animationBuildersProperty, null);
                        SetBit(Bits.AnimationBuilders, false);
                    }
                }
            }
            if (type != AnimationEventType.Idle)
                return;
            if (flag)
            {
                if (GetBit(Bits.ActiveAnimations))
                    return;
                TryToPlayIdleAnimation();
            }
            else
                StopIdleAnimation();
        }

        private Vector<ActiveSequence> GetActiveAnimations(bool createIfNone) => GetAnimationSequence(Bits.ActiveAnimations, s_activeAnimationsProperty, createIfNone);

        private Vector<ActiveSequence> GetIdleAnimations(bool createIfNone) => GetAnimationSequence(Bits.IdleAnimations, s_idleAnimationsProperty, createIfNone);

        private Vector<ActiveSequence> GetAnimationSequence(
          ViewItem.Bits propertyHint,
          DataCookie dynamicProperty,
          bool createIfNone)
        {
            Vector<ActiveSequence> vector = null;
            if (!GetBit(propertyHint))
            {
                if (createIfNone)
                {
                    vector = new Vector<ActiveSequence>();
                    SetData(dynamicProperty, vector);
                    SetBit(propertyHint, true);
                }
            }
            else
                vector = (Vector<ActiveSequence>)GetData(dynamicProperty);
            return vector;
        }

        private void OnAnimationListChanged()
        {
            if (!ChangeBit(Bits2.IsAlphaAnimationPlaying, DoesAnimationListContainAnimationType(GetActiveAnimations(false), ActiveTransitions.Alpha) || DoesAnimationListContainAnimationType(GetIdleAnimations(false), ActiveTransitions.Alpha)) || Alpha != 0.0)
                return;
            OnVisibilityChange();
        }

        private static bool DoesAnimationListContainAnimationType(
          Vector<ActiveSequence> animationList,
          ActiveTransitions type)
        {
            if (animationList != null)
            {
                foreach (ActiveSequence animation in animationList)
                {
                    if ((animation.GetActiveTransitions() & type) == type)
                        return true;
                }
            }
            return false;
        }

        public bool MouseInteractive
        {
            get => GetBit(Bits.MouseInteractive);
            set
            {
                if (!ChangeBit(Bits.MouseInteractive, value))
                    return;
                UI.UpdateMouseHandling(this);
                FireNotification(NotificationID.MouseInteractive);
            }
        }

        public bool ClipMouse
        {
            get => GetBit(Bits.ClipMouse);
            set
            {
                if (!ChangeBit(Bits.ClipMouse, value) || UI == null)
                    return;
                UI.UpdateMouseHandling(this);
            }
        }

        public bool AllowMouseInput
        {
            get
            {
                Vector<ActiveSequence> activeAnimations = GetActiveAnimations(false);
                if (activeAnimations != null)
                {
                    foreach (ActiveSequence activeSequence in activeAnimations)
                    {
                        if (activeSequence.Template is Animation template && template.DisableMouseInput)
                            return false;
                    }
                }
                return true;
            }
        }

        protected virtual void CreateVisualContainer(IRenderSession renderSession)
        {
            if (_container != null)
                return;
            VisualContainer = renderSession.CreateVisualContainer(this, this);
        }

        internal void CreateVisual(IRenderSession renderSession)
        {
            CreateVisualContainer(renderSession);
            AddVisualToParent(renderSession, _container);
            VisualScale = Scale;
            VisualAlpha = Alpha;
            VisualRotation = Rotation;
            VisualCenterPoint = CenterPointPercent;
            if (GetBit(Bits.PendingNavigateInto) && !GetBit(Bits.PendingNavigateIntoScheduled))
                ScheduleNavigateInto();
            MarkPaintInvalid();
        }

        protected virtual VisualOrder GetVisualOrder() => VisualOrder.Last;

        private void AddVisualToParent(IRenderSession renderSession, IVisual visual)
        {
            ViewItem viewItem = this;
            do
            {
                viewItem = (ViewItem)viewItem.NextSibling;
            }
            while (viewItem != null && !viewItem.HasVisual);
            ViewItem parent = Parent;
            VisualOrder nOrder = parent.GetVisualOrder();
            IVisual vSibling = null;
            if (viewItem != null)
            {
                vSibling = viewItem.RendererVisual;
                nOrder = VisualOrder.Before;
            }
            if (vSibling == null)
            {
                vSibling = parent.ContentVisual;
                nOrder = vSibling == null ? nOrder : VisualOrder.Before;
            }
            parent.VisualContainer.AddChild(visual, vSibling, nOrder);
        }

        protected IVisualContainer VisualContainer
        {
            get => _container;
            set
            {
                _container = value;
                if (!GetBit(Bits2.HasCamera))
                    return;
                _container.Camera = ((Camera)GetData(s_cameraProperty)).APICamera;
            }
        }

        private bool IsVisibleToRenderer => HasVisual || GetBit(Bits2.InsideContentChange);

        public void ForceContentChange()
        {
            if (!HasVisual)
                return;
            Dictionary<AnimationEventType, IAnimationProvider> animationSet = GetAnimationSet();
            if (animationSet == null || !animationSet.ContainsKey(AnimationEventType.ContentChangeShow) && !animationSet.ContainsKey(AnimationEventType.ContentChangeHide))
                return;
            SetBit(Bits2.InsideContentChange, true);
            if (UI == null)
                return;
            UI.DestroyVisualTree(this, true);
        }

        TransformSet IZoneDisplayChild.Transforms
        {
            get
            {
                TransformSet transformSet = new TransformSet();
                if (HasVisual)
                {
                    transformSet.positionPxlVector = VisualPosition;
                    transformSet.sizePxlVector = new Vector3(VisualSize.X, VisualSize.Y, 0.0f);
                    transformSet.scaleVector = VisualScale;
                    transformSet.centerPointScaleVector = VisualCenterPoint;
                }
                else
                    transformSet.scaleVector = Vector3.UnitVector;
                return transformSet;
            }
        }

        bool INavigationSite.ComputeBounds(
          out Vector3 positionPxlVector,
          out Vector3 sizePxlVector)
        {
            return ComputeBounds(null, out positionPxlVector, out sizePxlVector);
        }

        private bool ComputeBounds(
          IZoneDisplayChild ancestor,
          out Vector3 positionPxlVector,
          out Vector3 sizePxlVector)
        {
            positionPxlVector = Vector3.Zero;
            sizePxlVector = Vector3.Zero;
            if (!HasVisual)
                return false;
            Vector3 parentOffsetPxlVector;
            Vector3 scaleVector;
            GetAccumulatedOffsetAndScale(this, ancestor, out parentOffsetPxlVector, out scaleVector);
            positionPxlVector = parentOffsetPxlVector;
            Vector2 visualSize = VisualSize;
            sizePxlVector = new Vector3(visualSize.X, visualSize.Y, 0.0f) * scaleVector;
            return true;
        }

        public static void GetAccumulatedOffsetAndScale(
          IZoneDisplayChild childStart,
          IZoneDisplayChild childStop,
          out Vector3 parentOffsetPxlVector,
          out Vector3 scaleVector)
        {
            parentOffsetPxlVector = Vector3.Zero;
            scaleVector = Vector3.UnitVector;
            ArrayList arrayList = s_pathListCache.Acquire();
            if (!((ViewItem)childStart).GetParentChain(childStop as ViewItem, arrayList))
            {
                s_pathListCache.Release(arrayList);
            }
            else
            {
                for (int index = arrayList.Count - 1; index >= 0; --index)
                {
                    Vector3 vector3_1 = Vector3.Zero;
                    Vector3 vector3_2 = Vector3.UnitVector;
                    IZoneDisplayChild zoneDisplayChild = arrayList[index] as IZoneDisplayChild;
                    if (arrayList[index] is ViewItem viewItem && !viewItem.HasVisual)
                    {
                        Point layoutPosition = viewItem.LayoutPosition;
                        vector3_1 = new Vector3(layoutPosition.X, layoutPosition.Y, 0.0f);
                    }
                    else if (zoneDisplayChild != null)
                    {
                        TransformSet transforms = zoneDisplayChild.Transforms;
                        vector3_1 = transforms.positionPxlVector;
                        vector3_2 = transforms.scaleVector;
                    }
                    parentOffsetPxlVector += vector3_1 * scaleVector;
                    scaleVector *= vector3_2;
                }
                s_pathListCache.Release(arrayList);
            }
        }

        public RectangleF TransformFromAncestor(ViewItem ancestor, RectangleF rect)
        {
            Vector3 parentOffsetPxlVector;
            Vector3 scaleVector;
            GetAccumulatedOffsetAndScale(this, ancestor, out parentOffsetPxlVector, out scaleVector);
            rect.X -= parentOffsetPxlVector.X;
            rect.Y -= parentOffsetPxlVector.Y;
            rect.X /= scaleVector.X;
            rect.Y /= scaleVector.Y;
            rect.Width /= scaleVector.X;
            rect.Height /= scaleVector.Y;
            return rect;
        }

        public RectangleF TransformToAncestor(ViewItem ancestor, RectangleF rect)
        {
            Vector3 parentOffsetPxlVector;
            Vector3 scaleVector;
            GetAccumulatedOffsetAndScale(this, ancestor, out parentOffsetPxlVector, out scaleVector);
            rect.X *= scaleVector.X;
            rect.Y *= scaleVector.Y;
            rect.Width *= scaleVector.X;
            rect.Height *= scaleVector.Y;
            rect.X += parentOffsetPxlVector.X;
            rect.Y += parentOffsetPxlVector.Y;
            return rect;
        }

        public RectangleF BoundsRelativeToAncestor(ViewItem ancestor)
        {
            Vector3 parentOffsetPxlVector;
            Vector3 scaleVector;
            GetAccumulatedOffsetAndScale(this, ancestor, out parentOffsetPxlVector, out scaleVector);
            Vector2 vector2 = HasVisual ? VisualSize : Vector2.Zero;
            return new RectangleF(parentOffsetPxlVector.X, parentOffsetPxlVector.Y, vector2.X * scaleVector.X, vector2.Y * scaleVector.Y);
        }

        public Point ScreenToClient(Point screenPoint)
        {
            Zone.Form.ScreenToClient(ref screenPoint);
            return TransformFromAncestor(null, new RectangleF(screenPoint, Size.Zero)).Location.ToPoint();
        }

        public Point WindowToClient(Point windowPoint) => TransformFromAncestor(null, new RectangleF(windowPoint, Size.Zero)).Location.ToPoint();

        public Point ClientToWindow(Point clientPoint) => TransformToAncestor(null, new RectangleF(clientPoint, Size.Zero)).Location.ToPoint();

        public Point ClientToScreen(Point clientPoint)
        {
            Point window = ClientToWindow(clientPoint);
            Zone.Form.ClientToScreen(ref window);
            return window;
        }

        public virtual void ClipAreaOfInterest(ref AreaOfInterest interest, Size usedSize)
        {
        }

        private bool GetParentChain(ViewItem itemStop, ArrayList pathList)
        {
            pathList.Clear();
            ViewItem parent;
            for (ViewItem viewItem = this; viewItem != itemStop; viewItem = parent)
            {
                pathList.Add(viewItem);
                parent = viewItem.Parent;
                if (parent == null)
                {
                    if (!viewItem.IsRoot)
                        return false;
                    break;
                }
            }
            return true;
        }

        protected virtual void OnLayoutComplete(ViewItem sender)
        {
            if (GetEventHandler(s_layoutCompleteEvent) is LayoutCompleteEventHandler eventHandler)
                eventHandler(sender);
            if (!GetBit(Bits2.HasLayoutOutput))
                return;
            LayoutOutput.OnLayoutComplete(LayoutSize);
        }

        private void HACK_RemoveCachedScrollIntoViewAreasOfInterest()
        {
            for (ViewItem viewItem = this; viewItem != null; viewItem = viewItem.Parent)
            {
                viewItem._ownedAreasOfInterest &= ~AreaOfInterestID.ScrollIntoViewRequest;
                viewItem._containedAreasOfInterest &= ~AreaOfInterestID.ScrollIntoViewRequest;
            }
        }

        public event LayoutCompleteEventHandler LayoutComplete
        {
            add => AddEventHandler(s_layoutCompleteEvent, value);
            remove => RemoveEventHandler(s_layoutCompleteEvent, value);
        }

        internal void ClearStickyFocus() => NavigationServices.ClearDefaultFocus(this);

        public void ScrollIntoView()
        {
            if (PendingScrollIntoView)
                return;
            SetBit(Bits2.PendingScrollIntoView, true);
            LockVisible(true);
            DeferredCall.Post(DispatchPriority.LayoutSync, s_scrollIntoViewCleanup, this);
        }

        private static void CleanUpAfterScrollIntoView(object obj) => ((ViewItem)obj).CleanUpAfterScrollIntoView();

        private void CleanUpAfterScrollIntoView()
        {
            if (!PendingScrollIntoView)
                return;
            SetBit(Bits2.PendingScrollIntoView, false);
            UnlockVisible();
            HACK_RemoveCachedScrollIntoViewAreasOfInterest();
        }

        public bool PendingScrollIntoView => GetBit(Bits2.PendingScrollIntoView);

        public virtual bool DiscardOffscreenVisuals
        {
            get => false;
            set
            {
            }
        }

        public void LockVisible() => LockVisible(false);

        private void LockVisible(bool invalidateLayout)
        {
            KeepAliveLayoutInput aliveLayoutInput = (KeepAliveLayoutInput)GetLayoutInput(KeepAliveLayoutInput.Data);
            if (aliveLayoutInput == null)
            {
                aliveLayoutInput = new KeepAliveLayoutInput();
                SetLayoutInput(aliveLayoutInput, invalidateLayout);
            }
            ++aliveLayoutInput.Count;
        }

        public void UnlockVisible()
        {
            KeepAliveLayoutInput layoutInput = (KeepAliveLayoutInput)GetLayoutInput(KeepAliveLayoutInput.Data);
            --layoutInput.Count;
            if (layoutInput.Count != 0)
                return;
            SetLayoutInput(KeepAliveLayoutInput.Data, null, false);
        }

        public void NavigateInto() => NavigateInto(false);

        public void NavigateInto(bool isDefault)
        {
            if (ChangeBit(Bits.PendingNavigateInto, true))
            {
                LockVisible(!IsVisibleToRenderer);
                if (IsVisibleToRenderer)
                    ScheduleNavigateInto();
            }
            SetBit(Bits.PendingNavigateIntoIsDefault, isDefault);
        }

        private void ScheduleNavigateInto()
        {
            DeferredCall.Post(DispatchPriority.LayoutSync, new SimpleCallback(NavigateIntoWorker));
            SetBit(Bits.PendingNavigateIntoScheduled, true);
        }

        private void NavigateIntoWorker()
        {
            if (IsDisposed)
                return;
            UnlockVisible();
            SetBit(Bits.PendingNavigateInto, false);
            SetBit(Bits.PendingNavigateIntoScheduled, false);
            INavigationSite resultSite;
            if (!NavigationServices.FindNextWithin(this, Direction.Next, RectangleF.Zero, out resultSite) || resultSite == null || !(resultSite is ViewItem viewItem))
                return;
            viewItem.UI.NotifyNavigationDestination(GetBit(Bits.PendingNavigateIntoIsDefault) ? KeyFocusReason.Default : KeyFocusReason.Other);
        }

        public NavigationPolicies Navigation
        {
            get
            {
                NavigationPolicies navigationPolicies = NavigationPolicies.None;
                if (GetBit(Bits.HasNavMode))
                {
                    object data = GetData(s_navModeProperty);
                    if (data != null)
                        navigationPolicies = (NavigationPolicies)data;
                }
                return navigationPolicies;
            }
            set
            {
                if (value != NavigationPolicies.None)
                {
                    SetData(s_navModeProperty, value);
                    SetBit(Bits.HasNavMode, true);
                    FireNotification(NotificationID.Navigation);
                }
                else
                {
                    if (!GetBit(Bits.HasNavMode))
                        return;
                    SetData(s_navModeProperty, null);
                    SetBit(Bits.HasNavMode, false);
                    FireNotification(NotificationID.Navigation);
                }
            }
        }

        protected virtual NavigationPolicies ForcedNavigationFlags => NavigationPolicies.None;

        public int FocusOrder
        {
            get
            {
                int num = int.MaxValue;
                if (GetBit(Bits.HasFocusOrder))
                {
                    object data = GetData(s_focusOrderProperty);
                    if (data != null)
                        num = (int)data;
                }
                return num;
            }
            set
            {
                if (value != int.MaxValue)
                {
                    SetData(s_focusOrderProperty, value);
                    SetBit(Bits.HasFocusOrder, true);
                    FireNotification(NotificationID.FocusOrder);
                }
                else
                {
                    if (!GetBit(Bits.HasFocusOrder))
                        return;
                    SetData(s_focusOrderProperty, null);
                    SetBit(Bits.HasFocusOrder, false);
                    FireNotification(NotificationID.FocusOrder);
                }
            }
        }

        object INavigationSite.UniqueId => IDPath;

        public ViewItemID[] IDPath
        {
            get
            {
                int length = 0;
                for (ViewItem viewItem = this; viewItem.Parent != null; viewItem = viewItem.Parent)
                    ++length;
                if (length == 0)
                    return null;
                ViewItemID[] viewItemIdArray = new ViewItemID[length];
                int index = length - 1;
                for (ViewItem childItem = this; childItem.Parent != null; childItem = childItem.Parent)
                {
                    viewItemIdArray[index] = childItem.Parent.IDForChild(childItem);
                    --index;
                }
                return viewItemIdArray;
            }
        }

        INavigationSite INavigationSite.Parent => Parent;

        ICollection INavigationSite.Children => Children;

        bool INavigationSite.Visible => IsVisibleToRenderer;

        NavigationClass INavigationSite.Navigability
        {
            get
            {
                NavigationClass navigationClass = NavigationClass.None;
                if (_ownerUI != null)
                    navigationClass = _ownerUI.GetNavigability(this);
                return navigationClass;
            }
        }

        NavigationPolicies INavigationSite.Mode => Navigation | ForcedNavigationFlags;

        int INavigationSite.FocusOrder => FocusOrder;

        bool INavigationSite.IsLogicalJunction => this is Host;

        string INavigationSite.Description => Name;

        object INavigationSite.StateCache
        {
            get => GetData(s_navCacheProperty);
            set => SetData(s_navCacheProperty, value);
        }

        internal virtual void FaultInChild(ViewItemID component, ChildFaultedInDelegate handler)
        {
        }

        internal FindChildResult FindChildFromPath(
          ViewItemID[] parts,
          out ViewItem resultItem,
          out ViewItemID failedComponent)
        {
            FindChildResult findChildResult = FindChildResult.Failure;
            resultItem = Zone.RootViewItem;
            failedComponent = new ViewItemID();
            for (int index = 0; index < parts.Length; ++index)
            {
                ViewItem resultItem1;
                findChildResult = resultItem.ChildForID(parts[index], out resultItem1);
                if (findChildResult == FindChildResult.Success)
                {
                    resultItem = resultItem1;
                }
                else
                {
                    failedComponent = parts[index];
                    break;
                }
            }
            return findChildResult;
        }

        INavigationSite INavigationSite.LookupChildById(
          object uniqueIDObject)
        {
            INavigationSite navigationSite = null;
            ViewItem resultItem;
            if (uniqueIDObject is ViewItemID[] parts && FindChildFromPath(parts, out resultItem, out ViewItemID _) == FindChildResult.Success)
                navigationSite = resultItem;
            return navigationSite;
        }

        protected virtual ViewItemID IDForChild(ViewItem childItem) => new ViewItemID(childItem._uiID);

        protected virtual FindChildResult ChildForID(
          ViewItemID part,
          out ViewItem resultItem)
        {
            resultItem = null;
            if (part.IDValid && !part.StringPartValid)
            {
                foreach (ViewItem child in Children)
                {
                    if (child._uiID == part.ID)
                    {
                        resultItem = child;
                        break;
                    }
                }
            }
            return resultItem == null ? FindChildResult.Failure : FindChildResult.Success;
        }

        private bool GetBit(ViewItem.Bits lookupBit) => _bits[(int)lookupBit];

        protected bool GetBit(ViewItem.Bits2 lookupBit) => _bits2[(int)lookupBit];

        private uint GetBitAsUInt(ViewItem.Bits lookupBit) => ((ViewItem.Bits)_bits.Data & lookupBit) == ~(Bits.PendingNavigateInto | Bits.PendingNavigateIntoIsDefault | Bits.PendingNavigateIntoScheduled | Bits.ClipMouse | Bits.MouseInteractive | Bits.PaintInvalid | Bits.HasScale | Bits.ScaleChanged | Bits.LayoutInputMaxSize | Bits.LayoutInputMinSize | Bits.LayoutInputMargins | Bits.LayoutInputPadding | Bits.LayoutInputVisible | Bits.LayoutAlignment | Bits.LayoutChildAlignment | Bits.LayoutInputSharedSize | Bits.LayoutInputSharedSizePolicy | Bits.OutputSelfDirty | Bits.OutputTreeDirty | Bits.LayoutInvalid | Bits.ActiveAnimations | Bits.AnimationBuilders | Bits.IdleAnimations | Bits.HasNavMode | Bits.HasFocusOrder | Bits.DeepLayoutNotifySelf | Bits.DeepLayoutNotifyTree | Bits.Unused1 | Bits.Unused2 | Bits.Unused3 | Bits.Unused4 | Bits.Unused5) ? 0U : 1U;

        protected uint GetBitAsUInt(ViewItem.Bits2 lookupBit) => ((ViewItem.Bits2)_bits2.Data & lookupBit) == 0 ? 0U : 1U;

        private void SetBit(ViewItem.Bits changeBit, bool value) => _bits[(int)changeBit] = value;

        private void SetBit(ViewItem.Bits2 changeBit, bool value) => _bits2[(int)changeBit] = value;

        private bool ChangeBit(ViewItem.Bits bit, bool value)
        {
            if (_bits[(int)bit] == value)
                return false;
            _bits[(int)bit] = value;
            return true;
        }

        private bool ChangeBit(ViewItem.Bits2 bit, bool value)
        {
            if (_bits2[(int)bit] == value)
                return false;
            _bits2[(int)bit] = value;
            return true;
        }

        private static UIClass ValidateOwner(UIClass ui) => ui;

        public string Name
        {
            get => (string)GetData(s_nameProperty);
            set
            {
                if (!((string)GetData(s_nameProperty) != value))
                    return;
                string str = NotifyService.CanonicalizeString(value);
                SetData(s_nameProperty, str);
            }
        }

        public Color DebugOutline
        {
            get
            {
                object data = GetData(s_debugOutlineProperty);
                return data == null ? Color.Transparent : (Color)data;
            }
            set
            {
                if (!(DebugOutline != value))
                    return;
                SetData(s_debugOutlineProperty, value);
                FireNotification(NotificationID.DebugOutline);
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(GetType().Name);
            stringBuilder.Append(":");
            string name = Name;
            if (name != null)
            {
                stringBuilder.Append(" (");
                stringBuilder.Append(name);
                stringBuilder.Append(")");
            }
            return stringBuilder.ToString();
        }

        [Conditional("DEBUG")]
        private void DEBUG_DumpAnimation(ActiveSequence aseq)
        {
            foreach (BaseKeyframe keyframe in aseq.Template.Keyframes)
                ;
        }

        public delegate void PaintHandler(ViewItem senderItem);

        private struct LayoutApplyParams
        {
            public bool fullyVisible;
            public bool visibilityChanging;
            public bool parentOffscreen;
            public bool offscreenChanging;
            public bool allowAnimations;
            public bool deepLayoutChanged;
            public bool anyDeepChangesDelivered;
        }

        private enum Bits : uint
        {
            PendingNavigateInto = 1,
            PendingNavigateIntoIsDefault = 2,
            PendingNavigateIntoScheduled = 4,
            ClipMouse = 8,
            MouseInteractive = 16, // 0x00000010
            PaintInvalid = 32, // 0x00000020
            HasScale = 64, // 0x00000040
            ScaleChanged = 128, // 0x00000080
            LayoutInputMaxSize = 256, // 0x00000100
            LayoutInputMinSize = 512, // 0x00000200
            LayoutInputMargins = 1024, // 0x00000400
            LayoutInputPadding = 2048, // 0x00000800
            LayoutInputVisible = 4096, // 0x00001000
            LayoutAlignment = 8192, // 0x00002000
            LayoutChildAlignment = 16384, // 0x00004000
            LayoutInputSharedSize = 32768, // 0x00008000
            LayoutInputSharedSizePolicy = 65536, // 0x00010000
            OutputSelfDirty = 131072, // 0x00020000
            OutputTreeDirty = 262144, // 0x00040000
            LayoutInvalid = 524288, // 0x00080000
            ActiveAnimations = 1048576, // 0x00100000
            AnimationBuilders = 2097152, // 0x00200000
            IdleAnimations = 4194304, // 0x00400000
            HasNavMode = 8388608, // 0x00800000
            HasFocusOrder = 16777216, // 0x01000000
            DeepLayoutNotifySelf = 33554432, // 0x02000000
            DeepLayoutNotifyTree = 67108864, // 0x04000000
            Unused1 = 134217728, // 0x08000000
            Unused2 = 268435456, // 0x10000000
            Unused3 = 536870912, // 0x20000000
            Unused4 = 1073741824, // 0x40000000
            Unused5 = 2147483648, // 0x80000000
        }

        protected enum Bits2 : uint
        {
            HasAlpha = 1,
            HasCenterPointPercent = 2,
            HasLayer = 4,
            HasRotation = 8,
            IsOffscreen = 16, // 0x00000010
            PendingScrollIntoView = 32, // 0x00000020
            PaintChildrenInvalid = 64, // 0x00000040
            HasPositionNoSend = 128, // 0x00000080
            HasScaleNoSend = 256, // 0x00000100
            HasSizeNoSend = 512, // 0x00000200
            HasRotationNoSend = 1024, // 0x00000400
            HasAlphaNoSend = 2048, // 0x00000800
            HasEffect = 4096, // 0x00001000
            HasLayoutOutput = 8192, // 0x00002000
            AnimationHandles = 16384, // 0x00004000
            IsAlphaAnimationPlaying = 32768, // 0x00008000
            InsideContentChange = 65536, // 0x00010000
            HasCamera = 131072, // 0x00020000
            BuiltLayoutChildren = 262144, // 0x00040000
            Measured = 524288, // 0x00080000
            Arranged = 1048576, // 0x00100000
            Committed = 2097152, // 0x00200000
            ContributesToWidth = 4194304, // 0x00400000
            LayoutOffscreen = 8388608, // 0x00800000
            KeepAlive = 16777216, // 0x01000000
        }
    }
}
