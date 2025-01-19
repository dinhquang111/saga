using Background;
using FileServer;

var builder = Host.CreateApplicationBuilder(args);

await builder.Services.AddRabbitMqAsync(builder.Configuration);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();