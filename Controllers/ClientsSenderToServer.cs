using AspNetSignalIR.ClientListen;
using System.Net.WebSockets;
using System.Text;
using Path = System.IO.Path;
using AspNetSignalIR.Services.Image;

namespace AspNetSignalIR.Controllers;

public class ClientsSenderToServer
{
    internal static async Task SendExcel(WebSocket webSocket, string receiveData, Dictionary<Client, WebSocket> _clients, Client clientId)
    {
        try
        {

            string nameOfArchieve;

            if (receiveData.StartsWith("Enviar_excel_para_o_servidor"))
            {
                int spaceIndex = receiveData.IndexOf(' ');
                int twoDotsIndex = receiveData.IndexOf(':');
                nameOfArchieve = receiveData.Substring(spaceIndex + 1, twoDotsIndex);
                receiveData = receiveData[(twoDotsIndex + 1)..];
            }
            else
            {
                nameOfArchieve = string.Empty;
            }


            Console.WriteLine(CheckBase64.IsBase64String(receiveData));
            if (CheckBase64.IsBase64String(receiveData))
            {
                //Converte a string Base64 em um array de bytes
                byte[] imageBytes = Convert.FromBase64String(receiveData);
                Console.WriteLine($"tamanho: {imageBytes.Length}");

                int twoDotsIndex = nameOfArchieve.IndexOf(':');
                nameOfArchieve = nameOfArchieve[..twoDotsIndex];
                // Salva a imagem no sistema
                await Task.Run(() => SaveExcelOnServer(imageBytes, nameOfArchieve));

                // Processa mensagem de texto normal e faz o broadcast
                string responseMessage = $"Excel: Um novo excel foi salvo no servidor, {nameOfArchieve}";
                await webSocket.SendAsync(Encoding.UTF8.GetBytes(responseMessage), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                Console.WriteLine("Não é um Base64");
                string responseMessage = $"Excel: {nameOfArchieve} Não é um arquivo válido";
                await webSocket.SendAsync(Encoding.UTF8.GetBytes(responseMessage), WebSocketMessageType.Text, true, CancellationToken.None);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error no WebSocket ClientsSenderToServer.SendExcel: {ex.Message}");
        }
    }


    // Método para salvar a imagem no sistema de arquivos
    private static async Task SaveExcelOnServer(byte[] imageBytes, string nameOfArchieve)
    {
        try
        {
            // Define a pasta onde o arquivo Excel está localizado
            string folderPath = Path.Combine("Excel");
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, $"{nameOfArchieve}");

            // Salva a imagem
            await File.WriteAllBytesAsync(filePath, imageBytes);
            Console.WriteLine($"Image saved to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error no WebSocket ClientsSenderToServer.SaveExcelOnServer: {ex.Message}");
        }
    }


}
