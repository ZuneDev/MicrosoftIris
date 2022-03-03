// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Internal.ObjectTracker
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render.Internal
{
    internal sealed class ObjectTracker : TrackerBase
    {
        private ObjectTracker.ThreadMode m_nMode;
        private int m_idxNextIdentity;
        private GCHandle m_hndOwner;

        public ObjectTracker(RenderSession session, object objOwner)
          : this(session, ThreadMode.Private, objOwner)
        {
        }

        public ObjectTracker(RenderSession session, ObjectTracker.ThreadMode nMode, object objOwner)
        {
            Debug2.Validate(session != null || nMode != ThreadMode.Private, typeof(ArgumentException), "must specify a session for private trackers");
            Debug2.Validate(objOwner != null || nMode == ThreadMode.Master, typeof(ArgumentException), "must specify an owner (except mater tracker)");
            if (nMode == ThreadMode.Private)
                ObjectTrackerGroup.RegisterChildTracker(this);
            this.m_nMode = nMode;
            this.m_idxNextIdentity = 1;
            this.m_hndOwner = GCHandle.Alloc(objOwner, GCHandleType.Weak);
        }

        protected override void Dispose(bool fInDispose)
        {
            base.Dispose(fInDispose);
            if (this.m_hndOwner.IsAllocated)
            {
                this.m_hndOwner.Free();
                this.m_hndOwner = new GCHandle();
            }
            int num = fInDispose ? 1 : 0;
        }

        public object Owner
        {
            get
            {
                object obj = null;
                if (this.m_hndOwner.IsAllocated)
                    obj = this.m_hndOwner.Target;
                return obj;
            }
        }

        public int AddObject(object o)
        {
            int num;
            lock (this)
            {
                num = this.m_idxNextIdentity++;
                this.AddObject(num, o);
            }
            return num;
        }

        public void RemoveObject(int nIdentity) => this.RemoveKey(nIdentity);

        public object Find(int nIdentity) => this.Find((object)nIdentity);

        protected override void SetupObjectTable()
        {
            lock (this)
                base.SetupObjectTable();
        }

        private void DoCleanup()
        {
            if (this.Owner == null)
                this.Dispose();
            else
                this.RemoveDeadObjects();
        }

        private void CleanupTimer(object sender, EventArgs args)
        {
            this.DoCleanup();
            foreach (ObjectTracker liveObject in this.LiveObjects)
                liveObject.DoCleanup();
        }

        public enum ThreadMode
        {
            Private,
            Shared,
            Master,
        }
    }
}
