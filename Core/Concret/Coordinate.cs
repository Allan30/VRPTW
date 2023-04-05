namespace VRPTW.Concret;

public record struct Coordinate(float X, float Y)
{
    
    public double GetDistance(Coordinate coordinate) =>
        Math.Sqrt(Math.Pow(Math.Abs(X - coordinate.X), 2) + Math.Pow(Math.Abs(Y - coordinate.Y), 2));
    
    public override string ToString() => $"[{X}, {Y}]";
}