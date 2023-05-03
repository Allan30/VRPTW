using CommunityToolkit.Mvvm.ComponentModel;
using ControlzEx.Theming;
using ScottPlot;
using System.Collections.Generic;
using System.Linq;
using VRPTW.Concret;
using VRPTW.UI.ViewModels.Styles;

namespace VRPTW.UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public Routes? Solution { get; set; }

    public RoutesViewModel RoutesViewModel { get; set; } = new();

    private static readonly List<PlotStyleViewModel> _availablePlotStyles = Style.GetStyles().Select(style => new PlotStyleViewModel(style)).ToList();
    public static List<PlotStyleViewModel> AvailablePlotStyles => _availablePlotStyles;

    [ObservableProperty]
    private PlotStyleViewModel _selectedPlotStyle = AvailablePlotStyles.First();

    private static readonly List<AppThemeViewModel> _availableAppThemes = ThemeManager.Current.Themes.OrderBy(theme => theme.DisplayName).Select(theme => new AppThemeViewModel(theme)).ToList();
    public static List<AppThemeViewModel> AvailableAppThemes => _availableAppThemes;

    [ObservableProperty]
    private AppThemeViewModel _selectedAppTheme = AvailableAppThemes.First(themeVM => themeVM.Theme == ThemeManager.Current.DetectTheme()!);
}
