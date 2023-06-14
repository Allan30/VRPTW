namespace VRPTW.Core.Operators;

public class ExchangeInterOperator : InterOperator
{
    protected override OperatorEnum GetName()
    {
        return OperatorEnum.ExchangeInter;
    }

    protected override bool IndexSrcCondition(int indexSrc, Vehicle vehicle) =>
        indexSrc != vehicle.Clients.Count - 1;

    protected override bool IndexTrgCondition(int indexTrg, Vehicle vehicle) =>
        indexTrg != vehicle.Clients.Count - 1;

    protected override int IndexSrc => 1;

    protected override int IndexTrg => 1;

    protected override void PerformOperation(int indexSrc, int indexTrg, Vehicle vehicleSrc, Vehicle vehicleTrg)
    {
        var newVehicleSrc = (Vehicle)vehicleSrc.Clone();
        var newVehicleTrg = (Vehicle)vehicleTrg.Clone();
        newVehicleTrg.AddClientAfter(indexTrg, newVehicleSrc.Clients[indexSrc]);
        newVehicleSrc.AddClientAfter(indexSrc, newVehicleTrg.Clients[indexTrg]);
        newVehicleSrc.RemoveClient(indexSrc);
        newVehicleTrg.RemoveClient(indexTrg);
        if (newVehicleSrc.IsCorrect() && newVehicleTrg.IsCorrect())
        {
            var delta = vehicleSrc.TravelledDistance - newVehicleSrc.TravelledDistance +
                        vehicleTrg.TravelledDistance - newVehicleTrg.TravelledDistance;
            OperateVehicles.Add((newVehicleSrc, newVehicleTrg, delta, (GetName(), new List<int> { indexSrc, indexTrg })));
        }
    }
}