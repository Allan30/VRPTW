namespace VRPTW.Concret;

public class RelocateOperatorIntra : OperatorIntra
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
        return 0;
    }

    protected override void PerformOperation(int indexSrc, int indexTrg, Vehicle vehicle)
    {
        var newVehicle = (Vehicle) vehicle.Clone();
        newVehicle.AddClientAfter(indexTrg, newVehicle.Clients[indexSrc]);
        newVehicle.RemoveClient(indexSrc);
        if (newVehicle.IsCorrect())
        {
            var delta = vehicle.TravelledDistance - newVehicle.TravelledDistance;
            OperateVehicles.Add((newVehicle, newVehicle, delta));
        }
    }
}