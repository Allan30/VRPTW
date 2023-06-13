using VRPTW.Concret;

namespace VRPTWCore.NeighborhoodStrategies;

//public class RandomStrategy : INeighborhoodStrategy
//{
//    public void Calculate(ref Routes solution, List<OperatorEnum> operatorsNames, bool loopCondition)
//    {
//        var operators = GetOperatorsFromName(operatorsNames).ToList();
//        var nbOperators = operators.Count;
//        var r = new Random();
//        while (loopConditon)
//        {
//            solution = GetNewSolution(operators[r.Next(0, nbOperators)].Execute(solution), solution);
//            solution.DelEmptyVehicles();
//            var newFitness = solution.Fitness;
//            if (newFitness < BestFitness)
//            {
//                BestFitness = newFitness;
//                bestSolution = (Routes)solution.Clone();
//            }
//        }
//    }
//}
