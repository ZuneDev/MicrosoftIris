// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Layouts.FlowLayout
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Data;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using Microsoft.Iris.ViewItems;
using System;

namespace Microsoft.Iris.Layouts
{
    internal class FlowLayout : ILayout
    {
        private Orientation _orientationFlow;
        private MajorMinor _spacingMajorMinor;
        private MajorMinor _repeatGapMajorMinor;
        private RepeatPolicy _repeat;
        private StripAlignment _stripAlignment;
        private bool _allowWrapFlag;
        private MissingItemPolicy _policyForMissingItems;
        private int _minimumSampleSizeValue;
        private ItemAlignment _defaultChildAlignment;

        public FlowLayout()
        {
            _orientationFlow = Orientation.Horizontal;
            _minimumSampleSizeValue = 3;
        }

        public Orientation Orientation
        {
            get => _orientationFlow;
            set => _orientationFlow = value;
        }

        public MajorMinor Spacing
        {
            get => _spacingMajorMinor;
            set => _spacingMajorMinor = value;
        }

        public bool AllowWrap
        {
            get => _allowWrapFlag;
            set => _allowWrapFlag = value;
        }

        public StripAlignment StripAlignment
        {
            get => _stripAlignment;
            set => _stripAlignment = value;
        }

        public RepeatPolicy Repeat
        {
            get => _repeat;
            set => _repeat = value;
        }

        public MajorMinor RepeatGap
        {
            get => _repeatGapMajorMinor;
            set => _repeatGapMajorMinor = value;
        }

        public MissingItemPolicy MissingItemPolicy
        {
            get => _policyForMissingItems;
            set => _policyForMissingItems = value;
        }

        public int MinimumSampleSize
        {
            get => _minimumSampleSizeValue;
            set => _minimumSampleSizeValue = value;
        }

        public ItemAlignment DefaultChildAlignment
        {
            get => _defaultChildAlignment;
            set => _defaultChildAlignment = value;
        }

        Size ILayout.Measure(ILayoutNode layoutNode, Size constraint)
        {
            if (!(layoutNode.MeasureData is FlowLayout.Packet packet))
            {
                packet = new FlowLayout.Packet();
                layoutNode.MeasureData = packet;
            }
            else
                packet.Clear();
            packet.Subject = layoutNode;
            packet.Constraint = constraint;
            packet.Available = new MajorMinor(constraint, Orientation);
            packet.Cache = (FlowSizeMemoryLayoutInput)packet.Subject.GetLayoutInput(FlowSizeMemoryLayoutInput.Data);
            packet.RepeatOrientation = Orientation;
            if (AllowWrap)
                packet.RepeatOrientation = Orientation == Orientation.Vertical ? Orientation.Horizontal : Orientation.Vertical;
            DetermineCount(packet);
            if (layoutNode.LayoutChildrenCount == 0)
            {
                if (packet.PotentialCount > 0)
                    layoutNode.RequestMoreChildren(1);
                return Size.Zero;
            }
            CreateRecordsFromChildren(packet);
            HandleMissingRecords(packet);
            MeasureRecords(packet);
            if (packet.DesiredSize.IsEmpty && packet.PotentialCount != layoutNode.LayoutChildrenCount)
                layoutNode.RequestMoreChildren(1);
            return packet.DesiredSize.ToSize(Orientation);
        }

