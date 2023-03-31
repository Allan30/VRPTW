using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VRPTW.AbstractObjects;

namespace VRPTW.Concret;

public class Routes : ISolution
{
    public Client Depot { get; set; }
    public List<Client> Clients { get; set; }
    public List<Vehicle> Vehicles { get; set; }
    public int Capacity { get; set; }
    
    public Routes()
    {
        Capacity = -1;
        Clients = new List<Client>();
        Vehicles = new List<Vehicle>();
    }
    public Routes(Client depot, List<Client> clients)
    {
        Depot = depot;
        Clients = clients;
    }
    
    public void AddVehicle()
    {
        Vehicles.Add(new Vehicle(Vehicles.Count, Capacity, Depot));
    }
    
    public double GetFitness()
    {
        return Vehicles.Sum(vehicle => vehicle.GetTravelledDistance());
    }

    public List<ISolution> GetNeighbours()
    {
        //TODO: Implement this method
        throw new System.NotImplementedException();
    }

    public void SetBestRelocate(List<Client> tabouList)
    {
        var currentFitness = GetFitness();
        var trans = new Transformations();
        (LinkedList<Client> relocated, Client node) best = (Vehicles[0].Clients, null);
        var bestDelta = double.MinValue;
        var bestId = 0;
        foreach (var vehicle in Vehicles)
        {
            var currentDistance = vehicle.GetTravelledDistance();
            var relocationsOfClients = trans.RelocateIntra(vehicle.Clients, tabouList);
            foreach (var relocation in relocationsOfClients)
            {
                var distance = relocation.relocated.Zip(relocation.relocated.Skip(1), (prevClient, nextClient) => prevClient.Coordinate.GetDistance(nextClient.Coordinate)).Sum();
                if (currentDistance - distance <= bestDelta) continue;
                bestDelta = currentDistance - distance;
                best = relocation;
                bestId = vehicle.Id;
            }
        }
        Vehicles[bestId].Clients = best.relocated;
        if (currentFitness <= GetFitness())
        {
            if(tabouList.Count > 10) tabouList.RemoveAt(0);
            tabouList.Add(best.node);
        }
        
        
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

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("[");
        foreach (var vehicle in Vehicles)
        {
            sb.Append("{\"Vehicle\": ");
            sb.Append("\"");
            sb.Append(vehicle.Id);
            sb.Append("\"");
            sb.Append(", \"Clients\": [");
            foreach (var client in vehicle.Clients)
            {
                sb.Append("\"");
                sb.Append(client.Id);
                sb.Append("\"");
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);

            sb.Append("], \"Points\": [");
            foreach (var client in vehicle.Clients)
            {
                sb.Append("[");
                sb.Append(client.Coordinate.X);
                sb.Append(", ");
                sb.Append(client.Coordinate.Y);
                sb.Append("], ");
            }
            sb.Remove(sb.Length - 2, 2);
            
            sb.Append("], \"Distance\":\"");
            sb.Append(vehicle.GetTravelledDistance());
            sb.Append("\", \"Capacity\":");
            sb.Append(vehicle.Capacity);
            sb.Append("}, \n");
        }

        sb.Remove(sb.Length - 3, 3);

        sb.Append("]");

        return sb.ToString();
    }
}