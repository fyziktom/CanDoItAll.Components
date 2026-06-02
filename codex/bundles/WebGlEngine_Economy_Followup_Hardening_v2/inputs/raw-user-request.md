# Raw user request

Codex už bundle dodělal a oboje je pushnuté. Je potřeba zkontrolovat, jak to provedl v `CanDoItAll.Components` (`webgl-engine`) a `CanDoItAll.Economy`, pravděpodobně připravit další hardening/refaktoring, udělat důkladnou analýzu a připravit follow-up bundle.

Primary intent:
- Review the implemented WebGL engine + Economy simulation integration after the previous hardening bundle.
- Identify remaining architectural, runtime, packaging, validation, browser-proof, and maintainability gaps.
- Prepare a new CanDoItAll Workflow Bundle that Codex can execute phase by phase across both repositories.
