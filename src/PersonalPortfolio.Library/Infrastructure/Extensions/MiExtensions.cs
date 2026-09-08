using System.Text.Json;

namespace PersonalPortfolio.Library.Infrastructure.Extensions;

public static class MiExtensions
{
    private static readonly JsonSerializerOptions CamelCaseOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static T DeserializeWithCamelCase<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, CamelCaseOptions);
    }

    public static string Serialize<T>(this T data)
    {
        return JsonSerializer.Serialize(data);
    }

    public static string FirstCharToUpper(this string input)
    {
        return input switch
        {
            null => string.Empty,
            "" => input,
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };
    }

    /// <summary>
    /// Collapses whitespace and truncates at a word boundary so the result fits a
    /// search-result snippet (Google shows roughly 160 characters).
    /// </summary>
    public static string ToMetaDescription(this string text, int maxLength = 160)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        var collapsed = string.Join(' ', text.Split(default(char[]), StringSplitOptions.RemoveEmptyEntries));
        if (collapsed.Length <= maxLength) return collapsed;

        var truncated = collapsed[..(maxLength - 1)];
        var lastSpace = truncated.LastIndexOf(' ');
        if (lastSpace > 0) truncated = truncated[..lastSpace];

        return truncated.TrimEnd(',', ';', ':', '.', '—', '-') + "…";
    }
}