using System.Net.Http.Json;
using PersonalPortfolio.Library.Domain;

namespace PersonalPortfolio.Library.Infrastructure.Repo;

public class WebDatabaseService : IDatabaseService
{
    private readonly HttpClient _httpClient;

    public WebDatabaseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WebsiteDatabaseData> GetWebsiteDatabaseDataAsync()
    {
        return await _httpClient.GetFromJsonAsync<WebsiteDatabaseData>("database/websiteData.json")
               ?? throw new InvalidOperationException();
    }
}