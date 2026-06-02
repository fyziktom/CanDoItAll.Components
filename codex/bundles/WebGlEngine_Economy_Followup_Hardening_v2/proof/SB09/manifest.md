# SB09 proof manifest

Status: Completed 2026-06-02.

## Changed file hashes

Recorded in `proof/SB09/transcripts/changed-file-hashes.txt`.

| Repo | File | SHA256 |
| --- | --- | --- |
| Components | `README.md` | `580410734e6e37e4bb085af74232ab47100cc30de39e878895335ab8a64e57bb` |
| Components | `samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj` | `eb0e7d23f956965948a30a68b931bc14651a58eaabd8b7f445dc0bfd48f235bf` |
| Components | `samples/CanDoItAll.Components.WebGlLibOnlyViewer/README.md` | `5e698888a792b16d1756407e8c1100a72fb91d9e8786f0d5d26bdfc330290d91` |
| Components | `src/CanDoItAll.Components.WebGlLib/README.md` | `c67a3d2b26d7f4e9619189262157bcf52172cb1a104ffa3c2bd6c78e530ed3c3` |
| Components | `proof/SB09/package-proof.NuGet.config` | `27755218476922912dc67f29838c6bbe6dfae4b2a294fbd87b143caf625c033d` |
| Economy | `README.md` | `9f6bb37fd7f803154bdbbd897d890f47a8b3a46cc193c53912fe5864ac133723` |
| Economy | `src/CanDoItAll.Economy.Components/CanDoItAll.Economy.Components.csproj` | `817176e07e245ba98a0997ff27cea30c1521cf549c7b7ac109832980df2da8a1` |
| Economy | `src/CanDoItAll.Economy.SimulationSandbox/CanDoItAll.Economy.SimulationSandbox.csproj` | `ec5b33568c83d34c50dd8acedd032421b2fb29abf494125578a6f3df75e1a8f4` |

## Command transcripts

- Failing-first package-mode scan: `proof/SB09/transcripts/failing-first-webgllib-only-sample-package-mode-scan.txt`.
- Components pack: `proof/SB09/transcripts/components-dotnet-pack.txt` timed out after creating packages; final successful no-restore pack proof is `proof/SB09/transcripts/components-dotnet-pack-success.txt`.
- WebGlLib-only sample package restore/build: `proof/SB09/transcripts/webgllib-only-sample-isolated-restore.txt`, `proof/SB09/transcripts/webgllib-only-sample-package-mode-build.txt`.
- WebGlLib-only sample dependency proof: `proof/SB09/transcripts/webgllib-only-sample-dependency-graph-scan.txt`, `proof/SB09/transcripts/webgllib-only-sample-no-webglrunlib-assertion.txt`.
- WebGlLib-only sample project-reference sanity build: `proof/SB09/transcripts/webgllib-only-sample-project-mode-build.txt`.
- Economy WebGlBridge package restore/build: `proof/SB09/transcripts/economy-webglbridge-package-mode-restore.txt`, `proof/SB09/transcripts/economy-webglbridge-package-mode-build.txt`.
- Economy Components package-mode negative proof: `proof/SB09/transcripts/economy-components-package-mode-restore.txt`, `proof/SB09/transcripts/economy-components-package-mode-restore-after-propagation.txt`, `proof/SB09/transcripts/economy-components-package-mode-restore-final.txt` show static graph restore still resolves transitive Components WebGL references at stale `0.1.0` unless the bridge package flag is explicit.
- Economy Components package-mode passing proof: `proof/SB09/transcripts/economy-components-package-mode-restore-explicit-final.txt`, `proof/SB09/transcripts/economy-components-package-mode-build-explicit-final.txt`.
- Economy package dependency graph proof: `proof/SB09/transcripts/economy-webglbridge-dependency-graph-scan.txt`, `proof/SB09/transcripts/economy-components-dependency-graph-scan.txt`, `proof/SB09/transcripts/economy-package-mode-assets-assertion.txt`.
- Economy Components project-reference sanity build: first run hit a transient Windows compiler output file lock in `proof/SB09/transcripts/economy-components-project-mode-build.txt`; `proof/SB09/transcripts/dotnet-build-server-shutdown-before-economy-project-mode-retry.txt` and `proof/SB09/transcripts/economy-components-project-mode-build-retry.txt` show the lock cleared and the same build passed.
- Nupkg content audit: `proof/SB09/transcripts/nupkg-content-listing.txt`, `proof/SB09/transcripts/nupkg-content-audit-summary.txt`.
- Boundary and source scans: `proof/SB09/transcripts/components-webgllib-boundary-audit.txt`, `proof/SB09/transcripts/components-webglrunlib-boundary-audit.txt`, `proof/SB09/transcripts/source-policy-assertions.txt`, `proof/SB09/transcripts/anti-stub-placeholder-scan.txt`.

