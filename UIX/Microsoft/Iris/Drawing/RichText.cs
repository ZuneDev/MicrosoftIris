// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.RichText
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.Render.Text;
using Microsoft.Iris.Render.Text.Editing;
using Microsoft.Iris.RenderAPI;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.ViewItems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Iris.Drawing
{
    internal sealed class RichText : IDisposable
    {
        public const float MaxWidthConstraint = 4095f;
        public const float MaxHeightConstraint = 8191f;
        private Win32Api.HANDLE _rtoHandle;
        // Bound to _rtoHandle on Windows: routes SetContent/GetSimpleContent/
        // GetNaturalBounds/Measure through the cross-platform
        // Microsoft.Iris.Render.Text.TextDocument abstraction. On non-Windows
        // platforms this is a SixLaborsTextDocument instead - see the
        // constructor. Either way, _textDocument only ever backs the
        // *non-hosted* (display-only, callbacks == null) role of RichText:
        // the interactive-editing surface below (IME, keyboard/mouse
        // forwarding, undo, clipboard, scrollbars, timers) remains
        // native/Windows-only regardless of platform - see logs/ for the
        // rationale and scope decision.
        private readonly TextDocument _textDocument;
        private NativeApi.ReportRunCallback _rrcb;
        private string _currentlyMeasuringText;
        private bool _oversampled;
        private bool _hosted;
        private bool _inImeCompositionMode;
        private ArrayList _timers;
        private EventHandler _timerTickHandler;
        private object _lock;
#if !WINDOWS
        // Cross-platform counterpart of the native RTO's interactive-editing
        // state (hosted mode only - see the constructor). TextEditBuffer
        // covers content/selection/undo; everything else here (caret blink,
        // layout caching for hit-testing, mouse-drag selection) is the
        // managed equivalent of what the native RichEdit-style control does
        // internally and reports via IRichTextCallbacks.
        private TextEditBuffer _editBuffer;
        private IRichTextCallbacks _callbacks;
        private const uint CaretBlinkTimerId = 1;
        private bool _caretBlinkVisible;
        private bool _focused;
        private bool _mouseSelecting;
        private int _dragAnchor;
        // Fixed end of an in-progress Shift+arrow-key selection, independent
        // of _dragAnchor (mouse drag uses its own anchor). -1 means no
        // keyboard shift-selection is in progress - the next Shift+movement
        // starts one from the current caret.
        private int _selectionAnchor = -1;
        private bool _wordWrapFlag;
        // Cached from the most recent Measure() call - caret/hit-test math
        // needs the same layout the last render pass produced. A real native
        // RichEdit control tracks this internally; SixLaborsTextDocument
        // recomputes layout on demand instead, so RichText caches the inputs
        // of the last Measure() call to reuse for caret placement and mouse
        // hit-testing between Measure calls (e.g. while handling a key press
        // that didn't itself trigger a re-layout).
        private TextStyleInfo _lastMeasureStyle;
        private Size _lastMeasureConstraint;
        private bool _lastMeasureWordWrap;
#endif

        public RichText(bool richTextMode)
          : this(richTextMode, null)
        {
        }

        public unsafe RichText(bool richTextMode, IRichTextCallbacks callbacks)
        {
            Size sizeMaximumSurface = Size.Zero;
            if (UISession.Default != null)
                sizeMaximumSurface = UIImage.MaximumSurfaceSize(UISession.Default);
            if (callbacks != null)
            {
                _hosted = true;
                _timers = new ArrayList(6);
                _timerTickHandler = new EventHandler(OnTimerTick);
            }
#if WINDOWS
            RendererApi.IFC(NativeApi.SpRichTextBuildObject(richTextMode, sizeMaximumSurface, callbacks, out _rtoHandle));
            _textDocument = new SpTextDocument(_rtoHandle);
            _rrcb = new NativeApi.ReportRunCallback(OnReportRun);
#else
            // Cross-platform hosted (interactive-editing) mode: backed by
            // TextEditBuffer for content/selection/undo plus SixLaborsTextDocument
            // for layout/hit-testing, driving `callbacks` the way the native
            // RichEdit-style control would. See logs/text-abstraction.md for
            // the design and its scope (no IME composition, no scrollbars,
            // no system clipboard yet - all explicitly deferred there).
            if (_hosted)
            {
                _callbacks = callbacks;
                _editBuffer = new TextEditBuffer();
                _editBuffer.TextChanged += OnEditBufferTextChanged;
                _editBuffer.SelectionChanged += OnEditBufferSelectionChanged;
            }
            _textDocument = new SixLaborsTextDocument();
#endif
            _oversampled = false;
            _lock = new object();
        }

        public void Dispose() => Dispose(true);

        private void Dispose(bool inDispose)
        {
            if (!inDispose)
                return;
            GC.SuppressFinalize(this);
            lock (_lock)
            {
                _textDocument.Dispose();
#if WINDOWS
                NativeApi.SpRichTextDestroyObject(_rtoHandle);
                _rtoHandle.h = IntPtr.Zero;
#endif
            }
            if (_timers == null)
                return;
            for (int index = 0; index < _timers.Count; ++index)
                DisposeTimer((DispatcherTimer)_timers[index]);
            _timers.Clear();
        }

        ~RichText() => Dispose(false);

        public string Content
        {
            set
            {
                lock (_lock)
                    RendererApi.IFC(new HRESULT(_textDocument.SetContent(value).Int));
            }
        }

        public string SimpleContent
        {
            get
            {
                lock (_lock)
                {
                    RendererApi.IFC(new HRESULT(_textDocument.GetSimpleContent(out var content).Int));
                    return content;
                }
            }
        }

        public bool Oversample
        {
            set
            {
                if (_oversampled == value)
                    return;
#if WINDOWS
                lock (_lock)
                    RendererApi.IFC(NativeApi.SpRichTextSetOversampleMode(_rtoHandle, value));
#endif
                // TODO: SixLaborsTextDocument's rasterizer has no oversampled
                // (higher-res antialiasing) mode yet; the flag is tracked but
                // has no effect on non-Windows platforms.
                _oversampled = value;
            }
            get => _oversampled;
        }

        public int MaxLength
        {
            set
            {
                lock (_lock)
                {
#if WINDOWS
                    RendererApi.IFC(NativeApi.SpRichTextSetMaximumLength(_rtoHandle, value));
#else
                    if (_editBuffer != null)
                        _editBuffer.MaxLength = value;
#endif
                }
            }
        }

        public bool DetectUrls
        {
            set
            {
                lock (_lock)
                {
#if WINDOWS
                    RendererApi.IFC(NativeApi.SpRichTextSetDetectUrls(_rtoHandle, value));
#endif
                    // TODO: URL detection/highlighting isn't implemented
                    // cross-platform yet; the flag isn't even tracked since
                    // nothing on this path reads it.
                }
            }
        }

        public bool HasCallbacks => _hosted;

        public bool ReadOnly
        {
            set
            {
                lock (_lock)
                {
#if WINDOWS
                    RendererApi.IFC(NativeApi.SpRichTextSetReadOnly(_rtoHandle, value));
#else
                    if (_editBuffer != null)
                        _editBuffer.ReadOnly = value;
#endif
                }
            }
        }

        public void SetWordWrap(bool wordWrap)
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextSetWordWrap(_rtoHandle, wordWrap));
#else
                // Word wrap is otherwise sourced fresh from each Measure()
                // call's TextMeasureParams (see the non-Windows Measure
                // override below) - this flag only affects the native RTO's
                // own internal state (used e.g. by its GetNaturalBounds/hit
                // testing). Tracked for parity but not consumed by
                // GetNaturalBounds below yet - TODO.
                _wordWrapFlag = wordWrap;
#endif
            }
        }

        public Size GetNaturalBounds()
        {
            lock (_lock)
            {
                RendererApi.IFC(new HRESULT(_textDocument.GetNaturalBounds(out var bounds).Int));
                return bounds;
            }
        }

        public void SetSelectionRange(int selectionStart, int selectionEnd)
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextSetSelectionRange(_rtoHandle, selectionStart, selectionEnd));
#else
                _editBuffer?.SetSelection(selectionStart, selectionEnd);
