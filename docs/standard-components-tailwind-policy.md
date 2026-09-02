# Standard Components Tailwind Policy

This repository keeps standard component styling in the shared Tailwind input layer whenever the rule describes reusable layout, sizing, spacing, typography, or responsive behavior.

## Use Tailwind Composition

- Use `@apply` in `Tailwind/**` for common component structure: `flex`, `grid`, `min-w-0`, `w-full`, wrapping, alignment, gaps, padding, font sizing, leading, transitions, and responsive variants.
- Prefer Tailwind arbitrary values when a reusable component needs a precise non-token value, for example `gap-[0.35rem]`, instead of raw CSS declarations for simple layout.
- Keep component markup classes small and semantic. Use markup classes for component-local responsive behavior only when the behavior depends on component composition rather than a global selector.
- Rebuild `src/CanDoItAll.Components.BaseLib/wwwroot/css/output.css` after every Tailwind input change.
- Commit that distributed BaseLib output and review its generated diff with the owning input
  changes. CI rejects missing tracked output or regeneration drift; clean source consumers
  must not depend on local npm state. Sandbox/test outputs remain ignored.

## Keep Raw CSS With Rationale

Raw CSS is allowed when Tailwind utilities do not represent the behavior clearly or safely:

- Theme tokens, CSS custom properties, and semantic color variables.
- Browser state selectors and generated content such as `::before`, `::after`, `:focus-visible`, and disabled/ARIA state selectors when they depend on variables.
- `color-mix()`, gradients, shadows, and token-driven borders/backgrounds that must stay themeable.
- Component geometry expressed as custom properties because it is part of the component API, such as tabs panel radii or prefixed-field padding variables.
- Browser integration rules such as `scrollbar-width`, `scrollbar-gutter`, `color-scheme`, and scoped third-party overrides.

## Visual Gate

Styling changes are not complete from source inspection alone. Each affected standard component group needs screenshot proof at desktop and narrow mobile sizes, plus interaction proof for controls such as tabs, dropdowns, copy buttons, and menus.