## Browser artifacts

Not produced for SB09 by explicit subbundle exception. SB09 is a package-consumption proof for a Razor class library sample and package-mode consumers; SB11 remains the owner for browser UI and Node route proof.

## Source assertions

- `samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj` supports default project-reference mode and package mode through `UseComponentsWebGlLibPackage=true`, with a conditional `PackageReference` to `CanDoItAll.Components.WebGlLib`.
- `samples/CanDoItAll.Components.WebGlLibOnlyViewer/README.md`, Components `README.md`, and `src/CanDoItAll.Components.WebGlLib/README.md` document package-mode proof and isolated cache/fresh-feed requirements.
- `../CanDoItAll.Economy/src/CanDoItAll.Economy.Components/CanDoItAll.Economy.Components.csproj` and `../CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/CanDoItAll.Economy.SimulationSandbox.csproj` pass package-mode WebGl version properties through project-reference metadata for normal builds.
- `../CanDoItAll.Economy/README.md` documents the explicit `UseComponentsWebGlPackages=true` plus `UseComponentsWebGlRunLibPackage=true` command shape required for static graph restore package proof.
- `proof/SB09/package-proof.NuGet.config` clears inherited package sources and prioritizes the fresh SB09 package feed before nuget.org.

## Anti-stub audit

`proof/SB09/transcripts/anti-stub-placeholder-scan.txt` passes for changed SB09 source/documentation paths.

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Fresh Components package feed `artifacts/sb09-package-feed` | `dotnet pack CanDoItAll.Components.slnx /p:PackageVersion=0.1.0-sb09.20260602.1` | WebGlLib-only sample, Economy WebGlBridge, Economy Components | Local proof feed with matching isolated package caches | Initial Economy Components restore without explicit bridge package flag resolves stale/transitive `0.1.0`, captured in failed restore transcripts |
| WebGlLib-only sample package assets | `dotnet restore/build` with `UseComponentsWebGlLibPackage=true` | Minimal RCL sample | Restored from isolated cache `artifacts/sb09-nuget-cache-webgllibonly` | Dependency graph assertion fails if WebGlRunLib appears |
| Economy WebGL package assets | Package-mode restores with unique prerelease version and isolated caches | Economy WebGlBridge and Economy Components | Restored from SB09 NuGet.config and built without restore | Asset scans assert no Components WebGL project paths and all WebGL packages use `0.1.0-sb09.20260602.1` |
| Nupkg content listing | PowerShell ZipArchive audit | Package consumers and release reviewers | Full entry listing and summary retained in transcripts | Audit fails for codex/proof artifacts or entries above the huge-file threshold |

## Gate decision

Pass. SB09 proves WebGlLib-only package consumption, WebGlRunLib package consumption through Economy WebGlBridge and Economy Components, fresh-feed/isolated-cache hygiene, package content boundaries, and unchanged project-reference developer paths. Existing warnings are recorded in transcripts: `ncalc` NU1701 and upstream nullable/analyzer warnings; no SB09 compile errors remain.
