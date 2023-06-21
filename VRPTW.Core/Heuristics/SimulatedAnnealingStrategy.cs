using VRPTW.Core.Neighborhood;
using VRPTW.Core.Operators;

namespace VRPTW.Core.Heuristics;

public sealed class SimulatedAnnealingStrategy : HeuristicStrategyBase
{
    private int _currentStep = 0;
    protected override bool LoopConditon => _currentStep < NbSteps;
    public double InitialTemperature { get; set; } = 10_000;
    public double Temperature { get; private set; }
    public double Alpha { get; set; } = 0.985;

    public SimulatedAnnealingStrategy(INeighborhoodStrategy neighborhoodStrategy) : base(neighborhoodStrategy)
    {
    }

    protected override Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, (OperatorEnum name, List<int> clientsIndex) operation)> vehicles, Routes solution, IProgress<int> progress)
    {
        progress.Report(++_currentStep);
        Temperature = InitialTemperature;
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