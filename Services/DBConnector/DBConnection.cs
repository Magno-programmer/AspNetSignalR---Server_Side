using AspNetSignalIR.Utils;
using static AspNetSignalIR.Models.ConnectionSettings;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using System.Text.Json;
using AspNetSignalIR.Models;

namespace AspNetSignalIR.Services.DBConnector;

public class DBConnection
{
    public static void Executar(string receiveData)
    {
        try
        {
            // Divide a mensagem recebida no formato "alterar_conexao:Database:jsonContent"
            var parts = receiveData.Split(':', 3);

            if (parts.Length < 3 || parts[0] != "alterar_conexao")
            {
                Console.WriteLine("Formato inválido da mensagem.");
                return;
            }

            string banco = parts[1].ToLower(); // Nome do banco (Postgres, MySQL, SQLite, Access)
            string jsonContent = parts[2]; // Conteúdo JSON com as configurações

            // Carrega as configurações atuais do arquivo
            var currentSettings = AppConfiguration.LoadSettings();

            // Atualiza as configurações do banco correspondente
            switch (banco)
            {
                case "postgres":
                    var postgresConfig = JsonSerializer.Deserialize<PostgresSettings>(jsonContent);
                    if (postgresConfig != null)
                    {
                        currentSettings.ConnectionsSetting.Postgres = postgresConfig;
                        Console.WriteLine("Configurações do Postgres atualizadas.");
                    }
                    break;

                case "mysql":
                    var mysqlConfig = JsonSerializer.Deserialize<MySQLSettings>(jsonContent);
                    if (mysqlConfig != null)
                    {
                        currentSettings.ConnectionsSetting.MySQL = mysqlConfig;
                        Console.WriteLine("Configurações do MySQL atualizadas.");
                    }
                    break;

                case "sqlite":
                    var sqliteConfig = JsonSerializer.Deserialize<SQLiteSettings>(jsonContent);
                    if (sqliteConfig != null)
                    {
                        currentSettings.ConnectionsSetting.SQLite = sqliteConfig;
                        Console.WriteLine("Configurações do SQLite atualizadas.");
                    }
                    break;

                case "access":
                    var accessConfig = JsonSerializer.Deserialize<AccessSettings>(jsonContent);
                    if (accessConfig != null)
                    {
                        currentSettings.ConnectionsSetting.Access = accessConfig;
                        Console.WriteLine("Configurações do Access atualizadas.");
                    }
                    break;

                default:
                    Console.WriteLine($"Banco de dados '{banco}' não reconhecido.");
                    return;
            }

            // Salva as configurações atualizadas
            AppConfiguration.SaveSettings(currentSettings);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao processar a mensagem: {ex.Message}");
        }
    }
}
