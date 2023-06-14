using System.ComponentModel;

namespace VRPTW.Core.Neighborhood;

public enum NeighborhoodStrategyEnum
{
    [Description("Aléatoire")]
    Random,
    [Description("Meilleure")]
    Best
}
