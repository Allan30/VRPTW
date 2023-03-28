namespace VRPTW.Concret;

public record struct Coordinate(int X, int Y)
{
    public int GetDistance(Coordinate coordinate) =>
        Math.Abs(X - coordinate.X) + Math.Abs(Y - coordinate.Y);
}