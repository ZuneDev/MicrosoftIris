// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllProxyHandleTable
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal class DllProxyHandleTable : ProxyHandleTable<DllProxyObjectReference>
    {
        public DllProxyHandleTable.DllProxyHandleTableEnumerator GetEnumerator() => new DllProxyHandleTable.DllProxyHandleTableEnumerator(GetTableEnumerator());

        public ulong RegisterProxy(object d) => AllocateHandle(new DllProxyObjectReference()
        {
            Value = d
        });

        public void ReleaseProxy(ulong handle)
        {
            DllProxyObjectReference oldValue;
            ReleaseHandle(handle, out oldValue);
            oldValue.Value = null;
        }

        protected bool LookupByHandleWorker(ulong handle, out object obj)
        {
            DllProxyObjectReference proxyObjectReference;
            bool flag = InternalLookupByHandle(handle, out proxyObjectReference);
            obj = !flag ? null : proxyObjectReference.Value;
            return flag;
        }

        internal struct DllProxyHandleTableEnumerator : IEnumerator
        {
            private ProxyHandleTable<DllProxyObjectReference>.ProxyHandleTableEnumerator _enumerator;

            public DllProxyHandleTableEnumerator(
              ProxyHandleTable<DllProxyObjectReference>.ProxyHandleTableEnumerator enumerator)
            {
                _enumerator = enumerator;
            }

            public bool MoveNext()
            {
                bool flag;
                do
                {
                    flag = _enumerator.MoveNext();
                }
                while ((!flag || _enumerator.Current.Value == null) && flag);
                return flag;
            }

            public object Current => _enumerator.Current.Value;

            public void Reset() => _enumerator.Reset();
        }
    }
}
