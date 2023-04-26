using ControlzEx.Theming;

namespace VRPTW.UI.ViewModels.Styles;

public sealed class AppThemeViewModel
{
    public string Name { get; init; }
    public Theme Theme { get; init; }

    public AppThemeViewModel(Theme theme)
    {
        Theme = theme;
        Name = Theme.DisplayName;
    }

    public override string ToString() => Name;
}