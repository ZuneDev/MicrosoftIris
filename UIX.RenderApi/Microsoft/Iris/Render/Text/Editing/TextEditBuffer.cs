using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Iris.Render.Text.Editing;

// Pure-managed, platform-agnostic text-editor core: content buffer,
// selection, and undo/redo. Foundation piece of the "full cross-platform
// text engine" proposal in logs/text-abstraction.md - deliberately does NOT
// cover rendering/layout (that's TextDocument, elsewhere in this namespace),
// caret movement/word-boundary logic (that belongs to whatever input handler
// drives SetSelection - mirrors Microsoft.Iris.InputHandlers.TextEditingHandler's
// existing split of responsibilities), or IME composition (see IImeAdapter).
// Clipboard access is delegated to an injected IClipboardAdapter so this
// class has no OS dependency of its own.
//
// Not yet wired into Microsoft.Iris.Drawing.RichText's hosted (interactive)
// mode - that integration means replacing NativeApi.SpRichText*'s keyboard/
// mouse/IME forwarding and timer-driven caret blink, which is a much larger,
// separate effort. See logs/text-abstraction.md for why that's deliberately
// out of scope here.
public sealed class TextEditBuffer
{
    private readonly StringBuilder _text = new();
    private readonly List<UndoEntry> _undoStack = new();
    private readonly List<UndoEntry> _redoStack = new();
    private TextSelection _selection = TextSelection.Empty;

    public IClipboardAdapter ClipboardAdapter { get; set; }

    public int? MaxLength { get; set; }

    public bool ReadOnly { get; set; }

    public string Text => _text.ToString();

    public int Length => _text.Length;

    public TextSelection Selection => _selection;

    public bool CanUndo => _undoStack.Count > 0;

    public bool CanRedo => _redoStack.Count > 0;

    public event EventHandler TextChanged;

    public event EventHandler SelectionChanged;

    public void SetText(string text)
    {
        text ??= string.Empty;
        if (MaxLength is int max && text.Length > max)
            text = text.Substring(0, max);

        _text.Clear();
        _text.Append(text);
        _undoStack.Clear();
        _redoStack.Clear();
        SetSelectionCore(_text.Length, _text.Length);
        RaiseTextChanged();
    }

    public void SetSelection(int start, int end)
    {
        start = Math.Clamp(start, 0, _text.Length);
        end = Math.Clamp(end, 0, _text.Length);
        SetSelectionCore(start, end);
    }

    // Replaces the current selection (or inserts at the caret if the
    // selection is empty) with `text`. Mirrors typing a character, IME
    // composition commit, or a programmatic insert.
    public void Insert(string text)
    {
        if (ReadOnly || string.IsNullOrEmpty(text))
            return;
        Replace(_selection.Start, _selection.End, text);
    }

    public enum DeleteDirection
    {
        Backward,
        Forward,
    }

    public void Delete(DeleteDirection direction)
    {
        if (ReadOnly)
            return;
        if (!_selection.IsEmpty)
        {
            Replace(_selection.Start, _selection.End, string.Empty);
            return;
        }
        if (direction == DeleteDirection.Backward)
        {
            if (_selection.Start == 0)
                return;
            Replace(_selection.Start - 1, _selection.Start, string.Empty);
        }
        else
        {
            if (_selection.Start >= _text.Length)
                return;
            Replace(_selection.Start, _selection.Start + 1, string.Empty);
        }
    }

    public void Replace(int start, int end, string replacement)
    {
        if (ReadOnly)
            return;

        start = Math.Clamp(start, 0, _text.Length);
        end = Math.Clamp(end, start, _text.Length);
        replacement ??= string.Empty;

        if (MaxLength is int max)
        {
            var resultLength = _text.Length - (end - start) + replacement.Length;
            if (resultLength > max)
            {
                var overflow = resultLength - max;
                replacement = replacement.Substring(0, Math.Max(0, replacement.Length - overflow));
            }
        }

        var removed = end > start ? _text.ToString(start, end - start) : string.Empty;
        ApplyReplace(start, end, replacement);

        _undoStack.Add(new UndoEntry(start, removed, replacement, _selection));
        // TODO: coalesce consecutive single-character insert/delete entries
        // into one undo step (matching typical editor "typing = one undo
        // unit" behavior). Deferred - each edit is its own undo step today,
        // which is correct, just coarser-grained than a native RichEdit.
        _redoStack.Clear();

        var caret = start + replacement.Length;
        SetSelectionCore(caret, caret);
        RaiseTextChanged();
    }

    public void Undo()
    {
        if (_undoStack.Count == 0)
            return;
        var entry = Pop(_undoStack);
        ApplyReplace(entry.Start, entry.Start + entry.Inserted.Length, entry.Removed);
        _redoStack.Add(entry);
        SetSelectionCore(entry.SelectionBefore.Start, entry.SelectionBefore.End);
        RaiseTextChanged();
    }

    public void Redo()
    {
        if (_redoStack.Count == 0)
            return;
        var entry = Pop(_redoStack);
        ApplyReplace(entry.Start, entry.Start + entry.Removed.Length, entry.Inserted);
        _undoStack.Add(entry);
        var caret = entry.Start + entry.Inserted.Length;
        SetSelectionCore(caret, caret);
        RaiseTextChanged();
    }

    public void Cut()
    {
        if (ReadOnly || _selection.IsEmpty || ClipboardAdapter == null)
            return;
        ClipboardAdapter.SetText(_text.ToString(_selection.Start, _selection.Length));
        Replace(_selection.Start, _selection.End, string.Empty);
    }

    public void Copy()
    {
        if (_selection.IsEmpty || ClipboardAdapter == null)
            return;
        ClipboardAdapter.SetText(_text.ToString(_selection.Start, _selection.Length));
    }

    public void Paste()
    {
        if (ReadOnly || ClipboardAdapter == null)
            return;
        var clip = ClipboardAdapter.GetText();
        if (!string.IsNullOrEmpty(clip))
            Insert(clip);
    }

    private void ApplyReplace(int start, int end, string replacement)
    {
        _text.Remove(start, end - start);
        _text.Insert(start, replacement);
    }

    private void SetSelectionCore(int start, int end)
    {
        var next = new TextSelection(start, end);
        if (next == _selection)
            return;
        _selection = next;
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private static UndoEntry Pop(List<UndoEntry> stack)
    {
        var entry = stack[^1];
        stack.RemoveAt(stack.Count - 1);
        return entry;
    }

    private void RaiseTextChanged() => TextChanged?.Invoke(this, EventArgs.Empty);

    private readonly struct UndoEntry
    {
        public UndoEntry(int start, string removed, string inserted, TextSelection selectionBefore)
        {
            Start = start;
            Removed = removed;
            Inserted = inserted;
            SelectionBefore = selectionBefore;
        }

        public int Start { get; }
        public string Removed { get; }
        public string Inserted { get; }
        public TextSelection SelectionBefore { get; }
    }
}
