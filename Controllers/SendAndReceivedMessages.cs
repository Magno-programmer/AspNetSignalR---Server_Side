using System.Text;
using System.Net.WebSockets;
using AspNetSignalIR.ClientListen;
using static AspNetSignalIR.Services.OpenAITest.APIOpenAI;
using AspNetSignalIR.Services.SignalR;

namespace AspNetSignalIR.Controllers.HandleDialog;

public class SendAndReceivedMessages
{
    internal static async Task SendAndReceivedMessage(WebSocket webSocket, Dictionary<Client, WebSocket> _clients, Client clientId, string receivedMessage)
    {

        Console.WriteLine($"\nReceived a message from: \nClient Id: {clientId.Id} \nName: {clientId.Nome}");
        if (receivedMessage.StartsWith("::"))
        {
            int spaceIndex = receivedMessage.IndexOf(' ');
            string clientName = spaceIndex > 0 ? receivedMessage[..spaceIndex] : receivedMessage;
            string responseMessageForUniqueClient = $"{clientId.Nome}: {receivedMessage.Replace(clientName, spaceIndex > 0 ? "" : " ")}";
            if (clientName == "::chat")
            {
                string chatBox = await Conectar(receivedMessage);
                string resposta = $"ChatBox: {chatBox}";
                byte[] responseMessageChat = Encoding.UTF8.GetBytes(resposta);
                await webSocket.SendAsync(new ArraySegment<byte>(responseMessageChat), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                await BroadcastClass.BroadcastMessageAsync(responseMessageForUniqueClient, clientId, _clients, clientName);
            }
        }
        else
        {
            await BroadcastClass.BroadcastMessageAsync(receivedMessage, clientId, _clients, string.Empty);
        }
    }
}
