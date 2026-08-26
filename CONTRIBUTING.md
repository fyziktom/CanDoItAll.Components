# Contributing

CanDoItAll.Components accepts code contributions only from partners who have
been explicitly approved by the maintainer. Unsolicited pull requests are not
accepted.

To discuss becoming an approved partner, contact the maintainer on LinkedIn
using the handle `fyziktom`. Please wait for approval before preparing or
opening a pull request.

## Development setup

Install the .NET SDK selected by `global.json` and a current Node.js LTS
release, then run:

```powershell
npm ci
npm ci --prefix Tailwind
npm run build:tailwind
dotnet restore CanDoItAll.Components.slnx --configfile NuGet.config
dotnet build CanDoItAll.Components.slnx --configuration Release --no-restore
dotnet test CanDoItAll.Components.slnx --configuration Release --no-build
```

Use `samples/CanDoItAll.Components.Sandbox` for component examples and stress
scenarios. Viewport-sensitive `BaseLib` changes should include keyboard checks
and Playwright screenshots at small, medium, and large widths. Other libraries
target large-screen application use by default; preserve their existing
responsive behavior unless cross-viewport work is explicitly in scope. Keep
application-specific behavior out of the reusable libraries.

## Pull requests

- Open a pull request only after partner approval.
- Keep changes focused and explain public API changes.
- Add or update unit tests and sandbox scenarios for behavior changes.
- Regenerate Tailwind output with `npm run build:tailwind`.
- Do not update approval snapshots unless the public API or package input change is intentional and described in the pull request.
- Do not add third-party images, fonts, scripts, or models without recording their source, version, and redistribution license.
