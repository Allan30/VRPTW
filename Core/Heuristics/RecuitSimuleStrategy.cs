using VRPTW.AbstractObjects;
using VRPTW.Heuristics;

namespace VRPTW.Concret;

public class RecuitSimuleStrategy : IStrategy
{
    
    private double temperature = 100;
    private double mu = 0.999;
    private int currentStep = 0;
    private int nbStep = 1000;

    protected override bool LoopConditon()
    {
        return currentStep < nbStep;
    }

    protected override Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, (OperatorName name, List<int> clientsIndex) operation)> vehicles, Routes solution)
    {
        currentStep++;
        if (vehicles.Count == 0)
        {
            return solution;
        }
        var r = new Random();
        var newRoutes = (Routes) solution.Clone();
        var theOperation = vehicles[r.Next(0, vehicles.Count)];
        if (theOperation.delta > 0 || Math.Exp(theOperation.delta/temperature) > r.NextDouble())
        {
            newRoutes.ChangeVehicle(theOperation.src);
            newRoutes.ChangeVehicle(theOperation.trg);
        }
        temperature *= mu;
        Console.WriteLine(temperature);
        Console.WriteLine(theOperation.delta);
        return newRoutes;
    }
}