#endif
            }
        }

#if WINDOWS
        public unsafe TextFlow Measure(string content, ref TextMeasureParams measureParams)
        {
            TextFlow textFlow = new TextFlow();
            GCHandle gcHandle = GCHandle.Alloc(textFlow);
            _currentlyMeasuringText = content != null ? content : string.Empty;
            fixed (char* content1 = _currentlyMeasuringText)
            fixed (char* chPtr = measureParams._textStyle.TruncatedFontFace)
            {
                var style = new TextStyle.MarshalledData(measureParams._textStyle)
                {
                    _fontFace = chPtr
                };
                measureParams.SetContent(content1);
                measureParams._data._pTextStyle = &style;
                fixed (TextMeasureParams.FormattedRange* formattedRangePtr = measureParams._formattedRanges)
                {
                    measureParams._data._pFormattedRanges = formattedRangePtr;
                    fixed (TextStyle.MarshalledData* marshalledDataPtr = measureParams._formattedRangeStyles)
                    {
                        measureParams._data._pFormattedRangeStyles = marshalledDataPtr;
                        lock (_lock)
                            RendererApi.IFC(NativeApi.SpRichTextMeasure(_rtoHandle, ref measureParams._data, _rrcb, GCHandle.ToIntPtr(gcHandle)));
                    }
                }
            }
            gcHandle.Free();
            _currentlyMeasuringText = null;
            return textFlow;
        }
