// Decompiled with JetBrains decompiler
// Type: UIXControls.Helpers
// Assembly: UIXControls, Version=4.8.0.0, Culture=neutral, PublicKeyToken=ddd0da4d3e678217
// MVID: 78800EA5-2757-404C-BA30-C33FCFC2852A
// Assembly location: C:\Program Files\Zune\UIXcontrols.dll

using Microsoft.Iris;
using Microsoft.Iris.Data;

#nullable disable
namespace UIXControls
{
    public static class Helpers
    {
        public static bool IsModelItemDisposed(ModelItem item) => item.IsDisposed;

        public static void AddUIXControlsClrRedirect() => AddResourceRedirect("res://UIXControls!", "clr-res://UIXControls!");

        public static void AddResourceRedirect(string fromPrefix, string toPrefix)
        {
            Application.AddImportRedirect(fromPrefix, toPrefix);
            ResourceManager.Instance.AddUriRedirect(fromPrefix, toPrefix);
        }
    }
}
