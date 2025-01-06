using AspNetSignalIR.ClientListen;
using AspNetSignalIR.Connectors;
using System.Net.WebSockets;

namespace AspNetSignalIR.ImageConfigurations;

public class ImageSave
{

    internal static async Task SendImage(string receiveData, Dictionary<Client, WebSocket> _clients, Client clientId)
    {
        string clientName;

        if (receiveData.StartsWith("::"))
        {
            int spaceIndex = receiveData.IndexOf(' ');
            clientName = receiveData[..spaceIndex];
            receiveData = receiveData[(spaceIndex + 1)..];
        }
        else
        {
            clientName = string.Empty;
        }

        Console.WriteLine(CheckBase64.IsBase64String(receiveData));
        if (CheckBase64.IsBase64String(receiveData))
        {
            //Converte a string Base64 em um array de bytes
            byte[] imageBytes = Convert.FromBase64String(receiveData);
            Console.WriteLine($"tamanho: {imageBytes.Length}");

            // Salva a imagem no sistema
            await Task.Run(() => ImageSave.SaveImage(imageBytes, clientId));

            // Processa mensagem de texto normal e faz o broadcast
            string responseMessage = $"{clientId.Nome}: {clientId.Nome} enviou uma imagem.";
            await BroadcastClass.BroadcastMessageAsync(responseMessage, clientId, _clients, clientName);
        }
        else
        {
            Console.WriteLine("Não é um Base64");
            string responseMessage = $"Não é um arquivo válido";
            await BroadcastClass.BroadcastMessageAsync(responseMessage, clientId, _clients, clientName);
        }

    }


    // Método para salvar a imagem no sistema de arquivos
    private static async Task SaveImage(byte[] imageBytes, Client client)
    {
        try
        {
            // Define o caminho onde a imagem será salva
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            Directory.CreateDirectory(folderPath); // Garante que a pasta existe

            string filePath = Path.Combine(folderPath, $"{client.Nome}_{DateTime.Now:yyyyMMddHHmmss}.jpg");

            // Salva a imagem
            await File.WriteAllBytesAsync(filePath, imageBytes);
            Console.WriteLine($"Image saved to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar a imagem: {ex.Message}");
        }
    }


}
