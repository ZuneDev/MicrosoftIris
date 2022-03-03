// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Extensions.DebugCategoryEnabledSet
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System.Security;

namespace Microsoft.Iris.Render.Extensions
{
    [SuppressUnmanagedCodeSecurity]
    internal class DebugCategoryEnabledSet
    {
        private byte[] m_arbCache;
        private TraceRegistryHelper m_registryHelper;
        private bool m_invalidCache;

        internal DebugCategoryEnabledSet(TraceRegistryHelper registryHelper)
        {
            this.m_arbCache = new byte[38];
            for (int index = 0; index < this.m_arbCache.Length; ++index)
                this.m_arbCache[index] = 0;
            this.m_registryHelper = registryHelper;
            this.m_invalidCache = true;
        }

        public byte this[DebugCategory cat]
        {
            get => this.GetLevel(cat);
            set => this.SetLevel(cat, value);
        }

        public byte GetLevel(DebugCategory cat)
        {
            if (this.m_invalidCache)
                this.Refresh();
            return this.m_arbCache[(int)cat];
        }

        public void SetLevel(DebugCategory cat, byte bValue)
        {
            if (this.m_arbCache[(int)cat] == bValue)
                return;
            SetExternalDebugLevel(cat, bValue);
            this.m_arbCache[(int)cat] = bValue;
        }

        public void SetLevel(DebugCategory cat, bool fValue)
        {
            byte bValue = fValue ? (byte)1 : (byte)0;
            if (bValue != 0 && bValue <= this.GetLevel(cat))
                return;
            this.SetLevel(cat, bValue);
        }

        private void Refresh()
        {
            lock (this)
            {
                for (int index = 0; index < this.m_arbCache.Length; ++index)
                {
                    DebugCategory cat = (DebugCategory)index;
                    byte externalDebugLevel = GetExternalDebugLevel(cat);
                    byte level = externalDebugLevel;
                    byte bValue;
                    if (this.m_registryHelper.ReadValuesFromRegistry && this.ReadDebugCategoryValueFromRegistry(cat, out bValue))
                        level = bValue;
                    if (externalDebugLevel != level)
                        SetExternalDebugLevel(cat, level);
                    if (this.m_arbCache[index] != level)
                        this.m_arbCache[index] = level;
                }
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

        private bool ReadDebugCategoryValueFromRegistry(DebugCategory cat, out byte bValue) => this.m_registryHelper.ReadValueFromRegistry(cat.ToString(), out bValue);

        private static byte GetExternalDebugLevel(DebugCategory cat) => IsExternalCategory(cat) ? eDebugApi.DebugGetCategoryLevel(cat) : (byte)0;

        private static void SetExternalDebugLevel(DebugCategory cat, byte level)
        {
            if (!IsExternalCategory(cat))
                return;
            eDebugApi.DebugSetCategoryLevel(cat, level);
        }

        private static bool IsExternalCategory(DebugCategory cat) => (uint)cat < 25U;
    }
}
