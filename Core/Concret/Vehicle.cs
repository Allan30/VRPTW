using System.Text;

namespace VRPTW.Concret;

public class Vehicle : ICloneable
{
    public int Id;
    public static int MaxCapacity;
    public int CurrentCapacity;
    public LinkedList<Client> Clients;
    
    public Vehicle(int id, int capacity, Client depot)
    {
        Id = id;
        CurrentCapacity = 0;
        MaxCapacity = capacity;
        Clients = new LinkedList<Client>();
        Clients.AddFirst(depot);
        Clients.AddLast(depot);
    }

    public bool AddClient(Client client)
    {
        if (client.Demand + CurrentCapacity > MaxCapacity || GetTravelledDistance() + client.GetDistance(Clients.Last.Value) + client.GetDistance(Clients.Last.Previous.Value) - Clients.Last.Value.GetDistance(Clients.Last.Previous.Value) > 230)
        {
            return false;
        }
        Clients.AddBefore(Clients.Last, client);
        CurrentCapacity += client.Demand;
        return true;
    }
    
    public double GetTravelledDistance()
    {
        return Clients.Zip(Clients.Skip(1), (prevClient, nextClient) => prevClient.Coordinate.GetDistance(nextClient.Coordinate)).Sum();
    }

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
        return deltaDist + GetTravelledDistance() <= 230 && deltaCap + CurrentCapacity <= MaxCapacity;
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
    
    public int GetNbClients()
    {
        return Clients.Count - 2;
    }
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("[");
        foreach (var client in Clients)
        {
            sb.Append(client.Coordinate);
            sb.Append(",");
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append("]");
        return sb.ToString();
    }

    public string ToStringClient()
    {
        var sb = new StringBuilder();
        sb.Append("[");
        foreach (var client in Clients)
        {
            sb.Append(client.Id);
            sb.Append(",");
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append("]");
        return sb.ToString();
    }
}