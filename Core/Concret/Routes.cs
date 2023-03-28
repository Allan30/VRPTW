using VRPTW.AbstractObjects;

namespace VRPTW.Concret;

public class Routes : ISolution
{
    public Client Depot { get; set; }
    public List<Client> Clients { get; set; }
    public List<Vehicle> Vehicles { get; set; }
    public static int Capacity { get; set; }
    
    public Routes(Client depot, List<Client> clients)
    {
        Depot = depot;
        Clients = clients;
    }
    
    public void AddVehicle()
    {
        Vehicles.Add(new Vehicle(Vehicles.Count, Capacity, Depot));
    }
    
    public float GetFitness()
    {
        return Vehicles.Sum(vehicle => vehicle.GetTravelledDistance());
    }

    public List<ISolution> GetNeighbours()
    {
        //TODO: Implement this method
        throw new System.NotImplementedException();
    }

    public void GenerateRandomSolution()
    {
        var values = Enumerable.Range(0, Clients.Count).Select(x => x).ToList();
        var rand = new Random();
        var shuffled = values.OrderBy(_ => rand.Next()).ToList();
        
        var currentVehicle = new Vehicle(Vehicles.Count, Capacity, Depot);
        foreach (var client in values.Select(index => Clients[index]))
        {
            if (!currentVehicle.AddClient(client))
            {
                Vehicles.Add(currentVehicle);
                currentVehicle = new Vehicle(Vehicles.Count, Capacity, Depot);
            };
        }
    }
}