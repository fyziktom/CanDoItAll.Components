import fs from "node:fs";
import path from "node:path";
import crypto from "node:crypto";

const repoRoot = process.cwd();
const bundleRoot = path.join(repoRoot, "codex", "bundles", "StandardComponents_PublishingReadiness_v1");
const appComponentsRoot = "C:\\repositories\\CanDoItAll\\src\\CanDoItAll.AppComponents";

const standardProjects = [
  "src/CanDoItAll.Components.Common",
  "src/CanDoItAll.Components.BaseLib",
  "src/CanDoItAll.Components.Charts",
  "src/CanDoItAll.Components.OverlayLib",
  "src/CanDoItAll.Components.Mermaid",
  "src/CanDoItAll.Components.Sandbox",
];

const excludedSegment = new Set(["bin", "obj", ".git", "node_modules"]);

function toPosix(value) {
  return value.replaceAll("\\", "/");
}

function repoRef(relativePath) {
  return `repo://${toPosix(relativePath)}`;
}

function bundleRef(relativePath) {
  return `bundle://${toPosix(relativePath)}`;
}

function relFromRepo(filePath) {
  return toPosix(path.relative(repoRoot, filePath));
}

function readText(filePath) {
  return fs.readFileSync(filePath, "utf8");
}

function writeText(relativePath, content) {
  const target = path.join(bundleRoot, relativePath);
  fs.mkdirSync(path.dirname(target), { recursive: true });
  fs.writeFileSync(target, content.replace(/\r?\n/g, "\n"), "utf8");
}

function writeJson(relativePath, data) {
  writeText(relativePath, `${JSON.stringify(data, null, 2)}\n`);
}

function listFiles(rootPath, extensions) {
  const results = [];
  if (!fs.existsSync(rootPath)) {
    return results;
  }

  for (const entry of fs.readdirSync(rootPath, { withFileTypes: true })) {
    if (excludedSegment.has(entry.name)) {
      continue;
    }

    const next = path.join(rootPath, entry.name);
    if (entry.isDirectory()) {
      results.push(...listFiles(next, extensions));
      continue;
    }

    if (extensions.some((extension) => entry.name.endsWith(extension))) {
      results.push(next);
    }
  }

  return results.sort((a, b) => a.localeCompare(b));
}

function countMatches(text, pattern) {
  return [...text.matchAll(pattern)].length;
}

function extractParameterNames(text) {
  const names = [];
  const pattern = /\[Parameter(?:\([^\)]*\))?\]\s*(?:\r?\n\s*)+public\s+[\w<>,\?\.\s]+\s+(\w+)\s*\{/g;
  for (const match of text.matchAll(pattern)) {
    names.push(match[1]);
  }

  return names;
}

function inferProject(relativePath) {
  const parts = toPosix(relativePath).split("/");
  return parts.length >= 2 ? parts[1] : "";
}

function inferGroup(relativePath) {
  const parts = toPosix(relativePath).split("/");
  const componentIndex = parts.indexOf("Components");
  if (componentIndex >= 0 && parts[componentIndex + 1]) {
    if (parts[componentIndex + 1] === "Pages" && parts[componentIndex + 2]) {
      return `Sandbox/${parts[componentIndex + 1]}`;
    }

    return parts[componentIndex + 1];
  }

  if (parts.includes("Models")) {
    return "Models";
  }

  if (parts.includes("Infrastructure")) {
    return "Infrastructure";
  }

  if (parts.includes("Internal")) {
    return "Internal";
  }

  if (parts.includes("wwwroot")) {
    return "StaticAssets";
  }

  return "ProjectRoot";
}

function semanticComponentName(filePath) {
  return path.basename(filePath).replace(/\.(razor|cs|css)$/i, "").replace(/\.razor$/i, "");
}

function sha256(text) {
  return crypto.createHash("sha256").update(text).digest("hex");
}

function normalizeSourceForCompare(text) {
  return text
    .replace(/@namespace\s+CanDoItAll\.(AppComponents|Components\.BaseLib)/g, "@namespace")
    .replace(/\s+/g, " ")
    .trim();
}

function tailwindUtilitySmell(text) {
  const rawCssDeclarations = countMatches(text, /^\s*(display|align-items|justify-content|position|width|height|min-width|min-height|max-width|max-height|padding|margin|gap|border|border-radius|background|color|font-size|font-weight|line-height|box-shadow|overflow|text-overflow|white-space|flex|grid-template|inset|top|right|bottom|left|transform|transition)\s*:/gm);
  const applyCount = countMatches(text, /@apply\b/g);
  const mediaCount = countMatches(text, /@media\b/g);
  const cssVarCount = countMatches(text, /var\(--/g);
  const arbitraryValueCount = countMatches(text, /\[[^\]]+\]/g);
  const colorMixCount = countMatches(text, /color-mix\(/g);
  const importantCount = countMatches(text, /!important|!\w/g);

  let severity = "Low";
  if (rawCssDeclarations > applyCount * 2 && rawCssDeclarations > 20) {
    severity = "High";
  } else if (rawCssDeclarations > applyCount && rawCssDeclarations > 10) {
    severity = "Medium";
  }

  return {
    rawCssDeclarations,
    applyCount,
    mediaCount,
    cssVarCount,
    arbitraryValueCount,
    colorMixCount,
    importantCount,
    severity,
  };
}

function classifyComponent(component, oldMatch) {
  const group = component.group;
  if (component.project === "CanDoItAll.Components.Sandbox") {
    return {
      category: "Sandbox proof surface",
      recommendation: "Keep sandbox-only, but split standard proof from Canvas/WebGL proof and ensure every standard component has a direct demo row.",
      owner: "SB05",
    };
  }

  if (component.relativePath.includes("/Compatibility/")) {
    return {
      category: "Compatibility shim",
      recommendation: "Review for publish-time deprecation, alias strategy, or migration docs; do not remove until AppComponents migration proof passes.",
      owner: "SB10",
    };
  }

  if (["Forms"].includes(group)) {
    return {
      category: "Standard input component",
      recommendation: "Harden accessibility, long-text, disabled, dense, and mobile proof before publishing.",
      owner: "SB06",
    };
  }

  if (["Buttons", "Badges", "Feedback"].includes(group)) {
    return {
      category: "Standard action/feedback component",
      recommendation: "Validate wrapping, icon/text behavior, service host behavior, loading, empty, and disabled states.",
      owner: "SB07",
    };
  }

  if (["Layout", "Navigation", "Modals"].includes(group) || component.project === "CanDoItAll.Components.OverlayLib") {
    return {
      category: "Standard layout/navigation/overlay component",
      recommendation: "Validate available-space use, overflow, overlay open states, layering, and mobile behavior.",
      owner: "SB08",
    };
  }

  if (["DataDisplay", "DataVisualization", "Lists", "Cards", "Storage"].includes(group) || component.project === "CanDoItAll.Components.Charts" || component.project === "CanDoItAll.Components.Mermaid") {
    return {
      category: "Standard data-display component",
      recommendation: "Validate dense labels, empty states, chart/diagram nonblank rendering, and wrapper boundaries.",
      owner: "SB09",
    };
  }

  if (["ProjectRoot", "Infrastructure", "Models", "Internal"].includes(group) || component.project === "CanDoItAll.Components.Common") {
    return {
      category: "Shared base/helper/model",
      recommendation: "Isolate reusable primitives, remove duplicate enum/service definitions, and add contract tests before consumers migrate.",
      owner: "SB03",
    };
  }

  if (oldMatch) {
    return {
      category: "Standard component with AppComponents duplicate",
      recommendation: "Compare old behavior before deleting app copy; port improvements and add compatibility guidance.",
      owner: "SB04",
    };
  }

  return {
    category: "Standard component",
    recommendation: "Keep in standard component publishing scope and include in one-by-one visual QA matrix.",
    owner: "SB11",
  };
}

function parseSandboxRegistry() {
  const registryPath = path.join(repoRoot, "src", "CanDoItAll.Components.Sandbox", "SandboxCatalogRegistry.cs");
  const text = readText(registryPath);
  const groups = [];
  const examples = [];

  const groupPattern = /new\(\s*SandboxGroupKey\.(\w+),\s*"([^"]+)",\s*"([^"]+)",\s*"([^"]+)"/gms;
  for (const match of text.matchAll(groupPattern)) {
    groups.push({
      key: match[1],
      title: match[2],
      route: match[3],
      summary: match[4],
    });
  }

  for (const line of text.split(/\r?\n/)) {
    const trimmed = line.trim();
    if (!trimmed.startsWith("CreateExample(") && !trimmed.startsWith("CreateCustomExample(")) {
      continue;
    }

    const strings = [...trimmed.matchAll(/"([^"]+)"/g)].map((match) => match[1]);
    const groupMatch = trimmed.match(/SandboxGroupKey\.(\w+)/);
    const scenarioMatch = trimmed.match(/SandboxScenarioKey\.(\w+)/);
    if (strings.length < 3 || !groupMatch) {
      continue;
    }

    const id = strings[0];
    const isCustom = trimmed.startsWith("CreateCustomExample(");
    const title = strings[1];
    const route = isCustom ? strings[2] : `${groups.find((group) => group.key === groupMatch[1])?.route ?? ""}?scenario=${scenarioMatch?.[1] ?? "HappyPath"}`;
    const summaryIndex = isCustom ? 3 : 2;
    const componentNames = strings.slice(summaryIndex + 1).filter((value) => /^[A-Z][A-Za-z0-9]+$/.test(value));
    examples.push({
      id,
      group: groupMatch[1],
      title,
      route,
      scenario: scenarioMatch?.[1] ?? "HappyPath",
      componentNames,
    });
  }

  return { groups, examples };
}

