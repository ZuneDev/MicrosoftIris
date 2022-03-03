// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Markup.ScriptRunScheduler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System.Collections;

namespace Microsoft.Iris.Markup
{
    internal struct ScriptRunScheduler
    {
        private Vector<ScriptRunScheduler.PendingScript> _pendingList;
        private static ScriptRunScheduler.RunListCache s_listCache = new ScriptRunScheduler.RunListCache();

        public bool Pending => _pendingList != null;

        public void ScheduleRun(uint scriptId, bool ignoreErrors)
        {
            if (_pendingList == null)
                _pendingList = s_listCache.Acquire();
            int index;
            for (index = 0; index < _pendingList.Count; ++index)
            {
                uint scriptId1 = _pendingList[index].ScriptId;
                if ((int)scriptId == (int)scriptId1)
                    return;
                if (scriptId < scriptId1)
                    break;
            }
            _pendingList.Insert(index, new ScriptRunScheduler.PendingScript(scriptId, ignoreErrors));
        }

        public void Execute(IMarkupTypeBase markupTypeBase)
        {
            if (_pendingList == null)
                return;
            Vector<ScriptRunScheduler.PendingScript> pendingList = _pendingList;
            _pendingList = null;
            for (int index = 0; index < pendingList.Count; ++index)
            {
                ScriptRunScheduler.PendingScript pendingScript = pendingList[index];
                markupTypeBase.RunScript(pendingScript.ScriptId, pendingScript.IgnoreErrors, new ParameterContext());
            }
            s_listCache.Release(pendingList);
        }

        internal struct PendingScript
        {
            public uint ScriptId;
            public bool IgnoreErrors;

            public PendingScript(uint scriptId, bool ignoreErrors)
            {
                ScriptId = scriptId;
                IgnoreErrors = ignoreErrors;
            }
        }

        private class RunListCache
        {
            private const int c_MaxNumLists = 128;
            private const int c_MaxListSize = 32;
            private Stack _lists = new Stack(128);

            public Vector<ScriptRunScheduler.PendingScript> Acquire() => _lists.Count <= 0 ? new Vector<ScriptRunScheduler.PendingScript>() : (Vector<ScriptRunScheduler.PendingScript>)_lists.Pop();

            public void Release(Vector<ScriptRunScheduler.PendingScript> list)
            {
                if (_lists.Count >= 128 || list.Capacity > 32)
                    return;
                list.Clear();
                _lists.Push(list);
            }
        }
    }
}
