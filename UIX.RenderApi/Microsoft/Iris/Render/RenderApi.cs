// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.RenderApi
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocol;
using System;

namespace Microsoft.Iris.Render
{
    public static class RenderApi
    {
        public static IDebug DebugModule
        {
            get => Debug2.Module;
            set => Debug2.Module = value;
        }

        public static IRenderEngine CreateEngine(
          EngineInfo engineInfo,
          IRenderHost renderHost)
        {
            Debug2.Validate(engineInfo != null, typeof(ArgumentNullException), nameof(engineInfo));
            Debug2.Validate(renderHost != null, typeof(ArgumentNullException), nameof(renderHost));
            IRenderEngine renderEngine = null;
            if (engineInfo.Type == EngineType.Iris)
                renderEngine = new RenderEngine(engineInfo as IrisEngineInfo, renderHost);
            return renderEngine;
        }

        public static IRenderSession CreateSession(
          RenderToken sessionToken,
          IRenderHost renderHost)
        {
            Debug2.Validate(sessionToken != null, typeof(ArgumentNullException), nameof(sessionToken));
            Debug2.Validate(sessionToken.EngineInfo != null, typeof(ArgumentNullException), "sessionToken.EngineInfo");
            Debug2.Validate(renderHost != null, typeof(ArgumentNullException), nameof(renderHost));
            IRenderSession renderSession = null;
            if (sessionToken.EngineInfo.Type == EngineType.Iris)
                renderSession = new RenderSession(null);
            return renderSession;
        }

        public static void InitializeForToolOnly()
        {
            EngineApi.InitArgs args = new EngineApi.InitArgs(MessageCookieLayout.Default, RenderEngine.AllocateContextId());
            EngineApi.IFC(EngineApi.SpInit(ref args));
        }

        public static void ShutdownForToolOnly() => EngineApi.IFC(EngineApi.SpUninit());
    }
}
