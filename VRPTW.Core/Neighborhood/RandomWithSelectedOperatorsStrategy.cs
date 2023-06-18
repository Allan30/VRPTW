using VRPTW.Core.Operators;

namespace VRPTW.Core.Neighborhood;

public sealed class RandomWithSelectedOperatorsStrategy : INeighborhoodStrategy
{
    private readonly Random _random;

    public RandomWithSelectedOperatorsStrategy()
    {
        _random = new Random();
    }

    public List<(Vehicle, Vehicle, double, (OperatorEnum, List<int>))> FindNeighbors(List<OperatorBase> operators, Routes solution) =>
        (operators[_random.Next(0, operators.Count)].Execute(solution));
}
