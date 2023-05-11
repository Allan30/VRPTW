using System.Runtime.InteropServices;
using VRPTW.AbstractObjects;

namespace VRPTW.Concret;

public class DescentStrategy : IStrategy
{
    public void Execute(ref Routes solution)
    {
        var exchangeInter = new ExchangeOperatorInter();
        var relocateInter = new RelocateOperatorInter();
        var prevFitness = double.MaxValue;
        var prevSolution = (Routes) solution.Clone();
        var r = new Random();
        while(prevFitness > solution.GetFitness())
        {
            prevFitness = solution.GetFitness();
            prevSolution = (Routes) solution.Clone();
            Console.WriteLine(prevFitness);
            switch (r.Next(0, 2))
            {
                case 0:
                    relocateInter.Execute(solution);
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
            }
        }
        solution = prevSolution;
    }
}