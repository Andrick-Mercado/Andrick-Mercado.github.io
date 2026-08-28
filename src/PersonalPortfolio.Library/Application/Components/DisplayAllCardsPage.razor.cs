using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PersonalPortfolio.Library.Domain;
using PersonalPortfolio.Library.Infrastructure;

namespace PersonalPortfolio.Library.Application.Components;

public partial class DisplayAllCardsPage
{
    private OtherPages _currentPage;

    private bool _hasLoaded;
    private WebsiteData _websiteDatabaseData;
    private HashSet<string> _selectedTags = [];
    private List<string> _availableFilterTags = [];

    [Parameter] public string ClientRouteName { get; set; } = default!;
    [Inject] private IWebsiteRepo WebsiteRepo { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (_hasLoaded && ClientRouteName == _currentPage?.Endpoint) return;

        _websiteDatabaseData = await WebsiteRepo.GetWebsiteData();
        _currentPage = _websiteDatabaseData.OtherPages.FirstOrDefault(x => x.Endpoint == ClientRouteName);

        _hasLoaded = _currentPage is not null;
        BuildFilterTags();
        StateHasChanged();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_websiteDatabaseData is null)
        {
            _websiteDatabaseData = await WebsiteRepo.GetWebsiteData();
            _currentPage = _websiteDatabaseData.OtherPages.FirstOrDefault(x => x.Endpoint == ClientRouteName);
        }
        else if (_currentPage is not null && ClientRouteName != _currentPage.Endpoint)
        {
            _currentPage = _websiteDatabaseData.OtherPages.FirstOrDefault(x => x.Endpoint == ClientRouteName);
        }
        else
        {
            _currentPage ??= _websiteDatabaseData.OtherPages.FirstOrDefault(x => x.Endpoint == ClientRouteName);
        }

        _hasLoaded = _currentPage is not null;
        BuildFilterTags();
        StateHasChanged();
    }

    private void BuildFilterTags()
    {
        if (_currentPage?.PageFormat is not CardType.Project)
        {
            _availableFilterTags = [];
            return;
        }

        if (_currentPage.FilterTags is { Count: > 0 })
        {
            _availableFilterTags = _currentPage.FilterTags;
        }
        else
        {
            _availableFilterTags = _currentPage.Cards
                .SelectMany(c => c.Tags)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        _selectedTags.IntersectWith(_availableFilterTags);
    }

    private void ToggleTag(string tag)
    {
        if (!_selectedTags.Remove(tag))
            _selectedTags.Add(tag);
    }

    private void ClearFilters() => _selectedTags.Clear();

    private IReadOnlyList<Card> GetFilteredProjects()
    {
        if (_currentPage is null) return [];
        if (_selectedTags.Count == 0) return _currentPage.Cards;

        return _currentPage.Cards
            .Where(c => c.Tags.Any(t => _selectedTags.Contains(t)))
            .ToList();
    }

    private IEnumerable<IGrouping<string, Card>> GetCardsGroupedByCardName()
    {
        return _currentPage?.Cards.GroupBy(x => x.Name) ?? [];
    }
}