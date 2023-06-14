using System.ComponentModel;

namespace VRPTWCore.Neighborhood;

public enum NeighborhoodStrategyEnum
{
    [Description("Aléatoire")]
    Random,
    [Description("Meilleure")]
    Best
}
