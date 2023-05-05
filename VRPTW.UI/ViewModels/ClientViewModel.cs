namespace VRPTW.UI.ViewModels;

public sealed class ClientViewModel
{
    public bool IsDepot { get; set; }
    public string Id { get; set; }
    public CoordinateViewModel Coordinate { get; set; }
    public int ReadyTime { get; set; }
    public int DueTime { get; set; }
    public int Demand { get; set; }
    public int Service { get; set; }
}