        void ILayout.Arrange(ILayoutNode layoutNode, LayoutSlot slot)
        {
            if (layoutNode.LayoutChildrenCount == 0)
                return;
            FlowLayout.Packet measureData = (FlowLayout.Packet)layoutNode.MeasureData;
            measureData.Slot = slot;
            Rectangle peripheralView = slot.PeripheralView;
            Rectangle view = slot.View;
            measureData.PeripheralStart = new MajorMinor(new Size(peripheralView.Location), measureData.RepeatOrientation);
            measureData.PeripheralSize = new MajorMinor(peripheralView.Size, measureData.RepeatOrientation);
            measureData.PeripheralEnd = measureData.PeripheralStart + measureData.PeripheralSize;
            measureData.ViewStart = new MajorMinor(new Size(view.Location), measureData.RepeatOrientation);
            measureData.ViewSize = new MajorMinor(view.Size, measureData.RepeatOrientation);
            measureData.ViewEnd = measureData.ViewStart + measureData.ViewSize;
            DetermineRepeatPolicy(measureData);
            ArrangeChildren(measureData);
            CalculateVisualIntersections(measureData);
            RequestMissingChildren(measureData);
            MajorMinor majorMinor = measureData.DesiredSize;
            if (measureData.Repeat)
            {
                majorMinor = new MajorMinor(measureData.Available.Major, measureData.DesiredSize.Minor);
                Rectangle rectangle = new Rectangle(new MajorMinor(-measureData.Available.Major / 2, 0).ToPoint(_orientationFlow), majorMinor.ToSize(_orientationFlow));
                layoutNode.AddAreaOfInterest(new AreaOfInterest(rectangle, AreaOfInterestID.ScrollableRange));
            }
            layoutNode.SetVisibleIndexRange(measureData.BeginVisible, measureData.EndVisible, measureData.BeginVisibleOffscreen, measureData.EndVisibleOffscreen, measureData.FocusedItem);
        }

        private void DetermineCount(FlowLayout.Packet packet)
        {
            int num = !(packet.Subject.GetLayoutInput(CountLayoutInput.Data) is CountLayoutInput layoutInput) ? packet.Subject.LayoutChildrenCount : layoutInput.Count;
            packet.PotentialCount = num;
        }

        private void InitializeRecordList(ref Vector<FlowLayout.Record> recordList, int count)
        {
            if (recordList == null)
            {
                recordList = new Vector<FlowLayout.Record>(count);
            }
            else
            {
                int count1 = recordList.Count - count;
                if (count1 > 0)
                    recordList.RemoveRange(count, count1);
                foreach (FlowLayout.Record record in recordList)
                    record?.Clear();
            }
            for (int count1 = recordList.Count; count1 < count; ++count1)
                recordList.Add(null);
        }

        private void CreateRecordsFromChildren(FlowLayout.Packet packet)
        {
            InitializeRecordList(ref packet.Records, packet.PotentialCount);
            Vector<int> availableDataIndices = packet.AvailableDataIndices;
            Vector<int> availableVirtualIndices = packet.AvailableVirtualIndices;
            bool flag = false;
            int childIndex = 0;
            foreach (ILayoutNode layoutChild in packet.Subject.LayoutChildren)
            {
                int virtualIndex;
                int dataIndex;
                IndexType type;
                GetIndex(layoutChild, childIndex, packet.PotentialCount, out virtualIndex, out dataIndex, out int _, out type);
                Vector<FlowLayout.Record> vector = packet.Records;
                if (type == IndexType.Content)
                {
                    availableVirtualIndices.Add(virtualIndex);
                }
                else
                {
                    if (!flag)
                    {
                        InitializeRecordList(ref packet.Dividers, packet.PotentialCount);
                        flag = true;
                    }
                    vector = packet.Dividers;
                }
                FlowLayout.Record record1 = vector[dataIndex];
                if (Record.IsNullOrEmpty(record1))
                {
                    FlowLayout.Record record2 = record1;
                    if (record2 == null)
                        vector[dataIndex] = record2 = new FlowLayout.Record();
                    record2.Initialize(RecordSourceType.LayoutNode);
                    if (record2.Nodes == null)
                        record2.Nodes = new Vector<ILayoutNode>(1);
                    record2.Nodes.Add(layoutChild);
                    record2.Index = dataIndex;
                    if (type == IndexType.Content)
                    {
                        availableDataIndices.Add(record2.Index);
                    }
                    else
                    {
                        layoutChild.Measure(packet.Constraint);
                        record2.Size = new MajorMinor(layoutChild.DesiredSize, Orientation);
                        record2.SizeValid = true;
                    }
                }
                else
                    record1.Nodes.Add(layoutChild);
                ++childIndex;
            }
            if (packet.Cache == null)
            {
                packet.Cache = new FlowSizeMemoryLayoutInput();
                packet.Subject.SetLayoutInput(packet.Cache, false);
            }
            else
            {
                int index = 0;
                foreach (Size knownSiz in packet.Cache.KnownSizes)
                {
                    if (index >= packet.Records.Count)
                        break;
                    if (!knownSiz.IsEmpty)
                    {
                        MajorMinor majorMinor = new MajorMinor(knownSiz, Orientation);
                        if (!AllowWrap)
                            majorMinor = new MajorMinor(majorMinor.Major, 0);
                        FlowLayout.Record record = packet.Records[index];
                        if (!Record.IsNullOrEmpty(record))
                        {
                            record.CachedSize = majorMinor;
                        }
                        else
                        {
                            if (record == null)
                                packet.Records[index] = record = new FlowLayout.Record();
                            record.Initialize(RecordSourceType.SizeCache);
                            record.Index = index;
                            record.CachedSize = majorMinor;
                        }
                        ++index;
                    }
                }
            }
        }

