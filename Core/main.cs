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
        var tabou = new TabuStrategy();
        var recuit = new SimulatedAnnealingStrategy();
        var operators = new List<OperatorEnum>
        {
            OperatorEnum.ExchangeInter,
            OperatorEnum.ExchangeIntra,
            OperatorEnum.RelocateInter,
            OperatorEnum.RelocateIntra,
            OperatorEnum.TwoOpt
        };
        //tabou.BestOfSelectedOperators(ref solution, operators);
        //recuit.RandomWithSelectedOperators(ref solution, operators);
        Console.WriteLine(solution.Fitness);
        //writer.WritePythonFile("C:\\Users\\allan\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.2.json", solution);
    }
    
}