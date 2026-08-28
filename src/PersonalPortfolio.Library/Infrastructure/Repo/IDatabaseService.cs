using PersonalPortfolio.Library.Domain;

namespace PersonalPortfolio.Library.Infrastructure.Repo;

public interface IDatabaseService
{
    Task<WebsiteDatabaseData> GetWebsiteDatabaseDataAsync();
}