using MudBlazor;
using MudBlazor.Utilities;

namespace PersonalPortfolio.Library.Domain;

public static class ThemeManager
{
    /// <summary>
    /// CSS class placed on the html element by the pre-paint script in index.html when the stored
    /// preference is dark mode. It is also the scope of the theme returned by
    /// <see cref="GetPreloadDarkTheme" />.
    /// </summary>
    public const string PreloadDarkClass = "mud-preload-dark";

    /// <summary>
    /// Same theme as <see cref="GetMudTheme" /> but the generated CSS variables are scoped to
    /// <c>:root.mud-preload-dark</c> instead of <c>:root</c>. Rendering it produces a stylesheet that is
    /// captured by the prerenderer, letting the static snapshot be shown in dark mode before the
    /// WebAssembly app has booted and can apply the real theme.
    /// </summary>
    public static MudTheme GetPreloadDarkTheme(WebsiteTheme websiteTheme)
    {
        var theme = GetMudTheme(websiteTheme);
        theme.PseudoCss.Scope = $"root.{PreloadDarkClass}";

        return theme;
    }

    public static MudTheme GetMudTheme(WebsiteTheme websiteTheme)
    {
        var theme = new MudTheme();

        switch (websiteTheme)
        {
            case WebsiteTheme.Blue:
                theme = new MudTheme
                {
                    PaletteLight = new PaletteLight
                    {
                        AppbarBackground = "#0097FF",
                        AppbarText = "#FFFFFF",
                        Primary = "#007CD1",
                        TextPrimary = "#0C1217",
                        Background = "#F4FDFF",
                        TextSecondary = "#0C1217",
                        DrawerBackground = "#C5E5FF",
                        DrawerIcon = "#000000",
                        DrawerText = "#0C1217",
                        Surface = "#E4FAFF",
                        ActionDefault = "#0C1217",
                        ActionDisabled = "#2F678C",
                        TextDisabled = "#676767"
                    },
                    PaletteDark = new PaletteDark
                    {
                        AppbarBackground = "#0097FF",
                        AppbarText = "#FFFFFF",
                        Primary = "#007CD1",
                        Secondary = "#000000",
                        TextPrimary = "#FFFFFF",
                        Background = "#001524",
                        TextSecondary = "#E2EEF6",
                        DrawerBackground = "#093958",
                        DrawerIcon = "#FFFFFF",
                        DrawerText = "#FFFFFF",
                        Surface = "#093958",
                        ActionDefault = "#0C1217",
                        ActionDisabled = "#2F678C",
                        TextDisabled = "#B0B0B0"
                    }
                };
                break;

            case WebsiteTheme.Green:
                theme = new MudTheme
                {
                    PaletteLight = new PaletteLight
                    {
                        Primary = "#00A76F",
                        PrimaryDarken = "#007867",
                        PrimaryLighten = "#5BE49B",
                        Secondary = "#1C252E",
                        AppbarBackground = "rgba(255,255,255,0.85)",
                        AppbarText = "#1C252E",
                        Background = "#F7F9F8",
                        Surface = "#FFFFFF",
                        DrawerBackground = "#FFFFFF",
                        DrawerText = "#1C252E",
                        DrawerIcon = "#00A76F",
                        TextPrimary = "#1C252E",
                        TextSecondary = "#637381",
                        ActionDefault = "#637381",
                        LinesDefault = "#E9EEEC",
                        Divider = "#E9EEEC",
                        Success = "#00A76F"
                    },
                    PaletteDark = new PaletteDark
                    {
                        Primary = "#5BE49B",
                        PrimaryDarken = "#00A76F",
                        PrimaryLighten = "#C8FAD6",
                        Secondary = "#C8FAD6",
                        AppbarBackground = "rgba(20,26,33,0.85)",
                        AppbarText = "#F4F6F8",
                        Background = "#141A21",
                        Surface = "#1C252E",
                        DrawerBackground = "#1C252E",
                        DrawerText = "#F4F6F8",
                        DrawerIcon = "#5BE49B",
                        TextPrimary = "#F4F6F8",
                        TextSecondary = "#919EAB",
                        ActionDefault = "#919EAB",
                        LinesDefault = "#2F3944",
                        Divider = "#2F3944",
                        Success = "#5BE49B"
                    },
                    LayoutProperties = new LayoutProperties
                    {
                        DefaultBorderRadius = "12px"
                    }
                };
                break;
        }

        return theme;
    }
}