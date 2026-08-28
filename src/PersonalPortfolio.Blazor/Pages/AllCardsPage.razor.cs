using Microsoft.AspNetCore.Components;

namespace PersonalPortfolio.Blazor.Pages;

public partial class AllCardsPage
{
    [Parameter]
    public string ClientRouteName { get; set; } = default!;
}