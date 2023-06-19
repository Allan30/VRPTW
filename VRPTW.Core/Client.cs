using System.Numerics;

namespace VRPTW.Core;

public sealed class Client : ICloneable, IEquatable<Client>
{
    public string Id { get; }
    public Vector2 Position { get; }
    public int ReadyTime { get; }
    public int DueTime { get; }
    public int Demand { get; }
    public int Service { get; }
    public double TimeOnIt { get; }

    public double TimeAfterService => TimeOnIt + Service;

    public Client(string id, Vector2 position, int readyTime, int dueTime, int demand = 0, int service = 0)
    {
        Id = id;
        Position = position;
        ReadyTime = readyTime;
        DueTime = dueTime;
        Demand = demand;
        Service = service;
        PositionHashCode = Position.GetHashCode();
    }

    public float GetDistance(Client otherClient) => Vector2.Distance(Position, otherClient.Position);

    public object Clone() => new Client(Id, Position, ReadyTime, DueTime, Demand, Service);

    public int PositionHashCode { get; }
    public override int GetHashCode() => PositionHashCode;
    public override bool Equals(object? obj) =>
        obj is Client otherClient && Equals(otherClient);

    public bool Equals(Client? other) =>
        other is not null && Id == other.Id;
}