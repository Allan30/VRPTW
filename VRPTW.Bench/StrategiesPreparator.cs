using VRPTW.Core.Heuristics;
using VRPTW.Core.Neighborhood;
using VRPTW.Core.Operators;

namespace VRPTW.Bench;

public sealed class StrategiesPreparator
{
    public static List<double> DefaultAlphas() => new() { 0.1, 0.5, 0.8, 0.9, 0.95, 0.96, 0.97, 0.98, 0.99, 0.999 };
    public static List<int> DefaultTemperatures() => new() { 100, 1_000, 10_000, 100_000 };
    public static List<OperatorEnum> AllOperators() => new() { OperatorEnum.ExchangeInter, OperatorEnum.ExchangeIntra, OperatorEnum.RelocateInter, OperatorEnum.RelocateIntra, OperatorEnum.TwoOpt };

    public List<int> Temperatures = DefaultTemperatures();
    public List<double> Alphas { get; set; } = DefaultAlphas();
    public List<int> TabuSizes = new() { 10, 40, 80, 120 };
    public List<int> NumbersOfSteps = new() { 1_000, 10_000 };
    public List<List<OperatorEnum>> OperatorsChoices { get; set; } = new()
        {
            new List<OperatorEnum> { OperatorEnum.ExchangeInter, OperatorEnum.ExchangeIntra },
            new List<OperatorEnum> { OperatorEnum.RelocateInter, OperatorEnum.RelocateIntra },
            AllOperators(),
        };
    public bool UseRandomNeighborhoodStrategy { get; set; } = true;
    public ((int Temp, List<double> Alphas) FixedTemp, (double Alpha, List<int> Temps) FixedAlpha)? AlphaTempCombinations = null;

    public List<(List<OperatorEnum> Operators, HeuristicStrategyBase Strategy)> Prepare()
    {
        var strategies = new List<(List<OperatorEnum>, HeuristicStrategyBase)>();

        foreach (var opsList in OperatorsChoices)
        {
            strategies.Add((opsList, new DescentStrategy(new BestWithSelectedOperatorsStrategy())));
            if (UseRandomNeighborhoodStrategy)
                strategies.Add((opsList, new DescentStrategy(new RandomWithSelectedOperatorsStrategy())));

            foreach (var sNumber in NumbersOfSteps)
            {
                strategies.AddRange(AddTabuStrategies(opsList, sNumber));
                strategies.AddRange(AddSimulatedAnnealingStrategies(opsList, sNumber));
            }
        }

        return strategies;
    }

    private List<(List<OperatorEnum>, HeuristicStrategyBase)> AddTabuStrategies(List<OperatorEnum> operatorsList, int numberOfSteps)
    {
        var strategies = new List<(List<OperatorEnum>, HeuristicStrategyBase)>();
        
        foreach (var tSize in TabuSizes)
        {
            strategies.Add((operatorsList, new TabuStrategy(new BestWithSelectedOperatorsStrategy()) { TabuSize = tSize, NbSteps = numberOfSteps }));
            if (UseRandomNeighborhoodStrategy)
                strategies.Add((operatorsList, new TabuStrategy(new RandomWithSelectedOperatorsStrategy()) { TabuSize = tSize, NbSteps = numberOfSteps }));
        }

        return strategies;
    }

    private List<(List<OperatorEnum>, HeuristicStrategyBase)> AddSimulatedAnnealingStrategies(List<OperatorEnum> opsList, int sNumber)
    {
        var strategies = new List<(List<OperatorEnum>, HeuristicStrategyBase)>();

        if (AlphaTempCombinations is null)
        {
            foreach (var temp in Temperatures)
            {
                foreach (var alpha in Alphas)
                {
                    strategies.Add((opsList, new SimulatedAnnealingStrategy(new BestWithSelectedOperatorsStrategy()) { Alpha = alpha, Temperature = temp, NbSteps = sNumber }));
                    if (UseRandomNeighborhoodStrategy)
                        strategies.Add((opsList, new SimulatedAnnealingStrategy(new RandomWithSelectedOperatorsStrategy()) { Alpha = alpha, Temperature = temp, NbSteps = sNumber }));
                }
            }
        }
        else
        {
            foreach (var alpha in AlphaTempCombinations.Value.FixedTemp.Alphas)
            {
                strategies.Add((opsList, new SimulatedAnnealingStrategy(new BestWithSelectedOperatorsStrategy()) { Alpha = alpha, Temperature = AlphaTempCombinations.Value.FixedTemp.Temp, NbSteps = sNumber }));
                if (UseRandomNeighborhoodStrategy)
                    strategies.Add((opsList, new SimulatedAnnealingStrategy(new RandomWithSelectedOperatorsStrategy()) { Alpha = alpha, Temperature = AlphaTempCombinations.Value.FixedTemp.Temp, NbSteps = sNumber }));
            }
            foreach (var temp in AlphaTempCombinations.Value.FixedAlpha.Temps)
            {
                strategies.Add((opsList, new SimulatedAnnealingStrategy(new BestWithSelectedOperatorsStrategy()) { Alpha = AlphaTempCombinations.Value.FixedAlpha.Alpha, Temperature = temp, NbSteps = sNumber }));
                if (UseRandomNeighborhoodStrategy)
                    strategies.Add((opsList, new SimulatedAnnealingStrategy(new RandomWithSelectedOperatorsStrategy()) { Alpha = AlphaTempCombinations.Value.FixedAlpha.Alpha, Temperature = temp, NbSteps = sNumber }));
            }
        }

        return strategies;
    }
}
