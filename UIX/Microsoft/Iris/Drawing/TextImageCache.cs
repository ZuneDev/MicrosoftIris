// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.TextImageCache
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Queues;
using Microsoft.Iris.Render.Extensions;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.Drawing
{
    internal sealed class TextImageCache : ImageCache
    {
        private static TextImageCache s_theOnlyCache;
        private static readonly DeferredHandler s_dhReschedule = new DeferredHandler(Reschedule);
        private UISession _session;
        private TextImageCache.ScavengeCallback _callback;

        public static void Initialize(UISession session)
        {
            s_theOnlyCache = new TextImageCache(session);
            s_theOnlyCache.NumItemsToKeep = 500;
            s_theOnlyCache.ItemRetainTime = TimeSpan.Zero;
        }

        public static void Uninitialize(UISession session)
        {
            if (s_theOnlyCache == null)
                return;
            s_theOnlyCache.Dispose();
        }

        public static TextImageCache Instance => s_theOnlyCache;

        private TextImageCache(UISession session)
          : base(session.RenderSession, nameof(TextImageCache))
          => _session = session;

        protected override void OnDispose()
        {
            TimeoutManager timeoutManager = _session.Dispatcher.TimeoutManager;
            TextImageCache.ScavengeCallback callback = _callback;
            if (callback != null)
                timeoutManager.CancelTimeout(callback);
            _callback = null;
            base.OnDispose();
        }

        protected override void ScheduleScavenge()
        {
            if (!CleanupPending)
                DeferredCall.Post(DispatchPriority.Idle, s_dhReschedule, this);
            base.ScheduleScavenge();
        }

        private static void Reschedule(object arg)
        {
            TextImageCache cache = arg as TextImageCache;
            if (!cache.CleanupPending)
                return;
            TimeoutManager timeoutManager = cache._session.Dispatcher.TimeoutManager;
            TextImageCache.ScavengeCallback callback = cache._callback;
            cache._callback = null;
            if (callback != null)
                timeoutManager.CancelTimeout(callback);
            TextImageCache.ScavengeCallback scavengeCallback = new TextImageCache.ScavengeCallback(cache);
            timeoutManager.SetTimeoutRelative(scavengeCallback, TimeSpan.FromSeconds(5.0));
            cache._callback = scavengeCallback;
        }

        private class ScavengeCallback : QueueItem
        {
            private TextImageCache _cache;

            public ScavengeCallback(TextImageCache cache) => _cache = cache;

            public override void Dispatch()
            {
                if (_cache._callback != this)
                    return;
                _cache._callback = null;
                _cache.CullObjects();
            }
        }
    }
}
