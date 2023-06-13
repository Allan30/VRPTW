using VRPTW.UI.Extensions;
using VRPTW.UI.ViewModels.Heuristics;

namespace VRPTW.UI.ViewModels;

public sealed class HeuristicStrategyViewModel
{
    public HeuristicStrategyViewModel(HeuristicStrategyEnum heuristicStrategyType)
    {
        HeuristicStrategyType = heuristicStrategyType;
    }

    public NeighborhoodStrategyViewModel? NeighborhoodStrategy { get; set; }
    public HeuristicStrategyEnum HeuristicStrategyType { get; }
    public override string ToString() => HeuristicStrategyType.GetFriendlyName();
}
