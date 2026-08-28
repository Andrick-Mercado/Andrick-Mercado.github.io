using Microsoft.AspNetCore.Components;
using MudBlazor;
using PersonalPortfolio.Library.Domain;
using PersonalPortfolio.Library.Infrastructure;

namespace PersonalPortfolio.Library.Application.Components;

public partial class ProjectDetailPage
{
    private Card _project;
    private OtherPages _parentPage;
    private bool _hasLoaded;

    [Parameter] public string PageEndpoint { get; set; } = default!;
    [Parameter] public string Slug { get; set; } = default!;
    [Inject] private IWebsiteRepo WebsiteRepo { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        var websiteData = await WebsiteRepo.GetWebsiteData();
        _parentPage = websiteData?.OtherPages
            .FirstOrDefault(x => x.Endpoint == PageEndpoint && x.PageFormat == CardType.Project);
        _project = _parentPage?.Cards
            .FirstOrDefault(c => string.Equals(c.Slug, Slug, StringComparison.OrdinalIgnoreCase));

        _hasLoaded = true;
        StateHasChanged();
    }

    // Normalize common external URLs (e.g. convert YouTube watch links to embed links)
    public string GetEmbedUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return url;

        var trimmed = url.Trim();

        // If already an embed url, return as-is
        if (trimmed.Contains("/embed/", StringComparison.OrdinalIgnoreCase))
            return trimmed;

        // YouTube short link: https://youtu.be/{id}
        if (trimmed.Contains("youtu.be/", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var parts = trimmed.Split('/');
                var last = parts.LastOrDefault();
                if (string.IsNullOrEmpty(last) is false)
                {
                    var id = last.Split('?')[0];
                    return $"https://www.youtube.com/embed/{id}";
                }
            }
            catch
            {
                // fall through
            }
        }

        // Standard YouTube link: https://www.youtube.com/watch?v={id}
        if (trimmed.Contains("watch?v=", StringComparison.OrdinalIgnoreCase))
        {
            var idx = trimmed.IndexOf("watch?v=", StringComparison.OrdinalIgnoreCase);
            var idPart = trimmed.Substring(idx + "watch?v=".Length);
            var amp = idPart.IndexOf('&');
            var id = amp >= 0 ? idPart.Substring(0, amp) : idPart;
            if (!string.IsNullOrEmpty(id))
                return $"https://www.youtube.com/embed/{id}";
        }

        // Default: return original url
        return trimmed;
    }

    private static bool HasMedia(string imageUrl, string videoUrl)
        => string.IsNullOrEmpty(imageUrl) is false || string.IsNullOrEmpty(videoUrl) is false;

    // Every embed here is a multi-megabyte third-party page, so the placeholder has to say
    // what is about to be loaded rather than showing a generic spinner.
    private static bool IsYouTube(string url)
        => url?.Contains("youtube", StringComparison.OrdinalIgnoreCase) ?? false;

    private static bool IsItch(string url)
        => url?.Contains("itch.io", StringComparison.OrdinalIgnoreCase) ?? false;

    private static bool IsLocalDocument(string url)
        => string.IsNullOrEmpty(url) is false
           && url.StartsWith("http", StringComparison.OrdinalIgnoreCase) is false;

    private static string GetEmbedIcon(string url)
    {
        if (IsYouTube(url)) return Icons.Material.Filled.PlayArrow;
        if (IsItch(url)) return Icons.Material.Filled.SportsEsports;
        return IsLocalDocument(url) ? Icons.Material.Filled.AutoGraph : Icons.Material.Filled.OpenInNew;
    }

    private static string GetEmbedActionText(string url)
    {
        if (IsYouTube(url)) return "Play video";
        if (IsItch(url)) return "Launch game";
        return IsLocalDocument(url) ? "Show graphs" : "Load embed";
    }

    private static string GetEmbedDescription(string url)
    {
        if (IsYouTube(url)) return "Loaded from YouTube once you ask for it.";
        if (IsItch(url)) return "Playable build hosted on itch.io.";
        return IsLocalDocument(url)
            ? "Interactive charts, a few megabytes in size."
            : "Loaded from an external site once you ask for it.";
    }

    private static string GetEmbedOpenUrl(string url) => string.IsNullOrEmpty(url) ? null : url;
}