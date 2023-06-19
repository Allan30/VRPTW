using System.Numerics;

namespace VRPTW.Core.Tools;

public static class VrpParser
{
    public static Routes ExtractVrpFile(string vrpFile)
    {
        var lines = File.ReadAllLines(vrpFile);
        List<Client> clients = new();
        
        var maxCapacity = int.Parse(lines[6].Split(' ')[1]);
        var depotLineTokens = lines[9].Split(' ');
        var depot = new Client(depotLineTokens[0], new Vector2(float.Parse(depotLineTokens[1]), float.Parse(depotLineTokens[2])), int.Parse(depotLineTokens[3]), int.Parse(depotLineTokens[4]));

        for (var i = 12; i < lines.Length; i++)
        {
            var tokens = lines[i].Split(' ');
            clients.Add(new Client(tokens[0], new Vector2(float.Parse(tokens[1]), float.Parse(tokens[2])), int.Parse(tokens[3]), int.Parse(tokens[4]), int.Parse(tokens[5]), int.Parse(tokens[6])));
        }

        return new Routes(depot, clients, maxCapacity);
    }
}