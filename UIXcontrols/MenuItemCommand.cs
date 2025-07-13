// Decompiled with JetBrains decompiler
// Type: UIXControls.MenuItemCommand
// Assembly: UIXControls, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: 78800EA5-2757-404C-BA30-C33FCFC2852A
// Assembly location: C:\Program Files\Zune\UIXcontrols.dll

using Microsoft.Iris;
using System;

#nullable disable
namespace UIXControls
{
    public class MenuItemCommand : Command
    {
        private bool _hidden;

        public MenuItemCommand()
        {
        }

        public MenuItemCommand(string description)
          : base(null, description, null)
        {
        }

        public bool Hidden
        {
            get => _hidden;
            set
            {
                if (_hidden == value)
                    return;
                _hidden = value;
                FirePropertyChanged(nameof(Hidden));
            }
        }

        public virtual bool ShouldHide() => Hidden;

        public override string ToString()
        {
            return $"{GetType().Name}:\"{Description}\", Available = {Available}, Hidden = {Hidden}";
        }
    }
}
