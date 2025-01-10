using System.Text;
using System.Net.WebSockets;

namespace AspNetSignalIR.Services.Excel.SearchAnDownload;

public class ShowAllExcelInServer
{
    public static async Task ShowAllExcel(WebSocket webSocket)
    {
        string[] files = SearchExcelsOnServer.SearchExcel(string.Empty);
        List<string> showFiles = new();
        foreach (var fileForeach in files)
        {
            showFiles.Add(fileForeach.Replace(@"Excel\", ""));
        }
        string responseMessage = $"Excel: Excel existente no servidor: '{string.Join("'; '", showFiles.ToList())}'";
        await webSocket.SendAsync(Encoding.UTF8.GetBytes(responseMessage), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}
