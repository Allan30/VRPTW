namespace VRPTW.Concret;

public class ExchangeOperatorInter : OperatorInter
{
    protected override bool IndexSrcCondition(int indexSrc, Vehicle vehicle)
    {
        return indexSrc != vehicle.Clients.Count - 1;
    }

    protected override bool IndexTrgCondition(int indexTrg, Vehicle vehicle)
    {
        return indexTrg != vehicle.Clients.Count - 1;
    }

    protected override int GetIndexSrc()
    {
        return 1;
    }

    protected override int GetIndexTrg()
    {
        return 1;
    }

    protected override void PerformOperation(int indexSrc, int indexTrg, Vehicle vehicleSrc, Vehicle vehicleTrg)
    {
        var newVehicleSrc = (Vehicle) vehicleSrc.Clone();
        var newVehicleTrg = (Vehicle) vehicleTrg.Clone();
        newVehicleSrc.RemoveClient(indexSrc);
        newVehicleTrg.RemoveClient(indexTrg);
        newVehicleTrg.AddClientBefore(indexTrg, newVehicleSrc.Clients[indexSrc]);
        newVehicleSrc.AddClientBefore(indexSrc, newVehicleTrg.Clients[indexTrg]);
        if (newVehicleSrc.IsCorrect() && newVehicleTrg.IsCorrect())
        {
            var delta = vehicleSrc.TravelledDistance - newVehicleSrc.TravelledDistance +
                        vehicleTrg.TravelledDistance - newVehicleTrg.TravelledDistance;
            OperateVehicles.Add((newVehicleSrc, newVehicleTrg, delta));
        }
    }
}