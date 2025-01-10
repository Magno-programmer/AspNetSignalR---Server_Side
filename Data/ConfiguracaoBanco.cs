namespace AspNetSignalIR.Data;

public class ConfiguracaoBanco
{
    public static ServiceProvider? ConfigurarConexao(string banco)
    {
        try
        {
            return banco.ToLower() switch
            {
                "postgres" => ApplicationDbContext.ConexaoComBancoPostgres(),
                "mysql" => ApplicationDbContext.ConexaoComBancoMySQL(),
                "sqlite" => ApplicationDbContext.ConexaoComBancoSQLite(),
                _ => throw new ArgumentException($"Banco de dados '{banco}' não suportado.")
            };
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Erro na configuração de conexão: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro inesperado ao configurar a conexão com o banco '{banco}': {ex.Message}");
            return null;
        }
    }
}