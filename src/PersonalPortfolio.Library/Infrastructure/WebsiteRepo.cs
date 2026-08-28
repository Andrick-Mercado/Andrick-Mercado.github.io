using PersonalPortfolio.Library.Domain;
using PersonalPortfolio.Library.Infrastructure.Repo;

namespace PersonalPortfolio.Library.Infrastructure;

public interface IWebsiteRepo
{
    Task<Configurations> GetConfigurations();
    Task<PersonalInformation> GetPersonalInformation();
    Task<WebsiteData> GetWebsiteData();
}

public class WebsiteRepo : IWebsiteRepo
{
    private readonly IDatabaseService _databaseService;
    private bool _initialized;
    private WebsiteDatabaseData _websiteDatabaseData = default!;

    public WebsiteRepo(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<Configurations> GetConfigurations()
    {
        await EnsureInitializedAsync();
        return _websiteDatabaseData.Configurations;
    }

    public async Task<PersonalInformation> GetPersonalInformation()
    {
        await EnsureInitializedAsync();
        return _websiteDatabaseData.PersonalInformation;
    }

    public async Task<WebsiteData> GetWebsiteData()
    {
        await EnsureInitializedAsync();
        return _websiteDatabaseData.WebsiteData;
    }

    public async Task EnsureInitializedAsync()
    {
        if (_initialized is false)
        {
            _websiteDatabaseData = await _databaseService.GetWebsiteDatabaseDataAsync();
            _initialized = true;
        }
    }
}