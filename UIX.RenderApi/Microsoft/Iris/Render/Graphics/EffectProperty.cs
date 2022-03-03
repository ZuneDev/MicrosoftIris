// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.Graphics.EffectProperty
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Internal;

namespace Microsoft.Iris.Render.Graphics
{
    internal class EffectProperty : RenderObject
    {
        private const byte k_dynamicFlag = 128;
        private byte m_nID;
        private object m_oValue;
        private byte m_propertyType;

        internal EffectProperty(
          string stName,
          EffectPropertyType eptType,
          bool fIsDynamic,
          object oValue,
          byte nID)
        {
            this.m_propertyType = (byte)eptType;
            this.IsDynamic = fIsDynamic;
            this.m_nID = nID;
            this.Value = oValue;
        }

        private EffectProperty(EffectProperty copy)
        {
            this.m_propertyType = copy.m_propertyType;
            this.IsDynamic = copy.IsDynamic;
            this.m_nID = copy.m_nID;
            this.Value = copy.Value;
        }

        protected override void Dispose(bool fInDispose)
        {
            try
            {
                if (!fInDispose || this.m_oValue == null)
                    return;
                this.ModifyUsage(false);
                this.m_oValue = null;
            }
            finally
            {
                base.Dispose(fInDispose);
            }
        }

        internal object Value
        {
            get => this.m_oValue;
            set
            {
                this.ModifyUsage(false);
                this.m_oValue = value;
                this.ModifyUsage(true);
            }
        }

        internal bool IsDynamic
        {
            get => (m_propertyType & 128) == 128;
            set
            {
                if (value)
                    this.m_propertyType |= 128;
                else
                    this.m_propertyType &= 127;
            }
        }

        internal bool IsSharedResource => this.Type == EffectPropertyType.Image || this.Type == EffectPropertyType.ImageArray || this.Type == EffectPropertyType.Video;

        internal EffectPropertyType Type => (EffectPropertyType)(m_propertyType & 4294967167U);

        internal byte ID
        {
            get => this.m_nID;
            set => this.m_nID = value;
        }

        internal bool HasIdenticalValue(object oValue)
        {
            switch (this.Type)
            {
                case EffectPropertyType.Integer:
                    return (int)this.Value == (int)oValue;
                case EffectPropertyType.Float:
                    return !Math2.WithinEpsilon((float)this.Value, (float)oValue);
                case EffectPropertyType.Vector2:
                    return (Vector2)this.Value == (Vector2)oValue;
                case EffectPropertyType.Vector3:
                    return (Vector3)this.Value == (Vector3)oValue;
                case EffectPropertyType.Vector4:
                    return (Vector4)this.Value == (Vector4)oValue;
                case EffectPropertyType.Image:
                    return (Image)this.Value == (Image)oValue;
                case EffectPropertyType.Color:
                    return (ColorF)this.Value == (ColorF)oValue;
                case EffectPropertyType.ImageArray:
                    SharedResource[] resources1 = ((SharedResourceArray)oValue).Resources;
                    SharedResource[] resources2 = ((SharedResourceArray)this.Value).Resources;
                    if (resources1.Length != resources1.Length)
                        return false;
                    for (int index = 0; index < resources2.Length; ++index)
                    {
                        if (resources2[index] != resources1[index])
                            return false;
                    }
                    return true;
                case EffectPropertyType.Video:
                    return (VideoStream)this.Value == (VideoStream)oValue;
                default:
                    return false;
            }
        }

        internal EffectProperty Clone() => new EffectProperty(this);

        private void ModifyUsage(bool fLock)
        {
            if (this.m_oValue == null || !(this.m_oValue is SharedResource oValue))
                return;
            if (fLock)
            {
                oValue.RegisterUsage(this);
                oValue.AddActiveUser(this);
            }
            else
            {
                oValue.RemoveActiveUser(this);
                oValue.UnregisterUsage(this);
            }
        }
    }
}
