# SB01 - Cross-repo inventory and branch guard

Codex must:

- work in the currently checked-out branch in both repositories,
- not run `git switch -c`, `git checkout -b`, or create any new branch,
- record current branch and latest commit SHA for both repos,
- confirm the Components commit message typo does not affect branch selection,
- verify dependency direction:
  - Components -> no Economy
  - Economy.Simulation.WebGlBridge -> WebGlRunLib + Economy visualization/abstractions only
  - Economy.SimulationSandbox -> composition layer only
