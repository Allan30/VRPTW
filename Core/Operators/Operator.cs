namespace VRPTW.Concret;

public abstract class Operator
{
    protected List<(Vehicle src, Vehicle trg, double delta, string operation)> OperateVehicles;
    public Routes Solution;
    
    public List<(Vehicle src, Vehicle trg, double delta, string operation)> Execute(Routes solution)
    {
        OperateVehicles = new List<(Vehicle src, Vehicle trg, double delta, string operation)>();
        Solution = solution;
        Browse();
        return OperateVehicles;
    }
    protected abstract void Browse();
    
    protected abstract OperatorName GetName();
    
    protected abstract bool IndexSrcCondition(int indexSrc, Vehicle vehicle);
    protected abstract bool IndexTrgCondition(int indexTrg, Vehicle vehicle);
    protected abstract int GetIndexSrc();
    protected abstract int GetIndexTrg();
}