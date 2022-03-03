// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.ModifierInputHandler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Markup;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.InputHandlers
{
    internal class ModifierInputHandler : InputHandler
    {
        private InputHandlerModifiers _requiredModifiers;
        private InputHandlerModifiers _disallowedModifiers;
        private InputHandlerTransition _handlerTransition;

        public ModifierInputHandler()
        {
            _handlerTransition = InputHandlerTransition.Up;
            _requiredModifiers = InputHandlerModifiers.None;
            _disallowedModifiers = InputHandlerModifiers.None;
        }

        public InputHandlerTransition HandlerTransition
        {
            get => _handlerTransition;
            set
            {
                if (_handlerTransition == value)
                    return;
                _handlerTransition = value;
                FireNotification(NotificationID.HandlerTransition);
            }
        }

        public InputHandlerModifiers RequiredModifiers
        {
            get => _requiredModifiers;
            set
            {
                if (_requiredModifiers == value)
                    return;
                _requiredModifiers = value;
                FireNotification(NotificationID.RequiredModifiers);
            }
        }

        public InputHandlerModifiers DisallowedModifiers
        {
            get => _disallowedModifiers;
            set
            {
                if (_disallowedModifiers == value)
                    return;
                _disallowedModifiers = value;
                FireNotification(NotificationID.DisallowedModifiers);
            }
        }

        protected bool ShouldHandleEvent(InputHandlerModifiers modifiers) => (_disallowedModifiers == InputHandlerModifiers.None || !Library.Bits.TestAnyFlags((uint)modifiers, (uint)_disallowedModifiers)) && (_requiredModifiers == InputHandlerModifiers.None || Library.Bits.TestAllFlags((uint)modifiers, (uint)_requiredModifiers));
    }
}
