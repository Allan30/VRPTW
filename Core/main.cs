using System.Reflection;
using VRPTW.Concret;
using VRPTWCore.Parser;

namespace VRPTWCore;

public class Test
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");
        var parser = new VrpParser();
        var writer = new PythonParser();
        var solution = parser.ExtractVrpFile("C:\\Users\\allan\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.vrp");
        solution.GenerateRandomSolution();
        writer.WritePythonFile("C:\\Users\\allan\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.json", solution);
        Console.WriteLine(solution.GetFitness());
        var metaH = new TabouResearch(solution);
        metaH.performSolution(100);
        writer.WritePythonFile("C:\\Users\\allan\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.2.json", solution);
        Console.WriteLine(solution.GetFitness());
    }
    
}