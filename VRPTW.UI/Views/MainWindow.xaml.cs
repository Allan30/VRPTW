using ControlzEx.Theming;
using MahApps.Metro.Controls;
using ScottPlot.Plottable;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using VRPTW.UI.ViewModels;

namespace VRPTW.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow
{
    public MainWindow()
    {
        InitializeComponent();
        ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
        ThemeManager.Current.SyncTheme();
        PlotZone.Plot.Style(ScottPlot.Style.Black);
        PlotZone.Plot.Frameless();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
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
        foreach (var client in ViewModel.Solution!.Clients)
        {
            var clientPlot = PlotZone.Plot.AddPoint(client.Coordinate.X, client.Coordinate.Y, size: 10);
            var label = new Text
            {
                Label = client.Id,
                X = client.Coordinate.X,
                Y = client.Coordinate.Y,
                Color = clientPlot.Color
            };
        }
        PlotZone.Refresh();
    }

    private void DrawRoutes()
    {
        PlotZone.Plot.Clear();

        foreach (var vehicle in ViewModel.Solution!.Vehicles)
        {
            var color = PlotZone.Plot.GetNextColor();
            var xs = new double[vehicle.Clients.Count];
            var ys = new double[vehicle.Clients.Count];

            var currentNode = vehicle.Clients.First;
            if (currentNode is null) return;
            var nextNode = currentNode.Next;

            var i = 0;

            while (nextNode is not null)
            {
                xs[i] = currentNode.Value.Coordinate.X;
                ys[i] = currentNode.Value.Coordinate.Y;

                var label = new Text
                {
                    Label = currentNode.Value.Id,
                    X = currentNode.Value.Coordinate.X,
                    Y = currentNode.Value.Coordinate.Y,
                    Color = color
                };

                currentNode = nextNode;
                nextNode = currentNode.Next;
                i++;
            }
            PlotZone.Plot.AddScatter(xs, ys, color: color, markerSize: 10);
        }
        PlotZone.Refresh();
    }

    private void OnPlotZoneMouseMoved(object sender, MouseEventArgs e)
    {

        //(double mouseCoordX, double mouseCoordY) = PlotZone.GetMouseCoordinates();
        //double xyRatio = PlotZone.Plot.XAxis.Dims.PxPerUnit / PlotZone.Plot.YAxis.Dims.PxPerUnit;
        //(double pointX, double pointY, int pointIndex) = PlotZone.Plot.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);

        //// place the highlight over the point of interest
        //HighlightedPoint.X = pointX;
        //HighlightedPoint.Y = pointY;
        //HighlightedPoint.IsVisible = true;

        //// render if the highlighted point chnaged
        //if (LastHighlightedIndex != pointIndex)
        //{
        //    LastHighlightedIndex = pointIndex;
        //    wpfPlot1.Refresh();
        //}

        //// update the GUI to describe the highlighted point
        //double mouseX = e.GetPosition(this).X;
        //double mouseY = e.GetPosition(this).Y;
        //label1.Content = $"Closest point to ({mouseX:N2}, {mouseY:N2}) " +
        //    $"is index {pointIndex} ({pointX:N2}, {pointY:N2})";

    }
}
