// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.NotifyService
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;
using System.Diagnostics;

namespace Microsoft.Iris.Markup
{
    public struct NotifyService
    {
        private ListenerRootNode _listenerRoot;
        private static Map s_canonicalizedStrings = new Map(2048);

        public void Fire(string id)
        {
            if (_listenerRoot == null || _listenerRoot.Next == null)
                return;
            for (ListenerNodeBase next = _listenerRoot.Next; next != _listenerRoot; next = next.Next)
            {
                Listener listener = (Listener)next;
                if (ReferenceEquals(listener.Watch, id))
                    listener.OnNotify();
            }
        }

        public void FireThreadSafe(string id)
        {
            id = CanonicalizeString(id);
            if (UIDispatcher.IsUIThread)
                Fire(id);
            else
                DeferredCall.Post(DispatchPriority.AppEvent, new DeferredHandler(FireThreadSafeMarshalHandler), id);
        }

        public void FireThreadSafeMarshalHandler(object arg) => Fire((string)arg);

        public bool HasListeners => _listenerRoot != null && _listenerRoot.Next != null;

        public void AddListener(Listener listener)
        {
            if (_listenerRoot == null)
                _listenerRoot = new ListenerRootNode();
            _listenerRoot.AddPrevious(listener);
        }

        public void ClearListeners()
        {
            if (_listenerRoot == null)
                return;
            while (_listenerRoot.Next != null)
                _listenerRoot.Next.Unlink();
            _listenerRoot.Dispose();
            _listenerRoot = null;
        }

        public static string CanonicalizeString(string value) => GetCanonicalizedString(value, true);

        private static string GetCanonicalizedString(string value, bool addIfNotFound)
        {
            object obj = null;
            if (!s_canonicalizedStrings.TryGetValue(value, out obj) && addIfNotFound)
            {
                s_canonicalizedStrings[value] = value;
                obj = value;
            }
            return (string)obj;
        }

        [Conditional("DEBUG")]
        public static void AssertIsCanonicalized(string value) => GetCanonicalizedString(value, false);
    }
}
