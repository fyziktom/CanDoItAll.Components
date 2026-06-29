import fs from "node:fs";
import path from "node:path";

const repoRoot = process.cwd();
const bundleRoot = path.join(repoRoot, "codex", "bundles", "StandardComponents_PublishingReadiness_v1");
const proofRoot = path.join(bundleRoot, "proof", "SB05");

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

function readRepo(relativePath) {
  return readFile(path.join(repoRoot, relativePath));
}

function readProof(relativePath) {
  return readFile(path.join(proofRoot, relativePath));
}

function exists(relativePath) {
  return fs.existsSync(path.join(proofRoot, relativePath));
}

function assert(condition, message) {
  if (!condition) {
    throw new Error(message);
  }
}

const registry = readRepo("src/CanDoItAll.Components.Sandbox/SandboxCatalogRegistry.cs");
assert(registry.includes("public bool IsStandardProofGroup => Key is not SandboxGroupKey.Canvas;"), "registry must mark Canvas as non-standard proof scope");
assert(registry.includes("StandardGroups") && registry.includes("DeferredGroups"), "registry must expose standard and deferred groups");
assert(registry.includes("StandardExamples") && registry.includes("DeferredExamples"), "registry must expose standard and deferred examples");

const layout = readRepo("src/CanDoItAll.Components.Sandbox/Components/Layout/MainLayout.razor");
assert(layout.includes("Standard proof groups"), "layout must label standard proof nav");
assert(layout.includes("Deferred proof scope"), "layout must label deferred proof nav");
assert(layout.includes("/groups/coverage"), "layout must link to coverage index");

const home = readRepo("src/CanDoItAll.Components.Sandbox/Components/Pages/Home.razor");
assert(home.includes("Standard groups") && home.includes("Standard examples"), "home page must count standard groups and examples");
assert(home.includes("Deferred groups") && home.includes("Canvas/WebGL"), "home page must disclose deferred Canvas/WebGL scope");
assert(home.includes("SandboxCatalogRegistry.StandardGroups"), "home page group list must use standard groups");

const coveragePage = readRepo("src/CanDoItAll.Components.Sandbox/Components/Pages/Coverage.razor");
assert(coveragePage.includes('@page "/groups/coverage"'), "coverage route must exist");
assert(coveragePage.includes('data-testid="sandbox-coverage-index"'), "coverage page must expose stable root test id");
assert(coveragePage.includes("sandbox-coverage-group-"), "coverage page must expose stable group row test ids");
assert(coveragePage.includes("sandbox-coverage-deferred"), "coverage page must expose deferred scope test id");

const coverage = JSON.parse(readProof("data/standard-component-coverage.json"));
assert(coverage.scope === "standard-components", "coverage JSON must identify standard scope");
assert(coverage.standardProjects.length === 4, "coverage JSON must scan BaseLib, Charts, Mermaid, and OverlayLib");
assert(coverage.deferredProjects.some((project) => project.name === "CanvasLib"), "coverage JSON must document CanvasLib as deferred");
assert(coverage.deferredProjects.some((project) => project.name === "WebGlLib"), "coverage JSON must document WebGlLib as deferred");
assert(coverage.components.length >= 150, `expected at least 150 standard component rows, saw ${coverage.components.length}`);
assert(coverage.standardGroups.length === 10, `expected 10 standard groups, saw ${coverage.standardGroups.length}`);
assert(coverage.routeMatrix.some((route) => route.route === "/groups/coverage"), "route matrix must include coverage route");
assert(coverage.routeMatrix.some((route) => route.route === "/groups/navigation/tabs"), "route matrix must include focused tabs route");
assert(coverage.routeMatrix.some((route) => route.route === "/groups/layout/composition"), "route matrix must include focused layout composition route");

for (const row of coverage.components) {
  assert(row.componentName && row.project && row.path, `coverage row missing identity: ${JSON.stringify(row)}`);
  assert(row.ownerGroup && row.ownerGroupTitle && row.ownerRoute, `coverage row missing owner route: ${row.componentName}`);
  assert(["covered", "planned-route", "documented-exception"].includes(row.status), `invalid status for ${row.componentName}: ${row.status}`);
  assert(!row.path.includes("CanvasLib") && !row.path.includes("WebGl"), `deferred source leaked into standard coverage: ${row.path}`);
  if (row.status === "documented-exception") {
    assert(row.rationale.includes("Compatibility") || row.rationale.includes("asset"), `exception row needs rationale: ${row.componentName}`);
  }
}

const statusCounts = coverage.summary.byStatus;
assert((statusCounts.covered ?? 0) > 35, "coverage must include direct sandbox registry/page references");
assert((statusCounts["planned-route"] ?? 0) > 40, "coverage must keep owner-route rows for later one-by-one hardening");
assert((statusCounts["documented-exception"] ?? 0) > 10, "coverage must explicitly document compatibility/assets exceptions");

const routeSmoke = readProof("transcripts/sb05-route-smoke.txt");
for (const route of coverage.routeMatrix.map((entry) => entry.route)) {
  assert(routeSmoke.includes(`${route} 1366x900 PASS`), `route smoke missing desktop pass for ${route}`);
  assert(routeSmoke.includes(`${route} 390x844 PASS`), `route smoke missing mobile pass for ${route}`);
}

const visual = JSON.parse(readProof("data/sb05-visual-smoke.json"));
assert(visual.tool === "playwright-mcp", "SB05 visual smoke must be captured with Playwright MCP");
assert(visual.captures.length >= 4, "SB05 must include index and focused route screenshots");
for (const capture of visual.captures) {
  assert(capture.pageHorizontalOverflow === false, `${capture.route} ${capture.viewport} must not have page overflow`);
  assert(capture.overflowCount === 0, `${capture.route} ${capture.viewport} must not have element overflow`);
  assert(capture.visualInspection === "passed", `${capture.route} ${capture.viewport} must be visually inspected`);
  assert(fs.existsSync(path.join(bundleRoot, capture.screenshot)), `missing SB05 screenshot ${capture.screenshot}`);
}

const sandboxBuild = readProof("transcripts/sb05-components-sandbox-build.txt");
assert(sandboxBuild.includes("0 Error(s)") && sandboxBuild.includes("0 Warning(s)"), "sandbox build must be clean");

const sourceAssertions = readProof("transcripts/sb05-source-assertions.txt");
assert(sourceAssertions.includes("StandardGroups excludes Canvas: PASS"), "source assertions must prove Canvas is excluded from standard groups");
assert(sourceAssertions.includes("Coverage JSON has owner route for every standard component: PASS"), "source assertions must prove every row has an owner route");

const antiStub = readProof("transcripts/sb05-anti-stub-audit.txt");
assert(antiStub.includes("No stub markers found"), "anti-stub audit must pass");

console.log("SB05-INV-001 Sandbox registry separates standard and deferred proof scope");
console.log("SB05-INV-002 Coverage route exposes stable test hooks and standard group rows");
console.log("SB05-INV-003 Generated coverage maps every scanned standard component to an owner route or documented exception");
console.log("SB05-INV-004 Canvas/WebGL source is deferred and excluded from standard coverage rows");
console.log("SB05-INV-005 Playwright MCP route smoke covers standard routes at desktop and mobile");
