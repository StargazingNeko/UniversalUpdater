using System.Text.Json;
using UniversalUpdater.Configuration;

namespace UniversalUpdater.Json;

public class Deserialize
{
    public Config ParseConfig(FileInfo configFile)
    {
        return JsonSerializer.Deserialize<Config>(File.ReadAllText(configFile.FullName)) ?? throw new NullReferenceException();
    }
    
    public GitHub ParseGithubResponse(string response)
    {
        return JsonSerializer.Deserialize<GitHub>(response) ?? throw new NullReferenceException();
    }
}