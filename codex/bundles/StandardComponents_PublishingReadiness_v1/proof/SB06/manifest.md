# SB06 Proof Manifest - Forms And Inputs Behavior Visual Hardening

Status: `Passed`  
Completed local date: `2026-06-28`

## Production Changes

- Hardened form input behavior in `TextBox`, `TextArea`, `Numeric`, `DropDown`, `CheckBox`, `Switch`, `Slider`, `Password`, and `SecretField` so disabled/read-only controls do not raise stale callbacks and width classes include `min-w-0` where needed.
- Added FormField label cascade support to `Switch`, `Slider`, `Password`, `SecretField`, and `TagEditor`.
- Hardened `TagEditor` with root attribute merging, class/style compatibility, accessible naming, disabled remove guard, truncating chips, constrained suggestions, and focused sandbox coverage.
- Hardened `EntityPicker` with Tailwind-owned styling, disabled whole-component behavior, literal ARIA boolean strings, accessible listbox naming, and focused sandbox coverage.
- Reworked `PrefixedField` from fragile overlay padding into a structural prefix/input/suffix add-on layout after screenshots showed the prefix was hidden.
- Expanded `/groups/inputs` with live coverage for Slider, TagEditor, EntityPicker, PrefixedField, SettingsSwitchRow/Label, Fieldset, FormRow, FormStack, InlineActions, and FileUpload.

## Proof Artifacts

- Build transcript: `bundle://proof/SB06/transcripts/sb06-sandbox-build.txt`
- Tailwind rebuild transcript: `bundle://proof/SB06/transcripts/sb06-tailwind-build.txt`
- BaseLib tests: `bundle://proof/SB06/transcripts/sb06-baselib-tests.txt`
- Browser verifier transcript: `bundle://proof/SB06/transcripts/sb06-playwright-verifier.txt`
- Browser verifier JSON: `bundle://proof/SB06/data/sb06-input-validation.json`
- Source assertions: `bundle://proof/SB06/transcripts/sb06-source-assertions.txt`
- Git whitespace check: `bundle://proof/SB06/transcripts/sb06-git-diff-check.txt`
- Prepared-stage validator: `bundle://proof/SB06/transcripts/sb06-prepared-validator.txt`
- File hashes: `bundle://proof/SB06/transcripts/sb06-file-hashes.txt`
- Accessibility snapshot: `bundle://proof/SB06/transcripts/sb06-inputs-desktop-snapshot.md`
- Verifier script: `bundle://scripts/verify-sb06-inputs.mjs`

## Screenshots

- Final desktop full-page proof: `bundle://proof/SB06/screenshots/mcp/sb06-inputs-desktop-full-v3.png`
- Final mobile long-text proof: `bundle://proof/SB06/screenshots/mcp/sb06-inputs-mobile-long-text-full-v3.png`
- Final TagEditor suggestions/open-state proof: `bundle://proof/SB06/screenshots/mcp/sb06-inputs-tag-suggestions-v3.png`
- Disabled mobile proof: `bundle://proof/SB06/screenshots/mcp/sb06-inputs-mobile-disabled-full.png`
- Repair trail retained: v1/v2 screenshots show the intermediate visual review loop that found and fixed PrefixedField and EntityPicker layout issues.

## Validation Summary

- `dotnet build src/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj --no-restore`: passed, 0 warnings, 0 errors.
- `npm --prefix Tailwind run build`: passed.
- `dotnet test tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj --no-restore`: passed, 15 tests.
- `node codex/bundles/StandardComponents_PublishingReadiness_v1/scripts/verify-sb06-inputs.mjs`: passed, 22 checks, 0 console errors.
- `git diff --check`: no whitespace errors on stdout; line-ending warnings captured separately.
- `python .../scripts/validate_bundle.py ... --stage prepared`: passed.
- Playwright MCP screenshots were captured and visually reviewed at 1366x900 and 390x844, including long-text, disabled, and TagEditor open suggestions.

## Visual Findings Repaired

- PrefixedField prefix was visually hidden behind the input background in the first MCP screenshot; replaced overlay styling with a visible add-on control layout.
- EntityPicker had no Tailwind-owned styling and cramped meta placement on narrow layouts; added component CSS and moved meta badges out of the label line on small widths.
- EntityPicker originally rendered boolean ARIA values as empty attributes; fixed to emit literal `true` / `false` and verified with Playwright.

## Closure Decision

SB06 is closed. Downstream SB07-SB09 may rely on the hardened form foundation, the expanded inputs sandbox route, and the repeatable verifier pattern.
