# Structured Input

## Raw Notes

| ID | Exact Input Wording | Normalized Meaning |
|---|---|---|
| RAW01 | "Wee did preparation for publishing in part of basic components." | Reuse the successful basic/standard-components publishing readiness workflow instead of inventing a new bundle shape. |
| RAW02 | "study system of how we improved it (look at last bundles)" | Inspect recent bundles and preserve their sequencing: inventory, foundations, checkpoints, proof, visual matrix, publishing closure. |
| RAW03 | "prepare new bundle that will focus on preparation/refactor/improvement/hardening and true validation of canvas and floating windows parts." | Create an implementation-ready initiative bundle for CanvasLib and floating-window surfaces, including maintainability, refactor, hardening, documentation, and proof. |
| RAW04 | "do not do webgl part yet." | Exclude WebGL source, runtime, package, and docs changes from this bundle. WebGL may appear only as proof-pattern precedent or an explicit non-scope boundary. |
| RAW05 | "We must preserve all functionality." | Require compatibility-preserving refactors, failing-first proof for behavior changes, and package/API approval gates before closure. |
| RAW06 | "make it more maintainable, clear and well documented for soon publishing as opensource." | Include docs, package metadata, API approval, generated-asset policy, source maps, and open-source transfer readiness. |
| RAW07 | "avoid to use npm or being dependent on it... Main canvas and floating windows, calendar and things like that, implementation must be in pure JS." | Keep runtime implementation plain browser JavaScript plus C# and Razor. Do not add npm runtime dependencies; npm is allowed only for existing Tailwind, generated asset, test, or browser tooling. |

## Derived Scope

- CanvasLib includes workbench, graph primitives/overlays, interaction services, JS runtime modules, CSS, calendar, preview components, asset generation, package metadata, and sandbox samples.
- Floating windows include OverlayLib `OverlayWindow`, OverlayLib runtime/CSS/state, CanvasLib `CanvasFloatingWindow`, `CanvasWorkbenchWindowState`, and sandbox open-state proof for both overlay and canvas contexts.
- Standard component work already completed in `StandardComponents_PublishingReadiness_v1` should not be reopened except where Canvas or floating windows directly depend on OverlayLib decisions.

## Hard Constraints

- Preserve all current public functionality and API compatibility unless the subbundle explicitly documents and proves an intentional bug fix.
- No WebGL implementation work.
- No renderer replacement or true-canvas migration in this bundle; `CanvasBenchmark` may be validated as existing evidence only.
- No generated asset component hand edits.
- UI completion requires Playwright MCP or local Playwright proof with screenshots, actions, DOM assertions, and visual review notes.
- No new npm runtime dependency for Canvas, floating-window, calendar, preview, or related interactive implementation.

