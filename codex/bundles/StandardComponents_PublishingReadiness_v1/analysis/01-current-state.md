# Current State

Generated from repository inspection on `2026-06-28T18:39:57.469Z`.

## Scope Boundary

- Included: standard component libraries in `src/CanDoItAll.Components.BaseLib`, `src/CanDoItAll.Components.Common`, `src/CanDoItAll.Components.Charts`, `src/CanDoItAll.Components.OverlayLib`, `src/CanDoItAll.Components.Mermaid`, `src/CanDoItAll.Components.Sandbox`, and `Tailwind`.
- Included for duplicate analysis only: `C:\repositories\CanDoItAll\src\CanDoItAll.AppComponents\Components` and `C:\repositories\CanDoItAll\src\CanDoItAll.AppComponents\Primitives`.
- Excluded from implementation scope: `src/CanDoItAll.Components.WebGlLib`, `src/CanDoItAll.Components.WebGlRunLib`, `src/CanDoItAll.Components.WebGlSandbox`, and `src/CanDoItAll.Components.CanvasLib`. Canvas entries in the standard sandbox are inventory evidence only and must be split out or isolated by a later subbundle.

## Quantitative Snapshot

- Standard inventory rows: 268.
- Standard non-sandbox Razor components: 173.
- Old AppComponents rows with a standard-name match: 39.
- Standard non-sandbox Razor components with no direct sandbox registry example: 134.
- Tailwind/CSS files with high refactor pressure: 9; medium pressure: 4.
- Non-WebGL/non-Canvas test projects found: 0.

## Main Findings

- `F01` High: Old AppComponents contains many basic primitives duplicated by BaseLib/Charts. Evidence: Button, Card, TextBox, DropDown, Tabs, Chart, DataGrid, ProgressBar and related primitive enums exist in both repositories. Owner: `SB04`.
- `F02` High: No non-WebGL/non-Canvas test project is present for standard components. Evidence: tests/ currently contains only CanDoItAll.Components.WebGlLib.Tests and CanDoItAll.Components.WebGlRunLib.Tests. Owner: `SB10`.
- `F03` High: Tailwind inputs include a mix of @apply and raw CSS layout/property declarations. Evidence: Tailwind/input.css, controls/buttons.css, navigation/tabs.css, foundation/theme.css and forms/tag-editor.css contain substantial raw declarations and token work. Owner: `SB02`.
- `F04` Medium: Base component inheritance and attribute merging are not universal. Evidence: StyledComponentBase centralizes Class, Style, AdditionalAttributes, but support models, compatibility shims, and some wrappers need a deliberate boundary. Owner: `SB03`.
- `F05` High: Sandbox is grouped, but coverage is sampled rather than one-to-one for every standard component. Evidence: SandboxCatalogRegistry defines groups and examples, but many BaseLib components have zero direct example references. Owner: `SB05`.
- `F06` Medium: Canvas is still grouped inside the main sandbox registry. Evidence: SandboxGroupKey includes Canvas and Examples include canvas preview entries, while this publishing bundle must exclude Canvas/WebGL work. Owner: `SB05`.
- `F07` Medium: Some old AppComponents behavior may be newer than BaseLib and must not be deleted blindly. Evidence: Old AppComponents Button has an isClickInFlight guard; BaseLib Button adds anchors and compatibility looks but lacks that guard in current inspection. Owner: `SB04`.
- `F08` High: UI publishing proof must be visual and interactive, not source-only. Evidence: Dropdowns, dialogs, tooltips, sticky action footers, tabs, and long-text/dense states require open-state screenshots and browser assertions. Owner: `SB11`.

## Implementation-Relevant Observations

- `StyledComponentBase` centralizes `Class`, `Style`, `AdditionalAttributes`, and helpers around `ComponentAttributeExtensions.WithClassAndStyle`, but the bundle must decide which components should inherit it and which support models/helpers belong in `Common` versus `BaseLib`.
- `Button` in BaseLib supports anchors, token text, material icon fallback, and compatibility looks. The old app copy has an in-flight click guard; SB04 must compare and port that behavior if still wanted.
- `DropDown` in BaseLib adds `InputLook` and accessible label handling over the old app copy, but still renders a native `select`; Playwright proof should cover long option text and form-field label association.
- `SandboxCatalogRegistry` has group-level proof questions and scenario state, but group examples are representative rather than exhaustive. Publishing readiness needs a visual matrix where each standard component has at least one direct example and every interactive overlay/dropdown/dialog state has open-state proof.
- The Tailwind input set already uses `@apply`, but also contains raw layout, sizing, color, shadow, and media declarations. SB02 must separate acceptable token/state CSS from declarations that should become Tailwind utility composition.
