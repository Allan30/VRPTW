using VRPTW.Core.Concret;

namespace VRPTW.Core.Operators;

public abstract class IntraOperator : OperatorBase
{
    protected override void Browse()
    {
        foreach (var vehicle in Solution.Vehicles)
        {
            var indexSrc = IndexSrc;
            while (IndexSrcCondition(indexSrc, vehicle))
            {
                var indexTrg = IndexTrg;
                if (indexSrc != indexTrg)
                {
                    while (IndexTrgCondition(indexTrg, vehicle))
                    {
                        if (indexSrc != indexTrg) PerformOperation(indexSrc, indexTrg, vehicle);
                        indexTrg++;
                    }
                }
                indexSrc++;
            }
        }
    }

    protected abstract void PerformOperation(int indexSrc, int indexTrg, Vehicle vehicle);
}