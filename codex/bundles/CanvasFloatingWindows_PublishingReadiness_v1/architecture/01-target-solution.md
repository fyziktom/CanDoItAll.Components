# Target Solution

## End State

CanvasLib and OverlayLib are ready for open-source transfer as maintainable, documented, packageable shared component libraries. Their public state contracts, runtime asset boundaries, JavaScript interop surfaces, floating-window behavior, sandbox coverage, and visual proof are explicit enough that downstream consumers can upgrade without rediscovering hidden assumptions.

## Ownership Boundaries

- OverlayLib owns generic floating-window behavior: geometry normalization, container/safe-top placement, drag/resize/minimize/restore/hide lifecycle, generic CSS, generic JS runtime, and public `OverlayWindow` API.
- CanvasLib owns canvas-specific composition: workbench state, graph nodes/links, calendar surface, Canvas wrapper windows, workbench toolbar/stage integration, canvas runtime assets, preview components, and Canvas-specific docs.
- Sandbox owns proof scenarios and demo data only. It must not become the only place where production behavior exists.
- Tools under `tools/canvaslib` own generated asset synchronization. Generated `CanvasLibHeadAssets.razor` and `CanvasLibBodyAssets.razor` must be regenerated, not manually edited.
- Runtime Canvas, floating-window, calendar, preview, and related interactive implementation owns no npm runtime dependency. Use pure browser JavaScript plus C# and Razor; reserve npm/Node for existing Tailwind, generated-asset, test, or browser-validation tooling only.

## Refactor Strategy

- Prefer tests and source maps before refactor.
- Refactor large JS/CSS/Razor files only after current behavior is captured by source assertions and browser smoke.
- Keep public APIs stable; use approval updates only when intentionally reviewed.
- Move duplicated generic window semantics toward OverlayLib when proof shows the duplication is real and safe to consolidate.
- Keep Canvas tokens and CSS where they express domain-specific stage/rendering needs; improve clarity and documentation instead of forcing a broad styling migration.

## Validation Strategy

- Unit/contract tests: state normalization, window roundtrip, selection, serialization, layout/collision, viewport math, calendar request/response contracts, package/API approvals.
- Asset/tooling proof: `npm run canvaslib:verify-assets`, node syntax checks for Canvas/Overlay JS, generated-asset source assertions.
- Browser proof: Playwright MCP or local Playwright with screenshots, DOM assertions, console capture, action traces, and explicit visual review questions.
- Package proof: `dotnet build`, targeted tests, `dotnet pack`, NuGet content verifier, public API snapshots, and README/package metadata checks.
- Critical proof: every critical subbundle must create `proof/SBxx/manifest.md` and `proof/SBxx/semantic-invariants.md` before closure.

## Non-Goals

- No WebGL source work.
- No Economy or consuming-app migration.
- No wholesale renderer replacement.
- No broad design restyle disconnected from proof.
- No npm runtime dependencies for Canvas, floating-window, calendar, preview, or related interactive implementation.

