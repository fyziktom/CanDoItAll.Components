# Refactoring policy for this wave

Every 3-4 subbundles, Codex must perform a refactoring gate.

## File size policy

Soft warning:

- JS runtime module > 220 lines
- C# contract/service file > 220 lines
- Razor page > 180 lines
- CSS file > 220 lines

Hard warning:

- JS runtime module > 320 lines
- C# file > 320 lines
- Razor page > 260 lines
- CSS file > 360 lines

If a file crosses a hard warning, split it unless there is a strong documented reason.

## No new branch rule

Codex must not run:

```text
git switch -c
git checkout -b
git branch <new-name>
```

It must first report the current branch in each repository and continue there.