#else
        // Cross-platform counterpart of the block above, routed through the
        // TextDocument abstraction instead of NativeApi.SpRichTextMeasure.
        // Used for both non-hosted (display-only) and hosted (interactive-
        // editing) instances - Text viewitems render a hosted RichText's
        // content through this same path via ExternalRasterizer, exactly
        // like a non-hosted one (see TextEditingHandler.TextDisplay).
        // measureParams' edit-mode-only fields (SetEditMode/SetPasswordChar/
        // TrimLeftSideBearing) are still not honored here - an accepted,
        // documented gap for now (see logs/text-abstraction.md), not a
        // silent behavior change, since Windows keeps using the byte-
        // identical native branch above unconditionally.
        public TextFlow Measure(string content, ref TextMeasureParams measureParams)
        {
            content ??= string.Empty;
            var baseStyle = ToStyleInfo(measureParams._textStyle);
            var alignment = measureParams._data._alignment switch
            {
                1 => TextAlignment.Near,
                3 => TextAlignment.Center,
                2 => TextAlignment.Far,
                _ => TextAlignment.Near,
            };
            var constraint = new Size((int)measureParams._data._constraint.Width, (int)measureParams._data._constraint.Height);
            var wordWrap = (measureParams._data._flags & TextMeasureParams.MeasureFlags.WordWrap) != 0
                && (measureParams._data._flags & TextMeasureParams.MeasureFlags.WordWrapValue) != 0;

            List<TextStyleRun> formattedRanges = null;
            if (measureParams._formattedRanges is { Length: > 0 })
            {
                formattedRanges = new List<TextStyleRun>(measureParams._formattedRanges.Length);
                foreach (var range in measureParams._formattedRanges)
                {
                    TextStyleInfo rangeStyle = null;
                    if (measureParams._formattedRangeStyles != null && (uint)range.StyleIndex < (uint)measureParams._formattedRangeStyles.Length)
                        rangeStyle = ToStyleInfo(measureParams._formattedRangeStyles[range.StyleIndex]);
                    formattedRanges.Add(new TextStyleRun
                    {
                        FirstCharacter = range.FirstCharacter,
                        LastCharacter = range.LastCharacter,
                        Style = rangeStyle,
                    });
                }
            }

            _lastMeasureStyle = baseStyle;
            _lastMeasureConstraint = constraint;
            _lastMeasureWordWrap = wordWrap;

            var hresult = _textDocument.Measure(content, alignment, baseStyle, formattedRanges, constraint, wordWrap,
                out var glyphRuns);
            RendererApi.IFC(new HRESULT(hresult.Int));

            var textFlow = new TextFlow();
            foreach (var glyphRun in glyphRuns)
                textFlow.Add(TextRun.FromGlyphRunInfo(glyphRun, _textDocument));
            return textFlow;
        }

        private static TextStyleInfo ToStyleInfo(TextStyle style)
        {
            if (style == null)
                return null;
            return new TextStyleInfo
            {
                FontFace = style.FontFace,
                FontSize = style.FontSize,
                AltFontSize = style.AltFontSize,
                Bold = style.Bold,
                Italic = style.Italic,
                Underline = style.Underline,
                Color = style.Color.RenderConvert(),
                HasColor = style.HasColor,
                LineSpacing = style.LineSpacing,
                CharacterSpacing = style.CharacterSpacing,
                EnableKerning = style.EnableKerning,
            };
        }

        // TextStyle.MarshalledData is plain pinned managed memory here (built
        // by TextMeasureParams.SetFormattedRangeStyle), not native/P-Invoke
        // state, so reading it back on any platform is safe.
        private static unsafe TextStyleInfo ToStyleInfo(in TextStyle.MarshalledData marshalled)
        {
            var flags = (TextStyle.SetFlags)marshalled._flags;
            return new TextStyleInfo
            {
                FontFace = marshalled._fontFace != null ? new string(marshalled._fontFace) : null,
                FontSize = marshalled._fontHeightPts,
                AltFontSize = flags.HasFlag(TextStyle.SetFlags.AltFontHeight) ? marshalled._altFontHeightPts : 0f,
                Bold = flags.HasFlag(TextStyle.SetFlags.BoldValue),
                Italic = flags.HasFlag(TextStyle.SetFlags.ItalicValue),
                Underline = flags.HasFlag(TextStyle.SetFlags.UnderlineValue),
                Color = marshalled._textColor.RenderConvert(),
                HasColor = flags.HasFlag(TextStyle.SetFlags.TextColor),
                LineSpacing = marshalled._lineSpacing,
                CharacterSpacing = marshalled._characterSpacing,
                EnableKerning = flags.HasFlag(TextStyle.SetFlags.EnableKerningValue),
            };
        }
