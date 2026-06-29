import fs from "node:fs";
import path from "node:path";

const componentsRoot = process.cwd();
const mainRepoRoot = "C:\\repositories\\CanDoItAll";
const bundleRoot = path.join(componentsRoot, "codex", "bundles", "StandardComponents_PublishingReadiness_v1");
const appComponentsRoot = path.join(mainRepoRoot, "src", "CanDoItAll.AppComponents");

function readFile(filePath) {
  const buffer = fs.readFileSync(filePath);
  if (buffer.length >= 2 && buffer[0] === 0xff && buffer[1] === 0xfe) {
    return buffer.toString("utf16le").replace(/^\uFEFF/, "");
  }

  const text = buffer.toString("utf8");
  return text.includes("\u0000")
    ? buffer.toString("utf16le").replace(/^\uFEFF/, "")
    : text;
}

function readComponents(relativePath) {
  return readFile(path.join(componentsRoot, relativePath));
}

function readMain(relativePath) {
  return readFile(path.join(mainRepoRoot, relativePath));
}

function exists(filePath) {
  return fs.existsSync(filePath);
}

function assert(condition, message) {
  if (!condition) {
    throw new Error(message);
  }
}

const keptFiles = [
  "Components/AppShell.razor",
  "Components/AppShellMode.cs",
  "Components/AppShellNavigationMode.cs",
  "Components/AppTabStrip.razor",
  "Components/TunableComponentBoundary.razor",
  "Components/TuningBoundaryRequest.cs",
];

const deletedFiles = [
  "Primitives/ComponentPrimitives.cs",
  "Components/Alert.razor",
  "Components/Body.razor",
  "Components/Button.razor",
  "Components/Card.razor",
  "Components/CategoryAxis.razor",
  "Components/Chart.razor",
  "Components/CheckBox.razor",
  "Components/Column.razor",
  "Components/ContextMenu.razor",
  "Components/DataGrid.razor",
  "Components/DataGridColumn.razor",
  "Components/Dialog.razor",
  "Components/DropDown.razor",
  "Components/DropDownOption.cs",
  "Components/Fieldset.razor",
  "Components/FormField.razor",
  "Components/GridLines.razor",
  "Components/Header.razor",
  "Components/Icon.razor",
  "Components/Layout.razor",
  "Components/LineSeries.razor",
  "Components/Notification.razor",
  "Components/Numeric.razor",
  "Components/Password.razor",
  "Components/ProgressBar.razor",
  "Components/Row.razor",
  "Components/Sidebar.razor",
  "Components/Slider.razor",
  "Components/Stack.razor",
  "Components/Steps.razor",
  "Components/StepsItem.razor",
  "Components/Switch.razor",
  "Components/Tabs.razor",
  "Components/TabsItem.razor",
  "Components/TextArea.razor",
  "Components/TextBlock.razor",
  "Components/TextBox.razor",
  "Components/Tooltip.razor",
  "Components/ValueAxis.razor",
];

for (const file of keptFiles) {
  assert(exists(path.join(appComponentsRoot, file)), `kept app-specific file missing: ${file}`);
}

for (const file of deletedFiles) {
  assert(!exists(path.join(appComponentsRoot, file)), `parked duplicate still exists: ${file}`);
}

const remainingComponentFiles = fs
  .readdirSync(path.join(appComponentsRoot, "Components"))
  .filter((file) => fs.statSync(path.join(appComponentsRoot, "Components", file)).isFile())
  .sort()
  .map((file) => `Components/${file}`);

assert(
  JSON.stringify(remainingComponentFiles) === JSON.stringify(keptFiles.filter((file) => file.startsWith("Components/")).sort()),
  `unexpected remaining AppComponents files: ${remainingComponentFiles.join(", ")}`
);

const appCsproj = readMain("src/CanDoItAll.AppComponents/CanDoItAll.AppComponents.csproj");
assert(!appCsproj.includes("Compile Remove=\"Primitives"), "AppComponents csproj must not retain primitive exclusion workaround");
assert(!appCsproj.includes("Content Remove=\"Components"), "AppComponents csproj must not retain component exclusion workaround");
assert(!appCsproj.includes("Compile Include=\"Components"), "AppComponents csproj should rely on SDK default includes for app-specific cs files");
assert(!appCsproj.includes("Content Include=\"Components"), "AppComponents csproj should rely on SDK default includes for app-specific razor files");

