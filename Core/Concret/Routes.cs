using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VRPTW.AbstractObjects;

namespace VRPTW.Concret;

public class Routes : ISolution, ICloneable
{
    public Client Depot { get; set; }
    public List<Client> Clients { get; set; }
    public List<Vehicle> Vehicles { get; set; }
    public int MaxCapacity { get; set; }
    
    public Routes()
    {
        MaxCapacity = -1;
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
        Vehicles.Add(new Vehicle(Vehicles.Count, MaxCapacity, Depot));
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

    public void GenerateRandomSolution()
    {
        var values = Enumerable.Range(0, Clients.Count).Select(x => x).ToList();
        var rand = new Random();
        var shuffled = values.OrderBy(_ => rand.Next()).ToList();
        var currentVehicle = new Vehicle(Vehicles.Count, MaxCapacity, Depot);
        foreach (var client in shuffled.Select(index => Clients[index]))
        {
            if (!currentVehicle.AddClient(client))
            {
                Vehicles.Add(currentVehicle);
                currentVehicle = new Vehicle(Vehicles.Count, MaxCapacity, Depot);
                currentVehicle.AddClient(client);
            };
        }
        Vehicles.Add(currentVehicle);
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
            sb.Append((int) vehicle.GetTravelledDistance());
            sb.Append("\", \"Capacity\":");
            sb.Append(vehicle.CurrentCapacity);
            sb.Append("}, \n");
        }

        sb.Remove(sb.Length - 3, 3);

        sb.Append("]");

        return sb.ToString();
    }

    public object Clone()
    {
        var routes = new Routes();
        routes.Clients = Clients;
        routes.Depot = Depot;
        routes.MaxCapacity = MaxCapacity;
        routes.Vehicles = Vehicles.Select(vehicle => (Vehicle) vehicle.Clone()).ToList();
        return routes;
    }
    
    public int GetNbClients()
    {
        return Vehicles.Sum(vehicle => vehicle.GetNbClients());
    }
}