// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.CodeModel.Cpp.DllProxyObjectReference
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Runtime.InteropServices;

namespace Microsoft.Iris.CodeModel.Cpp
{
    internal struct DllProxyObjectReference
    {
        private GCHandle _reference;

        public object Value
        {
            get
            {
                object obj = null;
                if (_reference.IsAllocated)
                    obj = _reference.Target;
                return obj;
            }
            set
            {
                if (value != null)
                {
                    if (_reference.IsAllocated)
                        _reference.Target = value;
                    else
                        _reference = GCHandle.Alloc(value, GCHandleType.Weak);
                }
                else
                {
                    if (!_reference.IsAllocated)
                        return;
                    _reference.Free();
                }
            }
        }
    }
}
