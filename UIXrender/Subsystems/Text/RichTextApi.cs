using System;
using System.Runtime.InteropServices;
using Microsoft.Iris.Interop;
using Microsoft.Iris.Render.Engine;
using Microsoft.Iris.Render.Interop;
using Microsoft.Iris.Render.Interop.Drawing;
using Microsoft.Iris.Render.Interop.Text;
using Microsoft.Iris.Render.Interop.Win32;

namespace Microsoft.Iris.Render.Subsystems.Text;

// [UnmanagedCallersOnly] exports for the SpRichText*/SpSimpleText* families in
// UIX/Microsoft/Iris/OS/NativeApi.cs (~35 entry points).
//
// Editing, selection, clipboard, undo/redo and all the mode flags are real (see
// RichTextObject). Measurement is approximate (see TextMetrics) and rasterization is not
// implemented -- both flagged in logs/UIXrender/FullSurface.md rather than quietly
// returning plausible-looking output.
public static unsafe class RichTextApi
{
    // Win32 message ids the managed side forwards; declared in NativeApi.cs as constants.
    private const uint WM_KEYDOWN = 0x0100;
    private const uint WM_CHAR = 0x0102;

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextBuildObject")]
    public static HRESULT SpRichTextBuildObject(int fRichTextMode, Size sizeMaximumSurface, IntPtr pCallbacks, HANDLE* hRto)
    {
        if (hRto == null)
            return HRESULT.E_INVALIDARG;

        Interop.Com.ComVtable.AddRef(pCallbacks);
        var text = new RichTextObject(fRichTextMode != 0, sizeMaximumSurface, pCallbacks);
        hRto->h = HandleTable.Alloc(text);
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextDestroyObject")]
    public static void SpRichTextDestroyObject(HANDLE hRto)
    {
        if (HandleTable.TryGet(hRto.h, out RichTextObject text))
            Interop.Com.ComVtable.Release(text.Callbacks);
        HandleTable.Free(hRto.h);
    }

    // ---- content -----------------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextSetContent")]
    public static HRESULT SpRichTextSetContent(HANDLE hRto, char* pszContent)
    {
        if (!HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;

        text.SetContent(NativeString.UniToString(pszContent));
        return HRESULT.S_OK;
    }

    // Writes into the caller's StringBuilder buffer (marshaled as a char* of cchBuffer
    // characters), NUL-terminated and never overrunning.
    [UnmanagedCallersOnly(EntryPoint = "SpRichTextGetSimpleContent")]
    public static HRESULT SpRichTextGetSimpleContent(HANDLE hRto, char* textBuffer, int cchBuffer)
    {
        if (textBuffer == null || cchBuffer <= 0 || !HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;

        string content = text.Text;
        int copy = Math.Min(content.Length, cchBuffer - 1);
        content.AsSpan(0, copy).CopyTo(new Span<char>(textBuffer, copy));
        textBuffer[copy] = '\0';
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextGetSimpleContentLength")]
    public static HRESULT SpRichTextGetSimpleContentLength(HANDLE hRto, int* textLength)
    {
        if (textLength == null || !HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;

        *textLength = text.Length;
        return HRESULT.S_OK;
    }

    // ---- clipboard / editing -----------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextCopy")]
    public static HRESULT SpRichTextCopy(HANDLE hRto)
    {
        if (!HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;
        text.Copy();
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextCut")]
    public static HRESULT SpRichTextCut(HANDLE hRto)
    {
        if (!HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;
        return text.Cut() ? HRESULT.S_OK : HRESULT.E_FAIL;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextPaste")]
    public static HRESULT SpRichTextPaste(HANDLE hRto)
    {
        if (!HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;
        return text.Paste() ? HRESULT.S_OK : HRESULT.E_FAIL;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextDelete")]
    public static HRESULT SpRichTextDelete(HANDLE hRto)
    {
        if (!HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;
        return text.DeleteSelection() ? HRESULT.S_OK : HRESULT.E_FAIL;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextCanUndo")]
    public static HRESULT SpRichTextCanUndo(HANDLE hRto, int* canUndo)
    {
        if (canUndo == null || !HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;

        *canUndo = text.CanUndo ? 1 : 0;
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextUndo")]
    public static HRESULT SpRichTextUndo(HANDLE hRto)
    {
        if (!HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;
        return text.Undo() ? HRESULT.S_OK : HRESULT.E_FAIL;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextSetSelectionRange")]
    public static HRESULT SpRichTextSetSelectionRange(HANDLE hRto, int selectionStart, int selectionEnd)
    {
        if (!HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;

        text.SetSelectionRange(selectionStart, selectionEnd);
        return HRESULT.S_OK;
    }

    // ---- mode flags --------------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextSetReadOnly")]
    public static HRESULT SpRichTextSetReadOnly(HANDLE hRto, int readOnly) =>
        Apply(hRto, t => t.ReadOnly = readOnly != 0);

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextSetWordWrap")]
    public static HRESULT SpRichTextSetWordWrap(HANDLE hRto, int fWordWrap) =>
        Apply(hRto, t => t.WordWrap = fWordWrap != 0);

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextSetMaximumLength")]
    public static HRESULT SpRichTextSetMaximumLength(HANDLE hRto, int maximumLength) =>
        Apply(hRto, t => t.MaximumLength = maximumLength > 0 ? maximumLength : int.MaxValue);

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextSetDetectUrls")]
    public static HRESULT SpRichTextSetDetectUrls(HANDLE hRto, int detectUrls) =>
        Apply(hRto, t => t.DetectUrls = detectUrls != 0);

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextSetOversampleMode")]
    public static HRESULT SpRichTextSetOversampleMode(HANDLE hRto, int fOversample) =>
        Apply(hRto, t => t.Oversample = fOversample != 0);

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextSetScale")]
    public static HRESULT SpRichTextSetScale(HANDLE hRto, float flScale) =>
        Apply(hRto, t => t.Scale = flScale);

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextSetScrollbars")]
    public static HRESULT SpRichTextSetScrollbars(HANDLE hRto, int allowVertical, int allowHorizontal) =>
        Apply(hRto, t =>
        {
            t.AllowVerticalScroll = allowVertical != 0;
            t.AllowHorizontalScroll = allowHorizontal != 0;
        });

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextSetFocus")]
    public static HRESULT SpRichTextSetFocus(HANDLE hRto, int gainingFocus) =>
        Apply(hRto, t => t.SetFocus(gainingFocus != 0));

    // ---- input forwarding --------------------------------------------------------

    // Real: a WM_CHAR carrying a printable character is inserted into the buffer (which
    // fires TextChanged and honours read-only/max-length), anything else is reported
    // unhandled so the managed side can act on it.
    [UnmanagedCallersOnly(EntryPoint = "SpRichTextForwardKeyCharacter")]
    public static HRESULT SpRichTextForwardKeyCharacter(HANDLE hRto, uint message, int character, int scanCode, int repeatCount, uint modifierState, ushort flags, int* handled)
    {
        if (handled == null || !HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;

        *handled = 0;
        if (message != WM_CHAR || character < ' ')
            return HRESULT.S_OK;

        var inserted = new string((char)character, Math.Max(1, repeatCount));
        *handled = text.InsertText(inserted) ? 1 : 0;
        return HRESULT.S_OK;
    }

    // Editing keys (backspace/delete) act on the buffer; navigation and everything else
    // is left to the managed side, which owns caret movement.
    [UnmanagedCallersOnly(EntryPoint = "SpRichTextForwardKeyState")]
    public static HRESULT SpRichTextForwardKeyState(HANDLE hRto, uint message, int virtualKey, int scanCode, int repeatCount, uint modifierState, ushort flags, int* handled)
    {
        const int VK_BACK = 0x08;
        const int VK_DELETE = 0x2E;

        if (handled == null || !HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;

        *handled = 0;
        if (message != WM_KEYDOWN)
            return HRESULT.S_OK;

        if (virtualKey is VK_BACK or VK_DELETE)
            *handled = text.DeleteSelection() ? 1 : 0;

        return HRESULT.S_OK;
    }

    // Mouse hit-testing needs real glyph positions to map a point to a character offset,
    // which this implementation doesn't have (see TextMetrics). Reports unhandled rather
    // than moving the caret to a wrong offset.
    // TODO: implement alongside a real font backend.
    [UnmanagedCallersOnly(EntryPoint = "SpRichTextForwardMouseInput")]
    public static HRESULT SpRichTextForwardMouseInput(HANDLE hRto, uint message, uint modifierState, int mouseButton, int x, int y, int mouseWheelDelta, int* handled)
    {
        if (handled == null)
            return HRESULT.E_INVALIDARG;
        *handled = 0;
        return HRESULT.S_OK;
    }

    // IME composition needs a platform input-method context to interpret; the managed
    // side's own IME plumbing (SpRegisterImeCallbacks) is the path that actually carries
    // these today.
    [UnmanagedCallersOnly(EntryPoint = "SpRichTextForwardImeMessage")]
    public static HRESULT SpRichTextForwardImeMessage(HANDLE hRto, uint message, UIntPtr wParam, UIntPtr lParam) => HRESULT.S_OK;

    // ---- scrolling / timers ------------------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextScroll")]
    public static HRESULT SpRichTextScroll(HANDLE hRto, int whichBar, int scrollType) =>
        Apply(hRto, static _ => { });

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextScrollToPosition")]
    public static HRESULT SpRichTextScrollToPosition(HANDLE hRto, int whichBar, int whereTo) =>
        Apply(hRto, static _ => { });

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextOnTimerTick")]
    public static HRESULT SpRichTextOnTimerTick(HANDLE hRto, uint timerId) =>
        Apply(hRto, static t => t.NotifyShowCaret(t.HasFocus));

    // ---- measurement / rasterization ---------------------------------------------

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextGetNaturalBounds")]
    public static HRESULT SpRichTextGetNaturalBounds(HANDLE hRto, int* cWidth, int* cHeight)
    {
        if (cWidth == null || cHeight == null || !HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;

        // Font height isn't carried on the object (it arrives per-measure in a TextStyle),
        // so natural bounds use the surface's own height as the em size reference. No face
        // is known here, so the fallback font resolves.
        LoadedFont font = FontStore.Resolve(null);
        Size bounds = TextLayout.Measure(font, text.Text, DefaultFontHeight, text.WordWrap, text.MaximumSurface.width);
        *cWidth = bounds.width;
        *cHeight = bounds.height;
        return HRESULT.S_OK;
    }

    private const float DefaultFontHeight = 12f;

    // Real measurement over the resolved font (or the ratio fallback). The per-run
    // ReportRunCallback is still not invoked -- rich text's multi-run layout isn't modelled
    // yet; a caller gets correct overall bounds via the returned constraint size. Rich-text
    // rasterization runs through GlyphRun handles produced by the simple-text path.
    // TODO: emit real per-run glyph runs through rrcb.
    [UnmanagedCallersOnly(EntryPoint = "SpRichTextMeasure")]
    public static HRESULT SpRichTextMeasure(HANDLE hRto, TextMeasureParamsData* measureParams, IntPtr rrcb, IntPtr pvData)
    {
        if (measureParams == null || !HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;

        float fontHeight = measureParams->pTextStyle != null && measureParams->pTextStyle->fontHeightPts > 0
            ? measureParams->pTextStyle->fontHeightPts
            : DefaultFontHeight;

        string face = measureParams->pTextStyle != null ? NativeString.UniToString(measureParams->pTextStyle->fontFace) : null;
        string content = measureParams->content != null ? NativeString.UniToString(measureParams->content) : text.Text;
        bool wordWrap = (measureParams->flags & TextMeasureFlags.WordWrapValue) != 0;
        int constraint = (int)measureParams->constraint.width;

        LoadedFont font = FontStore.Resolve(face);
        Size measured = TextLayout.Measure(font, content, fontHeight, wordWrap, constraint);
        measureParams->constraint = new SizeF { width = measured.width, height = measured.height };
        return HRESULT.S_OK;
    }

    // Composites the glyph run into a straight-alpha ARGB32 bitmap. `phTextBitmap` is a
    // handle SpFreeDib frees; `ppvBits` points at the pixels. Returns S_FALSE-shaped
    // failure (E_FAIL) only if the run can't be resolved; an empty/fallback run yields a
    // null bitmap with S_OK (nothing to draw), not a fake.
    // Outline/shadow modes are accepted but not yet rendered.
    // TODO: outline + shadow passes.
    [UnmanagedCallersOnly(EntryPoint = "SpRichTextRasterize")]
    public static HRESULT SpRichTextRasterize(IntPtr hGlyphRunInfo, int fOutlineMode, Color clrText, int fShadowMode, IntPtr* phTextBitmap, IntPtr* ppvBits, Size* psizeBitmap)
    {
        if (phTextBitmap != null) *phTextBitmap = IntPtr.Zero;
        if (ppvBits != null) *ppvBits = IntPtr.Zero;
        if (psizeBitmap != null) *psizeBitmap = default;

        if (!HandleTable.TryGet(hGlyphRunInfo, out GlyphRun run))
            return HRESULT.E_INVALIDARG;

        IntPtr bits = run.Rasterize(clrText, out Size size);
        if (psizeBitmap != null) *psizeBitmap = size;

        if (bits == IntPtr.Zero)
            return HRESULT.S_OK; // nothing to draw (empty run or no real font)

        if (phTextBitmap != null) *phTextBitmap = HandleTable.Alloc(new TextBitmap(bits));
        if (ppvBits != null) *ppvBits = bits;
        return HRESULT.S_OK;
    }

    [UnmanagedCallersOnly(EntryPoint = "SpRichTextDestroyGlyphRunInfo")]
    public static void SpRichTextDestroyGlyphRunInfo(IntPtr hGlyphRunInfo) => HandleTable.Free(hGlyphRunInfo);

    // ---- helpers -----------------------------------------------------------------

    private static HRESULT Apply(HANDLE hRto, Action<RichTextObject> action)
    {
        if (!HandleTable.TryGet(hRto.h, out RichTextObject text))
            return HRESULT.E_INVALIDARG;

        action(text);
        return HRESULT.S_OK;
    }
}
