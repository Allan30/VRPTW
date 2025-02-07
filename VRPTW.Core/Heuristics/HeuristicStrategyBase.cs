using System.Diagnostics;
using VRPTW.Core.Neighborhood;
using VRPTW.Core.Operators;

namespace VRPTW.Core.Heuristics;

public abstract class HeuristicStrategyBase
{
    public List<OperatorBase> Operators { get; protected set; } = new();
    public List<double> BestFitnesses { get; protected set; } = new();
    public List<double> Fitnesses { get; protected set; } = new();
    
    public List<int> NumbersOfVehicles { get; } = new();
    protected Random Random { get; } = new();
    public double BestFitness { get; protected set; } = double.MaxValue;
    protected Routes? BestSolution;
    public int NbSteps { get; init; }
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
            if (neighbors.Count == 0)
            {
                break;
            }
            solution = GetNewSolution(neighbors, solution, progress);
            solution.DeleteEmptyVehicles();
            var newFitness = solution.Fitness;
            if (newFitness < BestFitness)
            {
                BestFitness = newFitness;
                BestSolution = (Routes)solution.Clone(); 
            }
            BestFitnesses.Add(BestFitness); 
            Fitnesses.Add(newFitness);
            NumbersOfVehicles.Add(solution.Vehicles.Count);
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