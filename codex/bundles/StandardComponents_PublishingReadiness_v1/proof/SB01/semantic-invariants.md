# SB01 Semantic Invariants

## Invariant SB01-INV-001

- Invariant ID: `SB01-INV-001`
- Source raw note: RAW02 says "You must do detailed study of the actual implementation"; RAW03 says "focus in this bundle only to parts with standard components and not webgl and canvas"; RAW07 says "map all in xlsx with correct references and explanations."
- Expected behavior: The bundle execution starts from a regenerated inventory that includes standard component libraries, Tailwind inputs, sandbox coverage, and old AppComponents duplicates, while excluding WebGL and Canvas implementation work from planned code edits.
- Disallowed shallow implementation: A hand-written summary that names a few components but does not scan source files, count standard rows, count old AppComponents rows, or preserve references to the generated xlsx.
- Failing-first test: N/A process/non-production inventory verification; no production behavior was changed in SB01, so the adversarial check is the verifier rejecting inventories below required minimum counts.
- Passing test: `bundle://proof/SB01/transcripts/sb01-verifier.txt` includes `SB01-INV-001 components=268`, `appComponents=46`, `standardMatches=39`, and `tailwindFiles=18`.
- Changed source files: `bundle://scripts/build-inventories.mjs` SHA-256 `ACA30604A6F0E4B304CE8974653692EFE2F9F72CCA8EAFF24A3750CA3B0144AC`; `bundle://scripts/verify-sb01.mjs` SHA-256 `45DA719D1C71E400537454B51C220BC691F6871BA3183E35BA5FC4D515CCF5CD`; `bundle://scripts/verify-sb01-downstream.mjs` SHA-256 `56B56361477309EDCF1024DFC44089F9F8F785CB13AA791B9710F830BE7C1593`.
- Production assertions: SB01 made no production component behavior changes; source assertion proof is in `bundle://proof/SB01/transcripts/sb01-verifier.txt`.
- Red-team negative case: The verifier exits non-zero if standard component rows are below 200, AppComponents rows below 40, Tailwind rows below 10, standard matches below 30, workbook bytes below 10000, preview count below 6, or the formula scan does not report zero matches.
- Downstream dependency check: `bundle://proof/SB01/transcripts/sb01-downstream-smoke.txt` proves required SB02 Tailwind source references exist and 13 Tailwind review rows are available.

## Invariant SB01-INV-002

- Invariant ID: `SB01-INV-002`
- Source raw note: RAW07 says "You must map all in xlsx with correct references and explanations."
- Expected behavior: The xlsx workbook exists, has rendered preview proof for all major sheets, and has a formula-error scan with zero matches.
- Disallowed shallow implementation: Creating an empty or tiny workbook file without inspecting rendered sheets or checking formula errors.
- Failing-first test: N/A process/non-production workbook artifact verification; no production behavior was changed in SB01, so the adversarial check is the verifier rejecting a missing, tiny, unrendered, or formula-error workbook.
- Passing test: `bundle://proof/SB01/transcripts/sb01-verifier.txt` includes `SB01-INV-002 workbookBytes=42231`, `previewCount=6`, and `formulaErrorScan=matched 0 entries`.
- Changed source files: `bundle://inventories/current-state-data.json` SHA-256 `895FB8C5EEB600F7F2DF8D8C70CD247C76371C380CEAF17A85D5F72FC463387C`; `bundle://inventories/standard-components-publishing-map.xlsx` SHA-256 `4E9B70AFB1C93EB5D37615885381125C4070F5F7F464D001741E3573A3DDA2EA`.
- Production assertions: SB01 made no production component behavior changes; workbook artifact assertions are in `bundle://proof/SB01/transcripts/sb01-verifier.txt`.
- Red-team negative case: The verifier rejects missing xlsx output, workbook output below 10000 bytes, fewer than 6 preview PNGs, or a formula-error scan not containing "matched 0 entries."
- Downstream dependency check: Later subbundles use `bundle://inventories/standard-components-publishing-map.xlsx` and `bundle://inventories/current-state-data.json` as their row maps.
