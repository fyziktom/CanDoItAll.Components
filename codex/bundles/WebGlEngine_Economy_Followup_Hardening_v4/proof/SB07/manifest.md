# SB07 proof manifest

Status: completed

## Semantic assertion

Economy SimulationSandbox now exposes async-first session APIs for load, scenario load, project, snapshot, export, and import; library persistence does not use sync-over-async; session exports include portable source metadata, scenario/input pack hashes, and snapshot references; import restores from a catalog scenario source before falling back to legacy path fields and validates source/hash/snapshot state before returning.

## Required proof artifacts

- `transcripts/failing-first.txt`
- `transcripts/passing-focused-tests.txt`
- `transcripts/passing-tests.txt`
- `transcripts/source-assertions.txt`
- `transcripts/boundary-audit.txt` if a package boundary is touched
- `transcripts/validator-audits.txt`
- `changed-file-hashes.md`

## Results

- Failing-first tests: `transcripts/failing-first.txt` failed at compile/API level because async load/project/snapshot methods and portable export fields did not exist.
- Passing focused tests: `transcripts/passing-focused-tests.txt` passed 3/3 SB07 tests for sync-over-async scan, async portable catalog round-trip, and legacy export compatibility.
- Passing session class: `transcripts/passing-tests.txt` passed 11/11 session tests, including existing async persistence, moved-catalog import, and new portable export/import coverage.
- Source assertions: `transcripts/source-assertions.txt` proves async APIs, portable export fields, catalog-source import preference, no sync-over-async matches, and SB07 tests.
- Boundary audit: `transcripts/boundary-audit.txt` records Economy-only changes and no generic Components package edits.

## Completion rules

This manifest cannot be marked completed unless all required proof files are non-empty and cite the command, result, and semantic assertion.
