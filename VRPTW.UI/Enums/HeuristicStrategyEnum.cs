using System.ComponentModel;

namespace VRPTW.UI.Enums;

public enum HeuristicStrategyEnum
{
    [Description("Descente")]
    Descent,
    [Description("Recuit simulé")]
    SimulatedAnnealing,
    [Description("Tabou")]
    Tabu
}
