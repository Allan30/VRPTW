using System.Reflection;
using VRPTW.Concret;
using VRPTWCore.Parser;

namespace VRPTWCore;

public class Test
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");
        var writer = new PythonParser();
        var solution = VrpParser.ExtractVrpFile("C:\\Users\\epulapp\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.vrp");
        solution.GenerateRandomSolution();
        writer.WritePythonFile("C:\\Users\\epulapp\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.json", solution);
        Console.WriteLine(solution.Fitness);
        var metaH = new TabouResearch(solution);
        metaH.performSolution(1000);
        Console.WriteLine(metaH.bestSolution.Fitness);
        writer.WritePythonFile("C:\\Users\\epulapp\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.2.json", metaH.bestSolution);
        writer.WritePythonFile("C:\\Users\\epulapp\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.3.json", metaH.solution);
    }
    
}