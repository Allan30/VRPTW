using VRPTW.Concret;
using VRPTW.Heuristics;
using VRPTWCore.Parser;

namespace VRPTWCore;

public class Test
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");
        var writer = new PythonParser();
        var solution = VrpParser.ExtractVrpFile("C:\\Users\\allan\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.vrp");
        solution.GenerateRandomSolution();
        writer.WritePythonFile("C:\\Users\\allan\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.json", solution);
        Console.WriteLine(solution.Fitness);
        var descent = new DescentStrategy();
        var tabou = new TabouResearchStrategy();
        var recuit = new RecuitSimuleStrategy();
        var operators = new List<OperatorName>();
        operators.Add(OperatorName.ExchangeInter);
        operators.Add(OperatorName.ExchangeIntra);
        operators.Add(OperatorName.RelocateInter);
        operators.Add(OperatorName.RelocateIntra);
        operators.Add(OperatorName.TwoOpt);
        //tabou.BestOfSelectedOperators(ref solution, operators);
        recuit.RandomWithSelectedOperators(ref solution, operators);
        Console.WriteLine(solution.Fitness);
        //writer.WritePythonFile("C:\\Users\\allan\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.2.json", solution);
    }
    
}