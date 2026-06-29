# SB04 Proof Manifest

## Scope

Subbundle: `04-appcomponents-duplicate-migration-audit`

Input closed:

- RAW06: Audit and reduce duplicate basic AppComponents components.

## Source Changes

Components repo:

- `src/CanDoItAll.Components.BaseLib/Components/Buttons/Button.razor`
  - Ported the old AppComponents async click in-flight guard into the standard Button.
  - Hash: `2E5B8FEB9A5489BD93D2A4CE8C48A42EEC0437FB7E604087D36A9104C3253A5E`
- `tests/CanDoItAll.Components.BaseLib.Tests/ButtonBehaviorTests.cs`
  - Adds contract proof that concurrent button clicks invoke only one callback while the first is in flight.
  - Hash: `B70D2E3DF0A59C7D1DFD65D38B2D463AC4884DEE71745A24E2E6CF96EAAEF779`

Main CanDoItAll repo local context only:

- Local context only main CanDoItAll `src/CanDoItAll.AppComponents/CanDoItAll.AppComponents.csproj`
  - Removed old explicit exclude/include workaround now that parked basic duplicates are gone.
  - Hash: `36AF1AB6AA3A40F4AF1E26698D0866B920F97B893F40CFFE8E13F5AAF088C092`
- Deleted 40 parked duplicate AppComponents files:
  - `Primitives/ComponentPrimitives.cs`
  - `Components/Alert.razor`, `Body.razor`, `Button.razor`, `Card.razor`, `CategoryAxis.razor`, `Chart.razor`, `CheckBox.razor`, `Column.razor`, `ContextMenu.razor`, `DataGrid.razor`, `DataGridColumn.razor`, `Dialog.razor`, `DropDown.razor`, `DropDownOption.cs`, `Fieldset.razor`, `FormField.razor`, `GridLines.razor`, `Header.razor`, `Icon.razor`, `Layout.razor`, `LineSeries.razor`, `Notification.razor`, `Numeric.razor`, `Password.razor`, `ProgressBar.razor`, `Row.razor`, `Sidebar.razor`, `Slider.razor`, `Stack.razor`, `Steps.razor`, `StepsItem.razor`, `Switch.razor`, `Tabs.razor`, `TabsItem.razor`, `TextArea.razor`, `TextBlock.razor`, `TextBox.razor`, `Tooltip.razor`, `ValueAxis.razor`.

## Semantic Contract

- `bundle://proof/SB04/semantic-invariants.md`

## Proof Scripts And Data

- `scripts/verify-sb04.mjs`
  - Hash: `036CFB89792B401543F626AFBDD90DBB600080DC2933F70C4B617817F3083E12`
- Migration matrix: `proof/SB04/data/appcomponents-migration-matrix.md`
  - Hash: `9C189CDC1DDC5AF5089669719B02F1FD3FEF6D962F6F87A100087BE4CB921E35`
- Actions visual smoke data: `proof/SB04/data/sb04-actions-visual-smoke.json`
  - Hash: `0CBE8B355CA691CB635ADF339494BC77C24694CF648661312BDD1FB3BDB81F06`
- MCP screenshots:
  - `proof/SB04/screenshots/mcp/sb04-actions-1366.png`
    - Hash: `F9A0A7E742E2D282F62C8074FA4479CB7FE7D9FA1B011FBAB3147E0809EC21C1`
  - `proof/SB04/screenshots/mcp/sb04-actions-390.png`
    - Hash: `789B341C6D5EB9E3C7330A722D4CFAC1D6A087C6500547202C8FDDDE28926E9D`

## Validation

- AppComponents build: `proof/SB04/transcripts/sb04-appcomponents-build.txt`
  - `ExitCode: 0`, `0 Warning(s)`, `0 Error(s)`.
- Main CanDoItAll web build: `proof/SB04/transcripts/sb04-main-web-build.txt`
  - `ExitCode: 0`, `0 Warning(s)`, `0 Error(s)`.
- BaseLib button behavior tests: `proof/SB04/transcripts/sb04-baselib-button-tests.txt`
  - `Passed!`, 3 tests.
- Components sandbox build: `proof/SB04/transcripts/sb04-components-sandbox-build.txt`
  - `ExitCode: 0`, `0 Warning(s)`, `0 Error(s)`.
- Semantic verifier: `proof/SB04/transcripts/sb04-verifier.txt`
  - `SB04-INV-001` through `SB04-INV-005` passed.
- Passing transcript: `bundle://proof/SB04/transcripts/sb04-verifier.txt`.
- Failing-first: N/A process/non-production proof normalization; SB04 negative checks are enforced by verifier/source assertions and documented in `bundle://proof/SB04/semantic-invariants.md`.
- Playwright MCP actions smoke: `proof/SB04/transcripts/sb04-playwright-mcp-actions-visual.txt`
  - Sandbox route `groups/actions` captured at `1366x900` and `390x844`.
  - Every capture has `pageHorizontalOverflow=false` and `overflowCount=0`.
  - The desktop capture clicked the BaseLib `Approve update` button successfully.
- Source assertions: `proof/SB04/transcripts/sb04-source-assertions.txt`
- Screenshot hashes: `proof/SB04/transcripts/sb04-screenshot-hashes.txt`
- Anti-stub audit: `proof/SB04/transcripts/sb04-anti-stub-audit.txt`
  - No `TODO`, `NotImplemented`, explicit stub, placeholder, or fake implementation matches in changed SB04 files.

## Raw Note Closure

- "old AppComponents basic components": solved for parked basic source duplicates by deletion.
- "some might have improvements": solved by comparing old Button and porting the in-flight guard into BaseLib.
- "AppComponents are more for complex components related to web app": solved for the project source boundary; AppComponents now keeps app shell, tab strip, and tuning boundary surfaces only.
- "consumer checks": solved by clean AppComponents and `CanDoItAll.Web` builds.
