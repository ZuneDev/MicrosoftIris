// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Gradient
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Gradient : SharedRenderObject, IGradient, ISharedRenderObject, IRenderHandleOwner
    {
        private RemoteGradient m_remoteGradient;
        private Orientation m_orientation;
        private ColorF m_colorMask;
        private float m_flOffset;

        internal Gradient(RenderSession session, RemoteDevice remoteDevice)
        {
            Debug2.Validate(session != null, typeof(ArgumentNullException), "Must have valid RenderSession");
            session.AssertOwningThread();
            Debug2.Validate(remoteDevice != null, typeof(ArgumentNullException), "Must have a valid remoteDevice in-param");
            RenderPort renderingPort = session.RenderingPort;
            RENDERHANDLE renderhandle = renderingPort.AllocHandle(this);
            try
            {
                remoteDevice.SendCreateGradient(renderhandle);
                this.m_remoteGradient = RemoteGradient.CreateFromHandle(renderingPort, renderhandle);
            }
            finally
            {
                if (this.m_remoteGradient != null && !this.m_remoteGradient.IsValid)
                    renderingPort.FreeHandle(renderhandle);
            }
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose && this.m_remoteGradient != null)
                    this.m_remoteGradient.Dispose();
                this.m_remoteGradient = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        Orientation IGradient.Orientation
        {
            get => this.m_orientation;
            set
            {
                if (this.m_orientation == value)
                    return;
                this.m_orientation = value;
                this.m_remoteGradient.SendSetOrientation(this.m_orientation);
            }
        }

        ColorF IGradient.ColorMask
        {
            get => this.m_colorMask;
            set
            {
                if (!(this.m_colorMask != value))
                    return;
                this.m_colorMask = value;
                this.m_remoteGradient.SendSetColorMask(this.m_colorMask);
            }
        }

        float IGradient.Offset
        {
            get => this.m_flOffset;
            set
            {
                if (m_flOffset == (double)value)
                    return;
                this.m_flOffset = value;
                this.m_remoteGradient.SendSetOffset(this.m_flOffset);
            }
        }

        void IGradient.AddValue(float flPosition, float flValue, RelativeSpace rsSpace) => this.m_remoteGradient.SendAddValue(flValue, flPosition, rsSpace);

        void IGradient.Clear() => this.m_remoteGradient.SendClear();

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteGradient.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteGradient = null;

        internal RemoteGradient RemoteGradient => this.m_remoteGradient;

        internal struct GradientSample
        {
            public float position;
            public float value;
            public RelativeSpace relSpace;

            internal GradientSample(float pos, float val, RelativeSpace rs)
            {
                this.position = pos;
                this.value = val;
                this.relSpace = rs;
            }
        }
    }
}