function buildStandardInventory() {
  const allStandardFiles = standardProjects.flatMap((project) => listFiles(path.join(repoRoot, project), [".razor", ".cs", ".css", ".csproj"]));
  const appFiles = [
    ...listFiles(path.join(appComponentsRoot, "Components"), [".razor", ".cs", ".css"]),
    ...listFiles(path.join(appComponentsRoot, "Primitives"), [".razor", ".cs", ".css"]),
  ];
  const appByName = new Map();
  for (const appFile of appFiles) {
    const text = readText(appFile);
    const name = semanticComponentName(appFile);
    if (!appByName.has(name)) {
      appByName.set(name, []);
    }

    appByName.get(name).push({
      name,
      absolutePath: appFile,
      relativePath: toPosix(path.relative(appComponentsRoot, appFile)),
      extension: path.extname(appFile),
      hash: sha256(normalizeSourceForCompare(text)),
      lineCount: text.split(/\r?\n/).length,
      parameters: extractParameterNames(text),
      hasInFlightGuard: /isClickInFlight/.test(text),
      hasJsInterop: /IJSRuntime|JSInvokable|DotNetObjectReference/.test(text),
      usesBaseLib: /CanDoItAll\.Components\.BaseLib/.test(text),
    });
  }

  const { groups: sandboxGroups, examples: sandboxExamples } = parseSandboxRegistry();
  const sandboxCoverage = new Map();
  for (const example of sandboxExamples) {
    for (const componentName of example.componentNames) {
      if (!sandboxCoverage.has(componentName)) {
        sandboxCoverage.set(componentName, []);
      }

      sandboxCoverage.get(componentName).push(`${example.group}/${example.scenario}/${example.route}`);
    }
  }

  const components = [];
  for (const filePath of allStandardFiles) {
    const relativePath = relFromRepo(filePath);
    const text = readText(filePath);
    const name = semanticComponentName(filePath);
    const extension = path.extname(filePath).toLowerCase();
    const project = inferProject(relativePath);
    const group = inferGroup(relativePath);
    const oldMatches = appByName.get(name) ?? [];
    const component = {
      name,
      project,
      group,
      kind: extension === ".razor" ? "Razor component" : extension === ".css" ? "CSS asset" : extension === ".csproj" ? "Project file" : "C# support",
      relativePath,
      repoRef: repoRef(relativePath),
      lineCount: text.split(/\r?\n/).length,
      parameterCount: extractParameterNames(text).length,
      parameters: extractParameterNames(text),
      eventCallbackCount: countMatches(text, /EventCallback/g),
      renderFragmentCount: countMatches(text, /RenderFragment/g),
      inheritsStyledBase: /@inherits\s+StyledComponentBase|:\s*StyledComponentBase/.test(text),
      usesAdditionalAttributes: /AdditionalAttributes|CaptureUnmatchedValues|BuildAttributes/.test(text),
      hasCssIsolation: fs.existsSync(filePath.replace(/\.razor$/i, ".razor.css")),
      hardcodedClassAssignments: countMatches(text, /class="[^"]+"/g),
      inlineStyleAssignments: countMatches(text, /style="[^"]+"|Style\s*=/g),
      usesTailwindUtilityStrings: /(?:\bflex\b|\bgrid\b|\bpx-\d|\bpy-\d|\btext-\w|\bbg-\w|\bborder-\w|\brounded-)/.test(text),
      usesCdaCssClasses: /cda-|cad-|zy-|rz-/.test(text),
      usesLegacyCompatibilityLook: /Compatibility|Legacy|SheetCard|zy-|btn\b/.test(text),
      sandboxExamples: sandboxCoverage.get(name) ?? [],
      appComponentsDuplicateCount: oldMatches.length,
      appComponentsDuplicates: oldMatches.map((match) => match.absolutePath),
      oldBehaviorSignals: oldMatches.map((match) => [
        match.hasInFlightGuard ? "old-click-in-flight-guard" : null,
        match.hasJsInterop ? "old-js-interop" : null,
        match.usesBaseLib ? "old-uses-baselib" : null,
      ].filter(Boolean)).flat(),
    };
    const classification = classifyComponent(component, oldMatches.length > 0);
    components.push({ ...component, ...classification });
  }

  const tailwindFiles = listFiles(path.join(repoRoot, "Tailwind"), [".css"]).map((filePath) => {
    const text = readText(filePath);
    const relativePath = relFromRepo(filePath);
    const metrics = tailwindUtilitySmell(text);
    return {
      file: relativePath,
      repoRef: repoRef(relativePath),
      lineCount: text.split(/\r?\n/).length,
      selectors: countMatches(text, /^\s*[:.#\w\[\]@][^{]*\{/gm),
      classes: [...new Set([...text.matchAll(/\.([A-Za-z0-9_-]+)/g)].map((match) => match[1]))].sort(),
      ...metrics,
      recommendation: metrics.severity === "High"
        ? "Refactor candidate: separate design-token CSS from utility-composable component classes and replace simple layout declarations with Tailwind @apply where supported."
        : metrics.severity === "Medium"
          ? "Review candidate: keep custom CSS only where tokens, state selectors, or browser features make Tailwind utilities less clear."
          : "Likely acceptable: retain as utility composition or token bridge unless visual proof finds layout defects.",
      owner: metrics.severity === "High" || metrics.severity === "Medium" ? "SB02" : "SB11",
    };
  });

  const appComponents = [...appByName.values()].flat().map((item) => {
    const standardMatches = components.filter((component) => component.name === item.name);
    let classification = "Needs migration audit";
    let recommendation = "Compare behavior against Components library, port useful behavior, then remove or alias old app-level copy.";
    let owner = "SB04";
    if (["AppShell", "AppTabStrip", "AppShellMode", "AppShellNavigationMode", "TunableComponentBoundary", "TuningBoundaryRequest"].includes(item.name)) {
      classification = "Likely app-complex component";
      recommendation = "Keep in AppComponents only if it remains app-specific; refactor it to consume Components primitives and remove basic primitive definitions from its local scope.";
      owner = "SB04";
    } else if (standardMatches.length === 0) {
      classification = "Missing from standard library or app-specific";
      recommendation = "Decide whether this is a generic framework component that belongs in Components or a complex app surface that should stay in AppComponents.";
      owner = "SB04";
    }

    return {
      name: item.name,
      absolutePath: item.absolutePath,
      localRef: item.absolutePath,
      relativePath: item.relativePath,
      lineCount: item.lineCount,
      parameterCount: item.parameters.length,
      hasStandardMatch: standardMatches.length > 0,
      standardMatches: standardMatches.map((component) => component.repoRef),
      oldBehaviorSignals: [
        item.hasInFlightGuard ? "click-in-flight-guard" : null,
        item.hasJsInterop ? "js-interop" : null,
        item.usesBaseLib ? "uses-baselib" : null,
      ].filter(Boolean),
      classification,
      recommendation,
      owner,
    };
  });

  const projectInventory = standardProjects.map((project) => {
    const root = path.join(repoRoot, project);
    const files = listFiles(root, [".razor", ".cs", ".css", ".csproj"]);
    const componentsInProject = components.filter((component) => component.relativePath.startsWith(toPosix(project)));
    return {
      project,
      repoRef: repoRef(project),
      files: files.length,
      razorComponents: componentsInProject.filter((component) => component.kind === "Razor component").length,
      cssAssets: componentsInProject.filter((component) => component.kind === "CSS asset").length,
      duplicateWithAppComponents: componentsInProject.filter((component) => component.appComponentsDuplicateCount > 0).length,
      sandboxCoveredComponents: componentsInProject.filter((component) => component.sandboxExamples.length > 0).length,
    };
  });

  const findings = [
    {
      id: "F01",
      severity: "High",
      title: "Old AppComponents contains many basic primitives duplicated by BaseLib/Charts.",
      evidence: "Button, Card, TextBox, DropDown, Tabs, Chart, DataGrid, ProgressBar and related primitive enums exist in both repositories.",
      owner: "SB04",
    },
    {
      id: "F02",
      severity: "High",
      title: "No non-WebGL/non-Canvas test project is present for standard components.",
      evidence: "tests/ currently contains only CanDoItAll.Components.WebGlLib.Tests and CanDoItAll.Components.WebGlRunLib.Tests.",
      owner: "SB10",
    },
    {
      id: "F03",
      severity: "High",
      title: "Tailwind inputs include a mix of @apply and raw CSS layout/property declarations.",
      evidence: "Tailwind/input.css, controls/buttons.css, navigation/tabs.css, foundation/theme.css and forms/tag-editor.css contain substantial raw declarations and token work.",
      owner: "SB02",
    },
    {
      id: "F04",
      severity: "Medium",
      title: "Base component inheritance and attribute merging are not universal.",
      evidence: "StyledComponentBase centralizes Class, Style, AdditionalAttributes, but support models, compatibility shims, and some wrappers need a deliberate boundary.",
      owner: "SB03",
    },
    {
      id: "F05",
      severity: "High",
      title: "Sandbox is grouped, but coverage is sampled rather than one-to-one for every standard component.",
      evidence: "SandboxCatalogRegistry defines groups and examples, but many BaseLib components have zero direct example references.",
      owner: "SB05",
    },
    {
      id: "F06",
      severity: "Medium",
      title: "Canvas is still grouped inside the main sandbox registry.",
      evidence: "SandboxGroupKey includes Canvas and Examples include canvas preview entries, while this publishing bundle must exclude Canvas/WebGL work.",
      owner: "SB05",
    },
    {
      id: "F07",
      severity: "Medium",
      title: "Some old AppComponents behavior may be newer than BaseLib and must not be deleted blindly.",
      evidence: "Old AppComponents Button has an isClickInFlight guard; BaseLib Button adds anchors and compatibility looks but lacks that guard in current inspection.",
      owner: "SB04",
    },
    {
      id: "F08",
      severity: "High",
      title: "UI publishing proof must be visual and interactive, not source-only.",
      evidence: "Dropdowns, dialogs, tooltips, sticky action footers, tabs, and long-text/dense states require open-state screenshots and browser assertions.",
      owner: "SB11",
    },
  ];

  return {
    generatedAt: new Date().toISOString(),
    repoRoot,
    bundleRoot,
    appComponentsRoot,
    projectInventory,
    components,
    tailwindFiles,
    appComponents,
    sandboxGroups,
    sandboxExamples,
    findings,
  };
}

function markdownTable(headers, rows) {
  const escapeCell = (value) => String(value ?? "").replaceAll("|", "\\|").replace(/\r?\n/g, "<br>");
  return [
    `| ${headers.join(" | ")} |`,
    `| ${headers.map(() => "---").join(" | ")} |`,
    ...rows.map((row) => `| ${row.map(escapeCell).join(" | ")} |`),
  ].join("\n");
}

function buildInventoryMarkdown(data) {
  const highPriorityComponents = data.components
    .filter((component) => component.appComponentsDuplicateCount > 0 || component.sandboxExamples.length === 0 || component.category.includes("component"))
    .slice(0, 80);

  return `# Scope Inventory

Generated: \`${data.generatedAt}\`

## Project Inventory

${markdownTable(
  ["Project", "Files", "Razor", "CSS", "App Duplicate Matches", "Sandbox-Covered"],
  data.projectInventory.map((project) => [
    project.repoRef,
    project.files,
    project.razorComponents,
    project.cssAssets,
    project.duplicateWithAppComponents,
    project.sandboxCoveredComponents,
  ]),
)}

## Standard Component Inventory Highlights

The detailed row-by-row map lives in \`${bundleRef("inventories/standard-components-publishing-map.xlsx")}\` and \`${bundleRef("inventories/current-state-data.json")}\`.

${markdownTable(
  ["Name", "Project", "Group", "Category", "Sandbox Examples", "App Duplicates", "Owner", "Recommendation"],
  highPriorityComponents.map((component) => [
    component.name,
    component.project,
    component.group,
    component.category,
    component.sandboxExamples.length,
    component.appComponentsDuplicateCount,
    component.owner,
    component.recommendation,
  ]),
)}

## Tailwind And CSS Inventory

${markdownTable(
  ["File", "Lines", "@apply", "Raw Decls", "Media", "Vars", "Severity", "Owner", "Recommendation"],
  data.tailwindFiles.map((file) => [
    file.repoRef,
    file.lineCount,
    file.applyCount,
    file.rawCssDeclarations,
    file.mediaCount,
    file.cssVarCount,
    file.severity,
    file.owner,
    file.recommendation,
  ]),
)}

## AppComponents Duplicate Inventory

${markdownTable(
  ["Name", "Old Path", "Standard Match", "Signals", "Classification", "Owner", "Recommendation"],
  data.appComponents.map((component) => [
    component.name,
    component.localRef,
    component.hasStandardMatch ? component.standardMatches.join("<br>") : "No",
    component.oldBehaviorSignals.join(", "),
    component.classification,
    component.owner,
    component.recommendation,
  ]),
)}
`;
}

function buildCurrentStateMarkdown(data) {
  const counts = {
    standardRows: data.components.length,
    razorComponents: data.components.filter((component) => component.kind === "Razor component" && component.project !== "CanDoItAll.Components.Sandbox").length,
    appDuplicates: data.appComponents.filter((component) => component.hasStandardMatch).length,
    uncovered: data.components.filter((component) => component.kind === "Razor component" && component.project !== "CanDoItAll.Components.Sandbox" && component.sandboxExamples.length === 0).length,
    tailwindHigh: data.tailwindFiles.filter((file) => file.severity === "High").length,
    tailwindMedium: data.tailwindFiles.filter((file) => file.severity === "Medium").length,
  };

  return `# Current State

Generated from repository inspection on \`${data.generatedAt}\`.

## Scope Boundary

- Included: standard component libraries in \`src/CanDoItAll.Components.BaseLib\`, \`src/CanDoItAll.Components.Common\`, \`src/CanDoItAll.Components.Charts\`, \`src/CanDoItAll.Components.OverlayLib\`, \`src/CanDoItAll.Components.Mermaid\`, \`src/CanDoItAll.Components.Sandbox\`, and \`Tailwind\`.
- Included for duplicate analysis only: \`C:\\repositories\\CanDoItAll\\src\\CanDoItAll.AppComponents\\Components\` and \`C:\\repositories\\CanDoItAll\\src\\CanDoItAll.AppComponents\\Primitives\`.
- Excluded from implementation scope: \`src/CanDoItAll.Components.WebGlLib\`, \`src/CanDoItAll.Components.WebGlRunLib\`, \`src/CanDoItAll.Components.WebGlSandbox\`, and \`src/CanDoItAll.Components.CanvasLib\`. Canvas entries in the standard sandbox are inventory evidence only and must be split out or isolated by a later subbundle.

## Quantitative Snapshot

- Standard inventory rows: ${counts.standardRows}.
- Standard non-sandbox Razor components: ${counts.razorComponents}.
- Old AppComponents rows with a standard-name match: ${counts.appDuplicates}.
- Standard non-sandbox Razor components with no direct sandbox registry example: ${counts.uncovered}.
- Tailwind/CSS files with high refactor pressure: ${counts.tailwindHigh}; medium pressure: ${counts.tailwindMedium}.
- Non-WebGL/non-Canvas test projects found: 0.

## Main Findings

${data.findings.map((finding) => `- \`${finding.id}\` ${finding.severity}: ${finding.title} Evidence: ${finding.evidence} Owner: \`${finding.owner}\`.`).join("\n")}

## Implementation-Relevant Observations

- \`StyledComponentBase\` centralizes \`Class\`, \`Style\`, \`AdditionalAttributes\`, and helpers around \`ComponentAttributeExtensions.WithClassAndStyle\`, but the bundle must decide which components should inherit it and which support models/helpers belong in \`Common\` versus \`BaseLib\`.
- \`Button\` in BaseLib supports anchors, token text, material icon fallback, and compatibility looks. The old app copy has an in-flight click guard; SB04 must compare and port that behavior if still wanted.
- \`DropDown\` in BaseLib adds \`InputLook\` and accessible label handling over the old app copy, but still renders a native \`select\`; Playwright proof should cover long option text and form-field label association.
- \`SandboxCatalogRegistry\` has group-level proof questions and scenario state, but group examples are representative rather than exhaustive. Publishing readiness needs a visual matrix where each standard component has at least one direct example and every interactive overlay/dropdown/dialog state has open-state proof.
- The Tailwind input set already uses \`@apply\`, but also contains raw layout, sizing, color, shadow, and media declarations. SB02 must separate acceptable token/state CSS from declarations that should become Tailwind utility composition.
`;
}

