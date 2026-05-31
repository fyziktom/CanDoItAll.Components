# Farmer-land probe analysis

The farmer-land probe prevents overfitting to water/well semantics.

Probe scenario:

- finite land area;
- several farmers start with explicit land parcels, labor, capital, and productivity;
- external buyers may be unlimited or parameter-limited;
- farmers prefer expansion because higher output increases payoff;
- without rules, one actor may become dominant;
- institutions may impose land caps, progressive taxes, commons-protection fees, or auction rules;
- all initial placement, land parcels, actor parameters, buyer demand, rule parameters, and expected invariants are JSON inputs.

Generic capabilities needed:

- resource stores are not always consumable goods; some represent land, rights, permits, reputation, or debt;
- capacity and ownership rules differ by resource kind;
- trade may transfer rights, not just quantities;
- rules may constrain concentration, not just per-step use;
- metrics need distribution measures, such as maximum ownership share, Gini-like inequality score, unresolved demand, and welfare/throughput.

This probe should be used in validation only. It should not create a full UI demo in this wave.
