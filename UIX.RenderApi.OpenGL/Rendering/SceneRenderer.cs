using System;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Microsoft.Iris.Render.OpenGL
{
    /// <summary>
    /// Immediate-mode style GL renderer for the visual tree. Draws each sprite as a quad
    /// in an orthographic, top-left-origin, pixel-space projection with alpha blending.
    /// </summary>
    internal sealed unsafe class SceneRenderer : IDisposable
    {
        private const string VertexSource = @"#version 330 core
layout(location = 0) in vec2 aPos;
layout(location = 1) in vec2 aTex;
uniform mat4 uModel;
uniform mat4 uProj;
uniform vec2 uSize;
out vec2 vTex;
void main()
{
    vTex = aTex;
    gl_Position = uProj * uModel * vec4(aPos * uSize, 0.0, 1.0);
}";

        private const string FragmentSource = @"#version 330 core
in vec2 vTex;
out vec4 fragColor;
uniform sampler2D uTex;
uniform int uUseTexture;
uniform vec4 uColor;
uniform float uAlpha;
void main()
{
    if (uUseTexture == 1)
    {
        vec4 t = texture(uTex, vTex);
        fragColor = vec4(t.rgb, t.a * uAlpha);
    }
    else
    {
        fragColor = vec4(uColor.rgb, uColor.a * uAlpha);
    }
}";

        private readonly GL m_gl;
        private readonly uint m_program;
        private readonly uint m_vao;
        private readonly uint m_vbo;

        private readonly int m_locModel;
        private readonly int m_locProj;
        private readonly int m_locSize;
        private readonly int m_locUseTexture;
        private readonly int m_locColor;
        private readonly int m_locAlpha;
        private readonly int m_locTex;

        private Matrix4X4<float> m_projection = Matrix4X4<float>.Identity;

        public SceneRenderer(GL gl)
        {
            m_gl = gl;

            m_program = BuildProgram(gl);
            m_locModel = gl.GetUniformLocation(m_program, "uModel");
            m_locProj = gl.GetUniformLocation(m_program, "uProj");
            m_locSize = gl.GetUniformLocation(m_program, "uSize");
            m_locUseTexture = gl.GetUniformLocation(m_program, "uUseTexture");
            m_locColor = gl.GetUniformLocation(m_program, "uColor");
            m_locAlpha = gl.GetUniformLocation(m_program, "uAlpha");
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
            m_gl.Uniform1(m_locUseTexture, 0);
            m_gl.Uniform4(m_locColor, color.R, color.G, color.B, color.A);
            m_gl.Uniform1(m_locAlpha, alpha);
            DrawQuad();
        }

        public void DrawTexturedQuad(Matrix4X4<float> model, float width, float height, GLImage image, float alpha)
        {
            image.EnsureUploaded(m_gl);
            if (image.TextureId == 0)
                return;

            m_gl.UseProgram(m_program);
            UploadMatrix(m_locModel, model);
            m_gl.Uniform2(m_locSize, width, height);
            m_gl.Uniform1(m_locUseTexture, 1);
            m_gl.Uniform1(m_locAlpha, alpha);
            m_gl.ActiveTexture(TextureUnit.Texture0);
            m_gl.BindTexture(TextureTarget.Texture2D, image.TextureId);
            m_gl.Uniform1(m_locTex, 0);
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
            uint vs = CompileShader(gl, ShaderType.VertexShader, VertexSource);
            uint fs = CompileShader(gl, ShaderType.FragmentShader, FragmentSource);
            uint program = gl.CreateProgram();
            gl.AttachShader(program, vs);
            gl.AttachShader(program, fs);
            gl.LinkProgram(program);
            gl.GetProgram(program, ProgramPropertyARB.LinkStatus, out int linked);
            if (linked == 0)
                throw new InvalidOperationException("Shader link failed: " + gl.GetProgramInfoLog(program));
            gl.DetachShader(program, vs);
            gl.DetachShader(program, fs);
            gl.DeleteShader(vs);
            gl.DeleteShader(fs);
            return program;
        }

        private static uint CompileShader(GL gl, ShaderType type, string source)
        {
            uint shader = gl.CreateShader(type);
            gl.ShaderSource(shader, source);
            gl.CompileShader(shader);
            gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
            if (status == 0)
                throw new InvalidOperationException($"{type} compile failed: " + gl.GetShaderInfoLog(shader));
            return shader;
        }

        public void Dispose()
        {
            m_gl.DeleteBuffer(m_vbo);
            m_gl.DeleteVertexArray(m_vao);
            m_gl.DeleteProgram(m_program);
        }
    }
}
