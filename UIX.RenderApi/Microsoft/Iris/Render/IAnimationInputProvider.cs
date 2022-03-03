// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IAnimationInputProvider
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

namespace Microsoft.Iris.Render
{
    public interface IAnimationInputProvider : IAnimatable, ISharedRenderObject
    {
        void PublishFloat(string propertyName, float value);

        void PublishVector2(string propertyName, Vector2 value);

        void PublishVector3(string propertyName, Vector3 value);

        void PublishVector4(string propertyName, Vector4 value);

        void PublishQuaternion(string propertyName, Quaternion value);

        void RevokeFloat(string propertyName);

        void RevokeVector2(string propertyName);

        void RevokeVector3(string propertyName);

        void RevokeVector4(string propertyName);

        void RevokeQuaternion(string propertyName);
    }
}