        private void GetIndex(
          ILayoutNode childNode,
          int childIndex,
          int itemsCount,
          out int virtualIndex,
          out int dataIndex,
          out int generationValue,
          out IndexType type)
        {
            IndexLayoutInput layoutInput = (IndexLayoutInput)childNode.GetLayoutInput(IndexLayoutInput.Data);
            if (layoutInput != null)
            {
                virtualIndex = layoutInput.Index.Value;
                ListUtility.GetWrappedIndex(virtualIndex, itemsCount, out dataIndex, out generationValue);
                type = layoutInput.Type;
            }
            else
            {
                virtualIndex = childIndex;
                dataIndex = childIndex;
                generationValue = 0;
                type = IndexType.Content;
            }
        }

        private void HandleMissingRecords(FlowLayout.Packet packet)
        {
            int count = packet.AvailableDataIndices.Count;
            if (packet.PotentialCount - count == 0)
                return;
            MissingItemPolicy missingItemPolicy = MissingItemPolicy;
            if (count < _minimumSampleSizeValue)
                missingItemPolicy = MissingItemPolicy.Wait;
            MajorMinor a = MissingItemPolicy != MissingItemPolicy.SizeToSmallest ? MajorMinor.Zero : new MajorMinor(16777215, 16777215);
            for (int index = 0; index < packet.Records.Count; ++index)
            {
                FlowLayout.Record record = packet.Records[index];
                if (!Record.IsNullOrEmpty(record))
                {
                    if (ListUtility.IsNullOrEmpty(record.Nodes))
                        ++count;
                    switch (missingItemPolicy)
                    {
                        case MissingItemPolicy.SizeToAverage:
                            a += record.CachedSize;
                            continue;
                        case MissingItemPolicy.SizeToSmallest:
                            a = MajorMinor.Min(a, record.CachedSize);
                            continue;
                        case MissingItemPolicy.SizeToLargest:
                            a = MajorMinor.Max(a, record.CachedSize);
                            continue;
                        default:
                            continue;
                    }
                }
            }
            if (packet.PotentialCount == count)
                return;
            if (missingItemPolicy == MissingItemPolicy.SizeToAverage)
            {
                a.Major = (int)Math.Round(a.Major / (double)count);
                a.Minor = (int)Math.Round(a.Minor / (double)count);
            }
            for (int index = 0; index < packet.Records.Count; ++index)
            {
                FlowLayout.Record record = packet.Records[index];
                if (Record.IsNullOrEmpty(record))
                {
                    if (record == null)
                        packet.Records[index] = record = new FlowLayout.Record();
                    record.Initialize(RecordSourceType.Fake);
                    record.Index = index;
                    record.CachedSize = a;
                }
            }
        }

