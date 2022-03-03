// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Render.IRenderSession
// Assembly: UIX.RenderApi, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: D47658B8-A8EA-43D6-8837-ECE823BFFFC1
// Assembly location: C:\Program Files\Zune\UIX.RenderApi.dll

using System;

namespace Microsoft.Iris.Render
{
    public interface IRenderSession : IRenderObject, IDisposable
    {
        IAnimationSystem AnimationSystem { get; }

        IInputSystem InputSystem { get; }

        IGraphicsDevice GraphicsDevice { get; }

        ISoundDevice SoundDevice { get; }

        IEffectTemplate CreateEffectTemplate(object objUser, string stName);

        IVideoStream CreateVideoStream(object objUser);

        IVisualContainer CreateVisualContainer(object objUser, object objOwnerData);

        ICamera CreateCamera(object objUser);

        IGradient CreateGradient(object objUser);

        ISprite CreateSprite(object objUser, object objOwnerData);

        IImage CreateImage(object objUser, string identifier, ContentNotifyHandler handler);
    }
}
