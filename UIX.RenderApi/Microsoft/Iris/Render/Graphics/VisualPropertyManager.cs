// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.VisualPropertyManager
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;
using System;
using System.Collections;

namespace Microsoft.Iris.Render.Graphics
{
    internal class VisualPropertyManager : IDisposable
    {
        private SmartMap<Vector3> m_scaleProp;
        private SmartMap<Vector3> m_centerPointScaleProp;
        private SmartMap<uint> m_layerProp;
        private SmartMap<ArrayList> m_coordMapProp;
        private SmartMap<float> m_alphaProp;
        private SmartMap<AxisAngle> m_rotationProp;
        private SmartMap<Vector<Gradient>> m_gradientProp;
        private SmartMap<Camera> m_cameraProp;

        public VisualPropertyManager()
        {
            this.m_alphaProp = new SmartMap<float>();
            this.m_coordMapProp = new SmartMap<ArrayList>();
            this.m_gradientProp = new SmartMap<Vector<Gradient>>();
            this.m_layerProp = new SmartMap<uint>();
            this.m_rotationProp = new SmartMap<AxisAngle>();
            this.m_scaleProp = new SmartMap<Vector3>();
            this.m_centerPointScaleProp = new SmartMap<Vector3>();
            this.m_cameraProp = new SmartMap<Camera>();
        }

        public void Dispose()
        {
            Debug2.Validate(this.m_alphaProp.IsEmpty, typeof(InvalidOperationException), "empty");
            Debug2.Validate(this.m_coordMapProp.IsEmpty, typeof(InvalidOperationException), "empty");
            Debug2.Validate(this.m_gradientProp.IsEmpty, typeof(InvalidOperationException), "empty");
            Debug2.Validate(this.m_layerProp.IsEmpty, typeof(InvalidOperationException), "empty");
            Debug2.Validate(this.m_rotationProp.IsEmpty, typeof(InvalidOperationException), "empty");
            Debug2.Validate(this.m_scaleProp.IsEmpty, typeof(InvalidOperationException), "empty");
            Debug2.Validate(this.m_centerPointScaleProp.IsEmpty, typeof(InvalidOperationException), "empty");
            Debug2.Validate(this.m_cameraProp.IsEmpty, typeof(InvalidOperationException), "empty");
        }

        public bool GetScaleProp(Visual key, out Vector3 result)
        {
            bool condition = this.m_scaleProp.TryGetValue((uint)key.GetHashCode(), out result);
            Debug2.Validate(condition, typeof(InvalidOperationException), "should have scale");
            return condition;
        }

        public void SetScaleProp(Visual key, Vector3 oValue)
        {
            if (oValue != Vector3.UnitVector)
            {
                this.m_scaleProp.SetValue((uint)key.GetHashCode(), oValue);
                key.SetDynamicValueSet(Visual.PropId.Scale, true);
            }
            else
            {
                if (!key.IsDynamicValueSet(Visual.PropId.Scale))
                    return;
                this.RemoveScaleProp(key);
            }
        }

        public void RemoveScaleProp(Visual key)
        {
            this.m_scaleProp.Remove((uint)key.GetHashCode());
            key.SetDynamicValueSet(Visual.PropId.Scale, false);
        }

        public bool GetAlphaProp(Visual key, out float result)
        {
            bool condition = this.m_alphaProp.TryGetValue((uint)key.GetHashCode(), out result);
            Debug2.Validate(condition, typeof(InvalidOperationException), "should have alpha");
            return condition;
        }

        public void SetAlphaProp(Visual key, float oValue)
        {
            if (!Math2.WithinEpsilon(oValue, 1f))
            {
                this.m_alphaProp.SetValue((uint)key.GetHashCode(), oValue);
                key.SetDynamicValueSet(Visual.PropId.Alpha, true);
            }
            else
            {
                if (!key.IsDynamicValueSet(Visual.PropId.Alpha))
                    return;
                this.RemoveAlphaProp(key);
            }
        }

        public void RemoveAlphaProp(Visual key)
        {
            this.m_alphaProp.Remove((uint)key.GetHashCode());
            key.SetDynamicValueSet(Visual.PropId.Alpha, false);
        }

        public bool GetRotationProp(Visual key, out AxisAngle result)
        {
            bool condition = this.m_rotationProp.TryGetValue((uint)key.GetHashCode(), out result);
            Debug2.Validate(condition, typeof(InvalidOperationException), "should have rotation");
            return condition;
        }

