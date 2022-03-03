// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Protocol.ObjectCache
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render.Protocol
{
    internal class ObjectCache
    {
        private ObjectCache.Callback m_callback;
        private object[] m_arFreeObjs;
        private int m_idxFree;

        public ObjectCache(ObjectCache.Callback callback, int cFreeItems, bool fPrePopulate)
        {
            Debug2.Validate(callback != null, typeof(ArgumentNullException), "calback");
            Debug2.Validate(cFreeItems > 0, typeof(ArgumentException), "Free list must have at least one entry", nameof(cFreeItems));
            this.m_callback = callback;
            this.m_arFreeObjs = new object[cFreeItems];
            this.m_idxFree = -1;
            if (!fPrePopulate)
                return;
            for (int index = 0; index < cFreeItems; ++index)
                this.Push(this.DoCallback(Operation.Alloc, null));
        }

        public void Dispose()
        {
            object[] arFreeObjs = this.m_arFreeObjs;
            if (arFreeObjs == null)
                return;
            int idxFree = this.m_idxFree;
            this.m_idxFree = -1;
            this.m_arFreeObjs = null;
            for (; idxFree >= 0; --idxFree)
                this.DoCallback(Operation.Free, arFreeObjs[idxFree]);
        }

        public object Pop()
        {
            ObjectCache.Operation operation = Operation.Alloc;
            object data = null;
            if (this.m_idxFree >= 0)
            {
                operation = Operation.Thaw;
                data = this.m_arFreeObjs[this.m_idxFree--];
            }
            return this.DoCallback(operation, data);
        }

        public void Push(object data)
        {
            if (this.m_idxFree + 1 >= this.m_arFreeObjs.Length)
            {
                this.DoCallback(Operation.Free, data);
            }
            else
            {
                data = this.DoCallback(Operation.Freeze, data);
                this.m_arFreeObjs[++this.m_idxFree] = data;
            }
        }

        private object DoCallback(ObjectCache.Operation operation, object data) => this.m_callback(operation, data);

        public enum Operation
        {
            Alloc,
            Free,
            Thaw,
            Freeze,
        }

        public delegate object Callback(ObjectCache.Operation operation, object data);
    }
}
