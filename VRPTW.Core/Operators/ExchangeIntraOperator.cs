namespace VRPTW.Core.Operators;

public sealed class ExchangeIntraOperator : IntraOperator
{
    public override OperatorEnum GetName()
    {
        return OperatorEnum.ExchangeIntra;
    }

    protected override bool IndexSrcCondition(int indexSrc, Vehicle vehicle)
    {
        return indexSrc != vehicle.Clients.Count - 1;
    }

    protected override bool IndexTrgCondition(int indexTrg, Vehicle vehicle)
    {
        return indexTrg != vehicle.Clients.Count - 1;
    }

    protected override int IndexSrc => 1;

    protected override int IndexTrg => 1;

    protected override void PerformOperation(int indexSrc, int indexTrg, Vehicle vehicle)
    {
        var newVehicle = (Vehicle)vehicle.Clone();
        var client1 = newVehicle.Clients[indexSrc];
        var client2 = newVehicle.Clients[indexTrg];
        newVehicle.Clients[indexTrg] = client1;
        newVehicle.Clients[indexSrc] = client2;
        if (newVehicle.IsCorrect())
        {
            var delta = vehicle.TravelledDistance - newVehicle.TravelledDistance;
            OperateVehicles.Add((newVehicle, newVehicle, delta, (GetName(), new List<int> { indexSrc, indexTrg })));
        }
    }
}