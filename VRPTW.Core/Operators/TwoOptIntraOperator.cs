namespace VRPTW.Core.Operators;

public sealed class TwoOptIntraOperator : IntraOperator
{
    public override OperatorEnum GetName() => OperatorEnum.TwoOpt;

    protected override bool IndexSrcCondition(int indexSrc, Vehicle vehicle) =>
        indexSrc != vehicle.Clients.Count - 2;

    protected override bool IndexTrgCondition(int indexTrg, Vehicle vehicle) =>
        indexTrg != vehicle.Clients.Count - 1;

    protected override int IndexSrc => 0;

    protected override int IndexTrg => 1;

    protected override void PerformOperation(int indexSrc, int indexTrg, Vehicle vehicle)
    {
        if (indexTrg - indexSrc < 2)
            return;
        var newVehicle = (Vehicle)vehicle.Clone();
        for (int index = indexSrc + 1; index < (indexTrg + indexSrc + 1) / 2 + 1; index++)
        {
            var client1 = newVehicle.Clients[index];
            newVehicle.Clients[index] = newVehicle.Clients[indexTrg + indexSrc + 1 - index];
            newVehicle.Clients[indexTrg + indexSrc + 1 - index] = client1;
        }
        if (newVehicle.IsCorrect())
        {
            var delta = vehicle.TravelledDistance - newVehicle.TravelledDistance;
            OperateVehicles.Add((newVehicle, newVehicle, delta, (GetName(), new List<int> { indexSrc, indexTrg })));
        }
    }
}