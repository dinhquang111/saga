using System.Text;
using System.Text.Json;
using Background.Model;
using Google.Protobuf;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Background;

public class Worker(IChannel channel) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await channel.QueueDeclareAsync(queue: "background", durable: true, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: stoppingToken);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 3, global: false, cancellationToken: stoppingToken);
        // Consumer
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var model = JsonSerializer.Deserialize<int[]>(message);
            var listPaper = new ListExamPaper
            {
                ExamPapers = { model.Select(GetPaper) }
            };
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "file-server", body: listPaper.ToByteArray(), cancellationToken: stoppingToken);
        };
        await channel.BasicConsumeAsync("background", autoAck: true, consumer: consumer, cancellationToken: stoppingToken);
        await Task.Run(() => stoppingToken.WaitHandle.WaitOne(), stoppingToken);
    }

    private ExamPaper GetPaper(int id)
    {
        return new ExamPaper
        {
            UId = id,
            TId = 1,
            TCId = 1,
            Questions =
            {
                new Question
                {
                    Id = 1,
                    Content = "Question 1",
                    Answers =
                    {
                        new Answer
                        {
                            Id = 1,
                            Content = "Asnwer 1"
                        }
                    }
                }
            }
        }; 
    }
}