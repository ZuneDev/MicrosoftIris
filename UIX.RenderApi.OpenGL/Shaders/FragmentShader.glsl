#version 330 core
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
}