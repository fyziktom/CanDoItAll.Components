# SB16 - Refactoring gate and closure

## Required before closing

1. Run file-size audits.
2. Run forbidden dependency scans.
3. Run large-screen-only WebGL policy scan.
4. Produce a markdown implementation report:
   - what changed
   - what was intentionally not changed
   - known risks
   - performance proof summary
   - validation commands and results
5. Update the XLSX-style implementation matrix if maintained in repo.

## Closure rule

Do not claim the bridge is implemented. This wave only prepares generic engine, generic run layer, scenario definitions, event streams, and visual intentions.
