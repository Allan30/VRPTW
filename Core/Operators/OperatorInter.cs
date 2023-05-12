using System.ComponentModel;

namespace VRPTW.Concret;

public abstract class OperatorInter : Operator
{
    protected override void Browse()
    {
        foreach (var vehicleSrc in Solution.Vehicles)
        {
            var indexSrc = GetIndexSrc();
            while (IndexSrcCondition(indexSrc, vehicleSrc))
            {
                foreach (var vehicleTrg in Solution.Vehicles.Where(x => x != vehicleSrc))
                {
                    var indexTrg = GetIndexTrg();
                    while (IndexTrgCondition(indexTrg, vehicleTrg))
                    {
                        PerformOperation(indexSrc, indexTrg, vehicleSrc, vehicleTrg);
                        indexTrg++;
                    }
                }
                indexSrc++;
            }
        }
    }
    
    protected abstract void PerformOperation(int indexSrc, int indexTrg, Vehicle vehicleSrc, Vehicle vehicleTrg);
}