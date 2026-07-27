// Based on decompilation with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.SimpleText
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;
using Microsoft.Iris.Render.Text;
using Microsoft.Iris.ViewItems;
using System;

namespace Microsoft.Iris.Drawing
{
    // Uses the cross-platform TextDocument abstraction (Microsoft.Iris.Render.Text)
    // instead of calling NativeApi.SpSimpleText* directly, so this class works on
    // any platform: TextDocumentFactory.CreateStandalone() resolves to the native
    // SpTextDocument on Windows and to a SixLabors.Fonts-backed TextDocument
    // elsewhere.
    internal class SimpleText : IDisposable
    {
        private readonly TextDocument _document;

        public SimpleText()
        {
            _document = TextDocumentFactory.CreateStandalone();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _document.Dispose();
        }

        public bool CanMeasure(string content, TextStyle textStyle)
        {
            _document.MeasurePossible(content, ToStyleInfo(textStyle), out var possible);
            return possible;
        }

        public TextFlow Measure(
          string content,
          LineAlignment alignment,
          TextStyle textStyle,
          Size constraint)
        {
            TextFlow textFlow = new TextFlow();
            if (content == null)
                content = string.Empty;

            var textAlignment = alignment switch
            {
                LineAlignment.Near => TextAlignment.Near,
                LineAlignment.Center => TextAlignment.Center,
                LineAlignment.Far => TextAlignment.Far,
                _ => TextAlignment.Near,
            };

            var hresult = _document.Measure(content, textAlignment, ToStyleInfo(textStyle), constraint, out var glyphRunInfo);
            if (hresult.IsSuccess() && glyphRunInfo != null)
            {
                var run = TextRun.FromGlyphRunInfo(glyphRunInfo, _document);
                textFlow.Add(run);
            }
            return textFlow;
        }

        private static TextStyleInfo ToStyleInfo(TextStyle textStyle) => new()
        {
            FontFace = textStyle.FontFace,
            FontSize = textStyle.FontSize,
            AltFontSize = textStyle.AltFontSize,
            Bold = textStyle.Bold,
            Italic = textStyle.Italic,
            Underline = textStyle.Underline,
            Color = textStyle.Color.RenderConvert(),
            HasColor = textStyle.HasColor,
            LineSpacing = textStyle.LineSpacing,
            CharacterSpacing = textStyle.CharacterSpacing,
            EnableKerning = textStyle.EnableKerning,
        };
    }
}
