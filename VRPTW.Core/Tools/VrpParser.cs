using VRPTW.Core.Concret;

namespace VRPTW.Core.Tools;

public static class VrpParser
{
    public static Routes ExtractVrpFile(string vrpFile)
    {
        var solution = new Routes();
        var lines = File.ReadAllLines(vrpFile);
        var index = 0;

        foreach (var line in lines)
        {
            var data = line.Split(' ');
            switch (index)
            {
                case 6:
                    solution.MaxCapacity = int.Parse(data[1]);
                    break;
                case 9:
                    solution.Depot = new Client(data[0], new Coordinate(int.Parse(data[1]), int.Parse(data[2])), int.Parse(data[3]), int.Parse(data[4]));
                    break;
                case > 11:
                    solution.Clients.Add(new Client(data[0], new Coordinate(int.Parse(data[1]), int.Parse(data[2])), int.Parse(data[3]), int.Parse(data[4]), int.Parse(data[5]), int.Parse(data[6])));
                    break;
            }

            index++;
        }

        return solution;
    }
}