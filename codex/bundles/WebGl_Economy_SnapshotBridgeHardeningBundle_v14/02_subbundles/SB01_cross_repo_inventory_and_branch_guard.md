# SB01 - Cross-repo inventory and branch guard

Goal:
- Confirm current branches in both repositories.
- Do not create new branches.
- Record source file inventory before edits.

Tasks:
1. Record current branch names.
2. Record changed source files relevant to WebGL, WebGlRunLib, Economy simulation, visualization, WebGlBridge.
3. Run existing build/tests/audits before edits.
4. Confirm Components has no Economy references.
5. Confirm Economy bridge is the only layer referencing Components WebGlRunLib.

Validation:
- `git branch --show-current` in both repos.
- `dotnet build`
- existing boundary audits.
