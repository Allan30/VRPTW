﻿using VRPTW.UI.Enums;
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
    public int TabuSize { get; set; } = 50;
    public double Alpha { get; set; } = 0.985;
    public int InitialTemperature { get; set; } = 10_000;
    public int NbSteps { get; set; } = 1_000;
    public bool IsSimulatedAnnealing => HeuristicStrategyType == HeuristicStrategyEnum.SimulatedAnnealing;
    public bool IsTabu => HeuristicStrategyType == HeuristicStrategyEnum.Tabu;
    public bool IsStepStrategy => HeuristicStrategyType == HeuristicStrategyEnum.Tabu || HeuristicStrategyType == HeuristicStrategyEnum.SimulatedAnnealing;
}