        private void MeasureRecords(FlowLayout.Packet packet)
        {
            MajorMinor offset = MajorMinor.Zero;
            MajorMinor zero = MajorMinor.Zero;
            int num1 = 0;
            int num2 = 0;
            MajorMinor stripSize = MajorMinor.Zero;
            int lastLaidOutIndex = -1;
            int count = packet.Records.Count;
            for (int index = 0; index < packet.Records.Count; ++index)
            {
                FlowLayout.Record record = packet.Records[index];
                MeasureRecord(packet, record, offset);
                MajorMinor majorMinor1 = offset + record.Size;
                MajorMinor majorMinor2 = packet.Available - majorMinor1;
                if (majorMinor2.Minor < 0)
                {
                    record.SizeValid = false;
                    break;
                }
                if (majorMinor2.Major < 0)
                {
                    if (AllowWrap)
                    {
                        if (num2 == 0)
                        {
                            record.SizeValid = false;
                            break;
                        }
                        FinalizeStrip(packet, stripSize, offset, ref zero);
                        ++num1;
                        num2 = 0;
                        stripSize = MajorMinor.Zero;
                        offset = new MajorMinor(0, zero.Minor + Spacing.Minor);
                        --index;
                    }
                    else
                        break;
                }
                else
                {
                    record.Strip = num1;
                    record.Offset = offset;
                    FlowLayout.Record divider = GetDivider(packet, record);
                    if (divider != null)
                    {
                        if (num2 > 0)
                        {
                            MajorMinor majorMinor3 = offset;
                            majorMinor3.Major -= (Spacing.Major + divider.Size.Major) / 2;
                            divider.Offset = majorMinor3;
                            divider.Strip = num1;
                        }
                        else
                            divider.Size = MajorMinor.Zero;
                    }
                    ++num2;
                    stripSize = new MajorMinor(majorMinor1.Major, Math.Max(record.Size.Minor, stripSize.Minor));
                    offset.Major = majorMinor1.Major + Spacing.Major;
                    lastLaidOutIndex = index;
                }
            }
            if (num2 != 0)
            {
                HandleLastItem(packet, ref lastLaidOutIndex, ref offset, ref stripSize);
                FinalizeStrip(packet, stripSize, offset, ref zero);
            }
            packet.LastLaidOutRecord = lastLaidOutIndex;
            packet.StoppedAtRecord = count;
            packet.DesiredSize = zero;
        }

        private void MeasureRecord(
          FlowLayout.Packet packet,
          FlowLayout.Record record,
          MajorMinor offset)
        {
            if (record.SizeValid)
                return;
            Size childConstraint = GetChildConstraint(packet, offset);
            record.Size = MajorMinor.Zero;
            if (record.Nodes != null && record.Nodes.Count > 0)
            {
                for (int index = 0; index < record.Nodes.Count; ++index)
                {
                    ILayoutNode node = record.Nodes[index];
                    GetIndex(node, 0, packet.PotentialCount, out int _, out int _, out int _, out IndexType _);
                    node.Measure(childConstraint);
                    MajorMinor a = new MajorMinor(node.DesiredSize, Orientation);
                    record.Size = index <= 0 || a.Equals(record.Size) ? a : MajorMinor.Max(a, record.Size);
                }
                packet.Cache.KnownSizes.ExpandTo(record.Index + 1);
                packet.Cache.KnownSizes[record.Index] = record.Size.ToSize(Orientation);
            }
            else
                record.Size = record.CachedSize;
            record.SizeValid = true;
        }

        private Size GetChildConstraint(FlowLayout.Packet packet, MajorMinor offset)
        {
            Size constraint = packet.Constraint;
            return AllowWrap ? constraint : (packet.Available - offset).ToSize(Orientation);
        }

        private void FinalizeStrip(
          FlowLayout.Packet packet,
          MajorMinor stripSize,
          MajorMinor offset,
          ref MajorMinor desiredSize)
        {
            packet.StripSizes.Add(stripSize);
            desiredSize = new MajorMinor(Math.Max(desiredSize.Major, stripSize.Major), offset.Minor + stripSize.Minor);
        }

        private FlowLayout.Record GetDivider(
          FlowLayout.Packet packet,
          FlowLayout.Record record)
        {
            return packet.Dividers == null ? null : packet.Dividers[record.Index];
        }

        private void HandleLastItem(
          FlowLayout.Packet packet,
          ref int lastLaidOutIndex,
          ref MajorMinor offset,
          ref MajorMinor stripSize)
        {
            int index = lastLaidOutIndex + 1;
            if (index == packet.Records.Count)
                return;
            FlowLayout.Record record = packet.Records[index];
            if (ListUtility.IsNullOrEmpty(record.Nodes))
                return;
            Size size = (packet.Available - offset).ToSize(Orientation);
            foreach (ILayoutNode node in record.Nodes)
                node.Measure(size);
            Size desiredSize = record.Nodes[0].DesiredSize;
            if (desiredSize.Width > size.Width || desiredSize.Height > size.Height)
                return;
            record.Size = new MajorMinor(desiredSize, Orientation);
            record.SizeValid = true;
            int major = offset.Major + record.Size.Major;
            offset = new MajorMinor(major + Spacing.Major, offset.Minor);
            lastLaidOutIndex = index;
            stripSize = new MajorMinor(major, Math.Max(record.Size.Minor, stripSize.Minor));
        }

