// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.ScrollingLayoutOutput
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;
using Microsoft.Iris.ViewItems;

namespace Microsoft.Iris.Layouts
{
    internal class ScrollingLayoutOutput : ExtendedLayoutOutput
    {
        private static readonly DataCookie s_dataProperty = DataCookie.ReserveSlot();
        private bool _canScrollNegative;
        private bool _canScrollPositive;
        private float _currentPage;
        private float _totalPages;
        private float _viewNear;
        private float _viewFar;
        private bool _processedScrollIntoViewRequest;
        private bool _scrollFocusIntoView = true;
        private VisibleIndexRangeLayoutOutput _visibleIndices;

        public bool CanScrollNegative
        {
            get => _canScrollNegative;
            set => _canScrollNegative = value;
        }

        public bool CanScrollPositive
        {
            get => _canScrollPositive;
            set => _canScrollPositive = value;
        }

        public float CurrentPage
        {
            get => _currentPage;
            set => _currentPage = value;
        }

        public float TotalPages
        {
            get => _totalPages;
            set => _totalPages = value;
        }

        public float ViewNear
        {
            get => _viewNear;
            set => _viewNear = value;
        }

        public float ViewFar
        {
            get => _viewFar;
            set => _viewFar = value;
        }

        public bool ProcessedExplicitScrollIntoViewRequest
        {
            get => _processedScrollIntoViewRequest;
            set => _processedScrollIntoViewRequest = value;
        }

        public bool ScrollFocusIntoView
        {
            get => _scrollFocusIntoView;
            set => _scrollFocusIntoView = value;
        }

        public VisibleIndexRangeLayoutOutput VisibleIndices
        {
            get => _visibleIndices;
            set => _visibleIndices = value;
        }

        public override DataCookie OutputID => DataCookie;

        public static DataCookie DataCookie => s_dataProperty;

        public override string ToString() => InvariantString.Format("{0}(CanScrollNegative={1}, CanScrollPositive={2}, CurrentPage={3}, TotalPages={4})", GetType().Name, _canScrollNegative, _canScrollPositive, _currentPage, _totalPages);
    }
}