#endif

        public void NotifyOfFocusChange(bool gainingFocus)
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextSetFocus(_rtoHandle, gainingFocus));
#else
                _focused = gainingFocus;
                if (_callbacks != null)
                {
                    if (gainingFocus)
                    {
                        UpdateCaretPosition();
                        _caretBlinkVisible = true;
                        RendererApi.IFC(_callbacks.ShowCaret(true));
                        SetTimer(CaretBlinkTimerId, (uint)Win32Api.GetCaretBlinkTime());
                    }
                    else
                    {
                        KillTimer(CaretBlinkTimerId);
                        RendererApi.IFC(_callbacks.ShowCaret(false));
                    }
                }
#endif
                if (gainingFocus)
                    return;
                _inImeCompositionMode = false;
            }
        }

        public void ScrollUp(ScrollbarType whichBar)
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextScroll(_rtoHandle, (int)whichBar, 0));
#endif
                // TODO: scrolling within a multi-line edit box isn't
                // implemented cross-platform yet - see SetScrollbars below.
            }
        }

        public void ScrollDown(ScrollbarType whichBar)
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextScroll(_rtoHandle, (int)whichBar, 1));
#endif
            }
        }

        public void PageUp(ScrollbarType whichBar)
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextScroll(_rtoHandle, (int)whichBar, 2));
#endif
            }
        }

        public void PageDown(ScrollbarType whichBar)
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextScroll(_rtoHandle, (int)whichBar, 3));
#endif
            }
        }

        public void ScrollToPosition(ScrollbarType whichBar, int whereTo)
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextScrollToPosition(_rtoHandle, (int)whichBar, whereTo));
#endif
            }
        }

        public void SetScrollbars(bool horizontalScrollbar, bool verticalScrollbar)
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextSetScrollbars(_rtoHandle, horizontalScrollbar, verticalScrollbar));
#endif
                // TODO: no scrollbar/viewport model exists for the
                // cross-platform hosted editor yet - a real implementation
                // needs RichText to know its own viewport size (currently
                // only supplied per-Measure-call as a wrapping constraint,
                // not tracked as a persistent viewport the way the native
                // RTO tracks it) to compute scroll extents. Deferred - see
                // logs/text-abstraction.md.
            }
        }

#if WINDOWS
        public bool ForwardKeyStateNotification(
          uint message,
          int virtualKey,
          int scanCode,
          int repeatCount,
          uint modifierState,
          ushort flags)
        {
            bool handled = true;
            lock (_lock)
            {
                if (!_inImeCompositionMode)
                    RendererApi.IFC(NativeApi.SpRichTextForwardKeyState(_rtoHandle, message, virtualKey, scanCode, repeatCount, modifierState, flags, out handled));
            }
            return handled;
        }

        public bool ForwardKeyCharacterNotification(
          uint message,
          int character,
          int scanCode,
          int repeatCount,
          uint modifierState,
          ushort flags)
        {
            bool handled = true;
            lock (_lock)
            {
                if (!_inImeCompositionMode)
                    RendererApi.IFC(NativeApi.SpRichTextForwardKeyCharacter(_rtoHandle, message, character, scanCode, repeatCount, modifierState, flags, out handled));
            }
            return handled;
        }

        public bool ForwardMouseInput(
          uint message,
          uint modifierState,
          int mouseButton,
          int x,
          int y,
          int mouseWheelDelta)
        {
            bool handled = true;
            lock (_lock)
            {
                if (!_inImeCompositionMode)
                    RendererApi.IFC(NativeApi.SpRichTextForwardMouseInput(_rtoHandle, message, modifierState, mouseButton, x, y, mouseWheelDelta, out handled));
            }
            return handled;
        }
