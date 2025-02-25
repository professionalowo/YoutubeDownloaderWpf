using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeDownloaderWpf.Services.AutoUpdater;

public class GitHubVersionClient(HttpClient client)
{
    [StringSyntax(StringSyntaxAttribute.Uri)]
    public const string LatestUrl = "https://github.com/professionalowo/YoutubeDownloaderWpf/releases/latest";
    public async Task<Updater.Version> GetNewestVersion(CancellationToken token = default)
    {
        HttpResponseMessage response = await client.GetAsync(LatestUrl, token);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new HttpRequestException("Request was not successfull");
        }
        string location = (response.RequestMessage?.RequestUri?.ToString()) ?? throw new HttpRequestException("Could not get newest tag");
        string tag = location.Split("/").Last().Trim().TrimStart('v');
        return Updater.Version.FromTag(tag);
    }
}

