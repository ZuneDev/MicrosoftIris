// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.RenderCaps
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;

namespace Microsoft.Iris.Render.Internal
{
    internal class RenderCaps : RenderObject, IRenderHandleOwner, IRenderCapsCallback
    {
        private RemoteRenderCaps m_remoteObject;
        private Vector<GraphicsCaps> m_graphicsCapsList;
        private Vector<SoundCaps> m_soundCapsList;
        private bool m_hasValidCaps;
        private bool m_capsPending;
        private uint m_currentRequestId;

        internal RenderCaps(RenderSession ownerSession)
        {
            this.m_hasValidCaps = false;
            this.m_capsPending = false;
            this.m_currentRequestId = 0U;
            this.m_graphicsCapsList = new Vector<GraphicsCaps>();
            this.m_soundCapsList = new Vector<SoundCaps>();
            this.m_remoteObject = ownerSession.RenderingProtocol.BuildRemoteRenderCaps(this, ownerSession.RenderingProtocol.LocalRenderCapsCallbackHandle);
        }

        internal bool HasValidCaps => this.m_hasValidCaps;

        internal Vector<GraphicsCaps> Graphics => this.m_graphicsCapsList;

        internal Vector<SoundCaps> Sound => this.m_soundCapsList;

        internal void RequestCaps()
        {
            if (this.m_capsPending)
                return;
            ++this.m_currentRequestId;
            this.m_remoteObject.SendCheckCaps(this.m_currentRequestId);
            this.m_capsPending = true;
        }

        internal event EventHandler CapsAvailable;

        protected void NotifyCapsAvailable()
        {
            if (this.CapsAvailable == null)
                return;
            this.CapsAvailable(this, EventArgs.Empty);
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => this.m_remoteObject.RenderHandle;

        void IRenderHandleOwner.OnDisconnect() => this.m_remoteObject = null;

        void IRenderCapsCallback.OnBeginCapsCheck(RENDERHANDLE target, uint cookie)
        {
            this.m_graphicsCapsList.Clear();
            this.m_soundCapsList.Clear();
            this.m_hasValidCaps = false;
        }

        void IRenderCapsCallback.OnEndCapsCheck(RENDERHANDLE target, uint cookie)
        {
            this.m_hasValidCaps = true;
            this.m_capsPending = false;
            this.NotifyCapsAvailable();
        }

        void IRenderCapsCallback.OnGraphicsCaps(
          RENDERHANDLE target,
          uint cookie,
          GraphicsCaps caps)
        {
            if ((int)this.m_currentRequestId != (int)cookie)
                return;
            this.m_graphicsCapsList.Add(caps);
        }

        void IRenderCapsCallback.OnSoundCaps(
          RENDERHANDLE target,
          uint cookie,
          SoundCaps caps)
        {
            if ((int)this.m_currentRequestId != (int)cookie)
                return;
            this.m_soundCapsList.Add(caps);
        }
    }
}
