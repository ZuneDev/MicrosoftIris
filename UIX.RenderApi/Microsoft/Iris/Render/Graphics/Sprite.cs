// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.Sprite
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using Microsoft.Iris.Render.Protocols.Splash.Rendering;
using System;
using System.Collections;

namespace Microsoft.Iris.Render.Graphics
{
    internal class Sprite : Visual, ISprite, IVisual, IRawInputSite, IAnimatable, ISharedRenderObject
    {
        private RemoteSprite m_remoteSprite;
        private Effect m_effect;

        public Sprite(RenderSession session, RenderWindow window, object objOwnerData)
          : base(session, window, objOwnerData)
        {
            this.m_remoteSprite = session.BuildRemoteSprite(this, window);
            this.m_remoteSprite.SendSetVisible(true);
            this.m_remoteVisual = m_remoteSprite;
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (fInDispose && this.m_effect != null)
                {
                    this.m_effect.UnregisterUsage(this);
                    this.m_effect = null;
                }
                this.PropertyManager.RemoveCoordmapProp(this);
                this.m_remoteSprite = null;
                this.m_effect = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        Vector3 ISprite.Position
        {
            get => this.Position;
            set => this.Position = value;
        }

        Vector2 ISprite.Size
        {
            get => this.Size;
            set => this.Size = value;
        }

        bool ISprite.Visible
        {
            get => this.Visible;
            set => this.Visible = value;
        }

        Vector3 ISprite.Scale
        {
            get => this.Scale;
            set => this.Scale = value;
        }

        AxisAngle ISprite.Rotation
        {
            get => this.Rotation;
            set => this.Rotation = value;
        }

        float ISprite.Alpha
        {
            get => this.Alpha;
            set => this.Alpha = value;
        }

        Vector3 ISprite.CenterPoint
        {
            get => this.CenterPointScale;
            set => this.CenterPointScale = value;
        }

        uint ISprite.Layer
        {
            get => this.Layer;
            set => this.Layer = value;
        }

        IEffect ISprite.Effect
        {
            get => Effect;
            set => this.Effect = (Effect)value;
        }

        internal Effect Effect
        {
            get => this.m_effect;
            set
            {
                Effect effect = value;
                if (this.m_effect == effect)
                    return;
                if (this.m_effect != null)
                    this.m_effect.UnregisterUsage(this);
                this.m_effect = effect;
                if (this.m_effect != null)
                    this.m_effect.RegisterUsage(this);
                this.m_remoteSprite.SendSetEffect(this.m_effect == null ? null : this.m_effect.RemoteStub);
            }
        }

        bool ISprite.RelativeSize
        {
            get => this.RelativeSize;
            set => this.RelativeSize = value;
        }

        void ISprite.AddGradient(IGradient gradient) => this.AddGradient((Gradient)gradient);

        void ISprite.RemoveAllGradients() => this.RemoveAllGradients();

        void ISprite.SetNineGrid(int left, int top, int right, int bottom)
        {
            Debug2.Validate(left >= 0 && right >= 0 && top >= 0 && bottom >= 0, typeof(ArgumentNullException), "invalid nine grid params");
            this.PropertyManager.RemoveCoordmapProp(this);
            this.m_remoteSprite.SendSetNineGrid(left, top, right, bottom);
            this.SetPropFlag(PropId.MaxDynamicProp, true);
        }

        void ISprite.SetCoordMap(int idxLayer, CoordMap coordMap)
        {
            Debug2.Validate(idxLayer >= 0, typeof(ArgumentOutOfRangeException), "idxLayer must be >= 0");
            Debug2.Validate(idxLayer < 4, typeof(ArgumentOutOfRangeException), "quick-check, currently we only support 4 layers/stages");
            if (this.IsPropFlagSet(PropId.MaxDynamicProp))
            {
                this.m_remoteSprite.SendClearCoordMaps();
                this.SetPropFlag(PropId.MaxDynamicProp, false);
            }
            if (!this.UpdateCoordMap(idxLayer, coordMap))
                return;
            this.m_remoteSprite.SendClearCoordMaps();
            if (!this.IsDynamicValueSet(PropId.CoordMap))
                return;
            ArrayList result;
            this.PropertyManager.GetCoordmapProp(this, out result);
            foreach (Sprite.CoordMapEntry coordMapEntry in result)
            {
                foreach (CoordMap.CoordMapSample rampSample in coordMapEntry.coordMap.RampSamples)
                    this.m_remoteSprite.SendAddCoordMapEntry(coordMapEntry.idxLayer, rampSample.Orientation, rampSample.flPosition, rampSample.flValue);
            }
        }

        private bool UpdateCoordMap(int idxLayer, CoordMap coordMap)
        {
            ArrayList result;
            if (this.IsDynamicValueSet(PropId.CoordMap))
            {
                this.PropertyManager.GetCoordmapProp(this, out result);
            }
            else
            {
                if (coordMap == null)
                    return false;
                result = new ArrayList();
                this.PropertyManager.SetCoordmapProp(this, result);
            }
            Sprite.CoordMapEntry coordMapEntry1 = null;
            foreach (Sprite.CoordMapEntry coordMapEntry2 in result)
            {
                if (coordMapEntry2.idxLayer == idxLayer)
                {
                    coordMapEntry1 = coordMapEntry2;
                    break;
                }
            }
            if (coordMapEntry1 == null)
            {
                if (coordMap == null)
                    return false;
                result.Add(new Sprite.CoordMapEntry(idxLayer, coordMap));
                return true;
            }
            if (coordMap == null)
                result.Remove(coordMapEntry1);
            else
                coordMapEntry1.coordMap = coordMap;
            return true;
        }

        private void ResetCoordmaps()
        {
            if (this.IsPropFlagSet(PropId.MaxDynamicProp) || this.IsDynamicValueSet(PropId.CoordMap))
            {
                this.m_remoteSprite.SendClearCoordMaps();
                this.SetPropFlag(PropId.MaxDynamicProp, false);
            }
            this.PropertyManager.RemoveCoordmapProp(this);
        }

        internal override void Reset()
        {
            base.Reset();
            this.ResetCoordmaps();
            this.Effect = null;
        }

        private class CoordMapEntry
        {
            internal int idxLayer;
            internal CoordMap coordMap;

            internal CoordMapEntry(int idxLayerParam, CoordMap coordMapParam)
            {
                this.idxLayer = idxLayerParam;
                this.coordMap = coordMapParam;
            }
        }
    }
}
