# Semantic Tailwind theme direction

[`test.html`](./test.html) and
[`input-test.css`](../Tailwind/input-test.css) are an editable proof for a
possible evolution of the production theme. The production baseline is
[`foundation/theme.css`](../Tailwind/foundation/theme.css), not an earlier
version of this demo.

Build the demo stylesheet once with:

```sh
./Tailwind/node_modules/.bin/tailwindcss --input ./Tailwind/input-test.css --output ./assets/output-test.css
```

Or keep `npm --prefix Tailwind run watch:test` running while editing.

## What production already does well

[`foundation/theme.css`](../Tailwind/foundation/theme.css) already owns the
component-facing theme contract:

- structural tokens: `--cad-color-page-bg`, `--cad-color-surface`,
  `--cad-color-border`, and text tokens;
- interaction tokens: `--cad-tone-*-solid-*` and `--cad-tone-*-soft-*`;
- light and dark values for those tokens; and
- scoped theme boundaries, including the correct `color-scheme` value for
  native controls and browser UI.

Production components consume that contract. For example,
[`buttons.css`](../Tailwind/controls/buttons.css) uses `--cad-tone-*-soft-bg`,
`--cad-tone-*-soft-border`, and `--cad-tone-*-soft-fg` rather than choosing a
color in each button rule.

The experiment should preserve this public component contract. Its contribution
is a semantic colour-scale layer that can provide the production values.

## Proposed layering

Use three layers, from least to most specific:

1. Semantic scales: `--color-chrome-*`, `--color-info-*`,
   `--color-success-*`, `--color-warn-*`, and `--color-danger-*`.
2. Existing production intent tokens: `--cad-color-*` and `--cad-tone-*-*`.
3. Components that consume only the production tokens.

The production-facing token names stay stable while the palette and contrast
mapping can change behind them.

## Original colours have one source

Literal colour definitions—hex, `rgb()`, `hsl()`, or `oklch()`—belong only in
the `@theme` block that defines the palette. This makes that block the single
auditable source for original colours and lets Tailwind generate the semantic
utility families from it.

Every layer after `@theme` must derive from those tokens: reference a token,
adjust alpha, use `color-mix()`, or use a relative colour. Do not introduce a
new literal colour in a component, theme scope, or sample just to make a
variant look right.

```css
@theme {
  --color-info-600: oklch(60.2% 0.157 243.8);
}

/* Derived values outside @theme. */
--cad-tone-info-soft-bg: color-mix(in oklch, var(--color-info-600) 14%, transparent);
--cad-tone-info-soft-border: color-mix(in oklch, var(--color-info-600) 42%, transparent);
```

This rule applies equally to light and dark mappings. A changed source colour
then propagates through CSS, rather than leaving near-duplicate values behind.

The `--color-` prefix is intentional: Tailwind turns a token such as
`--color-chrome-300` into utilities including `text-chrome-300`,
`bg-chrome-300`, and `border-chrome-300`. The `--cad-*` tokens are not colour
scales; they are component-facing CSS variables and should be consumed through
`var(...)`.

```css
/* Theme token values use semantic scales. */
--cad-color-page-bg: var(--color-chrome-50);
--cad-color-surface: var(--color-white);
--cad-color-border: var(--color-chrome-300);

--cad-tone-info-soft-bg: var(--color-info-50);
--cad-tone-info-soft-border: var(--color-info-300);
--cad-tone-info-soft-fg: var(--color-info-800);
```

The demo uses Tailwind's `--color-white` token for its white surface because
the custom `chrome` scale begins at `50`; the surface is distinct from the
neutral chrome ramp.

## Chrome is structural, not a status

`chrome` is the neutral family for the application frame: page backgrounds,
surfaces, borders, headings, muted text, dividers, and metadata. Use a role
such as `info`, `success`, `warn`, or `danger` only when the UI communicates
that meaning.

The semantic scale must be mapped in every production theme scope. In
particular, a light-scoped subtree must receive the light chrome mapping even
when it sits inside a dark parent; otherwise it inherits the parent’s neutral
ramp. That mapping belongs in `foundation/theme.css`, alongside the existing
light and dark `--cad-color-*` values.

Components then need only structural tokens:

```css
.cda-surface {
  border-color: var(--cad-color-border);
  background: var(--cad-color-surface);
  color: var(--cad-color-text);
}
```

Do not use `bg-white`, `border-slate-*`, or a mode-specific component selector
for reusable application surfaces.

## Role colours belong in production tone tokens

The raw semantic scale is useful for making a coherent palette. Components
should still consume the existing `--cad-tone-*` contract.

