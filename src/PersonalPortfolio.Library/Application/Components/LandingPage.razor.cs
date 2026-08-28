using Microsoft.AspNetCore.Components;
using PersonalPortfolio.Library.Domain;
using PersonalPortfolio.Library.Domain.Mappers;
using PersonalPortfolio.Library.Infrastructure;

namespace PersonalPortfolio.Library.Application.Components;

public partial class LandingPage
{
    private bool _hasLoaded;
    private MainPage _mainPage;
    private PersonalInformation _personalInformation;
    private Card _resume;
    private string _resumeUrl;
    private string _resumeTarget;
    private WebsiteData _websiteDatabaseData;
    [Inject] private IWebsiteRepo WebsiteRepo { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (_hasLoaded) return;

        _websiteDatabaseData = await WebsiteRepo.GetWebsiteData();
        _personalInformation = await WebsiteRepo.GetPersonalInformation();
        _hasLoaded = _websiteDatabaseData is not null;
        _mainPage = _websiteDatabaseData?.MainPage;
        _resume = _mainPage?.Resume;

        if (_resume is not null)
        {
            _resumeUrl = "#resume";
            _resumeTarget = null;
        }
        else
        {
            var resumePage = _websiteDatabaseData?.OtherPages
                .FirstOrDefault(x => x.PageFormat is CardType.Pdf or CardType.ExternalLink);
            _resumeUrl = resumePage?.PageFormat == CardType.Pdf
                ? RouteMapper.GetAllCardsRoute(resumePage.Endpoint)
                : resumePage?.Endpoint;
            _resumeTarget = resumePage?.PageFormat == CardType.ExternalLink ? "_blank" : null;
        }

        StateHasChanged();
    }
}