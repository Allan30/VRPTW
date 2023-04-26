using MahApps.Metro.Controls;
using System.ComponentModel;
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
        PlotZone.Plot.Style(ScottPlot.Style.Black);
        PlotZone.Plot.Frameless();
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
            PlotZone.Refresh();
        }
        else if (e.PropertyName == nameof(MainWindowViewModel.SelectedStyle))
        {
            PlotZone.Plot.Style(((MainWindowViewModel)DataContext).SelectedStyle.Style);
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

            PlotZone.Plot.AddScatter(xs, ys, lineWidth: 2, label: vehicle.Id.ToString());
        }
    }
}
