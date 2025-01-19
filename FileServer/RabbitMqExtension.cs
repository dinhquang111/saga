using RabbitMQ.Client;

namespace Background;

public static class RabbitMqExtension
{
    public static async Task AddRabbitMqAsync(this IServiceCollection services, IConfiguration configuration)
    {
        var factory = configuration.GetSection("RabbitMQ").Get<ConnectionFactory>();
        if (factory == null)
            throw new ArgumentException("Invalid rabbitmq factory");
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();
        services.AddSingleton(channel);
    }
}