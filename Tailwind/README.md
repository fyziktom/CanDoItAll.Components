# CanDoItAll.Components Tailwind

This workspace builds the shared component stylesheet shipped by `CanDoItAll.Components.BaseLib`.

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

The main CanDoItAll repo has its own Tailwind workspace for app-specific styles. Keep reusable component CSS here; keep main-only selectors in `C:\repositories\CanDoItAll\Tailwind`.
