// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.FocusHandler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Markup;
using Microsoft.Iris.Session;
using Microsoft.Iris.UI;
using System;

namespace Microsoft.Iris.InputHandlers
{
    internal class FocusHandler : ModifierInputHandler
    {
        private FocusChangeReason _changeReason;
        private WeakReference _gainedEventContext;
        private WeakReference _lostEventContext;

        public FocusHandler() => _changeReason = FocusChangeReason.Any;

        protected override void ConfigureInteractivity()
        {
            base.ConfigureInteractivity();
            if (!HandleDirect)
                return;
            UI.KeyInteractive = true;
        }

        public FocusChangeReason Reason
        {
            get => _changeReason;
            set
            {
                if (_changeReason == value)
                    return;
                _changeReason = value;
                FireNotification(NotificationID.Reason);
            }
        }

        public object GainedEventContext => CheckEventContext(ref _gainedEventContext);

        public object LostEventContext => CheckEventContext(ref _lostEventContext);

        protected override void OnGainKeyFocus(UIClass ui, KeyFocusInfo info)
        {
            if (!ShouldHandleEvent(info))
                return;
            SetEventContext(info.Target, ref _gainedEventContext, NotificationID.GainedEventContext);
            FireNotification(NotificationID.GainedFocus);
        }

        protected override void OnLoseKeyFocus(UIClass ui, KeyFocusInfo info)
        {
            if (!ShouldHandleEvent(info))
                return;
            SetEventContext(info.Target, ref _lostEventContext, NotificationID.LostEventContext);
            FireNotification(NotificationID.LostFocus);
        }

        private bool ShouldHandleEvent(KeyFocusInfo info) => Library.Bits.TestAnyFlags((uint)Reason, (uint)GetFocusChangeReason(info)) && ShouldHandleEvent(GetModifiers(UISession.Default.InputManager.Modifiers));

        private static FocusChangeReason GetFocusChangeReason(KeyFocusInfo info)
        {
            switch (info.FocusReason)
            {
                case KeyFocusReason.Directional:
                    return FocusChangeReason.Directional;
                case KeyFocusReason.Tab:
                    return FocusChangeReason.Tab;
                case KeyFocusReason.MouseDown:
                case KeyFocusReason.MouseEnter:
                    return FocusChangeReason.Mouse;
                default:
                    return FocusChangeReason.Other;
            }
        }
    }
}
