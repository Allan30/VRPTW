using VRPTW.Core.Operators;

namespace VRPTW.Core.Neighborhood;

public interface INeighborhoodStrategy
{
    public List<(Vehicle, Vehicle, double, (OperatorEnum, List<int>))> FindNeighbors(List<OperatorBase> operators, Routes solution);
}
