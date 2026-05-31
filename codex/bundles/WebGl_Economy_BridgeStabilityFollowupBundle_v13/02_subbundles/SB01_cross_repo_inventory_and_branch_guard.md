# SB01 — Cross-repo inventory, branch, and boundary guard

## Goal
Record current branches, changed projects, and dependency boundaries before implementation.

## Tasks
- Verify `CanDoItAll.Components` is on the currently checked out branch. Do not create a new branch.
- Verify `CanDoItAll.Economy` is on the currently checked out branch. Do not create a new branch.
- Record solution projects and relevant project references.
- Run and extend boundary audits.

## Must prove
- Components has no Economy references.
- Economy bridge references only Visualization, Abstractions, and WebGlRunLib.
- Economy lower layers do not reference Components/WebGL.
- Bridge does not reference SimpleAccounts or Ledger backends.

## Output proof
- `proof/SB01/manifest.md`
- `proof/SB01/semantic-invariants.md`
- command transcripts
