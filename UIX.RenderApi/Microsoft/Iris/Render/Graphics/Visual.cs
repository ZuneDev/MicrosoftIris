// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Visual
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Render.Animation;
using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;
using System.Collections.Specialized;

namespace Microsoft.Iris.Render.Graphics
{
    internal abstract class Visual :
      TreeNode,
      IVisual,
      IAnimatableObject,
      IAnimatable,
      ISharedRenderObject,
      IRenderHandleOwner,
      IRawInputSite
    {
        private const uint k_DebugBitShift = 5;
        private static ColorF s_colorTransparent = ColorF.FromArgb(0U);
        private static ColorF s_colorWhite = new ColorF(1f, 1f, 1f, 1f);
        public static readonly string PositionProperty = nameof(Position);
        public static readonly string AlphaProperty = nameof(Alpha);
        public static readonly string OrientationProperty = "Orientation";
        public static readonly string RotationProperty = nameof(Rotation);
        public static readonly string ScaleProperty = nameof(Scale);
        public static readonly string SizeProperty = nameof(Size);
        protected RenderWindow m_window;
        protected RemoteVisual m_remoteVisual;
        private Vector3 m_vecPosition;
        private Vector2 m_vecSize;
        private BitVector32 m_bvDataBits;
        private static BitVector32.Section s_bvsMouse = BitVector32.CreateSection(15);
        private static BitVector32.Section s_bvsDebug = BitVector32.CreateSection(3, s_bvsMouse);
        private static BitVector32.Section s_bvsValid = BitVector32.CreateSection(1, s_bvsDebug);
        private static BitVector32.Section s_bvsVisible = BitVector32.CreateSection(1, s_bvsValid);
        private static BitVector32.Section s_bvsAlpha;
        private static BitVector32.Section s_bvsCenterPointScale;
        private static BitVector32.Section s_bvsColor;
        private static BitVector32.Section s_bvsCoordmap;
        private static BitVector32.Section s_bvsGradient;
        private static BitVector32.Section s_bvsLayer;
        private static BitVector32.Section s_bvsRotation;
        private static BitVector32.Section s_bvsScale;
        private static BitVector32.Section s_bvsFlush = BitVector32.CreateSection(1, s_bvsVisible);
        private static BitVector32.Section s_bvsRelativeSize;
        private static BitVector32.Section s_bvsCamera;
        private static BitVector32.Section s_bvsNineGrid;
        private static BitVector32.Section[] s_sectionPropIDMap;
        private object m_objOwnerData;

        static Visual()
        {
            s_bvsAlpha = BitVector32.CreateSection(1, s_bvsFlush);
            s_bvsCenterPointScale = BitVector32.CreateSection(1, s_bvsAlpha);
            s_bvsColor = BitVector32.CreateSection(1, s_bvsCenterPointScale);
            s_bvsCoordmap = BitVector32.CreateSection(1, s_bvsColor);
            s_bvsGradient = BitVector32.CreateSection(1, s_bvsCoordmap);
            s_bvsLayer = BitVector32.CreateSection(1, s_bvsGradient);
            s_bvsRotation = BitVector32.CreateSection(1, s_bvsLayer);
            s_bvsScale = BitVector32.CreateSection(1, s_bvsRotation);
            s_bvsRelativeSize = BitVector32.CreateSection(1, s_bvsScale);
            s_bvsCamera = BitVector32.CreateSection(1, s_bvsRelativeSize);
            s_bvsNineGrid = BitVector32.CreateSection(1, s_bvsCamera);
            s_sectionPropIDMap = new BitVector32.Section[10];
            s_sectionPropIDMap[0] = s_bvsAlpha;
            s_sectionPropIDMap[1] = s_bvsScale;
            s_sectionPropIDMap[2] = s_bvsRotation;
            s_sectionPropIDMap[3] = s_bvsCenterPointScale;
            s_sectionPropIDMap[4] = s_bvsLayer;
            s_sectionPropIDMap[5] = s_bvsColor;
            s_sectionPropIDMap[6] = s_bvsCoordmap;
            s_sectionPropIDMap[7] = s_bvsGradient;
            s_sectionPropIDMap[8] = s_bvsCamera;
            s_sectionPropIDMap[9] = s_bvsNineGrid;
        }

        internal Visual(RenderSession session, RenderWindow window, object objOwnerData)
          : base(window)
        {
            Debug2.Validate(session != null, typeof(ArgumentNullException), "Must have valid session");
            Debug2.Validate(window != null, typeof(ArgumentNullException), "Must have valid render window");
            session.AssertOwningThread();
            this.m_window = window;
            this.m_objOwnerData = objOwnerData;
            this.m_vecPosition = Vector3.Zero;
            this.m_vecSize = Vector2.Zero;
            this.FlushValues = true;
            this.SetValid(true);
            this.SetVisible(true);
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose)
                {
                    this.SetValid(false);
                    this.RemoveAllGradients();
                    this.PropertyManager.RemoveAlphaProp(this);
                    this.PropertyManager.RemoveCenterPointScaleProp(this);
                    this.PropertyManager.RemoveLayerProp(this);
                    this.PropertyManager.RemoveRotationProp(this);
                    this.PropertyManager.RemoveScaleProp(this);
                    if (this.m_remoteVisual != null)
                        this.m_remoteVisual.Dispose();
                }
                this.m_remoteVisual = null;
                this.m_objOwnerData = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        MouseOptions IVisual.MouseOptions
        {
            get => this.MouseOptions;
            set => this.MouseOptions = value;
        }

        IVisualContainer IVisual.Parent => this.Parent as IVisualContainer;

        string IVisual.DebugID
        {
            get => this.DebugDescription;
            set
            {
            }
        }

        ColorF IVisual.DebugColor
        {
            get => this.DEBUG_OutlineColor;
            set => this.DEBUG_OutlineColor = value;
        }

        void IVisual.CopyFrom(IVisual visualSource)
        {
            Visual visual = (Visual)visualSource;
            this.Position = visual.Position;
            this.Size = visual.Size;
            this.Scale = visual.Scale;
            this.Rotation = visual.Rotation;
            this.Alpha = visual.Alpha;
            this.CenterPointScale = visual.CenterPointScale;
            this.Layer = visual.Layer;
            this.MouseOptions = visual.MouseOptions;
        }

        void IVisual.Remove() => this.RemoveFromTree();

        protected void AddGradient(Gradient gradient)
        {
            Vector<Gradient> result;
            if (this.IsDynamicValueSet(PropId.Gradients))
            {
                this.PropertyManager.GetGradientProp(this, out result);
            }
            else
            {
                result = new Vector<Gradient>();
                this.PropertyManager.SetGradientProp(this, result);
            }
            gradient.RegisterUsage(this);
            result.Add(gradient);
            this.m_remoteVisual.SendAddGradient(gradient.RemoteGradient);
        }

        protected void RemoveAllGradients()
        {
            if (!this.IsDynamicValueSet(PropId.Gradients))
                return;
            Vector<Gradient> result;
            this.PropertyManager.GetGradientProp(this, out result);
            this.PropertyManager.RemoveGradientProp(this);
            foreach (SharedRenderObject sharedRenderObject in result)
                sharedRenderObject.UnregisterUsage(this);
            result.Clear();
            this.m_remoteVisual.SendClearGradients();
        }

        internal void ChangeParent(Visual visualParent, Visual visualSibling, VisualOrder nOrder)
        {
            RemoteVisual remoteVisual1 = visualParent?.m_remoteVisual;
            RemoteVisual remoteVisual2 = this.m_remoteVisual;
            RemoteVisual remoteVisual3 = visualSibling?.m_remoteVisual;
            remoteVisual2.SendChangeParent(remoteVisual1, remoteVisual3, nOrder);
            this.ChangeParent(visualParent);
        }

        internal override void RemoveAllChildren()
        {
            for (Visual firstChild = (Visual)this.FirstChild; firstChild != null; firstChild = (Visual)this.FirstChild)
                firstChild.ChangeParent(null, null, VisualOrder.First);
            base.RemoveAllChildren();
        }

        protected void RemoveFromTree()
        {
            this.RemoveAllChildren();
            this.ChangeParent(null, null, VisualOrder.First);
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteVisual.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteVisual = null;

        internal RemoteVisual RemoteStub => this.m_remoteVisual;

        public object OwnerData
        {
            get => this.m_objOwnerData;
            set => this.m_objOwnerData = value;
        }

        protected Vector3 Position
        {
            get => this.m_vecPosition;
            set => this.SetPosition(value, false);
        }

        protected void SetPosition(Vector3 value, bool force)
        {
            if (!(this.m_vecPosition != value) && !force)
                return;
            this.m_vecPosition = value;
            if (!this.FlushValues)
                return;
            this.m_remoteVisual.SendSetPosition(this.m_vecPosition);
        }

        internal Vector2 Size
        {
            get => this.m_vecSize;
            set => this.SetSize(value, false);
        }

        internal void SetSize(Vector2 value, bool force)
        {
            Debug2.Validate(value.X >= 0.0 && value.Y >= 0.0, typeof(ArgumentOutOfRangeException), "Negative size not allowed");
            if (!(this.m_vecSize != value) && !force)
                return;
            this.m_vecSize = value;
            if (!this.FlushValues)
                return;
            this.m_remoteVisual.SendSetSize(this.m_vecSize);
        }

        protected float Alpha
        {
            get
            {
                float result = 1f;
                if (this.IsDynamicValueSet(PropId.Alpha))
                {
                    this.PropertyManager.GetAlphaProp(this, out result);
                    Debug2.Validate(!Math2.WithinEpsilon(result, 1f), typeof(InvalidOperationException), "expected non-opaque alpha if HasAlpha flag was set");
                }
                return result;
            }
            set => this.SetAlpha(value, false);
        }

        protected void SetAlpha(float value, bool force)
        {
            if (Alpha == (double)value && !force)
                return;
            Debug2.Validate(value >= 0.0 && value <= 1.0, typeof(ArgumentOutOfRangeException), "Alpha value must be within 0-1 range (incl)");
            this.PropertyManager.SetAlphaProp(this, value);
            if (!this.FlushValues)
                return;
            this.m_remoteVisual.SendSetAlpha(value);
        }

        protected Vector3 Scale
        {
            get
            {
                Vector3 result = Vector3.UnitVector;
                if (this.IsDynamicValueSet(PropId.Scale))
                {
                    this.PropertyManager.GetScaleProp(this, out result);
                    Debug2.Validate(result != Vector3.UnitVector, typeof(InvalidOperationException), "expected non-unit scale if HasScale flag was set");
                }
                return result;
            }
            set => this.SetScale(value, false);
        }

        protected void SetScale(Vector3 value, bool force)
        {
            if (!(this.Scale != value) && !force)
                return;
            this.PropertyManager.SetScaleProp(this, value);
            if (!this.FlushValues)
                return;
            this.m_remoteVisual.SendSetScale(value);
        }

        protected AxisAngle Rotation
        {
            get
            {
                AxisAngle result = AxisAngle.Identity;
                if (this.IsDynamicValueSet(PropId.Rotation))
                {
                    this.PropertyManager.GetRotationProp(this, out result);
                    Debug2.Validate(result != AxisAngle.Identity, typeof(InvalidOperationException), "expected non-null rotation data if HasRotation flag was set");
                }
                return result;
            }
            set => this.SetRotation(value, false);
        }

        protected void SetRotation(AxisAngle value, bool force)
        {
            if (!(this.Rotation != value) && !force)
                return;
            this.PropertyManager.SetRotationProp(this, value);
            AxisAngle aaRotation = value;
            if (this.m_window.IsRightToLeft && aaRotation.Axis.Y <= 0.0 && aaRotation.Axis.X <= 0.0)
                aaRotation.Angle = -aaRotation.Angle;
            if (!this.FlushValues)
                return;
            this.m_remoteVisual.SendSetRotation(aaRotation);
        }

        protected Vector3 CenterPointScale
        {
            get
            {
                Vector3 result = Vector3.Zero;
                if (this.IsDynamicValueSet(PropId.CenterPointScale))
                {
                    this.PropertyManager.GetCenterPointScaleProp(this, out result);
                    Debug2.Validate(result != Vector3.Zero, typeof(InvalidOperationException), "expected non-unit scale if HasCenterPointScale flag was set");
                }
                return result;
            }
            set
            {
                if (this.m_window.IsRightToLeft)
                    value.X = 1f - value.X;
                if (!(this.CenterPointScale != value))
                    return;
                this.PropertyManager.SetCenterPointScaleProp(this, value);
                if (!this.FlushValues)
                    return;
                this.m_remoteVisual.SendSetCenterPointScale(value);
            }
        }

        protected uint Layer
        {
            get
            {
                uint result = 0;
                if (this.IsDynamicValueSet(PropId.Layer))
                {
                    this.PropertyManager.GetLayerProp(this, out result);
                    Debug2.Validate(result != 0U, typeof(InvalidOperationException), "expected non-zero layer if HasLayer flag was set");
                }
                return result;
            }
            set
            {
                if ((int)this.Layer == (int)value)
                    return;
                this.PropertyManager.SetLayerProp(this, value);
                if (!this.FlushValues)
                    return;
                this.m_remoteVisual.SendSetLayer(value);
            }
        }

        protected bool Visible
        {
            get => this.GetVisible();
            set
            {
                if (this.Visible == value)
                    return;
                this.SetVisible(value);
                if (!this.FlushValues)
                    return;
                this.m_remoteVisual.SendSetVisible(value);
            }
        }

        protected bool RelativeSize
        {
            get => this.m_bvDataBits[s_bvsRelativeSize] != 0;
            set
            {
                int num = value ? 1 : 0;
                if (this.m_bvDataBits[s_bvsRelativeSize] == num)
                    return;
                this.m_bvDataBits[s_bvsRelativeSize] = num;
                this.m_remoteVisual.SendSetRelativeSize(value);
            }
        }

        internal bool IsValid => this.GetValid();

        public MouseOptions MouseOptions
        {
            get => (MouseOptions)this.m_bvDataBits[s_bvsMouse];
            set
            {
                Debug2.Validate((value & ~MouseOptions.ValidMask) == MouseOptions.None, typeof(ArgumentException), "Expected valid mouse bits");
                uint nMask = (uint)this.m_bvDataBits[s_bvsMouse] ^ (uint)(short)value;
                if (nMask != 0U && this.FlushValues)
                    this.m_remoteVisual.SendChangeDataBits((uint)value, nMask);
                this.m_bvDataBits[s_bvsMouse] = (short)value;
            }
        }

        internal Visual.DebugBits DebugOptions
        {
            get => (Visual.DebugBits)this.m_bvDataBits[s_bvsDebug];
            set
            {
                Debug2.Validate((value & ~DebugBits.Dump) == DebugBits.None, typeof(ArgumentException), "Expected valid debug bits");
                Debug2.Validate((uint)value < byte.MaxValue, typeof(ArgumentException), "Expected valid debug bits");
                uint nMask = (uint)((Visual.DebugBits)this.m_bvDataBits[s_bvsDebug] ^ value);
                if (nMask != 0U)
                {
                    uint nValue = (uint)value << 5;
                    if (this.m_remoteVisual.IsValid)
                        this.m_remoteVisual.SendChangeDataBits(nValue, nMask);
                }
                this.m_bvDataBits[s_bvsDebug] = (int)value;
            }
        }

        protected VisualPropertyManager PropertyManager => this.m_window.Session.VisualPropertyManager;

        internal ColorF DEBUG_OutlineColor
        {
            get => s_colorTransparent;
            set
            {
            }
        }

        private bool GetValid() => this.m_bvDataBits[s_bvsValid] != 0;

        private void SetValid(bool fValid) => this.m_bvDataBits[s_bvsValid] = fValid ? 1 : 0;

        private bool GetVisible() => this.m_bvDataBits[s_bvsVisible] != 0;

        private void SetVisible(bool fVisible) => this.m_bvDataBits[s_bvsVisible] = fVisible ? 1 : 0;

        internal bool IsDynamicValueSet(Visual.PropId propid)
        {
            Debug2.Validate(propid < PropId.MaxDynamicProp, typeof(InvalidOperationException), "Cannot call dynvalue on this propid");
            return this.m_bvDataBits[s_sectionPropIDMap[(int)propid]] != 0;
        }

        internal void SetDynamicValueSet(Visual.PropId propid, bool fValue)
        {
            Debug2.Validate(propid < PropId.MaxDynamicProp, typeof(InvalidOperationException), "Cannot call dynvalue on this propid");
            this.m_bvDataBits[s_sectionPropIDMap[(int)propid]] = fValue ? 1 : 0;
        }

        internal bool IsPropFlagSet(Visual.PropId propid)
        {
            Debug2.Validate(propid >= PropId.MaxDynamicProp && propid < PropId.MaxFlagProp, typeof(InvalidOperationException), "Cannot call IsPropFlagSet on this propid");
            return this.m_bvDataBits[s_sectionPropIDMap[(int)propid]] != 0;
        }

        internal void SetPropFlag(Visual.PropId propid, bool fValue)
        {
            Debug2.Validate(propid >= PropId.MaxDynamicProp && propid < PropId.MaxFlagProp, typeof(InvalidOperationException), "Cannot call SetPropFlag on this propid");
            this.m_bvDataBits[s_sectionPropIDMap[(int)propid]] = fValue ? 1 : 0;
        }

        internal override void Reset()
        {
            this.RemoveFromTree();
            this.RemoveAllGradients();
            bool flushValues = this.FlushValues;
            this.FlushValues = false;
            this.Position = Vector3.Zero;
            this.Scale = Vector3.UnitVector;
            this.Size = Vector2.Zero;
            this.Layer = 0U;
            this.Alpha = 1f;
            this.Rotation = AxisAngle.Identity;
            this.Visible = true;
            this.CenterPointScale = Vector3.Zero;
            this.OwnerData = null;
            this.MouseOptions = MouseOptions.None;
            this.FlushValues = flushValues;
            this.m_remoteVisual.SendReset();
        }

        private bool FlushValues
        {
            get => this.m_bvDataBits[s_bvsFlush] != 0;
            set => this.m_bvDataBits[s_bvsFlush] = value ? 1 : 0;
        }

        RENDERHANDLE IAnimatableObject.GetObjectId() => ((IRenderHandleOwner)this).RenderHandle;

        uint IAnimatableObject.GetPropertyId(string propertyName)
        {
            if (propertyName == PositionProperty)
                return 1;
            if (propertyName == AlphaProperty)
                return 2;
            if (propertyName == RotationProperty || propertyName == OrientationProperty)
                return 3;
            if (propertyName == ScaleProperty)
                return 4;
            if (propertyName == SizeProperty)
                return 5;
            Debug2.Validate(false, typeof(ArgumentException), "Unsupported property");
            return 0;
        }

        AnimationInputType IAnimatableObject.GetPropertyType(
          string propertyName)
        {
            if (propertyName == PositionProperty)
                return AnimationInputType.Vector3;
            if (propertyName == AlphaProperty)
                return AnimationInputType.Float;
            if (propertyName == RotationProperty)
                return AnimationInputType.Vector4;
            if (propertyName == OrientationProperty)
                return AnimationInputType.Quaternion;
            if (propertyName == ScaleProperty)
                return AnimationInputType.Vector3;
            if (propertyName == SizeProperty)
                return AnimationInputType.Vector2;
            Debug2.Validate(false, typeof(ArgumentException), "Unsupported property");
            return AnimationInputType.Float;
        }

        public override int GetHashCode() => this.m_remoteVisual.GetHashCode();

        public override bool Equals(object oRHS) => base.Equals(oRHS);

        [System.Flags]
        internal enum DebugBits
        {
            MarkForOutline = 1,
            IgnoreOutlineAll = 2,
            Dump = IgnoreOutlineAll | MarkForOutline, // 0x00000003
            None = 0,
            ValidMask = Dump, // 0x00000003
        }

        internal enum PropId
        {
            Alpha = 0,
            Scale = 1,
            Rotation = 2,
            CenterPointScale = 3,
            Layer = 4,
            OutlineColor = 5,
            CoordMap = 6,
            Gradients = 7,
            Camera = 8,
            MaxDynamicProp = 9,
            NineGrid = 9,
            MaxFlagProp = 10, // 0x0000000A
        }

        private enum AnimatableProperties : uint
        {
            Position = 1,
            Alpha = 2,
            Rotation = 3,
            Scale = 4,
            Size = 5,
        }
    }
}
