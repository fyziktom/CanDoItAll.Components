# SB05 - Economy bridge strict execution validation

Codex must harden `EconomyWebGlRunValidator` so strict mode fails when:

- a stage has no command and is not an explicit wait,
- a stage lacks source visual action id,
- a stage lacks source simulation frame id,
- a stage lacks input pack hash,
- a motion refers to an object not in initial scene,
- a patch refers to an object not in initial scene,
- a fallback object is used while fallback is disabled,
- no-op pose/symbol fallback is used while disabled.

The validator should report structured messages with path/code/severity.