        private void DetermineRepeatPolicy(FlowLayout.Packet packet)
        {
            MajorMinor majorMinor = new MajorMinor(packet.Slot.View.Size, Orientation);
            bool flag1 = packet.DesiredSize.Major <= majorMinor.Major;
            bool flag2;
            switch (Repeat)
            {
                case RepeatPolicy.WhenTooBig:
                    flag2 = !flag1;
                    break;
                case RepeatPolicy.WhenTooSmall:
                    flag2 = flag1;
                    break;
                case RepeatPolicy.Always:
                    flag2 = true;
                    break;
                default:
                    flag2 = false;
                    break;
            }
            packet.Repeat = flag2;
        }

        private void CalculateVisualIntersections(FlowLayout.Packet packet)
        {
            if (packet.DesiredSize.IsEmpty)
            {
                packet.BeginVisible = packet.BeginVisibleOffscreen = 0;
                packet.EndVisible = packet.EndVisibleOffscreen = packet.LastLaidOutRecord + 1;
            }
            else
            {
                int major1 = packet.PeripheralStart.Major;
                int major2 = packet.ViewStart.Major;
                int major3 = packet.ViewEnd.Major;
                int major4 = packet.PeripheralEnd.Major;
                packet.BeginVisibleOffscreen = IndexFromPosition(packet, major1, false) - 1;
                packet.BeginVisible = packet.BeginVisibleOffscreen;
                if (major2 != major1)
                    packet.BeginVisible = IndexFromPosition(packet, major2, false) - 1;
                packet.EndVisible = IndexFromPosition(packet, major3, false);
                packet.EndVisibleOffscreen = packet.EndVisible;
                if (major4 == major3)
                    return;
                packet.EndVisibleOffscreen = IndexFromPosition(packet, major4, false);
            }
        }

        private int GetRepeatMajor(FlowLayout.Packet packet, MajorMinor majorMinor) => Orientation != packet.RepeatOrientation ? majorMinor.Minor : majorMinor.Major;

        private int IndexFromPosition(FlowLayout.Packet packet, int position, bool exactMatch)
        {
            int dataIndex1 = position;
            int generationValue = 0;
            if (packet.PotentialCount == packet.LastLaidOutRecord + 1)
            {
                int itemsCount = GetRepeatMajor(packet, packet.DesiredSize) + GetRepeatMajor(packet, RepeatGap);
                ListUtility.GetWrappedIndex(position, itemsCount, out dataIndex1, out generationValue);
            }
            int dataIndex2 = packet.PotentialCount;
            for (int index = 0; index < packet.Records.Count; ++index)
            {
                FlowLayout.Record record = packet.Records[index];
                if (exactMatch && GetRepeatMajor(packet, record.Offset) <= dataIndex1 && GetRepeatMajor(packet, record.Offset) + GetRepeatMajor(packet, record.Size) >= dataIndex1)
                {
                    dataIndex2 = record.Index;
                    break;
                }
                if (index == packet.StoppedAtRecord)
                {
                    dataIndex2 = packet.PotentialCount;
                    break;
                }
                if (GetRepeatMajor(packet, record.Offset) >= dataIndex1 || index > packet.LastLaidOutRecord)
                {
                    dataIndex2 = record.Index;
                    break;
                }
            }
            int num = dataIndex2;
            if (packet.PotentialCount == packet.LastLaidOutRecord + 1)
                num = ListUtility.GetUnwrappedIndex(dataIndex2, generationValue, packet.PotentialCount);
            return num;
        }

