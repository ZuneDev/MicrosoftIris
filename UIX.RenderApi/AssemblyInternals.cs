using System.Runtime.CompilerServices;

// The in-process OpenGL render engine re-implements the Splash animation curve
// evaluation that originally lived in native UIXrender.dll. To apply the real
// curves it must read the interpolation parameters (Weight / Handle /
// ControlPoint1 / ControlPoint2) that the original API keeps `internal`. Grant
// the sibling assembly access rather than widening the original public surface.
// See logs/UIX.RenderApi.OpenGL/Implementation.md (2026-07-25 curve recovery).
[assembly: InternalsVisibleTo("UIX.RenderApi.OpenGL")]
