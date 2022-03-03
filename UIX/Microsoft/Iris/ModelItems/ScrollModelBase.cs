// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ModelItems.ScrollModelBase
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;

namespace Microsoft.Iris.ModelItems
{
    internal abstract class ScrollModelBase : NotifyObjectBase
    {
        public abstract int ScrollStep { get; set; }

        public abstract void Scroll(int amount);

        public abstract void ScrollUp();

        public abstract void ScrollDown();

        public abstract void PageUp();

        public abstract void PageDown();

        public abstract void Home();

        public abstract void End();

        public abstract void ScrollToPosition(float position);

        public abstract bool CanScrollUp { get; }

        public abstract bool CanScrollDown { get; }

        public abstract float CurrentPage { get; }

        public abstract float TotalPages { get; }

        public abstract float ViewNear { get; }

        public abstract float ViewFar { get; }
    }
}
