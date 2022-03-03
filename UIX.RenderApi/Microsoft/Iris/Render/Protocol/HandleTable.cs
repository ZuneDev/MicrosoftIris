// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.HandleTable
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Protocol
{
    internal class HandleTable : RenderObject, IComparer
    {
        private RenderPort m_portOwner;
        private HandleGroup m_groups;
        private uint m_MaxObjectsPerGroup;
        private int m_GroupShift;
        private int m_UniqueShift;
        private uint m_GroupMask;
        private uint m_ObjectMask;
        private uint m_UniqueMask;

        public HandleTable(RenderPort portOwner, MessageCookieLayout layout)
        {
            Debug2.Validate(portOwner != null, typeof(ArgumentNullException), nameof(portOwner));
            this.m_portOwner = portOwner;
            uint maxObjects = 1U << layout.numberOfGroupBits;
            this.m_MaxObjectsPerGroup = 1U << layout.numberOfObjectBits;
            this.m_GroupShift = layout.numberOfObjectBits;
            this.m_UniqueShift = layout.numberOfObjectBits + layout.numberOfGroupBits;
            this.m_GroupMask = (uint)((int)maxObjects - 1 << this.m_GroupShift);
            this.m_ObjectMask = this.m_MaxObjectsPerGroup - 1U;
            this.m_UniqueMask = (uint)~((int)this.m_GroupMask | (int)this.m_ObjectMask);
            this.m_groups = new HandleGroup(GCHandleType.Normal, this.m_portOwner, maxObjects);
        }

        protected override void Dispose(bool inDispose)
        {
            try
            {
                if (!inDispose)
                    return;
                foreach (HandleGroup group in this.m_groups)
                    group?.Dispose();
                this.m_groups.Clear();
                this.m_groups = null;
            }
            finally
            {
                base.Dispose(inDispose);
            }
        }

        public RENDERHANDLE Alloc(RENDERGROUP groupId, IRenderHandleOwner owner)
        {
            uint uint32 = RENDERGROUP.ToUInt32(groupId);
            HandleGroup group = this.m_groups[uint32] as HandleGroup;
            Debug2.Validate(group != null, typeof(ArgumentException), "Group does not exist", nameof(groupId));
            uint unique;
            uint num = group.Add(owner, out unique);
            return RENDERHANDLE.FromUInt32((uint)((int)unique << this.m_UniqueShift | (int)uint32 << this.m_GroupShift) | num);
        }

        public void AllocWithId(IRenderHandleOwner owner, RENDERHANDLE handle, bool fUpdateExisting)
        {
            uint unique;
            HandleGroup group;
            uint index;
            this.MapHandle(handle, true, out unique, out group, out index);
            group.AddAtIndexWithKnownUniqueness(owner, index, unique, fUpdateExisting);
        }

        public void Free(RENDERHANDLE handle)
        {
            uint unique;
            HandleGroup group;
            uint index;
            this.MapHandle(handle, false, out unique, out group, out index);
            group.Remove(index, unique);
        }

        public IRenderHandleOwner GetOwner(RENDERHANDLE handle) => this.GetOwner(handle, false);

        public IRenderHandleOwner GetOwner(RENDERHANDLE handle, bool fAllowInvalid)
        {
            uint unique;
            HandleGroup group;
            uint index;
            this.MapHandle(handle, false, out unique, out group, out index, fAllowInvalid);
            return group.Get(index, unique, fAllowInvalid);
        }

        public void SetOwner(RENDERHANDLE handle, IRenderHandleOwner owner)
        {
            uint unique;
            HandleGroup group;
            uint index;
            this.MapHandle(handle, false, out unique, out group, out index);
            group.Set(index, unique, owner);
        }

        public RENDERGROUP GetGroup(RENDERHANDLE handle) => RENDERGROUP.FromUInt32((RENDERHANDLE.ToUInt32(handle) & this.m_GroupMask) >> this.m_GroupShift);

        public RENDERGROUP AllocGroup() => RENDERGROUP.FromUInt32(this.m_groups.Add(this.CreateNewGroup()));

        public void AllocGroupWithId(RENDERGROUP groupId)
        {
            uint uint32 = RENDERGROUP.ToUInt32(groupId);
            this.m_groups.AddAtIndex(this.CreateNewGroup(), uint32);
        }

        public void FreeGroup(RENDERGROUP groupId) => this.m_groups.Remove(RENDERGROUP.ToUInt32(groupId));

        public void Validate(RENDERHANDLE handle)
        {
            uint unique;
            HandleGroup group;
            uint index;
            this.MapHandle(handle, false, out unique, out group, out index);
            Debug2.Validate(group.Get(index, unique) != null, typeof(ArgumentException), "Must point to valid object", nameof(handle));
        }

        private HandleGroup CreateNewGroup() => new HandleGroup(GCHandleType.Weak, this.m_portOwner, true, this.m_MaxObjectsPerGroup);

        private void MapHandle(
          RENDERHANDLE handle,
          bool fCreateGroup,
          out uint unique,
          out HandleGroup group,
          out uint index)
        {
            this.MapHandle(handle, fCreateGroup, out unique, out group, out index, false);
        }

        private void MapHandle(
          RENDERHANDLE handle,
          bool fCreateGroup,
          out uint unique,
          out HandleGroup group,
          out uint index,
          bool fAllowInvalid)
        {
            uint uint32 = RENDERHANDLE.ToUInt32(handle);
            uint num1 = (uint32 & this.m_UniqueMask) >> this.m_UniqueShift;
            uint index1 = (uint32 & this.m_GroupMask) >> this.m_GroupShift;
            uint num2 = uint32 & this.m_ObjectMask;
            group = null;
            if (index1 <= this.m_groups.Count)
                group = this.m_groups[index1] as HandleGroup;
            if (group == null)
            {
                if (fCreateGroup)
                {
                    group = this.CreateNewGroup();
                    this.m_groups.AddAtIndex(group, index1);
                }
                else
                    Debug2.Throw(fAllowInvalid, "Group does not exist");
            }
            unique = num1;
            index = num2;
        }

        int IComparer.Compare(object a, object b)
        {
            KeyValueEntry<string, uint> keyValueEntry1 = (KeyValueEntry<string, uint>)a;
            KeyValueEntry<string, uint> keyValueEntry2 = (KeyValueEntry<string, uint>)b;
            uint num1 = keyValueEntry1.Value;
            uint num2 = keyValueEntry2.Value;
            if (num1 > num2)
                return -1;
            return num1 < num2 ? 1 : string.Compare(keyValueEntry1.Key, keyValueEntry2.Key, StringComparison.Ordinal);
        }

        protected override void Invariant() => Debug2.Validate(this.m_groups != null, typeof(InvalidOperationException), "HandleTable has no HandleGroups");
    }
}
