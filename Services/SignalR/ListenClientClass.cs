using System.Net;
using System.Text;
using System.Net.WebSockets;
using AspNetSignalIR.ClientListen;

namespace AspNetSignalIR.Services.SignalR;

internal class ListenClientClass
{
    public static async Task ListenForClients(HttpContext context, Dictionary<Client, WebSocket> _clients)
    {
        byte[] buffer = new byte[512000];

        try
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                bool isAuthenticated = false;

                // Continua no loop até autenticação bem-sucedida
                while (!isAuthenticated && webSocket.State == WebSocketState.Open)
                {
                    // Recebe as credenciais do cliente
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    string receivedCredentials = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    // Divide as credenciais em login e senha
                    var credentials = receivedCredentials.Split(':');
                    if (credentials.Length != 2)
                    {
                        // Envia uma mensagem de falha de autenticação
                        string errorMessage = "Failure: Credenciais inválidas!";
                        await webSocket.SendAsync(Encoding.UTF8.GetBytes(errorMessage), WebSocketMessageType.Text, true, CancellationToken.None);
                        continue; // Permite que o cliente tente novamente
                    }

                    string login = credentials[0];
                    string senha = credentials[1];

                    // Exemplo de verificação de credenciais; substituir por uma verificação real, como em um banco de dados
                    Client newClient = new Client(login);

                    if (!newClient.ValidarCredenciais(login, senha))
                    {
                        string errorMessage;

                        // Define uma mensagem de erro específica com base na validação da senha
                        if (senha.Length >= 6)
                        {
                            errorMessage = "Failure: Falha na autenticação. Login ou senha incorretos.";
                        }
                        else
                        {
                            errorMessage = "Failure: Falha na autenticação. Senha muito pequena.";
                        }

                        await webSocket.SendAsync(Encoding.UTF8.GetBytes(errorMessage), WebSocketMessageType.Text, true, CancellationToken.None);
                        Client.RemoverCliente(newClient);
                    }
                    else
                    {
                        // Autenticação bem-sucedida
                        isAuthenticated = true;

                        // Adiciona o cliente à lista de clientes conectados
                        _clients.TryAdd(newClient, webSocket);
                        Console.WriteLine($"Client \nId: {newClient.Id} \nName: {newClient.Nome} \nStatus: conectado");

                        string connectedMessage = $"Successful: Cliente {newClient.Nome} conectado";
                        await webSocket.SendAsync(Encoding.UTF8.GetBytes(connectedMessage), WebSocketMessageType.Text, true, CancellationToken.None);

                        // Inicia o tratamento da comunicação com este cliente autenticado
                        await Task.Run(() => HandleClientClass.HandleClientAsync(newClient, webSocket, _clients));
                    }
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.CompleteAsync(); // Completa a resposta com segurança
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro no ListenClientClass: {ex.Message}");
        }
    }
}
