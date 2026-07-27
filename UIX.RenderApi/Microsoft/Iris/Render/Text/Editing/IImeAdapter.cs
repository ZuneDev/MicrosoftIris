namespace Microsoft.Iris.Render.Text.Editing;

// Per-platform IME composition hook for a future non-native text-editing
// engine built on TextEditBuffer. Not wired to anything yet on any platform -
// Microsoft.Iris.Drawing.RichText's existing hosted (interactive) mode still
// gets real IME support directly from the native RichEdit-style control via
// Microsoft.Iris.OS.Win32Api/IMM32 (see NativeApi.SpRichTextForwardImeMessage).
// This interface exists so that work can eventually move onto TextEditBuffer
// without inventing the seam later; other platforms are expected to start
// with a minimal/stubbed implementation, explicitly documented as a gap
// rather than blocking on full IME parity - see
// logs/text-abstraction.md ("Proposed follow-up").
public interface IImeAdapter
{
    bool IsComposing { get; }
    void BeginComposition();
    void UpdateComposition(string compositionText, int caretPosition);
    void EndComposition(bool commit);
}
