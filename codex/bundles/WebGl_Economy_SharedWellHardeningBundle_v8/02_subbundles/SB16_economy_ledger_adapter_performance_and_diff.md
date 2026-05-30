# SB16 — Economy: ledger adapter hardening

## Required work
- Validate snapshot sequence and run/scenario ids.
- Use dictionary-based diffs for stores/issues/artifacts/events.
- Add tests for:
  - unchanged store omitted from delta;
  - new evidence produces event/issue/artifact;
  - duplicate snapshot sequence rejected;
  - missing step returns diagnostic failure path where appropriate.
- Keep ledger adapter isolated from SimpleAccounts and Visualization dependencies unless explicitly justified.
