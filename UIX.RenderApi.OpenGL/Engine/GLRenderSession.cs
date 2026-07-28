using Microsoft.Iris.Render.OpenGL.Animation;
using Microsoft.Iris.Render.OpenGL.Scene;

namespace Microsoft.Iris.Render.OpenGL.Engine
{
    /// <summary>
    /// Thin factory that produces the render objects (visuals, sprites, images, effects,
    /// …) and exposes the animation/input/graphics/sound subsystems. All objects live in
    /// the same process and share the engine's single GL context.
    /// </summary>
    public sealed class GLRenderSession : IRenderSession
    {
        public GLRenderSession()
        {
            AnimationSystem = new GLAnimationSystem();
            InputSystem = new GLInputSystem();
        }

        public IAnimationSystem AnimationSystem { get; }
        public IInputSystem InputSystem { get; }
        public IGraphicsDevice GraphicsDevice { get; internal set; } = null!;
        public ISoundDevice SoundDevice { get; internal set; } = null!;

        internal GLInputSystem RawInput => (GLInputSystem)InputSystem;

        public IEffectTemplate CreateEffectTemplate(object objUser, string stName)
        {
            var t = new GLEffectTemplate(stName);
            t.RegisterUsage(objUser);
            return t;
        }

        public IVideoStream CreateVideoStream(object objUser)
        {
            var v = new GLVideoStream();
            v.RegisterUsage(objUser);
            return v;
        }

        public IVisualContainer CreateVisualContainer(object objUser, object objOwnerData)
        {
            var c = new GLVisualContainer(this, objOwnerData, isRoot: false);
            c.RegisterUsage(objUser);
            return c;
        }

        public ICamera CreateCamera(object objUser)
        {
            var c = new GLCamera();
            c.RegisterUsage(objUser);
            return c;
        }

        public IGradient CreateGradient(object objUser)
        {
            var g = new GLGradient();
            g.RegisterUsage(objUser);
            return g;
        }

        public ISprite CreateSprite(object objUser, object objOwnerData)
        {
            var s = new GLSprite(this, objOwnerData);
            s.RegisterUsage(objUser);
            return s;
        }

        public IImage CreateImage(object objUser, string identifier, ContentNotifyHandler handler)
        {
            var img = new GLImage(identifier, handler);
            img.RegisterUsage(objUser);
            return img;
        }

        public void Dispose() { }
    }
}
