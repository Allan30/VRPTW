using System.ComponentModel;

namespace VRPTW.UI.ViewModels.Neighborhood;

public enum NeighborhoodStrategyEnum
{
    [Description("Aléatoire")]
    Random,
    [Description("Meilleure")]
    Best
}
