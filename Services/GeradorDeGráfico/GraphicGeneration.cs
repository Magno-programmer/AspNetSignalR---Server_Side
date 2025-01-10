using SkiaSharp;
using System.Net.WebSockets;
using System.Text;

namespace AspNetSignalIR.Services.GeradorDeGráfico;

public class GraphicGeneration
{
    public static async Task GenerateGraph(WebSocket webSocket)
    {
        try
        {
            Console.WriteLine("Gerando gráfico no servidor...");

            // Dimensões do gráfico
            int width = 600;
            int height = 400;

            // Geração de valores randômicos
            var random = new Random();
            int numberOfSlices = random.Next(4, 10); // Número de fatias no gráfico
            var valores = new float[numberOfSlices];

            for (int i = 0; i < numberOfSlices; i++)
            {
                valores[i] = random.Next(10, 50); // Valores aleatórios entre 10 e 50
            }

            // Normaliza os valores para que representem percentuais
            float total = valores.Sum();
            valores = valores.Select(v => v / total * 100).ToArray();

            // Gerar labels automaticamente como letras maiúsculas
            var labels = Enumerable.Range(0, numberOfSlices) // Começa em 0
                .Select(i => ((char)('A' + i)).ToString()) // Converte índice em letra ASCII
                .ToArray();

            // Gerar cores automaticamente
            var cores = Enumerable.Range(0, numberOfSlices)
                .Select(_ => new SKColor(
                    (byte)random.Next(0, 256), // R
                    (byte)random.Next(0, 256), // G
                    (byte)random.Next(0, 256)  // B
                ))
                .ToArray();

            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;

            // Configura o plano de fundo
            canvas.Clear(SKColors.White);

            // Centro do gráfico e raio
            var center = new SKPoint(width / 2, height / 2);
            float radius = Math.Min(width, height) * 0.4f;

            // Variável para rastrear o início de cada fatia
            float startAngle = 0;

            // Pintura para desenhar o texto
            var textPaint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 16,
                IsAntialias = true
            };

            // Desenhar as fatias do gráfico de pizza
            for (int i = 0; i < valores.Length; i++)
            {
                float sweepAngle = valores[i] / 100 * 360; // Ângulo proporcional

                // Configurar pintura para a fatia
                var slicePaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = cores[i],
                    IsAntialias = true
                };

                // Desenhar a fatia
                var rect = new SKRect(center.X - radius, center.Y - radius, center.X + radius, center.Y + radius);
                canvas.DrawArc(rect, startAngle, sweepAngle, true, slicePaint);

                // Calcular o ângulo médio para posicionar o rótulo
                float midAngle = startAngle + sweepAngle / 2;
                float labelX = center.X + radius / 1.5f * (float)Math.Cos(midAngle * Math.PI / 180);
                float labelY = center.Y + radius / 1.5f * (float)Math.Sin(midAngle * Math.PI / 180);

                // Desenhar o rótulo
                canvas.DrawText($"{labels[i]} ({valores[i]:F1}%)", labelX - 20, labelY, textPaint);

                // Atualizar o ângulo de início
                startAngle += sweepAngle;
            }

            // Converter o canvas para uma imagem PNG
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            // Converter o array de bytes para Base64
            string base64Graph = Convert.ToBase64String(data.ToArray());

            // Enviar a imagem como Base64 via WebSocket
            await webSocket.SendAsync(Encoding.UTF8.GetBytes(base64Graph),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );

            Console.WriteLine("Gráfico enviado para o cliente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao gerar gráfico: {ex.Message}");

            // Envia mensagem de erro para o cliente
            string errorMessage = "Erro ao gerar gráfico. Por favor, tente novamente.";
            await webSocket.SendAsync(Encoding.UTF8.GetBytes(errorMessage),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }
    }

}
