# Normalized Requirements

## Requirements

| ID | Requirement | Bundle Destination | Owning Subbundle | Observable Acceptance |
| --- | --- | --- | --- | --- |
| R01 | Preserve publishing-prep scope and exclude WebGL/Canvas implementation. | README, inputs, plan | SB01 | Scope files and validators cite excluded paths. |
| R02 | Map all standard component, sandbox, style, and AppComponents duplicate surfaces in an xlsx. | inventories/standard-components-publishing-map.xlsx | SB01 | Workbook render and data inspection pass. |
| R03 | Define Tailwind styling policy before component-by-component visual changes. | architecture/01-target-solution.md, SB02 | SB02 | CSS metrics, refactor rules, build proof, screenshots. |
| R04 | Isolate shared bases/helpers/primitives for easier maintenance. | SB03 | SB03 | Contract tests and source assertions for helpers/base inheritance. |
| R05 | Audit and reduce old AppComponents basic primitives. | SB04 | SB04 | Duplicate matrix, migration tests, old/new behavior comparison. |
| R06 | Split and improve sandbox groups to match component-framework best practice. | SB05 | SB05 | Sandbox route matrix, coverage counts, Playwright proof setup. |
| R07 | Visually harden forms and inputs one by one. | SB06 | SB06 | Desktop/mobile screenshots, long text, disabled, dense, dropdown action proof. |
| R08 | Visually harden actions, badges, and feedback components. | SB07 | SB07 | Icon/text wrapping, loading, empty, notification proof. |
| R09 | Visually harden layout, navigation, and overlay components. | SB08 | SB08 | Available-space, clipping, open overlay, keyboard/focus proof. |
| R10 | Visually harden data-display, chart, and diagram components. | SB09 | SB09 | Dense labels, empty states, chart/diagram nonblank rendering proof. |
| R11 | Add publishing/packaging/API hardening for standard libraries. | SB10 | SB10 | Pack/build/API approvals, compatibility policy, non-WebGL tests. |
| R12 | Run a full Playwright visual validation matrix. | SB11 | SB11 | Browser analytics rows with screenshot paths and assertions. |
| R13 | Finish transfer readiness with raw-note closure and red-team proof. | SB12 | SB12 | Final validator, closure table, follow-up scope for Canvas/WebGL. |

## Hard Constraints

- Do not implement WebGL or Canvas refactors in this bundle.
- Do not remove old AppComponents primitives until behavior comparison and consumer migration proof exist.
- Do not accept CSS/styling changes without Playwright MCP screenshots and explicit visual review answers.
- Do not call the bundle ready for execution unless the xlsx, source inventory, dependency map, subbundle gates, and self-review all agree.
