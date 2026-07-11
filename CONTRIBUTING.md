# Contributing

Thank you for helping improve CanDoItAll.Components.

## Development setup

Install the .NET 10 SDK and a current Node.js LTS release, then run:

```powershell
npm ci
npm ci --prefix Tailwind
npm run tailwind:build
dotnet restore CanDoItAll.Components.slnx
dotnet build CanDoItAll.Components.slnx --configuration Release --no-restore
dotnet test CanDoItAll.Components.slnx --configuration Release --no-build
```

Use `samples/CanDoItAll.Components.Sandbox` for component examples and stress scenarios. UI changes should include keyboard checks and Playwright screenshots at desktop, tablet, and mobile widths. Keep application-specific behavior out of the reusable libraries.

## Pull requests

- Keep changes focused and explain public API changes.
- Add or update unit tests and sandbox scenarios for behavior changes.
- Regenerate Tailwind output with `npm run tailwind:build`.
- Do not update approval snapshots unless the public API or package input change is intentional and described in the pull request.
- Do not add third-party images, fonts, scripts, or models without recording their source, version, and redistribution license.
