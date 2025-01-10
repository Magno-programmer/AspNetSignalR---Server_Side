using System.Text;
using System.Net.WebSockets;
using AspNetSignalIR.ClientListen;
using AspNetSignalIR.Controllers;
using AspNetSignalIR.Controllers.HandleDialog;
using AspNetSignalIR.Data.ListDataAtDB;
using AspNetSignalIR.Data.SaveDataInDB;
using AspNetSignalIR.Services.Excel.SearchAnDownload;
using AspNetSignalIR.Services.Excel.SendToClient;
using AspNetSignalIR.Services.Image;
using AspNetSignalIR.Services.GeradorDeGráfico;
using AspNetSignalIR.Services.DBConnector;

namespace AspNetSignalIR.Services.SignalR;

internal class HandleClientClass
{
    public static async Task HandleClientAsync(Client clientId, WebSocket webSocket, Dictionary<Client, WebSocket> _clients)
    {
        string receiveData;
        string receivedMessage;
        WebSocketReceiveResult result;
        byte[] buffer = new byte[2048 * 1024];
        var receivedDataList = new List<string>();

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    receivedDataList.Add(receivedMessage); // For receive image in large scale
                }
                while (!result.EndOfMessage);

                receiveData = $"{string.Join("", receivedDataList.ToList())}";

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    // Handle client disconnection
                    Console.WriteLine($"Client {clientId.Id} : {clientId.Nome} disconnected");
                    Client.RemoverCliente(clientId);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                    _clients.Remove(clientId, out _);
                }
                else if (receiveData.Length >= 128 * 1024)
                {
                    if (receiveData.StartsWith("Enviar_excel_para_o_servidor"))
                    {
                        //Faz a requisição para mostrar todos excels no servidor
                        await ClientsSenderToServer.SendExcel(webSocket, receiveData, _clients, clientId);
                    }
                    else
                    {
                        //Envia uma imagem a um cliente
                        await ImageSave.SendImage(receiveData, _clients, clientId);

                    }
                }else if (receiveData.StartsWith("alterar_conexao"))
                {
                    DBConnection.Executar(receiveData);
                }
                else if (receiveData.StartsWith("Baixar_Excel_especifico: "))
                {
                    // Faz o download de um excel específico do servidor
                    await SendExcelToClients.SendExcelDataToClient(webSocket, receiveData);
                }
                else if (receiveData.StartsWith("Tamanho_do_Excel_especifico:"))
                {
                    // Faz a requisição do tamanho de um excel específico do servidor
                    await SendInformationOfExcelToClients.SendInformationOfExcelToClient(webSocket, receiveData);
                }
                else if (receiveData.StartsWith("Baixar_dataset_especifico:"))
                {
                    await SendDatasetToClients.SendDataSetToClient(webSocket, receiveData);
                }
                else if (receiveData == "Download_do_excel")
                {
                    // Faz o download de um excel do servidor
                    await SendExcelToClients.SendExcelDataToClient(webSocket, string.Empty);
                }
                else if (receiveData == "Tamanho_do_excel")
                {
                    // Faz a requisição do tamanho de um excel do servidor
                    await SendInformationOfExcelToClients.SendInformationOfExcelToClient(webSocket, string.Empty);
                }
                else if (receiveData == "Download_do_dataset")
                {
                    //Faz envio de um excel no formato DataSet
                    await SendDatasetToClients.SendDataSetToClient(webSocket, receiveData);
                }
                else if (receiveData == "Pesquisa_dos_exceis_disponiveis")
                {
                    //Faz a requisição para mostrar todos excels no servidor
                    await ShowAllExcelInServer.ShowAllExcel(webSocket);
                }
                else if (receiveData.StartsWith("Enviar_excel_para_o_servidor"))
                {
                    //Faz a requisição para mostrar todos excels no servidor
                    await ClientsSenderToServer.SendExcel(webSocket, receiveData, _clients, clientId);
                }
                else if (receiveData.StartsWith("Listar_todos_consumidores"))
                {
                    //Faz a listagem de Consumidores
                    await ListDataConsumer.DataCollector(webSocket, receiveData);
                }
                else if (receiveData.StartsWith("Cadastro"))
                {
                    // Extrai os dados do consumidor e processa
                    await CollectConsumerData.DataCollector(webSocket, receiveData);
                }
                else if (receiveData == "generate_graph")
                {
                    // Gera o gráfico e converte para Base64
                    await GraphicGeneration.GenerateGraph(webSocket);
                }
                else if (receiveData.Length <= 128 * 1024)
                {
                    await SendAndReceivedMessages.SendAndReceivedMessage(webSocket, _clients, clientId, receivedMessage);

                }
                receivedDataList.Clear();
            }
            // Close the WebSocket and remove the client from the list
            if (_clients.Remove(clientId, out _))
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro no HandleClientClass: {ex.Message}");
        }
    }
}
