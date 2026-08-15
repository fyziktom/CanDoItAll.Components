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

## Converting plain CSS to `@apply`

`npm run apply -- <path>` rewrites plain CSS declarations into Tailwind `@apply` utilities in place, using [`css-to-tailwindcss4`](https://www.npmjs.com/package/css-to-tailwindcss4). It walks directories recursively (skipping `node_modules`), and defaults to `sandbox/` if no path is given:

```powershell
npm run apply -- forms
npm run apply -- .
```

`npm run format` re-indents the result to 2 spaces (per `.editorconfig`) and restores the blank line between rules that the converter strips, via `prettier --write` followed by `stylelint --fix`.

### Always review the diff manually before committing

`css-to-tailwindcss4` is an early-stage package (v0.0.x) and its output is not safe to commit unreviewed. Issues found so far:

- **Not idempotent on partially-converted files.** If a file keeps a raw declaration the converter can't handle, rerunning `apply` on that same file reprocesses the raw declaration and can reintroduce the same broken class. Don't rerun `apply` on a file it has already touched without checking the diff again.
- **Can collapse multiple `@media` blocks into one unreadable line.** On `foundation/radzen-layout.css` it merged five separate breakpoint rules into a single ~900-character `@apply` line using arbitrary-value responsive variants (`sm:col-[...] md:col-[...] ...`). Technically valid and it builds, but unreadable — rewrite conversions like this back to plain `@media` CSS by hand.
- **Low value on files that already use `@apply` well.** Running it on already-converted files (e.g. `forms/*.css`) made no real conversion, just indentation/blank-line churn.

Recommended workflow: run `apply` one folder at a time, `npm run build` immediately after to catch invalid classes, then `npm run format`, then read the full diff before committing.
