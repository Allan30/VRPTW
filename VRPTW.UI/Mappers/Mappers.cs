using Riok.Mapperly.Abstractions;
using VRPTW.Concret;
using VRPTW.UI.ViewModels;

namespace VRPTW.UI.Mappers;

[Mapper]
public partial class RoutesMapper
{
    public partial void RoutesToRoutesViewModel(Routes routes, RoutesViewModel routesViewModel);
}

[Mapper]
public partial class VehicleMapper
{
    public partial VehicleViewModel VehicleToVehicleViewModel(Vehicle vehicle);
}
