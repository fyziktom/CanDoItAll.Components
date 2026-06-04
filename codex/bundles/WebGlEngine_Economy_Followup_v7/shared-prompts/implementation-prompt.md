# Implementation prompt

Execute subbundles in order. Work in both repositories as needed. Keep Components generic and Economy-specific interpretation in Economy.

For every subbundle:

1. write failing-first tests or proof script;
2. implement the smallest robust fix;
3. run focused tests;
4. produce proof artifacts;
5. update manifest and traceability;
6. stop at forced refactor gates.

Do not claim research readiness unless all strict, oracle, reproducibility, and settled-runtime gates pass.
