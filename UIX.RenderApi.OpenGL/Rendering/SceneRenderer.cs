using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Iris.Render.OpenGL.Scene;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Microsoft.Iris.Render.OpenGL.Rendering
{
    /// <summary>
    /// Immediate-mode style GL renderer for the visual tree. Draws each sprite as a quad
    /// in an orthographic, top-left-origin, pixel-space projection with alpha blending.
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
            {
                0f, 0f, 0f, 0f,
                1f, 0f, 1f, 0f,
                1f, 1f, 1f, 1f,
                0f, 0f, 0f, 0f,
                1f, 1f, 1f, 1f,
                0f, 1f, 0f, 1f,
            };

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

        public void BeginFrame(int widthPixels, int heightPixels, ColorF clear)
        {
            m_projection = Matrix4X4.CreateOrthographicOffCenter(0f, widthPixels, heightPixels, 0f, -1f, 1f);

            m_gl.Viewport(0, 0, (uint)Math.Max(1, widthPixels), (uint)Math.Max(1, heightPixels));
            m_gl.Disable(EnableCap.DepthTest);
            m_gl.Enable(EnableCap.Blend);
            m_gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            m_gl.ClearColor(clear.R, clear.G, clear.B, clear.A);
            m_gl.Clear((uint)ClearBufferMask.ColorBufferBit);

            m_gl.UseProgram(m_program);
            UploadMatrix(m_locProj, m_projection);
        }

        public void DrawColoredQuad(Matrix4X4<float> model, float width, float height, ColorF color, float alpha)
        {
            m_gl.UseProgram(m_program);
            UploadMatrix(m_locModel, model);
            m_gl.Uniform2(m_locSize, width, height);
            m_gl.Uniform1(m_locFlags, (int)FragmentFlags.Default);
            m_gl.Uniform4(m_locColor, color.R, color.G, color.B, color.A);
            m_gl.Uniform1(m_locAlpha, alpha);
            DrawQuad();
        }

        public void DrawTexturedQuad(Matrix4X4<float> model, float width, float height, GLImage image, float alpha, Inset? nineSlice)
        {
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
