using VRPTW.Core.Neighborhood;
using VRPTW.Core.Operators;

namespace VRPTW.Core.Heuristics;

public sealed class DescentStrategy : HeuristicStrategyBase
{
    
    private float _prevFitness = 0;
    private float _currentFitness = -1;

    public DescentStrategy(INeighborhoodStrategy neighborhoodStrategy) : base(neighborhoodStrategy)
    {
    }

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
        Operators.Add(GetOperatorsFromName(new List<OperatorEnum> { bestOperation.operation.name })[0]);
        return newRoutes;
    }
}