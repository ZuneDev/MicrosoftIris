// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.Animations.AnimationCompleteArgs
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using System;

namespace Microsoft.Iris.Animations
{
    internal class AnimationCompleteArgs : EventArgs
    {
        private float _progress;

        public AnimationCompleteArgs(float progress) => _progress = progress;

        public float Progress => _progress;
    }
}
