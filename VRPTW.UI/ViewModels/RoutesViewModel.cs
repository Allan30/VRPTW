using CommunityToolkit.Mvvm.ComponentModel;
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

    public double Fitness { get; set; }
    public int NbClients { get; set; }
    public int TotalDemand { get; set; }
    public int Capacity { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartVRPTWCommand))]
    [NotifyCanExecuteChangedFor(nameof(RandomSolutionCommand))]
    private bool _isSolutionLoaded;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ResetCommand))]
    private bool _isSolutionCalculating;

    [ObservableProperty]
    private bool _isSolutionCalculated;

    public bool IsSolutionCalculable => IsSolutionLoaded && !IsSolutionCalculating && SelectedOperators.Any();

    [ObservableProperty]
    private bool _displayAllRoutes = true;

    [RelayCommand(CanExecute = nameof(IsSolutionLoaded))]
    private void RandomSolution()
    {
        IsSolutionCalculated = false;
        _solution!.GenerateRandomSolution();
        _routesMapper.RoutesToRoutesViewModel(_solution, this);
        IsSolutionCalculated = true;
    }

    [RelayCommand(CanExecute = nameof(IsSolutionCalculating))]
    private void Reset()
    {
        IsSolutionCalculated = false;
        _solution!.Reset();
        _routesMapper.RoutesToRoutesViewModel(_solution, this);
    }

    [RelayCommand(CanExecute = nameof(IsSolutionCalculable))]
    private async Task StartVRPTWAsync()
    {
        IsSolutionCalculated = false;
        IsSolutionCalculating = true;
        _solution!.GenerateRandomSolution();
        var strategy = _heuristicStrategyMapper.HeuristicStrategyViewModelToHeuristicStrategyBase(SelectedHeuristicStrategy);
        strategy.NeighborhoodStrategy = SelectedNeighborhoodStrategy.NeighborhoodStrategyType;
        var routesTask = Task.Run(() => strategy.CalculateAsync(_solution, SelectedOperators.Select(op => op.OperatorType).ToList(), _cancellationTokenSource.Token));
        try
        {
            _solution = await routesTask;
            _routesMapper.RoutesToRoutesViewModel(_solution, this);
            IsSolutionCalculated = true;
        }
        catch (OperationCanceledException)
        {

        }
        finally
        {
            IsSolutionCalculating = false;
            _cancellationTokenSource = new();
        }
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