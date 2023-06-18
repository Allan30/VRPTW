using VRPTW.Core.Operators;

namespace VRPTW.Core.Heuristics;

public class SimulatedAnnealingStrategy : HeuristicStrategyBase
{

    private double _temperature = 100;
    public double Alpha { get; set; } = 0.999;
    private int _currentStep = 0;
    protected override bool LoopConditon => _currentStep < NbSteps;

    protected override Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, (OperatorEnum name, List<int> clientsIndex) operation)> vehicles, Routes solution, IProgress<int> progress)
    {
        progress.Report(++_currentStep);
        if (vehicles.Count == 0)
        {
            return solution;
        }
        var newRoutes = (Routes)solution.Clone();
        var theOperation = vehicles[Random.Next(0, vehicles.Count)];
        if (theOperation.delta > 0 || Math.Exp(theOperation.delta / _temperature) > Random.NextDouble())
        {
            newRoutes.ChangeVehicle(theOperation.src);
            newRoutes.ChangeVehicle(theOperation.trg);
        }
        _temperature *= Alpha;
        return newRoutes;
    }
}