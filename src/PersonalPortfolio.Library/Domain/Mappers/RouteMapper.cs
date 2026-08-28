namespace PersonalPortfolio.Library.Domain.Mappers;

public static class RouteMapper
{
    public static string GetAllCardsRoute(string endpoint)
    {
        return $"card/{endpoint}";
    }

    public static string GetProjectDetailRoute(string pageEndpoint, string slug)
    {
        return $"card/{pageEndpoint}/{slug}";
    }
}