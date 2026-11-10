using System.Net;
using UniversalUpdater.Json;

namespace UniversalUpdater.Http;

/// 
/// Warning, this class has SOME LLM code, any improvements and help to remove anything LLM generated is appreciated!! (I don't like LLM generated code!)
/// LLM generated code are marked with comments!
/// 
public class Client
{
    public Uri? _uri { get; private set;}
    private HttpClient _httpClient = new HttpClient();
    public string Host { get; private set; } = string.Empty;

    public Client Remote(string url, string host)
    {
        Host = host;
        if (host == "github")
        {
            _uri ??= new Uri(new UriReformatter().GitHub(url));
            return this;
        }
        
        Console.WriteLine("Unsupported Host");
        Thread.Sleep(5000);
        throw new Exception("Unsupported Host");
    }

    public async Task<Tuple<HttpResponseMessage, string>> Get()
    {
        if (_uri == null)
            throw new InvalidOperationException("No Url has been set, set this in the config file!!");
        
        if (string.IsNullOrEmpty(Host))
            throw new Exception("Host is not set!!");

        if(Host == "github")
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
        return new Tuple<HttpResponseMessage, string>(await _httpClient.GetAsync(_uri), _uri.ToString());
    }

    public async Task<GitHub.Asset> DoAssetSelectionPrompt(List<GitHub.Asset> assets)
    {
        int index = 0;
        List<string> outString = new();
        foreach (GitHub.Asset asset in assets)
        {
            outString.Add($"[{index++}]: {asset.name}");
        }

        Console.WriteLine(string.Join("\n", outString) + "\nSelect which version to download: ");

        int selectionIndex;
        while (true)
        {
            string? input = Console.ReadLine();
            if (int.TryParse(input, out selectionIndex))
            {
                if (selectionIndex >= 0 && selectionIndex < assets.Count)
                {
                    return assets[selectionIndex];
                }
            }
            
            Console.WriteLine("Invalid selection. Please try again.");
        }
    }
    
    private string GetDownloadUrl(GitHub.Asset asset)
    {
        return asset.browser_download_url;
    }

    //This method is LLM code, could be buggy. I'm bad with fancy console things.. :<
    private void ReportDownloadProgress(float percent)
    {
        const int barWidth = 30;
        percent = Math.Clamp(percent, 0, 100);

        int filled = (int)Math.Floor(percent) * barWidth / 100;
        string bar = new string('■', filled) + new string(' ', barWidth - filled);

        Console.Write($"\r[{bar}] {percent,6:F2}%");
    }
    //LLM generated code end!

    public async Task Download(GitHub.Asset asset)
    {
        string downloadUrl = GetDownloadUrl(asset);
        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
        
        if (LogResponse(httpResponseMessage) != HttpStatusCode.OK)
            throw new Exception("Failed to download asset.");
        
        //LLM code start
        long totalBytes = httpResponseMessage.Content.Headers.ContentLength ?? 0;
        long bytesDownloaded = 0;
        byte[] buffer = new byte[81920];
        Stream downloadStream = await httpResponseMessage.Content.ReadAsStreamAsync();
        Console.Clear();
        await using FileStream fileStream = new FileStream(asset.name, FileMode.Create, FileAccess.Write,
            FileShare.None, buffer.Length, useAsync: true);
        
        if (totalBytes > 0)
            ReportDownloadProgress(0);
        else
            Console.WriteLine("Server did not provide needed information needed to track download progress.\nFile is likely downloading but progress cannot be tracked, wait for \"Done!\" message.");
        
        int bytesRead;
        while ((bytesRead = await downloadStream.ReadAsync(buffer)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            bytesDownloaded += bytesRead;
            
            float progress = 100f * bytesDownloaded / totalBytes;
            ReportDownloadProgress(progress);
        }
        //LLM code end
        
        Console.WriteLine("\nDone!");
    }

    private HttpStatusCode LogResponse(HttpResponseMessage response)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                Console.WriteLine($"{this.GetType().Name}.Get(): {response.StatusCode}");
                break;
            case HttpStatusCode.NotFound:
                Console.WriteLine($"{this.GetType().Name}.Get(): {response.StatusCode}");
                break;
            case HttpStatusCode.Unauthorized:
                Console.WriteLine($"{this.GetType().Name}.Get(): {response.StatusCode}");
                break;
            case HttpStatusCode.Forbidden:
                Console.WriteLine($"{this.GetType().Name}.Get(): {response.StatusCode}");
                break;
            case HttpStatusCode.NoContent:
                Console.WriteLine($"{this.GetType().Name}.Get(): {response.StatusCode}");
                break;
            default:
                Console.WriteLine($"{this.GetType().Name}.Get(): {response.StatusCode}");
                break;
        }
        
        return response.StatusCode;
    }
}