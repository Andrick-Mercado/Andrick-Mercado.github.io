using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace PersonalPortfolio.Library.Application.Components;

/// <summary>
/// Holds a third-party iframe back until the reader can actually see it. The Google Drive
/// resume viewer and the YouTube, Milanote and itch.io embeds each pull megabytes of
/// scripts, so mounting them during first paint dominates the page load for content most
/// visitors never scroll to.
/// </summary>
public partial class DeferredEmbed : IAsyncDisposable
{
    private const string ModulePath = "./_content/PersonalPortfolio.Library/js/media.js";

    private ElementReference _container;
    private IJSObjectReference _module;
    private DotNetObjectReference<DeferredEmbed> _selfReference;
    private int _observerHandle;
    private bool _isMounted;
    private bool _isObserving;

    [Parameter, EditorRequired] public string Src { get; set; } = default!;
    [Parameter] public string Title { get; set; }
    [Parameter] public string Description { get; set; }
    [Parameter] public string Class { get; set; }

    /// <summary>CSS height for the reserved box, e.g. <c>80vh</c>. Keeps the layout stable.</summary>
    [Parameter] public string Height { get; set; } = "60vh";

    [Parameter] public string ActionText { get; set; } = "Load content";
    [Parameter] public string ActionIcon { get; set; } = Icons.Material.Filled.PlayArrow;
    [Parameter] public string Icon { get; set; } = Icons.Material.Filled.Public;
    [Parameter] public string Allow { get; set; } = "autoplay; encrypted-media; picture-in-picture";
    [Parameter] public string DownloadUrl { get; set; }
    [Parameter] public string OpenUrl { get; set; }

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    private string BoxStyle => $"height:{Height};";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
            _selfReference = DotNetObjectReference.Create(this);
        }

        if (_module is null || _isMounted || _isObserving)
            return;

        // react-snap runs the real app in Puppeteer and saves the resulting DOM. Auto-mounting
        // here would bake a third-party iframe into the static snapshot, so during prerender
        // the placeholder is all that gets captured and the live app takes over from there.
        if (await _module.InvokeAsync<bool>("isPrerender"))
            return;

        _isObserving = true;
        _observerHandle = await _module.InvokeAsync<int>("observeOnce", _container, _selfReference);
    }

    [JSInvokable]
    public Task OnVisibleAsync()
    {
        Mount();
        return Task.CompletedTask;
    }

    private void Mount()
    {
        if (_isMounted)
            return;

        _isMounted = true;
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_module is not null)
            {
                if (_observerHandle != 0)
                    await _module.InvokeVoidAsync("dispose", _observerHandle);

                await _module.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
            // The page is already tearing down; the observer goes with it.
        }

        _selfReference?.Dispose();
    }
}
