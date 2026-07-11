# CanDoItAll.Components.FileBrowser.Sandbox

| Setting | Value |
|---------|-------|
| **Interactivity Mode** | Server |
| **Interactivity Scope** | Global |

## Rendering configuration

This project uses global Interactive Server with prerendering. It was created with `dotnet new blazor -int Server -ai`.

All pages are interactive by default through `<Routes @rendermode="InteractiveServer" />` in `App.razor`.

## Adding new components

- Create routable pages in `Components/Pages/` and shared components in `Components/`.
- All pages are already interactive. Do not add a second `@rendermode` to individual pages.

## Data access

- Components may inject server-side providers directly.
- The sandbox must exercise production file-browser contracts; do not seed `FileBrowserSnapshot` instances by hand.
- Filesystem providers must remain confined to their configured sandbox roots.

## Environment constraints

- Components run on the server through SignalR.
- `HttpContext` is unavailable during the interactive circuit.
- Browser APIs require an existing BaseLib abstraction or explicit `IJSRuntime` interop.
- Every connected user holds a SignalR circuit on the server.

## Don'ts

- Do not add `@rendermode InteractiveServer` to pages.
- Do not inject `HttpContext` into interactive components.
- Do not call IPFS or other external services from the mock providers.
- Do not make the reusable file browser own the host `ThemeHost`, app shell, or overlay hosts.

