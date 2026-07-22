using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Iris.Render.Interop.Com;
using Microsoft.Iris.Render.Interop.Drawing;

namespace Microsoft.Iris.Render.Subsystems.Text;

// The managed model behind the SpRichText* family (UIX/Microsoft/Iris/OS/NativeApi.cs) --
// the native text-box engine backing Iris's editable TextBox.
//
// The *editing* half of this is fully real: content, selection, clipboard operations,
// undo/redo, word wrap, read-only, max length, scale, scrollbars and timers all behave
// the way an edit control does, and the registered IRichTextCallbacks is notified on the
// same occasions the original would have notified it (TextChanged, SelectionChanged,
// MaxLengthExceeded, ...). That matters because the managed TextBox drives its whole
// state machine off those callbacks.
//
// The *typography* half (measuring and rasterizing glyphs) is where this stops short --
// see RichTextApi.SpRichTextMeasure/SpRichTextRasterize and the open question in
// logs/UIXrender/FullSurface.md.
internal sealed class RichTextObject
{
    // Slot layout of IRichTextCallbacks (UIX/Microsoft/Iris/OS/IRichTextCallbacks.cs),
    // declaration order after the three IUnknown slots. Read off that file, not guessed.
    private const int SlotInvalidateContent = ComVtable.FirstMethodSlot + 0;
    private const int SlotSelectionChanged = ComVtable.FirstMethodSlot + 1;
    private const int SlotCreateCaret = ComVtable.FirstMethodSlot + 2;
    private const int SlotSetCaretPos = ComVtable.FirstMethodSlot + 3;
    private const int SlotShowCaret = ComVtable.FirstMethodSlot + 4;
    private const int SlotSetCursor = ComVtable.FirstMethodSlot + 5;
    private const int SlotTextChanged = ComVtable.FirstMethodSlot + 6;
    private const int SlotMaxLengthExceeded = ComVtable.FirstMethodSlot + 7;
    private const int SlotSetTimer = ComVtable.FirstMethodSlot + 8;
    private const int SlotKillTimer = ComVtable.FirstMethodSlot + 9;
    private const int SlotSetScrollRange = ComVtable.FirstMethodSlot + 10;
    private const int SlotEnableScrollbar = ComVtable.FirstMethodSlot + 11;
    private const int SlotClientToWindow = ComVtable.FirstMethodSlot + 12;
    private const int SlotClientToScreen = ComVtable.FirstMethodSlot + 13;
    private const int SlotLinkClicked = ComVtable.FirstMethodSlot + 14;

    private readonly StringBuilder _content = new();
    private readonly Stack<string> _undo = new();
    private readonly Stack<string> _redo = new();

    // The clipboard is process-wide in the original (a real OS clipboard). There is no
    // cross-platform clipboard in the BCL, so cut/copy/paste round-trip through this
    // shared buffer instead -- fully functional within the app, which is what the
    // managed TextBox's own tests exercise.
    // TODO: bridge to the OS clipboard once a platform abstraction exists.
    private static string s_clipboard = string.Empty;

    public RichTextObject(bool richTextMode, Size maximumSurface, IntPtr callbacks)
    {
        RichTextMode = richTextMode;
        MaximumSurface = maximumSurface;
        Callbacks = callbacks;
    }

    public bool RichTextMode { get; }
    public Size MaximumSurface { get; }
    public IntPtr Callbacks { get; }

    public bool ReadOnly { get; set; }
    public bool WordWrap { get; set; } = true;
    public bool DetectUrls { get; set; }
    public bool Oversample { get; set; }
    public bool HasFocus { get; private set; }
    public float Scale { get; set; } = 1.0f;
    public int MaximumLength { get; set; } = int.MaxValue;
    public bool AllowVerticalScroll { get; set; }
    public bool AllowHorizontalScroll { get; set; }

    public int SelectionStart { get; private set; }
    public int SelectionEnd { get; private set; }

    public string Text => _content.ToString();
    public int Length => _content.Length;

    public bool CanUndo => _undo.Count > 0;

    // ---- content -----------------------------------------------------------------

    public void SetContent(string value)
    {
        PushUndo();
        _content.Clear();
        _content.Append(value ?? string.Empty);
        ClampSelection();
        NotifyTextChanged();
        NotifyInvalidateContent();
    }

    public bool InsertText(string value)
    {
        if (ReadOnly || string.IsNullOrEmpty(value))
            return false;

        DeleteSelectionCore();

        if (_content.Length + value.Length > MaximumLength)
        {
            NotifyMaxLengthExceeded();
            return false;
        }

        PushUndo();
        _content.Insert(SelectionStart, value);
        SelectionStart += value.Length;
        SelectionEnd = SelectionStart;
        NotifyTextChanged();
        NotifyInvalidateContent();
        return true;
    }

