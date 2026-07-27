using System.Runtime.CompilerServices;
using Microsoft.Iris.Drawing;
using Microsoft.Iris.Render.Text;

namespace Microsoft.Iris.OS;

// Registers the native Sp*/NativeApi-backed text/font implementations with
// UIX.RenderApi's factories. Gated on WINDOWS so non-Windows builds of this
// assembly (UIX also targets plain net8.0, see Directory.Build.props) leave
// nothing registered and FontResourceLoader/TextDocumentFactory fall back to
// their SixLabors.Fonts-based defaults.
internal static class TextBackendRegistration
{
    [ModuleInitializer]
    internal static void Register()
    {
#if WINDOWS
        FontResourceLoader.RegisterWindowsBackend(() => new SpFontResource());
        TextDocumentFactory.RegisterWindowsBackend(() => new SpTextDocument());
#endif
    }
}
