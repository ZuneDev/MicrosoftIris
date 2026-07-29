using System;

namespace Microsoft.Iris.Render.OpenGL.Rendering;

[Flags]
public enum FragmentFlags
{
    Default = 0,
    UseTexture = 1 << 0,
    UseNineSlice = 1 << 1,
}