# SB02 Proof Manifest

## Scope

Subbundle: `02-tailwind-component-styling-foundation-hardening`

Inputs closed:

- RAW05: Tailwind-first component styling, with raw CSS retained only for token/state/browser-required behavior.
- RAW08: General foundation hardening before component-by-component visual phases.

## Source Changes

- `repo://Tailwind/forms/fields.css`
  - Converted prefixed-field mobile stacking and textarea layout declarations to Tailwind `@apply`.
  - Hash: `FEB683F7B74660974EE0D97636C2F1061732AFE4F43A1151E11DE648D31139CF`
- `repo://Tailwind/controls/buttons.css`
  - Converted copy-button layout, icon sizing, glyph leading, disabled state, and compact dimensions to Tailwind `@apply`.
  - Hash: `2F15B7829656A10D4FE3826EA4DAD83CE29614491D6D13F8BB0CD5ED4AF22D3F`
- `repo://Tailwind/foundation/theme.css`
  - Converted theme-host block/min-height layout to Tailwind `@apply`.
  - Hash: `41E52F83919BA4F1559D26A4D2E7ADC876ACFA452D5351E65E07A066FBB33DF5`
- `repo://Tailwind/navigation/tabs.css`
  - Added width constraints for tabs root/list/panels.
  - Converted simple scroll/list/tab text layout declarations to Tailwind `@apply`.
  - Fixed mobile vertical left/right tab specificity so vertical rails stack instead of squeezing panels.
  - Hash: `EE6DF3B91D493BDD4C1998DF5ECE8CC6828A92D7FA2A199CCA4C9D3E71B0B3D8`
- `repo://src/CanDoItAll.Components.BaseLib/Components/Lists/SelectionListItem.razor`
  - Allowed action area to wrap and stack full-width on mobile instead of clipping inline buttons.
  - Hash: `9DAD51BBDEA8385B3022239EEE34E007608E36E33E20904CCEA10386B897074B`
- `repo://src/CanDoItAll.Components.BaseLib/wwwroot/css/output.css`
  - Rebuilt Tailwind output.
  - Hash: `2914BF1071BEBEC4C3052053F907C90E3669168D676F4A344BA92DD5171E3E3D`
- `repo://docs/standard-components-tailwind-policy.md`
  - Codified Tailwind composition and raw CSS rationale rules.
  - Hash: `54DDF0898B31FF530B3CEF6BC6FF2B58447223C6E4CB6C3E6FE9CEA81894C95A`

## Semantic Contract

- `bundle://proof/SB02/semantic-invariants.md`

## Proof Scripts And Data

- `bundle://scripts/verify-sb02.mjs`
  - Hash: `0ABEFE78F07E993C02AFE71629C268BE22FAB4D88F5F532EDE06298DE02F34BD`
- `bundle://scripts/verify-sb02-visual.mjs`
  - Hash: `C0548A54DF70E46606152568EAE730D1C28BB0A20C51593AB714BA02C3C26354`
- Baseline visual data: `proof/SB02/data/sb02-visual-baseline.json`
  - Hash: `DCE17E4E487021979242324406C32BE50DC96839FD8406A6C6D623C77D11535A`
- After visual data: `proof/SB02/data/sb02-visual-after.json`
  - Hash: `60E88F87D79EE571E782D433CDB74E8C69CB7A0E5AE058864E4683176C0316A8`
- MCP screenshots:
  - `proof/SB02/screenshots/mcp/sb02-baseline-inputs-1366.png`
    - Hash: `8BA29E28C52BFDBAE7400DB8DCED43C8DAB2FC75527D1B77C3F53A8074DF68FE`
  - `proof/SB02/screenshots/mcp/sb02-after-tabs-390.png`
    - Hash: `11B2225A2C8CD119098B4846CA232A6AB4E41692189D7C32C4F9DB8FE4244BE4`

## Validation

- Passing transcript: `bundle://proof/SB02/transcripts/sb02-verifier.txt`.
- Failing-first: N/A process/non-production completed-stage proof normalization; SB02 baseline visual failures are documented below and in `bundle://proof/SB02/semantic-invariants.md`.
- Tailwind build: `proof/SB02/transcripts/sb02-tailwind-build.txt`
  - `ExitCode: 0`
- Clean .NET build: `proof/SB02/transcripts/sb02-dotnet-build.txt`
  - `ExitCode: 0`, `0 Warning(s)`, `0 Error(s)`
- Semantic verifier: `proof/SB02/transcripts/sb02-verifier.txt`
  - `SB02-INV-001`, `SB02-INV-002`, and `SB02-INV-003` passed.
- Strict Playwright visual verifier: `proof/SB02/transcripts/sb02-playwright-visual-strict.txt`
  - `ExitCode: 0`
  - Desktop and mobile screenshots captured for sandbox routes `groups/inputs`, `groups/actions`, and `groups/navigation/tabs`.
  - Action copy-button click state captured.
  - Tabs selected-state interaction captured.
- Source assertions: `proof/SB02/transcripts/sb02-source-assertions.txt`
- Anti-stub audit: `proof/SB02/transcripts/sb02-anti-stub-audit.txt`
  - No `TODO`, `NotImplemented`, explicit stub, or placeholder matches in changed production/docs/scripts.
- Prepared bundle validator: `proof/SB02/transcripts/sb02-prepared-validator.txt`
  - `ExitCode: 0`

## Visual Findings

Baseline red-team findings:

- Sandbox route `groups/actions` at `390x844`: inline row action buttons caused viewport overflow pressure (`viewportOverflows=3`).
- Sandbox route `groups/navigation/tabs` at `390x844`: vertical tabs and horizontal strips caused severe viewport overflow pressure (`viewportOverflows=44`).

After repair:

- Sandbox route `groups/actions` at `390x844`: `pageHorizontal=false`, `clipped=0`, `viewport=0`.
- Sandbox route `groups/navigation/tabs` at `390x844`: `pageHorizontal=false`, `clipped=0`; remaining viewport diagnostics are off-screen tab-list children inside intentional scroll strips, not document-level overflow.
- MCP after screenshot confirms the vertical tabs rail now stacks with readable panel width.

## Raw CSS Rationale

Raw CSS remains in Tailwind files only where it represents:

- theme tokens, CSS custom properties, and semantic colors;
- `color-mix()`, gradients, shadows, token-driven borders/backgrounds;
- focus, hover, disabled, ARIA, pseudo-element, and generated-content states;
- component API geometry expressed through CSS variables;
- browser-specific integration such as scrollbars and color-scheme.

Simple layout, sizing, alignment, text-leading, wrapping, and responsive declarations touched by this subbundle were moved to Tailwind composition.
