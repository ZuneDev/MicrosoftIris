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

float processAxis(float component, vec2 recInset, vec2 texInset)
{
    if (component < recInset.s)
    {
        return map(component, 0, recInset.s, 0, texInset.s);
    }
    else if (component > (1 - recInset.t))
    {
        return map(component, 1 - recInset.t, 1.0, 1 - texInset.t, 1.0);
    }
    else
    {
        return map(component, recInset.s, 1 - recInset.t, texInset.s, 1 - texInset.t);
    }
}

void main()
{
    bool useTexture = (uFlags & FLAG_USETEXTURE) != 0;
    if (useTexture)
    {
        bool useNineSlice = (uFlags & FLAG_USENINESLICE) != 0;

        if (useNineSlice)
        {
            // TODO: Can this be optimized by representing as a matrix?
            vec2 insetX = uNineGrid.xy; /* left, right */
            vec2 insetY = uNineGrid.zw; /* top, bottom */
            
            vec2 texInsetX = insetX / uTexSize.x;
            vec2 texInsetY = insetY / uTexSize.y;

            vec2 recInsetX = insetX / uSize.x;
            vec2 recInsetY = insetY / uSize.y;
            
            vec2 newUV = vec2(
                processAxis(vTex.x, recInsetX, texInsetX),
                processAxis(vTex.y, recInsetY, texInsetY));
            
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