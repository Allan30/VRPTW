using MahApps.Metro.Controls;
using ScottPlot;
using System.ComponentModel;
using System.Linq;
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
        // Configure Grid
        PlotZone.Plot.Style(ScottPlot.Style.Black);
        PlotZone.Plot.Grid(false);
        PlotZone.Plot.XAxis.IsVisible = false;
        PlotZone.Plot.YAxis.IsVisible = false;
        PlotZone.Plot.XAxis2.IsVisible = false;
        PlotZone.Plot.YAxis2.IsVisible = false;
        PlotZone.Plot.XAxis.Line(false);
        PlotZone.Plot.YAxis.Line(false);
        PlotZone.Plot.XAxis2.Line(false);
        PlotZone.Plot.YAxis2.Line(false);
        //
        ((MainWindowViewModel)DataContext).PropertyChanged += MainWindow_PropertyChanged;
    }

    private void MainWindow_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.IsSolutionLoaded))
        {
            PlotZone.Plot.Clear();
            DrawClients();
            PlotZone.Refresh();
        }
        else if (e.PropertyName == nameof(MainWindowViewModel.IsSolutionCalculated))
        {
            PlotZone.Plot.Clear();
            DrawRoutes();
            DrawClients();
            PlotZone.Refresh();
        }
    }

    private void DrawClients()
    {
        foreach (var client in ((MainWindowViewModel)DataContext).Solution!.Clients)
        {
            PlotZone.Plot.AddPoint(client.Coordinate.X, client.Coordinate.Y, size: 10).Text = client.Id;
        }
    }
    
    private void DrawRoutes()
    {
        foreach (var vehicle in ((MainWindowViewModel)DataContext).Solution!.Vehicles)
        {
            PlotZone.Plot.AddScatterLines(
                    vehicle.Clients.Select(c => (double)c.Coordinate.X).ToArray(),
                    vehicle.Clients.Select(c => (double)c.Coordinate.Y).ToArray(),
                    lineWidth: 2,
                    label: vehicle.Id.ToString()
                );
        }
    }
}
