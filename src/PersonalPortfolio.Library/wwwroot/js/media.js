// Small interop helpers for MediaFrame and DeferredEmbed. Deliberately dependency-free:
// the app is Blazor WebAssembly and none of this warrants a JS framework.

const observers = new Map();
let nextHandle = 1;

// An element that happens to sit just below the fold is "in view" the instant the page
// renders, which would defeat the whole point of deferring it. Nothing heavy is allowed to
// start until the reader has actually done something - or, for clips only, until the page
// has finished loading and there is spare bandwidth.
let isEngaged = false;
const engagementCallbacks = [];
const engagementEvents = ['scroll', 'wheel', 'touchstart', 'pointerdown', 'keydown'];

function onEngaged() {
    if (isEngaged) return;
    isEngaged = true;

    for (const event of engagementEvents) {
        window.removeEventListener(event, onEngaged);
    }

    while (engagementCallbacks.length) {
        engagementCallbacks.shift()();
    }
}

if (typeof window !== 'undefined') {
    for (const event of engagementEvents) {
        window.addEventListener(event, onEngaged, { passive: true });
    }
}

function whenEngaged(callback) {
    if (isEngaged) {
        callback();
        return;
    }
    engagementCallbacks.push(callback);
}

/**
 * Clips are cheap enough to start on their own, but only once the page has stopped
 * competing for bandwidth. Whichever comes first wins: the reader touching the page, or
 * the load event settling.
 */
function whenIdle(callback) {
    let done = false;
    const run = () => {
        if (done) return;
        done = true;
        callback();
    };

    whenEngaged(run);

    if (document.readyState === 'complete') {
        setTimeout(run, 300);
    } else {
        window.addEventListener('load', () => setTimeout(run, 300), { once: true });
    }
}

/**
 * react-snap drives the real app through Puppeteer and saves whatever DOM it finds. Its
 * user agent is the only reliable signal that we are being prerendered, and we use it to
 * keep deferred iframes out of the snapshot - a half-initialised third-party embed baked
 * into the static HTML is worse than no embed at all.
 */
export function isPrerender() {
    return typeof navigator !== 'undefined' && navigator.userAgent.indexOf('ReactSnap') >= 0;
}

function register(observer, element, start) {
    const handle = nextHandle++;
    observers.set(handle, observer);
    start(() => {
        // The element may already have been torn down while we waited.
        if (observers.has(handle)) observer.observe(element);
    });
    return handle;
}

/**
 * Invokes the .NET callback the first time the element comes near the viewport, then stops
 * observing. Used to mount heavy iframes only when the reader actually scrolls to them.
 */
export function observeOnce(element, dotNetRef) {
    if (!element || typeof IntersectionObserver === 'undefined') {
        // No observer support: fail open so the content is still reachable.
        dotNetRef.invokeMethodAsync('OnVisibleAsync');
        return 0;
    }

    let fired = false;
    const observer = new IntersectionObserver(
        (entries) => {
            for (const entry of entries) {
                if (!entry.isIntersecting || fired) continue;
                fired = true;
                observer.disconnect();
                dotNetRef.invokeMethodAsync('OnVisibleAsync');
            }
        },
        { rootMargin: '200px' }
    );

    return register(observer, element, whenEngaged);
}

/**
 * Starts a muted loop while the clip is on screen and pauses it as soon as it leaves, so a
 * page with six demo videos never downloads or decodes more than the one being read.
 * The video is declared preload="none", which means nothing is fetched until this fires.
 */
export function autoplayInView(video, dotNetRef) {
    if (!video) return 0;

    // Autoplay policies only allow muted playback, and the property has to be set (the
    // attribute alone is not always honoured after client-side rendering).
    video.muted = true;
    video.defaultMuted = true;

    if (dotNetRef) {
        video.addEventListener(
            'loadeddata',
            () => dotNetRef.invokeMethodAsync('OnMediaLoadedAsync'),
            { once: true }
        );
    }

    if (typeof IntersectionObserver === 'undefined') {
        video.preload = 'metadata';
        return 0;
    }

    const observer = new IntersectionObserver(
        (entries) => {
            for (const entry of entries) {
                if (entry.isIntersecting) {
                    // play() rejects when the tab is hidden or the source 404s; neither is
                    // worth surfacing, the poster simply stays put.
                    const played = video.play();
                    if (played && typeof played.catch === 'function') played.catch(() => {});
                } else if (!video.paused) {
                    video.pause();
                }
            }
        },
        { threshold: 0.25 }
    );

    return register(observer, video, whenIdle);
}

/**
 * Reports whether an image already finished loading. Covers the case where the browser
 * served it from cache before Blazor attached its onload handler, which would otherwise
 * leave the skeleton in the DOM forever.
 */
export function isImageComplete(image) {
    return !!image && image.complete && image.naturalWidth > 0;
}

export function dispose(handle) {
    const observer = observers.get(handle);
    if (!observer) return;
    observer.disconnect();
    observers.delete(handle);
}
