using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using static AspNetSignalIR.Data.ApplicationDbContext;

namespace AspNetSignalIR.Data.SaveDataInDB;

internal class CollectConsumerData
{
    // Divisor de banco para teste
    public static async Task DataCollector(WebSocket webSocket, string consumerData)
    {
        try
        {
            consumerData = consumerData.Replace("Cadastro", "");
            int twoDotsIndex = consumerData.IndexOf(':');
            string receiveName = consumerData[..twoDotsIndex];
            string receiveJSON = consumerData[($"{receiveName}".Length + 2)..];

            Console.WriteLine(receiveName);

            await ProcessConsumerDataAsync(webSocket, receiveJSON, receiveName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar os dados do consumidor: {ex.Message}");
        }
    }

    private static async Task ProcessConsumerDataAsync(WebSocket webSocket, string consumerData, string banco)
    {
        try
        {
            // Desserializa o JSON para o objeto Consumidor
            Consumidor consumidor = JsonSerializer.Deserialize<Consumidor>(consumerData)!;

            if (consumidor != null)
            {
                Console.WriteLine($"Dados do consumidor {consumidor.id_consumidor} recebidos:");
                Console.WriteLine($"Nome: {consumidor.nm_consumidor}");
                Console.WriteLine($"Documento: {consumidor.nr_documento}");
                Console.WriteLine($"Tipo de Documento: {consumidor.id_tipo_documento}");
                Console.WriteLine($"Email: {consumidor.ds_email}");
                Console.WriteLine($"Celular: {consumidor.nr_celular}");
                Console.WriteLine($"CRM: {consumidor.fl_crm}");
                Console.WriteLine($"SMS: {consumidor.fl_sms}");
                Console.WriteLine($"Email Marketing: {consumidor.fl_email}");
                Console.WriteLine($"CEP: {consumidor.nr_cep}");
                Console.WriteLine($"Endereço: {consumidor.ds_endereco}");
                Console.WriteLine($"Bairro: {consumidor.ds_bairro}");
                Console.WriteLine($"Cidade: {consumidor.nm_cidade}");
                Console.WriteLine($"UF: {consumidor.sg_uf}");
                Console.WriteLine($"Dia de Aniversário: {consumidor.nr_dia_aniversario}");
                Console.WriteLine($"Mês de Aniversário: {consumidor.nr_mes_aniversario}");

                // Salva o consumidor no banco de dados e obtém uma mensagem de confirmação
                string saveResult = await SaveInDataBase(consumidor, banco);
                Console.WriteLine(saveResult);
            }

            // Envia uma resposta ao cliente confirmando o recebimento dos dados
            var confirmationMessage = "Dados do consumidor recebidos com sucesso.";
            var messageBytes = Encoding.UTF8.GetBytes(confirmationMessage);
            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar os dados do consumidor: {ex.Message}");
        }
    }

    private static async Task<string> SaveInDataBase(Consumidor consumidor, string banco)
    {
        try
        {
            ServiceProvider serviceProvider = ConfiguracaoBanco.ConfigurarConexao(banco)!;

            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContextDinamico>();

                // Insere o registro do consumidor na tabela Consumidores
                context.Consumidores.Add(new Consumidor
                {
                    id_consumidor = consumidor.id_consumidor,
                    nm_consumidor = consumidor.nm_consumidor,
                    nr_documento = consumidor.nr_documento,
                    id_tipo_documento = consumidor.id_tipo_documento,
                    ds_email = consumidor.ds_email,
                    nr_celular = consumidor.nr_celular,
                    fl_crm = consumidor.fl_crm,
                    fl_sms = consumidor.fl_sms,
                    fl_email = consumidor.fl_email,
                    nr_cep = consumidor.nr_cep,
                    ds_endereco = consumidor.ds_endereco,
                    ds_bairro = consumidor.ds_bairro,
                    nm_cidade = consumidor.nm_cidade,
                    sg_uf = consumidor.sg_uf,
                    nr_dia_aniversario = consumidor.nr_dia_aniversario,
                    nr_mes_aniversario = consumidor.nr_mes_aniversario
                });

                // Salva as mudanças no banco de dados
                await context.SaveChangesAsync();
                return "Registro inserido com sucesso.";
            }
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException != null)
            {
                return $"Inner Exception: {ex.InnerException.Message}";
            }
            return $"Erro ao salvar as alterações: {ex.Message}";
        }
        catch (Exception ex)
        {
            return $"Erro ao inserir o registro: {ex.Message}";
        }
    }
}