        private void FilterIntersectionPoints(FlowLayout.Packet packet)
        {
            int num1 = packet.BeginVisibleOffscreen;
            int num2 = packet.EndVisibleOffscreen;
            if (!packet.Repeat)
            {
                num1 = 0;
                num2 = packet.PotentialCount;
            }
            if (packet.LastLaidOutRecord < packet.PotentialCount - 1)
            {
                num1 = Math.Max(Math.Min(num1, packet.LastLaidOutRecord), 0);
                num2 = Math.Max(Math.Min(num2, packet.LastLaidOutRecord + 2), num1 + 1);
            }
            if (num1 == packet.BeginVisibleOffscreen && num2 == packet.EndVisibleOffscreen)
                return;
            packet.BeginVisibleOffscreen = Math2.Clamp(packet.BeginVisibleOffscreen, num1, num2);
            packet.BeginVisible = Math2.Clamp(packet.BeginVisible, num1, num2);
            packet.BeginVisible = Math.Max(packet.BeginVisible, packet.BeginVisibleOffscreen);
            packet.EndVisible = Math.Max(packet.EndVisible, packet.BeginVisible + 1);
            packet.EndVisible = Math2.Clamp(packet.EndVisible, num1, num2);
            packet.EndVisibleOffscreen = Math2.Clamp(packet.EndVisibleOffscreen, num1, num2);
            packet.EndVisibleOffscreen = Math.Max(packet.EndVisibleOffscreen, packet.EndVisible);
        }

        private void RequestMissingChildren(FlowLayout.Packet packet)
        {
            FilterIntersectionPoints(packet);
            int visibleOffscreen = packet.BeginVisibleOffscreen;
            int num1 = packet.EndVisibleOffscreen - 1;
            int num2 = -1;
            if (packet.AvailableDataIndices.Count < _minimumSampleSizeValue)
                num2 = _minimumSampleSizeValue;
            Vector<int> indiciesList = null;
            for (int index = visibleOffscreen; index <= num1; ++index)
            {
                if (!IntListUtility.Contains(packet.AvailableVirtualIndices, index))
                {
                    if (indiciesList == null)
                        indiciesList = packet.Subject.GetSpecificChildrenRequestList();
                    indiciesList.Add(index);
                    if (num2 > 0 && indiciesList.Count >= num2)
                        break;
                }
            }
            if (ListUtility.IsNullOrEmpty(indiciesList))
                return;
            packet.Subject.RequestSpecificChildren(indiciesList);
        }

        private void ArrangeChildren(FlowLayout.Packet packet)
        {
            MajorMinor majorMinor1 = MajorMinor.Zero;
            int strip = -1;
            int childIndex = 0;
            foreach (ILayoutNode layoutChild in packet.Subject.LayoutChildren)
            {
                int virtualIndex;
                int dataIndex;
                int generationValue;
                IndexType type;
                GetIndex(layoutChild, childIndex, packet.PotentialCount, out virtualIndex, out dataIndex, out generationValue, out type);
                FlowLayout.Record record = type != IndexType.Content ? packet.Dividers[dataIndex] : packet.Records[dataIndex];
                if (!record.SizeValid)
                {
                    layoutChild.MarkHidden();
                }
                else
                {
                    if (type == IndexType.Content && strip != record.Strip)
                    {
                        strip = record.Strip;
                        majorMinor1 = GetStripOffset(packet, strip);
                    }
                    MajorMinor majorMinor2 = PositionFromIndex(packet, record.Offset, generationValue);
                    majorMinor2.Major += majorMinor1.Major;
                    Size size = new MajorMinor(new MajorMinor(layoutChild.DesiredSize, Orientation).Major, majorMinor1.Minor).ToSize(Orientation);
                    Rectangle bounds = new Rectangle(majorMinor2.ToPoint(Orientation), size);
                    layoutChild.Arrange(packet.Slot, bounds);
                    if (layoutChild.ContainsAreaOfInterest(AreaOfInterestID.Focus))
                        packet.FocusedItem = new int?(virtualIndex);
                    ++childIndex;
                }
            }
        }

        private MajorMinor GetStripOffset(FlowLayout.Packet packet, int strip)
        {
            MajorMinor zero = MajorMinor.Zero;
            if (strip >= 0 && strip < packet.StripSizes.Count)
            {
                MajorMinor stripSiz = packet.StripSizes[strip];
                MajorMinor majorMinor1 = new MajorMinor(packet.Slot.Bounds, Orientation);
                switch (StripAlignment)
                {
                    case StripAlignment.Center:
                        zero.Major = (majorMinor1.Major - stripSiz.Major) / 2;
                        break;
                    case StripAlignment.Far:
                        zero.Major = majorMinor1.Major - stripSiz.Major;
                        break;
                }
                if (!AllowWrap)
                {
                    MajorMinor majorMinor2 = new MajorMinor(packet.Slot.Bounds, Orientation);
                    zero.Minor = majorMinor2.Minor;
                }
                else
                    zero.Minor = stripSiz.Minor;
            }
            return zero;
        }

