using MahApps.Metro.Controls;
using ScottPlot;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using VRPTW.Concret;
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
        foreach (var client in ((MainWindowViewModel)DataContext).Solution!.Clients)
        {
            PlotZone.Plot.AddMarker(client.Coordinate.X, client.Coordinate.Y, MarkerShape.filledCircle, 10, Color.Red);
        }
    }
    
    private void DrawRoutes()
    {
        foreach (var vehicle in ((MainWindowViewModel)DataContext).Solution!.Vehicles)
        {
            var currentNode = vehicle.Clients.First;
            if (currentNode is null) return;
            var nextNode = currentNode.Next;

            while (nextNode is not null)
            {
                PlotZone.Plot.AddLine(currentNode.Value.Coordinate.X, currentNode.Value.Coordinate.Y, nextNode.Value.Coordinate.X, nextNode.Value.Coordinate.Y, Color.Blue);
                currentNode = nextNode;
                nextNode = currentNode.Next;
            }
        }
    }
}
