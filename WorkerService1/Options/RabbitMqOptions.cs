using System.ComponentModel.DataAnnotations;

namespace WorkerService1.Options;

public class RabbitMqOptions
{
    internal const string Section = "RabbitMq";
    
    [Required]
    public required string Host { get; set; }
    
    [Required]
    public required string Port { get; set; }
    
    [Required]
    public required string Username { get; set; }
    
    [Required]
    public required string Password { get; set; }
}