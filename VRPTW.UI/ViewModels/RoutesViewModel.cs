﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System;
using VRPTW.UI.Mappers;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using VRPTW.UI.Enums;
using VRPTW.Core.Neighborhood;
using VRPTW.Core.Operators;
using VRPTW.Core.Tools;
using VRPTW.Core;
using System.Threading.Tasks;
using System.Threading;

namespace VRPTW.UI.ViewModels;

public partial class RoutesViewModel : ObservableObject
{
    #region Mappers
    private readonly HeuristicStrategyMapper _heuristicStrategyMapper = new();
    private readonly RoutesMapper _routesMapper = new();
    #endregion

    private Routes? _solution;

    public static readonly List<NeighborhoodStrategyViewModel> NeighborhoodStrategies = new()
    {
        new(NeighborhoodStrategyEnum.Best),
        new(NeighborhoodStrategyEnum.Random)
    };

    [ObservableProperty]
    private NeighborhoodStrategyViewModel _selectedNeighborhoodStrategy = NeighborhoodStrategies.First();

    public static readonly List<HeuristicStrategyViewModel> HeuristicStrategies = new()
    {
        new(HeuristicStrategyEnum.Descent),
        new(HeuristicStrategyEnum.SimulatedAnnealing),
        new(HeuristicStrategyEnum.Tabu)
    };

    [ObservableProperty]
    private HeuristicStrategyViewModel _selectedHeuristicStrategy = HeuristicStrategies.First();

    public static readonly List<OperatorViewModel> Operators = new()
    {
        new(OperatorEnum.ExchangeInter),
        new(OperatorEnum.ExchangeIntra),
        new(OperatorEnum.RelocateInter),
        new(OperatorEnum.RelocateIntra),
        new(OperatorEnum.TwoOpt)
    };

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartVRPTWCommand))]
    private ObservableCollection<OperatorViewModel> _selectedOperators = new();

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
            if (SelectedVehicle is null || SelectedClient?.IsDepot is true || (SelectedClient is not null && SelectedVehicle is not null && !SelectedVehicle.Clients.Contains(SelectedClient)))
            {
                SelectedClient = null;
            }
        }
    }

    [ObservableProperty]
    private TimeSpan _timeToCalculate;
    public float Fitness { get; set; }
    public int NbClients { get; set; }
    public int TotalDemand { get; set; }
    public int Capacity { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartVRPTWCommand))]
    [NotifyCanExecuteChangedFor(nameof(RandomSolutionCommand))]
    [NotifyPropertyChangedFor(nameof(IsSolutionCalculable))]
    private bool _isSolutionLoaded;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RandomSolutionCommand))]
    [NotifyPropertyChangedFor(nameof(IsSolutionCalculable))]
    [NotifyCanExecuteChangedFor(nameof(ResetCommand))]
    private bool _isSolutionCalculating;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResetCommand))]
    private bool _isSolutionCalculated;

    public bool IsSolutionCalculatedAndNotCalculating => IsSolutionCalculated && !IsSolutionCalculating;
    public bool IsSolutionLoadedAndNotCalculating => IsSolutionLoaded && !IsSolutionCalculating;

    public bool IsSolutionCalculable => IsSolutionLoaded && !IsSolutionCalculating && SelectedOperators.Any();

    [ObservableProperty]
    private bool _displayAllRoutes = true;

    [RelayCommand(CanExecute = nameof(IsSolutionLoadedAndNotCalculating))]
    private void RandomSolution()
    {
        IsSolutionCalculated = false;
        _solution!.GenerateRandomSolution();
        _routesMapper.RoutesToRoutesViewModel(_solution, this);
        IsSolutionCalculated = true;
    }

    [RelayCommand(CanExecute = nameof(IsSolutionCalculatedAndNotCalculating))]
    private void Reset()
    {
        IsSolutionCalculated = false;
        _solution!.Reset();
        _routesMapper.RoutesToRoutesViewModel(_solution, this);
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Progress))]
    private int _currentStep;

    public int Progress => CurrentStep * 100 / SelectedHeuristicStrategy.NbSteps;

    [RelayCommand(CanExecute = nameof(IsSolutionCalculable))]
    private async Task StartVRPTWAsync()
    {
        _solution!.Reset();
        _routesMapper.RoutesToRoutesViewModel(_solution, this);
        IsSolutionCalculated = false;
        IsSolutionCalculating = true;
        _solution!.GenerateRandomSolution();
        var strategy = _heuristicStrategyMapper.HeuristicStrategyViewModelToHeuristicStrategyBase(SelectedHeuristicStrategy, SelectedNeighborhoodStrategy.NeighborhoodStrategyType);
        _solution = await Task.Run(() =>
            strategy.Calculate(
                _solution,
                SelectedOperators.Select(op => op.OperatorType).ToList(),
                new Progress<int>(step => CurrentStep = step),
                _cancellationTokenSource.Token
            )
        );
        _routesMapper.RoutesToRoutesViewModel(_solution, this);
        IsSolutionCalculating = false;
        IsSolutionCalculated = true;
        _cancellationTokenSource = new();
    }

    private CancellationTokenSource _cancellationTokenSource = new();

    [RelayCommand]
    private void CancelVRPTW()
    {
        _cancellationTokenSource.Cancel();
    }

    [RelayCommand]
    private void ChooseFile()
    {
        var appPath = AppDomain.CurrentDomain.BaseDirectory;
        var dialog = new OpenFileDialog()
        {
            Filter = "Fichiers vrp (*.vrp)|*.vrp|Tous les fichiers (*.*)|*.*",
            InitialDirectory = Path.Combine(appPath, "Data")
        };
        if (dialog.ShowDialog() is true)
        {
            IsSolutionLoaded = false;
            _solution = VrpParser.ExtractVrpFile(dialog.FileName);
            _routesMapper.RoutesToRoutesViewModel(_solution, this);
            IsSolutionLoaded = true;
            IsSolutionCalculated = false;
        }
    }
}