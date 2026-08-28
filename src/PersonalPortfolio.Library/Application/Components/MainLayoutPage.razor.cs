using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using PersonalPortfolio.Library.Domain;
using PersonalPortfolio.Library.Infrastructure;

namespace PersonalPortfolio.Library.Application.Components;

public partial class MainLayoutPage
{
    protected override async Task OnInitializedAsync()
    {
        _preferences = await ProfileService.GetPreferences();
        IsDarkCurrentTheme = _preferences.DarkMode;

        _websiteDatabaseData = await WebsiteRepo.GetWebsiteData();
        _configurations = await WebsiteRepo.GetConfigurations();
        _personalInformation = await WebsiteRepo.GetPersonalInformation();
        _mainPage = _websiteDatabaseData.MainPage;

        _hasLoaded = true;
        StateHasChanged();
    }

    /// <summary>
    /// The pre-paint script in index.html forces the dark palette on the prerendered markup while the app boots.
    /// Once the real theme provider has rendered the resolved preference it has to be released, otherwise its
    /// higher specificity would keep the app dark after the user switches back to light mode.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_hasLoaded is false || _hasReleasedPreloadTheme)
        {
            return;
        }

        _hasReleasedPreloadTheme = true;
        await JsRuntime.InvokeVoidAsync("document.documentElement.classList.remove", ThemeManager.PreloadDarkClass);
    }

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private async Task ChangeThemeAsync()
    {
        IsDarkCurrentTheme = await ProfileService.ToggleDarkMode();
        StateHasChanged();
    }

    #region Private fields

    private MudTheme _currentTheme => ThemeManager.GetMudTheme(_configurations?.WebsiteTheme ?? WebsiteTheme.Blue);
    private MudTheme _preloadDarkTheme => ThemeManager.GetPreloadDarkTheme(_configurations?.WebsiteTheme ?? WebsiteTheme.Blue);
    private bool _hasLoaded;
    private bool _hasReleasedPreloadTheme;
    private WebsiteData _websiteDatabaseData;
    private PersonalInformation _personalInformation;
    private Configurations _configurations;
    private MainPage _mainPage;
    private Preferences _preferences = new();
    private bool _drawerOpen = true;

    #endregion

    #region Public fields

    public MudTheme CurrentTheme => _currentTheme;

    public bool IsDarkCurrentTheme { get; private set; }

    #endregion

    #region Injected services

    [Inject] public IProfileService ProfileService { get; set; } = default!;
    [Inject] private IWebsiteRepo WebsiteRepo { get; set; } = default!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    #endregion
}