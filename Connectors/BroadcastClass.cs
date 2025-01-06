using AspNetSignalIR.ClientListen;
using System.Net.WebSockets;
using System.Text;

namespace AspNetSignalIR.Connectors;

internal class BroadcastClass
{
    public static async Task BroadcastMessageAsync(string message, Client clientid, Dictionary<Client, WebSocket> _clients, string specificClient)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        bool clientFound = false;
        try
        {

            foreach (var client in _clients)
            {
                if (client.Value.State == WebSocketState.Open)
                {
                    if (specificClient == string.Empty)
                    {
                        if (!client.Key.Equals(clientid))
                        {
                           await client.Value.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                    else
                    {

                        if (client.Key.Nome.Equals(specificClient[2..]))
                        {
                            await client.Value.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                            clientFound = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }

                    }

                }
            }

            if (!string.IsNullOrEmpty(specificClient) && clientFound == false)
            {
                string notFoundMessage = $"Cliente '{specificClient}' não encontrado para o envio da mensagem.";
                byte[] notFoundMessageBytes = Encoding.UTF8.GetBytes(notFoundMessage);

                if (_clients.TryGetValue(clientid, out WebSocket? senderSocket) && senderSocket.State == WebSocketState.Open)
                {
                    // Envia a mensagem de erro de volta para o cliente que fez a solicitação
                    await senderSocket.SendAsync(new ArraySegment<byte>(notFoundMessageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }

        }catch(Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro no BroadcastClass: {ex.Message}");

        }
    }
}
