// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layout.ILayoutNode
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;

namespace Microsoft.Iris.Layout
{
    public interface ILayoutNode
    {
        Size Measure(Size constraint);

        void Arrange(LayoutSlot parentSlot);

        void Arrange(LayoutSlot parentSlot, Rectangle bounds);

        void Arrange(LayoutSlot parentSlot, Rectangle bounds, Vector3 scale, Rotation rotation);

        void Commit();

        void MarkHidden();

        ILayoutInput GetLayoutInput(DataCookie inputID);

        void SetLayoutInput(ILayoutInput input);

        void SetLayoutInput(ILayoutInput input, bool invalidateLayout);

        ExtendedLayoutOutput GetExtendedLayoutOutput(DataCookie outputID);

        void SetExtendedLayoutOutput(ExtendedLayoutOutput newDataOutput);

        bool ContainsAreaOfInterest(AreaOfInterestID id);

        bool TryGetAreaOfInterest(AreaOfInterestID id, out AreaOfInterest area);

        void AddAreaOfInterest(AreaOfInterest interest);

        Vector<int> GetSpecificChildrenRequestList();

        void RequestMoreChildren(int childrenCount);

        void RequestSpecificChildren(Vector<int> indiciesList);

        object MeasureData { get; set; }

        int LayoutChildrenCount { get; }

        LayoutNodeEnumerator LayoutChildren { get; }

        ILayoutNode NextSibling { get; }

        ILayoutNode NextVisibleSibling { get; }

        bool LayoutContributesToWidth { get; set; }

        void SetVisibleIndexRange(
          int beginVisible,
          int endVisible,
          int beginVisibleOffscreen,
          int endVisibleOffscreen,
          int? focusedItem);

        Size DesiredSize { get; }

        Size AlignedSize { get; }

        Point AlignmentOffset { get; }

        Point LayoutPosition { get; }

        bool Visible { get; }
    }
}
