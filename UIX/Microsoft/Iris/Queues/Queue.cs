// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Queues.Queue
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Queues
{
    internal abstract class Queue
    {
        public abstract QueueItem GetNextItem();

        public event EventHandler Wake;

        protected virtual void OnWake()
        {
            if (Wake == null)
                return;
            Wake(this, EventArgs.Empty);
        }

        public virtual void Dispose()
        {
        }
    }
}
