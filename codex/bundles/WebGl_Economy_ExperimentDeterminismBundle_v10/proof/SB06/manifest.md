# SB06 proof manifest

## Scope

Components run document and input-pack bridge provenance contracts.

## Changed files

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunProvenance.cs`
- `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunActionPlannerTests.cs`

## Proof

- Transcript: `bundle://proof/SB02/transcripts/components-validation.txt`
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- Semantic invariants: `bundle://proof/SB06/semantic-invariants.md`

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| `InputDocumentRef`, `InputHashRef`, `RunSourceRef` | future bridge and WebGL run documents | input pack hashes -> generic run metadata | Validator rejects domain-specific terms in generic WebGL provenance. |
