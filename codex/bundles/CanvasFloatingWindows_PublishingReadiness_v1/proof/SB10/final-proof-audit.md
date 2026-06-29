# SB10 Final Proof Audit

## Result

- Result: Passed.
- Date: 2026-06-29.
- Scope: Final Canvas/Floating Windows publishing-readiness closeout, excluding WebGL implementation and package work.

## Subbundle Proof Inventory

| Subbundle | Manifest | Semantic invariants | Closure result |
|---|---|---|---|
| SB01 | `bundle://proof/SB01/manifest.md` | `bundle://proof/SB01/semantic-invariants.md` | Passed |
| SB02 | `bundle://proof/SB02/manifest.md` | `bundle://proof/SB02/semantic-invariants.md` | Passed |
| SB03 | `bundle://proof/SB03/manifest.md` | `bundle://proof/SB03/semantic-invariants.md` | Passed |
| SB04 | `bundle://proof/SB04/manifest.md` | `bundle://proof/SB04/semantic-invariants.md` | Passed |
| SB05 | `bundle://proof/SB05/manifest.md` | `bundle://proof/SB05/semantic-invariants.md` | Passed |
| SB06 | `bundle://proof/SB06/manifest.md` | `bundle://proof/SB06/semantic-invariants.md` | Passed |
| SB07 | `bundle://proof/SB07/manifest.md` | `bundle://proof/SB07/semantic-invariants.md` | Passed |
| SB08 | `bundle://proof/SB08/manifest.md` | `bundle://proof/SB08/semantic-invariants.md` | Passed |
| SB09 | `bundle://proof/SB09/manifest.md` | `bundle://proof/SB09/semantic-invariants.md` | Passed |
| SB10 | `bundle://proof/SB10/manifest.md` | `bundle://proof/SB10/semantic-invariants.md` | Passed |

## Browser Evidence Audit

- SB05 validates Canvas workbench scenarios, viewports, interactions, accessibility mirror, export, clipboard, help/settings overlays, and console quality.
- SB06 validates calendar and preview behavior, CRUD, playlist, export callback, accessibility mirror, and console quality.
- SB07 validates Canvas and Overlay floating-window lifecycle, safe-top/container bounds, runtime alias ownership, and console quality.
- SB08 validates the final route matrix across Canvas, Canvas benchmark, and Overlays with 20 screenshots and 0 warnings/errors/pageerrors.
- SB09 and SB10 did not change browser-visible runtime behavior, so no additional browser rerun was required.

## Build/Test/Pack Audit

- Final focused in-scope tests passed: `bundle://proof/SB10/transcripts/dotnet-test-final-in-scope.txt`.
- Final generated asset verifier passed: `bundle://proof/SB10/transcripts/npm-canvaslib-verify-assets-final.txt`.
- SB09 focused release builds passed: `bundle://proof/SB09/transcripts/dotnet-build-canvas-overlay-release.txt`.
- SB09 package creation passed: `bundle://proof/SB09/transcripts/dotnet-pack-canvas-overlay.txt`.
- SB09 package content inspection passed: `bundle://proof/SB09/transcripts/package-content-manifest.txt`.

## Exception Audit

- Broad solution Release build with `--no-restore` failed only on out-of-scope WebGL sample/test restore gaps, recorded at `bundle://proof/SB09/transcripts/dotnet-build-release.txt`.
- This exception is not hidden as a Canvas/Overlay pass; it is separated as future WebGL publishing readiness work.

## Decision

- The Canvas/Floating Windows bundle has enough artifact-backed proof for transfer.
- WebGL remains explicitly excluded and should be handled by a separate bundle.
