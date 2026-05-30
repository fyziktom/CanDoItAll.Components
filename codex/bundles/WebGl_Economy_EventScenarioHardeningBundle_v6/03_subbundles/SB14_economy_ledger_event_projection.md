# SB14 - Economy: Ledger adapter event projection

Ledger adapter currently prepares frames from minimal projection data. Extend it to emit backend-neutral events from ledger evidence without referencing SimpleAccounts.

Add:

- `LedgerSimulationEventProjector`
- mapping from ledger evidence to `SimulationEvent`:
  - transfer evidence -> resource-transfer;
  - issue evidence -> issue-raised;
  - rule execution evidence -> rule-applied;
  - balance change evidence -> store-changed.

Keep ledger-specific evidence in artifacts/metadata. Do not leak ledger UTXO details into generic event fields except through metadata and artifacts.
