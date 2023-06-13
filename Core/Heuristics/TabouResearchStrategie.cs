using VRPTW.AbstractObjects;
using VRPTW.Heuristics;

namespace VRPTW.Concret;

public class TabouResearchStrategy : IStrategy
{
    private int _tabouSize = 80;
    private int _currentStep = 0;
    private int _nbStep = 1000;

    private List<(OperatorName operatorName, List<string> clientName)> _tabouList = new();
    protected override bool LoopConditon
    {
        get
        {
            Console.WriteLine(_currentStep);
            return _currentStep < _nbStep;
        }
    }

    protected override Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, (OperatorName name, List<int> clientsIndex) operation)> vehicles, Routes solution)
    {
        _currentStep++;
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
            if (_tabouList.Count >= _tabouSize)
            {
                _tabouList.RemoveAt(0);
            }
            _tabouList.Add((bestOperation.operation.name, new List<string>() { solution.Vehicles[bestOperation.src.Id].Clients[bestOperation.operation.clientsIndex[0]].Id, solution.Vehicles[bestOperation.trg.Id].Clients[bestOperation.operation.clientsIndex[1]].Id }));
        }
        /*
        Console.WriteLine(bestOperation.operation.name+" : "+solution.Vehicles[bestOperation.src.Id].Clients[bestOperation.operation.clientsIndex[0]].Id+" <-> "+solution.Vehicles[bestOperation.trg.Id].Clients[bestOperation.operation.clientsIndex[1]].Id);
        Console.WriteLine(bestOperation.delta);
        Console.WriteLine("src      : " + solution.Vehicles[bestOperation.src.Id].ToStringClient());
        Console.WriteLine("trg      : " + solution.Vehicles[bestOperation.trg.Id].ToStringClient());
        Console.WriteLine("new src  : " + bestOperation.src.ToStringClient());
        Console.WriteLine("new trg  : " + bestOperation.trg.ToStringClient());
        */
        newRoutes.ChangeVehicle(bestOperation.src);
        newRoutes.ChangeVehicle(bestOperation.trg);
        return newRoutes;
    }
}