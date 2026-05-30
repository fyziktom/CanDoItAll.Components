# Mandatory rules for Codex

## Branch rule

Codex must work in the currently checked-out branch in each repo.

Forbidden:

```text
git checkout -b ...
git switch -c ...
git branch new-name
```

Allowed:

```text
git status
git branch --show-current
git log --oneline -n 5
```

## WebGL large-screen-only rule

WebGL surfaces are desktop/large-screen only.

Codex must not implement or optimize:

- mobile
- tablet
- phone
- small-screen
- medium-screen
- responsive redesign for WebGL

Allowed:

- desktop viewport proof at `1440x900` or larger
- unsupported-size warning if viewport is below minimum
- no mobile/tablet screenshot requirements

## Genericity rule

Shared-well and farmer-land are test cases only. Do not hardcode generic engine behavior to those scenario ids.
