using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AspNetSignalIR.Controllers;

public class ImageHub : Hub
{
    public async Task SendImage(byte[] imageBytes, string fileName)
    {
        // Envia a imagem para todos os clientes conectados
        await Clients.All.SendAsync("ReceiveImage", imageBytes, fileName);
    }
}
