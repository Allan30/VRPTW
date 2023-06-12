using VRPTW.Concret;

namespace VRPTW.Heuristics;

public class DescentStrategy : IStrategy
{
    
    private double prevFitness = 0;
    private double currentFitness = -1;
    protected override bool LoopConditon()
    {
        return prevFitness > currentFitness;
    }

    protected override Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, (OperatorName name, List<int> clientsIndex) operation)> vehicles, Routes solution)
    {
        prevFitness = solution.Fitness;
        if (vehicles.Count == 0)
        {
            return solution;
        }
        var newRoutes = (Routes) solution.Clone();
        var bestOperation = vehicles.MaxBy(v => v.delta);
        if (bestOperation.delta <= 0)
        {
            return newRoutes;
        }
        Console.WriteLine(bestOperation.operation.name+" : "+solution.Vehicles[bestOperation.src.Id].Clients[bestOperation.operation.clientsIndex[0]].Id+" <-> "+solution.Vehicles[bestOperation.trg.Id].Clients[bestOperation.operation.clientsIndex[1]].Id);
        Console.WriteLine(bestOperation.delta);
        Console.WriteLine("src      : " + solution.Vehicles[bestOperation.src.Id].ToStringClient());
        Console.WriteLine("trg      : " + solution.Vehicles[bestOperation.trg.Id].ToStringClient());
        Console.WriteLine("new src  : " + bestOperation.src.ToStringClient());
        Console.WriteLine("new trg  : " + bestOperation.trg.ToStringClient());
        
        newRoutes.ChangeVehicle(bestOperation.src);
        newRoutes.ChangeVehicle(bestOperation.trg);
        currentFitness = newRoutes.Fitness;
        return newRoutes;
    }
}