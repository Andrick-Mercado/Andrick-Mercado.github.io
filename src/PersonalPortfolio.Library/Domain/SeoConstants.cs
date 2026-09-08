namespace PersonalPortfolio.Library.Domain;

/// <summary>
/// Site-wide values baked into meta tags, canonical links, and structured data.
/// Keep in sync with the hosting URL and wwwroot/sitemap.xml.
/// </summary>
public static class SeoConstants
{
    public const string SiteUrl = "https://andrick-mercado.github.io";
    public const string SiteName = "Andrick Mercado";
    public const string DefaultImage = SiteUrl + "/images/headshot_01.webp";

    /// <summary>Absolute canonical URL for a site-relative route such as "card/projects".</summary>
    public static string Canonical(string route)
    {
        return string.IsNullOrEmpty(route) ? $"{SiteUrl}/" : $"{SiteUrl}/{route.TrimStart('/')}";
    }

    /// <summary>Absolute URL for a media path stored in websiteData.json (e.g. "images/foo.webp").</summary>
    public static string AbsoluteImage(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return DefaultImage;
        return imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? imageUrl
            : $"{SiteUrl}/{imageUrl.TrimStart('/')}";
    }
}
