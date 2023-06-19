using VRPTW.Core.Neighborhood;
using VRPTW.Core.Operators;

namespace VRPTW.Core.Heuristics;

public class SimulatedAnnealingStrategy : HeuristicStrategyBase
{
    public double Temperature { get; set; } = 10_000;
    public double Alpha { get; set; } = 0.985;
    private int _currentStep = 0;

    public SimulatedAnnealingStrategy(INeighborhoodStrategy neighborhoodStrategy) : base(neighborhoodStrategy)
    {
    }

    protected override bool LoopConditon => _currentStep < NbSteps;

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