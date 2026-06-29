# SB02 Semantic Invariants

## SB02-INV-001 Tailwind Utility Composition

- Invariant ID: `SB02-INV-001`
- Source raw note: RAW05 requires Tailwind-first component styling, with raw CSS retained only where it is token/state/browser-required behavior.
- Expected behavior: general layout declarations touched by SB02 are represented with Tailwind composition where safe in fields, buttons, theme host, and tabs Tailwind inputs.
- Disallowed shallow implementation: keep simple layout/sizing/alignment declarations as ad hoc raw CSS while claiming Tailwind ownership, or convert token/state/browser-specific CSS that should stay raw.
- Failing-first test: SB01 Tailwind review identified raw layout declarations that should be converted before downstream component hardening.
- Passing test: `bundle://proof/SB02/transcripts/sb02-verifier.txt` prints `SB02-INV-001`.
- Changed source files: `repo://Tailwind/forms/fields.css` SHA-256 `FEB683F7B74660974EE0D97636C2F1061732AFE4F43A1151E11DE648D31139CF`; `repo://Tailwind/controls/buttons.css` SHA-256 `2F15B7829656A10D4FE3826EA4DAD83CE29614491D6D13F8BB0CD5ED4AF22D3F`; `repo://Tailwind/foundation/theme.css` SHA-256 `41E52F83919BA4F1559D26A4D2E7ADC876ACFA452D5351E65E07A066FBB33DF5`; `repo://Tailwind/navigation/tabs.css` SHA-256 `EE6DF3B91D493BDD4C1998DF5ECE8CC6828A92D7FA2A199CCA4C9D3E71B0B3D8`.
- Production assertions: `bundle://proof/SB02/transcripts/sb02-source-assertions.txt` and the verifier prove the targeted layout declarations are Tailwind `@apply` owned while raw CSS rationale stays documented.
- Red-team negative case: replacing those declarations with raw layout CSS or removing policy coverage fails `verify-sb02.mjs`.
- Downstream dependency check: SB06-SB09 can build component hardening on the Tailwind policy and shared styling foundation.

## SB02-INV-002 Mobile Layout Hardening

- Invariant ID: `SB02-INV-002`
- Source raw note: RAW10 requires real visual proof for wrapping, clipping, and available-space behavior.
- Expected behavior: `SelectionListItem` action areas wrap and stack full-width on mobile, and vertical tabs stack with readable panel width below the mobile breakpoint.
- Disallowed shallow implementation: hide overflow, test only desktop, or fix the sandbox page while leaving shared component layout brittle.
- Failing-first test: baseline visual data showed mobile actions and navigation tabs overflow pressure before repair.
- Passing test: `bundle://proof/SB02/transcripts/sb02-verifier.txt` prints `SB02-INV-002`, and `bundle://proof/SB02/transcripts/sb02-playwright-visual-strict.txt` passes after repairs.
- Changed source files: `repo://src/CanDoItAll.Components.BaseLib/Components/Lists/SelectionListItem.razor` SHA-256 `9DAD51BBDEA8385B3022239EEE34E007608E36E33E20904CCEA10386B897074B`; `repo://Tailwind/navigation/tabs.css` SHA-256 `EE6DF3B91D493BDD4C1998DF5ECE8CC6828A92D7FA2A199CCA4C9D3E71B0B3D8`; `repo://src/CanDoItAll.Components.BaseLib/wwwroot/css/output.css` SHA-256 `2914BF1071BEBEC4C3052053F907C90E3669168D676F4A344BA92DD5171E3E3D`.
- Production assertions: after repair, sandbox route `groups/actions` reports `pageHorizontal=false`, `clipped=0`, `viewport=0`, and route `groups/navigation/tabs` reports `pageHorizontal=false`, `clipped=0`.
- Red-team negative case: the strict Playwright verifier rejects document overflow, clipped content, and missing interaction states.
- Downstream dependency check: SB05 and later visual hardening can use actions/tabs as proof routes without carrying known mobile overflow debt.

## SB02-INV-003 Real Visual Proof

- Invariant ID: `SB02-INV-003`
- Source raw note: RAW10 explicitly requires Playwright MCP screenshots and action-state proof instead of estimating styling fixes.
- Expected behavior: SB02 closure includes baseline screenshots/data before repair, after screenshots/data for desktop and mobile, copy-button click state, tabs selected-state switching, and strict verifier proof.
- Disallowed shallow implementation: close from source inspection only, capture screenshots without comparing baseline/after, or skip interactive state proof.
- Failing-first test: baseline visual data `bundle://proof/SB02/data/sb02-visual-baseline.json` records mobile overflow before repair.
- Passing test: `bundle://proof/SB02/transcripts/sb02-playwright-visual-strict.txt` and `bundle://proof/SB02/transcripts/sb02-verifier.txt` print `SB02-INV-003`.
- Changed source files: `bundle://scripts/verify-sb02-visual.mjs` SHA-256 `C0548A54DF70E46606152568EAE730D1C28BB0A20C51593AB714BA02C3C26354`; `bundle://proof/SB02/data/sb02-visual-after.json` SHA-256 `60E88F87D79EE571E782D433CDB74E8C69CB7A0E5AE058864E4683176C0316A8`.
- Production assertions: after capture covers sandbox routes `groups/inputs`, `groups/actions`, and `groups/navigation/tabs` at `1366x900` and `390x844`; MCP evidence is stored in `bundle://proof/SB02/screenshots/mcp`.
- Red-team negative case: missing screenshots, non-MCP evidence, or after-capture overflow fails the visual verifier.
- Downstream dependency check: later subbundles can rely on the strict visual verifier pattern and Tailwind policy for closure gates.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| Tailwind policy and generated CSS | `repo://Tailwind/forms/fields.css`, `repo://Tailwind/controls/buttons.css`, `repo://Tailwind/foundation/theme.css`, and `repo://Tailwind/navigation/tabs.css` own the Tailwind composition changes. | Components consume regenerated CSS at `repo://src/CanDoItAll.Components.BaseLib/wwwroot/css/output.css`. | `bundle://proof/SB02/transcripts/sb02-tailwind-build.txt` and `bundle://proof/SB02/transcripts/sb02-dotnet-build.txt` prove generated output and build validity. | `bundle://scripts/verify-sb02.mjs` rejects missing policy/source changes and visual verifier rejects overflow. |
| Mobile actions/tabs visual states | `repo://src/CanDoItAll.Components.BaseLib/Components/Lists/SelectionListItem.razor` and `repo://Tailwind/navigation/tabs.css` produce the layout repairs. | Sandbox routes `groups/actions` and `groups/navigation/tabs` consume the repairs. | `bundle://proof/SB02/transcripts/sb02-playwright-visual-strict.txt` proves baseline/after and interactions. | Baseline overflow cases are retained in `bundle://proof/SB02/data/sb02-visual-baseline.json` as rejected shallow states. |

## Semantic Gate Decision

Pass. SB02 includes Tailwind policy/source changes, generated CSS, baseline and after visual proof, Playwright MCP screenshots, build/verifier transcripts, and anti-stub audit proof.
