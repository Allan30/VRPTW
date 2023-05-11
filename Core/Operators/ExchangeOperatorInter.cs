namespace VRPTW.Concret;

public class ExchangeOperatorInter : OperatorInter
{
    protected override void SetNewSolution()
    {
        if (Nodes.src == null || Nodes.trg == null) return;
        var nodeToExchange2Next = Nodes.trg.Next.Value;
        
        Vehicles.trg.RemoveClient(Nodes.trg);
        Vehicles.src.AddClientBefore(Nodes.src, Nodes.trg);
        Vehicles.src.RemoveClient(Nodes.src);
        Vehicles.trg.AddClientBefore(Vehicles.trg.Clients.FindLast(nodeToExchange2Next), Nodes.src);
    }

    protected override bool NodeSrcCondition(LinkedListNode<Client> nodeSrc, Vehicle vehicle)
    {
        return !nodeSrc.Value.Equals(vehicle.Clients.Last.Value);
    }

    protected override bool NodeTrgCondition(LinkedListNode<Client> nodeTrg, Vehicle vehicle)
    {
        return !nodeTrg.Value.Equals(vehicle.Clients.Last.Value);
    }

    protected override LinkedListNode<Client> GetNodeSrc(Vehicle vehicle)
    {
        return vehicle.Clients.First.Next;
    }

    protected override LinkedListNode<Client> GetNodeTrg(Vehicle vehicle)
    {
        return vehicle.Clients.First.Next;
    }

    protected override double CheckDelta(LinkedListNode<Client> nodeSrc, LinkedListNode<Client> nodeTrg, Vehicle vehicleSrc, Vehicle vehicleTrg)
    {
        var currentDistanceVehicle1 = nodeSrc.Value.GetDistance(nodeSrc.Next.Value) + nodeSrc.Value.GetDistance(nodeSrc.Previous.Value);
        var newDistanceVehicle1 = nodeSrc.Value.GetDistance(nodeTrg.Next.Value) + nodeSrc.Value.GetDistance(nodeTrg.Previous.Value);
        var currentDistanceVehicle2 = nodeTrg.Value.GetDistance(nodeTrg.Next.Value) + nodeTrg.Value.GetDistance(nodeTrg.Previous.Value);
        var newDistanceVehicle2 = nodeTrg.Value.GetDistance(nodeSrc.Next.Value) + nodeTrg.Value.GetDistance(nodeSrc.Previous.Value);
        var deltaVehicle1 = newDistanceVehicle1 - currentDistanceVehicle1;
        var deltaVehicle2 = newDistanceVehicle2 - currentDistanceVehicle2;
        var newTimeSrc = nodeTrg.Value.Service + newDistanceVehicle1 - currentDistanceVehicle1 - nodeSrc.Value.Service;
        var newTimeTrg = nodeSrc.Value.Service + newDistanceVehicle2 - currentDistanceVehicle2 - nodeTrg.Value.Service;
        var delta = deltaVehicle1 + deltaVehicle2;
        if (delta < Delta &&
            vehicleSrc.StayCorrectWindow(newTimeSrc, nodeTrg.Value.Demand - nodeSrc.Value.Demand,
                nodeSrc.Next, vehicleSrc.Clients.Last) && vehicleTrg.StayCorrectWindow(newTimeTrg,
                nodeSrc.Value.Demand - nodeTrg.Value.Demand, nodeTrg.Next,
                vehicleTrg.Clients.Last))
        {
            return delta;
        }
        return double.NaN;
    }
}