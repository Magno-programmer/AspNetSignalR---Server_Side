using AspNetSignalIR.Models;
using System.Text.Json;

namespace AspNetSignalIR.Utils;

public class AppConfiguration
{
    private static readonly string ConfigFilePath = "appsettings.json";
    public static IConfiguration? Configuration { get; }
    public static ConnectionSettingJSON Settings => LoadSettings();

    public static void SaveSettings(ConnectionSettingJSON settings)
    {
        try
        {
            JsonSerializerOptions options = new() { WriteIndented = true };
            var json = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(ConfigFilePath, json);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Erro ao salvar configurações: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Permissão negada ao salvar o arquivo: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro inesperado ao salvar configurações: {ex.Message}");
        }
    }

    public static ConnectionSettingJSON LoadSettings()
    {
        try
        {
            if (!File.Exists(ConfigFilePath))
                return new ConnectionSettingJSON();

            var json = File.ReadAllText(ConfigFilePath);
            return JsonSerializer.Deserialize<ConnectionSettingJSON>(json) ?? new ConnectionSettingJSON();
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Arquivo de configuração não encontrado: {ex.Message}");
            throw;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Erro ao ler o JSON de configuração: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro inesperado na inicialização da configuração: {ex.Message}");
            throw;
        }
    }
    public static string GetConnectionString(string database)
    {
        try
        {
            return database.ToLower() switch
            {
                "postgres" => $"Host={Settings.ConnectionsSetting.Postgres.Host};" +
                              $"Port={Settings.ConnectionsSetting.Postgres.Port};" +
                              $"Database={Settings.ConnectionsSetting.Postgres.Database};" +
                              $"Username={Settings.ConnectionsSetting.Postgres.Username};" +
                              $"Password={Settings.ConnectionsSetting.Postgres.Password}",

                "mysql" => $"Server={Settings.ConnectionsSetting.MySQL.Server};" +
                           $"Database={Settings.ConnectionsSetting.MySQL.Database};" +
                           $"User={Settings.ConnectionsSetting.MySQL.User};" +
                           $"Password={Settings.ConnectionsSetting.MySQL.Password}",

                "sqlite" => $"Data Source={Settings.ConnectionsSetting.SQLite.DataSource}",

                "access" => $"Provider={Settings.ConnectionsSetting.Access.Provider};" +
                            $"Data Source={Settings.ConnectionsSetting.Access.DataSource};" +
                            $"Jet OLEDB:{Settings.ConnectionsSetting.Access.JetOLEDB} " +
                            $"Password={Settings.ConnectionsSetting.Access.Password}",

                _ => throw new ArgumentException("Banco de dados não suportado.")
            };
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Erro na obtenção da string de conexão: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro inesperado ao obter a string de conexão: {ex.Message}");
            throw;
        }
    }
}
