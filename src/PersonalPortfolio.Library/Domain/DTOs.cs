using System.Text.Json.Serialization;

namespace PersonalPortfolio.Library.Domain;

public class WebsiteDatabaseData
{
    public Configurations Configurations { get; set; }
    public PersonalInformation PersonalInformation { get; set; }
    public WebsiteData WebsiteData { get; set; }
}

public class Configurations
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WebsiteTheme WebsiteTheme { get; set; }

    public bool EnableDarkMode { get; set; }
}

public class PersonalInformation
{
    public Person Person { get; set; }
    public SocialMediaLinks SocialMediaLinks { get; set; }
}

public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
}

public class SocialMediaLinks
{
    public string LinkedIn { get; set; }
    public string Mail { get; set; }
    public string Twitter { get; set; }
    public string Github { get; set; }
}

public class WebsiteData
{
    public MainPage MainPage { get; set; }
    public List<OtherPages> OtherPages { get; set; }
}

public class MainPage
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Headline { get; set; }
    public string Tagline { get; set; }
    public string Location { get; set; }
    public string ImageRoute { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Icon Icon { get; set; }

    public List<string> Paragraphs { get; set; }
    public List<string> Highlights { get; set; } = [];
    public Card Resume { get; set; }
}

public class OtherPages
{
    public int SortOrder { get; set; }
    public string Title { get; set; }
    public string Endpoint { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CardType PageFormat { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Icon Icon { get; set; }

    public List<Card> Cards { get; set; }
    public List<string> FilterTags { get; set; } = [];
}

public class Card
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Period { get; set; }
    public string Body { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Icon Icon { get; set; }

    public string ImageUrl { get; set; }

    /// <summary>Path to an MP4 clip; the WebM sibling is derived from it.</summary>
    public string VideoUrl { get; set; }

    /// <summary>Still frame shown before the clip is played.</summary>
    public string PosterUrl { get; set; }

    public string EmbedUrl { get; set; }
    public string DownloadUrl { get; set; }
    public string LearnMoreUrl { get; set; }
    public string Slug { get; set; }
    public List<string> Tags { get; set; } = [];
    public List<BlogSection> BlogSections { get; set; } = [];
}

public class BlogSection
{
    public string Title { get; set; }
    public string Body { get; set; }
    public string ImageUrl { get; set; }

    /// <summary>Path to an MP4 clip; the WebM sibling is derived from it.</summary>
    public string VideoUrl { get; set; }

    /// <summary>Still frame shown before the clip is played.</summary>
    public string PosterUrl { get; set; }

    public string ImageCaption { get; set; }
    public string EmbedUrl { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BlogSectionType SectionType { get; set; }
}

public enum BlogSectionType
{
    Text,
    Image,
    TextWithImage,
    ExternalIframe
}

#region Enums

public enum WebsiteTheme
{
    Blue,
    Green
}

public enum CardType
{
    Experience,
    Education,
    Project,
    ExternalLink,
    Pdf
}

public enum Icon
{
    AccountCircle,
    Lightbulb,
    Construction,
    AutoGraph,
    InsertDriveFile,
    Work,
    School,
    Code,
    SportsEsports,
    Cloud,
    Storage,
    Groups,
    Terminal,
    Web,
    RocketLaunch,
    EditNote,
    ViewInAr,
    Rocket,
    Psychology,
    BugReport,
    Flutter,
    Terrain,
    Waves,
    AccountBalance,
    Lock,
    VolunteerActivism,
    Visibility,
    Hub,
    Extension,
    GridOn
}

#endregion