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
├── .github/
│   └── workflows/
│       └── deploy-blazor-wasm-to-github-pages.yml
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

## Build / run notes

Typical local run command:

```shell script
dotnet run --project src/PersonalPortfolio.Blazor/PersonalPortfolio.Blazor.csproj
```


Publish command used by CI:

```shell script
dotnet publish src/PersonalPortfolio.Blazor/PersonalPortfolio.Blazor.csproj -c Release -o prerender/output --nologo
```


## Deployment

GitHub Actions deploys from `main` to GitHub Pages using:

- `.NET 10.0.x`
- `wasm-tools`
- Blazor publish output
- `react-snap` prerendering
- `JamesIves/github-pages-deploy-action@v4`
- Deployment branch: `deployment-branch`