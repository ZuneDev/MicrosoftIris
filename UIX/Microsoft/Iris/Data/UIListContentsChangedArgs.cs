// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Data.UIListContentsChangedArgs
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Data
{
    internal class UIListContentsChangedArgs : EventArgs
    {
        private UIListContentsChangeType _type;
        private int _oldIndex;
        private int _newIndex;
        private int _count;

        public UIListContentsChangedArgs(
          UIListContentsChangeType type,
          int oldIndex,
          int newIndex,
          int count)
        {
            _count = count;
            _type = type;
            _oldIndex = oldIndex;
            _newIndex = newIndex;
        }

        public UIListContentsChangedArgs(UIListContentsChangeType type, int oldIndex, int newIndex)
          : this(type, oldIndex, newIndex, 1)
        {
        }

        public UIListContentsChangeType Type => _type;

        public int OldIndex => _oldIndex;

        public int NewIndex => _newIndex;

        public int Count => _count;

        public override string ToString() => string.Format("{0} type: {1}, old: {2}, new: {3}, count: {4}", base.ToString(), Type, OldIndex, NewIndex, Count);
    }
}
