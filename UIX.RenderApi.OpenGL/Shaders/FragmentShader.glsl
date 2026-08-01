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

// Alpha-ramp gradients (edge/scroll fades via IVisualContainer.AddGradient, text clip
// fades via ISprite.AddGradient -- see GLGradient/ResolvedGradient's doc comments for the
// original Iris semantics). uGradData packs every gradient active for this draw call
// (this sprite's own, plus every ancestor container's) sequentially starting at texel
// uGradBase, uGradCount entries long; layout matches SceneRenderer.BuildGradientBuffer:
// per gradient, 4 texels (transform rows, same row-as-column-vec4 convention as uProj/
// uModel below), 1 texel (x = orientation 0/1, y = stop count), then ceil(stopCount/2)
// texels of two packed (position, value) stop pairs each.
uniform samplerBuffer uGradData;
uniform int uGradBase;
uniform int uGradCount;

float map(float value, float originalMin, float originalMax, float newMin, float newMax) {
    return (value - originalMin) / (originalMax - originalMin) * (newMax - newMin) + newMin;
}

// Piecewise-linear lookup over stopCount stops packed two-per-texel starting at
// stopsBaseTexel, clamping to the nearest stop outside the recorded range.
float sampleGradientStops(int stopsBaseTexel, int stopCount, float axisPos) {
    if (stopCount <= 0)
        return 1.0;

    vec4 first = texelFetch(uGradData, stopsBaseTexel);
    float p0 = first.x;
    float v0 = first.y;
    if (axisPos <= p0)
        return v0;

    for (int i = 1; i < stopCount; i++) {
        vec4 stopTexel = texelFetch(uGradData, stopsBaseTexel + i / 2);
        float p1 = (i % 2 == 0) ? stopTexel.x : stopTexel.z;
        float v1 = (i % 2 == 0) ? stopTexel.y : stopTexel.w;
        if (axisPos <= p1) {
            float t = (p1 - p0) > 1e-5 ? (axisPos - p0) / (p1 - p0) : 0.0;
            return mix(v0, v1, clamp(t, 0.0, 1.0));
        }
        p0 = p1;
        v0 = v1;
    }
    return v0;
}

// Per-pixel product of every active gradient's alpha factor at localPos (this fragment's
// position in the drawing visual's own local pixel space, i.e. vTex * uSize).
float evaluateGradients(vec2 localPos) {
    float factor = 1.0;
    int texelIdx = uGradBase;
    vec4 localPos4 = vec4(localPos, 0.0, 1.0);

    for (int g = 0; g < uGradCount; g++) {
        mat4 transform = mat4(
            texelFetch(uGradData, texelIdx),
            texelFetch(uGradData, texelIdx + 1),
            texelFetch(uGradData, texelIdx + 2),
            texelFetch(uGradData, texelIdx + 3));
        texelIdx += 4;

        vec4 meta = texelFetch(uGradData, texelIdx);
        texelIdx += 1;
        int orientation = int(meta.x + 0.5);
        int stopCount = int(meta.y + 0.5);

        vec4 owner = transform * localPos4;
        float axisPos = (orientation == 0) ? owner.x : owner.y;
        factor *= sampleGradientStops(texelIdx, stopCount, axisPos);

        texelIdx += (stopCount + 1) / 2;
    }

    return factor;
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
    float gradientAlpha = evaluateGradients(vTex * uSize);

    bool useTexture = (uFlags & FLAG_USETEXTURE) != 0;
    if (useTexture)
    {
        bool useNineSlice = (uFlags & FLAG_USENINESLICE) != 0;
        vec2 uv = vTex;

        if (useNineSlice)
        {
            // TODO: Can this be optimized by representing as a matrix?
            vec2 insetX = uNineGrid.xy; /* left, right */
            vec2 insetY = uNineGrid.zw; /* top, bottom */

            vec2 texInsetX = insetX / uTexSize.x;
            vec2 texInsetY = insetY / uTexSize.y;

            vec2 recInsetX = insetX / uSize.x;
            vec2 recInsetY = insetY / uSize.y;

            uv = vec2(
                processAxis(vTex.x, recInsetX, texInsetX),
                processAxis(vTex.y, recInsetY, texInsetY));
        }

        vec4 t = texture(uTex, uv);
        fragColor = vec4(t.rgb, t.a * uAlpha * gradientAlpha);
    }
    else
    {
        fragColor = vec4(uColor.rgb, uColor.a * uAlpha * gradientAlpha);
    }
}