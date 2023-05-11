using System.Runtime.InteropServices;
using VRPTW.AbstractObjects;

namespace VRPTW.Concret;

public class DescentStrategy : IStrategy
{
    public void Execute(ref Routes solution)
    {
        var exchangeInter = new ExchangeOperatorInter();
        var relocateInter = new RelocateOperatorInter();
        var relocateIntra = new RelocateOperatorIntra();
        var exchangeIntra = new ExchangeOperatorIntra();
        var twoOptIntra = new TwoOptOperatorIntra();
        var prevFitness = double.MaxValue;
        var prevSolution = (Routes) solution.Clone();
        var r = new Random();
        while(prevFitness > solution.Fitness)
        {
            prevFitness = solution.Fitness;
            prevSolution = (Routes) solution.Clone();
            Console.WriteLine(prevFitness);
            /*
            switch (r.Next(0, 4))
            {
                case 0:
                    solution = GetNewSolution(relocateInter.Execute(solution), solution);
                    break;
                case 1:
                    solution = GetNewSolution(exchangeInter.Execute(solution), solution);
                    break;
                case 2:
                    solution = GetNewSolution(relocateIntra.Execute(solution), solution);
                    break;
                case 3:
                    solution = GetNewSolution(exchangeIntra.Execute(solution), solution);
                    break;
            }*/
            solution = GetNewSolution(relocateInter.Execute(solution).Concat(exchangeInter.Execute(solution).Concat(exchangeIntra.Execute(solution).Concat(relocateIntra.Execute(solution).Concat(twoOptIntra.Execute(solution))))).ToList(), solution);
            //solution = GetNewSolution(twoOptIntra.Execute(solution), solution);
            solution.DelEmptyVehicles();
            
        }
    }

    public Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta)> vehicles, Routes solution)
    {
        var newRoutes = (Routes) solution.Clone();
        var bestOperation = vehicles.MaxBy(v => v.delta);
        if (bestOperation.delta <= 0)
        {
            return newRoutes;
        }
        Console.WriteLine(bestOperation.delta);
        
        Console.WriteLine(solution.Vehicles[bestOperation.trg.Id].ToStringClient());
        Console.WriteLine(bestOperation.src.ToStringClient());
        
        newRoutes.ChangeVehicle(bestOperation.src);
        newRoutes.ChangeVehicle(bestOperation.trg);
        bestOperation.trg.IsCorrect();
        return newRoutes;
    }
}