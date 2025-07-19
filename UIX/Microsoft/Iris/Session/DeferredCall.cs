// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Session.DeferredCall
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Queues;
using Microsoft.Iris.Render;
using System;
using System.Threading;

namespace Microsoft.Iris.Session
{
    internal sealed class DeferredCall : QueueItem
    {
        private const int c_maxCacheCount = 100;
        private Delegate _target;
        private object _param;
        private EventArgs _args;
        private static DeferredCall s_cachedList = null;
        private static int s_cachedCount = 0;
        private static object s_cacheLock = new object();

        private DeferredCall()
        {
        }

        private static DeferredCall AllocateFromCache()
        {
            DeferredCall deferredCall = null;
            lock (s_cacheLock)
            {
                if (s_cachedList != null)
                {
                    deferredCall = s_cachedList;
                    s_cachedList = (DeferredCall)deferredCall._next;
                    deferredCall._next = null;
                    --s_cachedCount;
                }
            }
            deferredCall ??= new DeferredCall();
            return deferredCall;
        }

        public static DeferredCall Create(SimpleCallback callback)
        {
            DeferredCall deferredCall = AllocateFromCache();
            deferredCall._target = callback;
            return deferredCall;
        }

        public static DeferredCall Create(DeferredHandler handler, object param)
        {
            DeferredCall deferredCall = AllocateFromCache();
            deferredCall._target = handler;
            deferredCall._param = param;
            return deferredCall;
        }

        public static DeferredCall Create(
          EventHandler handler,
          object sender,
          EventArgs args)
        {
            DeferredCall deferredCall = AllocateFromCache();
            deferredCall._target = handler;
            deferredCall._param = sender;
            deferredCall._args = args;
            return deferredCall;
        }

        public static DeferredCall Create(IDeferredInvokeItem item)
        {
            DeferredCall deferredCall = AllocateFromCache();
            deferredCall._param = item;
            return deferredCall;
        }

        public override void Dispatch()
        {
            switch (_target)
            {
                case SimpleCallback simpleCallback:
                    simpleCallback();
                    break;

                case DeferredHandler deferredHandler:
                    deferredHandler(_param);
                    break;

                case EventHandler eventHandler:
                    eventHandler(_param, _args);
                    break;

                case null when _param is IDeferredInvokeItem deferredInvokeItem:
                    deferredInvokeItem.Dispatch();
                    break;

                default:
                    throw new InvalidOperationException($"Unrecognized call target type '{_target?.GetType().Name}'");
            }

            _target = null;
            _param = null;
            _args = null;
            _prev = null;
            _next = null;
            _owner = null;
            lock (s_cacheLock)
            {
                if (s_cachedCount >= c_maxCacheCount)
                    return;
                _next = s_cachedList;
                s_cachedList = this;
                ++s_cachedCount;
            }
        }

        public static void Post(DispatchPriority priority, SimpleCallback callback)
        {
            QueueItem queueItem = Create(callback);
            UIDispatcher.Post(priority, queueItem);
        }

        public static void Post(Thread thread, DispatchPriority priority, SimpleCallback callback)
        {
            QueueItem queueItem = Create(callback);
            UIDispatcher.Post(thread, priority, queueItem);
        }

        public static void Post(DispatchPriority priority, DeferredHandler handler)
        {
            QueueItem queueItem = Create(handler, null);
            UIDispatcher.Post(priority, queueItem);
        }

        public static void Post(DispatchPriority priority, DeferredHandler handler, object param)
        {
            QueueItem queueItem = Create(handler, param);
            UIDispatcher.Post(priority, queueItem);
        }

        public static void Post(TimeSpan delay, DeferredHandler handler, object param)
        {
            QueueItem queueItem = Create(handler, param);
            UIDispatcher.Post(delay, queueItem);
        }

        public static void Post(
          Thread thread,
          DispatchPriority priority,
          DeferredHandler handler,
          object param)
        {
            QueueItem queueItem = Create(handler, param);
            UIDispatcher.Post(thread, priority, queueItem);
        }

        public override string ToDebugPacketString()
        {
            string packetString = _target?.Target == null
                ? string.Empty
                : $"{_target.Target}.";

            packetString += _target switch
            {
                SimpleCallback simpleCallback => simpleCallback.Method.Name,
                DeferredHandler deferredHandler => $"{deferredHandler.Method.Name}({_param})",
                EventHandler eventHandler => $"{eventHandler.Method.Name}({_param}, {_args})",
                _ when _param is IDeferredInvokeItem deferredInvokeItem => deferredInvokeItem.ToString(),

                _ => throw new InvalidOperationException()
            };

            return packetString;
        }
    }
}
