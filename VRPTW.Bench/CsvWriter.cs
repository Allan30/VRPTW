using System.Text;

namespace VRPTW.Bench;

public static class CsvWriter
{
    public static void WriteCsv(string path, List<string> headers, List<List<string>> data)
    {
        var csv = new StringBuilder();

        // write headers
        csv.AppendLine(string.Join(";", headers));

        // write data
        for (var i = 0; i < data[0].Count; i++)
        {
            var line = "";
            foreach (var col in data)
            {
                try
                {
                    line += col[i] + ";";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    line += "";
                }
                
                csv.AppendLine(line);
            }
        }

        File.WriteAllText(path, csv.ToString());
    }
}