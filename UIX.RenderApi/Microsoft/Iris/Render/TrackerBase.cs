// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.TrackerBase
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Render
{
    public abstract class TrackerBase
    {
        private static readonly TrackerBase.ObjectTest s_testAll = new TrackerBase.ObjectTest(AllObjectTest);
        private static readonly TrackerBase.ObjectTest s_testDead = new TrackerBase.ObjectTest(DeadObjectTest);
        private static readonly TrackerBase.ObjectTest s_testObject = new TrackerBase.ObjectTest(SpecificObjectTest);
        private Map<object, GCHandle> m_tblObjects;

        ~TrackerBase() => this.Dispose(false);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        protected virtual void Dispose(bool fInDispose) => this.RemoveObjects(s_testAll);

        public bool IsEmpty => this.m_tblObjects.Count == 0;

        public ArrayList LiveObjects
        {
            get
            {
                ArrayList arrayList = new ArrayList();
                lock (this)
                {
                    if (this.m_tblObjects != null)
                    {
                        foreach (KeyValueEntry<object, GCHandle> tblObject in this.m_tblObjects)
                        {
                            object target = tblObject.Value.Target;
                            if (target != null)
                                arrayList.Add(target);
                        }
                    }
                }
                return arrayList;
            }
        }

        public void AddObject(object key, object value)
        {
            lock (this)
            {
                if (this.m_tblObjects == null)
                    this.SetupObjectTable();
                this.m_tblObjects[key] = GCHandle.Alloc(value, GCHandleType.Weak);
            }
        }

        public void RemoveKey(object key)
        {
            lock (this)
            {
                GCHandle gcHandle;
                if (!this.m_tblObjects.TryGetValue(key, out gcHandle))
                {
                    Debug2.Validate(false, typeof(InvalidOperationException), "Unable to find object with key " + key);
                }
                else
                {
                    gcHandle.Free();
                    this.m_tblObjects.Remove(key);
                }
            }
        }

        public void RemoveKeys(TrackerBase.KeyTest keyTest)
        {
            lock (this)
            {
                if (this.m_tblObjects == null)
                    return;
                ArrayList arrayList = null;
                foreach (string key in this.m_tblObjects.Keys)
                {
                    if (keyTest == null || keyTest(key))
                    {
                        if (arrayList == null)
                            arrayList = new ArrayList();
                        arrayList.Add(key);
                    }
                }
                if (arrayList == null)
                    return;
                foreach (object key in arrayList)
                    this.RemoveKey(key);
            }
        }

        public void RemoveObject(object objRemove) => this.RemoveObjects(s_testObject, objRemove);

        public void RemoveDeadObjects() => this.RemoveObjects(s_testDead);

        public void RemoveAllObjects() => this.RemoveObjects(s_testAll);

        public void RemoveObjects(TrackerBase.ObjectTest test) => this.RemoveObjects(test, null);

        public void RemoveObjects(TrackerBase.ObjectTest test, object objParam)
        {
            lock (this)
            {
                if (this.m_tblObjects == null)
                    return;
                ArrayList arrayList = null;
                foreach (KeyValueEntry<object, GCHandle> tblObject in this.m_tblObjects)
                {
                    GCHandle gcHandle = tblObject.Value;
                    object target = gcHandle.Target;
                    if (target == null || test(target, objParam))
                    {
                        if (arrayList == null)
                            arrayList = new ArrayList();
                        object key = tblObject.Key;
                        arrayList.Add(key);
                        gcHandle.Free();
                    }
                }
                if (arrayList == null)
                    return;
                foreach (object key in arrayList)
                    this.m_tblObjects.Remove(key);
            }
        }

        public object Find(object key)
        {
            object obj = null;
            lock (this)
            {
                if (this.m_tblObjects != null)
                {
                    GCHandle gcHandle;
                    if (this.m_tblObjects.TryGetValue(key, out gcHandle))
                    {
                        obj = gcHandle.Target;
                        if (obj == null)
                        {
                            this.m_tblObjects.Remove(key);
                            gcHandle.Free();
                        }
                    }
                }
            }
            return obj;
        }

        public bool ContainsKey(object key)
        {
            bool flag = false;
            lock (this)
            {
                if (this.m_tblObjects != null)
                    flag = this.m_tblObjects.ContainsKey(key);
            }
            return flag;
        }

        protected virtual void SetupObjectTable()
        {
            lock (this)
            {
                if (this.m_tblObjects != null)
                    return;
                this.m_tblObjects = new Map<object, GCHandle>();
            }
        }

        private static bool AllObjectTest(object objSubject, object objParam) => true;

        private static bool DeadObjectTest(object objSubject, object objParam) => false;

        private static bool SpecificObjectTest(object objSubject, object objParam) => objSubject == objParam;

        public delegate bool ObjectTest(object objSubject, object objParam);

        public delegate bool KeyTest(string key);
    }
}
