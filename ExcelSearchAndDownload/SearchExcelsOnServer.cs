namespace AspNetSignalIR.ExcelSearchAndDownload;

public class SearchExcelsOnServer
{
    internal static string[] SearchExcel(string name)
    {
        // Define a pasta onde o arquivo Excel está localizado
        string folderPath = Path.Combine("Excel");
        Directory.CreateDirectory(folderPath);
        string[] files;

        if (string.IsNullOrEmpty(name))
        {
            files = Directory.GetFiles(folderPath, "*.xlsx");
        }
        else
        {
            files = Directory.GetFiles(folderPath, $"*{name}*.xlsx");
        }
        Console.WriteLine($"Excel coletado da pasta {folderPath}");
        Console.WriteLine(files.Length);

        return files;
    }


}
