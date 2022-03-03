// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Surface
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;

namespace Microsoft.Iris.Render.Graphics
{
    internal sealed class Surface : SharedResource, IRenderHandleOwner
    {
        private ISurfaceContentOwner m_surfaceOwner;
        private RemoteSurface m_remoteSurface;
        private SurfacePool m_poolOwner;
        private Size m_sizeContentPxl;
        private Size m_sizeGutterPxl;
        private int m_nInstanceID;
        private bool m_fPrimaryOwner;

        internal Surface(SurfacePool poolOwner, ISurfaceContentOwner surfaceOwner, Size sizeContentPxl)
          : this(poolOwner, surfaceOwner, sizeContentPxl, Size.Zero, RENDERHANDLE.NULL)
        {
        }

        internal Surface(
          SurfacePool poolOwner,
          ISurfaceContentOwner surfaceOwner,
          Size sizeContentPxl,
          Size sizeGutter)
          : this(poolOwner, surfaceOwner, sizeContentPxl, sizeGutter, RENDERHANDLE.NULL)
        {
        }

        internal Surface(
          SurfacePool poolOwner,
          ISurfaceContentOwner surfaceOwner,
          Size sizeContentPxl,
          Size sizeGutterPxl,
          RENDERHANDLE rhExternal)
          : base(poolOwner.Device.Session)
        {
            this.m_surfaceOwner = surfaceOwner;
            this.m_poolOwner = poolOwner;
            this.m_remoteSurface = null;
            this.m_sizeContentPxl = new Size();
            this.m_sizeGutterPxl = sizeGutterPxl;
            RenderPort renderingPort = this.m_poolOwner.Device.Session.RenderingPort;
            RENDERHANDLE handle = rhExternal;
            if (handle == RENDERHANDLE.NULL)
            {
                handle = renderingPort.AllocHandle(this);
                this.m_fPrimaryOwner = true;
            }
            try
            {
                if (this.m_fPrimaryOwner)
                {
                    this.m_nInstanceID = this.m_poolOwner.CreateSurface(this, handle);
                    this.m_remoteSurface = RemoteSurface.CreateFromHandle(renderingPort, handle);
                }
                else
                {
                    this.m_nInstanceID = this.m_poolOwner.AttachSurface(this);
                    this.m_remoteSurface = RemoteSurface.CreateFromExternalHandle(renderingPort, handle, this);
                }
                this.m_sizeContentPxl = sizeContentPxl;
                Size sizeStoragePxl = new Size(sizeContentPxl.Width + this.m_sizeGutterPxl.Width * 2, sizeContentPxl.Height + this.m_sizeGutterPxl.Height * 2);
                if (!this.m_fPrimaryOwner)
                    return;
                this.m_remoteSurface.SendSetStorageSize(sizeStoragePxl);
            }
            finally
            {
                if (this.m_remoteSurface == null)
                {
                    GC.SuppressFinalize(this);
                    if (this.m_fPrimaryOwner)
                        renderingPort.FreeHandle(handle);
                }
            }
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose)
                {
                    if (this.m_remoteSurface != null)
                        this.m_remoteSurface.Dispose();
                    try
                    {
                        this.m_surfaceOwner.OnSurfaceDisposed(this);
                    }
                    finally
                    {
                        this.m_poolOwner.NotifyDisposeSurface(this.m_nInstanceID);
                        this.m_nInstanceID = 0;
                    }
                }
                this.m_remoteSurface = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteSurface.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteSurface = null;

        internal RemoteSurface RemoteStub => this.m_remoteSurface;

        internal SurfacePool SurfacePool => this.m_poolOwner;

        internal int SurfacePoolInstanceID => this.m_nInstanceID;

        internal Size ContentSize => this.m_sizeContentPxl;

        internal ISurfaceContentOwner ContentOwner => this.m_surfaceOwner;

        internal Size GutterSize => this.m_sizeGutterPxl;

        internal void RestoreContent()
        {
            if (!this.m_remoteSurface.IsValid)
                return;
            this.m_surfaceOwner.OnRestoreContent(this);
        }

        internal void NotifyNewPool(SurfacePool poolNew, int nNewInstanceID)
        {
            this.m_poolOwner = poolNew;
            this.m_nInstanceID = nNewInstanceID;
            Rectangle rcContentPxl = new Rectangle(0, 0, this.m_sizeContentPxl.Width, this.m_sizeContentPxl.Height);
            this.m_remoteSurface.SendRemapContainer(this.m_poolOwner.RemoteStub);
            this.m_remoteSurface.SendRemapLocation(rcContentPxl);
            this.m_remoteSurface.SendSetRotation(false);
            this.m_remoteSurface.SendSetGutter(this.m_sizeGutterPxl);
        }

        protected override void OnUsageChange(bool fUsed)
        {
        }
    }
}
