You are working in two already checked-out repositories:

- CanDoItAll.Components
- CanDoItAll.Economy

Do not create a new branch. Work in the currently checked-out branch in each repo.

Implement the follow-up hardening bundle `CanDoItAll_WebGl_Economy_BrowserJoinHardeningBundle_v20`.

Main goals:
1. Keep Components generic and Economy-free.
2. Keep the joined simulation + visualization implementation in Economy.
3. Harden the executable WebGlRun browser apply loop.
4. Add a minimal desktop-only Economy sandbox page/smoke proof.
5. Harden real scenario artifacts and readiness reporting.
6. Add session/snapshot persistence hooks.
7. Keep shared-resource and finite-resource probes generic.
8. Do not optimize WebGL for small/medium/mobile/tablet screens.
9. Do not migrate JavaScript to TypeScript.
10. All source code comments must be in English.

Before coding:
- Read every file in this bundle.
- Use `05_spreadsheets/implementation_matrix.xlsx` as the execution matrix.
- Keep proof transcripts non-empty.
- Update execution reports with real commands and real outputs.

After coding:
- Run all validation commands.
- Write a concise execution report.
- Include a readiness answer: headless test, browser smoke, or full UI demo.
