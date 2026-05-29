# One-shot Codex execution prompt

You are working across two local repositories:

- `CanDoItAll.Components`
- `CanDoItAll.Economy`

Work only in the currently checked-out branch in each repository. Do not create a new branch. Do not run `git switch -c`, `git checkout -b`, or equivalent branch creation commands.

Use `05_spreadsheets/implementation_matrix.xlsx` as the execution map.

Implement the subbundles in order. Keep the repositories prepared but not connected.

In `CanDoItAll.Components`:
- finish hardening the generic WebGL engine;
- centralize JS command result creation;
- improve scene document hashing/validation;
- harden render scheduler and resource disposal;
- add model batch diagnostics;
- create a generic `CanDoItAll.Components.WebGlRunLib` foundation if not already present;
- keep all WebGL code domain-neutral.

In `CanDoItAll.Economy`:
- inspect the current simulator and ledger structure;
- create shared simulation abstractions for both simple-account and ledger-backed simulations;
- create simple-account backend preparation;
- create ledger-backed adapter preparation;
- create generic economy visualization contracts without WebGL references;
- add shared-well and entrepreneur scenario seeds in simple-account prep;
- do not reference Components/WebGL yet.

Run all validation commands and write implementation reports in both repos.

All source code comments must be in English.
