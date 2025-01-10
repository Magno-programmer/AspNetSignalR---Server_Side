using AspNetSignalIR.Models;
using AspNetSignalIR.Utils;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace AspNetSignalIR.Data;

internal class ApplicationDbContext
{
    public class ApplicationDbContextDinamico : DbContext
    {
        public ApplicationDbContextDinamico(DbContextOptions<ApplicationDbContextDinamico> options) : base(options) { }

        public DbSet<Consumidor> Consumidores { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Consumidor>(entity =>
            {
                entity.ToTable("tb_consumidor");
                entity.HasKey(c => c.id_consumidor);
                entity.Property(c => c.id_consumidor)
                      .ValueGeneratedOnAdd();

            });

            base.OnModelCreating(modelBuilder);
        }
    }

    // Exceções personalizadas
    public class PostgresConnectionException : Exception
    {
        public PostgresConnectionException(string message, Exception innerException)
            : base($"Falha ao conectar ao banco de dados Postgres: {message}", innerException)
        { }
    }

    public class MySQLConnectionException : Exception
    {
        public MySQLConnectionException(string message, Exception innerException)
            : base($"Falha ao conectar ao banco de dados MySQL: {message}", innerException)
        { }
    }

    public class SQLiteConnectionException : Exception
    {
        public SQLiteConnectionException(string message, Exception innerException)
            : base($"Falha ao conectar ao banco de dados SQLite: {message}", innerException)
        { }
    }

    public class AccessConnectionException : Exception
    {
        public AccessConnectionException(string message, Exception innerException)
            : base($"Falha ao conectar ao banco de dados Access ou ao acessar a tabela: {message}", innerException)
        { }
    }

    // Método para configurar a conexão com Postgres
    public static ServiceProvider ConexaoComBancoPostgres()
    {
        try
        {
            var connectionString = AppConfiguration.GetConnectionString("Postgres");

            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContextDinamico>(options =>
                    options.UseNpgsql(connectionString))
                .BuildServiceProvider();

            return serviceProvider;
        }
        catch (Exception ex)
        {
            throw new PostgresConnectionException("Verifique as configurações de conexão ou o status do banco de dados.", ex);
        }
    }

    // Método para configurar a conexão com MySQL
    public static ServiceProvider ConexaoComBancoMySQL()
    {
        try
        {
            var connectionString = AppConfiguration.GetConnectionString("MySQL");

            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContextDinamico>(options =>
                    options.UseMySql(connectionString,
                                     new MySqlServerVersion(new Version(8, 0, 21))))
                .BuildServiceProvider();

            return serviceProvider;
        }
        catch (Exception ex)
        {
            throw new MySQLConnectionException("Verifique as configurações de conexão ou o status do banco de dados.", ex);
        }
    }

    // Método para configurar a conexão com SQLite
    public static ServiceProvider ConexaoComBancoSQLite()
    {
        try
        {
            var connectionString = AppConfiguration.GetConnectionString("SQLite");

            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContextDinamico>(options =>
                    options.UseSqlite(connectionString))
                .BuildServiceProvider();

            return serviceProvider;
        }
        catch (Exception ex)
        {
            throw new SQLiteConnectionException("Verifique as configurações de conexão ou o caminho do arquivo SQLite.", ex);
        }
    }
}