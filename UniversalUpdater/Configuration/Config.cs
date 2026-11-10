namespace UniversalUpdater.Configuration;

public class Config
{
    public class Authorization
    {
        public bool Required { get; init; } = false;
        public string Token { get; init; } = "";
    }
    
    public string Host { get; init; } = "github";
    public string Url { get; init; } = "api.github.com";
    public Authorization Auth { get; init; } = new();
    public string CurrentApplicationVersion { get; init; } = "0.0.0";
}