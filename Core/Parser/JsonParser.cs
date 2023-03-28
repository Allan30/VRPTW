using System.Text.Json;
using VRPTW.Concret;

namespace VRPTWCore.Parser;

public class PythonParser
{
    
    public void WritePythonFile(String pythonFile, Routes solution)
    {
        foreach (var route in solution.Vehicles)
        {
            var jsonString = JsonSerializer.Serialize(route.Clients.First.Value.Coordinate);
            File.WriteAllText(jsonString, pythonFile);
        }
    }
}