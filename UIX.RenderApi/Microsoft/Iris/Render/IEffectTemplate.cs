// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IEffectTemplate
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public interface IEffectTemplate : ISharedRenderObject
    {
        void AddEffectProperty(string stPath);

        bool Build(EffectInput input);

        IEffect CreateInstance(object objUser);

        bool IsBuilt { get; }

        string Name { get; }
    }
}
