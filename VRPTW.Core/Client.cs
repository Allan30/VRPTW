using System.Numerics;

namespace VRPTW.Core
{
    public sealed class Client : ICloneable, IEquatable<Client>
    {
        public string Id { get; set; }
        public Vector2 Position { get; set; }
        public int ReadyTime { get; set; }
        public int DueTime { get; set; }
        public int Demand { get; set; }
        public int Service { get; set; }

        public double TimeOnIt { get; set; }

        public double TimeAfterService => TimeOnIt + Service;

        public Client(string id, Vector2 position, int readyTime, int dueTime, int demand = 0, int service = 0)
        {
            Id = id;
            Position = position;
            ReadyTime = readyTime;
            DueTime = dueTime;
            Demand = demand;
            Service = service;
        }

        public double GetDistance(Client otherClient) => Vector2.Distance(Position, otherClient.Position);

        public object Clone() => new Client(Id, Position, ReadyTime, DueTime, Demand, Service);

        public override int GetHashCode() => Id.GetHashCode();

        public override bool Equals(object? obj) =>
            obj is Client otherClient && Equals(otherClient);

        public bool Equals(Client? other) =>
            other is not null && Id == other.Id;
    }
}