// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Drawing.TextFlow
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.RenderAPI.Drawing;

namespace Microsoft.Iris.Drawing
{
    internal sealed class TextFlow : DisposableObject
    {
        private object _runsList;
        private Vector _lineBounds;
        private Rectangle _bounds;
        private Rectangle _fitBounds;
        private TextRun _lastFitRun;
        private TextRun _firstFitOnFinalLineRun;
        private int _firstVisibleIndex;
        private int _lastVisibleIndex;

        internal TextFlow()
        {
            _bounds = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
            ResetFitTracking();
            ResetVisibleTracking();
        }

        protected override void OnDispose()
        {
            if (_runsList != null)
            {
                if (_runsList is TextRun runsList)
                {
                    runsList.UnregisterUsage(this);
                }
                else
                {
                    foreach (SharedDisposableObject runs in (Vector)_runsList)
                        runs.UnregisterUsage(this);
                }
            }
            base.OnDispose();
        }

        public TextRun this[int index] => _runsList is TextRun runsList ? runsList : (TextRun)((Vector)_runsList)[index];

        public int Count
        {
            get
            {
                if (_runsList is Vector runsList)
                    return runsList.Count;
                return _runsList == null ? 0 : 1;
            }
        }

        public Vector LineBounds => _lineBounds;

        internal Rectangle GetLastLineBounds(int lineAlignmentOffset)
        {
            Rectangle left = Rectangle.Zero;
            for (int index = Count - 1; index >= 0; --index)
            {
                TextRun run = this[index];
                if (IsOnLastLine(run))
                {
                    Point position = run.Position;
                    Size size = run.Size;
                    int x = position.X + lineAlignmentOffset;
                    int y = position.Y;
                    int num1 = x + size.Width;
                    int num2 = y + size.Height;
                    Rectangle right = new Rectangle(x, y, num1 - x, num2 - y);
                    left = !left.IsZero ? Rectangle.Union(left, right) : right;
                }
                else
                    break;
            }
            return left;
        }

        internal bool IsOnLastLine(TextRun run) => run.Line == LastFitRun.Line;

        public Rectangle Bounds => _bounds;

        public Rectangle FitBounds => _fitBounds;

        public TextRun LastFitRun => _lastFitRun;

        public TextRun FirstFitRunOnFinalLine => _firstFitOnFinalLineRun;

        public int FirstVisibleIndex => _firstVisibleIndex;

        public int LastVisibleIndex => _lastVisibleIndex;

        public bool HasVisibleRuns => _firstVisibleIndex != -1;

        public void Add(TextRun run)
        {
            _bounds = Rectangle.Union(_bounds, run.LayoutBounds);
            if (_runsList == null)
            {
                _runsList = run;
                _lineBounds = new Vector();
                _lineBounds.Add(run.RenderBounds);
            }
            else
            {
                if (!(_runsList is Vector vector))
                {
                    object runsList = _runsList;
                    vector = new Vector();
                    vector.Add(runsList);
                    _runsList = vector;
                }
                if (run.UnderlineStyle != NativeApi.UnderlineStyle.None)
                {
                    Rectangle underlineBounds = run.UnderlineBounds;
                    int line1 = run.Line;
                    for (int index = vector.Count - 1; index > 0; --index)
                    {
                        Point position = ((TextRun)vector[index]).Position;
                        int line2 = ((TextRun)vector[index]).Line;
                        if (position.X >= underlineBounds.Left && line1 == line2)
                            ((TextRun)vector[index]).UnderlineStyle = run.UnderlineStyle;
                        else
                            break;
                    }
                }
                if (_lineBounds.Count < run.Line)
                {
                    _lineBounds.Add(run.RenderBounds);
                }
                else
                {
                    RectangleF lineBound = (RectangleF)_lineBounds[run.Line - 1];
                    if ((int)lineBound.Y > (int)run.RenderBounds.Y || lineBound.IsEmpty)
                        _lineBounds[run.Line - 1] = run.RenderBounds;
                }
                vector.Add(run);
            }
            run.RegisterUsage(this);
        }

        public void AddFit(TextRun run)
        {
            _fitBounds = Rectangle.Union(_fitBounds, run.LayoutBounds);
            if (run.LayoutBounds.IsEmpty)
                return;
            if (_firstFitOnFinalLineRun == null || run.Line > _firstFitOnFinalLineRun.Line)
                _firstFitOnFinalLineRun = run;
            _lastFitRun = run;
        }

        public void AddVisible(int runIndex)
        {
            this[runIndex].Visible = true;
            if (_firstVisibleIndex == -1)
                _firstVisibleIndex = runIndex;
            _lastVisibleIndex = runIndex;
        }

        public void ResetFitTracking()
        {
            _lastFitRun = null;
            _fitBounds = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
        }

        public void ResetVisibleTracking()
        {
            _firstVisibleIndex = -1;
            _lastVisibleIndex = int.MinValue;
        }

        public void ClearSprites()
        {
            if (_runsList == null)
                return;
            if (_runsList is TextRun runsList)
            {
                runsList.TextSprite = null;
                runsList.HighlightSprite = null;
            }
            else
            {
                foreach (TextRun runs in (Vector)_runsList)
                {
                    runs.TextSprite = null;
                    runs.HighlightSprite = null;
                }
            }
        }
    }
}
