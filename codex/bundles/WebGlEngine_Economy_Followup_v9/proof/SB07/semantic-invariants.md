# SB07 Semantic Invariants

## Invariants

- Economy terms may appear in Components run documents only as allowed `source.*` provenance values.
- Executable action ids, stage ids, motion ids, patch ids, and command-batch ids handed to Components must be generic stable tokens.
- Raw bridge diagnostics must not write domain words into non-source `WebGlRunDocument.Metadata`.
- The Economy bridge validator must run the Components `WebGlRunDocumentValidator` with Economy-owned strict boundary options.
- Arbitrary input metadata must not be copied into generic run-document metadata.

## Proof Links

- `bundle://proof/SB07/mapping-boundary-report.md`
- `bundle://proof/SB07/economy-webgl-boundary-tests.txt`
- `bundle://proof/SB07/webglrunlib-tests.txt`
- `bundle://proof/SB07/transcripts/source-assertions.txt`
