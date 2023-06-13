using System.ComponentModel;

namespace VRPTW.UI.ViewModels.Heuristics;

public enum HeuristicStrategyEnum
{
    [Description("Descente")]
    Descent,
    [Description("Recuit simulé")]
    SimulatedAnnealing,
    [Description("Tabou")]
    Tabu
}
