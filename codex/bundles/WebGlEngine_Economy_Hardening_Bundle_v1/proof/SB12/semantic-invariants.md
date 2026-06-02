# SB12 Semantic Invariants

## Invariants

| Invariant ID | Requirement | Expected behavior | Disallowed shallow implementation | Negative proof | Positive proof |
| --- | --- | --- | --- | --- | --- |
| SB12-I01 | Package output must be real and current. | `dotnet pack CanDoItAll.Components.slnx --configuration Release --output artifacts/packages` emits WebGlLib and WebGlRunLib packages from the current Components checkout. | Reusing a pre-existing private-feed package or only building project references. | `failing-package-reference-stale-feed-order.txt` | `components-pack-release.txt`, `components-package-inventory.txt` |
| SB12-I02 | Economy local mode must be explicit. | The bridge builds with `ComponentsRepoRoot` pointing at the local Components checkout. | Implicitly relying on machine-specific relative paths without a validation error. | `failing-invalid-componentsrepo-root.txt` | `economy-webglbridge-project-reference-build.txt` |
| SB12-I03 | Economy package mode must consume both WebGl packages. | With `UseComponentsWebGlRunLibPackage=true`, the bridge restores WebGlLib and WebGlRunLib from the proof package feed and builds without project references. | Restoring stale `0.1.0` packages or letting WebGlRunLib transitively hide the bridge's direct WebGlLib dependency. | `failing-package-reference-stale-feed-order.txt`, `failed-package-reference-source-args.txt` | `economy-webglbridge-package-reference-build.txt`, `sb12-package-restore-graph.txt` |
| SB12-I04 | Components must remain domain-neutral. | Components WebGlLib/WebGlRunLib have no Economy references; forbidden domain terms appear only in docs, validator deny-list, and negative tests. | Adding Economy or production-line concepts to Components to make package mode easier. | `sb12-anti-stub-and-boundary-scan.txt` | `components-webgllib-boundary-audit.txt`, `components-webglrunlib-boundary-audit.txt` |

## Shallow-Pass Trap

A shallow package proof can pass by building only local project references or by restoring stale global packages that happen to satisfy old APIs. SB12 prevents that by running the required pack command, failing first against stale feed order, adding a proof NuGet.config with `<clear />`, restoring into an isolated `NUGET_PACKAGES` cache, and then building package mode with `--no-restore`.

## Browser Note

No browser route changed in SB12. Browser validation remains for SB13, where runtime/package output must be exercised through final route-level proof.

## Reopen Triggers

- Package version changes without rerunning pack plus package-mode restore/build.
- Economy bridge project properties change without rerunning local and package mode.
- Components WebGlLib/WebGlRunLib gain a domain reference or circular dependency.
- A consuming repo reports restore success from a stale private feed rather than fresh package output.
