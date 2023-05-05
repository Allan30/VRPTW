using Riok.Mapperly.Abstractions;
using System.Collections.ObjectModel;
using System.Linq;
using VRPTW.Concret;
using VRPTW.UI.ViewModels;

namespace VRPTW.UI.Mappers;

[Mapper]
public partial class RoutesMapper
{
    public void MapRoutesToRoutesViewModel(Routes routes, RoutesViewModel routesViewModel)
    {
        RoutesToRoutesViewModel(routes, routesViewModel);
        var clientMapper = new ClientMapper();
        var clients = routes.Clients.Select(clientMapper.ClientToClientViewModel);
        var depot = clientMapper.ClientToClientViewModel(routes.Depot);
        routesViewModel.ClientsWithDepot = new ObservableCollection<ClientViewModel>(clients.Prepend(depot));
    }

    private partial void RoutesToRoutesViewModel(Routes routes, RoutesViewModel routesViewModel);
}

[Mapper]
public partial class VehicleMapper
{
    public partial VehicleViewModel VehicleToVehicleViewModel(Vehicle vehicle);
}

[Mapper]
public partial class ClientMapper
{
    public partial ClientViewModel ClientToClientViewModel(Client client);
}
