# SB03 Semantic Invariants

## SB03-INV-001 Common Owns Attribute Merge Semantics

- Invariant ID: `SB03-INV-001`
- Source raw note: RAW02 requires detailed implementation study and isolation of shared bases/helpers.
- Expected behavior: `CanDoItAll.Components.Common.ComponentAttributes` is the canonical framework-neutral implementation for captured attributes, class merge, and style merge, and it reuses `CssClassBuilder` for class fragments.
- Disallowed shallow implementation: duplicate merge logic in BaseLib, keep the helper tied to Blazor-specific types, or skip order-preservation tests.
- Failing-first test: `bundle://proof/SB03/transcripts/sb03-failing-first-common-tests.txt` proves the new Common contract tests failed before `ComponentAttributes` existed.
- Passing test: `bundle://proof/SB03/transcripts/sb03-common-tests.txt` and `bundle://proof/SB03/transcripts/sb03-verifier.txt` prove the contract and print `SB03-INV-001`.
- Changed source files: `repo://src/CanDoItAll.Components.Common/ComponentAttributes.cs` SHA-256 `4C4E73A33C1B82AF0F964E0C63B848218D70314C776C2EE265C0605B0319C6D9`; `repo://tests/CanDoItAll.Components.Common.Tests` contains the new contract tests.
- Production assertions: Common owns captured-attribute/class/style merge behavior and remains framework-neutral.
- Red-team negative case: removing Common ownership or breaking merge order fails the Common tests and verifier.
- Downstream dependency check: SB04-SB09 can rely on one shared helper rather than component-specific merge copies.

## SB03-INV-002 BaseLib Compatibility Is Preserved

- Invariant ID: `SB03-INV-002`
- Source raw note: RAW02 and RAW08 require shared foundation isolation before downstream refactors without breaking existing consumers.
- Expected behavior: `ComponentAttributeExtensions` remains in BaseLib as a compatibility wrapper delegating to Common, and `StyledComponentBase` calls Common directly while preserving base, component, and captured class/style order.
- Disallowed shallow implementation: remove the BaseLib extension abruptly, change class/style precedence, or leave hidden duplicate merge logic.
- Failing-first test: Common/BaseLib contract tests were added before the final refactor to catch missing helpers and ordering regressions.
- Passing test: `bundle://proof/SB03/transcripts/sb03-baselib-tests.txt` and `bundle://proof/SB03/transcripts/sb03-verifier.txt` prove compatibility and print `SB03-INV-002`.
- Changed source files: `repo://src/CanDoItAll.Components.BaseLib/Infrastructure/ComponentAttributeExtensions.cs` SHA-256 `03AAA8004C85EE88D9650BB2F78EA54C78D9425B28A11BF4850E9207BC53E58A`; `repo://src/CanDoItAll.Components.BaseLib/StyledComponentBase.cs` SHA-256 `A60D418A962B48234383D8BA2BA410E998E2DB0E98442178E5FDBB17DF5BC4C7`.
- Production assertions: BaseLib preserves public compatibility while sharing the Common implementation.
- Red-team negative case: reversing class/style order or removing compatibility delegation fails BaseLib tests.
- Downstream dependency check: SB10 public API/package-input approvals can include the compatibility wrapper deliberately.

## SB03-INV-003 Duplicate Primitive Removal Has A Scoped Exception

- Invariant ID: `SB03-INV-003`
- Source raw note: RAW06 requires duplicate AppComponents basic components to be audited rather than silently ignored.
- Expected behavior: the Components repo has one canonical attribute merge implementation, while the remaining external AppComponents primitive/helper duplicate is documented as an SB04 migration exception.
- Disallowed shallow implementation: claim duplicate cleanup is complete while leaving external AppComponents primitives undocumented or deleting them without consumer proof.
- Failing-first test: source assertions found the local-context-only AppComponents primitive duplicate and routed it to SB04 instead of hiding it.
- Passing test: `bundle://proof/SB03/transcripts/sb03-source-assertions.txt` and `bundle://proof/SB03/transcripts/sb03-verifier.txt` prove the exception and print `SB03-INV-003`.
- Changed source files: `repo://docs/standard-components-foundation-ownership.md` SHA-256 `A7C9B40CC3D27A1CCD63E5E4DA493AE1A675D80589F06AB69FD28FE75282B2B8`.
- Production assertions: ownership documentation explicitly separates Common/BaseLib ownership from SB04 AppComponents duplicate migration.
- Red-team negative case: undocumented duplicate helper ownership or AppComponents deletion without consumer proof fails the source assertion gate.
- Downstream dependency check: SB04 consumes this exception and closes it with migration/build proof.

## SB03-INV-004 Visual Smoke Confirms Shared-Base Refactor

- Invariant ID: `SB03-INV-004`
- Source raw note: RAW10 requires real visual validation for component changes that may affect layout or styling.
- Expected behavior: sandbox routes `groups/inputs` and `groups/layout` render at desktop and mobile widths after the helper refactor without horizontal page or element overflow.
- Disallowed shallow implementation: rely only on unit tests after touching shared attribute/style merge behavior.
- Failing-first test: visual smoke was required because shared class/style merge changes could silently break rendered components.
- Passing test: `bundle://proof/SB03/transcripts/sb03-playwright-mcp-visual.txt` and `bundle://proof/SB03/transcripts/sb03-verifier.txt` prove desktop/mobile captures and print `SB03-INV-004`.
- Changed source files: `bundle://proof/SB03/data/sb03-visual-smoke.json` SHA-256 `42815D99B68C80C39705A2865A3D00BF8CE3C6EF38A612D0ED7D940197301D77`; MCP screenshots are stored in `bundle://proof/SB03/screenshots/mcp`.
- Production assertions: each capture reports `pageHorizontalOverflow=false` and `overflowCount=0`.
- Red-team negative case: missing screenshots or overflow metrics fail the SB03 verifier.
- Downstream dependency check: SB05-SB09 can build on the shared helper/base refactor without reopening foundation risk.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| Common attribute helper | `repo://src/CanDoItAll.Components.Common/ComponentAttributes.cs` owns captured attribute/class/style merge behavior. | BaseLib consumes it through `repo://src/CanDoItAll.Components.BaseLib/Infrastructure/ComponentAttributeExtensions.cs` and `repo://src/CanDoItAll.Components.BaseLib/StyledComponentBase.cs`. | `bundle://proof/SB03/transcripts/sb03-common-tests.txt`, `bundle://proof/SB03/transcripts/sb03-baselib-tests.txt`, and `bundle://proof/SB03/transcripts/sb03-dotnet-test-solution.txt` prove lifecycle tests/build. | `bundle://proof/SB03/transcripts/sb03-failing-first-common-tests.txt` proves missing helper contracts failed before implementation. |
| AppComponents duplicate exception | `repo://docs/standard-components-foundation-ownership.md` documents the temporary external duplicate boundary. | SB04 consumes the exception and removes/ports parked basics with consumer proof. | `bundle://proof/SB03/transcripts/sb03-source-assertions.txt` proves the exception is explicit. | Deleting external AppComponents duplicates without SB04 consumer proof is rejected by the progression gate. |

## Semantic Gate Decision

Pass. SB03 includes failing-first tests, Common/BaseLib contract tests, solution/build proof, MCP screenshots, source assertions, anti-stub audit proof, and a documented AppComponents duplicate exception for SB04.
