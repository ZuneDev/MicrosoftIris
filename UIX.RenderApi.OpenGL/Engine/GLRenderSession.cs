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
        // Held by the render thread for the duration of each frame (pulse + draw)
        // and by every app-side mutator reachable from that frame, so the two
        // threads never observe or corrupt a half-written scene/animation graph.
        // Deliberately one coarse lock, not per-object: GPU submission time dwarfs
        // lock hold time, and rendering code freely walks from one object into
        // another's state (e.g. a sprite reading its parent container's size), which
        // would make per-object locking deadlock-prone for no real benefit.
        public readonly object SyncRoot = new();

        public GLRenderSession()
        {
            AnimationSystem = new GLAnimationSystem(SyncRoot);
            InputSystem = new GLInputSystem();
        }

        public IAnimationSystem AnimationSystem { get; }
        public IInputSystem InputSystem { get; }
        public IGraphicsDevice GraphicsDevice { get; internal set; } = null!;
        public ISoundDevice SoundDevice { get; internal set; } = null!;

        internal GLInputSystem RawInput => (GLInputSystem)InputSystem;

        public IEffectTemplate CreateEffectTemplate(object objUser, string stName)
        {
            var t = new GLEffectTemplate(SyncRoot, stName);
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
            // Shares SyncRoot with the rest of the scene graph: its stops/orientation/
            // offset are read from the render thread every frame now (see GLGradient's
            // class doc comment), not just at RegisterUsage/UnregisterUsage time.
            var g = new GLGradient(SyncRoot);
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
            // Deliberately not SyncRoot -- see GLImage's class doc comment: it uses its
            // own private lock so a slow first-time image decode doesn't block the rest
            // of the scene graph.
            var img = new GLImage(identifier, handler);
            img.RegisterUsage(objUser);
            return img;
        }

        public void Dispose() { }
    }
}
