# SB04 Semantic Invariants

## SB04-INV-001 Parked Basic Duplicates Are Removed

- Invariant ID: `SB04-INV-001`
- Source raw note: RAW06 requires AppComponents to stop owning basic standard components.
- Expected behavior: AppComponents no longer contains parked basic component, primitive, or helper source; standard layout, forms, buttons, typography, feedback, navigation, data visualization, and helper ownership live in the standard Components packages.
- Disallowed shallow implementation: delete files without a migration matrix, leave primitive/helper duplicates behind, or keep basic components hidden behind project include/exclude rules.
- Failing-first test: pre-migration inventory and source assertions identified parked AppComponents basic duplicates before deletion.
- Passing test: `bundle://proof/SB04/transcripts/sb04-verifier.txt` prints `SB04-INV-001`.
- Changed source files: `repo://src/CanDoItAll.Components.BaseLib/Components/Buttons/Button.razor` SHA-256 `2E5B8FEB9A5489BD93D2A4CE8C48A42EEC0437FB7E604087D36A9104C3253A5E`; main CanDoItAll AppComponents duplicate files were deleted under local context only.
- Production assertions: `bundle://proof/SB04/transcripts/sb04-source-assertions.txt` proves no standard primitive/helper duplicate matches remain in AppComponents source.
- Red-team negative case: if a parked basic duplicate or primitive/helper returns to AppComponents, `verify-sb04.mjs` fails the invariant.
- Downstream dependency check: SB05 can treat AppComponents as app-specific only, and SB06-SB09 can rely on BaseLib/standard packages for basic primitives.

## SB04-INV-002 AppComponents Keeps Only App-Specific Surfaces

- Invariant ID: `SB04-INV-002`
- Source raw note: RAW06 says AppComponents should keep complex web-app surfaces, not generic component bases.
- Expected behavior: remaining AppComponents source is limited to app shell, tab strip, tuning boundary, and their app-specific mode/request support types.
- Disallowed shallow implementation: move generic basics into another AppComponents folder or rely on project-file exclusions instead of real source cleanup.
- Failing-first test: source assertions compared remaining AppComponents files against the expected app-specific allow list.
- Passing test: `bundle://proof/SB04/transcripts/sb04-verifier.txt` prints `SB04-INV-002`.
- Changed source files: local context only main CanDoItAll `CanDoItAll.AppComponents.csproj` SHA-256 `36AF1AB6AA3A40F4AF1E26698D0866B920F97B893F40CFFE8E13F5AAF088C092`.
- Production assertions: `bundle://proof/SB04/transcripts/sb04-source-assertions.txt` lists only `AppShell`, `AppTabStrip`, `TunableComponentBoundary`, and their support types.
- Red-team negative case: any new non-allowed basic source file in AppComponents fails the source assertion.
- Downstream dependency check: publishing transfer can keep AppComponents out of the pure standard package base.

## SB04-INV-003 Useful Old Button Behavior Was Ported

- Invariant ID: `SB04-INV-003`
- Source raw note: RAW06 warns that old duplicate components may contain useful improvements that must not be lost.
- Expected behavior: the old AppComponents Button async in-flight click guard is preserved in the standard BaseLib Button.
- Disallowed shallow implementation: delete the old Button without comparing behavior or add only a disabled-looking style without callback suppression.
- Failing-first test: behavior comparison found the old in-flight guard before the duplicate Button was removed.
- Passing test: `bundle://proof/SB04/transcripts/sb04-baselib-button-tests.txt` proves concurrent button clicks invoke only one callback while the first callback is pending, and `bundle://proof/SB04/transcripts/sb04-verifier.txt` prints `SB04-INV-003`.
- Changed source files: `repo://src/CanDoItAll.Components.BaseLib/Components/Buttons/Button.razor` SHA-256 `2E5B8FEB9A5489BD93D2A4CE8C48A42EEC0437FB7E604087D36A9104C3253A5E`; `repo://tests/CanDoItAll.Components.BaseLib.Tests/ButtonBehaviorTests.cs` SHA-256 `B70D2E3DF0A59C7D1DFD65D38B2D463AC4884DEE71745A24E2E6CF96EAAEF779`.
- Production assertions: BaseLib Button now owns the guard and the behavior test exercises the standard component, not the deleted duplicate.
- Red-team negative case: removing the guard or allowing reentrant callbacks fails the Button behavior test.
- Downstream dependency check: SB07 action hardening can rely on the standard Button callback contract.

## SB04-INV-004 Consumers Still Build

- Invariant ID: `SB04-INV-004`
- Source raw note: RAW06 requires migration proof before removing old AppComponents basics.
- Expected behavior: the main AppComponents project and `CanDoItAll.Web` build cleanly after duplicate deletion.
- Disallowed shallow implementation: delete source without proving consumers compile against standard packages.
- Failing-first test: deletion was gated behind consumer builds to catch missing aliases or unresolved component imports.
- Passing test: `bundle://proof/SB04/transcripts/sb04-appcomponents-build.txt` and `bundle://proof/SB04/transcripts/sb04-main-web-build.txt` both have `ExitCode: 0`; `bundle://proof/SB04/transcripts/sb04-verifier.txt` prints `SB04-INV-004`.
- Changed source files: local context only main CanDoItAll AppComponents project SHA-256 `36AF1AB6AA3A40F4AF1E26698D0866B920F97B893F40CFFE8E13F5AAF088C092`.
- Production assertions: consumer builds prove references resolve from BaseLib/Common/Charts rather than removed parked copies.
- Red-team negative case: unresolved component imports or missing standard package references fail the build transcripts.
- Downstream dependency check: SB10 package/API hardening can freeze standard package contracts without depending on old AppComponents basics.

## SB04-INV-005 Visual Action Route Remains Healthy

- Invariant ID: `SB04-INV-005`
- Source raw note: RAW10 requires real browser screenshot and interaction proof for changed component surfaces.
- Expected behavior: the sandbox `groups/actions` route remains readable at desktop and mobile widths after the Button behavior port, and the visible BaseLib action button remains clickable.
- Disallowed shallow implementation: rely only on unit tests after changing a visible action component.
- Failing-first test: visual acceptance was blocked until desktop and mobile screenshots plus button interaction metrics were captured.
- Passing test: `bundle://proof/SB04/transcripts/sb04-playwright-mcp-actions-visual.txt` and `bundle://proof/SB04/data/sb04-actions-visual-smoke.json` prove desktop/mobile captures, no horizontal overflow, and successful click interaction; `bundle://proof/SB04/transcripts/sb04-verifier.txt` prints `SB04-INV-005`.
- Changed source files: `bundle://proof/SB04/screenshots/mcp/sb04-actions-1366.png` SHA-256 `F9A0A7E742E2D282F62C8074FA4479CB7FE7D9FA1B011FBAB3147E0809EC21C1`; `bundle://proof/SB04/screenshots/mcp/sb04-actions-390.png` SHA-256 `789B341C6D5EB9E3C7330A722D4CFAC1D6A087C6500547202C8FDDDE28926E9D`.
- Production assertions: route smoke captured `pageHorizontalOverflow=false`, `overflowCount=0`, and a successful BaseLib `Approve update` click.
- Red-team negative case: missing screenshots, overflow metrics, or failed click interaction fail the SB04 verifier.
- Downstream dependency check: SB05 and SB07 can reuse the action route as a standard sandbox proof surface.
