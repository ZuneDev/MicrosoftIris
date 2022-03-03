// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Data.IVirtualList
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.ViewItems;
using System.Collections;

namespace Microsoft.Iris.Data
{
    internal interface IVirtualList : INotifyList, IList, ICollection, IEnumerable
    {
        void RequestItem(int idx, ItemRequestCallback callback);

        bool IsItemAvailable(int idx);

        void NotifyVisualsCreated(int idx);

        void NotifyVisualsReleased(int idx);

        Repeater RepeaterHost { get; set; }

        bool SlowDataRequestsEnabled { get; }

        void NotifyRequestSlowData(int index);

        SlowDataAcquireCompleteHandler SlowDataAcquireCompleteHandler { get; set; }
    }
}
