using AspNetSignalIR.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetSignalIR.ConnectionDB;

public class ApplicationDBContext
{
  
    public class ApplicationDbContextConsumidor : DbContext
    {
        public ApplicationDbContextConsumidor(DbContextOptions<ApplicationDbContextConsumidor> options) : base(options) { }

        public DbSet<Consumidor> Consumidores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Consumidor>(entity =>
            {
                entity.ToTable("tb_consumidor"); // Define o nome da tabela no banco de dados
                entity.HasKey(e => e.id_consumidor);
                entity.Property(e => e.id_consumidor)
                      .ValueGeneratedOnAdd();
            });

            base.OnModelCreating(modelBuilder);
        }
    }

    // Método para configurar a conexão com Postgres
    public static ServiceProvider ConexaoComBancoPostgres()
    {
        var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContextConsumidor>(options =>
                    options.UseNpgsql("Host=localhost;Port=5432;Database=*;Username=postgres;Password=*"))
                .BuildServiceProvider();

        return serviceProvider;
    }

    // Método para configurar a conexão com MySQL
    public static ServiceProvider ConexaoComBancoMySQL()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<ApplicationDbContextConsumidor>(options =>
                options.UseMySql("Server=localhost;Database=*;User=root;Password=*;",
                                 new MySqlServerVersion(new Version(8, 0, 21)))) // Use a versão do MySQL instalada em seu sistema
            .BuildServiceProvider();

        return serviceProvider;
    }

    // Método para configurar a conexão com SQLite
    public static ServiceProvider ConexaoComBancoSQLite()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<ApplicationDbContextConsumidor>(options =>
                options.UseSqlite(@"Data Source=DBInforlube\inforlube_6.db")) // Especifique o caminho do arquivo SQLite
            .BuildServiceProvider();

        return serviceProvider;
    }



}
