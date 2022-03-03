// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.RenderToken
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Protocol;
using System;

namespace Microsoft.Iris.Render
{
    [Serializable]
    public class RenderToken
    {
        private EngineInfo m_engineInfo;
        private ContextID m_localContextId;
        private ContextID m_destinationContextId;
        private RENDERGROUP m_renderGroupId;

        internal RenderToken(
          EngineInfo engineInfo,
          ContextID localContextId,
          ContextID destinationContextId,
          RENDERGROUP renderGroupId)
        {
            Debug2.Validate(engineInfo != null, typeof(ArgumentNullException), nameof(engineInfo));
            Debug2.Validate(localContextId != ContextID.NULL, typeof(ArgumentNullException), nameof(localContextId));
            Debug2.Validate(destinationContextId != ContextID.NULL, typeof(ArgumentNullException), nameof(destinationContextId));
            this.m_engineInfo = engineInfo;
            this.m_localContextId = localContextId;
            this.m_destinationContextId = destinationContextId;
            this.m_renderGroupId = renderGroupId;
        }

        internal EngineInfo EngineInfo => this.m_engineInfo;

        internal ContextID LocalContextId => this.m_localContextId;

        internal ContextID DestinationContextId => this.m_destinationContextId;

        internal RENDERGROUP RenderGroupId => this.m_renderGroupId;
    }
}
