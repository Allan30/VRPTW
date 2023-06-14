using Riok.Mapperly.Abstractions;
using System.Collections.ObjectModel;
using System.Linq;
using VRPTW.Core;
using VRPTW.Core.Heuristics;
using VRPTW.UI.Enums;
using VRPTW.UI.ViewModels;

namespace VRPTW.UI.Mappers;

[Mapper]
public partial class RoutesMapper
{
    public void RoutesToRoutesViewModel(Routes routes, RoutesViewModel routesViewModel)
    {
        Map(routes, routesViewModel);
        var clientMapper = new ClientMapper();
        var clients = routes.Clients.Select(c => clientMapper.ClientToClientViewModel(c));
        var depot = clientMapper.ClientToClientViewModel(routes.Depot, true);
        routesViewModel.ClientsWithDepot = new ObservableCollection<ClientViewModel>(clients.Prepend(depot));
    }

    [MapperIgnoreSource(nameof(Routes.Clients))]
    [MapperIgnoreSource(nameof(Routes.Depot))]
    [MapperIgnoreTarget(nameof(RoutesViewModel.ClientsWithDepot))]
    private partial void Map(Routes routes, RoutesViewModel routesViewModel);
}

[Mapper]
public partial class VehicleMapper
{
    public partial VehicleViewModel VehicleToVehicleViewModel(Vehicle vehicle);
}

[Mapper]
public partial class ClientMapper
{
    public ClientViewModel ClientToClientViewModel(Client client, bool isDepot = false)
    {
        var clientViewModel = Map(client);
        clientViewModel.IsDepot = isDepot;
        return clientViewModel;
    }

    [MapperIgnoreTarget(nameof(ClientViewModel.IsDepot))]
    private partial ClientViewModel Map(Client client);
}

[Mapper]
public partial class HeuristicStrategyMapper
{
    public HeuristicStrategyBase HeuristicStrategyViewModelToHeuristicStrategyBase(HeuristicStrategyViewModel heuristicStrategy) =>
        heuristicStrategy.HeuristicStrategyType switch
        {
            HeuristicStrategyEnum.Descent => new DescentStrategy(),
            HeuristicStrategyEnum.SimulatedAnnealing => new SimulatedAnnealingStrategy(),
            HeuristicStrategyEnum.Tabu => new TabuStrategy(),
            _ => throw new System.NotImplementedException(),
        };
}