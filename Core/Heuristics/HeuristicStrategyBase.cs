using VRPTW.Concret;
using VRPTWCore.NeighborhoodStrategies;

namespace VRPTW.Heuristics;

public abstract class HeuristicStrategyBase
{
    protected double BestFitness { get; set; } = double.MaxValue;
    protected Routes BestSolution = new();

    protected abstract bool LoopConditon { get; }
    public void RandomWithSelectedOperators(ref Routes solution, List<OperatorEnum> ops)
    {
        var r = new Random();
        var operators = GetOperatorsFromName(ops);
        while (LoopConditon)
        {
            solution = GetNewSolution(operators[r.Next(0, operators.Count)].Execute(solution), solution);
            solution.DeleteEmptyVehicles();
            var newFitness = solution.Fitness;
            if (newFitness < BestFitness)
            {
                BestFitness = newFitness;
                BestSolution = (Routes) solution.Clone();
            }
        }
    }

    public void BestOfSelectedOperators(ref Routes solution, List<OperatorEnum> ops)
    {
        BestSolution = (Routes) solution.Clone();
        var operators = GetOperatorsFromName(ops);
        while (LoopConditon)
        {
            var neighbors = new List<(Vehicle, Vehicle, double, (OperatorEnum, List<int>))>();
            foreach (var op in operators)
            {
                neighbors = neighbors.Concat(op.Execute(solution)).ToList();
            }
            solution = GetNewSolution(neighbors, solution);
            solution.DeleteEmptyVehicles();
            var newFitness = solution.Fitness;
            if (newFitness < BestFitness)
            {
                BestFitness = newFitness;
                BestSolution = (Routes) solution.Clone();
            }
        }
        Console.WriteLine(solution.Fitness);
        solution = BestSolution;
    }

    protected static List<OperatorBase> GetOperatorsFromName(List<OperatorEnum> operatorsName)
    {
        var operators = new List<OperatorBase>();
        foreach (var operatorName in operatorsName)
        {
            switch (operatorName)
            {
                case OperatorEnum.ExchangeInter:
                    operators.Add(new ExchangeOperatorInter());
                    break;

                case OperatorEnum.ExchangeIntra:
                    operators.Add(new ExchangeOperatorIntra());
                    break;

                case OperatorEnum.RelocateInter:
                    operators.Add(new RelocateOperatorInter());
                    break;

                case OperatorEnum.RelocateIntra:
                    operators.Add(new RelocateOperatorIntra());
                    break;

                case OperatorEnum.TwoOpt:
                    operators.Add(new TwoOptOperatorIntra());
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(operatorName));
            }
        }

        return operators;
    }

    protected abstract Routes GetNewSolution(
        List<(Vehicle src, Vehicle trg, double delta, (OperatorEnum name, List<int> clientsIndex) operation)> vehicles,
        Routes solution
    );
}