namespace VRPTW.UI.ViewModels;

public record struct CoordinateViewModel(double X, double Y)
{
    public override string ToString() => $"({X}, {Y})";
}
