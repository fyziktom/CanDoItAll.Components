import fs from "node:fs";
import path from "node:path";

const repoRoot = process.cwd();
const bundleRoot = path.join(repoRoot, "codex", "bundles", "StandardComponents_PublishingReadiness_v1");
const dataRoot = path.join(bundleRoot, "proof", "SB05", "data");
const sandboxRoot = path.join(repoRoot, "src", "CanDoItAll.Components.Sandbox");

const routeByGroup = new Map([
  ["Foundations", "/groups/foundations"],
  ["Inputs", "/groups/inputs"],
  ["Actions", "/groups/actions"],
  ["Navigation", "/groups/navigation"],
  ["Feedback", "/groups/feedback"],
  ["Layout", "/groups/layout"],
  ["DataDisplay", "/groups/data-display"],
  ["Charts", "/groups/charts"],
  ["Mermaid", "/groups/mermaid"],
  ["Overlays", "/groups/overlays"],
]);

const groupTitles = new Map([
  ["Foundations", "Foundations"],
  ["Inputs", "Inputs"],
  ["Actions", "Actions"],
  ["Navigation", "Navigation"],
  ["Feedback", "Feedback"],
  ["Layout", "Layout"],
  ["DataDisplay", "Data Display"],
  ["Charts", "Charts"],
  ["Mermaid", "Mermaid"],
  ["Overlays", "Overlays"],
]);

const standardProjects = [
  { name: "BaseLib", root: "src/CanDoItAll.Components.BaseLib" },
  { name: "Charts", root: "src/CanDoItAll.Components.Charts" },
  { name: "Mermaid", root: "src/CanDoItAll.Components.Mermaid" },
  { name: "OverlayLib", root: "src/CanDoItAll.Components.OverlayLib" },
];

const deferredProjects = [
  { name: "CanvasLib", root: "src/CanDoItAll.Components.CanvasLib", reason: "Canvas implementation is deferred to a separate publishing pass." },
  { name: "WebGlLib", root: "src/CanDoItAll.Components.WebGlLib", reason: "WebGL implementation is deferred to a separate publishing pass." },
  { name: "WebGlRunLib", root: "src/CanDoItAll.Components.WebGlRunLib", reason: "WebGL runtime implementation is deferred to a separate publishing pass." },
  { name: "WebGlSandbox", root: "src/CanDoItAll.Components.WebGlSandbox", reason: "WebGL sandbox proof is deferred to a separate publishing pass." },
];

function readText(filePath) {
  return fs.readFileSync(filePath, "utf8");
}

function toRepoPath(filePath) {
  return path.relative(repoRoot, filePath).replaceAll(path.sep, "/");
}

function walkRazorFiles(root) {
  const absoluteRoot = path.join(repoRoot, root);
  if (!fs.existsSync(absoluteRoot)) {
    return [];
  }

  const results = [];
  const stack = [absoluteRoot];

  while (stack.length > 0) {
    const current = stack.pop();
    for (const entry of fs.readdirSync(current, { withFileTypes: true })) {
      const fullPath = path.join(current, entry.name);
      if (entry.isDirectory()) {
        stack.push(fullPath);
      } else if (entry.isFile() && entry.name.endsWith(".razor") && entry.name !== "_Imports.razor") {
        results.push(fullPath);
      }
    }
  }

  return results.sort((left, right) => toRepoPath(left).localeCompare(toRepoPath(right)));
}

function collectSandboxComponentNames() {
  const componentNames = new Set();
  const registryPath = path.join(sandboxRoot, "SandboxCatalogRegistry.cs");
  const registry = readText(registryPath);
  for (const match of registry.matchAll(/"([A-Z][A-Za-z0-9_]*)"/g)) {
    componentNames.add(match[1]);
  }

  for (const file of walkRazorFiles("src/CanDoItAll.Components.Sandbox")) {
    const source = readText(file);
    for (const match of source.matchAll(/<([A-Z][A-Za-z0-9_]*)(?:\s|>|\/)/g)) {
      componentNames.add(match[1]);
    }
  }

  return [...componentNames].sort();
}

function classifyBaseLib(relativePath, componentName) {
  const parts = relativePath.split("/");
  const componentsIndex = parts.indexOf("Components");
  const segment = componentsIndex >= 0 ? parts[componentsIndex + 1] : "";

  if (segment === "Typography" || segment === "Identity") {
    return "Foundations";
  }

  if (segment === "Forms") {
    return "Inputs";
  }

  if (segment === "Buttons") {
    return "Actions";
  }

  if (segment === "Navigation") {
    return "Navigation";
  }

  if (segment === "Feedback" || segment === "Badges") {
    return "Feedback";
  }

  if (segment === "Layout") {
    return "Layout";
  }

  if (segment === "Modals") {
    return "Overlays";
  }

  if (segment === "DataVisualization") {
    return ["CategoryAxis", "Chart", "GridLines", "LineSeries", "ValueAxis"].includes(componentName)
      ? "Charts"
      : "DataDisplay";
  }

  if (segment === "Cards" || segment === "DataDisplay" || segment === "Lists" || segment === "Storage") {
    return "DataDisplay";
  }

  return "DataDisplay";
}

