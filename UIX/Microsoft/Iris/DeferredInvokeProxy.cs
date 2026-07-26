// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.DeferredInvokeProxy
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Session;

namespace Microsoft.Iris
{
    internal class DeferredInvokeProxy
    {
        private readonly DeferredInvokeHandler _method;

        private DeferredInvokeProxy(DeferredInvokeHandler method)
        {
            _method = method;
        }

        public static DeferredHandler Thunk(DeferredInvokeHandler method) => new DeferredInvokeProxy(method).Thunk;

        private void Thunk(object args) => _method(args);

        public override string ToString() => $"{_method.Method.DeclaringType?.FullName}.{_method.Method.Name}";
    }
}
