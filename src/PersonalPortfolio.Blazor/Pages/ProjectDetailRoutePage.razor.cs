using Microsoft.AspNetCore.Components;

namespace PersonalPortfolio.Blazor.Pages;

public partial class ProjectDetailRoutePage
{
    [Parameter]
    public string PageEndpoint { get; set; } = default!;

    [Parameter]
    public string Slug { get; set; } = default!;
}
