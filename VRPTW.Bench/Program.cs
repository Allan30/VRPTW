using System.Globalization;
using VRPTW.Core;
using VRPTW.Core.Heuristics;
using VRPTW.Core.Operators;
using VRPTW.Core.Tools;

namespace VRPTW.Bench;

public class Program
{
    private static void Main(string[] args)
    {
        var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        var files = Directory.GetFiles(dataPath, "*.vrp");

        var strategiesPreparator = new StrategiesPreparator
        {
            NumbersOfSteps = new() { 1_000 },
            UseRandomNeighborhoodStrategy = false,
            OperatorsChoices = new() { StrategiesPreparator.AllOperators() },
            AlphaTempCombinations = ((1_000, StrategiesPreparator.DefaultAlphas()), (0.99, StrategiesPreparator.DefaultTemperatures()))
        };

        foreach (var file in files)
        {
            var outputPath = Path.Combine("out", Path.GetFileName(file));

            if (Directory.Exists(outputPath))
            {
                Directory.Delete(outputPath, true);
            }

            Directory.CreateDirectory(outputPath);
            var routes = VrpParser.ExtractVrpFile(file);
            var strategies = strategiesPreparator.Prepare();

            Parallel.ForEach(strategies, new ParallelOptions { MaxDegreeOfParallelism = -1 }, (strategy, state, index) =>
            {
                strategy.Strategy.Calculate((Routes)routes.Clone(), strategy.Operators, new Progress<int>(), CancellationToken.None);                
                
                CsvWriter.WriteCsv(
                    $"{outputPath}/{index:D3}_{ConstructFileNameFromStrategy(strategy)}.csv",
                    new List<string> { "fitness", "best_fitnesses", "operators" },
                    new List<List<string>>
                    {
                        strategy.Strategy.Fitnesses.Select(f => f.ToString(CultureInfo.InvariantCulture)).ToList(),
                        strategy.Strategy.BestFitnesses.Select(bf => bf.ToString(CultureInfo.InvariantCulture)).ToList(),
                        strategy.Strategy.Operators.Select(op => op.GetName().ToString()).ToList(),
                    }
                );
            });
        }
    }

    private static string ConstructFileNameFromStrategy((List<OperatorEnum> Operators, HeuristicStrategyBase Strategy) strategy)
    {
        var strategyName = strategy.Strategy.GetType().Name.ToLower();
        var neighbStratName = strategy.Strategy.NeighborhoodStrategy.GetType().Name.ToLower();
        var operatorsName = strategy.Operators.SequenceEqual(StrategiesPreparator.AllOperators())
            ? "allops"
            : string.Join("_", strategy.Operators.Select(o => o.ToString().ToLower()));
        return string.Join('_', strategyName, neighbStratName, operatorsName);
    }
}