// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.TraceRegistryHelper
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render.Extensions
{
    internal class TraceRegistryHelper : RegistryListener
    {
        private const string k_stReadValuesKey = "ReadValuesFromRegistry";
        private const string k_stSaveValuesKey = "SaveValuesToRegistry";
        private const string k_stTimedWriteLinesKey = "TimedWriteLines";
        private bool m_invalidCache;
        private bool m_fReadValuesFromRegistry;
        private bool m_fSaveValuesToRegistry;

        internal TraceRegistryHelper(string stRegKey)
          : base(stRegKey)
          => this.m_invalidCache = true;

        private void Refresh()
        {
            lock (this)
            {
                bool fValue;
                if (this.ReadValueFromRegistry("ReadValuesFromRegistry", out fValue))
                    this.m_fReadValuesFromRegistry = fValue;
                if (this.ReadValueFromRegistry("SaveValuesToRegistry", out fValue))
                    this.m_fSaveValuesToRegistry = fValue;
                this.m_invalidCache = false;
            }
        }

        internal void Invalidate()
        {
            if (this.m_invalidCache)
                return;
            lock (this)
            {
                if (this.m_invalidCache)
                    return;
                this.m_invalidCache = true;
            }
        }

        public bool ReadValuesFromRegistry
        {
            get
            {
                if (this.m_invalidCache)
                    this.Refresh();
                return this.m_fReadValuesFromRegistry;
            }
            set
            {
                if (this.m_fReadValuesFromRegistry == value)
                    return;
                this.m_fReadValuesFromRegistry = value;
            }
        }

        public bool SaveValuesToRegistry
        {
            get
            {
                if (this.m_invalidCache)
                    this.Refresh();
                return this.m_fSaveValuesToRegistry;
            }
            set
            {
                if (this.m_fSaveValuesToRegistry == value)
                    return;
                this.m_fSaveValuesToRegistry = value;
            }
        }
    }
}
