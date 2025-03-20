using System.ComponentModel.DataAnnotations;

namespace WorkerService1.Options;

public class InfluxDbOptions
{
    internal const string Section = "InfluxDB";
    
    [Required]
    public required Uri Url { get; set; }
    
    [Required]
    public required string Token { get; set; }
    
    [Required]
    public required string Org { get; set; }
    
    [Required]
    public required string Bucket { get; set; }
}