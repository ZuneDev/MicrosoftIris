// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.ISprite
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Input;

namespace Microsoft.Iris.Render
{
    public interface ISprite : IVisual, IRawInputSite, IAnimatable, ISharedRenderObject
    {
        Vector3 Position { get; set; }

        Vector2 Size { get; set; }

        bool Visible { get; set; }

        Vector3 Scale { get; set; }

        AxisAngle Rotation { get; set; }

        float Alpha { get; set; }

        Vector3 CenterPoint { get; set; }

        uint Layer { get; set; }

        IEffect Effect { get; set; }

        bool RelativeSize { get; set; }

        void AddGradient(IGradient gradient);

        void RemoveAllGradients();

        void SetCoordMap(int idxLayer, CoordMap coordMap);

        void SetNineGrid(int left, int top, int right, int bottom);
    }
}
