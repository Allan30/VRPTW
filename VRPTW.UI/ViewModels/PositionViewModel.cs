namespace VRPTW.UI.ViewModels;

public record struct PositionViewModel(double X, double Y)
{
    public override string ToString() => $"({X}, {Y})";
}
