# Runtime Constraint Update

Captured local date: `2026-06-29`

Raw user update:

```text
we need to avoid to use npm or being dependent on it instead of use of tailwind or some specific testing where it can help us. Main canvas and floating windows, calendar and things like that, implementation must be in pure JS.

as senior C# and JS developer. execute bundle and implement all and validate all.
```

Interpretation:

- Main Canvas, floating-window, calendar, preview, and related interactive runtime implementation must remain plain browser JavaScript plus C# and Razor.
- Do not introduce npm runtime dependencies for these surfaces.
- Existing npm/Node usage is allowed only for repository tooling such as Tailwind, generated asset verification/build scripts, tests, and browser-validation tooling where it helps validation.
- Any new runtime dependency proposal must be rejected or explicitly converted to pure JavaScript before closure.

