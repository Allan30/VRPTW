using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Windows;
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
    
    private void DrawClients(bool showLabels = false)
    {
        PlotZone.Plot.Clear();
        foreach (var client in ViewModel.Solution!.Clients)
        {
            var clientPlot = PlotZone.Plot.AddPoint(client.Coordinate.X, client.Coordinate.Y, size: 10);
            if (showLabels)
            {
                PlotZone.Plot.AddText(client.Id, client.Coordinate.X, client.Coordinate.Y, color: clientPlot.Color);
            }
        }
        PlotZone.Refresh();
    }

    private void DrawRoutes(bool showLabels = false)
    {
        PlotZone.Plot.Clear();
        foreach (var vehicle in ViewModel.Solution!.Vehicles)
        {
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
                currentNode = nextNode;
                nextNode = currentNode.Next;
                i++;
            }
            var vehiculeScatter = PlotZone.Plot.AddScatter(xs, ys, markerSize: 10);
            if (showLabels)
            {
                foreach (var client in vehicle.Clients)
                {
                    PlotZone.Plot.AddText(client.Id, client.Coordinate.X, client.Coordinate.Y, color: vehiculeScatter.Color);
                }
            }
        }
        PlotZone.Refresh();
    }

    private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch && ViewModel.IsSolutionLoaded)
        {
            if (ViewModel.IsSolutionCalculated)
            {
                DrawRoutes(toggleSwitch.IsOn);
            }
            else
            {
                DrawClients(toggleSwitch.IsOn);
            }
        }
    }
}
