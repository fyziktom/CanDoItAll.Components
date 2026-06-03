# SB08 proof manifest

Status: completed

## Semantic assertion

Components WebGlRunLib now enforces provenance policy v2 with a narrow `source.*` allowlist, 96-character key limit, and 512-character opaque value limit. Economy bridge run documents now emit the agreed generic source shape and keep Economy-specific required command provenance checks inside `EconomyWebGlRunValidator`. Components has no Economy package reference or Economy-named interpretation path.

## Required proof artifacts

- `transcripts/failing-first.txt`
- `transcripts/passing-tests.txt`
- `transcripts/source-assertions.txt`
- `transcripts/boundary-audit.txt` if a package boundary is touched
- `transcripts/validator-audits.txt`
- `changed-file-hashes.md`
- browser artifacts if browser behavior is claimed

## Results

- Failing-first tests: `transcripts/failing-first.txt` records that the pre-implementation generic validator rejected new v2 source keys, still used the old 256-character value cap, and did not reject legacy source identifiers/hashes as expected by the new tests.
- Passing tests: `transcripts/passing-tests.txt` passed 11/11 focused Components validator tests and 2/2 focused Economy bridge/strict provenance tests.
- Source assertions: `transcripts/source-assertions.txt` proves the Components allowlist and limits, removal of stale Economy bridge run-layer source keys, and required Economy command provenance diagnostics.
- Boundary audit: `transcripts/boundary-audit.txt` proves WebGlRunLib has no Economy references and does not read `source.*` values for domain behavior.
- Validator audits: `transcripts/validator-audits.txt` records the bundle validator and proof-integrity audit passing after SB08 proof was added.
- Changed hashes: `changed-file-hashes.md` records hashes for SB08 code, tests, docs, and proof artifacts.

## Refactor Gate

- Changed Components files: WebGlRunDocumentValidator, WebGlRunProvenance labels, WebGlRunValidatorTests, and `docs/webgl/run-layer-boundary.md`.
- Changed Economy files: WebGl bridge run/frame/initial-scene/action metadata emitters plus bridge strict mapping tests.
- Public API changed: no type or method signature changes. Behavioral metadata contract changed: arbitrary run-layer `source.*` metadata is no longer accepted by generic validators; bridge users should map domain identifiers to `source.kind`, `source.domain`, `source.traceId`, `source.sequence`, or `source.parentId`, and keep bridge names in neutral metadata such as `bridge`.
- Open risks: WebGlLib scene metadata still has its older `source.*` escape hatch; SB08 was scoped to WebGlRunLib, so scene-layer provenance tightening remains a separate follow-up if desired.

## Completion rules

This manifest cannot be marked completed unless all required proof files are non-empty and cite the command, result, and semantic assertion.
