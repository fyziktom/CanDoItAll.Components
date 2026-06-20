# SB17 Semantic Invariants

Status: completed

## Invariants

- Final closure must not upgrade a scenario to `researchReady=true` unless headless readiness, oracle coverage, and browser observer evidence all support that claim.
- A `--no-oracle` final catalog run must leave `oracleValid=false` for every scenario in the final decision matrix.
- Browser observer proof must remain separate from headless economic truth: browser proof can validate rendered runtime behavior but cannot make failed headless scenarios research-ready.
- Generic Components source must not contain blocking economy-domain terms in affected WebGlLib/WebGlRunLib production code.
- Legacy catalog failures must remain visible in the final matrix with their strict validation causes.
- Proof closure must reject empty artifacts and placeholder/stub source markers.

## Production Behavior Artifact Matrix

| Signal or behavior | Producer | Consumer | Lifecycle | Negative or red-team proof |
| --- | --- | --- | --- | --- |
| Final scenario classification | Economy headless run summaries and readiness reports | SB17 final decision matrix | Headless catalog run writes per-scenario artifacts; SB17 maps them to exploratory/headless/oracle/browser/research-ready columns | Legacy `shared-well` and `farmer-land` rows remain failed/not research-ready in `bundle://proof/SB17/final-decision-matrix.json` |
| Oracle-valid separation | Economy CLI `--no-oracle` run mode | SB17 final decision matrix | Oracle coverage label `no-oracle` prevents oracle-valid and research-ready closure | All rows have `oracleValid=false` in `bundle://proof/SB17/final-decision-matrix.md` |
| Browser observer validity | SB02-SB05 and SB12 browser assertions | SB17 browser proof summary | Existing browser artifacts are checked for assertions, screenshots, console logs, runtime idle, hash match, and genericity | `bundle://proof/SB17/transcripts/browser-proof-verification.txt` |
| Generic Components boundary | WebGlLib/WebGlRunLib production source | SB17 domain leakage scan | Affected production files are scanned excluding vendor/generated/README noise | `blockingMatchCount=0` in `bundle://proof/SB17/domain-leakage-scan.txt` |
| Artifact hygiene | Bundle proof tree | SB17 inventory and validator | Proof artifacts are counted, sized, and checked for empties before final validation | `emptyFileCount=0` in `bundle://proof/SB17/artifact-inventory.json` |

## Proof

- `bundle://proof/SB17/final-red-team-report.md`
- `bundle://proof/SB17/final-decision-matrix.json`
- `bundle://proof/SB17/browser-proof-summary.json`
- `bundle://proof/SB17/domain-leakage-scan.txt`
- `bundle://proof/SB17/artifact-inventory.json`
- `bundle://proof/SB17/transcripts/source-assertions.txt`
- `bundle://proof/SB17/transcripts/anti-stub-audit.txt`
