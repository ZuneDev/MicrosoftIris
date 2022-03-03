// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Common.SharedRenderObject
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;
using System.Diagnostics;

namespace Microsoft.Iris.Render.Common
{
    internal class SharedRenderObject : ISharedRenderObject
    {
        private int m_cUsers;

        internal SharedRenderObject()
        {
        }

        ~SharedRenderObject() => this.Dispose(false);

        private void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool fInDispose)
        {
        }

        int ISharedRenderObject.UsageCount => this.m_cUsers;

        void ISharedRenderObject.RegisterUsage(object user) => this.RegisterUsage(user);

        void ISharedRenderObject.UnregisterUsage(object user) => this.UnregisterUsage(user);

        internal virtual void RegisterUsage(object user) => ++this.m_cUsers;

        internal virtual void UnregisterUsage(object user)
        {
            --this.m_cUsers;
            if (this.m_cUsers != 0)
                return;
            this.Dispose();
        }

        internal int UsageCount => this.m_cUsers;

        [Conditional("DEBUG")]
        public static void DEBUG_BeginTrackingObjects()
        {
        }

        [Conditional("DEBUG")]
        public static void DEBUG_StopTrackingObjects()
        {
        }

        internal string DebugDescription
        {
            get => "<unknown>";
            set
            {
            }
        }

        [Conditional("DEBUG")]
        protected virtual void Invariant()
        {
        }

        [Conditional("DEBUG")]
        private void Invariant(bool fValidateUsers)
        {
            if (!fValidateUsers)
                return;
            Debug2.Validate(this.m_cUsers > 0, typeof(ObjectDisposedException), "No registered user");
        }
    }
}
