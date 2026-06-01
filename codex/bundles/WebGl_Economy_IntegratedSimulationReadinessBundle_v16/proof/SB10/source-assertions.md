# SB10 Source Assertions

- Visual mapping remains renderer-neutral until WebGlBridge translates it into WebGlRun contracts.
- WebGlBridge project reference policy remains limited to Simulation.Abstractions, Simulation.Visualization, and Components.WebGlRunLib.
- Strict visual state validation rejects missing pose/symbol mappings unless explicit no-op fallback is enabled.

