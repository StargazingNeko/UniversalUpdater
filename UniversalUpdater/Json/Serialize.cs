using System.Text.Json;
using UniversalUpdater.Configuration;

namespace UniversalUpdater.Json;

public class Serialize
{
    public async Task<string> SerializeConfig(Config config)
    {
        return JsonSerializer.Serialize(config, new JsonSerializerOptions() {WriteIndented = true});
    }
}