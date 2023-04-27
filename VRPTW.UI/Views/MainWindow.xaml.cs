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

namespace VRPTW.UI;

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
        PlotZone.Plot.Style(ScottPlot.Style.Black);
        PlotZone.Plot.Frameless();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        PlotZone.Refresh();
    }

    public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.IsSolutionLoaded))
        {
            DrawClients();
        }
        else if (e.PropertyName == nameof(MainWindowViewModel.IsSolutionCalculated))
        {
            DrawRoutes();
        }
        else if (e.PropertyName == nameof(MainWindowViewModel.SelectedPlotStyle))
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

        var xs = new double[ViewModel.Solution!.Clients.Count];
        var ys = new double[ViewModel.Solution!.Clients.Count];
        
        for (var i = 0; i < ViewModel.Solution!.Clients.Count; i++)
        {
            PlotZone.Plot.AddPoint(ViewModel.Solution!.Clients[i].Coordinate.X, ViewModel.Solution!.Clients[i].Coordinate.Y, size: 10);

            xs[i] = ViewModel.Solution!.Clients[i].Coordinate.X;
            ys[i] = ViewModel.Solution!.Clients[i].Coordinate.Y;
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

        foreach (var vehicle in ViewModel.Solution!.Vehicles.Take(1))
        {
            var xs = new List<double>(ViewModel.Solution!.Vehicles.Count);
            var ys = new List<double>(ViewModel.Solution!.Vehicles.Count);

            var currentNode = vehicle.Clients.First;
            if (currentNode is null) return;
            var nextNode = currentNode.Next;

            var i = 0;

            while (nextNode is not null)
            {
                xs.Add(currentNode.Value.Coordinate.X);
                ys.Add(currentNode.Value.Coordinate.Y);

                currentNode = nextNode;
                nextNode = currentNode.Next;
                i++;
            }

            allXs.AddRange(xs);
            allYs.AddRange(ys);

            xs.Add(currentNode.Value.Coordinate.X);
            ys.Add(currentNode.Value.Coordinate.Y);

            PlotZone.Plot.AddScatter(xs.ToArray(), ys.ToArray(), markerSize: MARKER_SIZE);
        }
        _allPlots = PlotZone.Plot.AddScatter(allXs.ToArray(), allYs.ToArray(), color: Color.Transparent);
        PlotZone.Refresh();
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
}
