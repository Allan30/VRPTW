using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlzEx.Theming;
using Microsoft.Win32;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VRPTW.Concret;
using VRPTW.UI.ViewModels.Styles;
using VRPTWCore.Parser;

namespace VRPTW.UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public Routes? Solution { get; set; }
    
    private static readonly List<PlotStyleViewModel> _availablePlotStyles = Style.GetStyles().Select(style => new PlotStyleViewModel(style)).ToList();
    public static List<PlotStyleViewModel> AvailablePlotStyles => _availablePlotStyles;

    [ObservableProperty]
    private PlotStyleViewModel _selectedPlotStyle = new(Style.Black);

    private static readonly List<AppThemeViewModel> _availableAppThemes = ThemeManager.Current.Themes.OrderBy(theme => theme.DisplayName).Select(theme => new AppThemeViewModel(theme)).ToList();
    public static List<AppThemeViewModel> AvailableAppThemes => _availableAppThemes;

    [ObservableProperty]
    private AppThemeViewModel _selectedAppTheme = new(ThemeManager.Current.DetectTheme()!);

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartVRPTWCommand))]
    private bool _isSolutionLoaded;

    [ObservableProperty]
    private bool _isSolutionCalculated;

    [RelayCommand(CanExecute = nameof(CanStartVRPTWCommand))]
    private void StartVRPTW()
    {
        Solution!.GenerateRandomSolution();
        IsSolutionCalculated = true;
    }

    private bool CanStartVRPTWCommand() => IsSolutionLoaded;

    [RelayCommand]
    private void ChooseFile()
    {
        IsSolutionCalculated = false;
        var appPath = AppDomain.CurrentDomain.BaseDirectory;
        var dialog = new OpenFileDialog()
        {
            Filter = "Fichiers vrp (*.vrp)|*.vrp|Tous les fichiers (*.*)|*.*",
            InitialDirectory = Path.Combine(appPath, "Core", "Data")
        };
        if (dialog.ShowDialog() == true)
        {
            var parser = new VrpParser();
            Solution = parser.ExtractVrpFile(dialog.FileName);
            IsSolutionLoaded = true;
        }
    }
}
