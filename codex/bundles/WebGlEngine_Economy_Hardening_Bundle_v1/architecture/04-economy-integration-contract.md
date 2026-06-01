# Economy Integration Contract

## Economy bridge responsibilities

`CanDoItAll.Economy.Simulation.WebGlBridge` maps economy visual abstractions into generic WebGlRunLib abstractions.

It may know:

- economy visual frame IDs;
- economy visual action IDs;
- economy node IDs;
- economy visual mapping definitions;
- source input pack hash;
- diagnostic fallback options.

It must output:

- `WebGlRunDocument`;
- initial `WebGlSceneModel`;
- generic frames/stages/actions;
- source provenance metadata;
- diagnostics for unresolved mapping or fallback.

## Strict mode

Default mode must be strict:

```text
TreatUnresolvedMappingAsError = true
AllowFallbackObject = false
AllowNoOpPoseFallback = false
AllowNoOpSymbolFallback = false
AllowDiagnosticFallback = false
```

In strict mode, unresolved subject/target node, unsupported action kind, missing pose mapping and missing symbol mapping must be errors.

## Diagnostic mode

Diagnostic fallback may be useful during scenario authoring, but it must be explicit and auditable.

Allowed only when options explicitly enable it:

- fallback object;
- no-op pose;
- no-op symbol;
- unsupported action mapped to wait.

Every fallback must carry metadata and validator warning.

## Generic scenario rule

The two current economy examples must be implemented as scenario providers/mapping definitions over generic simulation contracts. Do not add bridge branches that hard-code those examples.