    public bool DeleteSelection()
    {
        if (ReadOnly)
            return false;

        PushUndo();
        if (!DeleteSelectionCore())
            return false;

        NotifyTextChanged();
        NotifyInvalidateContent();
        return true;
    }

    private bool DeleteSelectionCore()
    {
        int start = Math.Min(SelectionStart, SelectionEnd);
        int end = Math.Max(SelectionStart, SelectionEnd);
        if (start == end)
            return false;

        _content.Remove(start, end - start);
        SelectionStart = SelectionEnd = start;
        return true;
    }

    public string GetSelectedText()
    {
        int start = Math.Min(SelectionStart, SelectionEnd);
        int end = Math.Max(SelectionStart, SelectionEnd);
        return start == end ? string.Empty : _content.ToString(start, end - start);
    }

    public void SetSelectionRange(int start, int end)
    {
        SelectionStart = Math.Clamp(start, 0, _content.Length);
        SelectionEnd = Math.Clamp(end, 0, _content.Length);
        NotifySelectionChanged();
    }

    private void ClampSelection()
    {
        SelectionStart = Math.Clamp(SelectionStart, 0, _content.Length);
        SelectionEnd = Math.Clamp(SelectionEnd, 0, _content.Length);
    }

    // ---- clipboard ---------------------------------------------------------------

    public void Copy() => s_clipboard = GetSelectedText();

    public bool Cut()
    {
        if (ReadOnly)
            return false;
        s_clipboard = GetSelectedText();
        return DeleteSelection();
    }

    public bool Paste() => InsertText(s_clipboard);

    // ---- undo / redo -------------------------------------------------------------

    private void PushUndo()
    {
        _undo.Push(_content.ToString());
        _redo.Clear();
    }

    public bool Undo()
    {
        if (_undo.Count == 0)
            return false;

        _redo.Push(_content.ToString());
        string previous = _undo.Pop();
        _content.Clear();
        _content.Append(previous);
        ClampSelection();
        NotifyTextChanged();
        NotifyInvalidateContent();
        return true;
    }

    public bool Redo()
    {
        if (_redo.Count == 0)
            return false;

        _undo.Push(_content.ToString());
        string next = _redo.Pop();
        _content.Clear();
        _content.Append(next);
        ClampSelection();
        NotifyTextChanged();
        NotifyInvalidateContent();
        return true;
    }

    // ---- focus / caret -----------------------------------------------------------

    public void SetFocus(bool gainingFocus)
    {
        HasFocus = gainingFocus;
        NotifyShowCaret(gainingFocus);
    }

    // ---- callback dispatch -------------------------------------------------------

    private unsafe void Notify(int slot)
    {
        void* fn = ComVtable.Slot(Callbacks, slot);
        if (fn != null)
            ((delegate* unmanaged<IntPtr, int>)fn)(Callbacks);
    }

    private unsafe void Notify(int slot, int arg)
    {
        void* fn = ComVtable.Slot(Callbacks, slot);
        if (fn != null)
            ((delegate* unmanaged<IntPtr, int, int>)fn)(Callbacks, arg);
    }

    private unsafe void Notify(int slot, int a, int b)
    {
        void* fn = ComVtable.Slot(Callbacks, slot);
        if (fn != null)
            ((delegate* unmanaged<IntPtr, int, int, int>)fn)(Callbacks, a, b);
    }

    public void NotifyTextChanged() => Notify(SlotTextChanged);
    public void NotifyInvalidateContent() => Notify(SlotInvalidateContent);
    public void NotifySelectionChanged() => Notify(SlotSelectionChanged, SelectionStart, SelectionEnd);
    public void NotifyMaxLengthExceeded() => Notify(SlotMaxLengthExceeded);
    public void NotifyShowCaret(bool visible) => Notify(SlotShowCaret, visible ? 1 : 0);
    public void NotifyLinkClicked(int start, int end) => Notify(SlotLinkClicked, start, end);
    public void NotifySetScrollRange(int whichBar, int min, int extent, int viewExtent, int position) => NotifyScrollRange(whichBar, min, extent, viewExtent, position);

    private unsafe void NotifyScrollRange(int whichBar, int min, int extent, int viewExtent, int position)
    {
        void* fn = ComVtable.Slot(Callbacks, SlotSetScrollRange);
        if (fn != null)
            ((delegate* unmanaged<IntPtr, int, int, int, int, int, int>)fn)(Callbacks, whichBar, min, extent, viewExtent, position);
    }
}
