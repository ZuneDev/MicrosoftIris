// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IEffect
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public interface IEffect : IAnimatable, ISharedRenderObject
    {
        void SetProperty(string stPropertyName, int nValue);

        void SetProperty(string stPropertyName, float flValue);

        void SetProperty(string stPropertyName, Vector2 vValue);

        void SetProperty(string stPropertyName, Vector3 vValue);

        void SetProperty(string stPropertyName, Vector4 vValue);

        void SetProperty(string stPropertyName, ColorF colorValue);

        void SetProperty(string stPropertyName, IImage imgValue);

        void SetProperty(string stPropertyName, IImage[] imgValue);

        void SetProperty(string stPropertyName, IVideoStream streamValue);

        string Name { get; }

        IEffectTemplate Template { get; }
    }
}
