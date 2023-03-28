namespace VRPTW.Concret;

public class Vehicle
{
    public int Id;
    public int Capacity;
    public readonly LinkedList<Client> Clients = new();
    
    public Vehicle(int id, int capacity, Client depot)
    {
        Id = id;
        Capacity = capacity;
        Clients.AddFirst(depot);
        Clients.AddLast(depot);
    }
    
    public bool AddClient(Client client)
    {
        if (client.Demand > Capacity)
        {
            return false;
        }
        Clients.AddBefore(Clients.Last, client);
        Capacity -= client.Demand;
        return true;
    }
    
    public int GetTravelledDistance()
    {
        /*
        var sum = 0;
        var currentPoint = Clients.First.Value.Coordinate;
        foreach (var client in Clients)
        {
            sum += client.Coordinate.GetDistance(currentPoint);
            currentPoint = client.Coordinate;
        }
        return sum;
        */
        return Clients.Zip(Clients.Skip(1), (prevClient, nextClient) => prevClient.Coordinate.GetDistance(nextClient.Coordinate)).Sum();

    }
    
}