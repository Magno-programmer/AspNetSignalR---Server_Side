using AspNetSignalIR.ClientListen;
using AspNetSignalIR.Connectors;
using AspNetSignalIR.ImageConfigurations;
using ClosedXML.Excel;
using System.Data;
using System.Net.WebSockets;
using System.Text;

namespace AspNetSignalIR.ExcelSearchAndDownload;

public class SendExcelToClients
{

    internal static async Task SendExcelDataToClient(WebSocket webSocket, string name)
    {
        try
        {
            Console.WriteLine("Buscando o Excel...");
            string responseMessage;
            string arquivoExcel;
            int spaceIndex = name.IndexOf(' ');
            name = spaceIndex > 0 ? name[(spaceIndex + 1)..] : string.Empty;
            Console.WriteLine(name);

            string[] files = SearchExcelsOnServer.SearchExcel(name);

            // Encontra o primeiro arquivo .xlsx na pasta
            if (files.Length == 0 )
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
            Console.WriteLine(arquivoExcel);

            // Leia o arquivo e converta-o para Base64
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string base64File = Convert.ToBase64String(fileBytes);

            Console.WriteLine(CheckBase64.IsBase64String(base64File));
            if (CheckBase64.IsBase64String(base64File))
            {
                //Converte a string Base64 em um array de bytes
                byte[] imageBytes = Convert.FromBase64String(base64File);
                Console.WriteLine($"tamanho: {imageBytes.Length}");

                // Salva a imagem no sistema
                await Task.Run(() => SaveExcel(imageBytes, arquivoExcel));
                responseMessage = $"Excel: Download realizado com sucesso na pasta Downloads";
            }
            else
            {
                Console.WriteLine("Não é um Base64");
                responseMessage = $"Excel: Não é um arquivo válido";
            }
            await webSocket.SendAsync(Encoding.UTF8.GetBytes(responseMessage), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception ex)
        {
            string errorMessage = $"Excel: Erro ao ler o arquivo Excel: {ex.Message}";
            await webSocket.SendAsync(Encoding.UTF8.GetBytes(errorMessage), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }


    // Método para salvar a imagem no sistema de arquivos
    private static async Task SaveExcel(byte[] imageBytes, string fileName)
    {
        try
        {
            // Define o caminho onde a imagem será salva
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            Directory.CreateDirectory(folderPath); // Garante que a pasta existe

            string filePath = Path.Combine(folderPath, $"upload_{fileName}");

            // Salva a imagem
            await File.WriteAllBytesAsync(filePath, imageBytes);
            Console.WriteLine($"Excel saved to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar a imagem: {ex.Message}");
        }
    }

}
