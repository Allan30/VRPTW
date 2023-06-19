namespace VRPTW.Core;

public class Vehicle : ICloneable
{
    public int Id { get; set; }
    public int MaxCapacity { get; init; }
    public int CurrentCapacity { get; private set; }
    public List<Client> Clients { get; set; }

    public Client Depot;

    public Vehicle(int id, int capacity, Client depot)
    {
        Id = id;
        CurrentCapacity = 0;
        MaxCapacity = capacity;
        Clients = new List<Client>
        {
            depot,
            new Client(depot.Id + "_bis", depot.Position, depot.ReadyTime, depot.DueTime)
        };
        Depot = depot;
    }

    public bool TryAddClientWithWindow(Client client)
    {
        AddClientBefore(Clients.Count, client);
        if (IsCorrect())
        {
            return true;
        }
        RemoveClient(Clients.Count - 2);
        return false;
    }

    public double TravelledDistance =>
        Clients
            .Zip(Clients.Skip(1), (prevClient, nextClient) => prevClient.GetDistance(nextClient))
            .Sum();

    public double RouteTime => 
        Clients
            .Zip(Clients.Skip(1), (prevClient, nextClient) => prevClient.Service + prevClient.GetDistance(nextClient))
            .Sum();

    public object Clone() => 
        new Vehicle(Id, MaxCapacity, Depot)
        {
            CurrentCapacity = CurrentCapacity,
            Clients = new List<Client>(Clients)
        };

    public bool IsCorrect()
    {
        if (CurrentCapacity > MaxCapacity)
        {
            return false;
        }
        var currentTime = 0.0;
        for (var i = 0; i < Clients.Count - 1; i++)
        {
            var currentClient = Clients[i];
            var nextClient = Clients[i + 1];

            currentTime += currentClient.Service + currentClient.GetDistance(nextClient);
            if (currentTime > nextClient.DueTime)
            {
                return false;
            }
            if (currentTime < nextClient.ReadyTime)
            {
                currentTime = nextClient.ReadyTime;
            }
        }
        return true;
    }


    public void RemoveClient(int index)
    {
        CurrentCapacity -= Clients[index].Demand;
        Clients.RemoveAt(index);
    }

    public void AddClientAfter(int index, Client client)
    {
        Clients.Insert(index + 1, client);
        CurrentCapacity += client.Demand;
    }

    public void AddClientBefore(int index, Client client)
    {
        Clients.Insert(index - 1, client);
        CurrentCapacity += client.Demand;
    }

    public int NbClients => Clients.Count - 2;

    public int TotalDemand => Clients.Sum(client => client.Demand);
}