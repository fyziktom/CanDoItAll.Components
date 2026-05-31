# WebGL JS runtime policy

## No TypeScript for this wave
Keep plain JavaScript, but enforce module boundaries and small files.

## JS module rules

- `01-webgl-scene.js` remains a public façade only.
- Runtime modules must not import the public façade.
- No runtime JS file should exceed 320 lines.
- Files above 220 lines require split review.
- Avoid duplicate command result helpers and vector normalization helpers.
- No domain vocabulary in generic WebGL runtime: economy, ledger, account, water, well, entrepreneur, farmer, land, parcel, citizen.
- No `eval`, `new Function`, `document.write`, dynamic script elements, or unsafe HTML insertion.

## Current split candidates

- Motion runtime: split command normalization, interpolation/easing, queue lifecycle and completion callbacks if file grows further.
- Batch normalizer: split policy resolution, patch coalescing and motion de-duplication if file grows further.
- Stage runner: add barrier module instead of growing one file.

## Large-screen only
Do not add small-screen, medium-screen, mobile, tablet or phone optimization work.
