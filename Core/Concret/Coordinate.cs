namespace VRPTW.Concret;

public record struct Coordinate(double X, double Y)
{
    
    public double GetDistance(Coordinate coordinate) =>
        Math.Sqrt(Math.Pow(Math.Abs(X - coordinate.X), 2) + Math.Pow(Math.Abs(Y - coordinate.Y), 2));
    
    public override string ToString() => $"[{X}, {Y}]";
}