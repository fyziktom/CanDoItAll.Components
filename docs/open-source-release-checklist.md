# Open-source release checklist

## Blocking decisions

- [x] Adopt the MIT-derived license with the CanDoItAll website-link requirement, and describe it as a custom license rather than plain SPDX MIT.
- [x] Add the final `LICENSE` file and make NuGet package metadata reference the packaged license file.
- [ ] Confirm redistribution rights and provenance for the bundled avatar images and Material Icons font.
- [ ] Include the complete required third-party license and notice text in every package that redistributes third-party assets, including Mermaid.

## Engineering gates

- [ ] Clean checkout restores, builds, tests, and packs in CI.
- [ ] Generated Tailwind output contains no consumer-global Preflight reset.
- [x] BaseLib approval snapshots are intentionally updated and green.
- [x] Package versions, project URL, repository URL, README, symbols, and XML documentation are present and consistent.
- [ ] Sandbox Playwright checks pass at 1440px, 768px, and 390px with no unexpected horizontal overflow or unnamed controls.
- [ ] Dialog, tooltip, menu, tree, and data-grid keyboard flows pass in a real browser.
- [ ] Canvas and floating-window multi-selection, movement, layering, context-menu, persistence, and small-host scenarios pass before those packages are declared stable.
- [ ] Mermaid defaults to strict rendering for untrusted source; trusted HTML/click behavior is an explicit opt-in.
- [ ] QR rendering and scanner permission/error fallbacks have sandbox coverage.
