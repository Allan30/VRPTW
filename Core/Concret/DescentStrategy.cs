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
        var prevFitness = double.MaxValue;
        var prevSolution = (Routes) solution.Clone();
        var r = new Random();
        while(prevFitness > solution.GetFitness())
        {
            prevFitness = solution.GetFitness();
            prevSolution = (Routes) solution.Clone();
            Console.WriteLine(prevFitness);
            switch (r.Next(0, 1))
            {
                case 0:
                    solution = GetNewSolution(relocateInter.Execute(solution), solution);
                    if(prevFitness <= solution.GetFitness()){
                        solution = (Routes) prevSolution.Clone();
                        exchangeInter.Execute(solution);
                    }
                    break;
                case 1:
                    exchangeInter.Execute(solution);
                    if(prevFitness <= solution.GetFitness()){
                        solution = (Routes) prevSolution.Clone();
                        relocateInter.Execute(solution);
                    }
                    break;
                case 2:
                    relocateIntra.Execute(solution);
                    if(prevFitness <= solution.GetFitness()){
                        solution = (Routes) prevSolution.Clone();
                        exchangeInter.Execute(solution);
                    }

                    break;
            }
            solution.DelEmptyVehicles();
        }
        solution = prevSolution;
    }

    public Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta)> vehicles, Routes solution)
    {
        var newRoutes = (Routes) solution.Clone();
        var bestOperation = vehicles.MaxBy(v => v.delta);
        newRoutes.ChangeVehicle(bestOperation.src);
        newRoutes.ChangeVehicle(bestOperation.trg);
        return newRoutes;
    }
}