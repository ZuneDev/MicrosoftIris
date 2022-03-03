// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.HandleGroup
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocol
{
    internal class HandleGroup : RenderObject, IDisposable, IEnumerable, IRenderHandleOwner
    {
        private const int USAGE_INDEX_SHIFT = 5;
        private const uint USAGE_INDEX_MASK = 31;
        private const uint USAGE_SLOT_COUNT = 32;
        private const uint USAGE_FULL_MASK = 4294967295;
        private const uint LOWEST_BIT = 1;
        private const uint INITIAL_BSEARCH_SPLIT = 16;
        private const uint INITIAL_BSEARCH_TEST = 65535;
        private static GCHandle s_gchEmpty = new GCHandle();
        private RenderPort m_portOwner;
        private GCHandleType m_ht;
        private GCHandle[] m_objects;
        private uint[] m_usage;
        private byte[] m_uniqueness;
        private uint m_alloc;
        private uint m_used;
        private uint m_hint;
        private uint m_maxObjects;

        public HandleGroup(GCHandleType ht, RenderPort portOwner)
          : this(ht, portOwner, false, uint.MaxValue)
        {
        }

        public HandleGroup(GCHandleType ht, RenderPort portOwner, bool uniquify)
          : this(ht, portOwner, uniquify, uint.MaxValue)
        {
        }

        public HandleGroup(GCHandleType ht, RenderPort portOwner, uint maxObjects)
          : this(ht, portOwner, false, maxObjects)
        {
        }

        public HandleGroup(GCHandleType ht, RenderPort portOwner, bool uniquify, uint maxObjects)
        {
            Debug2.Validate(portOwner != null, typeof(ArgumentNullException), nameof(portOwner), "Must specify valid owner");
            this.m_ht = ht;
            this.m_portOwner = portOwner;
            this.m_maxObjects = maxObjects;
            if (!uniquify)
                return;
            this.m_uniqueness = new byte[1];
        }

        protected override void Dispose(bool inDispose)
        {
            try
            {
                this.Clear();
            }
            finally
            {
                base.Dispose(inDispose);
            }
        }

        RENDERHANDLE IRenderHandleOwner.RenderHandle => RENDERHANDLE.NULL;

        void IRenderHandleOwner.OnDisconnect()
        {
        }

        public IRenderHandleOwner this[uint index]
        {
            get
            {
                this.ValidateSlot(index);
                GCHandle gcHandle = this.m_objects[index];
                return !gcHandle.IsAllocated ? null : (IRenderHandleOwner)gcHandle.Target;
            }
            set
            {
                this.ValidateSlot(index);
                this.SetIndex(index, value);
            }
        }

        public uint Count => this.m_alloc;

        public IEnumerator GetEnumerator() => new HandleGroup.ObjectCopy(this).GetEnumerator();

        public IRenderHandleOwner Get(
          uint index,
          uint expectedUniqueness,
          bool fAllowInvalid)
        {
            if (!this.ValidateAccess(index, expectedUniqueness, fAllowInvalid))
                return null;
            GCHandle gcHandle = this.m_objects[index];
            return !gcHandle.IsAllocated ? null : (IRenderHandleOwner)gcHandle.Target;
        }

        public IRenderHandleOwner Get(uint index, uint expectedUniqueness) => this.Get(index, expectedUniqueness, false);

        public void Set(uint index, uint expectedUniqueness, IRenderHandleOwner owner)
        {
            this.ValidateAccess(index, expectedUniqueness);
            this.SetIndex(index, owner);
        }

        public void Clear()
        {
            if (this.m_objects == null)
                return;
            for (uint index = 0; index < m_objects.Length; ++index)
                this.SetIndex(index, null);
            this.m_objects = null;
            this.m_usage = null;
            if (this.m_uniqueness != null)
                this.m_uniqueness = new byte[1];
            this.m_alloc = 0U;
            this.m_used = 0U;
            this.m_hint = 0U;
        }

        public uint Add(IRenderHandleOwner owner) => this.Add(owner, out uint _);

        public uint Add(IRenderHandleOwner owner, out uint unique)
        {
            if (this.m_used >= this.m_alloc)
                this.GrowStorage();
            uint index = this.AllocSlot();
            this.SetIndex(index, owner);
            this.UpdateUniqueness(index, out unique);
            return index;
        }

        public void AddAtIndex(IRenderHandleOwner owner, uint index) => this.AddAtIndex(owner, index, out uint _);

        public void AddAtIndex(IRenderHandleOwner owner, uint index, out uint unique)
        {
            if (index >= this.m_alloc)
                this.GrowStorageTo(index + 1U);
            this.AllocSlotAt(index);
            this.SetIndex(index, owner);
            this.UpdateUniqueness(index, out unique);
        }

        public void AddAtIndexWithKnownUniqueness(
          IRenderHandleOwner owner,
          uint index,
          uint unique,
          bool fUpdateExisting)
        {
            if (index >= this.m_alloc)
                this.GrowStorageTo(index + 1U);
            if (fUpdateExisting && !this.ValidateSlotFree(index, true))
                Debug2.Validate(this.Get(index, unique) == null, typeof(ArgumentException), "Should only be updating a null owner");
            else
                this.AllocSlotAt(index);
            this.SetIndex(index, owner);
            this.SetUniqueness(index, unique);
        }

        public void Remove(uint index)
        {
            this.ValidateSlot(index);
            this.SetIndex(index, null);
            this.FreeSlot(index);
        }

        public void Remove(uint index, uint expectedUniqueness)
        {
            this.ValidateAccess(index, expectedUniqueness);
            this.SetIndex(index, null);
            this.FreeSlot(index);
        }

        private static bool IsEmpty(GCHandle gch) => GCHandle.ToIntPtr(gch) == IntPtr.Zero;

        private void SetIndex(uint index, IRenderHandleOwner newOwner)
        {
            GCHandle gch = this.m_objects[index];
            if (!IsEmpty(gch))
            {
                ((IRenderHandleOwner)gch.Target)?.OnDisconnect();
                gch.Free();
            }
            if (newOwner != null)
                this.m_objects[index] = GCHandle.Alloc(newOwner, this.m_ht);
            else
                this.m_objects[index] = s_gchEmpty;
        }

        private void UpdateUniqueness(uint index, out uint unique)
        {
            byte num1 = 0;
            if (this.m_uniqueness != null)
            {
                byte num2 = this.m_uniqueness[index];
                num1 = num2 >= byte.MaxValue ? (byte)1 : (byte)(num2 + 1U);
                this.m_uniqueness[index] = num1;
            }
            unique = num1;
        }

        private void SetUniqueness(uint index, uint unique)
        {
            Debug2.Validate(unique <= byte.MaxValue, typeof(ArgumentOutOfRangeException), "Uniqueness must be a byte");
            if (this.m_uniqueness == null)
                return;
            this.m_uniqueness[index] = (byte)(unique & byte.MaxValue);
        }

        private void ValidateSlot(uint index) => this.ValidateSlot(index, false);

        private bool ValidateSlot(uint index, bool fAllowInvalid)
        {
            bool flag = false;
            if (index < this.m_alloc && ((int)this.m_usage[index >> 5] & 1 << (int)index) != 0)
                flag = true;
            Debug2.Throw(flag || fAllowInvalid, "Slot is not allocated");
            return flag;
        }

        private void ValidateSlotFree(uint index) => this.ValidateSlotFree(index, false);

        private bool ValidateSlotFree(uint index, bool fAllowInvalid)
        {
            bool flag = false;
            if (index < this.m_alloc && ((int)this.m_usage[index >> 5] & 1 << (int)index) == 0)
                flag = true;
            Debug2.Throw(flag || fAllowInvalid, "Slot is not free");
            return flag;
        }

        private void ValidateAccess(uint index, uint expectedUniqueness) => this.ValidateAccess(index, expectedUniqueness, false);

        private bool ValidateAccess(uint index, uint expectedUniqueness, bool fAllowInvalid)
        {
            if (!this.ValidateSlot(index, fAllowInvalid))
                return false;
            uint num = 0;
            if (this.m_uniqueness != null)
                num = this.m_uniqueness[index];
            bool flag = (int)expectedUniqueness == (int)num;
            Debug2.Throw(flag || fAllowInvalid, "Slot uniqueness is stale");
            return flag;
        }

        private uint AllocSlot()
        {
            uint num1 = 0;
            uint hint;
            for (hint = this.m_hint; hint < m_usage.Length; ++hint)
            {
                num1 = this.m_usage[hint];
                if (num1 != uint.MaxValue)
                    break;
            }
            this.m_hint = hint;
            uint num2 = 16;
            uint maxValue = ushort.MaxValue;
            uint num3 = 0;
            do
            {
                if (((int)num1 & (int)maxValue) == (int)maxValue)
                {
                    num1 >>= (int)num2;
                    num3 += num2;
                }
                num2 >>= 1;
                maxValue >>= (int)num2;
            }
            while (num2 != 0U);
            this.m_usage[hint] |= 1U << (int)num3;
            ++this.m_used;
            return hint << 5 | num3;
        }

        private void AllocSlotAt(uint index)
        {
            this.ValidateSlotFree(index);
            this.m_usage[index >> 5] |= 1U << (int)index;
            ++this.m_used;
        }

        private void FreeSlot(uint index)
        {
            uint num = index >> 5;
            this.m_usage[num] &= (uint)~(1 << (int)index);
            --this.m_used;
            if (this.m_hint <= num)
                return;
            this.m_hint = num;
        }

        private void GrowStorage()
        {
            Debug2.Validate(this.m_alloc < this.m_maxObjects, typeof(InvalidOperationException), "Too many video resources have been allocated");
            uint newSize = this.m_alloc * 2U;
            if (newSize == 0U)
                newSize = 32U;
            if (newSize > this.m_maxObjects)
                newSize = this.m_maxObjects;
            this.GrowStorageTo(newSize);
        }

        private void GrowStorageTo(uint newSize)
        {
            Debug2.Validate(newSize > this.m_alloc, typeof(ArgumentException), nameof(newSize));
            Debug2.Validate(newSize <= this.m_maxObjects, typeof(InvalidOperationException), "Too many video resources have been allocated");
            GCHandle[] gcHandleArray = new GCHandle[newSize];
            uint[] numArray1 = null;
            byte[] numArray2 = null;
            uint num = (uint)((int)newSize + 32 - 1) >> 5;
            if (this.m_usage == null || m_usage.Length < num)
                numArray1 = new uint[num];
            if (this.m_uniqueness != null)
            {
                numArray2 = new byte[newSize];
                Array.Copy(m_uniqueness, numArray2, this.m_uniqueness.Length);
            }
            if (this.m_objects != null)
            {
                Array.Copy(m_objects, gcHandleArray, this.m_objects.Length);
                if (numArray1 != null)
                    Array.Copy(m_usage, numArray1, this.m_usage.Length);
                this.m_hint = (uint)this.m_usage.Length;
            }
            this.m_objects = gcHandleArray;
            if (numArray1 != null)
                this.m_usage = numArray1;
            this.m_uniqueness = numArray2;
            this.m_alloc = newSize;
        }

        private class ObjectCopy
        {
            private IRenderHandleOwner[] m_arObjects;

            public ObjectCopy(HandleGroup map)
            {
                uint used = map.m_used;
                this.m_arObjects = new IRenderHandleOwner[used];
                uint num = 0;
                for (uint index = 0; index < used; ++index)
                {
                    GCHandle gch = map.m_objects[index];
                    if (!IsEmpty(gch))
                    {
                        IRenderHandleOwner target = (IRenderHandleOwner)gch.Target;
                        if (target != null)
                            this.m_arObjects[num++] = target;
                    }
                }
            }

            public IEnumerator GetEnumerator() => this.m_arObjects.GetEnumerator();
        }
    }
}
