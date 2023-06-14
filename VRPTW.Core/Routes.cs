using VRPTW.Core.Extensions;

namespace VRPTW.Core;

public class Routes : ICloneable
{
    public Client Depot { get; set; }
    public List<Client> Clients { get; set; }
    public List<Vehicle> Vehicles { get; set; } = new();
    public int MaxCapacity { get; set; }

    public Routes(Client depot, List<Client> clients, int maxCapacity)
    {
        Depot = depot;
        Clients = clients;
        MaxCapacity = maxCapacity;
    }

    private Routes(Client depot, List<Client> clients, int maxCapacity, List<Vehicle> vehicles) : this(depot, clients, maxCapacity)
    {
        Vehicles = vehicles;
    }

    public void AddVehicle()
    {
        Vehicles.Add(new Vehicle(Vehicles.Count, MaxCapacity, Depot));
    }

    public void DeleteEmptyVehicles()
    {
        Vehicles.RemoveAll(vehicle => vehicle.Clients.Count == 2);
        for (var i = 0; i < Vehicles.Count; i++)
        {
            Vehicles[i].Id = i;
        }
    }

    public void ChangeVehicle(Vehicle vehicle)
    {
        Vehicles[vehicle.Id] = vehicle;
    }
    public double Fitness => Vehicles.Sum(vehicle => vehicle.TravelledDistance);

    public void GenerateRandomSolution()
    {
        if (Vehicles.Any())
        {
            Vehicles.Clear();
        }

        var clientIndices = Enumerable.Range(0, Clients.Count).ToList();
        clientIndices.Shuffle();
        foreach (var client in clientIndices.Select(index => Clients[index]))
        {
            var added = false;
            foreach (var vehicle in Vehicles)
            {
                if (vehicle.TryAddClientWithWindow(client))
                {
                    added = true;
                    break;
                }
            }
            if (!added)
            {
                Vehicles.Add(new Vehicle(Vehicles.Count, MaxCapacity, Depot));
                Vehicles[Vehicles.Count - 1].TryAddClientWithWindow(client);
            }
        }
    }

    public object Clone() =>
        new Routes(
            Depot,
            Clients = Clients.Select(client => (Client)client.Clone()).ToList(),
            MaxCapacity,
            Vehicles = Vehicles.Select(vehicle => (Vehicle)vehicle.Clone()).ToList()
        );

    public int NbClients => Vehicles.Sum(vehicle => vehicle.NbClients);

    public int TotalDemand => Vehicles.Sum(vehicle => vehicle.TotalDemand);

    public int Capacity => Vehicles.Sum(vehicle => vehicle.CurrentCapacity);
}