# SB12 - Simple simulation mutation and store-resolution split

Refactor high-risk mutation code.

Split responsibilities:
- store resolver,
- transfer applier,
- rejection/capacity policy,
- flow provenance writer,
- effect applier,
- diagnostic severity classifier.

Rules:
- no behavior change without golden test update,
- all refactoring must be covered by existing oracle cases,
- strict mode must fail ambiguity and hidden heuristic fallback.

Required proof:
- before/after golden oracle hash comparison where expected,
- focused store-resolution tests,
- strict-mode ambiguity negative tests.

