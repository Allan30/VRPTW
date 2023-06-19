using System.Diagnostics;
using VRPTW.Core.Neighborhood;
using VRPTW.Core.Operators;

namespace VRPTW.Core.Heuristics;

public abstract class HeuristicStrategyBase
{
    protected Random Random { get; } = new();
    protected float BestFitness { get; set; } = float.MaxValue;
    protected Routes? BestSolution;
    public int NbSteps { get; set; } = 1_000;
    protected abstract bool LoopConditon { get; }
    public INeighborhoodStrategy NeighborhoodStrategy { get; set; }

    protected HeuristicStrategyBase(INeighborhoodStrategy neighborhoodStrategy)
    {
        NeighborhoodStrategy = neighborhoodStrategy;
    }

    public Routes Calculate(Routes solution, List<OperatorEnum> ops, IProgress<int> progress, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        BestSolution = (Routes)solution.Clone();
        var operators = GetOperatorsFromName(ops);
        while (LoopConditon && !cancellationToken.IsCancellationRequested)
        {
            var neighbors = NeighborhoodStrategy.FindNeighbors(operators, solution);
            solution = GetNewSolution(neighbors, solution, progress);
            solution.DeleteEmptyVehicles();
            var newFitness = solution.Fitness;
            if (newFitness < BestFitness)
            {
                BestFitness = newFitness;
                BestSolution = (Routes)solution.Clone();
            }
        }
        stopwatch.Stop();
        BestSolution.TimeToCalculate = stopwatch.Elapsed;
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