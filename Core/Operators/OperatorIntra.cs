namespace VRPTW.Concret;

public abstract class OperatorIntra : Operator
{
    protected override void Browse()
    {
        foreach (var vehicle in Solution.Vehicles)
        {
            var indexSrc = GetIndexSrc();
            while (IndexSrcCondition(indexSrc, vehicle))
            {
                var indexTrg = GetIndexTrg();
                if (indexSrc == indexTrg) continue;
                while (IndexTrgCondition(indexSrc, vehicle))
                {
                    PerformOperation(indexSrc, indexTrg, vehicle);
                    indexTrg++;
                }
                indexSrc++;
            }
        }
    }
    
    protected abstract void PerformOperation(int indexSrc, int indexTrg, Vehicle vehicle);
}