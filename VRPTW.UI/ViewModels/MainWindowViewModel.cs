using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VRPTW.Concret;
using VRPTWCore.Parser;

namespace VRPTW.UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public Routes? Solution { get; set; }
    
    private static readonly List<StyleViewModel> _availableStyles = Style.GetStyles().Select(style => new StyleViewModel(style)).ToList();
    public static List<StyleViewModel> AvailableStyles => _availableStyles;

    [ObservableProperty]
    private StyleViewModel _selectedStyle = new(Style.Black);

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
