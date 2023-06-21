namespace VRPTW.Core.Operators;

public sealed class RelocateIntraOperator : IntraOperator
{
    public override OperatorEnum GetName() => OperatorEnum.RelocateIntra;

    protected override bool IndexSrcCondition(int indexSrc, Vehicle vehicle)
    {
        return indexSrc != vehicle.Clients.Count - 1;
    }

    protected override bool IndexTrgCondition(int indexTrg, Vehicle vehicle)
    {
        return indexTrg != vehicle.Clients.Count - 2;
    }

    protected override int IndexSrc => 1;

    protected override int IndexTrg => 0;

    protected override void PerformOperation(int indexSrc, int indexTrg, Vehicle vehicle)
    {
        if (indexSrc - 1 == indexTrg)
        {
            return;
        }
        var newVehicle = (Vehicle)vehicle.Clone();
        var client = newVehicle.Clients[indexSrc];
        newVehicle.RemoveClient(indexSrc);
        if (indexSrc < indexTrg)
        {
            indexTrg--;
        }
        newVehicle.AddClientAfter(indexTrg, client);
        if (newVehicle.IsCorrect())
        {
            var delta = vehicle.TravelledDistance - newVehicle.TravelledDistance;
            OperateVehicles.Add((newVehicle, newVehicle, delta, (GetName(), new List<int> { indexSrc, indexTrg })));
        }
    }
}