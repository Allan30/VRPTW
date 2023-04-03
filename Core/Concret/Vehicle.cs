namespace VRPTW.Concret;

public class Vehicle
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
        Clients.AddBefore(Clients.Last, client);
        if (GetTravelledDistance() > 230 || client.Demand + CurrentCapacity > MaxCapacity)
        {
            Clients.Remove(Clients.Last);
            Clients.Remove(Clients.Last);
            Clients.AddLast(Clients.First.Value);
            return false;
        }
        CurrentCapacity += client.Demand;
        return true;
    }
    
    public double GetTravelledDistance()
    {
        return Clients.Zip(Clients.Skip(1), (prevClient, nextClient) => prevClient.Coordinate.GetDistance(nextClient.Coordinate)).Sum();
    }
    
}