namespace VRPTW.Core
{
    public class Client : ICloneable
    {
        public string Id;
        public Coordinate Coordinate;
        public int ReadyTime;
        public int DueTime;
        public int Demand;
        public int Service;

        public double TimeOnIt { get; set; }

        public double TimeAfterService => TimeOnIt + Service;

        public Client(string id, Coordinate coordinate, int readyTime, int dueTime, int demand = 0, int service = 0)
        {
            Id = id;
            Coordinate = coordinate;
            ReadyTime = readyTime;
            DueTime = dueTime;
            Demand = demand;
            Service = service;
        }

        public double GetDistance(Client otherClient) =>
            Coordinate.GetDistance(otherClient.Coordinate);

        public override bool Equals(object? obj)
        {
            if (obj is Client client)
            {
                return Id == client.Id;
            }

            return false;
        }

        public object Clone() => new Client(Id, Coordinate, ReadyTime, DueTime, Demand, Service);

        public override int GetHashCode() => Id.GetHashCode();
    }
}