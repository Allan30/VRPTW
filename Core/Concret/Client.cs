using VRPTW.AbstractObjects;

namespace VRPTW.Concret;

public class Client : IElement
{
    public string Id;
    public Coordinate Coordinate;
    public int ReadyTime;
    public int DueTime;
    public int Demand;
    public int Service;

    public Client(string id, Coordinate coordinate, int readyTime, int dueTime, int demand = 0, int service = 0)
    {
        Id = id;
        Coordinate = coordinate;
        ReadyTime = readyTime;
        DueTime = dueTime;
        Demand = demand;
        Service = service;
        
    }

    public double GetDistance(Client otherClient)
    {
        return Coordinate.GetDistance(otherClient.Coordinate);
    }
    
}