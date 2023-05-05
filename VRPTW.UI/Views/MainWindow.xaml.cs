using ControlzEx.Theming;
using MahApps.Metro.Controls;
using ScottPlot;
using ScottPlot.Plottable;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VRPTW.UI.ViewModels;

namespace VRPTW.UI.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{
    private const int MARKER_SIZE = 10;
    private ScatterPlot? _allPlots;
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

    public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();
        ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
        ThemeManager.Current.SyncTheme();
        PlotZone.Plot.Style(ScottPlot.Style.Default);
        PlotZone.Plot.Frameless();
        ViewModel.RoutesViewModel.PropertyChanged += RoutesViewModel_PropertyChanged;
        PlotZone.Refresh();
        
    }

    private void RoutesViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RoutesViewModel.IsSolutionLoaded))
        {
            DrawClients();
        }
        else if (e.PropertyName == nameof(RoutesViewModel.IsSolutionCalculated))
        {
            DrawRoutes();
        }
    }


    private void DrawClients()
    {
        PlotZone.Plot.Clear();
        PlotZone.Plot.Add(_highlightedPoint);
        SetDepotAxes();
        
        var xs = new double[ViewModel.RoutesViewModel.ClientsWithDepot.Count];
        var ys = new double[ViewModel.RoutesViewModel.ClientsWithDepot.Count];
        
        for (var i = 0; i < ViewModel.RoutesViewModel.ClientsWithDepot.Count; i++)
        {
            PlotZone.Plot.AddPoint(ViewModel.RoutesViewModel.ClientsWithDepot[i].Coordinate.X, ViewModel.RoutesViewModel.ClientsWithDepot[i].Coordinate.Y, size: 10);

            xs[i] = ViewModel.RoutesViewModel.ClientsWithDepot[i].Coordinate.X;
            ys[i] = ViewModel.RoutesViewModel.ClientsWithDepot[i].Coordinate.Y;
        }
        
        _allPlots = PlotZone.Plot.AddScatterPoints(xs, ys, markerSize: MARKER_SIZE, color: Color.Transparent);
        PlotZone.Refresh();
    }

    private void DrawRoutes()
    {
        PlotZone.Plot.Clear();
        PlotZone.Plot.Add(_highlightedPoint);
        SetDepotAxes();

        var allXs = new List<double>();
        var allYs = new List<double>();

        foreach (var vehicle in ViewModel.RoutesViewModel.Vehicles)
        {
            var xs = new List<double>(ViewModel.RoutesViewModel.Vehicles.Count);
            var ys = new List<double>(ViewModel.RoutesViewModel.Vehicles.Count);

            foreach (var client in vehicle.Clients)
            {
                xs.Add(client.Coordinate.X);
                ys.Add(client.Coordinate.Y);
            }

            allXs.AddRange(xs);
            allYs.AddRange(ys);

            PlotZone.Plot.AddScatter(xs.ToArray(), ys.ToArray(), markerSize: MARKER_SIZE);
        }
        _allPlots = PlotZone.Plot.AddScatter(allXs.ToArray(), allYs.ToArray(), color: Color.Transparent);
        PlotZone.Refresh();
    }

    private void SetDepotAxes()
    {
        PlotZone.Plot.AddHorizontalLine(ViewModel.RoutesViewModel.ClientsWithDepot[0].Coordinate.Y, color: Color.DimGray, style: LineStyle.DashDotDot);
        PlotZone.Plot.AddVerticalLine(ViewModel.RoutesViewModel.ClientsWithDepot[0].Coordinate.X, color: Color.DimGray, style: LineStyle.DashDotDot);
    }

    private void OnPlotZoneMouseMoved(object sender, MouseEventArgs e)
    {
        if (_allPlots is null) return;
        
        (double mouseCoordX, double mouseCoordY) = PlotZone.GetMouseCoordinates();
        double xyRatio = PlotZone.Plot.XAxis.Dims.PxPerUnit / PlotZone.Plot.YAxis.Dims.PxPerUnit;

        (double pointX, double pointY, int pointIndex) = _allPlots.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);
        
        _highlightedPoint.X = pointX;
        _highlightedPoint.Y = pointY;
        _highlightedPoint.IsVisible = true;

        if (_lastHighlightedIndex != pointIndex)
        {
            _lastHighlightedIndex = pointIndex;
            PlotZone.Refresh();
        }
    }

    private void OnPlotZoneMouseLeft(object sender, MouseEventArgs e)
    {
        _highlightedPoint.IsVisible = false;
        _lastHighlightedIndex = -1;
        PlotZone.Refresh();
    }

    private void OnSelectedClientChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ViewModel.RoutesViewModel.SelectedClient is null) return;
        
        _highlightedPoint.X = ViewModel.RoutesViewModel.SelectedClient.Coordinate.X;
        _highlightedPoint.Y = ViewModel.RoutesViewModel.SelectedClient.Coordinate.Y;
        _highlightedPoint.IsVisible = true;

        AddSelectedClientTooltip();
        
        PlotZone.Refresh();
    }

    private void AddSelectedClientTooltip()
    {
        PlotZone.Plot.Clear(typeof(Tooltip));

        if (ViewModel.RoutesViewModel.SelectedClient is null) return;

        var str = ViewModel.RoutesViewModel.SelectedClient.ToString();
        var pos = $"Position : {ViewModel.RoutesViewModel.SelectedClient.Coordinate}";
        var demand = $"Demande : {ViewModel.RoutesViewModel.SelectedClient.Demand}";
        var readyTime = $"Heure min : {ViewModel.RoutesViewModel.SelectedClient.ReadyTime}";
        var dueTime = $"Heure max : {ViewModel.RoutesViewModel.SelectedClient.DueTime}";
        var service = $"Temps de chargement : {ViewModel.RoutesViewModel.SelectedClient.Service}";
        PlotZone.Plot.AddTooltip($"{str}\n{pos}\n{demand}\n{readyTime}\n{dueTime}\n{service}", _highlightedPoint.X, _highlightedPoint.Y);
    }

    private void OnPlotZoneLeftClicked(object sender, RoutedEventArgs e)
    {
        ViewModel.RoutesViewModel.SelectedClient =
            ViewModel
            .RoutesViewModel
            .ClientsWithDepot
            .First(x => x.Coordinate.X == _highlightedPoint.X && x.Coordinate.Y == _highlightedPoint.Y);
        
        AddSelectedClientTooltip();

        PlotZone.Refresh();
    }
}
