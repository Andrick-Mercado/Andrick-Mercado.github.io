# Personal Portfolio

A personal portfolio and interactive resume website built with .NET 10 Blazor WebAssembly, MudBlazor, and a react-snap prerendering step, deployed automatically to GitHub Pages.

[![Deploy Web App to GitHub Pages](https://github.com/Andrick-Mercado/Andrick-Mercado.github.io/actions/workflows/deploy-blazor-wasm-to-github-pages.yml/badge.svg)](https://github.com/Andrick-Mercado/Andrick-Mercado.github.io/actions/workflows/deploy-blazor-wasm-to-github-pages.yml)
[![GitHub issues](https://img.shields.io/github/issues/Andrick-Mercado/Andrick-Mercado.github.io)](https://github.com/Andrick-Mercado/Andrick-Mercado.github.io/issues)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![MudBlazor](https://img.shields.io/badge/MudBlazor-9.7.0-594AE2)](https://mudblazor.com/)

Live site: [andrick-mercado.github.io](https://andrick-mercado.github.io/)

## Overview

This repository is the source for my personal portfolio site. It is a single-page Blazor WebAssembly application that renders resume and portfolio content from a JSON data file, styled with the MudBlazor component library, and prerendered to static HTML so that the published pages load fast and are crawlable by search engines and link previews.

Because the repository is named `Andrick-Mercado.github.io`, GitHub Pages serves it as a user site at the root domain rather than under a project subpath.

## Features

* **.NET 10 Blazor WebAssembly.** A client-side single-page application running entirely in the browser via WebAssembly, with no server-side runtime to host or pay for.
* **Responsive UI with MudBlazor.** Material-style layout and components that adapt from desktop down to mobile.
* **Dark mode with persistent state.** The selected theme is stored in the browser via [Blazored.LocalStorage](https://github.com/Blazored/LocalStorage) so it survives reloads and return visits.
* **Static prerendering with react-snap.** Published output is crawled and prerendered by [react-snap](https://github.com/stereobooster/react-snap) so visitors and crawlers get meaningful HTML before the WASM payload finishes downloading.
* **Data-driven content.** Resume and portfolio content lives in `wwwroot/database/websiteData.json`, loaded through a repository/service layer, so updating the site is mostly a matter of editing data rather than markup.
* **Layered class library.** Domain, application, and infrastructure concerns are separated into `PersonalPortfolio.Library` and wired up with a single dependency-injection extension method.
* **Zero-touch deployment.** Every push to `main` builds, prerenders, and publishes the site through GitHub Actions.

## Tech stack

| Area | Technology |
| --- | --- |
| Framework | .NET 10 / Blazor WebAssembly |
| Language | C# 14 |
| UI components | MudBlazor 9.7.0 |
| Client storage | Blazored.LocalStorage 4.5.0 |
| Prerendering | react-snap via `npx` (Puppeteer + Chrome stable) |
| CI/CD | GitHub Actions |
| Hosting | GitHub Pages |

## Project structure

```text
Andrick-Mercado.github.io/
├── PersonalPortfolio.sln
├── AGENTS.md                      # Project context for AI coding agents
├── README.md
├── .github/
│   └── workflows/
│       └── deploy-blazor-wasm-to-github-pages.yml
├── prerender/
│   └── package.json               # react-snap configuration
├── tools/
│   └── media/                     # Images and media assets used by the site
└── src/
    ├── PersonalPortfolio.Blazor/  # Blazor WASM host application
    │   ├── App.razor
    │   ├── Program.cs
    │   ├── Pages/
    │   ├── wwwroot/
    │   │   └── database/
    │   │       └── websiteData.json
    │   └── PersonalPortfolio.Blazor.csproj
    └── PersonalPortfolio.Library/ # Shared Razor class library
        ├── Application/
        ├── Domain/
        ├── Infrastructure/
        ├── DependencyInjection.cs
        └── PersonalPortfolio.Library.csproj
```

### Architecture notes

`PersonalPortfolio.Blazor` is the WebAssembly entry point. `Program.cs` registers the root `App` component and a `HeadOutlet`, adds a scoped `HttpClient` bound to the host base address, registers MudBlazor services, and then calls the library's `AddBlazorLibraryAsync` extension.

`PersonalPortfolio.Library` is a Razor class library split into `Domain` (models), `Application` (service abstractions such as `IProfileService`), and `Infrastructure` (data access such as `WebDatabaseService` and `WebsiteRepo`). `AddBlazorLibraryAsync` eagerly initializes the website repository over HTTP, registers it as a singleton alongside `IProfileService`, and adds Blazored.LocalStorage as a singleton. Initializing the repository during startup means content is available before the first component renders, which is what makes the prerendered output meaningful.

## Getting started

### Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/download) (10.0.x)
* The `wasm-tools` workload, required for release builds and AOT-related tooling
* [Node.js](https://nodejs.org/) and npm, only if you want to run the prerender step locally
* A recent Chrome or Chromium install, only if you want to run the prerender step locally

Install the WebAssembly workload once per machine:

```bash
dotnet workload install wasm-tools
```

### Clone and run

```bash
git clone https://github.com/Andrick-Mercado/Andrick-Mercado.github.io.git
cd Andrick-Mercado.github.io
dotnet restore
dotnet run --project src/PersonalPortfolio.Blazor/PersonalPortfolio.Blazor.csproj
```

The console prints the local URL, typically `https://localhost:5001` or a similar port. Open it in a browser.

### Build a release bundle

```bash
dotnet publish src/PersonalPortfolio.Blazor/PersonalPortfolio.Blazor.csproj -c Release -o prerender/output --nologo
```

Static output lands in `prerender/output/wwwroot`. This is the exact command CI uses, so running it locally is the fastest way to reproduce a build failure.

### Run the prerender step locally

After publishing to `prerender/output`, run react-snap from the `prerender` directory:

```bash
cd prerender
npx react-snap
```

If Puppeteer picks a bundled Chromium that is too old for the Blazor runtime, point it at a local Chrome install:

```bash
PUPPETEER_EXECUTABLE_PATH="/path/to/chrome" npx react-snap
```

## Updating site content

Most content changes do not require touching Razor markup:

1. Edit `src/PersonalPortfolio.Blazor/wwwroot/database/websiteData.json`.
2. Add or replace any accompanying images under `tools/media/` and the app's `wwwroot` assets.
3. Run the site locally to confirm the new content renders correctly.
4. Commit and push to `main`; the deployment workflow handles the rest.

The `websiteData.json` file is marked `CopyToOutputDirectory=PreserveNewest`, so edits are picked up on the next build without a clean.

## Deployment

Deployment is fully automated by `.github/workflows/deploy-blazor-wasm-to-github-pages.yml`, which runs on every push to `main`:

1. Check out the repository.
2. Set up the .NET 10.0.x SDK and install the `wasm-tools` workload.
3. Publish the Blazor project in `Release` to `prerender/output`.
4. Install Chrome stable and expose it to Puppeteer through `PUPPETEER_EXECUTABLE_PATH`, avoiding stale bundled Chromium.
5. Prerender the published client with `npx react-snap`.
6. Add a `.nojekyll` file so GitHub Pages serves the underscore-prefixed `_framework` assets that Blazor requires.
7. Publish `prerender/output/wwwroot` to the `deployment-branch` using [`JamesIves/github-pages-deploy-action@v4`](https://github.com/JamesIves/github-pages-deploy-action).

GitHub Pages for this repository should be configured to serve from the `deployment-branch` branch at the root path.

### A note on the base tag

The workflow contains commented-out steps that rewrite the `<base href>` to a repository subfolder. Those steps are intentionally disabled: as a user site served from the root domain, the default `<base href="/">` is correct. Re-enable them only if this code is ever deployed as a project site under a subpath such as `/PortfolioV2/`.

## Troubleshooting

* **Blank page after deploy, 404s on `_framework` files.** The `.nojekyll` file is missing from the published output, or Pages is pointed at the wrong branch.
* **Prerender step hangs or crashes.** Usually a Chrome/Puppeteer mismatch. Set `PUPPETEER_EXECUTABLE_PATH` to a modern Chrome binary.
* **Assets resolve to the wrong path.** Check that `<base href>` matches how the site is served, root for a user site, `/<repo>/` for a project site.
* **`wasm-tools` workload errors on publish.** Run `dotnet workload install wasm-tools` and confirm the installed SDK is 10.0.x with `dotnet --info`.

## AI agent context

`AGENTS.md` at the repository root captures the project summary, structure, key dependencies, and build and deploy commands in a form intended for AI coding assistants. Keep it in sync when the stack, layout, or commands change.

## License

Copyright © Andrick Mercado. All rights reserved.

This is a personal project published for reference and portfolio purposes. It is not released under an open-source license, and the site content, resume text, and media assets are not licensed for reuse. Feel free to read the code for inspiration or open an issue with questions.