```css
/* In the appropriate production theme scope. */
--cad-tone-info-soft-bg: var(--color-info-50);
--cad-tone-info-soft-hover: var(--color-info-100);
--cad-tone-info-soft-border: var(--color-info-300);
--cad-tone-info-soft-fg: var(--color-info-800);

.cda-status-check-list__icon--info {
  border-color: var(--cad-tone-info-soft-border);
  background: var(--cad-tone-info-soft-bg);
  color: var(--cad-tone-info-soft-fg);
}
```

This is the same pattern already used by the production status checklist,
badges, and buttons. The dark theme changes the token values once; it does not
add a dark selector to every alert, badge, or button variation.

Include every visible state in the contract: solid background, foreground,
hover, soft background, soft foreground, soft border, focus ring, disabled
state, and selected or pressed state. Do not assume a solid button always has
white text; retain `--cad-tone-*-solid-fg` as the source of truth.

## HTML: use layout classes freely; use colours intentionally

Layout and typography mechanics can stay visible in HTML. They describe local
composition, not the theme contract.

```html
<div class="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
  <!-- cards -->
</div>
```

For one-off content, semantic colour utilities are useful and readable:

```html
<p class="text-chrome-600">Supporting description</p>
<p class="text-info-700">Informational label</p>
```

When the colour must follow the active theme’s resolved component token,
Tailwind’s arbitrary-value syntax can consume that token directly:

```html
<p class="text-[var(--cad-tone-info-soft-fg)]">Theme-aware info label</p>
```

Use this sparingly for local content. A reusable component should keep that
decision in its CSS and consume `--cad-color-*` or `--cad-tone-*` there.

Avoid raw palette utilities for ordinary application structure. They freeze
light/dark assumptions into markup and bypass the production token contract:

```html
<!-- Avoid for theme-aware application UI -->
<p class="text-slate-600">Supporting description</p>
<div class="border-slate-200 bg-white">...</div>
```

Custom components have states and reuse, so their CSS should use
`--cad-color-*` and `--cad-tone-*` variables rather than either raw palette
utilities or semantic scale steps. Keep focus indicators semantic and visibly
contrasting in both production themes.

## Native form controls

