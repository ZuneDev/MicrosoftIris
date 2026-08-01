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
        private readonly int m_locGradData;
        private readonly int m_locGradBase;
        private readonly int m_locGradCount;

        // Per-frame gradient stop/transform data lives in a GL_TEXTURE_BUFFER (core since
        // GL 3.1, samplerBuffer/texelFetch in GLSL 330 core -- no version bump needed)
        // rather than a fixed-size uniform array, since a draw call's gradient count
        // (own gradients plus every ancestor container's, per EdgeFade/Text's "applies to
        // the whole subtree" semantics -- see ResolvedGradient's doc comment) has no fixed
        // upper bound worth guessing at. Each texel is one RGBA32F (4 floats); see
        // BuildGradientBuffer for the exact per-gradient layout, mirrored in
        // FragmentShader.glsl's evaluateGradients.
        private readonly uint m_gradBuffer;
        private readonly uint m_gradTexture;
        private const TextureUnit GradTextureUnit = TextureUnit.Texture1;
        private const int GradTextureUnitIndex = 1;

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
            public readonly IReadOnlyList<ResolvedGradient> Gradients;

            public DrawCommand(Matrix4X4<float> model, float width, float height, float alpha, GLImage image, Inset? nineSlice, IReadOnlyList<ResolvedGradient> gradients)
            {
                Model = model; Width = width; Height = height; Alpha = alpha;
                Image = image; NineSlice = nineSlice; Color = default; Gradients = gradients;
            }

            public DrawCommand(Matrix4X4<float> model, float width, float height, float alpha, ColorF color, IReadOnlyList<ResolvedGradient> gradients)
            {
                Model = model; Width = width; Height = height; Alpha = alpha;
                Image = null; NineSlice = null; Color = color; Gradients = gradients;
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
            m_locGradData = gl.GetUniformLocation(m_program, "uGradData");
            m_locGradBase = gl.GetUniformLocation(m_program, "uGradBase");
            m_locGradCount = gl.GetUniformLocation(m_program, "uGradCount");

            m_gradBuffer = gl.GenBuffer();
            m_gradTexture = gl.GenTexture();

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
        public void DrawColoredQuad(Matrix4X4<float> model, float width, float height, ColorF color, float alpha, IReadOnlyList<ResolvedGradient> gradients)
            => m_commands.Add(new DrawCommand(model, width, height, alpha, color, gradients));

        /// <summary>Record-only: appends a command. No GL calls, no image upload/decode yet.</summary>
        public void DrawTexturedQuad(Matrix4X4<float> model, float width, float height, GLImage image, float alpha, Inset? nineSlice, IReadOnlyList<ResolvedGradient> gradients)
            => m_commands.Add(new DrawCommand(model, width, height, alpha, image, nineSlice, gradients));

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

            (int[] gradBase, int[] gradCount) = BuildGradientBuffer();

            // Bound once for the whole frame -- unlike uTex/Texture0 (rebound per textured
            // quad to whichever GLImage that quad uses), this buffer texture and its
            // sampler uniform don't change between quads.
            m_gl.ActiveTexture(GradTextureUnit);
            m_gl.BindTexture(TextureTarget.TextureBuffer, m_gradTexture);
            m_gl.Uniform1(m_locGradData, GradTextureUnitIndex);

            for (int i = 0; i < m_commands.Count; i++)
            {
                DrawCommand cmd = m_commands[i];
                if (cmd.Image != null)
                    DrawTexturedQuadImmediate(cmd.Model, cmd.Width, cmd.Height, cmd.Image, cmd.Alpha, cmd.NineSlice, gradBase[i], gradCount[i]);
                else
                    DrawColoredQuadImmediate(cmd.Model, cmd.Width, cmd.Height, cmd.Color, cmd.Alpha, gradBase[i], gradCount[i]);
            }
            m_commands.Clear();
        }

        /// <summary>
        /// Flattens every recorded command's <see cref="ResolvedGradient"/>s into one
        /// RGBA32F texel buffer, uploads it, and returns each command's
        /// (base texel index, gradient count) into that buffer -- what
        /// <see cref="FragmentShader.glsl"/>'s <c>uGradBase</c>/<c>uGradCount</c> need to
        /// walk its own slice. Per gradient, laid out sequentially as:
        /// 4 texels (the 4 rows of <see cref="ResolvedGradient.Transform"/>, same
        /// row-as-column-vec4 convention as <see cref="UploadMatrix"/>), 1 texel
        /// (x = orientation as 0/1, y = stop count, zw unused), then
        /// ceil(stopCount / 2) texels packing two (position, value) stop pairs per texel
        /// (xy = stop 2k, zw = stop 2k+1, zw unused/zero if stopCount is odd).
        /// </summary>
        private (int[] GradBase, int[] GradCount) BuildGradientBuffer()
        {
            var data = new List<float>();
            var gradBase = new int[m_commands.Count];
            var gradCount = new int[m_commands.Count];

            for (int i = 0; i < m_commands.Count; i++)
            {
                IReadOnlyList<ResolvedGradient> gradients = m_commands[i].Gradients;
                gradBase[i] = data.Count / 4;
                gradCount[i] = gradients.Count;

                foreach (ResolvedGradient g in gradients)
                {
                    Matrix4X4<float> m = g.Transform;
                    data.Add(m.M11); data.Add(m.M12); data.Add(m.M13); data.Add(m.M14);
                    data.Add(m.M21); data.Add(m.M22); data.Add(m.M23); data.Add(m.M24);
                    data.Add(m.M31); data.Add(m.M32); data.Add(m.M33); data.Add(m.M34);
                    data.Add(m.M41); data.Add(m.M42); data.Add(m.M43); data.Add(m.M44);

                    int stopCount = g.StopPositions.Length;
                    data.Add(g.Orientation == Orientation.Horizontal ? 0f : 1f);
                    data.Add(stopCount);
                    data.Add(0f); data.Add(0f);

                    for (int s = 0; s < stopCount; s += 2)
                    {
                        data.Add(g.StopPositions[s]);
                        data.Add(g.StopValues[s]);
                        bool hasNext = s + 1 < stopCount;
                        data.Add(hasNext ? g.StopPositions[s + 1] : 0f);
                        data.Add(hasNext ? g.StopValues[s + 1] : 0f);
                    }
                }
            }

            // A zero-length buffer store is legal but keeping at least one dummy texel
            // sidesteps any driver quirks around binding a zero-size buffer to a texture;
            // uGradCount is 0 for every command in this case, so the shader never reads it.
            if (data.Count == 0)
                data.AddRange(new float[4]);

            float[] array = data.ToArray();
            m_gl.BindBuffer(BufferTargetARB.TextureBuffer, m_gradBuffer);
            fixed (float* p = array)
                m_gl.BufferData(BufferTargetARB.TextureBuffer, (nuint)(array.Length * sizeof(float)), p, BufferUsageARB.DynamicDraw);
            // Re-associate after every BufferData call, since some drivers don't reliably
            // pick up a reallocated store on an already-attached buffer texture otherwise.
            m_gl.TexBuffer(TextureTarget.TextureBuffer, SizedInternalFormat.Rgba32f, m_gradBuffer);

            return (gradBase, gradCount);
        }

        private void DrawColoredQuadImmediate(Matrix4X4<float> model, float width, float height, ColorF color, float alpha, int gradBase, int gradCount)
        {
            m_gl.UseProgram(m_program);
            UploadMatrix(m_locModel, model);
            m_gl.Uniform2(m_locSize, width, height);
            m_gl.Uniform1(m_locFlags, (int)FragmentFlags.Default);
            m_gl.Uniform4(m_locColor, color.R, color.G, color.B, color.A);
            m_gl.Uniform1(m_locAlpha, alpha);
            m_gl.Uniform1(m_locGradBase, gradBase);
            m_gl.Uniform1(m_locGradCount, gradCount);
            DrawQuad();
        }

        private void DrawTexturedQuadImmediate(Matrix4X4<float> model, float width, float height, GLImage image, float alpha, Inset? nineSlice, int gradBase, int gradCount)
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
            m_gl.Uniform1(m_locGradBase, gradBase);
            m_gl.Uniform1(m_locGradCount, gradCount);
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
            m_gl.DeleteTexture(m_gradTexture);
            m_gl.DeleteBuffer(m_gradBuffer);
            m_gl.DeleteProgram(m_program);
        }
    }
}
