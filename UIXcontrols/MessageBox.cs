// Decompiled with JetBrains decompiler
// Type: UIXControls.MessageBox
// Assembly: UIXControls, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: 78800EA5-2757-404C-BA30-C33FCFC2852A
// Assembly location: C:\Program Files\Zune\UIXcontrols.dll

using Microsoft.Iris;
using System;

#nullable disable
namespace UIXControls
{
    public class MessageBox : DialogHelper
    {
        private string _title;
        private string _message;
        private bool _isOKDefault;
        private Command _okCommand;
        private Command _yesCommand;
        private Command _noCommand;
        private BooleanChoice _doNotAskMeAgain;

        public string Title
        {
            get => _title;
            set => _title = value;
        }

        public string Message
        {
            get => _message;
            set => _message = value;
        }

        public Command Yes => _yesCommand;

        public Command No => _noCommand;

        public Command Ok => _okCommand;

        public BooleanChoice DoNotAskMeAgain => _doNotAskMeAgain;

        public bool IsOKDefault => _isOKDefault;

        public static MessageBox Show(string title, string message, EventHandler okCommandHandler)
        {
            MessageBox dialog = new MessageBox(title, message, okCommandHandler, null, null, null, null);
            ShowCodeDialog(dialog);
            return dialog;
        }

        public static MessageBox Show(
          string title,
          string message,
          EventHandler yesCommandHandler,
          EventHandler noCommandHandler)
        {
            MessageBox dialog = new MessageBox(title, message, null, yesCommandHandler, noCommandHandler, null, null);
            ShowCodeDialog(dialog);
            return dialog;
        }

        public static MessageBox Show(
          string title,
          string message,
          EventHandler okCommandHandler,
          EventHandler yesCommandHandler,
          EventHandler noCommandHandler,
          EventHandler cancelCommandHandler)
        {
            MessageBox dialog = new MessageBox(title, message, okCommandHandler, yesCommandHandler, noCommandHandler, cancelCommandHandler, null);
            ShowCodeDialog(dialog);
            return dialog;
        }

        public static MessageBox Show(
          string title,
          string message,
          EventHandler okCommandHandler,
          EventHandler yesCommandHandler,
          EventHandler noCommandHandler,
          EventHandler cancelCommandHandler,
          BooleanChoice doNotAskMeAgain)
        {
            MessageBox dialog = new MessageBox(title, message, okCommandHandler, yesCommandHandler, noCommandHandler, cancelCommandHandler, doNotAskMeAgain);
            ShowCodeDialog(dialog);
            return dialog;
        }

        public static MessageBox Show(
          string title,
          string message,
          Command yesCommand,
          Command noCommand,
          BooleanChoice doNotAskMeAgain)
        {
            MessageBox dialog = new MessageBox(title, message, null, false, null, yesCommand, noCommand, null, doNotAskMeAgain);
            ShowCodeDialog(dialog);
            return dialog;
        }

        public static MessageBox ShowYesNo(string title, string message, Command yesCommand)
        {
            MessageBox dialog = new MessageBox(title, message, DialogNo, false, null, yesCommand, null, null, null);
            ShowCodeDialog(dialog);
            return dialog;
        }

        public static MessageBox Show(
          string title,
          string message,
          Command okCommand,
          BooleanChoice doNotAskMeAgain)
        {
            MessageBox dialog = new MessageBox(title, message, null, false, okCommand, null, null, null, doNotAskMeAgain);
            ShowCodeDialog(dialog);
            return dialog;
        }

        public static MessageBox Show(
          string title,
          string message,
          Command okCommand,
          string cancelText,
          bool isOKDefault)
        {
            MessageBox dialog = new MessageBox(title, message, cancelText, isOKDefault, okCommand, null, null, null, null);
            ShowCodeDialog(dialog);
            return dialog;
        }

        public static MessageBox Show(
          string title,
          string message,
          Command okCommand,
          string cancelText,
          EventHandler cancelCommand,
          bool isOKDefault)
        {
            MessageBox dialog = new MessageBox(title, message, cancelText, isOKDefault, okCommand, null, null, cancelCommand, null);
            ShowCodeDialog(dialog);
            return dialog;
        }

        public static MessageBox Show(
          string title,
          string message,
          string cancelText,
          bool isOKDefault,
          Command okCommand,
          Command yesCommand,
          Command noCommand,
          EventHandler cancelCommand,
          BooleanChoice doNotAskMeAgain)
        {
            MessageBox dialog = new MessageBox(title, message, cancelText, isOKDefault, okCommand, yesCommand, noCommand, cancelCommand, doNotAskMeAgain);
            ShowCodeDialog(dialog);
            return dialog;
        }

        public MessageBox()
          : this(null, null)
        {
        }

        protected MessageBox(string title, string message)
          : base("res://UIXControls!Dialog.uix#MessageBoxContentUI")
        {
            _title = title;
            _message = message;
        }

        protected MessageBox(
          string title,
          string message,
          EventHandler okCommandHandler,
          EventHandler yesCommandHandler,
          EventHandler noCommandHandler,
          EventHandler cancelCommandHandler,
          BooleanChoice doNotAskMeAgain)
          : this(title, message)
        {
            _doNotAskMeAgain = doNotAskMeAgain;
            EventHandler eventHandler = new EventHandler(OnInvoked);
            if (okCommandHandler == null && noCommandHandler == null)
            {
                if (yesCommandHandler == null)
                {
                    Cancel.Description = DialogOk;
                    Cancel.Invoked += eventHandler;
                }
                else
                    Cancel.Description = DialogNo;
            }
            if (okCommandHandler != null)
            {
                _okCommand = new Command(this, DialogOk, okCommandHandler);
                _okCommand.Invoked += eventHandler;
            }
            if (yesCommandHandler != null)
            {
                _yesCommand = new Command(this, DialogYes, yesCommandHandler);
                _yesCommand.Invoked += eventHandler;
            }
            if (noCommandHandler != null)
            {
                _noCommand = new Command(this, DialogNo, noCommandHandler);
                _noCommand.Invoked += eventHandler;
            }
            if (cancelCommandHandler == null)
                return;
            Cancel.Invoked += cancelCommandHandler;
        }

        protected MessageBox(
          string title,
          string message,
          string cancelText,
          bool isOKDefault,
          Command okCommand,
          Command yesCommand,
          Command noCommand,
          EventHandler cancelHandler,
          BooleanChoice doNotAskMeAgain)
          : this(title, message)
        {
            _doNotAskMeAgain = doNotAskMeAgain;
            EventHandler eventHandler = new EventHandler(OnInvoked);
            if (okCommand == null && yesCommand == null && noCommand == null && cancelHandler == null)
            {
                Cancel.Description = DialogOk;
                Cancel.Invoked += eventHandler;
            }
            if (okCommand != null)
            {
                _okCommand = okCommand;
                _okCommand.Invoked += eventHandler;
            }
            if (yesCommand != null)
            {
                _yesCommand = yesCommand;
                _yesCommand.Invoked += eventHandler;
            }
            if (noCommand != null)
            {
                _noCommand = noCommand;
                _noCommand.Invoked += eventHandler;
            }
            if (cancelHandler != null)
                Cancel.Invoked += cancelHandler;
            if (!string.IsNullOrEmpty(cancelText))
                Cancel.Description = cancelText;
            _isOKDefault = isOKDefault;
        }

        private void OnInvoked(object sender, EventArgs args) => Hide();

        protected static void ShowCodeDialog(MessageBox dialog)
        {
            CodeDialogManager.Instance.ShowCodeDialog(dialog);
        }
    }
}
