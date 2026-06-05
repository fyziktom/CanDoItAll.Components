# No Economy Modification Rule

This bundle intentionally targets only `CanDoItAll.Components`.

Economy is the main consumer that forced discovery of missing generic capabilities, but this phase is a stabilization/freeze wave for Components. Do not edit Economy code, scenarios, tests, or documentation while executing this bundle.

Allowed references to Economy during this bundle:

- Mentioning it in boundary documentation as a consuming package.
- Using its vocabulary only in forbidden-term fixtures or domain-leakage tests.
- Explaining why a requested feature belongs in a domain driver rather than Components.

Disallowed:

- Adding Economy-specific code or constants to Components.
- Adding Economy project/package references to Components.
- Adding test fixtures that require Economy packages.
- Widening generic APIs solely to make a specific Economy scenario easier.
