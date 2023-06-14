namespace VRPTW.Core;

public record struct Coordinate(double X, double Y)
{
    public double GetDistance(Coordinate coordinate) =>
        Math.Sqrt(Math.Pow(Math.Abs(X - coordinate.X), 2) + Math.Pow(Math.Abs(Y - coordinate.Y), 2));
}