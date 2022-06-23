// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.OS.HttpResources
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.OS
{
    internal class HttpResources : IResourceProvider
    {
        private static HttpResources s_instance = new HttpResources();
        private static EventHandler s_activationChangeHandler;

        public static void Startup()
        {
            ResourceManager.Instance.RegisterSource("http", s_instance);
            ResourceManager.Instance.RegisterSource("https", s_instance);
            NativeApi.SpHttpStartup();
        }

        public static void Shutdown()
        {
            if (s_activationChangeHandler != null)
                UISession.Default.Form.ActivationChange -= s_activationChangeHandler;
            NativeApi.SpHttpShutdown();
        }

        private static void OnActivationChanged(object sender, EventArgs args) => NativeApi.SpHttpFlushProxyCache();

        public Resource GetResource(string hierarchicalPart, string url, bool forceSynchronous)
        {
            if (s_activationChangeHandler == null)
            {
                s_activationChangeHandler = new EventHandler(OnActivationChanged);
                UISession.Default.Form.ActivationChange += s_activationChangeHandler;
            }
            return new HttpResource(url, forceSynchronous);
        }
    }
}
