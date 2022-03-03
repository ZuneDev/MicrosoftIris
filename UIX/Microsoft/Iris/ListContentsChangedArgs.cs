// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ListContentsChangedArgs
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris
{
    public class ListContentsChangedArgs : EventArgs
    {
        private ListContentsChangeType _type;
        private int _oldIndex;
        private int _newIndex;
        private int _count;

        internal ListContentsChangedArgs(
          ListContentsChangeType type,
          int oldIndex,
          int newIndex,
          int count)
        {
            _count = count;
            _type = type;
            _oldIndex = oldIndex;
            _newIndex = newIndex;
        }

        internal ListContentsChangedArgs(ListContentsChangeType type, int oldIndex, int newIndex)
          : this(type, oldIndex, newIndex, 1)
        {
        }

        public ListContentsChangeType Type => _type;

        public int OldIndex => _oldIndex;

        public int NewIndex => _newIndex;

        public int Count => _count;
    }
}
