using ScottPlot;
using ScottPlot.Plottable;
using System.Collections.Generic;

namespace VRPTW.UI.Extensions;

public static class PlotExtensions
{
    public static void AddRange(this Plot plot, IEnumerable<IPlottable> plottables)
    {
        foreach (var plottable in plottables)
        {
            plot.Add(plottable);
        }
    }
}
