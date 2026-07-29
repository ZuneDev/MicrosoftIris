#version 330 core

const int FLAG_USETEXTURE = 1 << 0;
const int FLAG_USENINESLICE = 1 << 1;

in vec2 vTex;
out vec4 fragColor;
uniform vec2 uTexSize;
uniform sampler2D uTex;
uniform int uFlags;
uniform vec4 uColor;
uniform float uAlpha;
uniform vec4 uNineGrid;
uniform vec2 uSize;

float map(float value, float originalMin, float originalMax, float newMin, float newMax) {
    return (value - originalMin) / (originalMax - originalMin) * (newMax - newMin) + newMin;
}

void main()
{
    bool useTexture = (uFlags & FLAG_USETEXTURE) != 0;
    if (useTexture)
    {
        bool useNineSlice = (uFlags & FLAG_USENINESLICE) != 0;

        if (useNineSlice)
        {
            float l = uNineGrid.x;
            float t = uNineGrid.y;
            float r = uNineGrid.z;
            float b = uNineGrid.w;
            
            // TODO: Can this be optimized by representing as a matrix?
            vec2 insetX = vec2(l, r);
            vec2 insetY = vec2(t, b);
            
            vec2 texInsetX = insetX / uTexSize.x;
            vec2 texInsetY = insetY / uTexSize.y;

            vec2 rectInsetX = insetX / uSize.x;
            vec2 rectInsetY = insetY / uSize.y;
            
            vec2 newUV = vTex;
            
            vec4 color1 = vec4(
                0.0,
                (vTex.x < rectInsetX.s) ? 0.5 : 0.0,
                (vTex.x > 1 - rectInsetX.t) ? 0.5 : 0.0,
                1.0
            );
            vec4 color2 = vec4(
                (vTex.y > 1 - rectInsetY.t) ? 0.5 : 0.0,
                (vTex.y < rectInsetY.s) ? 0.5 : 0.0,
                0.0,
                1.0
            );
            
            fragColor = color1 + color2;
            //fragColor = vec4(newUV, 0, 1);
            //fragColor = vec4(vTex, 1.0, uAlpha);
            //fragColor = texture(uTex, newUV);
        }
        else
        {
            vec4 t = texture(uTex, vTex);
            fragColor = vec4(t.rgb, t.a * uAlpha);
        }
    }
    else
    {
        fragColor = vec4(uColor.rgb, uColor.a * uAlpha);
    }
}