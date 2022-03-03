// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IVisualContainer
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using Microsoft.Iris.Input;

namespace Microsoft.Iris.Render
{
    public interface IVisualContainer : IVisual, IRawInputSite, IAnimatable, ISharedRenderObject
    {
        bool IsRoot { get; }

        Vector3 Position { get; set; }

        Vector2 Size { get; set; }

        bool Visible { get; set; }

        Vector3 Scale { get; set; }

        AxisAngle Rotation { get; set; }

        float Alpha { get; set; }

        Vector3 CenterPoint { get; set; }

        uint Layer { get; set; }

        int ChildCount { get; }

        ICamera Camera { get; set; }

        void SetPosition(Vector3 value, bool force);

        void SetSize(Vector2 value, bool force);

        void SetScale(Vector3 value, bool force);

        void SetRotation(AxisAngle value, bool force);

        void SetAlpha(float value, bool force);

        void AddGradient(IGradient gradient);

        void RemoveAllGradients();

        void AddChild(IVisual vChild, IVisual vSibling, VisualOrder nOrder);

        void RemoveChild(IVisual vChild);

        void RemoveAllChildren();
    }
}
