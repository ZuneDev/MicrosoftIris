// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.DataTracker
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;

namespace Microsoft.Iris.Render.Extensions
{
    public class DataTracker : IDisposable
    {
        private Map<object, IDisposable> m_dictData;

        public DataTracker() => this.m_dictData = new Map<object, IDisposable>();

        public void AddData(object o, IDisposable data)
        {
            Debug2.Validate(o != null, typeof(ArgumentNullException), "IRenderObject is invalid");
            Debug2.Validate(data != null, typeof(ArgumentNullException), "Data is invalid");
            Debug2.Validate(!this.m_dictData.ContainsKey(o), typeof(InvalidOperationException), "DataTracker only supports one resource per IRenderObject");
            this.m_dictData[o] = data;
        }

        public void DisposeAndRemoveData(object o)
        {
            IDisposable disposable;
            if (!this.m_dictData.TryGetValue(o, out disposable))
                return;
            disposable.Dispose();
            this.m_dictData.Remove(o);
        }

        public void Dispose()
        {
            foreach (IDisposable disposable in this.m_dictData.Values)
                disposable.Dispose();
            this.m_dictData = null;
            GC.SuppressFinalize(this);
        }
    }
}