function buildAssumptionsMarkdown() {
  return `# Assumptions And Risks

## Assumptions

- The new pure repositories will receive polished output later; this bundle prepares and hardens source before transfer.
- WebGL and Canvas implementation work are out of scope and will be handled in separate preparation.
- Existing compatibility components may be retained temporarily only when they are migration aids with clear deprecation or alias policy.
- AppComponents in the main CanDoItAll repo should keep only complex app-specific surfaces after basic primitives are migrated or removed.

## Critical Path Risks

- SB02, SB03, SB04, and SB05 are critical foundations. If Tailwind policy, shared bases, duplicate migration, or sandbox taxonomy are wrong, every later visual hardening phase can produce misleading proof.
- Removing old AppComponents copies without behavior comparison could drop improvements that never made it into BaseLib.
- Visual styling refactors cannot be trusted without real browser screenshots at desktop and narrow widths.
- A sandbox that keeps Canvas and standard components in one undifferentiated catalog can hide missing standard-component coverage.

## Validation Risks

- The repo currently has no standard-component test project, so early proof may start as build plus Playwright visual proof until SB10 adds contract tests.
- Native browser controls such as \`select\` expose OS/browser styling limits; the proof plan must record what can and cannot be asserted visually.
- Overlay proof needs open-state capture for dialogs, help popovers, tooltips, context menus, notification toasts, and sticky footers; source-only checks are insufficient.
- Tailwind refactors can pass builds while breaking wrapping, width usage, or parent clipping; every affected component needs screenshot review questions answered in the execution report.

## Reopen Triggers

- Reopen SB02 if a later screenshot shows layout, wrapping, or alignment defects caused by a shared Tailwind utility or token decision.
- Reopen SB03 if a later component needs duplicated \`Class\`, \`Style\`, \`AdditionalAttributes\`, enum, service, or CSS-class composition logic.
- Reopen SB04 if old AppComponents behavior is discovered in production usage after a migration/deletion decision.
- Reopen SB05 if a later subbundle cannot capture Playwright proof because the sandbox lacks a focused route, scenario, or test hook.
- Reopen the owning visual-hardening subbundle when Playwright captures text overflow, clipping, dead space, insufficient width/height stretching, or inaccessible interactive state.
`;
}

