// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.Text
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Drawing;
using Microsoft.Iris.InputHandlers;
using Microsoft.Iris.Layout;
using Microsoft.Iris.Layouts;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.Render.Extensions;
using Microsoft.Iris.RenderAPI.Drawing;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Microsoft.Iris.ViewItems
{
    internal class Text : ViewItem, ILayout
    {
        private const float c_scaleEpsilon = 0.01f;
        private const float c_lineSpacingDefault = 0.0f;
        private const float c_characterSpacingDefault = 0.0f;
        private const bool c_enableKerningDefault = false;
        private uint _bits;
        private TextFlow _flow;
        private float _lastLineExtentLeft;
        private float _lastLineExtentRight;
        private Size _textSize;
        private Size _slotSize;
        private int _lineAlignmentOffset;
        private string _content;
        private Font _font;
        private Color _textColor;
        private Color _textHighlightColor;
        private Color _backHighlightColor;
        private char _passwordChar;
        private float _scale;
        private LineAlignment _lineAlignment;
        private float _fadeSize;
        private int _maxLines;
        private int _lastVisibleRun;
        private TextBounds _boundsType;
        private bool _disableIme;
        private TextStyle _textStyle;
        private IDictionary _namedStyles;
        private RichText _richTextRasterizer;
        private TextEditingHandler _externalEditingHandler;
        private string _parsedContent;
        private ArrayList _parsedContentMarkedRanges;
        private ArrayList _fragments;
        private TextFlowRenderingHelper _renderingHelper;
        private static Font s_defaultFont = new Font("Arial", 24f);
        private static RichText s_sharedOversampledRasterizer;
        private static RichText s_sharedNonOversampledRasterizer;
        private static SimpleText s_sharedSimpleTextRasterizer;
        private static bool s_simpleTextMeasureAvailable;
        private Vector<float> _recentScaleChanges;
        private float _lineSpacing;
        private float _characterSpacing;
        private bool _enableKerning;
        private static char[] s_whitespaceChars = new char[3]
        {
            ' ',
            '\r',
            '\n'
        };

        public Text()
        {
            Layout = this;
            _font = s_defaultFont;
            _textColor = Color.Black;
            _textHighlightColor = Color.White;
            _backHighlightColor = Color.Black;
            _scale = 1f;
            _fadeSize = 32f;
            _maxLines = int.MaxValue;
            _passwordChar = '•';
            _lineAlignment = LineAlignment.Near;
            _richTextRasterizer = SharedNonOversampledRasterizer;
            _renderingHelper = new TextFlowRenderingHelper();
            ContributesToWidth = true;
            TextFitsWidth = true;
            TextFitsHeight = true;
            SetClipped(false);
            MarkScaleDirty();
            if (s_simpleTextMeasureAvailable)
                return;
            ClearBit(Bits.FastMeasurePossible);
            SetBit(Bits.FastMeasureValid);
        }

        public Text(UIClass ownerUI)
          : this()
        {
        }

        protected override void OnDispose()
        {
            DisposeFlow();
            UnregisterFragmentUsage();
            base.OnDispose();
        }

        private void DisposeFlow()
        {
            if (_flow == null)
                return;
            _flow.Dispose(this);
            _flow = null;
        }

        public static void Initialize()
        {
        }

        public static void Uninitialize()
        {
            if (s_sharedOversampledRasterizer != null)
            {
                s_sharedOversampledRasterizer.Dispose();
                s_sharedOversampledRasterizer = null;
            }
            if (s_sharedNonOversampledRasterizer != null)
            {
                s_sharedNonOversampledRasterizer.Dispose();
                s_sharedNonOversampledRasterizer = null;
            }
            if (s_sharedSimpleTextRasterizer == null)
                return;
            s_sharedSimpleTextRasterizer.Dispose();
            s_sharedSimpleTextRasterizer = null;
        }

        private static SimpleText SharedSimpleTextRasterizer
        {
            get
            {
                if (s_sharedSimpleTextRasterizer == null)
                    s_sharedSimpleTextRasterizer = new SimpleText();
                return s_sharedSimpleTextRasterizer;
            }
        }

        private static RichText SharedOversampledRasterizer
        {
            get
            {
                if (s_sharedOversampledRasterizer == null)
                {
                    s_sharedOversampledRasterizer = new RichText(true);
                    s_sharedOversampledRasterizer.Oversample = true;
                    s_simpleTextMeasureAvailable = NativeApi.SpSimpleTextIsAvailable();
                }
                return s_sharedOversampledRasterizer;
            }
        }

        private static RichText SharedNonOversampledRasterizer
        {
            get
            {
                if (s_sharedNonOversampledRasterizer == null)
                {
                    s_sharedNonOversampledRasterizer = new RichText(true);
                    s_simpleTextMeasureAvailable = NativeApi.SpSimpleTextIsAvailable();
                }
                return s_sharedNonOversampledRasterizer;
            }
        }

        public string Content
        {
            get => _content;
            set
            {
                if (!(_content != value))
                    return;
                if (value != null && value.Length > 3)
                {
                    _content = value.TrimEnd(s_whitespaceChars);
                    char ch1 = value[value.Length - 1];
                    char ch2 = value[value.Length - 2];
                    if (ch1 == ' ' && ch2 != ' ')
                        _content += ch1;
                }
                else
                    _content = value;
                _parsedContent = null;
                _parsedContentMarkedRanges = null;
                OnDisplayedContentChange();
                FireNotification(NotificationID.Content);
            }
        }

        public void MarkScaleDirty() => SetBit(Bits.ScaleDirty);

        private bool InMeasure
        {
            get => GetBit(Bits.InMeasure);
            set => SetBit(Bits.InMeasure, value);
        }

        public void OnDisplayedContentChange()
        {
            if (InMeasure)
                return;
            MarkTextLayoutInvalid();
            MarkPaintInvalid();
            ForceContentChange();
        }

        public Font Font
        {
            get => _font;
            set
            {
                if (_font == value)
                    return;
                _font = value;
                MarkPaintInvalid();
                MarkTextLayoutInvalid();
                ForceContentChange();
                FireNotification(NotificationID.Font);
            }
        }

        public Color Color
        {
            get => _textColor;
            set
            {
                if (!(_textColor != value))
                    return;
                _textColor = value;
                if (KeepFlowAlive)
                {
                    MarkPaintInvalid();
                }
                else
                {
                    MarkLayoutInvalid();
                    if (HasEverPainted)
                        KeepFlowAlive = true;
                }
                FireNotification(NotificationID.Color);
            }
        }

        public Color HighlightColor
        {
            get => _backHighlightColor;
            set
            {
                if (!(_backHighlightColor != value))
                    return;
                _backHighlightColor = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.HighlightColor);
            }
        }

        public Color TextHighlightColor
        {
            get => _textHighlightColor;
            set
            {
                if (!(TextHighlightColor != value))
                    return;
                _textHighlightColor = value;
                MarkPaintInvalid();
                FireNotification(NotificationID.TextHighlightColor);
            }
        }

        public TextSharpness TextSharpness
        {
            get => !_richTextRasterizer.Oversample ? TextSharpness.Sharp : TextSharpness.Soft;
            set
            {
                if (TextSharpness == value)
                    return;
                bool flag = false;
                switch (value)
                {
                    case TextSharpness.Sharp:
                        flag = false;
                        break;
                    case TextSharpness.Soft:
                        flag = true;
                        break;
                }
                if (_richTextRasterizer == s_sharedNonOversampledRasterizer && flag)
                    _richTextRasterizer = SharedOversampledRasterizer;
                else if (_richTextRasterizer == s_sharedOversampledRasterizer && !flag)
                    _richTextRasterizer = SharedNonOversampledRasterizer;
                else
                    _richTextRasterizer.Oversample = flag;
                MarkTextLayoutInvalid();
                ForceContentChange();
                FireNotification(NotificationID.TextSharpness);
            }
        }

        public bool WordWrap
        {
            get => GetBit(Bits.WordWrap);
            set
            {
                if (WordWrap == value)
                    return;
                SetBit(Bits.WordWrap, value);
                if (value)
                    KeepFlowAlive = true;
                MarkPaintInvalid();
                MarkTextLayoutInvalid();
                UpdateWordWrapForExternalRasterizer();
                FireNotification(NotificationID.WordWrap);
            }
        }

        public bool UsePasswordMask
        {
            get => GetBit(Bits.PasswordMasked);
            set
            {
                if (UsePasswordMask == value)
                    return;
                SetBit(Bits.PasswordMasked, value);
                MarkPaintInvalid();
                MarkTextLayoutInvalid();
                FireNotification(NotificationID.UsePasswordMask);
            }
        }

        public char PasswordMask
        {
            get => _passwordChar;
            set
            {
                if (_passwordChar == value)
                    return;
                _passwordChar = value;
                if (UsePasswordMask)
                {
                    MarkPaintInvalid();
                    MarkTextLayoutInvalid();
                }
                FireNotification(NotificationID.PasswordMask);
            }
        }

        public int MaximumLines
        {
            get => _maxLines;
            set
            {
                if (_maxLines == value)
                    return;
                _maxLines = value;
                MarkPaintInvalid();
                MarkTextLayoutInvalid();
                FireNotification(NotificationID.MaximumLines);
            }
        }

        public LineAlignment LineAlignment
        {
            get => _lineAlignment;
            set
            {
                if (_lineAlignment == value)
                    return;
                _lineAlignment = value;
                MarkPaintInvalid();
                MarkTextLayoutInvalid();
                FireNotification(NotificationID.LineAlignment);
            }
        }

        public float LineSpacing
        {
            get => !GetBit(Bits.LineSpacingSet) ? 0.0f : _lineSpacing;
            set
            {
                if (LineSpacing == (double)value)
                    return;
                _lineSpacing = value;
                SetBit(Bits.LineSpacingSet);
                MarkPaintInvalid();
                MarkTextLayoutInvalid();
                FireNotification(NotificationID.LineSpacing);
            }
        }

        public float CharacterSpacing
        {
            get => !GetBit(Bits.CharacterSpacingSet) ? 0.0f : _characterSpacing;
            set
            {
                if (CharacterSpacing == (double)value)
                    return;
                _characterSpacing = value;
                SetBit(Bits.CharacterSpacingSet);
                MarkPaintInvalid();
                MarkTextLayoutInvalid();
                FireNotification(NotificationID.CharacterSpacing);
            }
        }

        public bool EnableKerning
        {
            get => GetBit(Bits.EnableKerningSet) && _enableKerning;
            set
            {
                if (EnableKerning == value)
                    return;
                _enableKerning = value;
                SetBit(Bits.EnableKerningSet);
                MarkPaintInvalid();
                MarkTextLayoutInvalid();
                FireNotification(NotificationID.EnableKerning);
            }
        }

        public float FadeSize
        {
            get => _fadeSize;
            set
            {
                if (_fadeSize == (double)value)
                    return;
                _fadeSize = value;
                InvalidateGradients();
                FireNotification(NotificationID.FadeSize);
            }
        }

        public TextStyle Style
        {
            get => _textStyle;
            set
            {
                if (_textStyle == value)
                    return;
                _textStyle = value;
                MarkPaintInvalid();
                MarkTextLayoutInvalid();
                ForceContentChange();
                FireNotification(NotificationID.Style);
            }
        }

        public IDictionary NamedStyles
        {
            get => _namedStyles;
            set
            {
                if (_namedStyles == value)
                    return;
                _namedStyles = value;
                MarkPaintInvalid();
                MarkTextLayoutInvalid();
                ForceContentChange();
                FireNotification(NotificationID.NamedStyles);
            }
        }

        public IList Fragments => _fragments;

        public bool DisableIme
        {
            get => _disableIme;
            set
            {
                if (_disableIme == value)
                    return;
                _disableIme = value;
                FireNotification(NotificationID.DisableIme);
            }
        }

        public Rectangle LastLineBounds => _flow != null ? _flow.GetLastLineBounds(_lineAlignmentOffset) : new Rectangle(_textSize);

        public int NumberOfLines
        {
            get
            {
                if (_flow == null)
                    return 1;
                return _flow.Count < 1 ? 0 : _flow[_flow.Count - 1].Line;
            }
        }

        public int NumberOfVisibleLines
        {
            get
            {
                if (_flow == null)
                    return 1;
                return _flow.FirstFitRunOnFinalLine == null ? 0 : _flow.FirstFitRunOnFinalLine.Line;
            }
        }

        public bool ContributesToWidth
        {
            get => GetBit(Bits.ContributesToWidth);
            set
            {
                if (ContributesToWidth == value)
                    return;
                SetBit(Bits.ContributesToWidth, value);
                FireNotification(NotificationID.ContributesToWidth);
                MarkLayoutInvalid();
            }
        }

        public TextBounds BoundsType
        {
            get => _boundsType;
            set
            {
                if (_boundsType == value)
                    return;
                _boundsType = value;
                MarkLayoutInvalid();
                FireNotification(NotificationID.BoundsType);
            }
        }

        public bool Clipped => GetBit(Bits.Clipped);

        private void SetClipped(bool value)
        {
            if (GetBit(Bits.Clipped) == value)
                return;
            SetBit(Bits.Clipped, value);
            FireNotification(NotificationID.Clipped);
        }

        public RichText ExternalRasterizer
        {
            set
            {
                if (_richTextRasterizer == value)
                    return;
                bool oversample = _richTextRasterizer.Oversample;
                _richTextRasterizer = value;
                if (value != null)
                {
                    _richTextRasterizer.Oversample = oversample;
                    MarkScaleDirty();
                }
                else
                    _richTextRasterizer = !oversample ? SharedNonOversampledRasterizer : SharedOversampledRasterizer;
                MarkTextLayoutInvalid();
            }
        }

        public TextEditingHandler ExternalEditingHandler
        {
            set
            {
                if (_externalEditingHandler == value)
                    return;
                _externalEditingHandler = value;
                UpdateWordWrapForExternalRasterizer();
            }
        }

        private void UpdateWordWrapForExternalRasterizer()
        {
            if (_externalEditingHandler == null)
                return;
            _externalEditingHandler.WordWrap = WordWrap;
        }

        public ItemAlignment DefaultChildAlignment => ItemAlignment.Default;

        public bool IsViewDependent(ViewItem node) => GetBit(Bits.ViewDependent);

        public void GetInitialChildrenRequests(out int more) => more = 0;

        Size ILayout.Measure(ILayoutNode layoutNode, Size constraint)
        {
            Size zero = Size.Zero;
            InMeasure = true;
            LineAlignment alignment = RichText.ReverseAlignment(_lineAlignment, UISession.IsRtl);
            if (!TextLayoutInvalid && _flow != null && (!WordWrap && alignment == LineAlignment.Near) && (TextFitsWidth && constraint.Width >= _textSize.Width && (TextFitsHeight && constraint.Height >= _textSize.Height)))
            {
                Size size = Size.Min(_textSize, constraint);
                InMeasure = false;
                return size;
            }
            int boundingWidth = constraint.Width;
            if (!ContributesToWidth)
            {
                boundingWidth = 16777215;
                layoutNode.LayoutContributesToWidth = false;
            }
            MeasureText(boundingWidth, constraint.Height, alignment);
            if (alignment != LineAlignment.Near && _flow.Bounds.Width < boundingWidth && _flow.Bounds.Height >= 0)
                MeasureText(_flow.Bounds.Width, _flow.Bounds.Height, alignment);
            bool flag1 = false;
            bool flag2 = false;
            if (_flow.Count > 0)
            {
                Size naturalSize = GetNaturalSize();
                flag1 = naturalSize.Width > boundingWidth;
                flag2 = naturalSize.Height > constraint.Height;
            }
            int x = 0;
            int y = 0;
            if (_flow.Bounds.Left < 0)
                x = -_flow.Bounds.Left;
            if (_flow.Bounds.Top < 0)
                y = -_flow.Bounds.Top;
            Point point1 = new Point(x, y);
            ClipToHeight = false;
            int num1 = -1;
            int num2 = 0;
            int val1_1 = 0;
            int val1_2 = 0;
            int num3 = MaximumLines;
            if (num3 == 0)
                num3 = int.MaxValue;
            int index;
            for (index = 0; index < _flow.Count; ++index)
            {
                TextRun run = _flow[index];
                if (run.Line > num3)
                {
                    flag2 = true;
                    break;
                }
                if (BoundsType != TextBounds.Full)
                {
                    if (run.Line != num1)
                    {
                        num2 += val1_1 + val1_2;
                        val1_1 = 0;
                        val1_2 = 0;
                        num1 = run.Line;
                    }
                    Point offsetPoint = point1;
                    offsetPoint.Y -= num2;
                    if ((BoundsType & TextBounds.AlignToAscender) != TextBounds.Full)
                    {
                        val1_1 = Math.Max(val1_1, run.AscenderInset);
                        offsetPoint.Y -= run.AscenderInset;
                    }
                    if ((BoundsType & TextBounds.AlignToBaseline) != TextBounds.Full)
                        val1_2 = Math.Max(val1_2, run.BaselineInset);
                    run.ApplyOffset(offsetPoint);
                }
                int num4 = (BoundsType & TextBounds.AlignToBaseline) != TextBounds.Full ? run.BaselineInset : 0;
                if (run.LayoutBounds.Bottom - num4 > constraint.Height)
                {
                    if (run.Line == 1)
                        ClipToHeight = true;
                    else
                        break;
                }
                _flow.AddFit(run);
            }
            _lastVisibleRun = index - 1;
            TextFitsWidth = WordWrap || !flag1;
            TextFitsHeight = !flag2;
            if (!TextFitsHeight && _flow.HasVisibleRuns)
            {
                _lastLineExtentLeft = _flow.FirstFitRunOnFinalLine.LayoutBounds.Left;
                _lastLineExtentRight = _flow.LastFitRun.LayoutBounds.Right;
            }
            else
                _lastLineExtentLeft = _lastLineExtentRight = 0.0f;
            InvalidateGradients();
            if (UpdateFragmentsAfterLayout)
                ((ViewItem)layoutNode).MarkLayoutOutputDirty(true);
            Point point2 = new Point(_flow.Bounds.Location.X + point1.X, _flow.Bounds.Location.Y + point1.Y);
            _textSize = new Size(_flow.Bounds.Width + point2.X, _flow.FitBounds.Height + point2.Y - (val1_1 + val1_2));
            Size constraint1 = Size.Min(_textSize, constraint);
            DefaultLayout.Measure(layoutNode, constraint1);
            FireNotification(NotificationID.LastLineBounds);
            InMeasure = false;
            return constraint1;
        }

        void ILayout.Arrange(ILayoutNode layoutNode, LayoutSlot slot)
        {
            _slotSize = slot.Bounds;
            if (!ContributesToWidth)
                TextFitsWidth = _textSize.Width <= slot.Bounds.Width;
            _lineAlignmentOffset = 0;
            if (TextFitsWidth)
            {
                if (LineAlignment == LineAlignment.Center)
                    _lineAlignmentOffset = (_slotSize.Width - _textSize.Width) / 2;
                else if (LineAlignment == LineAlignment.Far)
                    _lineAlignmentOffset = _slotSize.Width - _textSize.Width;
            }
            bool flag1 = false;
            if (_flow != null)
            {
                Rectangle view = slot.View;
                Size size = view.Size;
                bool flag2 = _textSize.Width > size.Width || _textSize.Height > size.Height;
                flag1 = ((flag1 ? 1 : 0) | (!KeepFlowAlive ? 0 : (flag2 ? 1 : 0))) != 0;
                _flow.ResetVisibleTracking();
                if (flag1)
                {
                    bool flag3 = false;
                    for (int index = 0; index <= _lastVisibleRun; ++index)
                    {
                        TextRun textRun = _flow[index];
                        bool flag4 = !textRun.LayoutBounds.IsEmpty && textRun.LayoutBounds.IntersectsWith(view);
                        if (!flag3 && textRun.Visible != flag4)
                        {
                            MarkPaintInvalid();
                            UpdateFragmentsAfterLayout = true;
                            flag3 = true;
                        }
                        if (flag4)
                            _flow.AddVisible(index);
                        else
                            textRun.Visible = false;
                    }
                }
                else
                {
                    for (int index = 0; index <= _lastVisibleRun; ++index)
                    {
                        TextRun textRun = _flow[index];
                        if (!textRun.LayoutBounds.IsEmpty)
                        {
                            if (!textRun.Visible)
                                MarkPaintInvalid();
                            _flow.AddVisible(index);
                        }
                    }
                }
            }
            KeepFlowAlive |= flag1;
            SetBit(Bits.ViewDependent, flag1);
            DefaultLayout.Arrange(layoutNode, slot);
        }

        private Size GetNaturalSize()
        {
            if (!FastMeasurePossible)
                return _richTextRasterizer.GetNaturalBounds();
            return _flow.Count == 1 ? _flow[0].NaturalExtent : Size.Zero;
        }

        private void MeasureText(int boundingWidth, int boundingHeight, LineAlignment alignment)
        {
            if (FastMeasurePossible)
                DoFastMeasure(boundingWidth, boundingHeight, alignment);
            else
                DoRichEditMeasure(boundingWidth, boundingHeight, alignment);
        }

        private void DoFastMeasure(int boundingWidth, int boundingHeight, LineAlignment alignment)
        {
            TextStyle effectiveTextStyle = GetEffectiveTextStyle();
            Size constraint = new Size(boundingWidth, boundingHeight);
            DisposeFlow();
            _flow = SharedSimpleTextRasterizer.Measure(_content, alignment, effectiveTextStyle, constraint);
            _flow.DeclareOwner(this);
        }

        private void DoRichEditMeasure(int boundingWidth, int boundingHeight, LineAlignment alignment)
        {
            TextMeasureParams measureParams = new TextMeasureParams();
            measureParams.Initialize();
            float width = Math.Min(boundingWidth, 4095f);
            float height = Math.Min(boundingHeight, 8191f);
            measureParams.SetConstraint(new SizeF(width, height));
            TextStyle effectiveTextStyle = GetEffectiveTextStyle();
            string empty = string.Empty;
            string content;
            if (!UsedForEditing)
            {
                content = Content;
                if (_namedStyles != null)
                {
                    if (_parsedContent == null && content != null)
                    {
                        _parsedContentMarkedRanges = new ArrayList();
                        _parsedContent = ParseMarkedUpText(content, _parsedContentMarkedRanges);
                    }
                    content = _parsedContent;
                }
                measureParams.SetWordWrap(WordWrap);
            }
            else
                content = _richTextRasterizer.SimpleContent;
            measureParams.SetEditMode(UsedForEditing);
            if (UsePasswordMask)
                measureParams.SetPasswordChar(_passwordChar);
            measureParams.SetFormat(alignment, effectiveTextStyle);
            if ((_boundsType & TextBounds.TrimLeftSideBearing) != TextBounds.Full && _lineAlignment == LineAlignment.Near)
                measureParams.TrimLeftSideBearing();
            if (!UsedForEditing || GetBit(Bits.ScaleDirty))
            {
                ClearBit(Bits.ScaleDirty);
                measureParams.SetScale(_scale);
            }
            if (_parsedContent != null)
                ApplyContentFormatting(ref measureParams);
            DisposeFlow();
            _flow = _richTextRasterizer.Measure(content, ref measureParams);
            measureParams.Dispose();
            _flow.DeclareOwner(this);
            UpdateFragmentsAfterLayout = true;
        }

        private bool FastMeasurePossible
        {
            get
            {
                if (!GetBit(Bits.FastMeasureValid))
                {
                    bool flag = UsingSharedRasterizer && !WordWrap && (_namedStyles == null && _scale == 1.0) && (TextSharpness == TextSharpness.Sharp && !UsePasswordMask) && !Zone.Session.IsRtl;
                    if (flag)
                        flag = SharedSimpleTextRasterizer.CanMeasure(Content, GetEffectiveTextStyle());
                    SetBit(Bits.FastMeasurePossible, flag);
                    SetBit(Bits.FastMeasureValid);
                }
                return GetBit(Bits.FastMeasurePossible);
            }
        }

        internal TextStyle GetEffectiveTextStyle()
        {
            TextStyle textStyle = new TextStyle();
            Font font = _font ?? s_defaultFont;
            textStyle.FontFace = font.FontName;
            textStyle.FontSize = font.FontSize;
            if (font.AltFontSize != (double)font.FontSize)
                textStyle.AltFontSize = font.AltFontSize;
            if ((font.FontStyle & FontStyles.Bold) != FontStyles.None)
                textStyle.Bold = true;
            if ((font.FontStyle & FontStyles.Italic) != FontStyles.None)
                textStyle.Italic = true;
            if ((font.FontStyle & FontStyles.Underline) != FontStyles.None)
                textStyle.Underline = true;
            textStyle.Color = _textColor;
            if (GetBit(Bits.LineSpacingSet))
                textStyle.LineSpacing = LineSpacing;
            if (GetBit(Bits.CharacterSpacingSet))
                textStyle.CharacterSpacing = CharacterSpacing;
            if (GetBit(Bits.EnableKerningSet))
                textStyle.EnableKerning = EnableKerning;
            if (_textStyle != null)
                textStyle.Add(_textStyle);
            return textStyle;
        }

        protected override void OnLayoutComplete(ViewItem sender)
        {
            ArrayList arrayList = null;
            if (UpdateFragmentsAfterLayout)
            {
                if (_namedStyles != null)
                    arrayList = AnnotateFragments();
                bool flag = false;
                if (_fragments != null || arrayList != null)
                    flag = TextLayoutInvalid || !AreFragmentListsEquivalent(_fragments, arrayList);
                if (flag)
                {
                    UnregisterFragmentUsage();
                    _fragments = arrayList;
                    RegisterFragmentUsage();
                    FireNotification(NotificationID.Fragments);
                }
                else if (_fragments != null)
                {
                    for (int index = 0; index < _fragments.Count; ++index)
                        ((TextFragment)_fragments[index]).NotifyPaintInvalid();
                }
                ResetMarkedRanges();
                UpdateFragmentsAfterLayout = false;
            }
            SetClipped(!TextFitsWidth || !TextFitsHeight);
            TextLayoutInvalid = false;
            base.OnLayoutComplete(sender);
        }

        private static bool AreFragmentListsEquivalent(IList lhsFragments, IList rhsFragments)
        {
            int num1 = lhsFragments != null ? lhsFragments.Count : 0;
            int num2 = rhsFragments != null ? rhsFragments.Count : 0;
            if (num1 != num2)
                return false;
            for (int index = 0; index < num1; ++index)
            {
                if (!((TextFragment)lhsFragments[index]).IsLayoutEquivalentTo((TextFragment)rhsFragments[index]))
                    return false;
            }
            return true;
        }

        private void ApplyContentFormatting(ref TextMeasureParams measureParams)
        {
            if (_parsedContentMarkedRanges == null || _namedStyles == null)
                return;
            if (!UsingSharedRasterizer)
            {
                ErrorManager.ReportError("Text: Complex formatting unsupported on text that is editable");
            }
            else
            {
                measureParams.AllocateFormattedRanges(_parsedContentMarkedRanges.Count, _namedStyles.Count);
                TextStyle[] array = new TextStyle[_namedStyles.Count];
                int index1 = 0;
                foreach (TextStyle style in _namedStyles.Values)
                {
                    measureParams.SetFormattedRangeStyle(index1, style);
                    array[index1] = style;
                    ++index1;
                }
                TextMeasureParams.FormattedRange[] formattedRanges = measureParams.FormattedRanges;
                for (int index2 = 0; index2 < _parsedContentMarkedRanges.Count; ++index2)
                {
                    Microsoft.Iris.ViewItems.Text.MarkedRange contentMarkedRange = (Microsoft.Iris.ViewItems.Text.MarkedRange)_parsedContentMarkedRanges[index2];
                    if (_namedStyles.Contains(contentMarkedRange.tagName))
                    {
                        TextStyle namedStyle = _namedStyles[contentMarkedRange.tagName] as TextStyle;
                        contentMarkedRange.cachedStyle = namedStyle;
                        if (namedStyle != null)
                        {
                            formattedRanges[index2].FirstCharacter = contentMarkedRange.firstCharacter;
                            formattedRanges[index2].LastCharacter = contentMarkedRange.lastCharacter;
                            formattedRanges[index2].Color = contentMarkedRange.RangeIDAsColor;
                            formattedRanges[index2].StyleIndex = Array.IndexOf<TextStyle>(array, namedStyle);
                        }
                    }
                    else
                        contentMarkedRange.cachedStyle = null;
                }
            }
        }

        private ArrayList AnnotateFragments()
        {
            ArrayList arrayList = null;
            if (_parsedContentMarkedRanges != null && _flow != null)
            {
                for (int firstVisibleIndex = _flow.FirstVisibleIndex; firstVisibleIndex <= _flow.LastVisibleIndex; ++firstVisibleIndex)
                {
                    TextRun textRun = _flow[firstVisibleIndex];
                    textRun.IsFragment = false;
                    if (textRun.RunColor.A != byte.MaxValue)
                    {
                        Microsoft.Iris.ViewItems.Text.MarkedRange markedRange = null;
                        for (int index = 0; index < _parsedContentMarkedRanges.Count; ++index)
                        {
                            Microsoft.Iris.ViewItems.Text.MarkedRange contentMarkedRange = (Microsoft.Iris.ViewItems.Text.MarkedRange)_parsedContentMarkedRanges[index];
                            if (contentMarkedRange.RangeIDAsColor == textRun.RunColor)
                            {
                                textRun.OverrideColor = contentMarkedRange.GetEffectiveColor(Color.Transparent);
                                markedRange = contentMarkedRange;
                                break;
                            }
                        }
                        if (markedRange != null && markedRange.IsInFragment)
                        {
                            textRun.IsFragment = true;
                            if (markedRange.fragment == null)
                            {
                                markedRange.fragment = new TextFragment(markedRange.tagName, markedRange.attributes, this);
                                if (arrayList == null)
                                    arrayList = new ArrayList();
                                arrayList.Add(markedRange.fragment);
                            }
                            markedRange.fragment.InternalRuns.Add(new TextRunData(textRun, IsOnLastLine(textRun), this, _lineAlignmentOffset));
                        }
                    }
                }
            }
            return arrayList;
        }

        private void RegisterFragmentUsage()
        {
            if (_fragments == null)
                return;
            foreach (TextFragment fragment in _fragments)
            {
                if (fragment.Runs != null)
                {
                    foreach (TextRunData run in fragment.Runs)
                        run.Run.RegisterUsage(this);
                }
            }
        }

        private void UnregisterFragmentUsage()
        {
            if (_fragments == null)
                return;
            foreach (TextFragment fragment in _fragments)
            {
                if (fragment.Runs != null)
                {
                    foreach (TextRunData run in fragment.Runs)
                        run.Run.UnregisterUsage(this);
                }
            }
            _fragments = null;
        }

        private void ResetMarkedRanges()
        {
            if (_parsedContentMarkedRanges == null)
                return;
            for (int index = 0; index < _parsedContentMarkedRanges.Count; ++index)
                ((Microsoft.Iris.ViewItems.Text.MarkedRange)_parsedContentMarkedRanges[index]).fragment = null;
        }

        private static string ParseMarkedUpText(string content, ArrayList markedRanges)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ArrayList arrayList = new ArrayList();
            uint num = 0;
            try
            {
                using (ManagedXmlReader nativeXmlReader = new ManagedXmlReader(content, true))
                {
                    MarkedRange markedRange1 = null;
                    XmlNodeType nodeType;
                    while (nativeXmlReader.Read(out nodeType))
                    {
                        switch (nodeType)
                        {
                            case XmlNodeType.Element:
                                if (!nativeXmlReader.IsEmptyElement)
                                {
                                    string name = nativeXmlReader.Name;
                                    MarkedRange markedRange2 = new MarkedRange();
                                    markedRange2.tagName = name;
                                    markedRange2.firstCharacter = stringBuilder.Length;
                                    markedRange2.lastCharacter = int.MaxValue;
                                    markedRange2.rangeID = ++num;
                                    arrayList.Add(markedRange2);
                                    markedRanges.Add(markedRange2);
                                    markedRange2.parentRange = markedRange1;
                                    markedRange1 = markedRange2;
                                    while (nativeXmlReader.ReadAttribute())
                                    {
                                        if (markedRange1.attributes == null)
                                            markedRange1.attributes = new Dictionary<object, object>();
                                        markedRange1.attributes[nativeXmlReader.Name] = nativeXmlReader.Value;
                                    }
                                    continue;
                                }
                                continue;
                            case XmlNodeType.Text:
                            case XmlNodeType.CDATA:
                            case XmlNodeType.Whitespace:
                                string str = nativeXmlReader.Value;
                                if (str.IndexOf("\r\n", StringComparison.Ordinal) >= 0)
                                    str = str.Replace("\r\n", "\r");
                                stringBuilder.Append(str);
                                continue;
                            case XmlNodeType.EndElement:
                                string name1 = nativeXmlReader.Name;
                                for (int index = arrayList.Count - 1; index >= 0; --index)
                                {
                                    MarkedRange markedRange2 = (MarkedRange)arrayList[index];
                                    markedRange2.lastCharacter = stringBuilder.Length;
                                    arrayList.RemoveAt(index);
                                    if (markedRange2.tagName == name1)
                                        break;
                                }
                                if (markedRange1 != null)
                                {
                                    markedRange1 = markedRange1.parentRange;
                                    continue;
                                }
                                continue;
                            default:
                                continue;
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                markedRanges.Clear();
                stringBuilder = null;
            }
            if (stringBuilder == null)
                return content;
            return stringBuilder.ToString();
        }

        public void CreateFadeGradientsHelper(
          ref IGradient gradientClipLeftRight,
          ref IGradient gradientMultiLine)
        {
            bool flag1 = true;
            if (TextFitsWidth && TextFitsHeight)
                flag1 = false;
            float fadeSize = FadeSize;
            if (!flag1 || fadeSize <= 0.0)
                return;
            if (!WordWrap)
            {
                if (TextFitsWidth)
                    return;
                float num = 0.0f;
                float flPosition = 0.0f;
                LineAlignment lineAlignment = LineAlignment;
                if (lineAlignment == LineAlignment.Center)
                {
                    flPosition = num = fadeSize;
                }
                else
                {
                    bool flag2 = lineAlignment == LineAlignment.Near;
                    if (UISession.IsRtl)
                        flag2 = !flag2;
                    if (flag2)
                        num = fadeSize;
                    else
                        flPosition = fadeSize;
                }
                IGradient gradient = UISession.RenderSession.CreateGradient(this);
                gradient.Orientation = Orientation.Horizontal;
                if (flPosition > 0.0)
                {
                    gradient.AddValue(-1f, 0.0f, RelativeSpace.Min);
                    gradient.AddValue(flPosition, 1f, RelativeSpace.Min);
                }
                if (num > 0.0)
                {
                    gradient.AddValue(_slotSize.Width - num, 1f, RelativeSpace.Min);
                    gradient.AddValue(_slotSize.Width + 1, 0.0f, RelativeSpace.Min);
                }
                gradientClipLeftRight = gradient;
            }
            else
            {
                if (TextFitsHeight)
                    return;
                float flPosition1 = 0.0f;
                float flPosition2 = 0.0f;
                if (!UISession.IsRtl)
                {
                    if (_flow.LastFitRun != null)
                    {
                        flPosition2 = _flow.LastFitRun.LayoutBounds.Width;
                        flPosition1 = flPosition2 - fadeSize;
                    }
                }
                else
                {
                    flPosition2 = 0.0f;
                    flPosition1 = flPosition2 + fadeSize;
                }
                IGradient gradient = UISession.RenderSession.CreateGradient(this);
                gradient.ColorMask = new ColorF(byte.MaxValue, 0, 0, 0);
                gradient.Orientation = Orientation.Horizontal;
                gradient.AddValue(flPosition1, 1f, RelativeSpace.Min);
                gradient.AddValue(flPosition2, 0.0f, RelativeSpace.Min);
                gradientMultiLine = gradient;
            }
        }

        private bool UsingSharedRasterizer => !_richTextRasterizer.HasCallbacks;

        private bool UsedForEditing => _externalEditingHandler != null;

        private void ResetCachedScaleState()
        {
            IgnoreEffectiveScaleChanges = false;
            _recentScaleChanges = null;
        }

        private void CreateVisuals(IVisualContainer topVisual, IRenderSession renderSession)
        {
            VisualOrder nOrder = VisualOrder.First;
            IGradient gradientClipLeftRight = null;
            IGradient gradientMultiLine = null;
            CreateFadeGradientsHelper(ref gradientClipLeftRight, ref gradientMultiLine);
            TextRun textRun = UISession.IsRtl ? _flow.FirstFitRunOnFinalLine : _flow.LastFitRun;
            for (int firstVisibleIndex = _flow.FirstVisibleIndex; firstVisibleIndex <= _flow.LastVisibleIndex; ++firstVisibleIndex)
            {
                TextRun run = _flow[firstVisibleIndex];
                if (run.Visible && !run.IsFragment)
                {
                    Color effectiveColor = GetEffectiveColor(run);
                    IImage imageForRun = GetImageForRun(UISession, run, effectiveColor);
                    if (imageForRun != null)
                    {
                        float x = run.RenderBounds.Left + _lineAlignmentOffset;
                        if (run.Highlighted)
                        {
                            RectangleF lineBound = (RectangleF)_flow.LineBounds[run.Line - 1];
                            ISprite sprite = renderSession.CreateSprite(this, this);
                            sprite.Effect = EffectManager.CreateColorFillEffect(this, _backHighlightColor);
                            sprite.Effect.UnregisterUsage(this);
                            sprite.Position = new Vector3(x, lineBound.Top, 0.0f);
                            sprite.Size = new Vector2(run.RenderBounds.Width, lineBound.Height);
                            topVisual.AddChild(sprite, null, nOrder);
                            sprite.UnregisterUsage(this);
                            run.HighlightSprite = sprite;
                        }
                        ISprite sprite1 = renderSession.CreateSprite(this, this);
                        sprite1.Effect = EffectClass.CreateImageRenderEffectWithFallback(Effect, this, imageForRun);
                        sprite1.Effect.UnregisterUsage(this);
                        sprite1.Position = new Vector3(x, run.RenderBounds.Top, 0.0f);
                        sprite1.Size = new Vector2(run.RenderBounds.Width, run.RenderBounds.Height);
                        if (gradientMultiLine != null && run == textRun)
                            sprite1.AddGradient(gradientMultiLine);
                        topVisual.AddChild(sprite1, null, nOrder);
                        sprite1.UnregisterUsage(this);
                        run.TextSprite = sprite1;
                    }
                }
            }
            topVisual.RemoveAllGradients();
            if (gradientClipLeftRight != null)
                topVisual.AddGradient(gradientClipLeftRight);
            gradientClipLeftRight?.UnregisterUsage(this);
            gradientMultiLine?.UnregisterUsage(this);
        }

        private Color GetEffectiveColor(TextRun run)
        {
            Color color = _textColor;
            if (run.Highlighted)
                color = _textHighlightColor;
            else if (run.OverrideColor != Color.Transparent)
                color = run.OverrideColor;
            else if (run.Link && _externalEditingHandler != null && _externalEditingHandler.LinkColor != Color.Transparent)
                color = _externalEditingHandler.LinkColor;
            else if (_textStyle != null && _textStyle.HasColor)
                color = _textStyle.Color;
            return color;
        }

        protected override void DisposeAllContent()
        {
            base.DisposeAllContent();
            DisposeContent(true);
        }

        protected virtual void DisposeContent(bool removeFromTree)
        {
            if (removeFromTree)
                VisualContainer.RemoveAllChildren();
            Effect?.DoneWithRenderEffects(this);
            if (_flow == null)
                return;
            _flow.ClearSprites();
        }

        protected override void OnPaint(bool visible)
        {
            DisposeAllContent();
            base.OnPaint(visible);
            ResetCachedScaleState();
            if (_flow == null)
            {
                if (_content == null && UsingSharedRasterizer)
                    return;
                MarkTextLayoutInvalid();
            }
            else
            {
                CreateVisuals(VisualContainer, UISession.RenderSession);
                if (!KeepFlowAlive)
                    DisposeFlow();
                HasEverPainted = true;
            }
        }

        private bool IsOnLastLine(TextRun run) => _flow.IsOnLastLine(run);

        private void InvalidateGradients()
        {
            _renderingHelper.InvalidateGradients();
            MarkPaintInvalid();
        }

        protected override void OnEffectiveScaleChange()
        {
            float y = ComputeEffectiveScale().Y;
            if (!ScaleDifferenceIsGreaterThanThreshold(y, _scale))
                return;
            Vector<float> vector = _recentScaleChanges;
            if (!IgnoreEffectiveScaleChanges && vector != null)
            {
                foreach (float newScale in vector)
                {
                    if (!ScaleDifferenceIsGreaterThanThreshold(y, newScale))
                    {
                        IgnoreEffectiveScaleChanges = true;
                        break;
                    }
                }
            }
            if (IgnoreEffectiveScaleChanges)
                return;
            if (vector == null)
                vector = new Vector<float>(4);
            vector.Add(y);
            _recentScaleChanges = vector;
            MarkTextLayoutInvalid();
            _scale = y;
            MarkScaleDirty();
        }

        private bool ScaleDifferenceIsGreaterThanThreshold(float oldScale, float newScale) => Math.Abs(oldScale - newScale) > 0.00999999977648258;

        private void MarkTextLayoutInvalid()
        {
            TextLayoutInvalid = true;
            if (s_simpleTextMeasureAvailable)
                ClearBit(Bits.FastMeasureValid);
            DisposeFlow();
            MarkLayoutInvalid();
        }

        internal static IImage GetImageForRun(UISession session, TextRun run, Color textColor)
        {
            if (string.IsNullOrEmpty(run.Content))
                return null;
            string str = "aa";
            bool flag = false;
            RichTextInfoKey richTextInfoKey = new RichTextInfoKey(run, str, flag, textColor);
            ImageCache instance = TextImageCache.Instance;
            ImageCacheItem imageCacheItem = instance.Lookup(richTextInfoKey);
            if (imageCacheItem == null)
            {
                imageCacheItem = new TextImageItem(session.RenderSession, run, str, flag, textColor);
                instance.Add(richTextInfoKey, imageCacheItem);
            }
            return imageCacheItem.RenderImage;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.ToString());
            if (_content != null)
            {
                stringBuilder.Append(" (Content=\"");
                stringBuilder.Append(_content);
                stringBuilder.Append("\")");
            }
            return stringBuilder.ToString();
        }

        private bool TextFitsWidth
        {
            get => GetBit(Bits.TextFitsWidth);
            set => SetBit(Bits.TextFitsWidth, value);
        }

        private bool TextFitsHeight
        {
            get => GetBit(Bits.TextFitsHeight);
            set => SetBit(Bits.TextFitsHeight, value);
        }

        private bool ClipToHeight
        {
            get => GetBit(Bits.ClipToHeight);
            set => SetBit(Bits.ClipToHeight, value);
        }

        private bool KeepFlowAlive
        {
            get => GetBit(Bits.KeepFlowAlive);
            set => SetBit(Bits.KeepFlowAlive, value);
        }

        private bool HasEverPainted
        {
            get => GetBit(Bits.HasEverPainted);
            set => SetBit(Bits.HasEverPainted, value);
        }

        private bool TextLayoutInvalid
        {
            get => GetBit(Bits.TextLayoutInvalid);
            set => SetBit(Bits.TextLayoutInvalid, value);
        }

        private bool UpdateFragmentsAfterLayout
        {
            get => GetBit(Bits.UpdateFragmentsAfterLayout);
            set => SetBit(Bits.UpdateFragmentsAfterLayout, value);
        }

        private bool IgnoreEffectiveScaleChanges
        {
            get => GetBit(Bits.IgnoreEffectiveScaleChanges);
            set => SetBit(Bits.IgnoreEffectiveScaleChanges, value);
        }

        private bool GetBit(Bits lookupBit) => ((Bits)_bits & lookupBit) != 0;

        private void SetBit(Bits changeBit, bool value) => _bits = value ? (uint)((Bits)_bits | changeBit) : (uint)((Bits)_bits & ~changeBit);

        private void SetBit(Bits changeBit)
        {
            Text text = this;
            text._bits = (uint)((Bits)text._bits | changeBit);
        }

        private void ClearBit(Bits changeBit)
        {
            Text text = this;
            text._bits = (uint)((Bits)text._bits & ~changeBit);
        }

        private class MarkedRange
        {
            public string tagName;
            public int firstCharacter;
            public int lastCharacter;
            public uint rangeID;
            public Dictionary<object, object> attributes;
            public MarkedRange parentRange;
            public TextStyle cachedStyle;
            public TextFragment fragment;
            private static uint s_rangeIDIndicator = 1073741824;

            public Color RangeIDAsColor => new Color(s_rangeIDIndicator | rangeID);

            public Color GetEffectiveColor(Color defaultColor)
            {
                if (cachedStyle != null && cachedStyle.HasColor)
                    return cachedStyle.Color;
                return parentRange != null ? parentRange.GetEffectiveColor(defaultColor) : defaultColor;
            }

            public bool IsInFragment
            {
                get
                {
                    bool flag = false;
                    for (MarkedRange markedRange = this; markedRange != null; markedRange = markedRange.parentRange)
                    {
                        TextStyle cachedStyle = markedRange.cachedStyle;
                        if (cachedStyle != null && cachedStyle.Fragment)
                        {
                            flag = true;
                            break;
                        }
                    }
                    return flag;
                }
            }
        }

        private new enum Bits : uint
        {
            TextFitsWidth = 1,
            TextFitsHeight = 2,
            ClipToHeight = 4,
            WordWrap = 8,
            PasswordMasked = 16, // 0x00000010
            KeepFlowAlive = 32, // 0x00000020
            TextLayoutInvalid = 64, // 0x00000040
            UpdateFragmentsAfterLayout = 128, // 0x00000080
            IgnoreEffectiveScaleChanges = 256, // 0x00000100
            Clipped = 512, // 0x00000200
            InMeasure = 1024, // 0x00000400
            FastMeasureValid = 2048, // 0x00000800
            FastMeasurePossible = 4096, // 0x00001000
            ContributesToWidth = 8192, // 0x00002000
            ScaleDirty = 16384, // 0x00004000
            HasEverPainted = 32768, // 0x00008000
            LineSpacingSet = 65536, // 0x00010000
            CharacterSpacingSet = 131072, // 0x00020000
            EnableKerningSet = 262144, // 0x00040000
            ViewDependent = 524288, // 0x00080000
        }
    }
}
