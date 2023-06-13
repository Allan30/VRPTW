using VRPTW.Concret;

namespace VRPTW.Heuristics;

public abstract class IStrategy
{
    protected double bestFitness = double.MaxValue;
    protected Routes bestSolution = new();

    protected abstract bool LoopConditon { get; }
    public void RandomWithSelectedOperators(ref Routes solution, List<OperatorName> operatorsNames)
    {
        var operators = GetOperatorsFromName(operatorsNames).ToList();
        var nbOperators = operators.Count;
        var r = new Random();
        while (LoopConditon)
        {
            solution = GetNewSolution(operators[r.Next(0, nbOperators)].Execute(solution), solution);
            solution.DelEmptyVehicles();
            var newFitness = solution.Fitness;
            if (newFitness < bestFitness)
            {
                bestFitness = newFitness;
                bestSolution = (Routes) solution.Clone();
            }
        }
    }

    public void BestOfSelectedOperators(ref Routes solution, List<OperatorName> operatorsName)
    {
        bestSolution = (Routes) solution.Clone();
        var operators = GetOperatorsFromName(operatorsName);
        while(LoopConditon)
        {
            var neighbors = new List<(Vehicle, Vehicle, double, (OperatorName, List<int>))>();
            foreach (var op in operators)
            {
                neighbors = neighbors.Concat(op.Execute(solution)).ToList();
            }
            solution = GetNewSolution(neighbors, solution);
            solution.DelEmptyVehicles();
            var newFitness = solution.Fitness;
            if(newFitness < bestFitness)
            {
                bestFitness = newFitness;
                bestSolution = (Routes) solution.Clone();
            }
        }
        Console.WriteLine(solution.Fitness);
        solution = bestSolution;
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
                    throw new ArgumentOutOfRangeException(nameof(operatorName));
            }
        }

        return operators;
    }

    protected abstract Routes GetNewSolution(List<(Vehicle src, Vehicle trg, double delta, (OperatorName name, List<int> clientsIndex) operation)> vehicles,
        Routes solution);


}