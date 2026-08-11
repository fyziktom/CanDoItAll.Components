# TODO

Extracted from the sibling app repo's UI/UX refactor notes where this component library is used.

## Findings summary

- **`ListDetailShell` is the single highest-leverage fix.** It's used in 14 files / 30
  references app-side, but it's a plain two-column grid with a hardcoded rounded/shadowed
  card look, no collapse, no resize, wrong responsive behavior (stacks panes instead of
  switching list-or-detail), and no keyboard selection semantics.
- **The design-token layer is real and mostly good** (`--cad-tone-*`, `--cad-color-*`,
  `--cad-space-*`, `--cad-radius-*`, `--cad-shadow-*` in `foundation/theme.css`), but
  `ListDetailShell` itself bypasses it with hardcoded `slate`/`white` classes.
- **Dark mode is fully built and unused.** The dark palette, `ThemeHost`, and a working
  toggle all exist (proven out in the Sandbox), but nothing in the app turns it on — and
  it can't work anywhere `ListDetailShell` is used until that hardcoding is fixed.
- **Typography has ~30 ways to render a string and no token layer.** 17 `TextStyle`
  members plus 13 standalone typography components, zero `--cad-font-*`/`--cad-text-*`
  tokens, with known duplicates (`Subtitle1`/`H5`, `Note`/`Muted`).
- **The library is more complete than it looks from outside**, but per-component API
  documentation is essentially absent, and the Sandbox catalogue — the library's best
  asset — is invisible unless you clone the repo and run it.
- **Two smaller primitives contribute to app-side "everything is loud" feel**: `CompactStat`
  has no muted-when-zero mode, and `PageHeader.Description` has no collapsed state.
- **`SecondaryTabs` (key-based) and `Tabs` (index-based) render differently**, which is
  part of why tab styling is inconsistent across app pages.

## TODO

### Documentation / catalogue
- [ ] Write per-component API documentation: prop tables, usage guidance, do/don't
  (`docs/architecture/` currently has exactly one file).
- [ ] Write a short "which component when" decision guide (e.g. `SectionHead` vs.
  `SectionHeading` vs. `Header`; `SecondaryTabs` vs. `Tabs`).
- [ ] Publish the Sandbox catalogue as a GitHub Page. Recommended approach: add a WASM
  host project alongside the existing Blazor Server sandbox, sharing the same
  catalogue pages and component registry, and static-publish that. There's already a
  `ci.yml` to hang a Pages job off of. (Prerendering to static HTML or a screenshot
  gallery are cheaper fallback options but lose interactivity.)

### `ListDetailShell` (highest leverage)
- [ ] Add a flat/hairline variant — no default `rounded-[1.75rem]`, no default
      `shadow-sm shadow-slate-200/60`; radius and shadow should be opt-in, not baked into
      `ResolveHeaderClass()`/`ResolveContentClass()`.
- [ ] Replace hardcoded `px-4 py-3.5` / `px-4 py-4` padding with `--cad-space-*` tokens.
- [ ] Support collapsing the list pane to give the detail pane full width.
- [ ] Add a real resizable/draggable divider. Today `Split.razor` is a plain flex row and
      `WorkspaceSplit.razor` is a bare `<div>` — the only resize behavior anywhere in the
      library is `AllowResize` on floating `OverlayWindow`.
- [ ] Fix the responsive behavior: the existing `46rem` container query in
      `list-detail.css` stacks the panes vertically, which is wrong for this shape. Replace
      with a list-*or*-detail switch plus a back affordance.
- [ ] Add selection semantics: keyboard/arrow-key navigation, roving focus, `aria-selected`.
- [ ] Remove the hardcoded `slate`/`white` classes so the component is token-driven (also
      the blocking piece for dark mode wherever this component is used).

### Typography consolidation
- [ ] Collapse the `TextStyle` enum (17 members) and the 13 standalone typography
      components toward a target of roughly 5 sizes × 3 weights.
- [ ] Introduce `--cad-font-*`/`--cad-text-*` tokens to back `typography/text.css` (there
      are currently zero).
- [ ] Resolve known duplicate styles: `Subtitle1`/`H5` render identically, as do
      `Note`/`Muted`.
- [ ] Keep old enum members as aliases (mark `[Obsolete]` with a warning) so the app's 472
      `TextBlock` call sites can migrate incrementally rather than all at once.

### Other primitive gaps
- [ ] `CompactStat`: add a muted-when-zero mode (today a `0` count still renders in full
      tone color).
- [ ] `PageHeader.Description`: support a collapsed/expandable state for long hint text.
- [ ] Reconcile `SecondaryTabs` (key-based) vs. `Tabs` (index-based) — two primitives doing
      the same job with different visual output. Either consolidate or document clearly
      which to use when.

### Dark mode readiness
- [ ] No new build needed — palette, `ThemeHost`, and the Sandbox toggle
      (`SandboxThemeState.cs`) already work. The only blocker is the `ListDetailShell`
      hardcoding above; once that's fixed, verify no other library component hardcodes raw
      palette colors instead of tokens.

### Policy / process
- [ ] Add a CI lint rule that rejects raw palette Tailwind utilities (e.g. `slate-*`,
      `sky-*`) outside the token layer, to enforce the existing Tailwind policy rather than
      relying on review discipline.
- [ ] Reconcile this repo's Visual Gate (`docs/standard-components-tailwind-policy.md`,
      which requires narrow-mobile screenshot proof for every styling change) with the app
      repo's `ui-support-scope.md`, which currently scopes the app to large-desktop only.
      The two documents currently disagree.
- [ ] Confirm whether output-CSS regeneration in this repo has build enforcement (the app
      repo's generated `output.css` is committed with none, which is a silent-drift risk
      worth checking here too).

### Open questions (joint with app repo)
- How aggressive to be on the typography collapse, and who absorbs the migration cost
  across the app's 472 `TextBlock` call sites (some mappings are mechanical, an estimated
  15–20% need a human judgment call).
- Concrete density reference numbers to adopt as token targets — roughly a 4px base grid,
  32px list rows, 13–14px body text, taken from the Linear-style target referenced in the
  source doc.
- Whether per-component API documentation is worth writing at all, versus the published
  Sandbox catalogue plus a short decision guide being sufficient.
