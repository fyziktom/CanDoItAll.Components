# SB01 Semantic Invariants

## Invariant ID: SB01-BRANCH-GUARD

Raw note: Codex must work in the currently checked-out branch in both repositories, must not create a new branch, and must record current branch and latest commit SHA for both repositories.

Expected behavior: Branch and commit evidence is captured from git, not inferred from commit messages or bundle assumptions.

Disallowed shallow implementation: Claiming the expected branch names from `bundle.json` without running git commands.

Failing-first proof: `bundle://proof/SB01/transcripts/branch-inventory-dependencies.txt` would expose a mismatch if either repository branch differed from the expected current branch.

Passing proof: The transcript records Components branch `webgl-engine`, Economy branch `main`, and both latest commit SHAs.

Changed source files and hashes: Bundle repair hashes are in `bundle://proof/SB01/transcripts/changed-file-hashes.txt`; no production source files were edited in SB01.

Production assertions: No production source behavior was changed.

Red-team negative case: A typo or vague latest commit subject cannot steer branch selection because branch evidence comes from `git branch --show-current`.

Downstream dependency check: SB02 and all later subbundles may proceed only from the recorded branch baseline.

Shallow-pass trap: Treating the bundle's expected branch names as proof without recording actual local git state.

Adversarial negative proof: The transcript includes actual git branch, status, and HEAD outputs for both repositories.

Semantic positive proof: The recorded branch and commit values establish the exact implementation baseline.

Anti-stub audit: `bundle://proof/SB01/transcripts/anti-stub-audit.txt`

## Invariant ID: SB01-BOUNDARY-BASELINE

Raw note: Verify dependency direction: Components has no Economy dependency; Economy WebGlBridge points toward WebGlRunLib plus Economy visualization/abstractions; SimulationSandbox is the composition layer.

Expected behavior: Boundary scans must distinguish clean constraints from baseline findings and map findings to downstream repair subbundles.

Disallowed shallow implementation: Marking boundary verification complete from project names only while ignoring source imports or composition references.

Failing-first proof: `bundle://proof/SB01/transcripts/branch-inventory-dependencies.txt` surfaced BF-SB01-001 and BF-SB01-002 as downstream findings.

Passing proof: The same transcript records that Components remains Economy-free and maps each Economy boundary finding to its downstream owner instead of hiding it.

Changed source files and hashes: Bundle repair hashes are in `bundle://proof/SB01/transcripts/changed-file-hashes.txt`; no production source files were edited in SB01.

Production assertions: `bundle://proof/SB01/transcripts/source-assertions.txt`

Red-team negative case: A scan that only inspects `.csproj` files would miss WebGlBridge source imports; SB01 includes source-level scan output.

Downstream dependency check: BF-SB01-001 must be addressed or explicitly closed by SB06/SB07, and BF-SB01-002 by SB11 before final closure.

Shallow-pass trap: Accepting a clean project-reference graph as proof of clean source coupling.

Adversarial negative proof: The source scan reports the bridge WebGlLib imports and sandbox SimpleAccounts composition references.

Semantic positive proof: The Components scans prove no Economy text or forbidden domain terms in the scoped runtime source.

Anti-stub audit: `bundle://proof/SB01/transcripts/anti-stub-audit.txt`
