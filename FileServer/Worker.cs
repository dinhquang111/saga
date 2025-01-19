using System.Text.Json;
using FileServer.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FileServer;

public class Worker(IChannel channel) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await channel.QueueDeclareAsync(queue: "file-server", durable: true, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: stoppingToken);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 3, global: false, cancellationToken: stoppingToken);
        
        // Consumer
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = ListExamPaper.Parser.ParseFrom(body);
            var listPaper = message.ExamPapers.ToArray();
            const string containerFolderPath = "/app/data";
            foreach (var examPaper in listPaper)
            {
                var filePath = Path.Combine(containerFolderPath, $"{examPaper.UId}.txt");
                await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(examPaper), stoppingToken);
            }
            Console.WriteLine("Write batch success");
        };
        
        await channel.BasicConsumeAsync("file-server", autoAck: true, consumer: consumer, cancellationToken: stoppingToken);
        await Task.Run(() => stoppingToken.WaitHandle.WaitOne(), stoppingToken);
    }
}