function buildRequirementsMarkdown() {
  const rows = [
    ["R01", "Preserve publishing-prep scope and exclude WebGL/Canvas implementation.", "README, inputs, plan", "SB01", "Scope files and validators cite excluded paths."],
    ["R02", "Map all standard component, sandbox, style, and AppComponents duplicate surfaces in an xlsx.", "inventories/standard-components-publishing-map.xlsx", "SB01", "Workbook render and data inspection pass."],
    ["R03", "Define Tailwind styling policy before component-by-component visual changes.", "architecture/01-target-solution.md, SB02", "SB02", "CSS metrics, refactor rules, build proof, screenshots."],
    ["R04", "Isolate shared bases/helpers/primitives for easier maintenance.", "SB03", "SB03", "Contract tests and source assertions for helpers/base inheritance."],
    ["R05", "Audit and reduce old AppComponents basic primitives.", "SB04", "SB04", "Duplicate matrix, migration tests, old/new behavior comparison."],
    ["R06", "Split and improve sandbox groups to match component-framework best practice.", "SB05", "SB05", "Sandbox route matrix, coverage counts, Playwright proof setup."],
    ["R07", "Visually harden forms and inputs one by one.", "SB06", "SB06", "Desktop/mobile screenshots, long text, disabled, dense, dropdown action proof."],
    ["R08", "Visually harden actions, badges, and feedback components.", "SB07", "SB07", "Icon/text wrapping, loading, empty, notification proof."],
    ["R09", "Visually harden layout, navigation, and overlay components.", "SB08", "SB08", "Available-space, clipping, open overlay, keyboard/focus proof."],
    ["R10", "Visually harden data-display, chart, and diagram components.", "SB09", "SB09", "Dense labels, empty states, chart/diagram nonblank rendering proof."],
    ["R11", "Add publishing/packaging/API hardening for standard libraries.", "SB10", "SB10", "Pack/build/API approvals, compatibility policy, non-WebGL tests."],
    ["R12", "Run a full Playwright visual validation matrix.", "SB11", "SB11", "Browser analytics rows with screenshot paths and assertions."],
    ["R13", "Finish transfer readiness with raw-note closure and red-team proof.", "SB12", "SB12", "Final validator, closure table, follow-up scope for Canvas/WebGL."],
  ];

  return `# Normalized Requirements

## Requirements

${markdownTable(["ID", "Requirement", "Bundle Destination", "Owning Subbundle", "Observable Acceptance"], rows)}

## Hard Constraints

- Do not implement WebGL or Canvas refactors in this bundle.
- Do not remove old AppComponents primitives until behavior comparison and consumer migration proof exist.
- Do not accept CSS/styling changes without Playwright MCP screenshots and explicit visual review answers.
- Do not call the bundle ready for execution unless the xlsx, source inventory, dependency map, subbundle gates, and self-review all agree.
`;
}

function buildArchitectureMarkdown() {
  return `# Target Solution

## Target Publishing Shape

- \`CanDoItAll.Components.Common\`: small non-UI helper and primitive contracts that can be shared without pulling Razor dependencies.
- \`CanDoItAll.Components.BaseLib\`: core standard Razor components, shared base classes, compatibility shims with clear migration policy, and standard CSS classes generated from Tailwind input.
- \`CanDoItAll.Components.Charts\`, \`CanDoItAll.Components.Mermaid\`, and \`CanDoItAll.Components.OverlayLib\`: optional standard packages with clear service/assets registration and visual proof routes.
- \`CanDoItAll.Components.Sandbox\`: a standard-component proof harness with logical component-framework groups, separated from Canvas/WebGL proof surfaces.
- \`CanDoItAll.AppComponents\`: complex app-level surfaces only; no duplicated basic Button/Card/Input/Navigation/DataDisplay primitives unless temporarily aliased for migration.

## Styling Policy

- Prefer Tailwind \`@apply\` for simple layout, spacing, typography, border, sizing, and flex/grid composition inside Tailwind input files.
- Keep raw CSS where it expresses design tokens, CSS variables, pseudo-elements, Radzen/third-party interop, browser features such as \`color-mix\`, media queries that encode component-specific behavior, or state selectors that would be less maintainable as repeated utilities.
- Remove ad-hoc one-off component styling from Razor pages when it can be expressed through shared component parameters, shared classes, or Tailwind component-layer classes.
- Every styling refactor must be validated with Playwright screenshots, including long text and constrained width.

## Sandbox Taxonomy Target

- Foundations: typography, icons, theme, tokens.
- Inputs: text, numeric, selection, boolean, editable, upload, secret.
- Actions: buttons, copy actions, inline actions.
- Navigation: tabs, steps, menus, toolbars, tree/list navigation.
- Feedback: alerts, notifications, tooltips, help, loading, empty, status.
- Layout: stack, grid, row/column, split, page shells, sticky footers.
- Data Display: cards, lists, badges/chips, stats, timelines, tables.
- Visualization: charts and Mermaid diagrams.
- Overlays: dialogs, popovers, contextual menus, overlay windows where standard.
- Excluded: Canvas/WebGL routes and proof are separate bundle scopes.

## Checkpoint Strategy

- Checkpoint A after SB02 and SB03: shared style/base foundations are stable enough for component work.
- Checkpoint B after SB04 and SB05: duplicate migration and sandbox proof harness are trustworthy.
- Checkpoint C after SB06 through SB09: component groups have visual proof and source/test changes are coherent.
- Checkpoint D after SB10 and SB11: packaging/API and full visual matrix are ready for final transfer.
`;
}

function buildPhasePlanMarkdown() {
  return `# Phase Plan

## Phase Sequence

1. SB01 freezes scope and current-state inventory, including the mandatory xlsx.
2. SB02 and SB03 establish critical styling/base foundations.
3. SB04 and SB05 compare old AppComponents and make the sandbox a trustworthy proof harness.
4. SB06 through SB09 harden standard component groups with visual proof.
5. SB10 adds publishing, compatibility, API, and test hardening.
6. SB11 runs the full Playwright visual validation matrix.
7. SB12 performs final red-team transfer readiness and raw-note closure.

## Subbundle Dependency Map

\`\`\`mermaid
gantt
title Standard components publishing-readiness dependency map
dateFormat  YYYY-MM-DD
axisFormat  %m-%d
section Inventory
SB01 inventory and scope freeze                  :crit, sb01, 2026-06-28, 1d
section Foundations
SB02 Tailwind styling foundation                 :crit, sb02, after sb01, 1d
SB03 shared bases helpers primitives             :crit, sb03, after sb01, 1d
Checkpoint A foundation review                   :milestone, cpa, after sb03, 0d
section Migration And Harness
SB04 AppComponents duplicate audit               :crit, sb04, after cpa, 1d
SB05 sandbox taxonomy and coverage               :crit, sb05, after cpa, 1d
Checkpoint B migration and harness review        :milestone, cpb, after sb05, 0d
section Component Hardening
SB06 forms and inputs                            :sb06, after cpb, 1d
SB07 actions badges feedback                     :sb07, after cpb, 1d
SB08 layout navigation overlays                  :sb08, after cpb, 1d
SB09 data display charts diagrams                :sb09, after cpb, 1d
Checkpoint C component visual review             :milestone, cpc, after sb09, 0d
section Publishing Proof
SB10 packaging compatibility API tests           :crit, sb10, after cpc, 1d
SB11 full Playwright visual matrix               :crit, sb11, after sb10, 1d
Checkpoint D release-candidate review            :milestone, cpd, after sb11, 0d
section Closure
SB12 final transfer readiness audit              :crit, sb12, after cpd, 1d
\`\`\`

## Critical Subbundles

- SB01 is a critical foundation because all later scope and duplicate decisions depend on its inventory.
- SB02 is a critical foundation because shared Tailwind changes can affect every component screenshot.
- SB03 is a critical foundation because shared base/helper decisions control attribute merging, class composition, and primitive duplication.
- SB04 is a critical foundation because AppComponents removal can break the main app or drop old behavior.
- SB05 is a critical foundation because later Playwright proof depends on sandbox route coverage and test hooks.
- SB10, SB11, and SB12 are critical closure foundations because they determine publishing readiness and transfer risk.

Every critical subbundle requires a Semantic Adequacy Gate, \`proof/SBxx/manifest.md\`, \`proof/SBxx/semantic-invariants.md\`, command transcripts, source assertions, anti-stub audit, and failing-first or explicit non-behavior exemption as applicable.

## Phase Gates

- Prepared gate: run \`python scripts/validate_bundle.py . --profile initiative --stage prepared --repo-root C:\\repositories\\CanDoItAll.Components\` from the bundle root and repair failures.
- Entry gate for each subbundle: run the subbundle validator against root README, this phase plan, the subbundle README, and relevant traceability rows before editing source.
- Checkpoint A: SB02 and SB03 closure proof must include source assertions, build/test transcripts, and at least one dependent sandbox screenshot smoke before SB04-SB09 start.
- Checkpoint B: SB04 and SB05 must prove duplicate decisions and sandbox coverage are sufficient before group hardening starts.
- Checkpoint C: SB06-SB09 must update browser analytics rows with screenshot paths and explicit review answers before publishing hardening.
- Checkpoint D: SB10-SB11 must prove package/API/build and visual matrix readiness before SB12 closure.
- Final closure: run completed-stage validator, close each raw note as Solved, Partially solved, or Not solved, and document WebGL/Canvas follow-up separately.
`;
}

function buildTraceabilityMarkdown() {
  const rows = [
    ["RAW01", "Preparation of repository for publishing.", "R01,R13", "SB01,SB12", "Scope freeze plus final transfer readiness report."],
    ["RAW02", "Detailed study of actual implementation and identify refactoring/hardening.", "R02,R03,R04,R06-R12", "SB01-SB12", "Inventory workbook, current-state analysis, subbundle proof."],
    ["RAW03", "Focus only on standard components, not WebGL and Canvas.", "R01", "SB01,SB05,SB12", "Excluded scope files and sandbox split proof."],
    ["RAW04", "Analyze sandbox missing components and logical grouping.", "R06", "SB05", "Coverage matrix and Playwright route plan."],
    ["RAW05", "Use Tailwind for component styling and inspect custom CSS/hacks.", "R03", "SB02", "Tailwind metrics, refactor policy, screenshots."],
    ["RAW06", "Audit main CanDoItAll AppComponents duplicate basic components.", "R05", "SB04", "Duplicate matrix and migration closure rows."],
    ["RAW07", "Map all in xlsx with correct references and explanations.", "R02", "SB01", "standard-components-publishing-map.xlsx."],
    ["RAW08", "Identify phases; first general foundations like input Tailwind and base isolation.", "R03,R04", "SB02,SB03", "Phase plan and critical subbundle gates."],
    ["RAW09", "Design subbundles with refactoring checkpoints.", "R01-R13", "SB01-SB12", "Checkpoint A-D in phase plan."],
    ["RAW10", "Real Playwright MCP screenshots one by one, including interactive states.", "R07-R12", "SB06-SB11", "Browser analytics and proof requirements."],
  ];

  return `# Requirement Traceability

${markdownTable(["Raw Note", "Exact Input Wording", "Requirements", "Owning Subbundles", "Planned Proof"], rows)}
`;
}

