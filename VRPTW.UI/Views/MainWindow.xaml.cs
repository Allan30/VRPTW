using ControlzEx.Theming;
using MahApps.Metro.Controls;
using ScottPlot;
using ScottPlot.Plottable;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using VRPTW.UI.Extensions;
using VRPTW.UI.ViewModels;
namespace VRPTW.UI.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{
    private const int MARKER_SIZE = 10;
    private ScatterPlot? _combinedScatters;
    private readonly Dictionary<int, ScatterPlot> _allScatters = new();
    private int _lastSelectedVehicle = -1;
    private int _lastHighlightedIndex = -1;
    private static readonly MarkerPlot _highlightedPoint = new()
    {
        X = 0,
        Y = 0,
        Color = Color.Red,
        MarkerSize = MARKER_SIZE + 2,
        MarkerShape = MarkerShape.openCircle,
        IsVisible = false
    };

    public RoutesViewModel ViewModel => (RoutesViewModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();
        ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
        ThemeManager.Current.SyncTheme();
        PlotZone.Plot.Style(ScottPlot.Style.Gray1);
        PlotZone.Plot.XAxis.Label("X");
        PlotZone.Plot.YAxis.Label("Y");
        PlotZone.Plot.XAxis.MinimumTickSpacing(1);
        PlotZone.Plot.YAxis.MinimumTickSpacing(1);
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        PlotZone.Refresh();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(RoutesViewModel.IsSolutionLoaded):
                DrawClients();
                break;
            case nameof(RoutesViewModel.IsSolutionCalculated):
                DrawRoutes();
                break;
            case nameof(RoutesViewModel.SelectedClient):
                HighlightSelectedClient();
                break;
            case nameof(RoutesViewModel.SelectedVehicle):
                HighlightSelectedVehicle();
                break;
            default:
                break;
        }
    }

    private void DrawClients()
    {
        PlotZone.Plot.Clear();
        _allScatters.Clear();
        PlotZone.Plot.Add(_highlightedPoint);
        ConfigurePlot();

        var xs = new double[ViewModel.ClientsWithDepot.Count];
        var ys = new double[ViewModel.ClientsWithDepot.Count];

        for (var i = 0; i < ViewModel.ClientsWithDepot.Count; i++)
        {
            PlotZone.Plot.AddPoint(ViewModel.ClientsWithDepot[i].Coordinate.X, ViewModel.ClientsWithDepot[i].Coordinate.Y, size: 10);

            xs[i] = ViewModel.ClientsWithDepot[i].Coordinate.X;
            ys[i] = ViewModel.ClientsWithDepot[i].Coordinate.Y;
        }

        _combinedScatters = PlotZone.Plot.AddScatterPoints(xs, ys, markerSize: MARKER_SIZE, color: Color.Transparent);
        PlotZone.Refresh();
    }

    private void DrawRoutes()
    {
        PlotZone.Plot.Clear();
        _allScatters.Clear();
        PlotZone.Plot.Add(_highlightedPoint);
        ConfigurePlot();

        var allXs = new List<double>();
        var allYs = new List<double>();

        foreach (var vehicle in ViewModel.Vehicles)
        {
            var xs = new List<double>(ViewModel.Vehicles.Count);
            var ys = new List<double>(ViewModel.Vehicles.Count);

            foreach (var client in vehicle.Clients)
            {
                xs.Add(client.Coordinate.X);
                ys.Add(client.Coordinate.Y);
            }

            allXs.AddRange(xs);
            allYs.AddRange(ys);

            var scatter = PlotZone.Plot.AddScatter(xs.ToArray(), ys.ToArray(), markerSize: MARKER_SIZE);
            _allScatters.Add(vehicle.Id, scatter);
        }
        _combinedScatters = new ScatterPlot(allXs.ToArray(), allYs.ToArray());
        PlotZone.Refresh();
    }

    private void HighlightSelectedClient()
    {
        if (ViewModel.SelectedClient is null)
        {
            _highlightedPoint.IsVisible = false;
            _lastHighlightedIndex = -1;
            PlotZone.Plot.Clear(typeof(CustomTooltip));
        }
        else
        {
            _highlightedPoint.X = ViewModel.SelectedClient.Coordinate.X;
            _highlightedPoint.Y = ViewModel.SelectedClient.Coordinate.Y;
            _highlightedPoint.IsVisible = true;
            PlotZone.Plot.Clear(typeof(CustomTooltip));
            var str = ViewModel.SelectedClient.ToString();
            var pos = $"Position : {ViewModel.SelectedClient.Coordinate}";
            var demand = $"Demande : {ViewModel.SelectedClient.Demand}";
            var readyTime = $"Heure min : {ViewModel.SelectedClient.ReadyTime}";
            var dueTime = $"Heure max : {ViewModel.SelectedClient.DueTime}";
            var service = $"Temps de chargement : {ViewModel.SelectedClient.Service}";
            PlotZone.Plot.AddCustomTooltip($"{str}\n{pos}\n{demand}\n{readyTime}\n{dueTime}\n{service}", _highlightedPoint.X, _highlightedPoint.Y);
        }
        PlotZone.Refresh();
    }

    private void HighlightSelectedVehicle()
    {
        const int weight = 2;
        if (ViewModel.SelectedVehicle is null)
        {
            if (_lastSelectedVehicle != -1)
            {
                _allScatters[_lastSelectedVehicle].LineWidth -= weight;
                PlotZone.Refresh();
            }
            _lastSelectedVehicle = -1;

        }
        else
        {
            if (_lastSelectedVehicle != -1)
            {
                _allScatters[_lastSelectedVehicle].LineWidth -= weight;
            }
            _allScatters[ViewModel.SelectedVehicle.Id].LineWidth += weight;
            _lastSelectedVehicle = ViewModel.SelectedVehicle.Id;
            PlotZone.Refresh();
        }
    }

    private void ConfigurePlot()
    {
        PlotZone.Plot.AddHorizontalLine(ViewModel.ClientsWithDepot[0].Coordinate.Y, color: Color.DimGray, style: LineStyle.DashDotDot);
        PlotZone.Plot.AddVerticalLine(ViewModel.ClientsWithDepot[0].Coordinate.X, color: Color.DimGray, style: LineStyle.DashDotDot);

        const int margin = 10;

        PlotZone.Plot.XAxis.SetBoundary(
            ViewModel.ClientsWithDepot.Min(x => x.Coordinate.X - margin),
            ViewModel.ClientsWithDepot.Max(x => x.Coordinate.X + margin));
        PlotZone.Plot.YAxis.SetBoundary(
            ViewModel.ClientsWithDepot.Min(x => x.Coordinate.Y - margin),
            ViewModel.ClientsWithDepot.Max(x => x.Coordinate.Y + margin));
        PlotZone.Plot.XAxis.SetZoomInLimit(10);
        PlotZone.Plot.YAxis.SetZoomInLimit(10);
    }

    private void OnPlotZoneMouseMoved(object sender, MouseEventArgs e)
    {
        if (_combinedScatters is null) return;

        (double mouseCoordX, double mouseCoordY) = PlotZone.GetMouseCoordinates();
        double xyRatio = PlotZone.Plot.XAxis.Dims.PxPerUnit / PlotZone.Plot.YAxis.Dims.PxPerUnit;

        if (_combinedScatters.TryGetPointNearest(mouseCoordX, mouseCoordY, out var point, 5, xyRatio))
        {
            if (_lastHighlightedIndex != point.index)
            {
                _lastHighlightedIndex = point.index;
                ViewModel.SelectedClient =
                    ViewModel                    
                    .ClientsWithDepot
                    .First(x => x.Coordinate.X == point.x && x.Coordinate.Y == point.y);
            }
        }
        else
        {
            ViewModel.SelectedClient = null;
        }
    }

    private void OnPlotZoneLeftClicked(object sender, RoutedEventArgs e)
    {
        if (_combinedScatters is null || ViewModel.SelectedClient is null) return;

        (double mouseCoordX, double mouseCoordY) = PlotZone.GetMouseCoordinates();
        double xyRatio = PlotZone.Plot.XAxis.Dims.PxPerUnit / PlotZone.Plot.YAxis.Dims.PxPerUnit;

        if (!_combinedScatters.TryGetPointNearest(mouseCoordX, mouseCoordY, out var _, 5, xyRatio))
        {
            ViewModel.SelectedClient = null;
        }
    }
}