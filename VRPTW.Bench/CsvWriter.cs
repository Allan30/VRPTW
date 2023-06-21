using System.Text;

namespace VRPTW.Bench;

public static class CsvWriter
{
    public static void WriteCsv(string path, List<string> headers, List<List<string>> data)
    {
        var csv = new StringBuilder();

        // write headers
        csv.AppendLine(string.Join(";", headers));

        // write data. Each List<string> in the List<List<string>> represents a column
        int maxRowCount = data.Max(column => column.Count); // Determine the maximum number of rows in any column
        for (int row = 0; row < maxRowCount; row++)
        {
            var rowData = new List<string>();

            foreach (var column in data)
            {
                if (row < column.Count)
                    rowData.Add(column[row]);
                else
                    rowData.Add(""); // Fill empty cells with empty string if the row is shorter in that column
            }

            csv.AppendLine(string.Join(";", rowData));
        }

        File.WriteAllText(path, csv.ToString());
    }
}