const appSource = keptFiles.map((file) => readFile(path.join(appComponentsRoot, file))).join("\n");
assert(!appSource.includes("public static class ComponentAttributeExtensions"), "AppComponents must not define attribute merge helpers");
assert(!appSource.includes("public enum ButtonStyle"), "AppComponents must not define standard button primitives");
assert(!appSource.includes("public enum TextStyle"), "AppComponents must not define standard typography primitives");
assert(!appSource.includes("public sealed class NotificationService"), "AppComponents must not define standard notification service");

const button = readComponents("src/CanDoItAll.Components.BaseLib/Components/Buttons/Button.razor");
const buttonTest = readComponents("tests/CanDoItAll.Components.BaseLib.Tests/ButtonBehaviorTests.cs");
assert(button.includes("private bool isClickInFlight;"), "BaseLib Button must have the in-flight guard");
assert(button.includes("Disabled || IsBusy || isClickInFlight"), "BaseLib Button disabled state must include the in-flight guard");
assert(button.includes("isClickInFlight = true;") && button.includes("isClickInFlight = false;"), "BaseLib Button must set and clear in-flight guard");
assert(buttonTest.includes("ButtonPreventsConcurrentClickCallbacks"), "BaseLib Button behavior test must prove concurrent click guard");

const matrix = readFile(path.join(bundleRoot, "proof", "SB04", "data", "appcomponents-migration-matrix.md"));
for (const file of deletedFiles) {
  assert(matrix.includes(file), `migration matrix missing deleted file: ${file}`);
}
for (const file of keptFiles) {
  assert(matrix.includes(file), `migration matrix missing kept file: ${file}`);
}
assert(matrix.includes("Prevent concurrent button click callbacks"), "migration matrix must record ported button behavior");

const visual = JSON.parse(readFile(path.join(bundleRoot, "proof", "SB04", "data", "sb04-actions-visual-smoke.json")));
assert(visual.tool === "playwright-mcp", "SB04 visual smoke must be captured with Playwright MCP");
assert(visual.captures.length === 2, "SB04 must capture actions route at desktop and mobile");
for (const capture of visual.captures) {
  assert(capture.pageHorizontalOverflow === false, `${capture.route} ${capture.viewport} must not have page overflow`);
  assert(capture.overflowCount === 0, `${capture.route} ${capture.viewport} must not have element overflow`);
  assert(capture.buttonCount === 31, `${capture.route} ${capture.viewport} must expose expected action buttons`);
  assert(capture.visualInspection === "passed", `${capture.route} ${capture.viewport} must be visually inspected`);
  assert(exists(path.join(bundleRoot, capture.screenshot)), `missing SB04 screenshot ${capture.screenshot}`);
}

const appBuild = readFile(path.join(bundleRoot, "proof", "SB04", "transcripts", "sb04-appcomponents-build.txt"));
const webBuild = readFile(path.join(bundleRoot, "proof", "SB04", "transcripts", "sb04-main-web-build.txt"));
const buttonTests = readFile(path.join(bundleRoot, "proof", "SB04", "transcripts", "sb04-baselib-button-tests.txt"));
const actionVisualTranscript = readFile(path.join(bundleRoot, "proof", "SB04", "transcripts", "sb04-playwright-mcp-actions-visual.txt"));
assert(appBuild.includes("0 Error(s)") && appBuild.includes("0 Warning(s)"), "AppComponents build must be clean");
assert(webBuild.includes("0 Error(s)") && webBuild.includes("0 Warning(s)"), "main web build must be clean");
assert(buttonTests.includes("Passed!") && buttonTests.includes("Total:     3"), "BaseLib tests must include button behavior test");
assert(actionVisualTranscript.includes("pageHorizontalOverflow=false") && actionVisualTranscript.includes("clicked the BaseLib"), "SB04 action visual transcript must include overflow and interaction proof");

console.log("SB04-INV-001 Parked AppComponents basic duplicates are deleted");
console.log("SB04-INV-002 AppComponents keeps only app-specific shell, tab, and tuning surfaces");
console.log("SB04-INV-003 Old Button in-flight guard is ported to BaseLib and contract-tested");
console.log("SB04-INV-004 Main AppComponents and CanDoItAll.Web builds pass after migration");
console.log("SB04-INV-005 Actions route Playwright MCP smoke passes after Button behavior port");
