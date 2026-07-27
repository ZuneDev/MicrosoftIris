using System;

namespace Microsoft.Iris.Render.Text;

// See FontResourceLoader for why this is a registration seam rather than a
// #if WINDOWS switch: the Windows backend needs Microsoft.Iris.OS.NativeApi's
// marshalling, which lives in the downstream UIX project.
public static class TextDocumentFactory
{
    private static Func<TextDocument> s_windowsStandaloneBackend;

    public static void RegisterWindowsBackend(Func<TextDocument> factory) => s_windowsStandaloneBackend = factory;

    // A standalone TextDocument owns its own backend state independently of
    // any other object - this is what Microsoft.Iris.Drawing.SimpleText uses.
    public static TextDocument CreateStandalone() => s_windowsStandaloneBackend?.Invoke() ?? new SixLaborsTextDocument();
}
