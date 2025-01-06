using AspNetSignalIR.ConnectionDB;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using static AspNetSignalIR.ConnectionDB.ApplicationDBContext;

namespace AspNetSignalIR.ListDataAtDB;

public class ListDataConsumer
{
    private static string? json;

    public static async Task DataCollector(WebSocket webSocket, string consumerData)
    {
        try
        {
            consumerData = consumerData.Replace("Listar_todos_consumidores", "");
            int twoDotsIndex = consumerData.IndexOf(':');
            string receiveName = consumerData[..twoDotsIndex];

            Console.WriteLine(receiveName);

            await FetchAllConsumersAsync(webSocket, receiveName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar os dados do consumidor: {ex.Message}");
        }
    }

    private static async Task FetchAllConsumersAsync(WebSocket webSocket, string banco)
    {

        try
        {
            ServiceProvider serviceProvider;

            // Configuração do provedor de serviços e do DbContext
            if (banco == "Postgres") serviceProvider = ApplicationDBContext.ConexaoComBancoPostgres();
            else if (banco == "MySQL") serviceProvider = ApplicationDBContext.ConexaoComBancoMySQL();
            else if (banco == "SQLite") serviceProvider = ApplicationDBContext.ConexaoComBancoSQLite();
            else return;

            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContextConsumidor>();

                // Consulta para obter todos os consumidores
                List<Consumidor> consumidores = await context.Consumidores.ToListAsync();

                // Serializa a lista de consumidores em JSON
                json = JsonSerializer.Serialize(consumidores);
                byte[] dataBytes = Encoding.UTF8.GetBytes("JSON: " + json);
                await webSocket.SendAsync(new ArraySegment<byte>(dataBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine("Consumidores listados com sucesso!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar consumidores: {ex.Message}");
            json = $"Erro ao buscar consumidores: {ex.Message}";
            byte[] dataBytes = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(dataBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
