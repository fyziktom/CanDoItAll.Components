# SB14 - Design matrix and result comparison hardening

Harden experiment comparison.

Tasks:
- factor binding registry with supported binding kinds and proof that each factor changes effective
  input hash when intended.
- result comparability report: same driver hash, same metric registry version, same oracle corpus,
  compatible scenario class, compatible performance profile.
- mark not-comparable instead of failed when performance/noise prevents comparison.

Required proof:
- no-effect factor negative test,
- design matrix hash test,
- comparison mismatch test,
- multi-goods canary comparison run.

