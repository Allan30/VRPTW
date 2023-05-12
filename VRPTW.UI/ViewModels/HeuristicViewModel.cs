namespace VRPTW.UI.ViewModels;

public sealed class HeuristicViewModel
{
    public string Name { get; set; }
    public HeuristicViewModel(string name)
    {
        Name = name;
    }
    public override string ToString() => Name;
}
