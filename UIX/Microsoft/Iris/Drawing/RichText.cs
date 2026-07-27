// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.RichText
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.Render.Text;
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
            // Interactive editing (hosted mode) still needs the native
            // RichEdit-style control - not yet implemented cross-platform.
            // See the "full cross-platform text engine" proposal in
            // logs/text-abstraction.md. Display-only (non-hosted) usage -
            // e.g. Text viewitem's shared word-wrapping/formatted-text
            // rasterizer - works cross-platform via SixLaborsTextDocument.
            if (_hosted)
                throw new PlatformNotSupportedException("Interactive rich-text editing (IRichTextCallbacks) requires the native Windows RichEdit backend, which is not yet implemented cross-platform. See logs/text-abstraction.md.");
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
                    RendererApi.IFC(NativeApi.SpRichTextSetMaximumLength(_rtoHandle, value));
            }
        }

        public bool DetectUrls
        {
            set
            {
                lock (_lock)
                    RendererApi.IFC(NativeApi.SpRichTextSetDetectUrls(_rtoHandle, value));
            }
        }

        public bool HasCallbacks => _hosted;

        public bool ReadOnly
        {
            set
            {
                lock (_lock)
                    RendererApi.IFC(NativeApi.SpRichTextSetReadOnly(_rtoHandle, value));
            }
        }

        public void SetWordWrap(bool wordWrap)
        {
            lock (_lock)
                RendererApi.IFC(NativeApi.SpRichTextSetWordWrap(_rtoHandle, wordWrap));
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
                RendererApi.IFC(NativeApi.SpRichTextSetSelectionRange(_rtoHandle, selectionStart, selectionEnd));
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
        // Only reachable in non-hosted (display-only) mode - see the
        // constructor - so measureParams' edit-mode-only fields
        // (SetEditMode/SetPasswordChar/TrimLeftSideBearing) are not honored
        // here; that's an accepted gap, not a silent behavior change, since
        // hosted mode already can't be constructed on this platform.
        public TextFlow Measure(string content, ref TextMeasureParams measureParams)
        {
            if (_hosted)
                throw new PlatformNotSupportedException("Interactive rich-text editing requires the native Windows RichEdit backend, which is not yet implemented cross-platform. See logs/text-abstraction.md.");

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

            RendererApi.IFC(new HRESULT(_textDocument.Measure(content, alignment, baseStyle, formattedRanges, constraint, wordWrap, out var glyphRuns).Int));

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
                RendererApi.IFC(NativeApi.SpRichTextSetFocus(_rtoHandle, gainingFocus));
                if (gainingFocus)
                    return;
                _inImeCompositionMode = false;
            }
        }

        public void ScrollUp(ScrollbarType whichBar)
        {
            lock (_lock)
                RendererApi.IFC(NativeApi.SpRichTextScroll(_rtoHandle, (int)whichBar, 0));
        }

        public void ScrollDown(ScrollbarType whichBar)
        {
            lock (_lock)
                RendererApi.IFC(NativeApi.SpRichTextScroll(_rtoHandle, (int)whichBar, 1));
        }

        public void PageUp(ScrollbarType whichBar)
        {
            lock (_lock)
                RendererApi.IFC(NativeApi.SpRichTextScroll(_rtoHandle, (int)whichBar, 2));
        }

        public void PageDown(ScrollbarType whichBar)
        {
            lock (_lock)
                RendererApi.IFC(NativeApi.SpRichTextScroll(_rtoHandle, (int)whichBar, 3));
        }

        public void ScrollToPosition(ScrollbarType whichBar, int whereTo)
        {
            lock (_lock)
                RendererApi.IFC(NativeApi.SpRichTextScrollToPosition(_rtoHandle, (int)whichBar, whereTo));
        }

        public void SetScrollbars(bool horizontalScrollbar, bool verticalScrollbar)
        {
            lock (_lock)
                RendererApi.IFC(NativeApi.SpRichTextSetScrollbars(_rtoHandle, horizontalScrollbar, verticalScrollbar));
        }

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
                RendererApi.IFC(NativeApi.SpRichTextForwardImeMessage(_rtoHandle, message, wParam, lParam));
            }
            return new HRESULT(0);
        }

        public bool CanUndo
        {
            get
            {
                bool canUndo;
                lock (_lock)
                    RendererApi.IFC(NativeApi.SpRichTextCanUndo(_rtoHandle, out canUndo));
                return canUndo;
            }
        }

        public void Undo()
        {
            lock (_lock)
                RendererApi.IFC(NativeApi.SpRichTextUndo(_rtoHandle));
        }

        public void Cut()
        {
            lock (_lock)
                RendererApi.IFC(NativeApi.SpRichTextCut(_rtoHandle));
        }

        public void Copy()
        {
            lock (_lock)
                RendererApi.IFC(NativeApi.SpRichTextCopy(_rtoHandle));
        }

        public void Paste()
        {
            lock (_lock)
                RendererApi.IFC(NativeApi.SpRichTextPaste(_rtoHandle));
        }

        public void Delete()
        {
            lock (_lock)
                RendererApi.IFC(NativeApi.SpRichTextDelete(_rtoHandle));
        }

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
                RendererApi.IFC(NativeApi.SpRichTextOnTimerTick(_rtoHandle, (uint)dispatcherTimer.UserData));
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
