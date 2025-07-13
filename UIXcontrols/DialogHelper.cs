// Decompiled with JetBrains decompiler
// Type: UIXControls.DialogHelper
// Assembly: UIXControls, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: 78800EA5-2757-404C-BA30-C33FCFC2852A
// Assembly location: C:\Program Files\Zune\UIXcontrols.dll

using Microsoft.Iris;

#nullable disable
namespace UIXControls
{
    public class DialogHelper : ModelItem
    {
        private static string s_dialogCancel;
        private static string s_dialogYes;
        private static string s_dialogNo;
        private static string s_dialogOk;
        private string _contentUI;
        private Command _cancel;
        private bool _wouldLikeToBeHidden;

        public static string DialogCancel
        {
            get => s_dialogCancel;
            set => s_dialogCancel = value;
        }

        public static string DialogYes
        {
            get => s_dialogYes;
            set => s_dialogYes = value;
        }

        public static string DialogNo
        {
            get => s_dialogNo;
            set => s_dialogNo = value;
        }

        public static string DialogOk
        {
            get => s_dialogOk;
            set => s_dialogOk = value;
        }

        public DialogHelper() : this(null)
        {
        }

        public DialogHelper(string contentUI)
        {
            _contentUI = contentUI;
            _cancel = new Command();
            _cancel.Description = DialogCancel;
        }

        public string ContentUI => _contentUI;

        public Command Cancel => _cancel;

        public bool WouldLikeToBeHidden
        {
            get => _wouldLikeToBeHidden;
            private set
            {
                if (_wouldLikeToBeHidden == value)
                    return;
                _wouldLikeToBeHidden = value;
                FirePropertyChanged(nameof(WouldLikeToBeHidden));
            }
        }

        public void Hide() => WouldLikeToBeHidden = true;

        public void Show() => CodeDialogManager.Instance.ShowCodeDialog(this);
    }
}
