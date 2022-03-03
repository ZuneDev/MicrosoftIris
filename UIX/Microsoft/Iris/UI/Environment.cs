// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.UI.Environment
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Library;
using Microsoft.Iris.Markup;
using Microsoft.Iris.OS;
using Microsoft.Iris.Render;
using Microsoft.Iris.Session;
using System;

namespace Microsoft.Iris.UI
{
    internal sealed class Environment : NotifyObjectBase
    {
        private bool _mouseActiveFlag;
        private bool _soundEffectsEnabledFlag;
        private ColorScheme _currentColorScheme;
        private static Environment s_instance;
        private static float s_dpiScale = Math.Max(1f, NativeApi.SpGetDpi() / 96f);

        private Environment() => _soundEffectsEnabledFlag = true;

        public static Environment Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new Environment();
                return s_instance;
            }
        }

        public bool IsMouseActive => _mouseActiveFlag;

        public void SetIsMouseActive(bool value)
        {
            if (_mouseActiveFlag == value)
                return;
            _mouseActiveFlag = value;
        }

        public bool IsRightToLeft => UISession.Default.IsRtl;

        public ColorScheme ColorScheme => _currentColorScheme;

        public void SetColorScheme(ColorScheme value)
        {
            if (_currentColorScheme == value)
                return;
            _currentColorScheme = value;
            FireNotification(NotificationID.ColorScheme);
        }

        public bool SoundEffectsEnabled => _soundEffectsEnabledFlag;

        public void SetSoundEffectsEnabled(bool value)
        {
            if (_soundEffectsEnabledFlag == value)
                return;
            _soundEffectsEnabledFlag = value;
        }

        public static float DpiScale => s_dpiScale;

        public float AnimationSpeed
        {
            get => AnimationSystem.SpeedAdjustment;
            set => AnimationSystem.SpeedAdjustment = value;
        }

        public int AnimationUpdatesPerSecond
        {
            get => AnimationSystem.UpdatesPerSecond;
            set => AnimationSystem.UpdatesPerSecond = value;
        }

        private IAnimationSystem AnimationSystem => UISession.Default.RenderSession.AnimationSystem;

        public void AnimationAdvance(int milliseconds) => AnimationSystem.PulseTimeAdvance(milliseconds);
    }
}
