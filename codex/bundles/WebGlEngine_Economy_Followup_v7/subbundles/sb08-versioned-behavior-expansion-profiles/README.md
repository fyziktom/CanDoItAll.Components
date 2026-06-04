# SB08 — Versioned behavior-expansion profiles

## Repository scope

Economy

## Goal

Make automatic event expansion explicit, versioned, declared, and hashable.

## Tasks

- Introduce behavior expansion profile id/version.
- Require scenario packs to declare the expansion profile in research mode.
- Record expansion provenance for each derived event.
- Include expansion profile hash in run manifest and frame hash chain.
- Add tests comparing expanded vs non-expanded profile outcomes.

## Acceptance criteria

- Automatic expansion is never hidden in research-ready runs.
- Changed expansion rules change the reproducibility manifest/hash.
- Readiness report lists the behavior profile.

## Required proof artifacts

- `proof/SB08/transcripts/behavior-profile-tests.txt`
- `proof/SB08/artifacts/expanded-event-provenance.json`

## Gate

Forced refactor review after SB08.

## Implementation notes

- Prefer failing-first tests before changing implementation.
- Keep Components domain-neutral.
- Keep Economy economic semantics outside Components.
- Do not close this subbundle with screenshots only.
- Update traceability and proof manifest before moving on.

## Closure notes

Status: completed

- Added versioned, hashable behavior expansion profile descriptors.
- Propagated profile id/version/hash through event streams, derived events, scenario manifests, frames, deltas, readiness reports, and headless run artifacts.
- Scenario definition and manifest hashes now include the resolved expansion profile descriptor.
- Required proof is captured in `proof/SB08/transcripts/behavior-profile-tests.txt` and `proof/SB08/artifacts/expanded-event-provenance.json`.
