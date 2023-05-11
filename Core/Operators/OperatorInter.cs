using System.ComponentModel;

namespace VRPTW.Concret;

public abstract class OperatorInter : Operator
{
    protected override void Browse()
    {
        foreach (var vehicleSrc in Solution.Vehicles)
        {
            var nodeSrc = GetNodeSrc(vehicleSrc);
            while (NodeSrcCondition(nodeSrc, vehicleSrc))
            {
                foreach (var vehicleTrg in Solution.Vehicles.Where(x => x != vehicleSrc))
                {
                    var nodeTrg = GetNodeTrg(vehicleTrg);
                    while (NodeTrgCondition(nodeTrg, vehicleTrg))
                    {
                        var deltaValue = CheckDelta(nodeSrc, nodeTrg, vehicleSrc, vehicleTrg);
                        if (!double.IsNaN(deltaValue))
                        {
                            Delta = deltaValue;
                            Vehicles = (vehicleSrc, vehicleTrg);
                            Nodes = (nodeSrc, nodeTrg);
                        }
                        nodeTrg = nodeTrg.Next;
                    }
                }
                nodeSrc = nodeSrc.Next;
            }
        }
    }
}