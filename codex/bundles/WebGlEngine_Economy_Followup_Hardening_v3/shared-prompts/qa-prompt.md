# Shared QA Prompt

Review the current subbundle as a senior QA inspector.

Reject closure if:
- proof transcripts are empty or command-only;
- only screenshots exist without diagnostics/assertions;
- tests seed production-only signals manually;
- browser apply can mutate a stale scene after reset failure;
- package proof uses stale global NuGet cache;
- runtime UI depends on tests directory paths;
- generic Components code contains economy/ledger/market/production-line semantics.
