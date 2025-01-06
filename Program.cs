using AspNetSignalIR.ClientListen;
using System.Net;
using System.Net.WebSockets;
using static AspNetSignalIR.Connectors.ListenClientClass;
using AspNetSignalIR.Models;

Dictionary<Client, WebSocket> _clients = new();
var builder = WebApplication.CreateBuilder(args);

// Adicione os controladores ao container de serviços
builder.Services.AddControllers();
// Adiciona o SignalR ao contêiner de serviços
builder.Services.AddSignalR();

var app = builder.Build();
app.UseWebSockets();
app.UseRouting();

// Middleware para capturar o IP antes de processar as rotas
app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        // Captura o parâmetro de consulta "ip"
        var ipAddress = context.Request.Query["ip"];

        if (string.IsNullOrEmpty(ipAddress))
        {
            Console.WriteLine("Conexão rejeitada: IP não enviado.");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.CompleteAsync();
            return;
        }

        Console.WriteLine($"Conexão recebida do IP: {ipAddress}");
    }

    await next();
});

app.MapHub<ImageHub>("/imageHub");
app.MapControllers();

app.Map(pattern: "/", requestDelegate: async context =>
{
    try
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.CompleteAsync(); // Completa a resposta de forma segura
            return;
        }
        else
        {
            Task teste = ListenForClients(context, _clients);
            await teste;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ocorreu um erro no app.Map: {ex.Message}");
    }
});

await app.RunAsync();
