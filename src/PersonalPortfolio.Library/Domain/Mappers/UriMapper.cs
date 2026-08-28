namespace PersonalPortfolio.Library.Domain.Mappers;

public static class UriMapper
{
    public static string GetLinkedInUri(string username)
    {
        return $"https://www.linkedin.com/in/{username}/";
    }

    public static string GetMailUri(string username)
    {
        return $"mailto:{username}";
    }

    public static string GetTwitterUri(string username)
    {
        return $"https://twitter.com/{username}";
    }

    public static string GetGitHubUri(string username)
    {
        return $"https://github.com/{username}";
    }

    public static string GetExternalLinked(string url)
    {
        return url;
    }
}