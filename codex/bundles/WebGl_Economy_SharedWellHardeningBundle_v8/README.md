# CanDoItAll WebGL + Economy Shared-Well Hardening Bundle v8

This bundle is a follow-up after Codex implemented the cross-repo WebGL run/action and Economy scenario/event/visual-action preparation.

## Core rule

Codex must not create a new branch. It must work in the branch that is already checked out in each repository.

## Repositories

- `CanDoItAll.Components`
  - Continue in the current branch, expected `webgl-engine`.
  - Keep WebGL runtime generic.
  - Keep WebGL validation desktop/large-screen only.
  - Do not optimize WebGL for small/medium/mobile/tablet screens.

- `CanDoItAll.Economy`
  - Continue in the current branch, expected current local working branch.
  - Keep Economy independent from WebGL/Components.
  - Prepare simulation scenario definitions, event streams, and visual intentions, but do not bridge directly to WebGL in this repository.

## Main goal

Harden the implementation enough to support a generic scenario such as a shared-well community:

- actors have homes and distances to shared resources;
- actors use/collect/transfer resources;
- closer actors may build inventory and trade with farther actors;
- rules create administrative actions, tax/fees, compliance, and conflict/trust changes;
- visualization expresses events as generic visual actions, not as domain-specific WebGL code.

The shared-well case is only a validation scenario. The architecture must remain generic.
