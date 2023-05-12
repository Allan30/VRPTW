namespace VRPTW.Concret;

public class ExchangeOperatorIntra : OperatorIntra
{
    protected override OperatorName GetName()
    {
        return OperatorName.ExchangeIntra;
    }

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

    protected override void PerformOperation(int indexSrc, int indexTrg, Vehicle vehicle)
    {
        var newVehicle = (Vehicle) vehicle.Clone();
        var client1 = newVehicle.Clients[indexSrc];
        var client2 = newVehicle.Clients[indexTrg];
        newVehicle.Clients[indexTrg] = client1;
        newVehicle.Clients[indexSrc] = client2;
        if (newVehicle.IsCorrect())
        {
            var delta = vehicle.TravelledDistance - newVehicle.TravelledDistance;
            OperateVehicles.Add((newVehicle, newVehicle, delta, GetName()+"_"+indexSrc+"_"+indexTrg));
        }
    }
}