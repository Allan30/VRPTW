using VRPTW.Concret;

namespace VRPTW.AbstractObjects;

public interface IStrategy
{
    public void Execute(ref Routes solution);
}