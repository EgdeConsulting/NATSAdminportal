namespace vite_api.Config;

public class AppConfig
{
    [ConfigurationKeyName("NATS_SERVER_URL")]
    public string? NatsServerUrl { get; set; }
}