#else
        // Win32 message IDs, kept numeric (not an enum) to match the
        // existing convention in this file/TextEditingHandler (which already
        // hardcodes WM_LBUTTONDOWN=513/WM_LBUTTONUP=514 and WM_IME_START/
        // ENDCOMPOSITION=269/270) - this codebase's whole input pipeline is
        // built around literal Win32 message numbers regardless of platform,
        // not just something native RichEdit needs.
        private const uint WM_KEYDOWN = 0x100;
        private const uint WM_LBUTTONDOWN = 0x201;
        private const uint WM_LBUTTONUP = 0x202;
        private const uint WM_MOUSEMOVE = 0x200;

        // Cross-platform counterpart of the block above: real keyboard-driven
        // caret movement/selection/editing backed by TextEditBuffer, in place
        // of forwarding to a native RichEdit control. Only reachable in
        // hosted mode (see the constructor - _editBuffer is null otherwise).
        public bool ForwardKeyStateNotification(
          uint message,
          int virtualKey,
          int scanCode,
          int repeatCount,
          uint modifierState,
          ushort flags)
        {
            lock (_lock)
            {
                if (_inImeCompositionMode || _editBuffer == null || message != WM_KEYDOWN)
                    return false;
                return HandleKeyDown((Keys)virtualKey, (InputModifiers)modifierState);
            }
        }

        public bool ForwardKeyCharacterNotification(
          uint message,
          int character,
          int scanCode,
          int repeatCount,
          uint modifierState,
          ushort flags)
        {
            lock (_lock)
            {
                if (_inImeCompositionMode || _editBuffer == null || _editBuffer.ReadOnly)
                    return false;
                var ch = (char)character;
                // Control characters other than the already-vetted Tab/Enter
                // (TextEditingHandler only forwards those when
                // AcceptsTab/AcceptsEnter accepted the preceding key-down) -
                // e.g. Backspace's char 8 - are handled via
                // ForwardKeyStateNotification instead, not inserted as text.
                if (char.IsControl(ch) && ch != '\r' && ch != '\t')
                    return false;

                if (_editBuffer.MaxLength is int max && _editBuffer.Length - _editBuffer.Selection.Length + 1 > max)
                {
                    RendererApi.IFC(_callbacks.MaxLengthExceeded());
                    return false;
                }

                _selectionAnchor = -1;
                _editBuffer.Insert(ch.ToString());
                return true;
            }
        }

        public bool ForwardMouseInput(
          uint message,
          uint modifierState,
          int mouseButton,
          int x,
          int y,
          int mouseWheelDelta)
        {
            lock (_lock)
            {
                if (_inImeCompositionMode || _editBuffer == null)
                    return false;

                switch (message)
                {
                    case WM_LBUTTONDOWN:
                        var downIndex = HitTestPoint(new Point(x, y));
                        _selectionAnchor = -1;
                        _dragAnchor = ((InputModifiers)modifierState & InputModifiers.ShiftKey) != 0 && !_editBuffer.Selection.IsEmpty
                            ? _editBuffer.Selection.Start
                            : downIndex;
                        _mouseSelecting = true;
                        _editBuffer.SetSelection(_dragAnchor, downIndex);
                        return true;
                    case WM_MOUSEMOVE:
                        if (!_mouseSelecting)
                            return false;
                        _editBuffer.SetSelection(_dragAnchor, HitTestPoint(new Point(x, y)));
                        return true;
                    case WM_LBUTTONUP:
                        _mouseSelecting = false;
                        return true;
                    default:
                        return false;
                }
            }
        }
