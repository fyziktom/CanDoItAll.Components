# CanDoItAll.Components.FileBrowser.Sandbox

Dedicated interactive proof host for the provider-neutral CanDoItAll file browser.

The sandbox composes realistic project/subproject and shallow IPFS-like contract mocks, a root-confined local filesystem provider, and empty/retryable-error scenarios. It demonstrates full-page source navigation, a compact floating-window-sized composition, action dispatch, exact retry without rebuilding the session, light/dark themes, and provider-call telemetry.

The IPFS source is intentionally a mock of the required shallow/page-aware contract. It is not an adapter over the legacy recursive IPFS listing API; production integration remains deferred until that API boundary is available.

Run it from the repository root:

```powershell
dotnet run --project samples/CanDoItAll.Components.FileBrowser.Sandbox/CanDoItAll.Components.FileBrowser.Sandbox.csproj
```

Browser artifacts belong under `output/playwright/`.
