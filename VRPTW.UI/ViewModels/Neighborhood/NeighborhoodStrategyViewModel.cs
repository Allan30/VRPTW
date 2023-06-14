﻿using VRPTW.UI.Extensions;
using VRPTWCore.Neighborhood;

namespace VRPTW.UI.ViewModels;

public sealed class NeighborhoodStrategyViewModel
{
    public NeighborhoodStrategyViewModel(NeighborhoodStrategyEnum neighborhoodStrategyType)
    {
        NeighborhoodStrategyType = neighborhoodStrategyType;
    }
    public NeighborhoodStrategyEnum NeighborhoodStrategyType { get; }
    public override string ToString() => NeighborhoodStrategyType.GetFriendlyName();
}
