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

    public MainWindow()
    {
        InitializeComponent();
        ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
        ThemeManager.Current.SyncTheme();
        PlotZone.Plot.Style(ScottPlot.Style.Default);
        PlotZone.Plot.Frameless();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
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

    public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.SelectedPlotStyle))
        {
            PlotZone.Plot.Style(ViewModel.SelectedPlotStyle.Style);
            PlotZone.Refresh();
        }
        else if (e.PropertyName == nameof(MainWindowViewModel.SelectedAppTheme))
        {
            ThemeManager.Current.ChangeTheme(Application.Current, ViewModel.SelectedAppTheme.Theme);
        }
    }

    private void DrawClients()
    {
        PlotZone.Plot.Clear();
        PlotZone.Plot.Add(_highlightedPoint);

        var xs = new double[ViewModel.RoutesViewModel.Clients.Count];
        var ys = new double[ViewModel.RoutesViewModel.Clients.Count];
        
        for (var i = 0; i < ViewModel.RoutesViewModel.Clients.Count; i++)
        {
            PlotZone.Plot.AddPoint(ViewModel.RoutesViewModel.Clients[i].Coordinate.X, ViewModel.RoutesViewModel.Clients[i].Coordinate.Y, size: 10);

            xs[i] = ViewModel.RoutesViewModel.Clients[i].Coordinate.X;
            ys[i] = ViewModel.RoutesViewModel.Clients[i].Coordinate.Y;
        }
        
        _allPlots = PlotZone.Plot.AddScatterPoints(xs, ys, markerSize: MARKER_SIZE, color: Color.Transparent);
        PlotZone.Refresh();
    }

    private void DrawRoutes()
    {
        PlotZone.Plot.Clear();
        PlotZone.Plot.Add(_highlightedPoint);

        var allXs = new List<double>();
        var allYs = new List<double>();

        foreach (var vehicle in ViewModel.RoutesViewModel.Vehicles)
        {
            var xs = new List<double>(ViewModel.RoutesViewModel.Vehicles.Count);
            var ys = new List<double>(ViewModel.RoutesViewModel.Vehicles.Count);

            foreach (var client in vehicle.Clients.SkipLast(1))
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
    
    private void OnPlotZoneMouseMoved(object sender, MouseEventArgs e)
    {
        if (_allPlots is null) return;
        
        (double mouseCoordX, double mouseCoordY) = PlotZone.GetMouseCoordinates();
        //double xyRatio = PlotZone.Plot.XAxis.Dims.PxPerUnit / PlotZone.Plot.YAxis.Dims.PxPerUnit;
        //(double pointX, double pointY, int pointIndex) = _allPlots.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);

        _highlightedPoint.X = mouseCoordX;
        _highlightedPoint.Y = mouseCoordY;
        _highlightedPoint.IsVisible = true;

        //if (_lastHighlightedIndex != pointIndex)
        //{
        //    _lastHighlightedIndex = pointIndex;
            PlotZone.Refresh();
        //}
    }
}
