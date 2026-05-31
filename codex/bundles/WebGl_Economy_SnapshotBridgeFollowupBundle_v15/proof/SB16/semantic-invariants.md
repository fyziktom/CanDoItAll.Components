# Semantic invariants SB16

Status: Completed.

Invariant ID: SB16-final-closure-integrity

SB16 closes only when every subbundle has a completed or explicitly blocked proof manifest, raw notes are closed with evidence, browser analytics are documented, and the completed-stage validator passes against the bundle artifacts.

## Shallow-Pass Trap

Shallow-pass trap: SB16 rejects closure based only on prose summaries while proof manifests, traceability rows, or validator artifacts remain stale.

## Adversarial Negative Proof

Adversarial negative proof: `bundle://proof/SB16/transcripts/completed-validator-first-pass.txt` proves the completed validator caught stale rows, missing literal proof tokens, and the missing final fake-proof artifact before closure.

## Semantic Positive Proof

Semantic positive proof: `bundle://proof/SB16/transcripts/completed-validator-final.txt` proves the final bundle artifacts satisfy the completed-stage validator after report, traceability, proof manifest, and fake-proof-resistance updates.

## Anti-Stub Audit

Anti-stub audit: `bundle://proof/SB16/source-assertions/final-proof-path-existence-check.txt` verifies referenced proof files exist, and `bundle://proof/SB16/final-fake-proof-resistance.md` records why the closure is backed by executable transcripts and source assertions rather than placeholder text.
