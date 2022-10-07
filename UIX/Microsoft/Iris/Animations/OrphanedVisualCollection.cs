// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.OrphanedVisualCollection
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Render;
using System;

namespace Microsoft.Iris.Animations
{
    public class OrphanedVisualCollection : DisposableObject
    {
        private int _countEventsRemaining;
        private Vector<IVisual> _orphansList;
        private Vector<ActiveSequence> _sequenceList;
        private AnimationManager _animationManager;

        public OrphanedVisualCollection(AnimationManager aniManager)
        {
            DeclareOwner(aniManager);
            _orphansList = new Vector<IVisual>();
            _sequenceList = new Vector<ActiveSequence>();
            _animationManager = aniManager;
            _countEventsRemaining = 1;
            _animationManager.RegisterAnimatedOrphans(this);
        }

        protected override void OnDispose()
        {
            foreach (ActiveSequence sequence in _sequenceList)
            {
                sequence.Stop();
                sequence.Dispose(this);
            }
            _sequenceList.Clear();
            foreach (IVisual orphans in _orphansList)
            {
                orphans.Remove();
                orphans.UnregisterUsage(this);
            }
            _orphansList.Clear();
            _animationManager = null;
            base.OnDispose();
        }

        public void AddOrphan(IVisual visual)
        {
            visual.RegisterUsage(this);
            _orphansList.Add(visual);
        }

        public void RegisterWaitForAnimation(ActiveSequence aseq, bool transfer)
        {
            aseq.AnimationCompleted += new EventHandler(OnDestroyAnimationComplete);
            ++_countEventsRemaining;
            RegisterAnimation(aseq, transfer);
        }

        public void RegisterAnimation(ActiveSequence aseq, bool transfer)
        {
            if (transfer)
                aseq.TransferOwnership(this);
            else
                aseq.DeclareOwner(this);
            _sequenceList.Add(aseq);
        }

        public void OnLayoutApplyComplete() => OnEventComplete(null, EventArgs.Empty);

        private void OnDestroyAnimationComplete(object sender, EventArgs args)
        {
            ActiveSequence activeSequence = sender as ActiveSequence;
            activeSequence.AnimationCompleted -= new EventHandler(OnDestroyAnimationComplete);
            _sequenceList.Remove(activeSequence);
            activeSequence.Dispose(this);
            OnEventComplete(sender, args);
        }

        private void OnEventComplete(object sender, EventArgs args)
        {
            --_countEventsRemaining;
            if (Waiting)
                return;
            if (_animationManager != null)
                _animationManager.UnregisterAnimatedOrphans(this);
            Dispose(_animationManager);
        }

        public bool Waiting => _countEventsRemaining > 0;

        public override string ToString() => InvariantString.Format("Orphans(WaitCount={0}, OrphanCount={1})", _sequenceList != null ? _sequenceList.Count.ToString() : "None", _orphansList.Count);
    }
}
