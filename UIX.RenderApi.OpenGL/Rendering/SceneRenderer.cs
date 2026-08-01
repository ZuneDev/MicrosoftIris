using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Iris.Render.OpenGL.Scene;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Microsoft.Iris.Render.OpenGL.Rendering
{
    /// <summary>
    /// Record-then-execute renderer for the visual tree: <see cref="BeginFrame"/>/
    /// <see cref="DrawColoredQuad"/>/<see cref="DrawTexturedQuad"/> only record a quad's
    /// parameters (called while GLRenderEngine.DrawFrame holds GLRenderSession.SyncRoot,
    /// walking the live scene graph); <see cref="Flush"/> issues the actual GL calls
    /// (called after that lock is released). This split exists specifically so the real
    /// cost of a frame -- GL submission, and above all GLImage.EnsureUploaded's
    /// synchronous first-time image decode via ImageCacheItem/ImageLoader, genuinely slow
    /// CPU work -- never happens while every other app-thread scene mutation is blocked
    /// waiting on the same lock. See logs/UIX.RenderApi.OpenGL/Implementation.md.
    /// </summary>
    internal sealed unsafe class SceneRenderer : IDisposable
    {
        private readonly GL m_gl;
        private readonly uint m_program;
        private readonly uint m_vao;
        private readonly uint m_vbo;

        private readonly int m_locModel;
        private readonly int m_locProj;
        private readonly int m_locTexSize;
        private readonly int m_locSize;
        private readonly int m_locFlags;
        private readonly int m_locColor;
        private readonly int m_locAlpha;
        private readonly int m_locNineGrid;
        private readonly int m_locTex;

        private Matrix4X4<float> m_projection = Matrix4X4<float>.Identity;

        private readonly struct DrawCommand
        {
            public readonly Matrix4X4<float> Model;
            public readonly float Width;
            public readonly float Height;
            public readonly float Alpha;
            public readonly GLImage? Image;    // non-null => textured quad
            public readonly Inset? NineSlice;  // only meaningful when Image != null
            public readonly ColorF Color;      // only meaningful when Image == null

            public DrawCommand(Matrix4X4<float> model, float width, float height, float alpha, GLImage image, Inset? nineSlice)
            {
                Model = model; Width = width; Height = height; Alpha = alpha;
                Image = image; NineSlice = nineSlice; Color = default;
            }

            public DrawCommand(Matrix4X4<float> model, float width, float height, float alpha, ColorF color)
            {
                Model = model; Width = width; Height = height; Alpha = alpha;
                Image = null; NineSlice = null; Color = color;
            }
        }

        private readonly List<DrawCommand> m_commands = new();
        private int m_pendingWidth;
        private int m_pendingHeight;
        private ColorF m_pendingClear;

        public SceneRenderer(GL gl)
        {
            m_gl = gl;

            m_program = BuildProgram(gl);
            m_locModel = gl.GetUniformLocation(m_program, "uModel");
            m_locProj = gl.GetUniformLocation(m_program, "uProj");
            m_locTexSize = gl.GetUniformLocation(m_program, "uTexSize");
            m_locSize = gl.GetUniformLocation(m_program, "uSize");
            m_locFlags = gl.GetUniformLocation(m_program, "uFlags");
            m_locColor = gl.GetUniformLocation(m_program, "uColor");
            m_locAlpha = gl.GetUniformLocation(m_program, "uAlpha");
            m_locNineGrid = gl.GetUniformLocation(m_program, "uNineGrid");
            m_locTex = gl.GetUniformLocation(m_program, "uTex");

            // Unit quad: interleaved position (xy) + texcoord (uv). Texcoords are
            // y-flipped so BGRA image rows (top-down) map upright in our y-down space.
            float[] verts =
            [
                0f, 0f, 0f, 0f,
                1f, 0f, 1f, 0f,
                1f, 1f, 1f, 1f,
                0f, 0f, 0f, 0f,
                1f, 1f, 1f, 1f,
                0f, 1f, 0f, 1f
            ];

            m_vao = gl.GenVertexArray();
            gl.BindVertexArray(m_vao);
            m_vbo = gl.GenBuffer();
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, m_vbo);
            fixed (float* v = verts)
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(verts.Length * sizeof(float)), v, BufferUsageARB.StaticDraw);
            }
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)0);
            gl.EnableVertexAttribArray(1);
            gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)(2 * sizeof(float)));
            gl.BindVertexArray(0);
        }

        /// <summary>Record-only: latches frame size/clear color and resets the command list. No GL calls.</summary>
        public void BeginFrame(int widthPixels, int heightPixels, ColorF clear)
        {
            m_pendingWidth = widthPixels;
            m_pendingHeight = heightPixels;
            m_pendingClear = clear;
            m_commands.Clear();
        }

        /// <summary>Record-only: appends a command. No GL calls.</summary>
        public void DrawColoredQuad(Matrix4X4<float> model, float width, float height, ColorF color, float alpha)
            => m_commands.Add(new DrawCommand(model, width, height, alpha, color));

        /// <summary>Record-only: appends a command. No GL calls, no image upload/decode yet.</summary>
        public void DrawTexturedQuad(Matrix4X4<float> model, float width, float height, GLImage image, float alpha, Inset? nineSlice)
            => m_commands.Add(new DrawCommand(model, width, height, alpha, image, nineSlice));

        /// <summary>
        /// Executes everything BeginFrame/DrawColoredQuad/DrawTexturedQuad recorded since
        /// the last call: the actual GL viewport/clear, then each quad in order (including
        /// GLImage.EnsureUploaded's texture upload/first-decode). Must run on the render
        /// thread (GL-context-affine) but deliberately NOT under GLRenderSession.SyncRoot --
        /// see the class doc comment.
        /// </summary>
        public void Flush()
        {
            m_projection = Matrix4X4.CreateOrthographicOffCenter(0f, m_pendingWidth, m_pendingHeight, 0f, -1f, 1f);

            m_gl.Viewport(0, 0, (uint)Math.Max(1, m_pendingWidth), (uint)Math.Max(1, m_pendingHeight));
            m_gl.Disable(EnableCap.DepthTest);
            m_gl.Enable(EnableCap.Blend);
            m_gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            m_gl.ClearColor(m_pendingClear.R, m_pendingClear.G, m_pendingClear.B, m_pendingClear.A);
            m_gl.Clear(ClearBufferMask.ColorBufferBit);

            m_gl.UseProgram(m_program);
            UploadMatrix(m_locProj, m_projection);

            foreach (var cmd in m_commands)
            {
                if (cmd.Image != null)
                    DrawTexturedQuadImmediate(cmd.Model, cmd.Width, cmd.Height, cmd.Image, cmd.Alpha, cmd.NineSlice);
                else
                    DrawColoredQuadImmediate(cmd.Model, cmd.Width, cmd.Height, cmd.Color, cmd.Alpha);
            }
            m_commands.Clear();
        }

        private void DrawColoredQuadImmediate(Matrix4X4<float> model, float width, float height, ColorF color, float alpha)
        {
            m_gl.UseProgram(m_program);
            UploadMatrix(m_locModel, model);
            m_gl.Uniform2(m_locSize, width, height);
            m_gl.Uniform1(m_locFlags, (int)FragmentFlags.Default);
            m_gl.Uniform4(m_locColor, color.R, color.G, color.B, color.A);
            m_gl.Uniform1(m_locAlpha, alpha);
            DrawQuad();
        }

        private void DrawTexturedQuadImmediate(Matrix4X4<float> model, float width, float height, GLImage image, float alpha, Inset? nineSlice)
        {
            // Real CPU decode work can happen here on first use (see GLImage.EnsureUploaded)
            // -- deliberately unlocked with respect to GLRenderSession.SyncRoot at this
            // point; only image's own private lock is involved.
            image.EnsureUploaded(m_gl);
            if (image.TextureId == 0)
                return;

            m_gl.UseProgram(m_program);

            var flags = FragmentFlags.UseTexture;
            if (nineSlice.HasValue)
            {
                flags |= FragmentFlags.UseNineSlice;
                m_gl.Uniform4(m_locNineGrid,
                    (float)nineSlice.Value.Left, (float)nineSlice.Value.Top,
                    (float)nineSlice.Value.Right, (float)nineSlice.Value.Bottom);
            }

            UploadMatrix(m_locModel, model);
            m_gl.Uniform2(m_locSize, width, height);
            m_gl.Uniform1(m_locFlags, (int)flags);
            m_gl.Uniform1(m_locAlpha, alpha);
            m_gl.ActiveTexture(TextureUnit.Texture0);
            m_gl.BindTexture(TextureTarget.Texture2D, image.TextureId);
            m_gl.Uniform1(m_locTex, 0);
            m_gl.Uniform2(m_locTexSize, (float)image.Size.Width, (float)image.Size.Height);
            DrawQuad();
        }

        private void DrawQuad()
        {
            m_gl.BindVertexArray(m_vao);
            m_gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
            m_gl.BindVertexArray(0);
        }

        // Silk.NET.Maths stores matrices row-major; uploading with transpose=false makes
        // GLSL read the transpose, so the shader uses column-vector order uProj*uModel*v.
        private void UploadMatrix(int location, Matrix4X4<float> m)
        {
            float[] a =
            {
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44,
            };
            fixed (float* p = a)
                m_gl.UniformMatrix4(location, 1, false, p);
        }

        private static uint BuildProgram(GL gl)
        {
            List<(ShaderType, string)> shaders =
            [
                (ShaderType.VertexShader, "VertexShader"),
                (ShaderType.FragmentShader, "FragmentShader"),
            ];
            
            var program = gl.CreateProgram();
            
            var shaderHandles = new uint[shaders.Count];
            for (var s = 0; s < shaders.Count; s++)
            {
                var (type, name) = shaders[s];
                var shader = CompileShader(gl, type, ReadShader(name));
                gl.AttachShader(program, shader);
                shaderHandles[s] = shader;
            }

            gl.LinkProgram(program);
            gl.GetProgram(program, ProgramPropertyARB.LinkStatus, out var linked);
            if (linked == 0)
                throw new InvalidOperationException("Shader link failed: " + gl.GetProgramInfoLog(program));
            
            for (var s = 0; s < shaders.Count; s++)
            {
                gl.DetachShader(program, shaderHandles[s]);
                gl.DeleteShader(shaderHandles[s]);
            }
            
            return program;
        }

        private static string ReadShader(string shaderName)
        {
            var o = Shaders.Shaders.ResourceManager.GetObject(shaderName);
            return o is byte[] data
                ? System.Text.Encoding.UTF8.GetString(data)
                : throw new FileNotFoundException($"Failed to load {shaderName}");
        }

        private static uint CompileShader(GL gl, ShaderType type, string source)
        {
            var shader = gl.CreateShader(type);
            gl.ShaderSource(shader, source);
            gl.CompileShader(shader);
            gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);
            return status != 0
                ? shader
                : throw new InvalidOperationException($"{type} compile failed: " + gl.GetShaderInfoLog(shader));
        }

        public void Dispose()
        {
            m_gl.DeleteBuffer(m_vbo);
            m_gl.DeleteVertexArray(m_vao);
            m_gl.DeleteProgram(m_program);
        }
    }
}
