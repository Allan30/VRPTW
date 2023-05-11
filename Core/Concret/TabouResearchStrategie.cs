using VRPTW.AbstractObjects;

namespace VRPTW.Concret;

public class TabouResearchStrategy : IStrategy
{
    public void Execute(ref Routes solution)
    {
        using var progress = new ProgressBar();
        var relocateInter = new RelocateOperatorInter();
        var nbSteps = 1000;
        for (var i = 0; i <= nbSteps; i++)
        {
            progress.Report((double)i / nbSteps);
            var r = new Random();
            switch (r.Next(0, 0))
            {
                case 0:
                    relocateInter.Execute(solution);
                    break;
            }
        }
    }
}