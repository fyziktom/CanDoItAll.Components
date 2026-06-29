# Target Solution

## Target Publishing Shape

- `CanDoItAll.Components.Common`: small non-UI helper and primitive contracts that can be shared without pulling Razor dependencies.
- `CanDoItAll.Components.BaseLib`: core standard Razor components, shared base classes, compatibility shims with clear migration policy, and standard CSS classes generated from Tailwind input.
- `CanDoItAll.Components.Charts`, `CanDoItAll.Components.Mermaid`, and `CanDoItAll.Components.OverlayLib`: optional standard packages with clear service/assets registration and visual proof routes.
- `CanDoItAll.Components.Sandbox`: a standard-component proof harness with logical component-framework groups, separated from Canvas/WebGL proof surfaces.
- `CanDoItAll.AppComponents`: complex app-level surfaces only; no duplicated basic Button/Card/Input/Navigation/DataDisplay primitives unless temporarily aliased for migration.

## Styling Policy

- Prefer Tailwind `@apply` for simple layout, spacing, typography, border, sizing, and flex/grid composition inside Tailwind input files.
- Keep raw CSS where it expresses design tokens, CSS variables, pseudo-elements, Radzen/third-party interop, browser features such as `color-mix`, media queries that encode component-specific behavior, or state selectors that would be less maintainable as repeated utilities.
- Remove ad-hoc one-off component styling from Razor pages when it can be expressed through shared component parameters, shared classes, or Tailwind component-layer classes.
- Every styling refactor must be validated with Playwright screenshots, including long text and constrained width.

## Sandbox Taxonomy Target

- Foundations: typography, icons, theme, tokens.
- Inputs: text, numeric, selection, boolean, editable, upload, secret.
- Actions: buttons, copy actions, inline actions.
- Navigation: tabs, steps, menus, toolbars, tree/list navigation.
- Feedback: alerts, notifications, tooltips, help, loading, empty, status.
- Layout: stack, grid, row/column, split, page shells, sticky footers.
- Data Display: cards, lists, badges/chips, stats, timelines, tables.
- Visualization: charts and Mermaid diagrams.
- Overlays: dialogs, popovers, contextual menus, overlay windows where standard.
- Excluded: Canvas/WebGL routes and proof are separate bundle scopes.

## Checkpoint Strategy

- Checkpoint A after SB02 and SB03: shared style/base foundations are stable enough for component work.
- Checkpoint B after SB04 and SB05: duplicate migration and sandbox proof harness are trustworthy.
- Checkpoint C after SB06 through SB09: component groups have visual proof and source/test changes are coherent.
- Checkpoint D after SB10 and SB11: packaging/API and full visual matrix are ready for final transfer.
