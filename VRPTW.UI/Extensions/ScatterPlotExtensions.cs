using ScottPlot;
using ScottPlot.Plottable;
using System.Collections.Generic;
using System.Linq;
using VRPTW.UI.Plottable;

namespace VRPTW.UI.ScottPlotExtended;

public static class ScatterPlotExtensions
{
    public static bool TryGetPointNearest(this ScatterPlot plot, double x, double y, out (double x, double y, int index) point, int snapDistance, double xyRatio = 1)
    {
        int from = plot.MinRenderIndex ?? 0;
        int to = plot.MaxRenderIndex ?? (plot.Ys.Length - 1);

        List<(double x, double y)> points = plot.Xs.Zip(plot.Ys, (first, second) => (first, second)).Skip(from).Take(to - from + 1).ToList();

        double xyRatioSquared = xyRatio * xyRatio;
        double pointDistanceSquared(double x1, double y1) =>
            (x1 - x) * (x1 - x) * xyRatioSquared + (y1 - y) * (y1 - y);

        double minDistance = double.PositiveInfinity;
        int minIndex = 0;
        for (int i = 0; i < points.Count; i++)
        {
            if (double.IsNaN(points[i].x) || double.IsNaN(points[i].y))
                continue;

            double currDistance = pointDistanceSquared(points[i].x, points[i].y);
            if (currDistance < minDistance)
            {
                minIndex = i;
                minDistance = currDistance;
            }
        }

        point = (plot.Xs[minIndex], plot.Ys[minIndex], minIndex);
        
        return minDistance <= snapDistance;
    }

    public static CustomTooltip AddCustomTooltip(this Plot plot, string label, double x, double y)
    {
        var plottable = new CustomTooltip(new Tooltip { Label = label, X = x, Y = y});
        plot.Add(plottable);
        return plottable;
    }
}
