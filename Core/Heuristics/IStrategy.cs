using VRPTW.Concret;

namespace VRPTW.Heuristics;

public abstract class IStrategy
{
    public void RandomWithSelectedOperators(ref Routes solution, List<OperatorName> operatorsNames, int nbSteps)
    {
        var operators = GetOperatorsFromName(operatorsNames).ToList();
        var nbOperators = operators.Count();
        var r = new Random();
        for(var i = 0; i < nbSteps; i++)
        {
            solution = GetNewSolution(operators[r.Next(0, nbOperators)].Execute(solution), solution);
            solution.DelEmptyVehicles();
        }
    }

    public void BestOfSelectedOperators(ref Routes solution, List<OperatorName> operatorsName, int nbSteps)
    {
        var operators = GetOperatorsFromName(operatorsName);
        for(var i = 0; i < nbSteps; i++)
        {
            var neighbors = new List<(Vehicle, Vehicle, double, (OperatorName, List<int>))>();
            foreach (var op in operators)
            {
                neighbors = neighbors.Concat(op.Execute(solution)).ToList();
            }
            solution = GetNewSolution(neighbors, solution);
            solution.DelEmptyVehicles();
        }
    }

    protected List<Operator> GetOperatorsFromName(List<OperatorName> operatorsName)
    {
        var operators = new List<Operator>();
        foreach (var operatorName in operatorsName)
        {
            switch (operatorName)
            {
                case OperatorName.ExchangeInter: 
                    operators.Add(new ExchangeOperatorInter());
                    break;
                    
                case OperatorName.ExchangeIntra:
                    operators.Add(new ExchangeOperatorIntra());
                    break;
                    
                case OperatorName.RelocateInter:
                    operators.Add(new RelocateOperatorInter());
                    break;
                    
                case OperatorName.RelocateIntra:
                    operators.Add(new RelocateOperatorIntra());
                    break;
                    
                case OperatorName.TwoOpt:
                    operators.Add(new TwoOptOperatorIntra());
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return operators;
    }

    protected abstract Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, (OperatorName name, List<int> clientsIndex) operation)> vehicles,
        Routes solution);


}