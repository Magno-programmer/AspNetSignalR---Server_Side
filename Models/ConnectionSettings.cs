using System.Text.Json.Serialization;

namespace AspNetSignalIR.Models;
public class ConnectionSettingJSON
{
    public ConnectionSettings ConnectionsSetting { get; set; } = new();
}
public class ConnectionSettings
{
    public PostgresSettings Postgres { get; set; } = new();
    public MySQLSettings MySQL { get; set; } = new();
    public SQLiteSettings SQLite { get; set; } = new();
    public AccessSettings Access { get; set; } = new();
}

public class PostgresSettings
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Database { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class MySQLSettings
{
    public string? Server { get; set; }
    public string? Database { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
}

public class SQLiteSettings
{
    public string? DataSource { get; set; }
}

public class AccessSettings
{
    public string? Provider { get; set; }
    public string? DataSource { get; set; }

    [JsonPropertyName("Jet OLEDB")]
    public string? JetOLEDB { get; set; }
    public string? Password { get; set; }
}

public class Root
{
    public ConnectionSettings? ConnectionsSetting { get; set; }
}

