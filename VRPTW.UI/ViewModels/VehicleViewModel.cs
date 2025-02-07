﻿using System.Collections.ObjectModel;

namespace VRPTW.UI.ViewModels;

public sealed class VehicleViewModel
{
    public int Id { get; set; }
    public ObservableCollection<ClientViewModel> Clients { get; set; }
    public int NbClients { get; set; }
    public int MaxCapacity { get; set; }
    public int TotalDemand { get; set; }
    public double TravelledDistance { get; set; }
    public int ARGBColor { get; set; }
    public override string ToString() => Id.ToString();
}
