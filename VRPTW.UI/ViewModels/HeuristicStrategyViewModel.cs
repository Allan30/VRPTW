using VRPTW.UI.Enums;
using VRPTW.UI.Extensions;

namespace VRPTW.UI.ViewModels;

public sealed class HeuristicStrategyViewModel
{
    public HeuristicStrategyViewModel(HeuristicStrategyEnum heuristicStrategyType)
    {
        HeuristicStrategyType = heuristicStrategyType;
    }
    public HeuristicStrategyEnum HeuristicStrategyType { get; }
    public override string ToString() => HeuristicStrategyType.GetFriendlyName();

    
}
