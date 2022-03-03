// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Input.InputInfo
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Diagnostics;

namespace Microsoft.Iris.Input
{
    internal abstract class InputInfo
    {
        private static InputInfo.InfoPool[] s_pools;
        protected ICookedInputSite _target;
        protected InputEventType _eventType;
        private byte _lockCount;
        private bool _routeTruncated;
        private bool _handled;

        protected void Initialize(InputEventType eventType)
        {
            _eventType = eventType;
            _lockCount = 0;
            _routeTruncated = false;
            _handled = false;
        }

        [Conditional("DEBUG")]
        protected void DEBUG_AssertInitialized()
        {
        }

        [Conditional("DEBUG")]
        private void DEBUG_AssertZombied()
        {
        }

        protected static void SetPoolLimitMode(InputInfo.InfoType type, bool keepSingle)
        {
            if (s_pools == null)
                s_pools = new InputInfo.InfoPool[9];
            s_pools[(int)type].SetLimitMode(keepSingle);
        }

        protected abstract InputInfo.InfoType PoolType { get; }

        protected virtual void Zombie()
        {
            _target = null;
            _eventType = InputEventType.Invalid;
        }

        protected static InputInfo GetFromPool(InputInfo.InfoType poolType) => s_pools[(int)poolType].GetPooledInfo();

        public void ReturnToPool()
        {
            if (!Poolable)
                return;
            s_pools[(int)PoolType].RecycleInfo(this);
        }

        private bool Poolable => _lockCount == 0;

        public void Lock() => ++_lockCount;

        public void Unlock()
        {
            --_lockCount;
            if (_lockCount != 0)
                return;
            ReturnToPool();
        }

        public bool Handled => _handled;

        public void MarkHandled() => _handled = true;

        public bool RouteTruncated => _routeTruncated;

        public ICookedInputSite Target => _target;

        public virtual InputEventType EventType => _eventType;

        public void TruncateRoute() => _routeTruncated = true;

        public virtual void UpdateTarget(ICookedInputSite target) => _target = target;

        public override string ToString() => GetType().Name;

        private struct InfoPool
        {
            private int _numEntries;
            private int _maxEntries;
            private object _storage;

            public void SetLimitMode(bool keepSingle) => _maxEntries = keepSingle ? 1 : 10;

            public InputInfo GetPooledInfo()
            {
                InputInfo inputInfo = null;
                if (_numEntries > 0)
                {
                    inputInfo = _maxEntries != 1 ? ((InputInfo[])_storage)[_numEntries - 1] : (InputInfo)_storage;
                    --_numEntries;
                }
                return inputInfo;
            }

            public bool RecycleInfo(InputInfo info)
            {
                bool flag = false;
                if (_numEntries < _maxEntries)
                {
                    info.Zombie();
                    if (_maxEntries == 1)
                    {
                        _storage = info;
                    }
                    else
                    {
                        if (_storage == null)
                            _storage = (new InputInfo[_maxEntries]);
                        ((InputInfo[])_storage)[_numEntries] = info;
                    }
                    ++_numEntries;
                    flag = true;
                }
                return flag;
            }
        }

        public enum InfoType
        {
            KeyCommand,
            KeyFocus,
            KeyState,
            KeyCharacter,
            MouseFocus,
            MouseMove,
            MouseButton,
            MouseWheel,
            DragDrop,
            NumTypes,
        }
    }
}
