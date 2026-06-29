# QA Prompt

Review the selected subbundle from the perspective of publishing readiness. Confirm prerequisites, exact source references, dependency impact, and proof requirements before implementation starts.

For UI work, use Playwright MCP screenshots rather than visual assumptions. Check readability, wrapping, clipping, lateral overflow, overlay layering, available width/height use, disabled/loading/empty states, and interactive open states. Record route, viewport, actions, assertions, screenshots, and pass/fail result in `reviews/01-execution-report.md`.

If a later screenshot or test weakens a foundation, reopen the earlier subbundle instead of marking residual risk.
