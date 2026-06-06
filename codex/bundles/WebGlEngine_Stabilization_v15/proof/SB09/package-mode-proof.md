# SB09 Package-Mode Proof

Package version validated:

- `0.1.0-rcv15.20260606022842`

Fresh local package source:

- `artifacts/webgl-engine-rc-v15/packages`

Package-mode consumers:

- `samples/CanDoItAll.Components.WebGlLibOnlyViewer`
- `samples/CanDoItAll.Components.WebGlRunLibGenericSample`

Proof steps passed:

- packed WebGlLib/WebGlRunLib and dependencies;
- restored WebGlLib-only viewer with `UseComponentsWebGlLibPackage=true`;
- built WebGlLib-only viewer with `--no-restore`;
- restored WebGlRunLib generic sample with `UseComponentsWebGlRunLibPackage=true`;
- built and ran WebGlRunLib generic sample.

Transcript:

- `proof/SB18/transcripts/rc-validation-transcript.txt`
