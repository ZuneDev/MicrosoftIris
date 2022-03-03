// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.StackIListEnumerator
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System.Collections;

namespace Microsoft.Iris.Render.Internal
{
    internal struct StackIListEnumerator : IEnumerator
    {
        private const int START_INVALID_INDEX = -1;
        private IList m_list;
        private int m_idxCurrent;

        internal StackIListEnumerator(IList list)
        {
            this.m_list = list;
            this.m_idxCurrent = -1;
        }

        public StackIListEnumerator GetEnumerator() => this;

        public bool MoveNext() => ++this.m_idxCurrent < this.m_list.Count;

        public object Current => this.m_list[this.m_idxCurrent];

        public void Reset() => this.m_idxCurrent = -1;
    }
}
