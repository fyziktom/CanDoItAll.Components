# Requirement Traceability

| Raw Note | Exact Input Wording | Requirements | Owning Subbundles | Planned Proof |
| --- | --- | --- | --- | --- |
| RAW01 | Preparation of repository for publishing. | R01,R13 | SB01,SB12 | Scope freeze plus final transfer readiness report. |
| RAW02 | Detailed study of actual implementation and identify refactoring/hardening. | R02,R03,R04,R06-R12 | SB01-SB12 | Inventory workbook, current-state analysis, subbundle proof. |
| RAW03 | Focus only on standard components, not WebGL and Canvas. | R01 | SB01,SB05,SB12 | Excluded scope files and sandbox split proof. |
| RAW04 | Analyze sandbox missing components and logical grouping. | R06 | SB05 | Coverage matrix and Playwright route plan. |
| RAW05 | Use Tailwind for component styling and inspect custom CSS/hacks. | R03 | SB02 | Tailwind metrics, refactor policy, screenshots. |
| RAW06 | Audit main CanDoItAll AppComponents duplicate basic components. | R05 | SB04 | Duplicate matrix and migration closure rows. |
| RAW07 | Map all in xlsx with correct references and explanations. | R02 | SB01 | standard-components-publishing-map.xlsx. |
| RAW08 | Identify phases; first general foundations like input Tailwind and base isolation. | R03,R04 | SB02,SB03 | Phase plan and critical subbundle gates. |
| RAW09 | Design subbundles with refactoring checkpoints. | R01-R13 | SB01-SB12 | Checkpoint A-D in phase plan. |
| RAW10 | Real Playwright MCP screenshots one by one, including interactive states. | R07-R12 | SB06-SB11 | Browser analytics and proof requirements. |
