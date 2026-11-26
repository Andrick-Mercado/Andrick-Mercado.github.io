Initialized project context.

## Project summary

- **Project:** Personal Portfolio
- **Stack:** ASP.NET / Blazor WebAssembly
- **Target framework:** `.NET 10.0`
- **Language:** C# 14
- **Solution:** `PersonalPortfolio.sln`
- **Main app project:** `src/PersonalPortfolio.Blazor`
- **Shared/library project:** `src/PersonalPortfolio.Library`
- **Package manager for Node tooling:** `npm`

## Structure

```plain text
PortfolioV2/
├── PersonalPortfolio.sln
├── README.md
├── global.json                    # Pins .NET 10 SDK (rollForward: latestFeature)
├── .github/
│   └── workflows/
│       ├── ci.yml                    # PR CI: build + publish
│       └── deploy.yml                # Deploy on push to main
├── prerender/
│   └── package.json
└── src/
    ├── PersonalPortfolio.Blazor/
    │   ├── App.razor
    │   ├── Program.cs
    │   ├── Pages/
    │   ├── wwwroot/
    │   └── PersonalPortfolio.Blazor.csproj
    └── PersonalPortfolio.Library/
        ├── Application/
        ├── Domain/
        ├── Infrastructure/
        ├── DependencyInjection.cs
        └── PersonalPortfolio.Library.csproj
```


## Key dependencies

- `Microsoft.AspNetCore.Components.WebAssembly`
- `Microsoft.AspNetCore.Components.WebAssembly.DevServer`
- `Microsoft.AspNetCore.Components.Web`
- `MudBlazor`
- `Blazored.LocalStorage`
- `react-snap` via `npx` for prerendering

## SEO notes

- `wwwroot/robots.txt` and `wwwroot/sitemap.xml` list all crawlable routes.
- `index.html` holds site-wide meta tags and JSON-LD (`Person` + `WebSite`).
- Each page component sets its own title, meta description, canonical link, and Open Graph/Twitter tags via `HeadContent`; react-snap bakes them into the prerendered HTML.
- Shared URL/description helpers: `PersonalPortfolio.Library/Domain/SeoConstants.cs` and `MiExtensions.ToMetaDescription`.
- The `reactSnap.include` array in `prerender/package.json` must stay in sync with `sitemap.xml` whenever `websiteData.json` adds or removes an `otherPages` endpoint or a card `slug`.

## Build / run notes

Typical local run command:

```shell script
dotnet run --project src/PersonalPortfolio.Blazor/PersonalPortfolio.Blazor.csproj
```


Publish command used by CI:

```shell script
dotnet publish src/PersonalPortfolio.Blazor/PersonalPortfolio.Blazor.csproj -c Release -o prerender/output --nologo
```


## CI/CD

Two workflows:

- **`.github/workflows/ci.yml`** — Runs on pull requests to `main`. Builds and publishes the Blazor project. Provides fast feedback without prerendering or deploying.
- **`.github/workflows/deploy.yml`** — Runs on push to `main`. Builds, publishes, prerenders with react-snap, and deploys to the `deployment-branch`.

Both use:
- `.NET 10.0.x` (pinned via `global.json` with `rollForward: latestFeature`)
- Workloads restored automatically with `dotnet workload restore`
- NuGet caching

When upgrading to a new major .NET version, update `dotnet-version` in both workflows and the `version` field in `global.json`. Workload restore handles the rest.
