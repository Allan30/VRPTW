namespace VRPTW.Core.Operators;

public abstract class OperatorBase
{
    protected List<(Vehicle src, Vehicle trg, double delta, (OperatorEnum operation, List<int> clientsIndex))> OperateVehicles { get; set; }
    public Routes Solution { get; set; }

    public List<(Vehicle src, Vehicle trg, double delta, (OperatorEnum operation, List<int> clientsIndex))> Execute(Routes solution)
    {
        OperateVehicles = new List<(Vehicle src, Vehicle trg, double delta, (OperatorEnum operation, List<int> clientsIndexs))>();
        Solution = solution;
        Browse();
        return OperateVehicles;
    }
    protected abstract void Browse();

    protected abstract OperatorEnum GetName();

    protected abstract bool IndexSrcCondition(int indexSrc, Vehicle vehicle);
    protected abstract bool IndexTrgCondition(int indexTrg, Vehicle vehicle);
    protected abstract int IndexSrc { get; }
    protected abstract int IndexTrg { get; }
}