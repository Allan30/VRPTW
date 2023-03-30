namespace VRPTW.Concret;

public class Vehicle
{
    public int Id;
    public int Capacity;
    public int time;
    public LinkedList<Client> Clients = new();
    
    public Vehicle(int id, int capacity, Client depot)
    {
        Id = id;
        Capacity = capacity;
        Clients.AddFirst(depot);
        Clients.AddLast(depot);
        time = GetTravelledDistance();
    }
    
    public bool AddClient(Client client)
    {
        /*
        var deltaTime = 0;
        if (Clients.Count == 2)
        {
            deltaTime = Clients.First.Value.Coordinate.GetDistance(client.Coordinate) +
                        Clients.Last.Value.Coordinate.GetDistance(client.Coordinate);
        }
        else
        {
            deltaTime =  Clients.Last.Previous.Previous.Value.Coordinate.GetDistance(client.Coordinate) + Clients.Last.Previous.Previous.Value.Coordinate.GetDistance(client.Coordinate) - Clients.Last.Previous.Value.Coordinate.GetDistance(Clients.Last.Value.Coordinate);
            if (client.Demand > Capacity || time + deltaTime > 230)
            {
                return false;
            }
        }
        */
        
        Clients.AddBefore(Clients.Last, client);
        if (GetTravelledDistance() > 230)
        {
            Clients.Remove(Clients.Last);
            Clients.Remove(Clients.Last);
            Clients.AddLast(Clients.First.Value);
            return false;
        }
        Capacity -= client.Demand;
        //time += deltaTime;
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