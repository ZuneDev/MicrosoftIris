namespace Microsoft.Iris.Render.Text.Editing;

// Per-platform clipboard access used by TextEditBuffer.Cut/Copy/Paste. No
// implementation is registered/wired to anything yet - TextEditBuffer simply
// no-ops those members when ClipboardAdapter is null. See the "full
// cross-platform text engine" proposal in logs/text-abstraction.md.
public interface IClipboardAdapter
{
    string GetText();
    void SetText(string text);
}
