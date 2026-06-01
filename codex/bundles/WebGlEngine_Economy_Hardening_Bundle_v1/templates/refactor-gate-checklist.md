# Mandatory Refactor Gate Checklist

Run this checklist before closing every subbundle.

| Check | Pass/Fail | Notes |
| --- | --- | --- |
| Every touched source file was reread after implementation |  |  |
| No fixture-only branches were introduced |  |  |
| No TODO/NotImplemented production paths remain |  |  |
| No lower-layer package references a higher-layer package |  |  |
| Duplicate C# and JS behavior is either intentionally mirrored with parity tests or centralized |  |  |
| Public DTO/API changes have docs and tests |  |  |
| Browser-visible changes have browser proof or explicit blocker |  |  |
| Critical proof manifest and semantic invariants exist where required |  |  |