#endif

        public HRESULT ForwardImeMessage(uint message, UIntPtr wParam, UIntPtr lParam)
        {
            lock (_lock)
            {
                switch (message)
                {
                    case 269:
                        _inImeCompositionMode = true;
                        break;
                    case 270:
                        _inImeCompositionMode = false;
                        break;
                }
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextForwardImeMessage(_rtoHandle, message, wParam, lParam));
#endif
                // TODO: IME composition isn't implemented cross-platform yet
                // - see Microsoft.Iris.Render.Text.Editing.IImeAdapter and
                // logs/text-abstraction.md.
            }
            return new HRESULT(0);
        }

        public bool CanUndo
        {
            get
            {
                lock (_lock)
                {
#if WINDOWS
                    RendererApi.IFC(NativeApi.SpRichTextCanUndo(_rtoHandle, out var canUndo));
                    return canUndo;
#else
                    return _editBuffer?.CanUndo ?? false;
#endif
                }
            }
        }

        public void Undo()
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextUndo(_rtoHandle));
#else
                _selectionAnchor = -1;
                _editBuffer?.Undo();
#endif
            }
        }

        public void Cut()
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextCut(_rtoHandle));
#else
                // No-op unless a Microsoft.Iris.Render.Text.Editing.IClipboardAdapter
                // is ever assigned to _editBuffer - none is on any platform
                // yet (see logs/text-abstraction.md), so this trims the
                // selection out of _editBuffer.Text without touching the
                // system clipboard.
                _selectionAnchor = -1;
                _editBuffer?.Cut();
#endif
            }
        }

        public void Copy()
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextCopy(_rtoHandle));
#else
                _editBuffer?.Copy();
#endif
            }
        }

        public void Paste()
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextPaste(_rtoHandle));
#else
                _selectionAnchor = -1;
                _editBuffer?.Paste();
#endif
            }
        }

        public void Delete()
        {
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextDelete(_rtoHandle));
#else
                _selectionAnchor = -1;
                _editBuffer?.Delete(TextEditBuffer.DeleteDirection.Forward);
#endif
            }
        }

