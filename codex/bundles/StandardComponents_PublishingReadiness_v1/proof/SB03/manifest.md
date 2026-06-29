# SB03 Proof Manifest

## Scope

Subbundle: `03-shared-bases-helpers-and-primitives-isolation`

Inputs closed:

- RAW02: Detailed implementation study and foundation refactoring/hardening.
- RAW08: General shared foundations before downstream component group work.

## Source Changes

- `repo://src/CanDoItAll.Components.Common/ComponentAttributes.cs`
  - Added the canonical framework-neutral captured-attribute, class, and style merge helper.
  - Hash: `4C4E73A33C1B82AF0F964E0C63B848218D70314C776C2EE265C0605B0319C6D9`
- `repo://src/CanDoItAll.Components.BaseLib/Infrastructure/ComponentAttributeExtensions.cs`
  - Replaced duplicate merge logic with compatibility delegation to Common.
  - Hash: `03AAA8004C85EE88D9650BB2F78EA54C78D9425B28A11BF4850E9207BC53E58A`
- `repo://src/CanDoItAll.Components.BaseLib/StyledComponentBase.cs`
  - Calls `ComponentAttributes.WithClassAndStyle` while preserving existing class/style order.
  - Hash: `A60D418A962B48234383D8BA2BA410E998E2DB0E98442178E5FDBB17DF5BC4C7`
- `repo://CanDoItAll.Components.slnx`
  - Adds Common and BaseLib contract test projects.
  - Hash: `4DE551D4842DB40C42CBE225098FEE406BDECCB03DA47FF4F5150B49C7069FA8`
- `repo://docs/standard-components-foundation-ownership.md`
  - Documents Common/BaseLib ownership and the AppComponents duplicate exception owned by SB04.
  - Hash: `A7C9B40CC3D27A1CCD63E5E4DA493AE1A675D80589F06AB69FD28FE75282B2B8`
- `repo://tests/CanDoItAll.Components.Common.Tests`
  - Pins `ComponentAttributes` and `CssClassBuilder` behavior.
- `repo://tests/CanDoItAll.Components.BaseLib.Tests`
  - Pins BaseLib compatibility extension and `StyledComponentBase` behavior.

## Semantic Contract

- `bundle://proof/SB03/semantic-invariants.md`

## Proof Scripts And Data

- `bundle://scripts/verify-sb03.mjs`
  - Hash: `1D50611724F86145D87BFB3704C06DA9360C947D5B2B24D97C0FFE853EBCBF90`
- Visual smoke data: `proof/SB03/data/sb03-visual-smoke.json`
  - Hash: `42815D99B68C80C39705A2865A3D00BF8CE3C6EF38A612D0ED7D940197301D77`
- MCP screenshots:
  - `proof/SB03/screenshots/mcp/sb03-inputs-1366.png`
    - Hash: `97B8363D5453B9CCEE0D7A179D658CEDF6AAC30EBE23EAD5C223E07898281F3F`
  - `proof/SB03/screenshots/mcp/sb03-inputs-390.png`
    - Hash: `06129F3141DD5C8A83A8E3BE0F432F93A1F34D6A41239FD0695EDE2F22B4EE79`
  - `proof/SB03/screenshots/mcp/sb03-layout-1366.png`
    - Hash: `3F9EFABC48C5F2590F2677A354A0F56AACA65C1ED39730ADEF9E6F4677FA990C`
  - `proof/SB03/screenshots/mcp/sb03-layout-390.png`
    - Hash: `2CCFB37DE0B67BE7049D4F307D8BABA373440E40DBF53DD15895A878B69BF781`

## Validation

- Failing-first Common test transcript: `proof/SB03/transcripts/sb03-failing-first-common-tests.txt`
  - Proves the new contract tests failed before `ComponentAttributes` existed.
- Common contract tests: `proof/SB03/transcripts/sb03-common-tests.txt`
  - `Passed!`, 5 tests.
- BaseLib contract tests: `proof/SB03/transcripts/sb03-baselib-tests.txt`
  - `Passed!`, 2 tests.
- Passing transcript: `bundle://proof/SB03/transcripts/sb03-verifier.txt`.
- Solution test run: `proof/SB03/transcripts/sb03-dotnet-test-solution.txt`
  - `ExitCode: 0`.
- Sandbox build: `proof/SB03/transcripts/sb03-dotnet-build-sandbox.txt`
  - `ExitCode: 0`, `0 Warning(s)`, `0 Error(s)`.
- Semantic verifier: `proof/SB03/transcripts/sb03-verifier.txt`
  - `SB03-INV-001` through `SB03-INV-004` passed.
- Playwright MCP visual smoke: `proof/SB03/transcripts/sb03-playwright-mcp-visual.txt`
  - Sandbox routes `groups/inputs` and `groups/layout` captured at `1366x900` and `390x844`.
  - Every capture has `pageHorizontalOverflow=false` and `overflowCount=0`.
- Source assertions: `proof/SB03/transcripts/sb03-source-assertions.txt`
- Screenshot hashes: `proof/SB03/transcripts/sb03-screenshot-hashes.txt`
- Anti-stub audit: `proof/SB03/transcripts/sb03-anti-stub-audit.txt`
  - No `TODO`, `NotImplemented`, explicit stub, placeholder, or fake implementation matches in changed SB03 files.

## Duplicate Primitive Exception

The local-context-only audit found the main CanDoItAll AppComponents primitive file still duplicated `Orientation`, `AlignItems`, `JustifyContent`, `FlexWrap`, `ButtonStyle`, `ButtonSize`, `Variant`, `Shade`, and `ComponentAttributeExtensions`.

SB03 does not delete those external AppComponents definitions because migration requires consumer checks and compatibility proof. SB04 owns that duplicate migration audit and removal plan.

## Raw Note Closure

- "detailed study of the actual implementation": closed for shared helper/base ownership through source audit, implementation, and tests.
- "identify correct phases... first parts that are totally general": closed by isolating the general attribute merge helper before downstream component groups.
- "isolation of bases, helpers": closed for class/style/attribute merging; remaining AppComponents duplicate cleanup is explicitly routed to SB04.
