using VRPTW.Core.Neighborhood;
using VRPTW.Core.Operators;

namespace VRPTW.Core.Heuristics;

public sealed class SimulatedAnnealingStrategy : HeuristicStrategyBase
{
    private int _currentStep = 0;
    protected override bool LoopConditon => _currentStep < NbSteps;
    public double InitialTemperature { get; init; }
    public double Temperature { get; private set; }
    public double Alpha { get; init; }

    public SimulatedAnnealingStrategy(INeighborhoodStrategy neighborhoodStrategy, int nbSteps, double initialTemperature, double alpha) : base(neighborhoodStrategy)
    {
        NbSteps = nbSteps;
        InitialTemperature = initialTemperature;
        Temperature = initialTemperature;
        Alpha = alpha;
    }

    protected override Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, (OperatorEnum name, List<int> clientsIndex) operation)> vehicles, Routes solution, IProgress<int> progress)
    {
        progress.Report(++_currentStep);
        if (vehicles.Count == 0)
        {
            return solution;
        }
        var newRoutes = (Routes)solution.Clone();
        var theOperation = vehicles[Random.Next(0, vehicles.Count)];
        if (theOperation.delta > 0 || Math.Exp(theOperation.delta / Temperature) > Random.NextDouble())
        {
            newRoutes.ChangeVehicle(theOperation.src);
            newRoutes.ChangeVehicle(theOperation.trg);
        }
        Temperature *= Alpha;
        Operators.Add(GetOperatorsFromName(new List<OperatorEnum> { theOperation.operation.name })[0]);
        return newRoutes;
    }
}