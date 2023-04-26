using System.Text;

namespace VRPTW.Concret;

public class Vehicle : ICloneable
{
    public int Id { get; set; }
    public int MaxCapacity { get; init; }
    public int CurrentCapacity { get; private set; }
    public LinkedList<Client> Clients { get; set; }
    
    public Vehicle(int id, int capacity, Client depot)
    {
        Id = id;
        CurrentCapacity = 0;
        MaxCapacity = capacity;
        Clients = new LinkedList<Client>();
        Clients.AddFirst(depot);
        Clients.AddLast(new Client(depot.Id+"_bis", depot.Coordinate, depot.Demand, depot.DueTime));
    }

    public bool AddClient(Client client)
    {
        if (client.Demand + CurrentCapacity > MaxCapacity || TravelledDistance + client.GetDistance(Clients.Last.Value) + client.GetDistance(Clients.Last.Previous.Value) - Clients.Last.Value.GetDistance(Clients.Last.Previous.Value) > 230)
        {
            return false;
        }
        Clients.AddBefore(Clients.Last, client);
        CurrentCapacity += client.Demand;
        return true;
    }
    
    public double TravelledDistance => Clients.Zip(Clients.Skip(1), (prevClient, nextClient) => prevClient.Coordinate.GetDistance(nextClient.Coordinate)).Sum();

    public object Clone()
    {
        var vehicle = new Vehicle(Id, MaxCapacity, Clients.First.Value)
        {
            CurrentCapacity = CurrentCapacity,
            Clients = new LinkedList<Client>(Clients)
        };
        return vehicle;
    }
    
    public bool StayCorrect(double deltaDist, double deltaCap)
    {
        return deltaDist + TravelledDistance <= 230 && deltaCap + CurrentCapacity <= MaxCapacity;
    }
    
    public void RemoveClient(LinkedListNode<Client> client)
    {
        Clients.Remove(client);
        CurrentCapacity -= client.Value.Demand;
    }
    
    public void AddClientAfter(LinkedListNode<Client>  after, LinkedListNode<Client> client)
    {
        Clients.AddAfter(after, client);
        CurrentCapacity += client.Value.Demand;
    }
    
    public void AddClientBefore(LinkedListNode<Client> before, LinkedListNode<Client> client)
    {
        Clients.AddBefore(before, client);
        CurrentCapacity += client.Value.Demand;
    }
    
    public int NbClients => Clients.Count - 2;
    
    public int GetTotalDemand => Clients.Sum(client => client.Demand);
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        foreach (var client in Clients)
        {
            sb.Append(client.Coordinate);
            sb.Append(',');
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append(']');
        return sb.ToString();
    }

    public string ToStringClient()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        foreach (var client in Clients)
        {
            sb.Append(client.Id);
            sb.Append(',');
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append(']');
        return sb.ToString();
    }
}