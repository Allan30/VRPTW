using VRPTW.Core.Neighborhood;
using VRPTW.Core.Operators;

namespace VRPTW.Core.Heuristics;

public sealed class TabuStrategy : HeuristicStrategyBase
{
    public int TabuSize { get; init; } = 80;

    private int _currentStep = 0;

    private readonly List<(OperatorEnum operatorName, List<string> clientName)> _tabouList = new();

    public TabuStrategy(INeighborhoodStrategy neighborhoodStrategy, int nbStebs, int tabuSize) : base(neighborhoodStrategy)
    {
        NbSteps = nbStebs;
        TabuSize = tabuSize;
    }

    protected override bool LoopConditon => _currentStep < NbSteps;

    protected override Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, (OperatorEnum name, List<int> clientsIndex) operation)> vehicles, Routes solution, IProgress<int> progress)
    {
        progress.Report(++_currentStep);
        if (vehicles.Count == 0)
        {
            return solution;
        }
        var newRoutes = (Routes)solution.Clone();
        var bestOperation = vehicles.MaxBy(v => v.delta);
        while (_tabouList.Any(t => t.operatorName == bestOperation.operation.name && t.clientName.SequenceEqual(new List<string>() { solution.Vehicles[bestOperation.src.Id].Clients[bestOperation.operation.clientsIndex[0]].Id, solution.Vehicles[bestOperation.trg.Id].Clients[bestOperation.operation.clientsIndex[1]].Id })))
        {
            vehicles.Remove(bestOperation);
            bestOperation = vehicles.MaxBy(v => v.delta);
        }
        if (bestOperation.delta <= 0)
        {
            if (_tabouList.Count >= TabuSize)
            {
                _tabouList.RemoveAt(0);
            }
            _tabouList.Add((bestOperation.operation.name, new List<string>() { solution.Vehicles[bestOperation.src.Id].Clients[bestOperation.operation.clientsIndex[0]].Id, solution.Vehicles[bestOperation.trg.Id].Clients[bestOperation.operation.clientsIndex[1]].Id }));
        }
        newRoutes.ChangeVehicle(bestOperation.src);
        newRoutes.ChangeVehicle(bestOperation.trg);
        Operators.Add(GetOperatorsFromName(new List<OperatorEnum> { bestOperation.operation.name })[0]);
        return newRoutes;
    }
}