function buildExecutionReportMarkdown() {
  return `# Execution Report

## Status

Execution status: \`Not started\`

Prepared bundle only. No production component source has been modified by this preparation pass.

## Subbundle Gate Results

| Subbundle | Entry gate | Closure gate | Downstream dependencies checked | Progression result | Notes |
|---|---|---|---|---|---|
| SB01 | Pending | Pending | Pending | Pending | Inventory and xlsx are prepared artifacts; implementation still requires entry gate. |
| SB02 | Pending | Pending | Pending | Pending | Critical foundation. |
| SB03 | Pending | Pending | Pending | Pending | Critical foundation. |
| SB04 | Pending | Pending | Pending | Pending | Critical foundation. |
| SB05 | Pending | Pending | Pending | Pending | Critical foundation. |
| SB06 | Pending | Pending | Pending | Pending | Depends on SB02-SB05. |
| SB07 | Pending | Pending | Pending | Pending | Depends on SB02-SB05. |
| SB08 | Pending | Pending | Pending | Pending | Depends on SB02-SB05. |
| SB09 | Pending | Pending | Pending | Pending | Depends on SB02-SB05. |
| SB10 | Pending | Pending | Pending | Pending | Depends on SB06-SB09. |
| SB11 | Pending | Pending | Pending | Pending | Depends on SB10. |
| SB12 | Pending | Pending | Pending | Pending | Depends on SB11. |

## Browser Validation Analytics

| Subbundle | Route | Viewport | Playwright MCP evidence | Screenshots | Result |
|---|---|---|---|---|---|
| SB06 | /groups/inputs plus focused routes created by SB05 | Maximized desktop, 1366x900, 390x844 | Pending | Pending | Pending |
| SB07 | /groups/actions and /groups/feedback | Maximized desktop, 1366x900, 390x844 | Pending | Pending | Pending |
| SB08 | /groups/layout, /groups/navigation, /groups/overlays | Maximized desktop, 1366x900, 390x844 | Pending | Pending | Pending |
| SB09 | /groups/data-display, /groups/charts, /groups/mermaid | Maximized desktop, 1366x900, 390x844 | Pending | Pending | Pending |
| SB11 | every standard component route/scenario in the final matrix | Maximized desktop, tablet, mobile | Pending | Pending | Pending |

## Analytics Review

- Pending implementation. Each UI subbundle must answer readability, clipping, wrapping, layering, available-space use, disabled/loading/empty state, and interactive-open-state review questions while screenshots are fresh.

## Raw Note Closure

| Raw note | Status | Proof |
|---|---|---|
| RAW01 | Pending | Planned SB01/SB12. |
| RAW02 | Pending | Planned SB01-SB12. |
| RAW03 | Pending | Planned SB01/SB05/SB12. |
| RAW04 | Pending | Planned SB05. |
| RAW05 | Pending | Planned SB02. |
| RAW06 | Pending | Planned SB04. |
| RAW07 | Pending | Planned SB01 xlsx. |
| RAW08 | Pending | Planned SB02/SB03. |
| RAW09 | Pending | Planned phase checkpoints. |
| RAW10 | Pending | Planned SB06-SB11 Playwright proof. |
`;
}

function buildSelfReviewMarkdown() {
  return `# Bundle Self Review

## QA Review

- Raw request preserved: Pass.
- Source artifacts listed: Pass.
- Requirements normalized with literal must/all/every language preserved: Pass.
- Every raw note maps to at least one owning subbundle: Pass.
- UI proof requires Playwright MCP screenshots and review questions: Pass.
- Remaining QA concern: actual screenshot evidence is intentionally pending implementation.

## Senior C# Blazor Architect Review

- Real source files and projects are named: Pass.
- WebGL/Canvas implementation excluded: Pass.
- Shared library ownership and AppComponents boundary are explicit: Pass.
- Critical foundations are labeled before visual work: Pass.
- Remaining architect concern: SB04 must compare old behavior before deleting duplicates.

## Senior Manager Review

- Critical path is explicit in the mermaid map: Pass.
- Checkpoints force refactoring reviews between work groups: Pass.
- Mandatory xlsx is present as a planned and generated artifact: Pass.
- Execution can proceed phase by phase without guessing: Pass, subject to prepared-stage validator output.
`;
}

function subbundleReadme(id, slug, title, covered, refs, scope, dependency, validation, steps, acceptance, proof, browser, prompt, critical = false) {
  const criticalText = critical
    ? "\n- Critical foundation: before closure, create `proof/" + id + "/manifest.md` and `proof/" + id + "/semantic-invariants.md` with Semantic Adequacy Gate evidence, changed-file hashes, transcripts, source assertions, anti-stub audit, and raw-note literal closure.\n"
    : "";

  return `# ${id} ${title}

## Status

- Status: \`Ready\`

## Objective

${scope.objective}

## Covered Inputs

${covered.map((item) => `- ${item}`).join("\n")}

## Prerequisites

${scope.prerequisites.map((item) => `- ${item}`).join("\n")}

## Exact Source References

${refs.map((item) => `- ${item}`).join("\n")}

## Deliverables

${scope.deliverables.map((item) => `- ${item}`).join("\n")}

## Dependency Impact

${dependency.map((item) => `- ${item}`).join("\n")}

## Validation Depth

${validation.map((item) => `- ${item}`).join("\n")}${criticalText}

## Implementation Steps

${steps.map((item) => `- ${item}`).join("\n")}

## Scope Exceptions

- WebGL and Canvas implementation changes are excluded unless this subbundle explicitly updates sandbox separation metadata without touching their component internals.

## Do Not Do

- Do not refactor unrelated WebGL or Canvas source.
- Do not delete compatibility or AppComponents code without migration proof and consumer checks.
- Do not close UI work from source inspection only.

## Acceptance Checklist

${acceptance.map((item) => `- ${item}`).join("\n")}

## Proof Required

${proof.map((item) => `- ${item}`).join("\n")}

## Browser Validation Logging

${browser.map((item) => `- ${item}`).join("\n")}

## Progression Gate

- The subbundle validator must pass closure review before downstream dependent subbundles start.
- If proof is weak or a screenshot shows wrapping, clipping, layout, available-space, or interaction defects, keep this subbundle \`In progress\` and reopen prerequisites as needed.

## Suggested Agent Prompt

${prompt}
`;
}

