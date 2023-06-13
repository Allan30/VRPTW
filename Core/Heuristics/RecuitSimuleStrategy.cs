using VRPTW.AbstractObjects;
using VRPTW.Heuristics;

namespace VRPTW.Concret;

public class RecuitSimuleStrategy : IStrategy
{
    
    private double _temperature = 100;
    private static double _mu = 0.999;
    private int _currentStep = 0;
    private int _nbStep = 1000;//Math.Round(Math.Log(Math.Log(0.8)/Math.Log(0.1))/Math.Log(mu));

    protected override bool LoopConditon => _currentStep < _nbStep;

    protected override Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, (OperatorName name, List<int> clientsIndex) operation)> vehicles, Routes solution)
    {
        _currentStep++;
        if (vehicles.Count == 0)
        {
            return solution;
        }
        var r = new Random();
        var newRoutes = (Routes) solution.Clone();
        var theOperation = vehicles[r.Next(0, vehicles.Count)];
        if (theOperation.delta > 0 || Math.Exp(theOperation.delta/_temperature) > r.NextDouble())
        {
            newRoutes.ChangeVehicle(theOperation.src);
            newRoutes.ChangeVehicle(theOperation.trg);
        }
        _temperature *= _mu;
        Console.WriteLine(_temperature);
        Console.WriteLine(theOperation.delta);
        return newRoutes;
    }
}