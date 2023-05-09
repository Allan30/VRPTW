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
        Clients.AddLast(new Client(depot.Id+"_bis", depot.Coordinate, depot.ReadyTime, depot.DueTime));
    }

    public bool AddClient(Client client)
    {
        if (client.Demand + CurrentCapacity > MaxCapacity || TravelledDistance + client.GetDistance(Clients.Last.Value) + client.GetDistance(Clients.Last.Previous.Value) - Clients.Last.Value.GetDistance(Clients.Last.Previous.Value) > Clients.First.Value.DueTime)
        {
            return false;
        }
        Clients.AddBefore(Clients.Last, client);
        CurrentCapacity += client.Demand;
        return true;
    }

    public bool AddClientWithWindow(Client client)
    {
        var timeToReach = client.TimeToReachAfter(Clients.Last.Previous.Value);
        if (timeToReach >= 0)
        {
            client.TimeOnIt = timeToReach;
            if (Clients.Last.Value.TimeToReachAfter(client) >= 0 &&
                Clients.Last.Value.TimeToReachAfter(client) <= Clients.Last.Value.DueTime &&
                client.Demand + CurrentCapacity <= MaxCapacity)
            {
                Clients.AddBefore(Clients.Last, client);
                CurrentCapacity += client.Demand;
                return true;
            }
        }
        return false;
    }
    
    public double TravelledDistance => Clients.Zip(Clients.Skip(1), (prevClient, nextClient) => prevClient.GetDistance(nextClient)).Sum();
    
    public double RouteTime => Clients.Zip(Clients.Skip(1), (prevClient, nextClient) => prevClient.Service + prevClient.GetDistance(nextClient)).Sum();

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

    public bool StayCorrectWindow(double deltaTime, double deltaCap, LinkedListNode<Client> startClient, LinkedListNode<Client> endCLient)
    {
        var currentClient = startClient;
        var currentDeltaTime = deltaTime;
        while (currentClient != endCLient.Next)
        {
            currentDeltaTime = currentClient.Value.StayInTimeWithDelta(currentDeltaTime);
            //Console.WriteLine(currentDeltaTime);
            if (double.IsNaN(currentDeltaTime))
            {
                return false;
            }
            if (currentDeltaTime == 0)
            {
                break;
            }
            currentClient = currentClient.Next;
        }
        return deltaCap + CurrentCapacity <= MaxCapacity;
    }
    
    public void setClientsTime(LinkedListNode<Client> client)
    {
        var startNode = client;
        while (startNode != null)
        {
            var newTimeOnIt = startNode.Previous.Value.TimeAfterService + startNode.Previous.Value.GetDistance(startNode.Value);
            startNode.Value.TimeOnIt = newTimeOnIt < startNode.Value.ReadyTime ? newTimeOnIt : startNode.Value.ReadyTime;
            startNode = startNode.Next; 
        }
    }
    
    
    public void RemoveClient(LinkedListNode<Client> client)
    {
        var startNode = client.Previous.Previous == null ? client.Next : client.Previous;
        Clients.Remove(client);
        CurrentCapacity -= client.Value.Demand;
        setClientsTime(startNode);
    }
    
    public void AddClientAfter(LinkedListNode<Client>  after, LinkedListNode<Client> client)
    {
        Clients.AddAfter(after, client);
        CurrentCapacity += client.Value.Demand;
        setClientsTime(client);
    }
    
    public void AddClientBefore(LinkedListNode<Client> before, LinkedListNode<Client> client)
    {
        Clients.AddBefore(before, client);
        CurrentCapacity += client.Value.Demand;
        setClientsTime(client);
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