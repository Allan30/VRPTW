namespace VRPTW.Concret;

public abstract class OperatorIntra : Operator
{
    protected override void Browse()
    {
        foreach (var vehicle in Solution.Vehicles)
        {
            var nodeSrc = GetNodeSrc(vehicle);
            while (NodeSrcCondition(nodeSrc, vehicle))
            {
                var nodeTrg = GetNodeTrg(vehicle);
                while (NodeTrgCondition(nodeTrg, vehicle))
                {
                    var deltaValue = CheckDelta(nodeSrc, nodeTrg, vehicle, vehicle);
                    if (!double.IsNaN(deltaValue))
                    {
                        Delta = deltaValue;
                        Vehicles = (vehicle, vehicle)!;
                        Nodes = (nodeSrc, nodeTrg)!;
                    }
                    nodeTrg = nodeTrg.Next;
                }
                nodeSrc = nodeSrc.Next;
            }
        }
    }
}