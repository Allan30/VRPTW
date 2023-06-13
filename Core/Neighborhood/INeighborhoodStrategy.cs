using VRPTW.Concret;

namespace VRPTWCore.NeighborhoodStrategies;

public interface INeighborhoodStrategy
{
    public void Calculate(ref Routes solution, List<OperatorEnum> operatorsNames);
}
