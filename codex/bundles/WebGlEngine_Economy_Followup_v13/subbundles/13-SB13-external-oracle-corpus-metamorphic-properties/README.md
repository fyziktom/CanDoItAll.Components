# SB13 - External oracle corpus and metamorphic properties

Expand external oracle coverage.

Tasks:
- Move/duplicate oracle expectations into JSON corpus under tests/fixtures.
- Add corpus loader and schema.
- Add metamorphic tests:
  - increasing investment changes dependency/concentration metrics predictably,
  - increasing fee changes liquidity/admin burden predictably,
  - conservation holds across transfer-only variations,
  - store resolution changes only when policy changes.
- Keep oracle expectations independent from implementation code.

Required proof:
- broken oracle negative test,
- corpus schema validation,
- canary metamorphic results.

