using System.Text.Json.Serialization;
using InfluxDB.Client.Core;

namespace WorkerService1;

[Measurement("mh_sensor")]
public class MhSensorData
{
    // Add serial number
    
    [JsonPropertyName("dataType")]
    [Column("data_type")]
    public string DataType { get; set; }
    
    [JsonPropertyName("c_state")]
    [Column("c_state")]
    public int CState { get; set; }
    
    [JsonPropertyName("w_state")]
    [Column("w_state")]
    public int WState { get; set; }
}