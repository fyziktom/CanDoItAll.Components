# Quality Attributes

## Genericity

- `WebGlLib` must be generic and light enough for simple 3D model display and generic scene visualization.
- `WebGlRunLib` must be generic and reusable by Economy, future production-line simulations, and other domains.
- Economy-specific terms must remain in `CanDoItAll.Economy.*`.

## Performance

- Transform-only and symbol-only changes must not rebuild the entire dynamic scene.
- Large primitive scenes must be profiled with diagnostics.
- GLB profile switching and repeated import/export must remain stable.

## Safety

- Invalid patches must not partially mutate runtime state.
- Resource ownership must prevent shared texture disposal bugs.
- Strict Economy mapping must fail loudly on unresolved source nodes/actions.

## Observability

- Diagnostics must expose rebuild counts, patch classification counts, asset cache stats, missing/fallback assets, frame timing and command batch metrics.
- Critical proof must be artifact-backed under `proof/SBxx/`.

## Packaging

- Components must build and pack independently.
- Economy must support local project-reference development and package-reference consumption.
