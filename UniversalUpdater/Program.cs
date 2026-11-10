// See https://aka.ms/new-console-template for more information

using UniversalUpdater.Configuration;
using UniversalUpdater.Http;
using UniversalUpdater.Json;

namespace UniversalUpdater;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        FileInfo configFile = new("config.json");
        if (!configFile.Exists)
        {
            using (StreamWriter writer = configFile.CreateText())
                await writer.WriteAsync(await new Serialize().SerializeConfig(new Config()));
            
            Console.WriteLine("Config file created\nAdd the values and re-run the program\nPress any key to exit");
            Console.ReadKey();
            Environment.Exit(0);
        }
        
        Client client = new();
        Config config = new Deserialize().ParseConfig(configFile);
        string currentVersion = config.CurrentApplicationVersion;
        bool configNeedsUpdate = false;
        
        (HttpResponseMessage response, string newUrl) = await client.Remote(config.Url, config.Host).Get();
        GitHub GitHubObj = new Deserialize().ParseGithubResponse(response.Content.ReadAsStringAsync().Result);
        
        if (newUrl != config.Url || currentVersion != GitHubObj.tag_name)
            configNeedsUpdate = true;
        
        if (currentVersion != GitHubObj.tag_name)
            await client.Download(await client.DoAssetSelectionPrompt(GitHubObj.assets));
        
        if (configNeedsUpdate)
            config = new Config() {Auth = config.Auth, Url = newUrl, CurrentApplicationVersion = GitHubObj.tag_name, Host = config.Host};
        
        if (configNeedsUpdate)
            using (StreamWriter writer = configFile.CreateText())
                await writer.WriteAsync(await new Serialize().SerializeConfig(config));
    }
}