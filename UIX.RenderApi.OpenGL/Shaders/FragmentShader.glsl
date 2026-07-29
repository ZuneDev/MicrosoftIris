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
            float left = uNineGrid.x;
            float top = uNineGrid.y;
            float right = uNineGrid.z;
            float bottom = uNineGrid.w;
            
            // TODO: Can this be optimized by representing as a matrix?
            vec2 insetX = vec2(left, right);
            vec2 insetY = vec2(top, bottom);
            
            vec2 texInsetX = insetX / uTexSize.x;
            vec2 texInsetY = insetY / uTexSize.y;

            vec2 recInsetX = insetX / uSize.x;
            vec2 recInsetY = insetY / uSize.y;
            
            vec2 newUV = vTex;
            
            if (vTex.x < recInsetX.s && vTex.y < recInsetY.s)
            {
                newUV = vec2(
                    map(vTex.x, 0, recInsetX.s, 0, texInsetX.s),
                    map(vTex.y, 0, recInsetY.s, 0, texInsetY.s));
            }
            
            //fragColor = vec4(newUV, 0, 1);
            //fragColor = vec4(newUV, 1.0, uAlpha);
            fragColor = texture(uTex, newUV);
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