        public void SetRotationProp(Visual key, AxisAngle oValue)
        {
            if (oValue != AxisAngle.Identity)
            {
                this.m_rotationProp.SetValue((uint)key.GetHashCode(), oValue);
                key.SetDynamicValueSet(Visual.PropId.Rotation, true);
            }
            else
            {
                if (!key.IsDynamicValueSet(Visual.PropId.Rotation))
                    return;
                this.RemoveRotationProp(key);
            }
        }

        public void RemoveRotationProp(Visual key)
        {
            this.m_rotationProp.Remove((uint)key.GetHashCode());
            key.SetDynamicValueSet(Visual.PropId.Rotation, false);
        }

        public bool GetCenterPointScaleProp(Visual key, out Vector3 result)
        {
            bool condition = this.m_centerPointScaleProp.TryGetValue((uint)key.GetHashCode(), out result);
            Debug2.Validate(condition, typeof(InvalidOperationException), "should have center point scale");
            return condition;
        }

        public void SetCenterPointScaleProp(Visual key, Vector3 oValue)
        {
            if (oValue != Vector3.Zero)
            {
                this.m_centerPointScaleProp.SetValue((uint)key.GetHashCode(), oValue);
                key.SetDynamicValueSet(Visual.PropId.CenterPointScale, true);
            }
            else
            {
                if (!key.IsDynamicValueSet(Visual.PropId.CenterPointScale))
                    return;
                this.RemoveCenterPointScaleProp(key);
            }
        }

        public void RemoveCenterPointScaleProp(Visual key)
        {
            this.m_centerPointScaleProp.Remove((uint)key.GetHashCode());
            key.SetDynamicValueSet(Visual.PropId.CenterPointScale, false);
        }

        public bool GetLayerProp(Visual key, out uint result)
        {
            bool condition = this.m_layerProp.TryGetValue((uint)key.GetHashCode(), out result);
            Debug2.Validate(condition, typeof(InvalidOperationException), "should have layer");
            return condition;
        }

        public void SetLayerProp(Visual key, uint oValue)
        {
            if (oValue != 0U)
            {
                this.m_layerProp.SetValue((uint)key.GetHashCode(), oValue);
                key.SetDynamicValueSet(Visual.PropId.Layer, true);
            }
            else
            {
                if (!key.IsDynamicValueSet(Visual.PropId.Layer))
                    return;
                this.RemoveLayerProp(key);
            }
        }

        public void RemoveLayerProp(Visual key)
        {
            this.m_layerProp.Remove((uint)key.GetHashCode());
            key.SetDynamicValueSet(Visual.PropId.Layer, false);
        }

        public bool GetGradientProp(Visual key, out Vector<Gradient> result)
        {
            bool condition = this.m_gradientProp.TryGetValue((uint)key.GetHashCode(), out result);
            Debug2.Validate(condition, typeof(InvalidOperationException), "should have gradient");
            return condition;
        }

        public void SetGradientProp(Visual key, Vector<Gradient> oValue)
        {
            this.m_gradientProp.SetValue((uint)key.GetHashCode(), oValue);
            key.SetDynamicValueSet(Visual.PropId.Gradients, true);
        }

        public void RemoveGradientProp(Visual key)
        {
            this.m_gradientProp.Remove((uint)key.GetHashCode());
            key.SetDynamicValueSet(Visual.PropId.Gradients, false);
        }

        public bool GetCameraProp(Visual key, out Camera result)
        {
            bool condition = this.m_cameraProp.TryGetValue((uint)key.GetHashCode(), out result);
            Debug2.Validate(condition, typeof(InvalidOperationException), "should have Camera");
            return condition;
        }

        public void SetCameraProp(Visual key, Camera oValue)
        {
            this.m_cameraProp.SetValue((uint)key.GetHashCode(), oValue);
            key.SetDynamicValueSet(Visual.PropId.Camera, true);
        }

        public void RemoveCameraProp(Visual key)
        {
            this.m_cameraProp.Remove((uint)key.GetHashCode());
            key.SetDynamicValueSet(Visual.PropId.Camera, false);
        }

        public bool GetCoordmapProp(Visual key, out ArrayList result)
        {
            bool condition = this.m_coordMapProp.TryGetValue((uint)key.GetHashCode(), out result);
            Debug2.Validate(condition, typeof(InvalidOperationException), "should have coordmap");
            return condition;
        }

        public void SetCoordmapProp(Visual key, ArrayList oValue)
        {
            this.m_coordMapProp.SetValue((uint)key.GetHashCode(), oValue);
            key.SetDynamicValueSet(Visual.PropId.CoordMap, true);
        }

        public void RemoveCoordmapProp(Visual key)
        {
            this.m_coordMapProp.Remove((uint)key.GetHashCode());
            key.SetDynamicValueSet(Visual.PropId.CoordMap, false);
        }
    }
}
