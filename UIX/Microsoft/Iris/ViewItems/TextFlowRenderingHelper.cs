// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.ViewItems.TextFlowRenderingHelper
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Render;

namespace Microsoft.Iris.ViewItems
{
    internal class TextFlowRenderingHelper
    {
        private IGradient _gradientMultiLine;
        private IGradient _gradientClipLeftRight;

        public void InvalidateGradients()
        {
        }

        public void CreateFadeGradients(Text textViewItem, IVisualContainer visContainer)
        {
            if (visContainer == null)
                return;
            DisposeFadeGradients(visContainer);
            textViewItem.CreateFadeGradientsHelper(ref _gradientClipLeftRight, ref _gradientMultiLine);
            if (_gradientClipLeftRight != null)
                visContainer.AddGradient(_gradientClipLeftRight);
            if (_gradientMultiLine == null)
                return;
            visContainer.AddGradient(_gradientMultiLine);
        }

        private void DisposeFadeGradients(IVisualContainer visContainer)
        {
            visContainer.RemoveAllGradients();
            if (_gradientMultiLine != null)
            {
                _gradientMultiLine.UnregisterUsage(this);
                _gradientMultiLine = null;
            }
            if (_gradientClipLeftRight == null)
                return;
            _gradientClipLeftRight.UnregisterUsage(this);
            _gradientClipLeftRight = null;
        }
    }
}
