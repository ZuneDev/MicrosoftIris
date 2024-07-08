using Microsoft.Iris.Layouts;
using Microsoft.Iris.Render;
using System.Linq;

namespace Microsoft.Iris.Layout;

public static class PredefinedLayouts
{
    public static bool TryGetFromName(string name, out ILayout layout)
    {
        var prop = typeof(PredefinedLayouts).GetProperties()
            .FirstOrDefault(p => p.Name.Equals(name, System.StringComparison.InvariantCultureIgnoreCase));

        if (prop is null)
        {
            layout = null;
            return false;
        }

        layout = (ILayout)prop.GetValue(null);
        return true;
    }

    public static bool TryConvertToString(ILayout layout, out string name)
    {
        foreach (var prop in typeof(PredefinedLayouts).GetProperties())
        {
            var value = prop.GetValue(null);

            if (value == layout)
            {
                name = prop.Name;
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
}
