using Microsoft.Iris.Layouts;
using Microsoft.Iris.Render;
using System.Collections.Generic;

namespace Microsoft.Iris.Layout;

public static class PredefinedLayouts
{
    public static bool TryGetFromName(string name, out ILayout layout)
    {
        return s_NameToLayoutMap.TryGetValue(name.ToLowerInvariant(), out layout);
    }

    public static bool TryConvertToString(ILayout layout, out string name)
    {
        foreach (var kvp in s_NameToLayoutMap)
        {
            if (kvp.Value.Equals(layout))
            {
                name = kvp.Key;
                return true;
            }
        }

        name = null;
        return false;
    }

    public static ILayout Anchor { get; } = new AnchorLayout();
    public static ILayout Default { get; } = DefaultLayout.Instance;
    public static ILayout Dock { get; } = new DockLayout();
    public static ILayout Grid { get; } = new GridLayout();
    public static ILayout Scale { get; } = new ScaleLayout();
    public static ILayout Popup { get; } = new PopupLayout();
    public static ILayout Stack { get; } = new StackLayout();
    public static ILayout Form { get; } = new AnchorLayout
    {
        SizeToHorizontalChildren = false,
        SizeToVerticalChildren = false
    };
    public static ILayout HorizontalFlow { get; } = new FlowLayout
    {
        Orientation = Orientation.Horizontal
    };
    public static ILayout VerticalFlow { get; } = new FlowLayout
    {
        Orientation = Orientation.Vertical
    };

    private static readonly Dictionary<string, ILayout> s_NameToLayoutMap = new(10)
    {
        ["anchor"] = Anchor,
        ["default"] = Default,
        ["dock"] = Dock,
        ["grid"] = Grid,
        ["scale"] = Scale,
        ["popup"] = Popup,
        ["stack"] = Stack,
        ["form"] = Form,
        ["horizontalflow"] = HorizontalFlow,
        ["verticalflow"] = VerticalFlow,
    };
}
