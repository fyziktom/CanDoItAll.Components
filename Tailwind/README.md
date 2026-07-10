# CanDoItAll.Components Tailwind

This workspace builds the shared Tailwind CSS shipped by `CanDoItAll.Components.BaseLib`. It is the styling source for the reusable Blazor UI library, not the place for product-specific page styling.

Install dependencies:

```powershell
npm install
```

Build once:

```powershell
npm run build
```

Watch mode:

```powershell
npm run watch
```

The output is written to `..\src\CanDoItAll.Components.BaseLib\wwwroot\css\output.css`.

Keep reusable component selectors, tokens, and patterns here. Keep application-only selectors in the consuming application's Tailwind workspace. After changing the shared styles, build them and verify the relevant Sandbox route so a component's markup and CSS remain aligned.
