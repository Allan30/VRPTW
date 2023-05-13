using VRPTW.Concret;

namespace VRPTW.Heuristics;

public class DescentStrategy : IStrategy
{

    public void RandomWithSelectedOperators(ref Routes solution, List<OperatorName> operatorsName)
    {
        var prevFitness = double.MaxValue;
        var operators = GetOperatorsFromName(operatorsName).ToList();
        var nbOperators = operators.Count();
        var r = new Random();
        while (prevFitness > solution.Fitness)
        {
            prevFitness = solution.Fitness;
            solution = GetNewSolution(operators[r.Next(0, nbOperators)].Execute(solution), solution);
            solution.DelEmptyVehicles();
        }
    }

    public void BestOfSelectedOperators(ref Routes solution, List<OperatorName> operatorsName)
    {
        var prevFitness = double.MaxValue;
        var operators = GetOperatorsFromName(operatorsName);
        while (prevFitness > solution.Fitness)
        {
            prevFitness = solution.Fitness;
            var neighbors = new List<(Vehicle, Vehicle, double, (OperatorName , List<int>))>();
            foreach (var op in operators)
            {
                neighbors = neighbors.Concat(op.Execute(solution)).ToList();
            }

            solution = GetNewSolution(neighbors, solution);
            solution.DelEmptyVehicles();
        }
    }

    protected override Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, (OperatorName name, List<int> clientsIndex) operation)> vehicles, Routes solution)
    {
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
        return newRoutes;
    }
}