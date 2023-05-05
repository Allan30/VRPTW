﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using VRPTW.Concret;
using VRPTW.UI.Mappers;
using VRPTWCore.Parser;

namespace VRPTW.UI.ViewModels;

public partial class RoutesViewModel : ObservableObject
{
    private readonly RoutesMapper _routesMapper = new();

    private Routes? _solution;
    
    private ObservableCollection<ClientViewModel> _clientsWithDepot = new();
    public ObservableCollection<ClientViewModel> ClientsWithDepot
    {
        get => _clientsWithDepot;
        set => SetProperty(ref _clientsWithDepot, value);
    }

    [ObservableProperty]
    private ClientViewModel? _selectedClient;

    public ObservableCollection<VehicleViewModel> Vehicles { get; set; } = new();
    public double Fitness { get; set; }
    public int NbClients { get; set; }
    public int TotalDemand { get; set; }
    public int Capacity { get; set; }
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartVRPTWCommand))]
    private bool _isSolutionLoaded;

    [ObservableProperty]
    private bool _isSolutionCalculated;

    [RelayCommand(CanExecute = nameof(IsSolutionLoaded))]
    private void StartVRPTW()
    {
        _solution!.GenerateRandomSolution();
        _routesMapper.RoutesToRoutesViewModel(_solution, this);
        IsSolutionCalculated = true;
    }

    [RelayCommand]
    private void ChooseFile()
    {
        IsSolutionLoaded = false;
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
            _solution = parser.ExtractVrpFile(dialog.FileName);
            _routesMapper.RoutesToRoutesViewModel(_solution, this);
            IsSolutionLoaded = true;
        }
    }
}
