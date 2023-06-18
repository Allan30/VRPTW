using System;

namespace VRPTW.UI.ViewModels;

public sealed class ClientViewModel : IEquatable<ClientViewModel>
{
    public bool IsDepot { get; set; }
    public string Id { get; set; }
    public CoordinateViewModel Coordinate { get; set; }
    public int ReadyTime { get; set; }
    public int DueTime { get; set; }
    public int Demand { get; set; }
    public int Service { get; set; }

    public override string ToString() => IsDepot switch
    {
        true => $"{Id} (Dépôt)",
        _ => Id
    };

    public override bool Equals(object? obj) => obj is ClientViewModel client && Equals(client);

    public bool Equals(ClientViewModel? other) => other is not null && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
