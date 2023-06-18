using VRPTW.Core.Operators;

namespace VRPTW.Core.Heuristics;

public class DescentStrategy : HeuristicStrategyBase
{

    private double _prevFitness = 0;
    private double _currentFitness = -1;
    protected override bool LoopConditon => _prevFitness > _currentFitness;

    protected override Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, (OperatorEnum name, List<int> clientsIndex) operation)> vehicles, Routes solution, IProgress<int> progress)
    {
        progress.Report(-1);
        _prevFitness = solution.Fitness;
        if (vehicles.Count == 0)
        {
            return solution;
        }
        var newRoutes = (Routes)solution.Clone();
        var bestOperation = vehicles.MaxBy(v => v.delta);
        if (bestOperation.delta <= 0)
        {
            return newRoutes;
        }
        
        newRoutes.ChangeVehicle(bestOperation.src);
        newRoutes.ChangeVehicle(bestOperation.trg);
        _currentFitness = newRoutes.Fitness;
        return newRoutes;
    }
}