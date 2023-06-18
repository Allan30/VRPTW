using Riok.Mapperly.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using VRPTW.Core;
using VRPTW.Core.Heuristics;
using VRPTW.Core.Neighborhood;
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
    public HeuristicStrategyBase HeuristicStrategyViewModelToHeuristicStrategyBase(HeuristicStrategyViewModel heuristicStrategyViewModel, NeighborhoodStrategyEnum neighborhoodStrategyType)
    {
        INeighborhoodStrategy neighborhoodStrategy = neighborhoodStrategyType switch
        {
            NeighborhoodStrategyEnum.Random => new RandomWithSelectedOperatorsStrategy(),
            NeighborhoodStrategyEnum.Best => new BestWithSelectedOperatorsStrategy(),
            _ => throw new NotImplementedException()
        };
        HeuristicStrategyBase heuristicStrategy = heuristicStrategyViewModel.HeuristicStrategyType switch
        {
            HeuristicStrategyEnum.Descent => new DescentStrategy(neighborhoodStrategy),
            HeuristicStrategyEnum.SimulatedAnnealing => new SimulatedAnnealingStrategy(neighborhoodStrategy) { NbSteps = heuristicStrategyViewModel.NbSteps, Alpha = heuristicStrategyViewModel.Alpha },
            HeuristicStrategyEnum.Tabu => new TabuStrategy(neighborhoodStrategy) { NbSteps = heuristicStrategyViewModel.NbSteps },
            _ => throw new NotImplementedException(),
        };
        return heuristicStrategy;
    }
        
}