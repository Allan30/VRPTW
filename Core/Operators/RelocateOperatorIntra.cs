namespace VRPTW.Concret;

public class RelocateOperatorIntra : OperatorIntra
{
    protected override void SetNewSolution()
    {
        if (Nodes.src is null) return;
        Vehicles.src.RemoveClient(Nodes.src);
        Vehicles.trg.AddClientAfter(Nodes.trg, Nodes.src);
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
        return vehicle.Clients.First;
    }

    protected override double CheckDelta(LinkedListNode<Client> nodeSrc, LinkedListNode<Client> nodeTrg, Vehicle vehicleSrc, Vehicle vehicleTrg)
    {
        var currentDistance = nodeSrc.Value.GetDistance(nodeSrc.Previous.Value) +
                              nodeSrc.Value.GetDistance(nodeSrc.Next.Value)
                              + nodeTrg.Value.GetDistance(nodeTrg.Next.Value);
                        
        var newTimeSrc = nodeSrc.Previous.Value.GetDistance(nodeSrc.Next.Value)
                         - nodeSrc.Value.GetDistance(nodeTrg.Value)
                         - nodeSrc.Value.GetDistance(nodeTrg.Next.Value)
                         - nodeSrc.Value.Service;
                        
        var newTimeTrg = nodeSrc.Value.GetDistance(nodeTrg.Value)
                         + nodeSrc.Value.GetDistance(nodeTrg.Next.Value)
                         + nodeSrc.Value.Service
                         - nodeTrg.Value.GetDistance(nodeTrg.Next.Value);

        var newDistance = nodeSrc.Previous.Value.GetDistance(nodeSrc.Next.Value) +
                          nodeSrc.Value.GetDistance(nodeTrg.Value)
                          + nodeSrc.Value.GetDistance(nodeTrg.Next.Value);
        var delta = newDistance - currentDistance;

        if (delta > Delta || !vehicleSrc.StayCorrectWindow(newTimeSrc, 0, nodeSrc.Next, nodeTrg))
        {
            return double.NaN;
        }

        return delta;
    }
}