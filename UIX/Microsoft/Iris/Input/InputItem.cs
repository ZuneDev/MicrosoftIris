// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.InputItem
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Debug;
using Microsoft.Iris.Queues;

namespace Microsoft.Iris.Input
{
    internal class InputItem : QueueItem
    {
        private const int c_maxCacheCount = 5;
        private static InputItem s_cache;
        private static int s_cachedCount;
        private InputManager _manager;
        private ICookedInputSite _target;
        private InputInfo _info;

        private InputItem()
        {
        }

        public static InputItem Create(
          InputManager manager,
          ICookedInputSite target,
          InputInfo info)
        {
            InputItem inputItem = AllocateFromPool();
            inputItem._manager = manager;
            inputItem._target = target;
            inputItem._info = info;
            return inputItem;
        }

        private static InputItem AllocateFromPool()
        {
            InputItem inputItem = null;
            if (s_cache != null)
            {
                inputItem = s_cache;
                s_cache = (InputItem)inputItem._next;
                inputItem._next = null;
                --s_cachedCount;
            }
            if (inputItem == null)
                inputItem = new InputItem();
            return inputItem;
        }

        public void ReturnToPool() => ReturnToPool(true);

        private void ReturnToPool(bool returnInfoToo)
        {
            if (returnInfoToo)
                _info.ReturnToPool();
            _manager = null;
            _target = null;
            _info = null;
            _prev = null;
            _next = null;
            _owner = null;
            if (s_cachedCount >= 5)
                return;
            _next = s_cache;
            s_cache = this;
            ++s_cachedCount;
        }

        public InputManager Manager => _manager;

        public InputInfo Info => _info;

        public override string ToString() => base.ToString() + " -> " + _info + " -> " + DebugHelpers.DEBUG_ObjectToString(_target);

        public override void Dispatch()
        {
            if (_info.Target == null)
                _info.UpdateTarget(_target);
            _info.Lock();
            _manager.DeliverInput(_target, _info);
            _info.Unlock();
            ReturnToPool(false);
        }

        internal void UpdateInputSite(ICookedInputSite target) => _target = target;
    }
}
