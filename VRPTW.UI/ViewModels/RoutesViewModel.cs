using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System;
using VRPTW.Concret;
using VRPTW.UI.Mappers;
using VRPTWCore.Parser;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using VRPTW.Heuristics;

namespace VRPTW.UI.ViewModels;

public partial class RoutesViewModel : ObservableObject
{
    public static List<HeuristicViewModel> AvailableHeuristics => new()
    {
        new("Foo"),
        new("Bar"),
    };
    public HeuristicViewModel? SelectedHeuristic { get; set; }
    
    private readonly RoutesMapper _routesMapper = new();

    private Routes? _solution;

    private ObservableCollection<ClientViewModel> _clientsWithDepot = new();
    public ObservableCollection<ClientViewModel> ClientsWithDepot
    {
        get => _clientsWithDepot;
        set => SetProperty(ref _clientsWithDepot, value);
    }

    private ClientViewModel? _selectedClient;
    public ClientViewModel? SelectedClient
    {
        get => _selectedClient;
        set
        {
            if (!(value is null || value.IsDepot))
            {
                SelectedVehicle = Vehicles.SingleOrDefault(v => v.Clients.Contains(value));
            }
            SetProperty(ref _selectedClient, value);
        }
    }

    private ObservableCollection<VehicleViewModel> _vehicles = new();
    public ObservableCollection<VehicleViewModel> Vehicles
    {
        get => _vehicles;
        set => SetProperty(ref _vehicles, value);
    }

    private VehicleViewModel? _selectedVehicle;
    public VehicleViewModel? SelectedVehicle
    {
        get => _selectedVehicle;
        set
        {
            SetProperty(ref _selectedVehicle, value);
            if (SelectedClient?.IsDepot is true)
            {
                SelectedClient = null;
            }
        }
    }

    [ObservableProperty]
    private int _wantedIterations = 1_000;
    public const int ITERATIONS_MAX = 1_000_000;
    public const int ITERATIONS_MIN = 100;

    public double Fitness { get; set; }
    public int NbClients { get; set; }
    public int TotalDemand { get; set; }
    public int Capacity { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartVRPTWCommand))]
    private bool _isSolutionLoaded;

    [ObservableProperty]
    private bool _isSolutionCalculated;

    private bool _isSolutionCalculable;
    public bool IsSolutionCalculable => IsSolutionLoaded && SelectedHeuristic is not null;
    //{
    //    get => _isSolutionCalculable;
    //    set
    //    {
    //        var val = IsSolutionLoaded && SelectedHeuristic is not null;
    //        SetProperty(ref _isSolutionCalculable, val);
    //    }
    //}

    [RelayCommand(CanExecute = nameof(IsSolutionLoaded))]
    private void StartVRPTW()
    {
        _solution!.GenerateRandomSolution();
        var descent = new DescentStrategy();
        descent.Execute(ref _solution);
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
        if (dialog.ShowDialog() is true)
        {
            _solution = VrpParser.ExtractVrpFile(dialog.FileName);
            _routesMapper.RoutesToRoutesViewModel(_solution, this);
            IsSolutionLoaded = true;
        }
    }
}