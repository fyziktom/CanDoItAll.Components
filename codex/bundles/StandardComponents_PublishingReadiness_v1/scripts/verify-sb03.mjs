import fs from "node:fs";
import path from "node:path";

const root = process.cwd();
const bundleRoot = path.join(root, "codex", "bundles", "StandardComponents_PublishingReadiness_v1");

function read(relativePath) {
  const buffer = fs.readFileSync(path.join(root, relativePath));
  if (buffer.length >= 2 && buffer[0] === 0xff && buffer[1] === 0xfe) {
    return buffer.toString("utf16le").replace(/^\uFEFF/, "");
  }

  const text = buffer.toString("utf8");
  return text.includes("\u0000")
    ? buffer.toString("utf16le").replace(/^\uFEFF/, "")
    : text;
}

function assert(condition, message) {
  if (!condition) {
    throw new Error(message);
  }
}

function exists(relativePath) {
  return fs.existsSync(path.join(root, relativePath));
}

const commonAttributes = read("src/CanDoItAll.Components.Common/ComponentAttributes.cs");
const baseLibExtensions = read("src/CanDoItAll.Components.BaseLib/Infrastructure/ComponentAttributeExtensions.cs");
const styledBase = read("src/CanDoItAll.Components.BaseLib/StyledComponentBase.cs");
const ownershipDoc = read("docs/standard-components-foundation-ownership.md");
const solution = read("CanDoItAll.Components.slnx");
const commonTests = read("tests/CanDoItAll.Components.Common.Tests/ComponentAttributesTests.cs");
const baseLibTests = read("tests/CanDoItAll.Components.BaseLib.Tests/ComponentAttributeCompatibilityTests.cs");

assert(commonAttributes.includes("namespace CanDoItAll.Components.Common;"), "ComponentAttributes must live in Common namespace");
assert(commonAttributes.includes("public static class ComponentAttributes"), "Common must own ComponentAttributes");
assert(commonAttributes.includes("WithClassAndStyle("), "ComponentAttributes must expose class/style merge");
assert(commonAttributes.includes("CssClassBuilder.Join(first, second)"), "ComponentAttributes must reuse CssClassBuilder for class fragments");

assert(baseLibExtensions.includes("using CanDoItAll.Components.Common;"), "BaseLib extension wrapper must import Common");
assert(baseLibExtensions.includes("ComponentAttributes.WithClass(attributes, baseClass)"), "WithClass wrapper must delegate to Common");
assert(baseLibExtensions.includes("ComponentAttributes.WithClassAndStyle(attributes, baseClass, baseStyle)"), "WithClassAndStyle wrapper must delegate to Common");
assert(!baseLibExtensions.includes("ReadAttribute("), "BaseLib wrapper must not keep duplicate merge helpers");
assert(!baseLibExtensions.includes("TrimTrailingSemicolon("), "BaseLib wrapper must not keep duplicate style helpers");

assert(styledBase.includes("ComponentAttributes.WithClassAndStyle("), "StyledComponentBase must call Common ComponentAttributes");
assert(styledBase.includes("CssClassBuilder.Join(baseClass, Class)"), "StyledComponentBase must preserve base/component class order");
assert(styledBase.includes("CssClassBuilder.JoinStyles(baseStyle, Style)"), "StyledComponentBase must preserve base/component style order");

assert(ownershipDoc.includes("Common"), "Ownership doc must describe Common boundary");
assert(ownershipDoc.includes("BaseLib"), "Ownership doc must describe BaseLib boundary");
assert(ownershipDoc.includes("AppComponents Duplicate Exception"), "Ownership doc must record AppComponents duplicate exception");
assert(ownershipDoc.includes("SB04 owns the migration audit"), "Ownership doc must hand AppComponents duplicate removal to SB04");

assert(solution.includes("tests/CanDoItAll.Components.Common.Tests/CanDoItAll.Components.Common.Tests.csproj"), "solution must include Common tests");
assert(solution.includes("tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj"), "solution must include BaseLib tests");
assert(commonTests.includes("WithClassAndStyle_MergesBaseAndCapturedAttributes"), "Common tests must pin merge order");
assert(commonTests.includes("WithClassAndStyle_RemovesEmptyClassAndStyleAttributes"), "Common tests must pin empty cleanup");
assert(baseLibTests.includes("BaseLibExtensionPreservesClassAndStyleMergeOrder"), "BaseLib tests must pin compatibility wrapper");
assert(baseLibTests.includes("StyledComponentBasePreservesBaseClassComponentClassAndCapturedClassOrder"), "BaseLib tests must pin StyledComponentBase behavior");

const visual = JSON.parse(read("codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB03/data/sb03-visual-smoke.json"));
assert(visual.tool === "playwright-mcp", "visual smoke data must come from Playwright MCP");
assert(visual.captures.length === 4, "SB03 must capture inputs/layout at desktop and mobile");
for (const capture of visual.captures) {
  assert(capture.pageHorizontalOverflow === false, `${capture.route} ${capture.viewport} must not have page overflow`);
  assert(capture.overflowCount === 0, `${capture.route} ${capture.viewport} must not have element viewport overflow`);
  assert(capture.visualInspection === "passed", `${capture.route} ${capture.viewport} must be visually inspected`);
  assert(exists(path.join("codex/bundles/StandardComponents_PublishingReadiness_v1", capture.screenshot)), `missing screenshot ${capture.screenshot}`);
}

const transcripts = [
  "proof/SB03/transcripts/sb03-failing-first-common-tests.txt",
  "proof/SB03/transcripts/sb03-common-tests.txt",
  "proof/SB03/transcripts/sb03-baselib-tests.txt",
  "proof/SB03/transcripts/sb03-dotnet-test-solution.txt",
  "proof/SB03/transcripts/sb03-dotnet-build-sandbox.txt",
  "proof/SB03/transcripts/sb03-playwright-mcp-visual.txt",
];

for (const transcript of transcripts) {
  assert(exists(path.join("codex/bundles/StandardComponents_PublishingReadiness_v1", transcript)), `missing transcript ${transcript}`);
}

const failingFirstTranscript = read("codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB03/transcripts/sb03-failing-first-common-tests.txt");
assert(failingFirstTranscript.includes("CS0103") && failingFirstTranscript.includes("ComponentAttributes"), "failing-first transcript must prove missing Common owner");
assert(read("codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB03/transcripts/sb03-common-tests.txt").includes("Passed!"), "Common tests must pass");
assert(read("codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB03/transcripts/sb03-baselib-tests.txt").includes("Passed!"), "BaseLib tests must pass");
assert(read("codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB03/transcripts/sb03-dotnet-build-sandbox.txt").includes("0 Error(s)"), "sandbox build must pass");

console.log("SB03-INV-001 Common owns canonical attribute/class/style merging");
console.log("SB03-INV-002 BaseLib compatibility and StyledComponentBase semantics are contract-tested");
console.log("SB03-INV-003 Duplicate primitive exception is documented for SB04");
console.log("SB03-INV-004 Playwright MCP smoke screenshots show no inputs/layout overflow");
