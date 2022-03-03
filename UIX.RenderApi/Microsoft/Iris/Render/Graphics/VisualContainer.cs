// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.VisualContainer
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;

namespace Microsoft.Iris.Render.Graphics
{
    internal class VisualContainer :
      Visual,
      IVisualContainer,
      IVisual,
      IRawInputSite,
      IAnimatable,
      ISharedRenderObject
    {
        private bool m_fIsRoot;

        internal VisualContainer(
          bool isRoot,
          RenderSession session,
          RenderWindow window,
          object objOwnerData,
          out RemoteVisual remoteVisual)
          : base(session, window, objOwnerData)
        {
            Debug2.Validate(session != null, typeof(ArgumentNullException), "Must have valid RenderSession");
            session.AssertOwningThread();
            Debug2.Validate(this.m_window != null, typeof(InvalidOperationException), "Parent visual should have saved our render window");
            this.m_fIsRoot = isRoot;
            this.m_remoteVisual = session.BuildRemoteVisualContainer(this);
            this.m_remoteVisual.SendSetVisible(true);
            remoteVisual = this.m_remoteVisual;
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (!fInDispose)
                    return;
                this.RemoveAllChildren();
                if (!this.IsDynamicValueSet(PropId.Camera))
                    return;
                this.PropertyManager.RemoveCameraProp(this);
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        private void SetCamera(ICamera camera)
        {
            this.PropertyManager.SetCameraProp(this, (Camera)camera);
            RemoteVisualContainer remoteVisual = (RemoteVisualContainer)this.m_remoteVisual;
            if (camera != null)
                remoteVisual.SendSetCamera(((Camera)camera).RemoteStub);
            else
                remoteVisual.SendSetCamera(null);
        }

        bool IVisualContainer.IsRoot => this.m_fIsRoot;

        Vector3 IVisualContainer.Position
        {
            get => this.Position;
            set => this.Position = value;
        }

        void IVisualContainer.SetPosition(Vector3 value, bool force) => this.SetPosition(value, force);

        Vector2 IVisualContainer.Size
        {
            get => this.Size;
            set => this.Size = value;
        }

        void IVisualContainer.SetSize(Vector2 value, bool force) => this.SetSize(value, force);

        bool IVisualContainer.Visible
        {
            get => this.Visible;
            set => this.Visible = value;
        }

        Vector3 IVisualContainer.Scale
        {
            get => this.Scale;
            set => this.Scale = value;
        }

        void IVisualContainer.SetScale(Vector3 value, bool force) => this.SetScale(value, force);

        AxisAngle IVisualContainer.Rotation
        {
            get => this.Rotation;
            set => this.Rotation = value;
        }

        void IVisualContainer.SetRotation(AxisAngle value, bool force) => this.SetRotation(value, force);

        float IVisualContainer.Alpha
        {
            get => this.Alpha;
            set => this.Alpha = value;
        }

        void IVisualContainer.SetAlpha(float value, bool force) => this.SetAlpha(value, force);

        Vector3 IVisualContainer.CenterPoint
        {
            get => this.CenterPointScale;
            set => this.CenterPointScale = value;
        }

        int IVisualContainer.ChildCount => this.ChildCount;

        uint IVisualContainer.Layer
        {
            get => this.Layer;
            set => this.Layer = value;
        }

        ICamera IVisualContainer.Camera
        {
            get
            {
                Camera result = null;
                if (this.IsDynamicValueSet(PropId.Camera))
                    this.PropertyManager.GetCameraProp(this, out result);
                return result;
            }
            set => this.SetCamera(value);
        }

        void IVisualContainer.AddGradient(IGradient gradient) => this.AddGradient((Gradient)gradient);

        void IVisualContainer.RemoveAllGradients() => this.RemoveAllGradients();

        void IVisualContainer.AddChild(
          IVisual vChild,
          IVisual vSibling,
          VisualOrder nOrder)
        {
            Visual visual = (Visual)vChild;
            Visual visualSibling = null;
            if (vSibling != null)
                visualSibling = (Visual)vSibling;
            visual.ChangeParent(this, visualSibling, nOrder);
        }

        void IVisualContainer.RemoveChild(IVisual vChild) => ((Visual)vChild).ChangeParent(null, null, VisualOrder.First);

        void IVisualContainer.RemoveAllChildren() => this.RemoveAllChildren();
    }
}