#if !WINDOWS
        private bool HandleKeyDown(Keys key, InputModifiers modifiers)
        {
            bool shift = (modifiers & InputModifiers.ShiftKey) != 0;
            bool ctrl = (modifiers & InputModifiers.ControlKey) != 0;

            switch (key)
            {
                case Keys.Left:
                    MoveCaretHorizontal(-1, shift, ctrl);
                    return true;
                case Keys.Right:
                    MoveCaretHorizontal(1, shift, ctrl);
                    return true;
                case Keys.Up:
                    MoveCaretVertical(-1, shift);
                    return true;
                case Keys.Down:
                    MoveCaretVertical(1, shift);
                    return true;
                case Keys.Home:
                    MoveCaretToLineBoundary(true, shift);
                    return true;
                case Keys.End:
                    MoveCaretToLineBoundary(false, shift);
                    return true;
                case Keys.Back:
                    _selectionAnchor = -1;
                    _editBuffer.Delete(TextEditBuffer.DeleteDirection.Backward);
                    return true;
                case Keys.Delete:
                    _selectionAnchor = -1;
                    _editBuffer.Delete(TextEditBuffer.DeleteDirection.Forward);
                    return true;
                case Keys.Z when ctrl:
                    Undo();
                    return true;
                case Keys.Y when ctrl:
                    _editBuffer.Redo();
                    return true;
                case Keys.C when ctrl:
                    Copy();
                    return true;
                case Keys.X when ctrl:
                    Cut();
                    return true;
                case Keys.V when ctrl:
                    Paste();
                    return true;
                default:
                    return false;
            }
        }

        // Word-boundary/multi-line caret navigation (Ctrl+arrow, Up/Down,
        // Home/End) plus keyboard Shift-selection, all layered on top of
        // TextEditBuffer's plain SetSelection(start, end). _selectionAnchor
        // tracks the fixed end of an in-progress Shift+movement across
        // multiple key presses (TextSelection itself only stores a sorted
        // Start/End, not which end is the "anchor").
        private void MoveCaretHorizontal(int direction, bool extend, bool wordJump)
        {
            var sel = _editBuffer.Selection;
            var caret = _selectionAnchor >= 0 ? (sel.Start == _selectionAnchor ? sel.End : sel.Start) : sel.End;

            if (!extend)
            {
                _selectionAnchor = -1;
                var target = !sel.IsEmpty ? (direction < 0 ? sel.Start : sel.End) : ClampIndex(NextCaretIndex(caret, direction, wordJump));
                _editBuffer.SetSelection(target, target);
                return;
            }

            if (_selectionAnchor < 0)
                _selectionAnchor = caret;
            _editBuffer.SetSelection(_selectionAnchor, ClampIndex(NextCaretIndex(caret, direction, wordJump)));
        }

        private int NextCaretIndex(int from, int direction, bool wordJump)
        {
            if (!wordJump)
                return from + direction;

            var text = _editBuffer.Text;
            var i = from;
            if (direction < 0)
            {
                while (i > 0 && char.IsWhiteSpace(text[i - 1])) i--;
                while (i > 0 && !char.IsWhiteSpace(text[i - 1])) i--;
            }
            else
            {
                while (i < text.Length && char.IsWhiteSpace(text[i])) i++;
                while (i < text.Length && !char.IsWhiteSpace(text[i])) i++;
            }
            return i;
        }

        private int ClampIndex(int index) => Math.Clamp(index, 0, _editBuffer.Length);

        // Up/Down: real native RichEdit tracks visual line geometry
        // internally; here the caret's current pixel position is hit-tested
        // one line above/below via the same SixLaborsTextDocument layout
        // Measure() last produced (see _lastMeasure* fields) - "good enough,
        // functionally correct" rather than a byte-exact line model, matching
        // the precedent set by SixLaborsTextDocument's formatted-range
        // reconstruction (see logs/text-abstraction.md).
        private void MoveCaretVertical(int direction, bool extend)
        {
            var sel = _editBuffer.Selection;
            var caret = _selectionAnchor >= 0 ? (sel.Start == _selectionAnchor ? sel.End : sel.Start) : sel.End;
            var rect = GetCaretRect(caret);
            var target = HitTestPoint(new Point(rect.X, rect.Y + direction * Math.Max(1, rect.Height)));

            if (!extend)
            {
                _selectionAnchor = -1;
                _editBuffer.SetSelection(target, target);
                return;
            }

            if (_selectionAnchor < 0)
                _selectionAnchor = caret;
            _editBuffer.SetSelection(_selectionAnchor, target);
        }

        // Home/End move to the nearest logical ('\n'-delimited) paragraph
        // boundary, not the current wrapped visual line - SixLaborsTextDocument's
        // layout isn't queried for soft line breaks here. TODO: this is
        // wrong for Home/End inside a single word-wrapped paragraph; only
        // affects word-wrapping multi-line hosted RichText instances.
        private void MoveCaretToLineBoundary(bool home, bool extend)
        {
            var text = _editBuffer.Text;
            var sel = _editBuffer.Selection;
            var caret = _selectionAnchor >= 0 ? (sel.Start == _selectionAnchor ? sel.End : sel.Start) : sel.End;

            var target = caret;
            if (home)
            {
                while (target > 0 && text[target - 1] != '\n') target--;
            }
            else
            {
                while (target < text.Length && text[target] != '\n') target++;
            }

            if (!extend)
            {
                _selectionAnchor = -1;
                _editBuffer.SetSelection(target, target);
                return;
            }

            if (_selectionAnchor < 0)
                _selectionAnchor = caret;
            _editBuffer.SetSelection(_selectionAnchor, target);
        }

        private Rectangle GetCaretRect(int characterIndex)
        {
            RendererApi.IFC(new HRESULT(_textDocument.GetCaretMetrics(_editBuffer.Text, _lastMeasureStyle, _lastMeasureConstraint, _lastMeasureWordWrap, characterIndex, out var rect).Int));
            return rect;
        }

        private int HitTestPoint(Point point)
        {
            RendererApi.IFC(new HRESULT(_textDocument.HitTest(_editBuffer.Text, _lastMeasureStyle, _lastMeasureConstraint, _lastMeasureWordWrap, point, out var index).Int));
            return index;
        }

        private void UpdateCaretPosition()
        {
            if (_callbacks == null)
                return;
            var rect = GetCaretRect(_editBuffer.Selection.End);
            RendererApi.IFC(_callbacks.CreateCaret(1, Math.Max(1, rect.Height)));
            RendererApi.IFC(_callbacks.SetCaretPos(rect.X, rect.Y));
        }

        private void OnEditBufferTextChanged(object sender, EventArgs ea)
        {
            if (_callbacks == null)
                return;
            RendererApi.IFC(_callbacks.TextChanged());
            RendererApi.IFC(_callbacks.InvalidateContent());
        }

        private void OnEditBufferSelectionChanged(object sender, EventArgs ea)
        {
            if (_callbacks == null)
                return;
            var sel = _editBuffer.Selection;
            RendererApi.IFC(_callbacks.SelectionChanged(sel.Start, sel.End));
            if (!_focused)
                return;
            UpdateCaretPosition();
            _caretBlinkVisible = true;
            RendererApi.IFC(_callbacks.ShowCaret(sel.IsEmpty));
        }
