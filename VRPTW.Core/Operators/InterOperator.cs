using VRPTW.Core.Concret;

namespace VRPTW.Core.Operators;

public abstract class InterOperator : OperatorBase
{
    protected override void Browse()
    {
        foreach (var vehicleSrc in Solution.Vehicles)
        {
            var indexSrc = IndexSrc;
            while (IndexSrcCondition(indexSrc, vehicleSrc))
            {
                foreach (var vehicleTrg in Solution.Vehicles.Where(x => x != vehicleSrc))
                {
                    var indexTrg = IndexTrg;
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