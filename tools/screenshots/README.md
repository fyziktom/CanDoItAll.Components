# Screenshots tool

Screenshots the Sandbox catalog with Playwright, diffs against the previous run with
`odiff-bin`, and writes an interlinked Markdown report. Each run is committed to its own
orphan `screenshots_{yyyy-MM-dd}_{HH-mm}` (UTC) branch in a **separate** git repository
(configured via `storageRepoPath`), so history never touches this repo, branch names sort
and compare correctly across timezones, and a run can be reclaimed by deleting its branch.

This is a local, opt-in dev tool — it is not wired into CI.

## One-time setup

1. Create (or clone) the repo used for screenshot storage. For local use, a sibling directory
   works well and is what `screenshots.config.json` defaults to:

   ```
   git init ../../../CanDoItAll.Components.Screenshots
   ```

   (that path is written relative to `tools/screenshots/`, i.e. a sibling of this repo's
   own root — see `storageRepoPath` in `screenshots.config.json`.)

2. Start the Sandbox app so the tool has something to capture:

   ```
   dotnet run --project samples/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj
   ```

## Usage

```
npm run screenshots           # capture + diff + report + store, in one go
npm run screenshots:capture   # screenshot every route × viewport × theme × alternative into .artifacts/screenshots
npm run screenshots:diff      # odiff the fresh capture against the most recent screenshots_* branch
npm run screenshots:report    # render the Markdown report from the manifest + diff results
npm run screenshots:store     # commit the current .artifacts/screenshots run to a new screenshots_* branch
npm run screenshots:prune     # delete old local screenshots_* branches (interactive confirm, --yes to skip, --remote to also delete on origin)
```

`screenshots:snapshot` computes the branch name once up front and passes it to both `report.cjs`
(so the report's `<h1>` matches) and `store.cjs` (so that's the branch it actually commits
to). Neither ever pushes — `screenshots:store`/`screenshots:snapshot` print the `git push` command to
run yourself once you want to share a branch.

By default (`screenshots.config.json`) every script captures the trimmed matrix: desktop
only, light only, happy-path only. Pass `--config tools/screenshots/screenshots.config.full.json`
to any of the scripts above to capture the full matrix instead (desktop + tablet + mobile,
light + dark, and all six scenario alternatives) — e.g. `npm run screenshots:snapshot -- --config
tools/screenshots/screenshots.config.full.json`.

## Report structure

Each run's `.artifacts/screenshots` (and, once stored, each `screenshots_*` branch) looks like:

```
README.md                       — per-alternative screenshot grid + page index, titled with the branch name
.gitignore
data/
  manifest.json, diff-results.json
current/{viewport}_{theme}_{alternative}/
  {page}.png, README.md         — gallery of every page's current screenshot for that combo
baseline/{combo}/{page}.png     — only for changed/removed pages
diff/{combo}/
  {page}.png, README.md         — gallery of diffs, only written if that combo has any
pages/{alternative}/{page}.md   — baseline/current/diff per page, with Previous/1/2/…/Next pagination
```

The root `README.md`'s `## Alternative: {name}` headings (and the `<small>` alternative
suffix on each page's `<h1>`) are only shown when more than one `alternatives` entry is
configured — with the default `happy-path`-only config there's nothing to disambiguate, so
they're omitted.

## Config (`screenshots.config.json` / `screenshots.config.full.json`)

- `baseUrl` — origin the Sandbox app is running on.
- `routesEndpoint` — fetched at capture time; the Sandbox app exposes its page catalog (the
  main pages only, not their scenario variants — see "alternatives" below) at
  `/api/pages.json` (see `Program.cs`), so new pages are picked up automatically. Each entry
  is `{ path, title, group }` — `group` is a coarse category (`"Home"` or `"Components"`),
  not a unique identifier; the tool derives each page's own identity from `title`.
- `routes` — inline, hand-authored targets, merged with `routesEndpoint` (routes with the same
  `path` from `routes` win). Either or both may be used; this makes the tool reusable against
  apps that don't expose a page catalog endpoint at all.
- `viewports` — real Playwright viewport sizes. `frameQuery` also sets the Sandbox's
  `?frame=desktop|mobile` query param so the in-page demo-frame CSS preset matches the actual
  viewport being captured, instead of fighting it. The Sandbox has no dedicated tablet preset,
  so the `tablet` entry in the full config omits `frameQuery` and falls back to the
  unconstrained "live viewport" styling.
- `themes` — explicit `{ name: "query-string-fragment" }` map, e.g.
  `{ "light": "", "dark": "dark=true" }`. Not hardcoded to the Sandbox's `dark` param, so a
  different target app with a different theme param (or more than two themes) needs no code
  changes, only config.
- `alternatives` — explicit `{ name: "query-string-fragment" }` map, same shape as `themes`,
  for the Sandbox's `?scenario=` variants (dense content, empty state, loading, disabled, long
  text). `screenshots.config.json` only has `{ "happy-path": "" }`; `screenshots.config.full.json`
  has all six. Every route × viewport × theme is captured once per `alternatives` entry.
- `readySelector` — waited for before a screenshot is taken, in addition to `networkidle` and
  `document.fonts.ready`.
- `defaultSettleMs` — extra pause after the ready selector appears, to let any last layout
  settle. Screenshots are also taken with `animations: "disabled"` and the page's
  `reducedMotion` set to `"reduce"`.
- `storageRepoPath` — the separate git repo screenshots are committed to.
- `gitignore` — optional array of `.gitignore` patterns (default: `[".idea/", ".vscode/", ".DS_Store", "Thumbs.db"]`). Each `screenshots_*` branch is an orphan root with its own tree, so a `.gitignore` in one branch (e.g. on `master`) doesn't cover any other — `store.cjs` writes this list as a `.gitignore` into every new run before committing it, so IDE/OS cruft stays out regardless of which branch happens to be checked out. Set to `[]` to skip writing one.

## Known limitations (not solved here, tracked for follow-up)

- **Charts and Mermaid** can still show noise beyond what `animations: "disabled"` + the settle
  wait fixes — Charts animate via Apex, Mermaid assigns randomized SVG element ids per render.
  Per-group masking (`odiff`'s `ignoreRegions`) is a follow-up once real flake is observed.
- **Gantt's timeline is anchored to `DateTime.Now`**, so every day's baseline diffs against a
  new one until the Sandbox's clock is pinned to a fixed distant-past instant. Not built yet.
- Publishing a `screenshots_*` branch anywhere is a manual, explicit step you run yourself.
