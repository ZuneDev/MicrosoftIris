namespace Microsoft.Iris.Support;

// Mirrors Microsoft.Iris.Render.DebugCategory (UIX.RenderApi/Microsoft/Iris/Render/DebugCategory.cs)
// field-for-field: ordinal values are part of the wire contract with the already-shipped
// managed callers (DebugGetCategoryLevel/DebugSetCategoryLevel pass this by value), so
// the order here must match exactly, not just the names.
public enum DebugCategory
{
    Animation,
    Critical,
    Deprecated,
    Dispatcher,
    Dump,
    Dx9Wrapper,
    DynamicSurface,
    Failure,
    FormWindow,
    GraphicsCache,
    IFC,
    ObjectCache,
    Performance,
    Profile,
    Queues,
    Render,
    Sounds,
    Surface,
    SurfacePool,
    SurfaceRestoration,
    Timeouts,
    Transport,
    WindowState,
    Effect,
    HTTP,
    ExternalCount,
    Protocol,
    RenderHost,
    DumpException,
    Focus,
    Input,
    TreeDisposal,
    VisualSizePos,
    Visual,
    Painting,
    RenderWindow,
    ImageLoad,
    ImageCache,
    TotalCount,
}
