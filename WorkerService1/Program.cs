using WorkerService1;
using WorkerService1.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<RabbitMqOptions>()
    .Bind(builder.Configuration.GetSection(RabbitMqOptions.Section))
    .ValidateOnStart();

builder.Services.AddOptions<InfluxDbOptions>()
    .Bind(builder.Configuration.GetSection(InfluxDbOptions.Section))
    .ValidateOnStart();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();