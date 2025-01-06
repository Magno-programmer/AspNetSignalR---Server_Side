using ClosedXML.Excel;
using System.Net.WebSockets;
using System.Text;

namespace AspNetSignalIR.ExcelSearchAndDownload;

public class SendInformationOfExcelToClients
{
    internal static async Task SendInformationOfExcelToClient(WebSocket webSocket, string name)
    {
        try
        {
            string arquivoExcel;
            int spaceIndex = name.IndexOf(' ');
            name = spaceIndex > 0 ? name[(spaceIndex + 1)..] : string.Empty;

            string[] files = SearchExcelsOnServer.SearchExcel(name);

            // Encontra o primeiro arquivo .xlsx na pasta
            if (files.Length == 0)
            {
                // Se nenhum arquivo for encontrado, envia uma mensagem de erro para o cliente
                string errorMessage = "Excel: Nenhum arquivo Excel encontrado na pasta especificada.";
                await webSocket.SendAsync(Encoding.UTF8.GetBytes(errorMessage), WebSocketMessageType.Text, true, CancellationToken.None);
                return;
            }

            // Usa o primeiro arquivo encontrado
            string filePath = files[0];
            int barraIndex = filePath.IndexOf(@"\");

            arquivoExcel = barraIndex > 0 ? filePath[(barraIndex + 1)..] : filePath;

            // Coletar a quantidade de linhas e colunas no Excel
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.First();
            int rows = worksheet.RangeUsed()!.RowCount();
            int columns = worksheet.RangeUsed()!.ColumnCount();

            // Envia o link de download para o cliente

            string informacaoDoTamanhoDoExcel = $"Excel: Servidor: Esse excel tem {rows} linhas e {columns} colunas";
            byte[] responseMessage = Encoding.UTF8.GetBytes(informacaoDoTamanhoDoExcel);
            await webSocket.SendAsync(new ArraySegment<byte>(responseMessage), WebSocketMessageType.Text, true, CancellationToken.None);

        }
        catch (Exception ex)
        {
            string errorMessage = $"Excel: Erro ao ler o arquivo Excel: {ex.Message}";
            await webSocket.SendAsync(Encoding.UTF8.GetBytes(errorMessage), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        
    }
}
