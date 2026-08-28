using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PersonalPortfolio.Library.Application.Components;

/// <summary>
/// A fixed-size media box that reserves its layout before anything downloads.
/// Images are lazy by default; clips are declared <c>preload="none"</c> and only start
/// streaming once an IntersectionObserver reports them on screen, which keeps a project
/// page with half a dozen demos from fetching all of them at once.
/// </summary>
public partial class MediaFrame : IAsyncDisposable
{
    private const string ModulePath = "./_content/PersonalPortfolio.Library/js/media.js";

    private ElementReference _imageElement;
    private ElementReference _videoElement;
    private IJSObjectReference _module;
    private DotNetObjectReference<MediaFrame> _selfReference;
    private int _observerHandle;
    private bool _isLoaded;
    private string _webmSrc;
    private string _lastMediaKey;

    /// <summary>Image source. Ignored when <see cref="VideoSrc"/> is set.</summary>
    [Parameter] public string Src { get; set; }

    /// <summary>Path to the MP4; the WebM sibling is derived from it.</summary>
    [Parameter] public string VideoSrc { get; set; }

    [Parameter] public string PosterSrc { get; set; }
    [Parameter] public string Alt { get; set; }
    [Parameter] public string Class { get; set; }

    /// <summary>Opts the element out of lazy loading. Use only for the LCP image.</summary>
    [Parameter] public bool Eager { get; set; }

    [Parameter] public double AspectRatio { get; set; } = 16d / 9d;
    [Parameter] public int? Height { get; set; }
    [Parameter] public int? MaxHeight { get; set; }

    /// <summary>Defaults to <c>cover</c> for images and <c>contain</c> for clips, whose
    /// source aspect ratios vary too much to crop safely.</summary>
    [Parameter] public string ObjectFit { get; set; }

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    private bool IsVideo => string.IsNullOrEmpty(VideoSrc) is false;
    private string LoadingMode => Eager ? "eager" : "lazy";
    private string FetchPriority => Eager ? "high" : "auto";
    private string MediaStyle => $"object-fit:{ObjectFit ?? (IsVideo ? "contain" : "cover")};";

    private string FrameStyle
    {
        get
        {
            var style = Height.HasValue
                ? $"height:{Height.Value}px;"
                : $"aspect-ratio:{AspectRatio.ToString("0.####", CultureInfo.InvariantCulture)};";

            if (MaxHeight.HasValue)
                style += $"max-height:{MaxHeight.Value}px;";

            return style;
        }
    }

    protected override void OnParametersSet()
    {
        _webmSrc = IsVideo && VideoSrc.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)
            ? string.Concat(VideoSrc.AsSpan(0, VideoSrc.Length - 4), ".webm")
            : null;

        // Reusing the component for a different asset has to bring the skeleton back.
        var mediaKey = IsVideo ? VideoSrc : Src;
        if (_lastMediaKey is not null && _lastMediaKey != mediaKey)
            _isLoaded = false;

        _lastMediaKey = mediaKey;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
            _selfReference = DotNetObjectReference.Create(this);
        }

        if (_module is null)
            return;

        if (IsVideo)
        {
            if (_observerHandle != 0)
                return;

            // Prerendering must not pull megabytes of video into the react-snap run.
            if (await _module.InvokeAsync<bool>("isPrerender"))
                return;

            _observerHandle = await _module.InvokeAsync<int>("autoplayInView", _videoElement, _selfReference);
            return;
        }

        if (_isLoaded || string.IsNullOrEmpty(Src))
            return;

        // A cached image can finish decoding before Blazor wires up @onload, in which case
        // the event never arrives and the skeleton would otherwise linger in the DOM.
        if (await _module.InvokeAsync<bool>("isImageComplete", _imageElement))
            MarkLoaded();
    }

    [JSInvokable]
    public Task OnMediaLoadedAsync()
    {
        MarkLoaded();
        return Task.CompletedTask;
    }

    private void MarkLoaded()
    {
        if (_isLoaded)
            return;

        _isLoaded = true;
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
            // The circuit or page is already gone; nothing left to clean up.
        }

        _selfReference?.Dispose();
    }
}
