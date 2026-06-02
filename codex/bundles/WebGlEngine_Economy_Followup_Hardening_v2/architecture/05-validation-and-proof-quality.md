# Validation and proof quality rules

## Proof quality floor

A subbundle cannot close a critical finding with only:

- grep/source assertions;
- non-empty diagnostic counters;
- build success;
- screenshots without route/action assertions;
- generated proof files that are empty or not cited by a manifest.

## Required proof types

- Failing-first proof for critical behavior changes.
- Semantic positive proof that exercises production code paths.
- Negative proof for shallow/stub/fake implementations.
- Browser proof for UI/runtime behavior.
- Package proof with isolated NuGet cache and fresh local package feed.
- Manifest with changed-file hashes and transcript paths.

## Proof hygiene

Each proof transcript referenced by a manifest must be:

- present;
- non-empty unless the manifest explicitly states why an empty transcript is expected;
- tied to an executed command or browser action;
- readable enough for a reviewer to understand the result.
