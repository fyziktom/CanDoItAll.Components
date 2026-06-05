# SB07 - Browser observer proof must use real browser state

Make browser observer proof independent from expected in-memory objects.

Tasks:
- Export browser-loaded run document hash and live scene content hash from the JS/browser runtime.
- Compare expected run document hash to browser-exported hash.
- Compare final object positions from browser proof snapshot to expected final state.
- Do not accept fallback expected document == expected document comparison.
- Store observer proof artifact as JSON with schema version and hash.

Required proof:
- positive browser proof,
- negative mismatch proof,
- observer artifact consumed by readiness evidence resolver.

