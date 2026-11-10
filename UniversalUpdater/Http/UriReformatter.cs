using System.Text.RegularExpressions;

namespace UniversalUpdater.Http;

public class UriReformatter
{
    public string GitHub(string uriString)
    {
        bool UrlTest(string uri)
        {
            return Regex.IsMatch(uri, @"https://api\.github\.com/repos/.*?/.*?/releases/latest");
        }

        string CraftCorrectUrl(string uri)
        {
            Match uriMatch = Regex.Match(uri, @"github\.com/(?<owner>[^/]+)/(?<repo>[^/]+)");
            if (uriMatch.Success)
                return $"https://api.github.com/repos/{uriMatch.Groups["owner"].Value}/{uriMatch.Groups["repo"].Value}/releases/latest";
            
            throw new Exception("Invalid URI");
        }
        
        return UrlTest(uriString) ? uriString : CraftCorrectUrl(uriString);
    }
}