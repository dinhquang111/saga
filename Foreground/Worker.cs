using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Foreground;

public class Worker(IChannel channel) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await channel.QueueDeclareAsync(queue: "foreground", durable: true, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: stoppingToken);
        
        // Consumer
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Message {message} is save successfully");
            return Task.CompletedTask;
        };
        await channel.BasicConsumeAsync("foreground", autoAck: true, consumer: consumer, cancellationToken: stoppingToken);
        
        // Producer
        await Task.Delay(10000, stoppingToken);
        for (var i = 1; i < 1001; i++)
        {
            var list = Enumerable.Range((i - 1) * 100, 100).ToArray();
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(list));
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "background", body: body, cancellationToken: stoppingToken);
        }
        await Task.Run(() => stoppingToken.WaitHandle.WaitOne(), stoppingToken);
    }
}