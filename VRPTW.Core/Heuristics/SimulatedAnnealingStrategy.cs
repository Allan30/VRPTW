using VRPTW.Core.Operators;

namespace VRPTW.Core.Heuristics;

public class SimulatedAnnealingStrategy : HeuristicStrategyBase
{

    private double _temperature = 100;
    private static readonly double _mu = 0.999;
    private int _currentStep = 0;
    protected override bool LoopConditon => _currentStep < NbSteps;

    protected override Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, (OperatorEnum name, List<int> clientsIndex) operation)> vehicles, Routes solution)
    {
        _currentStep++;
        if (vehicles.Count == 0)
        {
            return solution;
        }
        var r = new Random();
        var newRoutes = (Routes)solution.Clone();
        var theOperation = vehicles[r.Next(0, vehicles.Count)];
        if (theOperation.delta > 0 || Math.Exp(theOperation.delta / _temperature) > r.NextDouble())
        {
            newRoutes.ChangeVehicle(theOperation.src);
            newRoutes.ChangeVehicle(theOperation.trg);
        }
        _temperature *= _mu;
        return newRoutes;
    }
}