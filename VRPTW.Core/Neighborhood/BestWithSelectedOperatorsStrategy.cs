using VRPTW.Core.Operators;

namespace VRPTW.Core.Neighborhood;

public sealed class BestWithSelectedOperatorsStrategy : INeighborhoodStrategy
{
    public List<(Vehicle, Vehicle, double, (OperatorEnum, List<int>))> FindNeighbors(List<OperatorBase> operators, Routes solution)
    {
        var neighbors = new List<(Vehicle, Vehicle, double, (OperatorEnum, List<int>))>();
        foreach (var op in operators)
        {
            neighbors = neighbors.Concat(op.Execute(solution)).ToList();
        }
        return neighbors;
    }
}
