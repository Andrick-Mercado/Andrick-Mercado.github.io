using Microsoft.AspNetCore.Components;
using PersonalPortfolio.Library.Domain;
using PersonalPortfolio.Library.Infrastructure;

namespace PersonalPortfolio.Library.Application.Components;

public partial class SideBarPage
{
    private bool hasLoaded;
    private IOrderedEnumerable<OtherPages> sortedOtherPages;
    private WebsiteData websiteData;
    [Inject] private IWebsiteRepo WebsiteRepo { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (hasLoaded) return;

        websiteData = await WebsiteRepo.GetWebsiteData();
        hasLoaded = websiteData is not null;
        sortedOtherPages = websiteData?.OtherPages?.OrderBy(x => x.SortOrder);
        StateHasChanged();
    }
}