#endif

        public static LineAlignment ReverseAlignment(
          LineAlignment alignment,
          bool condition)
        {
            if (condition)
            {
                switch (alignment)
                {
                    case LineAlignment.Near:
                        alignment = LineAlignment.Far;
                        break;
                    case LineAlignment.Far:
                        alignment = LineAlignment.Near;
                        break;
                }
            }
            return alignment;
        }

        internal static Dib Rasterize(
          IntPtr hGlyphRunInfo,
          bool outlineMode,
          Color textColor,
          bool shadowMode)
        {
            IntPtr phTextBitmap = IntPtr.Zero;
            IntPtr ppvBits;
            Size psizeBitmap;
            RendererApi.IFC(NativeApi.SpRichTextRasterize(hGlyphRunInfo, outlineMode ? 1 : 0, textColor, shadowMode ? 1 : 0, out phTextBitmap, out ppvBits, out psizeBitmap));
            return new Dib(phTextBitmap, ppvBits, psizeBitmap);
        }

        public void SetTimer(uint id, uint timeout)
        {
            DispatcherTimer dispatcherTimer = FindTimer(id);
            if (dispatcherTimer == null)
            {
                dispatcherTimer = new DispatcherTimer();
                _timers.Add(dispatcherTimer);
            }
            dispatcherTimer.UserData = id;
            dispatcherTimer.Interval = (int)timeout;
            dispatcherTimer.Tick += _timerTickHandler;
            dispatcherTimer.Start();
        }

        public void KillTimer(uint id)
        {
            DispatcherTimer timer = FindTimer(id);
            if (timer == null)
                return;
            DisposeTimer(timer);
            _timers.Remove(timer);
        }

        private DispatcherTimer FindTimer(uint id)
        {
            for (int index = 0; index < _timers.Count; ++index)
            {
                DispatcherTimer timer = (DispatcherTimer)_timers[index];
                if ((int)(uint)timer.UserData == (int)id)
                    return timer;
            }
            return null;
        }

        private void DisposeTimer(DispatcherTimer timer)
        {
            timer.Tick -= _timerTickHandler;
            timer.Stop();
        }

        private void OnTimerTick(object sender, EventArgs ea)
        {
            DispatcherTimer dispatcherTimer = (DispatcherTimer)sender;
            lock (_lock)
            {
#if WINDOWS
                RendererApi.IFC(NativeApi.SpRichTextOnTimerTick(_rtoHandle, (uint)dispatcherTimer.UserData));
#else
                // Native RichEdit drives its own caret blink internally and
                // reports it via IRichTextCallbacks.SetTimer/ShowCaret; here
                // RichText owns that logic instead (see NotifyOfFocusChange,
                // which is what starts this timer via SetTimer).
                if ((uint)dispatcherTimer.UserData == CaretBlinkTimerId && _callbacks != null && _focused)
                {
                    _caretBlinkVisible = !_caretBlinkVisible;
                    RendererApi.IFC(_callbacks.ShowCaret(_caretBlinkVisible && _editBuffer.Selection.IsEmpty));
                }
#endif
            }
        }

        private unsafe HRESULT OnReportRun(
          IntPtr hGlyphRunInfo,
          NativeApi.RasterizeRunPacket* runPacketPtr,
          IntPtr lpString,
          uint nChars,
          IntPtr dataPtr)
        {
            bool flag = false;
            if (_currentlyMeasuringText != null && _currentlyMeasuringText.Length == nChars)
            {
                flag = true;
                char* pointer = (char*)lpString.ToPointer();
                for (int index = 0; index < nChars; ++index)
                {
                    if (_currentlyMeasuringText[index] != pointer[index])
                    {
                        flag = false;
                        break;
                    }
                }
            }
            string content = flag ? _currentlyMeasuringText : Marshal.PtrToStringUni(lpString, (int)nChars);
            TextRun run = TextRun.FromRunPacket(hGlyphRunInfo, runPacketPtr, content);
            if (run != null)
                ((TextFlow)GCHandle.FromIntPtr(dataPtr).Target).Add(run);
            return new HRESULT(0);
        }
    }
}