function buildSubbundles() {
  const raw = {
    RAW01: "RAW01: Preparation of repository for publishing.",
    RAW02: "RAW02: Detailed study of actual implementation and identify all refactoring/hardening.",
    RAW03: "RAW03: Focus only on standard components, not WebGL and Canvas.",
    RAW04: "RAW04: Analyze sandbox missing components and logical grouping.",
    RAW05: "RAW05: Use Tailwind for component styling and inspect custom CSS/hacks.",
    RAW06: "RAW06: Audit duplicate AppComponents basic components.",
    RAW07: "RAW07: Map all in xlsx with references and explanations.",
    RAW08: "RAW08: Identify phases with general foundations first.",
    RAW09: "RAW09: Design subbundles with refactoring checkpoints.",
    RAW10: "RAW10: Real Playwright screenshots one by one, including interactive states.",
  };

  return [
    ["SB01", "current-state-inventory-and-publishing-scope-freeze", "Current State Inventory And Publishing Scope Freeze", [raw.RAW01, raw.RAW02, raw.RAW03, raw.RAW07], [repoRef("README.md"), repoRef("CanDoItAll.Components.slnx"), repoRef("src/CanDoItAll.Components.BaseLib"), repoRef("Tailwind"), "C:\\repositories\\CanDoItAll\\src\\CanDoItAll.AppComponents\\Components"], { objective: "Freeze the standard-component publishing scope and keep the generated inventory/xlsx as the durable handoff for all later work.", prerequisites: ["Prepared bundle exists.", "Repository source is readable from both Components and main CanDoItAll paths."], deliverables: ["Updated inventory JSON and xlsx.", "Current-state analysis with excluded WebGL/Canvas boundary.", "Traceability from raw notes to subbundles."] }, ["All later subbundles depend on this inventory and scope boundary.", "If inventory is incomplete, every duplicate or sandbox decision becomes untrustworthy."], ["Structural bundle validation plus workbook visual verification.", "Manual spot-check of high-risk inventory rows."], ["Regenerate inventory before implementation starts.", "Confirm excluded WebGL/Canvas paths are not planned for code edits.", "Use the xlsx as the canonical row map for subbundle ownership."], ["Inventory workbook exists and opens.", "Every raw note maps to an owning subbundle.", "No WebGL/Canvas implementation scope is included."], ["Prepared-stage validator transcript.", "Workbook inspect/render proof.", "Source assertion that AppComponents and standard paths were scanned."], ["N/A for implementation; SB01 plans UI proof but does not change UI.", "If inventory UI screenshots are taken, log route, viewport, screenshot, and result in execution report."], "Execute SB01 by regenerating and reviewing the inventory artifacts, then run the readiness validator. Do not edit component source.", true],
    ["SB02", "tailwind-component-styling-foundation-hardening", "Tailwind Component Styling Foundation Hardening", [raw.RAW05, raw.RAW08], [repoRef("Tailwind/input.css"), repoRef("Tailwind/forms/fields.css"), repoRef("Tailwind/controls/buttons.css"), repoRef("Tailwind/navigation/tabs.css"), repoRef("Tailwind/foundation/theme.css")], { objective: "Refactor the shared Tailwind input layer before component-by-component fixes so styling rules are consistent and maintainable.", prerequisites: ["SB01 inventory accepted.", "Tailwind/CSS severity rows reviewed."], deliverables: ["Tailwind policy codified in source/docs.", "Simple layout declarations converted to Tailwind @apply where appropriate.", "Token/state CSS retained only with documented rationale."] }, ["Unlocks every visual hardening subbundle.", "Weak Tailwind proof requires reopening before SB06-SB09 continue."], ["Critical Semantic Adequacy Gate.", "Build CSS output and compare changed file hashes.", "Desktop and narrow screenshots for representative input/button/tabs surfaces."], ["Classify raw CSS as token/state/browser-required or refactorable utility composition.", "Refactor in small groups and rebuild output.css.", "Capture before/after screenshots for affected surfaces."], ["No broad visual regression in Inputs, Actions, Navigation.", "Raw CSS that remains has a reason.", "Tailwind build output is updated intentionally."], ["Failing-first or red-team visual assertion for a known refactor target.", "Passing Tailwind build transcript.", "Playwright screenshots for affected routes.", "Anti-stub audit for TODO/NotImplemented in styling scripts."], ["Routes: /groups/inputs, /groups/actions, /groups/navigation/tabs.", "Viewports: maximized desktop, 1366x900, 390x844.", "Open dropdown/tabs states where affected and record clipping/wrapping answers."], "Execute SB02 by hardening Tailwind foundations first. Keep changes small, rebuild CSS, and capture real screenshots before allowing downstream visual work.", true],
    ["SB03", "shared-bases-helpers-and-primitives-isolation", "Shared Bases Helpers And Primitives Isolation", [raw.RAW02, raw.RAW08], [repoRef("src/CanDoItAll.Components.Common/CssClassBuilder.cs"), repoRef("src/CanDoItAll.Components.Common/LayoutPrimitives.cs"), repoRef("src/CanDoItAll.Components.BaseLib/StyledComponentBase.cs"), repoRef("src/CanDoItAll.Components.BaseLib/Components/Forms/FormPrimitives.cs"), repoRef("src/CanDoItAll.Components.BaseLib/Components/Buttons/ButtonPrimitives.cs")], { objective: "Separate reusable helpers, bases, enum primitives, and services so generic components are easier to maintain and AppComponents no longer carries basic definitions.", prerequisites: ["SB01 inventory accepted.", "SB02 style policy known if class output changes."], deliverables: ["Clear Common vs BaseLib ownership.", "Consistent attribute/class/style merge behavior.", "Contract tests for helper behavior."] }, ["Unlocks duplicate migration and every component group.", "If base behavior changes later screenshots must re-check all affected groups."], ["Critical Semantic Adequacy Gate.", "Unit/contract tests for helpers.", "Source assertions for every migrated primitive."], ["Audit duplicated primitive enums/services.", "Move non-Razor helpers to Common only when dependency direction stays clean.", "Standardize component base inheritance where appropriate."], ["No duplicate primitive source remains without an exception.", "Components preserve class/style/additional attribute behavior.", "Build and tests pass."], ["Failing-first helper/primitive tests where behavior changes.", "Passing dotnet test transcript.", "Changed-file hashes and source assertion manifest."], ["N/A unless component markup/base behavior changes. If it does, smoke /groups/inputs and /groups/layout at desktop and mobile."], "Execute SB03 by isolating shared bases and primitives conservatively, proving attribute/class/style semantics before any downstream migration.", true],
    ["SB04", "appcomponents-duplicate-migration-audit", "AppComponents Duplicate Migration Audit", [raw.RAW06], ["C:\\repositories\\CanDoItAll\\src\\CanDoItAll.AppComponents\\Components", "C:\\repositories\\CanDoItAll\\src\\CanDoItAll.AppComponents\\Primitives", repoRef("src/CanDoItAll.Components.BaseLib/Components"), repoRef("src/CanDoItAll.Components.Charts/Components")], { objective: "Reduce old basic components in the main CanDoItAll AppComponents project while preserving useful behavior and app-specific complex surfaces.", prerequisites: ["SB03 base/helper decisions complete.", "Inventory duplicate matrix reviewed."], deliverables: ["Duplicate-by-duplicate migration table.", "Behavior improvements ported to Components or recorded as app-specific exceptions.", "Main app consumers migrated or compatibility aliases defined."] }, ["Unlocks publishing transfer because basic primitives must live in Components.", "Mistakes can break the main CanDoItAll app, so consumer proof is required."], ["Critical Semantic Adequacy Gate.", "Cross-repo source assertions and build proof.", "Behavior comparison tests for old/new copies."], ["Compare old and standard component parameters and behavior.", "Port useful old behavior such as click in-flight guard when appropriate.", "Remove, alias, or mark app-specific each old component."], ["Every old basic component is Solved, Partially solved, or intentionally app-specific.", "Main CanDoItAll app builds after migration.", "No standard primitive remains only in AppComponents unless exception is documented."], ["Cross-repo build transcripts.", "Migration matrix updates.", "Source assertions for deleted/aliased/ported components.", "Playwright proof for migrated visual surfaces if routes exist."], ["Use app and sandbox routes for migrated components when visible.", "Capture desktop and mobile screenshots for any changed component surface.", "Open menus/dialogs/tabs/dropdowns where relevant."], "Execute SB04 as a careful migration audit, not a deletion spree. Compare behavior first, port improvements, then remove or alias old basics with proof.", true],
    ["SB05", "sandbox-taxonomy-and-standard-coverage-expansion", "Sandbox Taxonomy And Standard Coverage Expansion", [raw.RAW04, raw.RAW10], [repoRef("src/CanDoItAll.Components.Sandbox/SandboxCatalogRegistry.cs"), repoRef("src/CanDoItAll.Components.Sandbox/Components/Pages"), repoRef("src/CanDoItAll.Components.Sandbox/Components/Routes.razor")], { objective: "Make the sandbox a reliable standard-component proof harness with logical groups and one-by-one component coverage, while separating Canvas/WebGL proof surfaces.", prerequisites: ["SB01 inventory accepted.", "SB03 public component taxonomy known."], deliverables: ["Revised group taxonomy.", "Coverage matrix for every standard component.", "Focused routes/test hooks for Playwright proof." ] }, ["Unlocks all visual hardening subbundles.", "If sandbox coverage is weak, screenshots cannot prove publishing readiness."], ["Critical Semantic Adequacy Gate.", "Coverage count assertions.", "Playwright route smoke for every group."], ["Split standard proof groups from Canvas/WebGL entries.", "Add missing component examples and scenario coverage.", "Expose stable routes/test IDs for one-by-one validation."], ["Every standard component has an owning group and proof route or a documented exception.", "Canvas/WebGL entries are excluded or clearly separated.", "Playwright can navigate to each planned route."], ["Sandbox route smoke transcript.", "Coverage matrix updated in xlsx or generated JSON.", "Screenshots of group index and representative focused route."], ["Routes: every standard group route plus focused routes introduced by this subbundle.", "Viewports: desktop and mobile.", "Result rows must include component coverage count and screenshot path."], "Execute SB05 by treating the sandbox as test infrastructure. Split scope cleanly, then add missing standard-component demos with stable proof hooks.", true],
    ["SB06", "forms-and-inputs-behavior-visual-hardening", "Forms And Inputs Behavior Visual Hardening", [raw.RAW10], [repoRef("src/CanDoItAll.Components.BaseLib/Components/Forms"), repoRef("Tailwind/forms"), repoRef("src/CanDoItAll.Components.Sandbox/Components/Pages/Inputs.razor")], { objective: "Harden every standard form/input component for behavior, accessibility, wrapping, disabled/loading/long-text states, and available width.", prerequisites: ["Checkpoint B passed.", "SB05 routes available."], deliverables: ["Source fixes for forms/inputs.", "Focused tests where behavior changes.", "Playwright screenshots for each input component."] }, ["Depends on SB02-SB05.", "Findings can reopen Tailwind or sandbox foundations."], ["Group-level UI proof plus behavior tests.", "Open-state proof for dropdown/select/file upload interactions where possible."], ["Validate TextBox, TextArea, Numeric, DropDown, CheckBox, Switch, Slider, Password, SecretField, Editable, FormField, FormSection, FileUpload, TagEditor.", "Fix width/stretch/wrapping/accessibility defects.", "Record each component result in browser analytics."], ["No text overflow or clipped controls in standard scenarios.", "Labels and accessible names are coherent.", "Interactive changes raise expected callbacks."], ["dotnet build/test transcript.", "Playwright screenshots per component and state.", "Source assertions for accessibility/parameter behavior."], ["Route: /groups/inputs and focused input routes.", "Actions: open dropdowns, type long text, toggle switches/checkboxes, drag/drop or picker where feasible.", "Viewports: maximized desktop, 1366x900, 390x844."], "Execute SB06 one component at a time and do not infer styling success without screenshots of each relevant state.", false],
    ["SB07", "actions-badges-and-feedback-visual-hardening", "Actions Badges And Feedback Visual Hardening", [raw.RAW10], [repoRef("src/CanDoItAll.Components.BaseLib/Components/Buttons"), repoRef("src/CanDoItAll.Components.BaseLib/Components/Badges"), repoRef("src/CanDoItAll.Components.BaseLib/Components/Feedback"), repoRef("src/CanDoItAll.Components.Sandbox/Components/Pages/Actions.razor"), repoRef("src/CanDoItAll.Components.Sandbox/Components/Pages/Feedback.razor")], { objective: "Harden actions, badges, and feedback surfaces for icon/text rhythm, long labels, loading, empty, status, notification, and tooltip behavior.", prerequisites: ["Checkpoint B passed.", "SB02 button/feedback styling stable."], deliverables: ["Button/badge/feedback source fixes.", "Behavior tests for services/callbacks.", "Visual proof for states and open overlays." ] }, ["Depends on SB02-SB05.", "Can reopen SB02 if button/alert shared classes fail visual proof."], ["UI proof with interactive states.", "Service behavior tests for notifications/tooltips when changed."], ["Validate Button, CopyButton, Badge, Chip, StatusBadge, Alert, Callout, EmptyState, LoadingState, Notification, Tooltip, HelpPopover, verification/status lists.", "Fix wrapping, icon fallback, disabled/loading states.", "Capture open tooltip/popover/toast states."], ["Icon-only buttons remain accessible.", "Long labels wrap without disrupting layout.", "Feedback states are visually distinct and not clipped."], ["Build/test transcript.", "Playwright screenshots for actions and feedback states.", "Open-state overlay screenshots."], ["Routes: /groups/actions, /groups/feedback, /groups/overlays where tooltip/help is shared.", "Actions: hover/click tooltip, show toast, copy button state, loading/disabled examples.", "Viewports: desktop and mobile."], "Execute SB07 with screenshot-first discipline for every stateful action/feedback component.", false],
    ["SB08", "layout-navigation-and-overlay-hardening", "Layout Navigation And Overlay Hardening", [raw.RAW10], [repoRef("src/CanDoItAll.Components.BaseLib/Components/Layout"), repoRef("src/CanDoItAll.Components.BaseLib/Components/Navigation"), repoRef("src/CanDoItAll.Components.BaseLib/Components/Modals"), repoRef("src/CanDoItAll.Components.OverlayLib"), repoRef("src/CanDoItAll.Components.Sandbox/Components/Pages/Layout.razor"), repoRef("src/CanDoItAll.Components.Sandbox/Components/Pages/Navigation.razor"), repoRef("src/CanDoItAll.Components.Sandbox/Components/Pages/Overlays.razor")], { objective: "Harden structural components that control available space, navigation, overlays, dialogs, and layout composition.", prerequisites: ["Checkpoint B passed.", "SB05 route/test hooks for overlays available."], deliverables: ["Layout/navigation/overlay fixes.", "Open-state visual proof.", "Keyboard/focus checks where feasible." ] }, ["Depends on foundations and feeds later app migration.", "Overlay clipping defects can reopen Tailwind or sandbox foundations."], ["Deep UI proof for responsive layout and open overlays.", "Dependent smoke through migrated AppComponents surfaces if SB04 touched them."], ["Validate Stack, Grid, Row, Column, Split, PageScaffold, PageHeader, Sidebar, WorkspaceSplit, Tabs, SecondaryTabs, Steps, ContextMenu, Toolbar, TreeView, Dialog, DialogHost, StickyActionFooter, OverlayWindow.", "Open every overlay/menu/dialog state and inspect layering/clipping.", "Fix width/height stretch and dead-space defects."], ["Desktop uses available space intentionally.", "Mobile first viewport remains oriented.", "Overlays are readable, unclipped, layered correctly, and dismissible."], ["Build/test transcript.", "Playwright screenshot matrix for open states.", "Keyboard/focus assertions where practical."], ["Routes: /groups/layout, /groups/layout/composition, /groups/navigation, /groups/navigation/tabs, /groups/overlays.", "Actions: open dialogs, popovers, context menus, tooltips, tab overflow, tree expansion.", "Viewports: maximized desktop, 1366x900, 390x844."], "Execute SB08 as the most visual standard-component pass: open the overlays and inspect the actual screenshots before claiming success.", false],
    ["SB09", "data-display-charts-and-diagram-hardening", "Data Display Charts And Diagram Hardening", [raw.RAW10], [repoRef("src/CanDoItAll.Components.BaseLib/Components/DataDisplay"), repoRef("src/CanDoItAll.Components.BaseLib/Components/DataVisualization"), repoRef("src/CanDoItAll.Components.BaseLib/Components/Lists"), repoRef("src/CanDoItAll.Components.BaseLib/Components/Cards"), repoRef("src/CanDoItAll.Components.Charts"), repoRef("src/CanDoItAll.Components.Mermaid"), repoRef("src/CanDoItAll.Components.Sandbox/Components/Pages/DataDisplay.razor"), repoRef("src/CanDoItAll.Components.Sandbox/Components/Pages/Charts.razor"), repoRef("src/CanDoItAll.Components.Sandbox/Components/Pages/Mermaid.razor")], { objective: "Harden standard display, visualization, chart, and diagram wrappers for dense labels, empty states, nonblank rendering, and wrapper boundaries.", prerequisites: ["Checkpoint B passed.", "SB05 focused routes available."], deliverables: ["Data-display/chart/diagram fixes.", "Nonblank browser proof.", "Dense and empty-state screenshots." ] }, ["Depends on sandbox and styling foundations.", "Chart/Mermaid failures can block final visual matrix."], ["UI proof plus nonblank DOM/canvas/svg checks for chart and diagram wrappers.", "Behavior tests for source normalizers/options factories if changed."], ["Validate cards, lists, summary tiles, timeline, progress/data grid primitives, CdaChart, MermaidDiagram.", "Check long labels and dense metadata.", "Assert rendered charts/diagrams are nonblank and readable."], ["Dense data remains scannable.", "Charts and diagrams render nonblank.", "Errors/empty states are informative."], ["Build/test transcript.", "Playwright screenshots.", "Nonblank chart/diagram assertions."], ["Routes: /groups/data-display, /groups/charts, /groups/mermaid.", "Actions: Mermaid click/pan/zoom/error where supported, chart dense/empty states.", "Viewports: desktop and mobile."], "Execute SB09 by proving visualizations actually render and dense display surfaces do not collapse under realistic labels.", false],
    ["SB10", "compatibility-cleanup-packaging-and-public-api-hardening", "Compatibility Cleanup Packaging And Public API Hardening", [raw.RAW01, raw.RAW06], [repoRef("Directory.Build.props"), repoRef("CanDoItAll.Components.slnx"), repoRef("src/CanDoItAll.Components.BaseLib/CanDoItAll.Components.BaseLib.csproj"), repoRef("src/CanDoItAll.Components.Common/CanDoItAll.Components.Common.csproj"), repoRef("src/CanDoItAll.Components.Charts/CanDoItAll.Components.Charts.csproj"), repoRef("src/CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj"), repoRef("src/CanDoItAll.Components.Mermaid/CanDoItAll.Components.Mermaid.csproj")], { objective: "Prepare standard packages for transfer by hardening packability, public API boundaries, compatibility policy, and non-WebGL test coverage.", prerequisites: ["Checkpoint C passed.", "SB04 duplicate decisions complete."], deliverables: ["Standard component test project(s).", "Pack/build validation.", "Compatibility/deprecation documentation.", "Public API or package content approvals where appropriate." ] }, ["Blocks final visual matrix and transfer readiness.", "Can reopen SB03/SB04 if public API exposes wrong primitives."], ["Critical Semantic Adequacy Gate.", "Clean build/test/pack transcripts.", "Package content and public API assertions."], ["Create/extend non-WebGL standard component tests.", "Audit IsPackable and project references.", "Lock public API/package content for standard libraries.", "Document compatibility shims and removal path."], ["Standard package build/test/pack succeeds.", "Compatibility shims have owner and migration path.", "No Canvas/WebGL changes required."], ["dotnet build/test/pack transcripts.", "Changed-file hashes.", "API/package assertion artifacts.", "Anti-stub audit."], ["N/A unless docs/sandbox UI changes; if UI docs routes change, capture affected route screenshots."], "Execute SB10 after visual groups are stable. Treat package/API proof as a release gate, not a paperwork step.", true],
    ["SB11", "full-playwright-visual-validation-matrix", "Full Playwright Visual Validation Matrix", [raw.RAW10], [repoRef("src/CanDoItAll.Components.Sandbox"), repoRef("src/CanDoItAll.Components.BaseLib"), repoRef("src/CanDoItAll.Components.Charts"), repoRef("src/CanDoItAll.Components.OverlayLib"), repoRef("src/CanDoItAll.Components.Mermaid")], { objective: "Run the complete browser proof program across every standard component group, route, state, and interactive open state required for publishing readiness.", prerequisites: ["SB10 passed.", "All standard component groups closed or explicitly blocked."], deliverables: ["Browser analytics fully populated.", "Screenshots under proof/SB11.", "Visual review answers and defect reopen decisions." ] }, ["Blocks final closure.", "Any defect reopens the owning SB06-SB10 phase."], ["Critical Semantic Adequacy Gate.", "Playwright MCP proof with screenshots and assertions.", "Final visual matrix review."], ["Start dev server.", "Use maximized desktop first, then fixed desktop/tablet/mobile widths.", "Navigate every standard route/scenario.", "Open every dropdown/menu/dialog/popover/tooltip/toast/contextual layer.", "Record pass/fail and reopen defects."], ["Every planned standard component has visual evidence or explicit exception.", "No text overflow/clipping/layering defect remains unresolved.", "Execution report browser analytics rows are populated."], ["Playwright transcripts.", "Screenshot files.", "Screenshot review notes.", "Anti-stub visual proof audit."], ["Routes: all standard sandbox routes and focused routes from SB05.", "Viewports: maximized headed browser, 1366x900, 1024x768, 390x844.", "Actions: all open interactive states listed by component inventory."], "Execute SB11 as a real visual audit. Screenshots must be reviewed against questions, and any real defect reopens the owning implementation subbundle.", true],
    ["SB12", "final-publishing-transfer-readiness-audit", "Final Publishing Transfer Readiness Audit", [raw.RAW01, raw.RAW02, raw.RAW03, raw.RAW09], [repoRef("README.md"), repoRef("CanDoItAll.Components.slnx"), repoRef("codex/bundles/StandardComponents_PublishingReadiness_v1"), "C:\\repositories\\CanDoItAll\\src\\CanDoItAll.AppComponents"], { objective: "Close the bundle with transfer readiness proof, raw-note closure, residual risk review, and explicit follow-up separation for WebGL/Canvas.", prerequisites: ["Checkpoint D passed.", "SB11 visual matrix complete."], deliverables: ["Final red-team report.", "Raw-note closure table.", "Completed-stage validator output.", "Transfer checklist for pure repositories." ] }, ["Final closure only; cannot hide unresolved gaps as prose.", "Any weak proof reopens the owning subbundle."], ["Critical Semantic Adequacy Gate.", "Completed-stage bundle validator.", "Final red-team fake-proof resistance audit."], ["Reopen original raw notes.", "Audit every subbundle proof manifest and browser analytics row.", "Run final build/test/pack and completed validator.", "Document follow-up bundle scope for WebGL/Canvas only."], ["Every raw note is Solved, Partially solved, or Not solved with proof.", "No completed critical subbundle lacks manifest/invariants/transcripts.", "Transfer checklist is ready."], ["Completed validator transcript.", "Final red-team report.", "Raw-note closure proof.", "Build/test/pack summaries."], ["N/A unless final spot screenshots are needed for reopened UI issues.", "If screenshots are captured, cite route, viewport, action, screenshot, and result."], "Execute SB12 only after all prior gates pass. Red-team the proof, close raw notes literally, and leave WebGL/Canvas as explicit separate follow-up scope.", true],
  ].map(([id, slug, title, covered, refs, scope, dependency, validation, steps, acceptance, proof, browser, prompt, critical]) => ({
    id,
    slug,
    readme: subbundleReadme(id, slug, title, covered, refs, scope, dependency, validation, steps, acceptance, proof, browser, prompt, critical),
  }));
}

