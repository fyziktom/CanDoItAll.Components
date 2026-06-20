# Experiment readiness model

## Current answer

The current system can be used for exploratory runs and engineering validation. It should not yet be used for strong economic claims without the additional gates in this bundle.

## Research-ready gate checklist

- [ ] scenario pack manifest passes per-file hash validation
- [ ] strict policy passes with zero unallowlisted warnings
- [ ] behavior expansion profile is declared and hashed
- [ ] golden oracle suite passes
- [ ] deterministic repeated runs produce identical frame hash chains
- [ ] metric/invariant registries validate all definitions
- [ ] readiness report status is `research-ready`
- [ ] browser observer proof, if claimed, is idle/settled and source-hash matched
- [ ] performance budget is within configured profile or marked not-comparable
- [ ] artifact manifest is complete and reproducible

## Status meanings

- `engineering-demo`: UI/pipeline demo only.
- `exploratory`: useful for hypothesis building; not evidence.
- `headless-valid`: can run without UI and produces valid artifacts.
- `oracle-valid`: golden oracles pass.
- `browser-observer-valid`: browser visual proof matches generated run document and settles.
- `research-ready`: all hard gates pass.
- `not-comparable`: run succeeded but performance/config/noise prevents comparison.
- `failed`: a hard gate failed.
