// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.EffectLayer
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Render.Common;
using Microsoft.Iris.Render.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Iris.Render
{
    public class EffectLayer : EffectInput
    {
        internal EffectInput m_input;
        internal IList m_listOperations;

        public EffectLayer(EffectInput inputElement, List<EffectOperation> listOperations)
          : this()
        {
            Debug2.Validate(inputElement != null, typeof(ArgumentNullException), nameof(inputElement));
            Debug2.Validate(listOperations != null, typeof(ArgumentNullException), nameof(listOperations));
            this.m_input = inputElement;
            this.m_listOperations = listOperations;
        }

        public EffectLayer(EffectInput inputElement, EffectOperation efoOperation)
          : this()
        {
            Debug2.Validate(inputElement != null, typeof(ArgumentNullException), nameof(inputElement));
            Debug2.Validate(efoOperation != null, typeof(ArgumentNullException), nameof(efoOperation));
            this.m_input = inputElement;
            this.m_listOperations = new List<EffectOperation>(1);
            this.m_listOperations.Add(efoOperation);
        }

        public EffectLayer() => this.m_typeInput = EffectInputType.Layer;

        public EffectInput Input
        {
            get => this.m_input;
            set => this.m_input = value;
        }

        public IList Operations
        {
            get => this.m_listOperations;
            set => this.m_listOperations = value;
        }

        internal override bool Process(Map<string, EffectProperty> dictProperties)
        {
            if (!this.m_input.Process(dictProperties))
                return false;
            foreach (EffectElement listOperation in m_listOperations)
            {
                if (!listOperation.Process(dictProperties))
                    return false;
            }
            return true;
        }

        internal override int PreProcessProperties(
          Map<string, EffectProperty> dictionary,
          ref byte nNextUniqueID)
        {
            int num = base.PreProcessProperties(dictionary, ref nNextUniqueID) + this.m_input.PreProcessProperties(dictionary, ref nNextUniqueID);
            foreach (EffectOperation listOperation in m_listOperations)
                num += listOperation.PreProcessProperties(dictionary, ref nNextUniqueID);
            return num;
        }

        internal override void AddCacheKey(ByteBuilder cacheKey)
        {
            base.AddCacheKey(cacheKey);
            this.m_input.AddCacheKey(cacheKey);
            foreach (EffectElement listOperation in m_listOperations)
                listOperation.AddCacheKey(cacheKey);
        }
    }
}
