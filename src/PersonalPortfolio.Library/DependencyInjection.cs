using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using PersonalPortfolio.Library.Infrastructure;
using PersonalPortfolio.Library.Infrastructure.Repo;

namespace PersonalPortfolio.Library;

public static class DependencyInjection
{
    public static async Task<IServiceCollection> AddBlazorLibraryAsync(this IServiceCollection services, string baseAddress)
    {
        var websiteRepo =
            new WebsiteRepo(new WebDatabaseService(new HttpClient { BaseAddress = new Uri(baseAddress) }));
        await websiteRepo.EnsureInitializedAsync();
        services.AddSingleton<IWebsiteRepo>(websiteRepo);

        services.AddSingleton<IProfileService, ProfileService>();

        services.AddBlazoredLocalStorageAsSingleton();
        return services;
    }
}