The demo uses [@tailwindcss/forms](https://github.com/tailwindlabs/tailwindcss-forms)
as an opt-in reset with its `class` strategy. Markup composes the plugin base
with the semantic CAD class:

```html
<input class="form-input cad-input cad-input--outlined">
<select class="form-select cad-select cad-select--filled">...</select>
<input class="form-checkbox cad-checkbox cad-checkbox--outlined" type="checkbox">
```

`cad-input`, `cad-select`, and `cad-checkbox` own the theme-aware surface,
border, text, and focus treatments. The forms plugin emits its `.form-*`
classes outside cascade layers, so CAD overrides must also remain unlayered and
appear later in the stylesheet; a rule in `@layer components` loses to the
plugin regardless of source order. Do not solve this with `!important`.

## Typography proof

The experiment loads **Noto Sans** and **Noto Sans Mono** from Google Fonts,
then assigns them to Tailwind’s `--font-sans` and `--font-mono` theme tokens.
That makes `font-sans` and `font-mono` available to the demo and ensures code
within `@tailwindcss/typography` uses the same mono face.

The typography plugin is enabled in `input-test.css`. Its full normal prose
contract is defined explicitly in both the light and dark `data-cad-theme`
scopes: `--tw-prose-body`, headings, lead, links, bold text, lists, rules,
quotes, captions, keyboard keys, inline and block code, and table borders.
They resolve from the local `chrome` scale; dark mode remaps the ordinary
variables directly, including the code-block foreground and background.

Consequently content needs only `class="prose"` (and normally
`max-w-none` inside a card). Do not add `prose-invert`: that modifier activates
the plugin’s separate invert variable family, which this prototype deliberately
does not use. The typography section in [`test.html`](./test.html) includes
light/dark-safe specimens for prose, headings, links, lists, blockquotes,
inline code, code blocks, and tables.

## Material Symbols Rounded proof

The demo loads the [Material Symbols Rounded variable font](https://fonts.googleapis.com/css2?family=Material+Symbols+Rounded:opsz,wght,FILL,GRAD@20..48,100..700,0..1,-50..200)
from Google Fonts in `test.html`. Use the `material-symbols-rounded` class with
the icon's ligature name as its text content. It defaults to an outlined,
300-weight, 24px optical-size glyph; add
`material-symbols-rounded--filled` for the filled variant.

Set an icon's rendered size directly in markup with a Tailwind text-size
utility, such as `text-2xl` or `text-6xl`, and set the font's optical size
alongside it with `[--cad-symbol-optical-size:24]` or
`[--cad-symbol-optical-size:48]`. The only icon-specific CSS is the font
primitive and its fill modifier; gallery layout stays in the proof markup.

`--cad-symbol-grade` supplies the theme-specific `GRAD` axis value without
increasing `wght`, so glyph proportions remain stable while perceived stroke
weight can be tuned per theme.

## JavaScript and canvas consumers

CSS custom properties are not a practical palette API for canvas, WebGL, or
other JavaScript rendering code. The same original colours therefore need a
shared configuration source that can produce both the `@theme` definitions and
a JavaScript/TypeScript module (or a generated JSON asset). JavaScript should
read that shared palette, not repeat hex values in switches.

For example,
[`06-canvas-renderers.js`](../src/CanDoItAll.Components.CanvasLib/wwwroot/js/runtime/workbench/06-canvas-renderers.js)
currently resolves marker accents from literals. The target shape is:

```js
import { markerAccentByTone } from "./theme-colors.js";

function resolveMarkerToneAccentColor(tone, fallbackAccent) {
  return markerAccentByTone[(tone || "").toLowerCase()] ?? fallbackAccent ?? markerAccentByTone.accent;
}
```

`theme-colors.js` should be generated from, or share the canonical data with,
the CSS palette source. It can expose the exact colour needed by the renderer,
while CSS continues to derive theme-specific surface and text treatments. The
same audit should cover the palette switches in
[`canvasBenchmarkPage.js`](../samples/CanDoItAll.Components.Sandbox/wwwroot/js/canvasBenchmarkPage.js).

## Sandbox migration candidates

The Sandbox contains direct `slate` colours that will not respond to the
production theme contract today. These are migration candidates, not a request
to mechanically replace every colour:

- [`ThemeHostComparison.razor`](../samples/CanDoItAll.Components.Sandbox/Components/Examples/Layout/ThemeHostComparison.razor)
  hard-codes both the
  light and dark cards (`bg-white`, `bg-slate-900`, and `text-slate-*`). It is
  the most useful first proof route for semantic production tokens.
- Layout samples including
  [`SidebarToggle.razor`](../samples/CanDoItAll.Components.Sandbox/Components/Examples/Layout/SidebarToggle.razor),
  [`PageShellMiniShell.razor`](../samples/CanDoItAll.Components.Sandbox/Components/Examples/Layout/PageShellMiniShell.razor),
  [`LayoutFitViewport.razor`](../samples/CanDoItAll.Components.Sandbox/Components/Examples/Layout/LayoutFitViewport.razor),
  and [`BodyScrollable.razor`](../samples/CanDoItAll.Components.Sandbox/Components/Examples/Layout/BodyScrollable.razor)
  use `slate` for
  ordinary surfaces, borders, and copy. Those should move to production
  chrome/surface/text tokens.
- Form samples such as
  [`ReminderSwitch.razor`](../samples/CanDoItAll.Components.Sandbox/Components/Examples/Forms/ReminderSwitch.razor),
  [`SwitchLabelVariants.razor`](../samples/CanDoItAll.Components.Sandbox/Components/Examples/Forms/SwitchLabelVariants.razor),
  [`ApprovalCheckboxes.razor`](../samples/CanDoItAll.Components.Sandbox/Components/Examples/Forms/ApprovalCheckboxes.razor),
  and [`MusicXmlUpload.razor`](../samples/CanDoItAll.Components.Sandbox/Components/Examples/Forms/MusicXmlUpload.razor)
  use `bg-white` and
  `border-slate-200`; reusable control framing should use structural tokens.
- [`BenchmarkPrototypeResults.razor`](../samples/CanDoItAll.Components.Sandbox/Components/Examples/Canvas/BenchmarkPrototypeResults.razor)
  mixes regular table UI with a
  `bg-slate-950` canvas host. Migrate the table chrome, but retain an explicit
  dark visual treatment for the canvas only if that is content of the preview,
  rather than application chrome.
- Purposeful branded previews, gradients, and data visualisations are not
  automatically chrome. Examples include
  [`ShellCustomization.razor`](../samples/CanDoItAll.Components.Sandbox/Components/Examples/Navigation/ShellCustomization.razor)
  and canvas previews; classify their visual intent before changing them.

Generated `wwwroot/css/output-*.css` files merely reflect source usage and are
not migration targets.

## Layout flexibility proof

The demo contains three neutral cards with the same content and only their
padding utilities changed. This is intentional: layout flexibility stays in
HTML, while the card’s surface, border, and text remain component tokens.

```html
<article class="demo-card">...</article>
<article class="demo-card p-8">...</article>
<article class="demo-card p-12">...</article>
```

## Suggested next steps

1. Agree the semantic scales and contrast mappings in this demo.
2. Map those scales to the existing `--cad-color-*` and `--cad-tone-*-*`
   contract in [`foundation/theme.css`](../Tailwind/foundation/theme.css).
3. Convert reusable production components to the production tokens where they
   still use raw colour values.
4. Migrate Sandbox proof routes from structural `slate`/`white` utilities to
   chrome and surface tokens, preserving intentional visual-content colours.
