using VRPTW.Concret;

namespace VRPTW.Heuristics;

public interface IStrategy
{
    public void Execute(ref Routes solution);
}