        private MajorMinor PositionFromIndex(
          FlowLayout.Packet packet,
          MajorMinor recordoffset,
          int generationValue)
        {
            if (generationValue == 0)
                return recordoffset;
            MajorMinor majorMinor = recordoffset;
            if (Orientation == packet.RepeatOrientation)
                majorMinor.Major = ListUtility.GetUnwrappedIndex(majorMinor.Major, generationValue, packet.DesiredSize.Major + RepeatGap.Major);
            else
                majorMinor.Minor = ListUtility.GetUnwrappedIndex(majorMinor.Minor, generationValue, packet.DesiredSize.Minor + RepeatGap.Minor);
            return majorMinor;
        }

        public bool Equals(ILayout other)
        {
            if (other is not FlowLayout o) return false;

            return DefaultChildAlignment == o.DefaultChildAlignment
                && Orientation == o.Orientation
                && Spacing == o.Spacing
                && AllowWrap == o.AllowWrap
                && StripAlignment == o.StripAlignment
                && Repeat == o.Repeat
                && RepeatGap == o.RepeatGap
                && MissingItemPolicy == o.MissingItemPolicy
                && MinimumSampleSize == o.MinimumSampleSize;
        }

        internal class Record
        {
            public int Index;
            public Vector<ILayoutNode> Nodes;
            public MajorMinor Size;
            public bool SizeValid;
            public MajorMinor CachedSize;
            public MajorMinor Offset;
            public int Strip;

            public static bool IsNullOrEmpty(FlowLayout.Record record) => record == null || record.Index == int.MinValue;

            public void Clear() => Initialize(RecordSourceType.Unspecified);

            public void Initialize(FlowLayout.RecordSourceType source)
            {
                Index = int.MinValue;
                if (Nodes != null)
                    Nodes.Clear();
                Size = MajorMinor.Zero;
                SizeValid = false;
                CachedSize = MajorMinor.Zero;
                Offset = MajorMinor.Zero;
                Strip = 0;
            }

            public override string ToString() => InvariantString.Format("Record[{0}] Size:{1}", Index, Size);
        }

        internal enum RecordSourceType
        {
            Unspecified,
            LayoutNode,
            SizeCache,
            Fake,
        }

        private class Packet
        {
            public ILayoutNode Subject;
            public Size Constraint;
            public LayoutSlot Slot;
            public MajorMinor Available;
            public FlowSizeMemoryLayoutInput Cache;
            public Orientation RepeatOrientation;
            public MajorMinor PeripheralStart;
            public MajorMinor PeripheralSize;
            public MajorMinor PeripheralEnd;
            public MajorMinor ViewStart;
            public MajorMinor ViewSize;
            public MajorMinor ViewEnd;
            public Vector<FlowLayout.Record> Records;
            public Vector<FlowLayout.Record> Dividers;
            public int PotentialCount;
            public Vector<int> AvailableVirtualIndices = new Vector<int>();
            public Vector<int> AvailableDataIndices = new Vector<int>();
            public MajorMinor DesiredSize;
            public Vector<MajorMinor> StripSizes = new Vector<MajorMinor>();
            public int LastLaidOutRecord;
            public int StoppedAtRecord;
            public bool Repeat;
            public int BeginVisibleOffscreen;
            public int BeginVisible;
            public int EndVisible;
            public int EndVisibleOffscreen;
            public int? FocusedItem;

            public void Clear()
            {
                PeripheralStart = MajorMinor.Zero;
                PeripheralSize = MajorMinor.Zero;
                PeripheralEnd = MajorMinor.Zero;
                ViewStart = MajorMinor.Zero;
                ViewSize = MajorMinor.Zero;
                ViewEnd = MajorMinor.Zero;
                PotentialCount = 0;
                AvailableVirtualIndices.Clear();
                AvailableDataIndices.Clear();
                DesiredSize = MajorMinor.Zero;
                StripSizes.Clear();
                LastLaidOutRecord = 0;
                StoppedAtRecord = 0;
                Repeat = false;
                BeginVisibleOffscreen = 0;
                BeginVisible = 0;
                EndVisible = 0;
                EndVisibleOffscreen = 0;
                FocusedItem = new int?();
            }
        }
    }
}
