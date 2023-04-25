using MahApps.Metro.Controls;
using ScottPlot;
using System.ComponentModel;
using System.Drawing;
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
        PlotZone.Plot.Grid(false);
        PlotZone.Plot.XAxis.IsVisible = false;
        PlotZone.Plot.YAxis.IsVisible = false;
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
            DrawClients();
            DrawRoutes();
            PlotZone.Refresh();
        }
    }

    private void DrawClients()
    {
        foreach (var client in ((MainWindowViewModel)DataContext).Solution!.Clients.Select(x => x.Coordinate))
        {
            PlotZone.Plot.AddMarker(client.X, client.Y, MarkerShape.filledCircle, 10, Color.Red);
        }
    }

    private void DrawRoutes()
    {
        foreach (var route in ((MainWindowViewModel)DataContext).Solution!.Vehicles)
        {            
            for (int i = 0; i < route.Clients.Count - 1; i++)
            {
                var client1 = route.Clients.ElementAt(i);
                var client2 = route.Clients.ElementAt(i + 1);
                PlotZone.Plot.AddLine(client1.Coordinate.X, client1.Coordinate.Y, client2.Coordinate.X, client2.Coordinate.Y, Color.Blue);
            }
        }
    }
}
