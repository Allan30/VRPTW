using ScottPlot.Styles;
using System.Linq;

namespace VRPTW.UI.ViewModels;

public sealed class StyleViewModel
{
    public string Name { get; init; }
    public IStyle Style { get; }

    public StyleViewModel(IStyle style)
    {
        Style = style;
        Name = Style.ToString()!.Split('.').Last();
    }
}
