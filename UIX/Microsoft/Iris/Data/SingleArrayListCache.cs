// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Data.SingleArrayListCache
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections;

namespace Microsoft.Iris.Data
{
    internal struct SingleArrayListCache
    {
        private ArrayList _list;

        public ArrayList Acquire()
        {
            ArrayList arrayList = _list;
            _list = null;
            if (arrayList == null)
                arrayList = new ArrayList();
            return arrayList;
        }

        public void Release(ArrayList list)
        {
            list.Clear();
            _list = list;
        }
    }
}