function classify(projectName, relativePath, componentName) {
  if (projectName === "Charts") {
    return "Charts";
  }

  if (projectName === "Mermaid") {
    return "Mermaid";
  }

  if (projectName === "OverlayLib") {
    return "Overlays";
  }

  return classifyBaseLib(relativePath, componentName);
}

function exceptionReason(relativePath, componentName) {
  if (relativePath.includes("/Compatibility/")) {
    return "Compatibility component; covered through the owning group route and consumer migration, not a dedicated one-by-one standard proof row.";
  }

  if (
    componentName.endsWith("HeadAssets")
    || componentName.endsWith("BodyAssets")
    || relativePath.includes("/Shared/Assets/")
    || relativePath.includes("/Components/Assets/")
  ) {
    return "Head/body asset support component; validated through the owning interactive component route and package asset checks.";
  }

  return "";
}

function buildCoverageRows() {
  const sandboxComponentNames = new Set(collectSandboxComponentNames());
  const rows = [];

  for (const project of standardProjects) {
    for (const file of walkRazorFiles(project.root)) {
      const relativePath = toRepoPath(file);
      const componentName = path.basename(file, ".razor");
      const ownerGroup = classify(project.name, relativePath, componentName);
      const ownerRoute = routeByGroup.get(ownerGroup);
      const documentedException = exceptionReason(relativePath, componentName);
      const registryOrPageReference = sandboxComponentNames.has(componentName);
      const status = documentedException
        ? "documented-exception"
        : registryOrPageReference
          ? "covered"
          : "planned-route";

      rows.push({
        componentName,
        project: project.name,
        path: relativePath,
        ownerGroup,
        ownerGroupTitle: groupTitles.get(ownerGroup),
        ownerRoute,
        status,
        evidence: registryOrPageReference ? "sandbox-registry-or-page-reference" : "owner-group-route",
        rationale: documentedException || "Standard component assigned to an owning group route for Playwright screenshot proof.",
      });
    }
  }

  return rows.sort((left, right) => `${left.ownerGroup}/${left.project}/${left.componentName}`.localeCompare(`${right.ownerGroup}/${right.project}/${right.componentName}`));
}

function summarize(rows) {
  const byStatus = {};
  const byGroup = {};

  for (const row of rows) {
    byStatus[row.status] = (byStatus[row.status] ?? 0) + 1;
    byGroup[row.ownerGroup] ??= {
      title: row.ownerGroupTitle,
      route: row.ownerRoute,
      total: 0,
      covered: 0,
      plannedRoute: 0,
      documentedException: 0,
    };

    byGroup[row.ownerGroup].total += 1;
    if (row.status === "covered") {
      byGroup[row.ownerGroup].covered += 1;
    } else if (row.status === "planned-route") {
      byGroup[row.ownerGroup].plannedRoute += 1;
    } else if (row.status === "documented-exception") {
      byGroup[row.ownerGroup].documentedException += 1;
    }
  }

  return { byStatus, byGroup };
}

function buildRouteMatrix() {
  const standardGroupRoutes = [...routeByGroup.entries()].map(([group, route]) => ({
    group,
    title: groupTitles.get(group),
    route,
    kind: "group",
  }));

  return [
    { group: "Catalog", title: "Component catalog", route: "/", kind: "index" },
    { group: "Catalog", title: "Standard coverage index", route: "/groups/coverage", kind: "coverage" },
    ...standardGroupRoutes,
    { group: "Navigation", title: "Tabs lab", route: "/groups/navigation/tabs", kind: "focused" },
    { group: "Layout", title: "Layout composition lab", route: "/groups/layout/composition", kind: "focused" },
  ];
}

function buildDeferredSummary() {
  return deferredProjects.map((project) => ({
    ...project,
    razorFileCount: walkRazorFiles(project.root).length,
  }));
}

function writeMarkdown(output, rows) {
  const lines = [
    "# SB05 Standard Component Coverage",
    "",
    "| Component | Project | Owner group | Owner route | Status | Rationale |",
    "|---|---|---|---|---|---|",
  ];

  for (const row of rows) {
    lines.push(`| ${row.componentName} | ${row.project} | ${row.ownerGroupTitle} | \`${row.ownerRoute}\` | ${row.status} | ${row.rationale.replaceAll("|", "\\|")} |`);
  }

  fs.writeFileSync(path.join(dataRoot, "standard-component-coverage.md"), `${lines.join("\n")}\n`);
}

fs.mkdirSync(dataRoot, { recursive: true });

const rows = buildCoverageRows();
const summary = summarize(rows);
const output = {
  generatedAtUtc: new Date().toISOString(),
  scope: "standard-components",
  standardProjects,
  deferredProjects: buildDeferredSummary(),
  standardGroups: [...groupTitles.entries()].map(([key, title]) => ({
    key,
    title,
    route: routeByGroup.get(key),
  })),
  routeMatrix: buildRouteMatrix(),
  summary,
  components: rows,
};

fs.writeFileSync(path.join(dataRoot, "standard-component-coverage.json"), `${JSON.stringify(output, null, 2)}\n`);
writeMarkdown(output, rows);

console.log(`SB05 coverage rows: ${rows.length}`);
console.log(`SB05 status counts: ${JSON.stringify(summary.byStatus)}`);
console.log(`SB05 standard routes: ${output.routeMatrix.length}`);
