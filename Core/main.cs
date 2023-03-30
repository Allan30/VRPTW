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
        var trans = new Transformations();
        var solution = parser.ExtractVrpFile("C:\\Users\\Epulapp\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.vrp");
        solution.GenerateRandomSolution();
        writer.WritePythonFile("C:\\Users\\Epulapp\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.json", solution);
        //var lists = trans.RelocateIntra(solution.Vehicles[0].Clients);
        var tabouList = new List<Client>();
        for (var i = 0; i < 1000; i++)
        {
            solution.SetBestRelocate(tabouList);
        }
        writer.WritePythonFile("C:\\Users\\Epulapp\\OneDrive\\Bureau\\S8\\OD\\VRPTW\\Core\\Data\\data101.2.json", solution);
    }
    
}