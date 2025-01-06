using System.Data;
using System.Text;

namespace AspNetSignalIR.ExcelSearchAndDownload;

public class SaveDataSetsAsCSV
{
    // Criador de link de download do arquivo CSV criado através do DataSet
    internal static string SaveDataSetAsCsv(DataSet dataSet)
    {
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, $"DataSet_{DateTime.Now:yyyyMMddHHmmss}.csv");

        using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            foreach (DataTable table in dataSet.Tables)
            {
                writer.WriteLine($"Tabela: {table.TableName}");

                // Escreve o cabeçalho
                var columnNames = string.Join(",", table.Columns.Cast<DataColumn>().Select(column => column.ColumnName));
                writer.WriteLine(columnNames);

                // Escreve as linhas
                foreach (DataRow row in table.Rows)
                {
                    var fields = string.Join(",", row.ItemArray);
                    writer.WriteLine(fields);
                }
                writer.WriteLine();
            }
        }

        return filePath;
    }

}
