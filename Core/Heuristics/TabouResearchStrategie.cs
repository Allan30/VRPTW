using VRPTW.AbstractObjects;
using VRPTW.Heuristics;

namespace VRPTW.Concret;

public class TabouResearchStrategy : IStrategy
{
    protected override Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, string operation)> vehicles, Routes solution)
    {
        throw new NotImplementedException();
    }
}