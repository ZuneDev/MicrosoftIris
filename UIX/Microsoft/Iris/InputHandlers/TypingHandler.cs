// Decompiled with JetBrains decompiler
// Type: Microsoft.Iris.InputHandlers.TypingHandler
// Assembly: UIX, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: A56C6C9D-B7F6-46A9-8BDE-B3D9B8D60B11
// Assembly location: C:\Program Files\Zune\UIX.dll

using Microsoft.Iris.Input;
using Microsoft.Iris.Markup;
using Microsoft.Iris.ModelItems;
using Microsoft.Iris.UI;

namespace Microsoft.Iris.InputHandlers
{
    internal class TypingHandler : InputHandler
    {
        private EditableTextData _edit;
        private bool _submitOnEnter;
        private bool _treatEscapeAsBackspace;
        private bool _handlingBackspaceFlag;
        private bool _handlingDeleteFlag;

        protected override void ConfigureInteractivity()
        {
            base.ConfigureInteractivity();
            if (!HandleDirect)
                return;
            UI.MouseInteractive = true;
            UI.KeyInteractive = true;
        }

        public bool SubmitOnEnter
        {
            get => _submitOnEnter;
            set
            {
                if (_submitOnEnter == value)
                    return;
                _submitOnEnter = value;
                FireNotification(NotificationID.SubmitOnEnter);
            }
        }

        public bool TreatEscapeAsBackspace
        {
            get => _treatEscapeAsBackspace;
            set
            {
                if (_treatEscapeAsBackspace == value)
                    return;
                _treatEscapeAsBackspace = value;
                FireNotification(NotificationID.TreatEscapeAsBackspace);
            }
        }

        private void FireTypingInputRejectedEvent() => FireNotification(NotificationID.TypingInputRejected);

        public EditableTextData EditableTextData
        {
            get => _edit;
            set
            {
                if (_edit == value)
                    return;
                _edit = value;
                FireNotification(NotificationID.EditableTextData);
            }
        }

        protected override void OnKeyDown(UIClass ui, KeyStateInfo info)
        {
            if (ShouldIgnoreInput(info) || _edit == null)
                return;
            switch (info.Key)
            {
                case Keys.Back:
                    if (IsEditableTextEmpty() && !_handlingBackspaceFlag)
                        break;
                    info.MarkHandled();
                    break;
                case Keys.Enter:
                    if (!_submitOnEnter)
                        break;
                    _edit.Submit();
                    info.MarkHandled();
                    break;
                case Keys.Escape:
                    if (!_treatEscapeAsBackspace)
                        break;
                    goto case Keys.Back;
                case Keys.Delete:
                    if (IsEditableTextEmpty())
                        break;
                    RemoveChar();
                    info.MarkHandled();
                    _handlingDeleteFlag = true;
                    break;
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                    info.MarkHandled();
                    break;
            }
        }

        protected override void OnKeyCharacter(UIClass ui, KeyCharacterInfo info)
        {
            if (ShouldIgnoreInput(info) || _edit == null)
                return;
            switch (info.Character)
            {
                case '\b':
                    if (!IsEditableTextEmpty())
                    {
                        RemoveChar();
                        info.MarkHandled();
                        _handlingBackspaceFlag = true;
                        break;
                    }
                    if (!_handlingBackspaceFlag)
                        break;
                    info.MarkHandled();
                    break;
                case '\t':
                    break;
                case '\r':
                    if (!_submitOnEnter)
                        break;
                    info.MarkHandled();
                    break;
                case '\x001B':
                    if (!_treatEscapeAsBackspace)
                        break;
                    goto case '\b';
                default:
                    InputChar(info.Character);
                    info.MarkHandled();
                    break;
            }
        }

        protected override void OnKeyUp(UIClass ui, KeyStateInfo info)
        {
            if (ShouldIgnoreInput(info) || _edit == null)
                return;
            switch (info.Key)
            {
                case Keys.Back:
                    if (!_handlingBackspaceFlag)
                        break;
                    info.MarkHandled();
                    _handlingBackspaceFlag = false;
                    break;
                case Keys.Enter:
                    if (!_submitOnEnter)
                        break;
                    info.MarkHandled();
                    break;
                case Keys.Escape:
                    if (!_treatEscapeAsBackspace)
                        break;
                    goto case Keys.Back;
                case Keys.Delete:
                    if (!_handlingDeleteFlag)
                        break;
                    info.MarkHandled();
                    _handlingDeleteFlag = false;
                    break;
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                    info.MarkHandled();
                    break;
            }
        }

        private void RemoveChar()
        {
            if (_edit == null || IsEditableTextEmpty())
                return;
            if (!_edit.ReadOnly)
                _edit.Value = _edit.Value.Substring(0, _edit.Value.Length - 1);
            else
                FireTypingInputRejectedEvent();
        }

        private void Clear()
        {
            if (_edit == null || IsEditableTextEmpty())
                return;
            if (!_edit.ReadOnly)
                _edit.Value = string.Empty;
            else
                FireTypingInputRejectedEvent();
        }

        private void InputChar(char ch)
        {
            if (_edit == null)
                return;
            if (!_edit.ReadOnly && (IsEditableTextEmpty() || _edit.Value.Length < _edit.MaxLength))
                _edit.Value += (string)(object)ch;
            else
                FireTypingInputRejectedEvent();
        }

        private bool ShouldIgnoreInput(KeyActionInfo info) => (info.Modifiers & InputModifiers.ControlKey) == InputModifiers.ControlKey || (info.Modifiers & InputModifiers.AltKey) == InputModifiers.AltKey || (info.Modifiers & InputModifiers.WindowsKey) == InputModifiers.WindowsKey;

        private bool IsEditableTextEmpty() => string.IsNullOrEmpty(_edit.Value);
    }
}
