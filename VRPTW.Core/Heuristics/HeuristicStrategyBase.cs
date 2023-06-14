using VRPTW.Core.Neighborhood;
using VRPTW.Core.Operators;

namespace VRPTW.Core.Heuristics;

public abstract class HeuristicStrategyBase
{
    protected double BestFitness { get; set; } = double.MaxValue;
    protected Routes BestSolution = new();
    public int NbSteps { get; set; } = 1_000;

    protected abstract bool LoopConditon { get; }

    public NeighborhoodStrategyEnum NeighborhoodStrategy { get; set; } = NeighborhoodStrategyEnum.Best;

    public void Calculate(ref Routes solution, List<OperatorEnum> ops)
    {
        if (NeighborhoodStrategy == NeighborhoodStrategyEnum.Best)
        {
            BestOfSelectedOperators(ref solution, ops);
        }
        else
        {
            RandomWithSelectedOperators(ref solution, ops);
        }
    }

    private void RandomWithSelectedOperators(ref Routes solution, List<OperatorEnum> ops)
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
                BestSolution = (Routes)solution.Clone();
            }
        }
    }

    private void BestOfSelectedOperators(ref Routes solution, List<OperatorEnum> ops)
    {
        BestSolution = (Routes)solution.Clone();
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
                BestSolution = (Routes)solution.Clone();
            }
        }
        Console.WriteLine(solution.Fitness);
        solution = BestSolution;
    }

    protected static List<OperatorBase> GetOperatorsFromName(List<OperatorEnum> operatorsName)
    {
        var operators = new List<OperatorBase>();
        foreach (var operatorName in operatorsName.Distinct())
        {
            OperatorBase @operator = operatorName switch
            {
                OperatorEnum.ExchangeInter => new ExchangeInterOperator(),
                OperatorEnum.ExchangeIntra => new ExchangeIntraOperator(),
                OperatorEnum.RelocateInter => new RelocateInterOperator(),
                OperatorEnum.RelocateIntra => new RelocateIntraOperator(),
                OperatorEnum.TwoOpt => new TwoOptIntraOperator(),
                _ => throw new NotImplementedException(),
            };

            operators.Add(@operator);
        }

        return operators;
    }

    protected abstract Routes GetNewSolution(
        List<(Vehicle src, Vehicle trg, double delta, (OperatorEnum name, List<int> clientsIndex) operation)> vehicles,
        Routes solution
    );
}