using System.Text.Json;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Options;
using MQTTnet;
using WorkerService1.Options;

namespace WorkerService1;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMqttClient _mqttClient;
    private readonly IInfluxDBClient _influxDbClient;
    private readonly MqttClientOptions _mqttClientOptions;
    private readonly InfluxDbOptions _influxDbClientOptions;

    public Worker(
        ILogger<Worker> logger, 
        IOptions<RabbitMqOptions> rabbitMqOptions,
        IOptions<InfluxDbOptions> influxDbOptions)
    {
        var rabbitOptions = rabbitMqOptions.Value;
        var mqttClientFactory = new MqttClientFactory();
        _mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(rabbitOptions.Host, int.Parse(rabbitOptions.Port))
            .WithTlsOptions(o => o.UseTls())
            .WithCredentials(rabbitOptions.Username, rabbitOptions.Password)
            .Build();
        
        _influxDbClientOptions = influxDbOptions.Value;
        
        _logger = logger;
        _influxDbClient = new InfluxDBClient(_influxDbClientOptions.Url.AbsoluteUri, _influxDbClientOptions.Token);
        _mqttClient = mqttClientFactory.CreateMqttClient();
        _mqttClient.ApplicationMessageReceivedAsync += HandleMqttMessageAsync;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _mqttClient.ConnectAsync(_mqttClientOptions, stoppingToken);
            _logger.LogInformation("Connected to MQTT broker");

            await _mqttClient.SubscribeAsync("adaptit/#", cancellationToken: stoppingToken);
            

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "An error occured");
        } 
    }

    private Task HandleMqttMessageAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        using var writeApi = _influxDbClient.GetWriteApi() ;
        _logger.LogInformation("Received message from MQTT broker");
        var jsonData = args.ApplicationMessage.ConvertPayloadToString();
        var data = JsonSerializer.Deserialize<MhSensorData>(jsonData)!;
        
        writeApi.WriteMeasurement(data, WritePrecision.Ns, _influxDbClientOptions.Bucket, _influxDbClientOptions.Org);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _influxDbClient.Dispose();
        _mqttClient.Dispose();
        base.Dispose();
    }
}