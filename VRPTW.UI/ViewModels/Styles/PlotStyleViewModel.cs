using ScottPlot.Styles;
using System.Linq;

namespace VRPTW.UI.ViewModels.Styles;

public sealed class PlotStyleViewModel
{
    public string Name { get; init; }
    public IStyle Style { get; init; }

    public PlotStyleViewModel(IStyle style)
    {
        Style = style;
        Name = Style.ToString()!.Split('.').Last();
    }

    public override string ToString() => Name;
}
