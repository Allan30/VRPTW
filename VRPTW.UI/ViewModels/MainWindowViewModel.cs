using CommunityToolkit.Mvvm.ComponentModel;

namespace VRPTW.UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public RoutesViewModel RoutesViewModel { get; set; } = new();
}