function main() {
  const data = buildStandardInventory();
  writeJson("inventories/current-state-data.json", data);
  writeText("inventories/01-scope-inventory.md", buildInventoryMarkdown(data));
  writeText("analysis/01-current-state.md", buildCurrentStateMarkdown(data));
  writeText("analysis/02-assumptions-and-risks.md", buildAssumptionsMarkdown());
  writeText("requirements/01-normalized-requirements.md", buildRequirementsMarkdown());
  writeText("architecture/01-target-solution.md", buildArchitectureMarkdown());
  writeText("plan/01-phase-plan.md", buildPhasePlanMarkdown());
  writeText("traceability/01-requirement-traceability.md", buildTraceabilityMarkdown());
  writeText("reviews/01-execution-report.md", buildExecutionReportMarkdown());
  writeText("reviews/00-bundle-self-review.md", buildSelfReviewMarkdown());

  const sourceArtifacts = `# Source Artifacts

- ${repoRef("README.md")}
- ${repoRef("CanDoItAll.Components.slnx")}
- ${repoRef("src/CanDoItAll.Components.Common")}
- ${repoRef("src/CanDoItAll.Components.BaseLib")}
- ${repoRef("src/CanDoItAll.Components.Charts")}
- ${repoRef("src/CanDoItAll.Components.OverlayLib")}
- ${repoRef("src/CanDoItAll.Components.Mermaid")}
- ${repoRef("src/CanDoItAll.Components.Sandbox")}
- ${repoRef("Tailwind")}
- C:\\repositories\\CanDoItAll\\src\\CanDoItAll.AppComponents\\Components
- C:\\repositories\\CanDoItAll\\src\\CanDoItAll.AppComponents\\Primitives
- ${bundleRef("inventories/current-state-data.json")}
- ${bundleRef("inventories/standard-components-publishing-map.xlsx")}
`;
  writeText("inputs/01-source-artifacts.md", sourceArtifacts);

  const structuredInput = `# Structured Input

## Profile

- Bundle profile: \`initiative\`
- Reason: publishing preparation spans architecture, source inventory, Tailwind policy, sandbox grouping, duplicate migration, visual proof, package hardening, and cross-repo AppComponents cleanup.

## Normalized Input Groups

- Scope and transfer preparation: RAW01, RAW03.
- Implementation study and refactoring/hardening: RAW02, RAW08, RAW09.
- Tailwind styling: RAW05.
- Sandbox taxonomy and visual validation: RAW04, RAW10.
- Main app duplicate cleanup: RAW06.
- Mandatory xlsx map: RAW07.
`;
  writeText("inputs/02-structured-input.md", structuredInput);

  const rawRequest = `# Original Request

Use [$candoitall-bundle-workflow](C:\\Users\\dell\\.codex\\skills\\candoitall-bundle-workflow\\SKILL.md) to solve this:

Main goal:
Preparation of repository for publishing.

Architect notes:
- for final publishing we have new pure repositories. But now we must do all polishing before transfer.
- You must do detailed study of the actual implementation. You must identify all parts that need refactoring or hardening. Especially isolation of bases, helpers and things we should isolate and used as shared for easier maintenance.
- focus in this bundle only to parts with standard components and not webgl and canvas. We will do webgl and canvas preparation separatelly.
- our components sandbox might missing some components. Analyze also splitting of components into groups. Do we have them logically similar as best practice in components frameworks? If not we must improve our sandbox.
- we still might have lots of styles in some custom css. Our goal is to use tailwind for styling of components. But maybe some of them still using some "hacks". I also seen that in our tailwind inputs css we use more pure css to define some prepared style rather then using prepared tailwind system for same thing. I am not frontend specialist, so I cannot say if it is correct or not, but I guess, that in tailwind input files we should use more prepared tailwind constructions (for example when we define some align, etc).
- in "C:\\repositories\\CanDoItAll\\src\\CanDoItAll.AppComponents\\Components" we still have some duplicities components I think. Some of them might be old just forgotten there and we already have implementation in Components libs, but some of them might have some improvements and might be used. We will need to reduce all basic type of components in main candoitall repo. AppComponents are more for kind of "complex" components related to web app (some prepared forms that repeats and things like this). All basic kind of components must be in here. This must be generic component base of whole framework. It must be usable also for building other projects.

Mandatory steps:
- it is very complex and it consider lots of components. You must map all in xlsx with correct references and explanations.
- you must identify correct phases of the refactoring and hardening. During planning phase it is important to do first parts that are totally general for all components (like refactor of input tailwinds) isolation of bases and things like this.
- you can make bundle larger. It is complex long run.
- design subbundles with refactoring checkpoints that will force to analyze implementation and do refactoring each few subbundles.
- you must do real validations with playwright mcp and screenshots to analyze real look of the component one by one. It means also in action (in cases of dropdowns and things like that). Sometimes happens that components has trouble with wrapping texts, and then it overflow component, or they behavie weird in layout and do not use available space (they are not stretched over width or height). those things are ususally visible from screenshots. You must not just estimate that something will help in case of styling. Styling requires visual real inspection.
`;
  writeText("inputs/00-original-request.md", rawRequest);

  for (const subbundle of buildSubbundles()) {
    const directory = fs.readdirSync(path.join(bundleRoot, "subbundles"), { withFileTypes: true })
      .filter((entry) => entry.isDirectory())
      .map((entry) => entry.name)
      .find((name) => name.toLowerCase().includes(subbundle.slug.split("-").slice(0, 3).join("-")));
    const finalDirectory = directory ?? `${subbundle.id.slice(2).padStart(2, "0")}-${subbundle.slug}`;
    writeText(path.join("subbundles", finalDirectory, "README.md"), subbundle.readme);
  }

  writeText("shared-prompts/implementation-prompt.md", `# Implementation Prompt

Use this bundle as the durable source of truth. Before editing source, run the selected subbundle entry gate against \`README.md\`, \`plan/01-phase-plan.md\`, the subbundle README, and \`traceability/01-requirement-traceability.md\`.

Implement only the selected subbundle. Respect the standard-component scope: do not refactor WebGL or Canvas implementation. Prefer existing repo patterns, shared bases, and Tailwind component-layer utilities. When styling changes are made, capture real Playwright screenshots at the required viewports and answer the screenshot review questions while proof is fresh.

For critical subbundles, create \`proof/SBxx/manifest.md\` and \`proof/SBxx/semantic-invariants.md\` with changed-file hashes, transcripts, source assertions, anti-stub audit, failing-first or explicit exemption, passing proof, and raw-note literal closure.
`);

  writeText("shared-prompts/qa-prompt.md", `# QA Prompt

Review the selected subbundle from the perspective of publishing readiness. Confirm prerequisites, exact source references, dependency impact, and proof requirements before implementation starts.

For UI work, use Playwright MCP screenshots rather than visual assumptions. Check readability, wrapping, clipping, lateral overflow, overlay layering, available width/height use, disabled/loading/empty states, and interactive open states. Record route, viewport, actions, assertions, screenshots, and pass/fail result in \`reviews/01-execution-report.md\`.

If a later screenshot or test weakens a foundation, reopen the earlier subbundle instead of marking residual risk.
`);

  const rootReadme = `# Standard Components Publishing Readiness v1

Bundle id: \`CanDoItAll.Components.StandardComponents.PublishingReadiness.v1\`  
Created local date: \`2026-06-28\`  
Profile: \`initiative\`  
Repository: \`C:\\repositories\\CanDoItAll.Components\`  
Cross-repo duplicate source: \`C:\\repositories\\CanDoItAll\\src\\CanDoItAll.AppComponents\`

## Mission

Prepare the standard CanDoItAll component libraries for transfer into pure publishing repositories by inventorying actual implementation, isolating shared foundations, planning Tailwind/style hardening, improving sandbox coverage, reducing old AppComponents basic duplicates, and requiring real Playwright visual proof for every standard component group.

## Hard Scope Rules

- In scope: standard components, shared helpers, Tailwind input CSS, Charts, Mermaid, OverlayLib, and the standard sandbox.
- Out of scope for implementation: WebGL and Canvas components. They may appear only as exclusion evidence or sandbox-split targets.
- Do not remove old AppComponents basic components until behavior comparison and migration proof exist.
- Do not close styling or layout work without real browser screenshots and explicit visual review.

## Validation Summary

- Bundle preparation status: \`Prepared\`
- Bundle readiness gate: \`Pending validator run\`
- Execution status: \`Not started\`
- Subbundle gate review: \`Not started\`
- Final closure gate: \`Not started\`
- Browser validation analytics: \`Planned\`

## Primary Artifacts

- Inventory workbook: \`${bundleRef("inventories/standard-components-publishing-map.xlsx")}\`
- Inventory JSON: \`${bundleRef("inventories/current-state-data.json")}\`
- Phase plan: \`${bundleRef("plan/01-phase-plan.md")}\`
- Traceability: \`${bundleRef("traceability/01-requirement-traceability.md")}\`

## Handoff Notes

Execution must start at SB01 entry validation and then proceed in dependency order. Checkpoints A-D are mandatory review points and are designed to force refactoring review after foundations, migration/harness setup, component group hardening, and publishing proof.
`;
  writeText("README.md", rootReadme);
}

main();
