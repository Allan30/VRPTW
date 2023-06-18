using VRPTW.Core.Neighborhood;
using VRPTW.Core.Operators;

namespace VRPTW.Core.Heuristics;

public abstract class HeuristicStrategyBase
{
    protected Random Random { get; } = new();
    protected double BestFitness { get; set; } = double.MaxValue;
    protected Routes? BestSolution;
    public int NbSteps { get; set; } = 1_000;

    protected abstract bool LoopConditon { get; }

    public NeighborhoodStrategyEnum NeighborhoodStrategy { get; set; } = NeighborhoodStrategyEnum.Best;

    public Routes Calculate(Routes solution, List<OperatorEnum> ops, IProgress<int> progress, CancellationToken cancellationToken)
    {
        if (NeighborhoodStrategy == NeighborhoodStrategyEnum.Best)
        {
            return BestOfSelectedOperators(solution, ops, progress, cancellationToken);
        }
        else if (NeighborhoodStrategy == NeighborhoodStrategyEnum.Random)
        {
            return RandomWithSelectedOperators(solution, ops, progress, cancellationToken);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private Routes RandomWithSelectedOperators(Routes solution, List<OperatorEnum> ops, IProgress<int> progress, CancellationToken cancellationToken)
    {
        var operators = GetOperatorsFromName(ops);
        while (LoopConditon && !cancellationToken.IsCancellationRequested)
        {
            solution = GetNewSolution(operators[Random.Next(0, operators.Count)].Execute(solution), solution, progress);
            solution.DeleteEmptyVehicles();
            var newFitness = solution.Fitness;
            if (newFitness < BestFitness)
            {
                BestFitness = newFitness;
                BestSolution = (Routes)solution.Clone();
            }
        }
        return solution;
    }
    
    private Routes BestOfSelectedOperators(Routes solution, List<OperatorEnum> ops, IProgress<int> progress, CancellationToken cancellationToken)
    {
        BestSolution = (Routes)solution.Clone();
        var operators = GetOperatorsFromName(ops);
        while (LoopConditon && !cancellationToken.IsCancellationRequested)
        {
            var neighbors = new List<(Vehicle, Vehicle, double, (OperatorEnum, List<int>))>();
            foreach (var op in operators)
            {
                neighbors = neighbors.Concat(op.Execute(solution)).ToList();
            }
            solution = GetNewSolution(neighbors, solution, progress);
            solution.DeleteEmptyVehicles();
            var newFitness = solution.Fitness;
            if (newFitness < BestFitness)
            {
                BestFitness = newFitness;
                BestSolution = (Routes)solution.Clone();
            }
        }
        return BestSolution;
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
        Routes solution,
        IProgress<int> progress
    );
}