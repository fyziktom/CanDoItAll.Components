// Scans one or more src/*/Components directories (first-level group folders,
// first-level .razor files within each group; loose top-level .razor files fall
// under a "(root)" group) and cross-references every component name against
// .razor files in sibling repos, producing a Markdown usage matrix plus a
// machine-readable "unused components" audit (JSON) and a generated C# set
// consumed by the Sandbox app to filter its catalog.
// Does not touch git. Writes only the configured output paths.
// Usage: node components-usage.cjs [--config <path>]

"use strict";

const fs = require("node:fs");
const path = require("node:path");

const REQUIRED_KEYS = [
  "componentsPaths",
  "outputPath",
  "siblings",
  "excludeDirs",
  "unusedRequiredSiblings",
  "unusedJsonOutputPath",
  "unusedCsOutputPath"
];

main().catch(error => {
  console.error(error.stack || error.message || String(error));
  process.exitCode = 1;
});

async function main() {
  const args = parseArgs(process.argv.slice(2));
  const configPath = path.resolve(args.config || path.join(__dirname, "components-usage.cfg.json"));
  const config = loadConfig(configPath);

  const groups = config.componentsPaths.flatMap(componentsPath => discoverComponents(componentsPath));
  const componentCount = groups.reduce((sum, group) => sum + group.components.length, 0);
  console.log(
    `Discovered ${componentCount} components across ${groups.length} groups in ${config.componentsPaths.length} components paths`
  );

  // TODO: "Self" usage is only computed within BaseLib today (see baseLibFiles below), so
  // components in CanvasLib, Charts, Gantt, Mermaid, OverlayLib, QRCode, and WebGlLib never get
  // Self credit for being composed by a sibling component in their own library, even when they
  // genuinely are. This under-counts usage for those libraries and can make internally-composed
  // components look unused (including in unusedRequiredSiblings below) when they are not. Revisit
  // by scoping Self per-library (or globally) if that turns out to matter in practice.
  const baseLibFiles = groups
    .filter(group => group.library === "BaseLib")
    .flatMap(group => group.components)
    .map(component => ({
      filePath: component.filePath,
      content: fs.readFileSync(component.filePath, "utf8")
    }));

  const siblingResults = config.siblings.map(sibling => {
    const razorFiles = findRazorFiles(sibling.path, config.excludeDirs);
    console.log(`Scanning ${sibling.name}: ${razorFiles.length} .razor files in ${sibling.path}`);
    const content = razorFiles.map(file => fs.readFileSync(file, "utf8")).join("\n");
    return { name: sibling.name, content };
  });

  const rows = buildComponentRows(groups, siblingResults, baseLibFiles);

  const markdown = renderReport(rows, siblingResults.map(sibling => sibling.name), config.usageExceptions);
  fs.writeFileSync(config.outputPath, markdown);
  console.log(`Report written to ${config.outputPath}`);

  const unused = computeUnused(rows, config.unusedRequiredSiblings, config.usageExceptions);
  fs.writeFileSync(config.unusedJsonOutputPath, renderUnusedJson(unused.rows));
  console.log(`Unused component audit written to ${config.unusedJsonOutputPath} (${unused.rows.length} entries)`);

  fs.writeFileSync(config.unusedCsOutputPath, renderUnusedCs(unused.names));
  console.log(`Unused component set written to ${config.unusedCsOutputPath} (${unused.names.length} names)`);
}

function parseArgs(values) {
  const parsed = {};
  for (let index = 0; index < values.length; index += 1) {
    const value = values[index];
    if (!value.startsWith("--")) {
      continue;
    }

    const key = value.slice(2);
    const next = values[index + 1];
    if (!next || next.startsWith("--")) {
      parsed[key] = "true";
    } else {
      parsed[key] = next;
      index += 1;
    }
  }

  return parsed;
}

function loadConfig(configPath) {
  if (!fs.existsSync(configPath)) {
    throw new Error(`Config file not found: ${configPath}`);
  }

  const configDir = path.dirname(configPath);
  const config = JSON.parse(fs.readFileSync(configPath, "utf8"));

  for (const key of REQUIRED_KEYS) {
    if (!(key in config)) {
      throw new Error(`Missing required config key "${key}" in ${configPath}`);
    }
  }

  const componentsPaths = config.componentsPaths.map(componentsPath => {
    const resolvedPath = path.resolve(configDir, componentsPath);
    if (!fs.existsSync(resolvedPath)) {
      throw new Error(
        `componentsPaths entry does not exist: ${resolvedPath}\n` +
          `Check "componentsPaths" in ${configPath}.`
      );
    }
    return resolvedPath;
  });

  const siblings = config.siblings.map(sibling => {
    const resolvedPath = path.resolve(configDir, sibling.path);
    if (!fs.existsSync(resolvedPath)) {
      throw new Error(
        `Sibling "${sibling.name}" path does not exist: ${resolvedPath}\n` +
          `Check "siblings" in ${configPath}.`
      );
    }
    return { name: sibling.name, path: resolvedPath };
  });

  const siblingNames = new Set(siblings.map(sibling => sibling.name));
  for (const name of config.unusedRequiredSiblings) {
    if (!siblingNames.has(name)) {
      throw new Error(
        `unusedRequiredSiblings entry "${name}" is not a configured sibling in ${configPath}.`
      );
    }
  }

  return {
    componentsPaths,
    outputPath: path.resolve(configDir, config.outputPath),
    siblings,
    excludeDirs: new Set(config.excludeDirs),
    unusedRequiredSiblings: config.unusedRequiredSiblings,
    usageExceptions: new Set(config.usageExceptions ?? []),
    unusedJsonOutputPath: path.resolve(configDir, config.unusedJsonOutputPath),
    unusedCsOutputPath: path.resolve(configDir, config.unusedCsOutputPath)
  };
}

function libraryNameFor(componentsPath) {
  const projectName = path.basename(path.dirname(componentsPath));
  const prefix = "CanDoItAll.Components.";
  return projectName.startsWith(prefix) ? projectName.slice(prefix.length) : projectName;
}

function discoverComponents(componentsPath) {
  const library = libraryNameFor(componentsPath);
  const entries = fs.readdirSync(componentsPath, { withFileTypes: true });

  const groupNames = entries
    .filter(entry => entry.isDirectory())
    .map(entry => entry.name)
    .sort((a, b) => a.localeCompare(b));

  const groups = groupNames.map(groupName => {
    const groupPath = path.join(componentsPath, groupName);
    const components = fs
      .readdirSync(groupPath, { withFileTypes: true })
      .filter(entry => entry.isFile() && entry.name.endsWith(".razor"))
      .map(entry => ({
        name: entry.name.slice(0, -".razor".length),
        filePath: path.join(groupPath, entry.name)
      }))
      .sort((a, b) => a.name.localeCompare(b.name));

    return { library, group: groupName, components };
  });

  const rootComponents = entries
    .filter(entry => entry.isFile() && entry.name.endsWith(".razor"))
    .map(entry => ({
      name: entry.name.slice(0, -".razor".length),
      filePath: path.join(componentsPath, entry.name)
    }))
    .sort((a, b) => a.name.localeCompare(b.name));

  if (rootComponents.length > 0) {
    groups.push({ library, group: "(root)", components: rootComponents });
  }

  return groups;
}

function findRazorFiles(rootPath, excludeDirs) {
  const results = [];

  function walk(dir) {
    const entries = fs.readdirSync(dir, { withFileTypes: true });
    for (const entry of entries) {
      if (entry.isDirectory()) {
        if (excludeDirs.has(entry.name)) {
          continue;
        }
        walk(path.join(dir, entry.name));
      } else if (entry.isFile() && entry.name.endsWith(".razor")) {
        results.push(path.join(dir, entry.name));
      }
    }
  }

  walk(rootPath);
  return results;
}

function escapeRegex(value) {
  return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

function buildUsageRegex(componentName) {
  return new RegExp(`<(?:\\w+\\.)?${escapeRegex(componentName)}(?=[\\s/>])`, "g");
}

function countUsages(content, componentName) {
  return content.match(buildUsageRegex(componentName))?.length ?? 0;
}

// One row per discovered component, in stable Library/Group/Component order, with usage counts
// precomputed so both the Markdown report and the unused-components audit read from the same data.
function buildComponentRows(groups, siblingResults, baseLibFiles) {
  const sortedGroups = [...groups].sort(
    (a, b) => a.library.localeCompare(b.library) || a.group.localeCompare(b.group)
  );

  const rows = [];
  for (const group of sortedGroups) {
    for (const component of group.components) {
      const selfContent = baseLibFiles
        .filter(file => file.filePath !== component.filePath)
        .map(file => file.content)
        .join("\n");
      const selfUsageCount = countUsages(selfContent, component.name);

      const siblingUsage = {};
      for (const sibling of siblingResults) {
        siblingUsage[sibling.name] = countUsages(sibling.content, component.name);
      }

      rows.push({
        library: group.library,
        group: group.group,
        name: component.name,
        filePath: component.filePath,
        selfUsageCount,
        siblingUsage
      });
    }
  }

  return rows;
}

function renderReport(rows, siblingNames, usageExceptions) {
  const header = ["Library", "Group", "Component", "Self", ...siblingNames];
  const separator = header.map(() => "---");

  const usedCounts = siblingNames.map(() => 0);
  let selfCount = 0;

  const tableRows = rows.map(row => {
    if (row.selfUsageCount > 0) {
      selfCount += 1;
    }

    const cells = siblingNames.map((name, index) => {
      const usageCount = row.siblingUsage[name] ?? 0;
      if (usageCount > 0) {
        usedCounts[index] += 1;
      }
      return usageCount > 0 ? `✅\u202F${usageCount}` : " ";
    });

    const selfCell = usageExceptions.has(row.name)
      ? "❤️"
      : row.selfUsageCount > 0
        ? `⭐\u202F${row.selfUsageCount}`
        : " ";

    return [
      row.library,
      row.group,
      row.name,
      selfCell,
      ...cells
    ];
  });

  const totalRow = [
    "**Total**",
    "",
    `${rows.length}`,
    `${selfCount}`,
    ...usedCounts.map(count => `${count}`)
  ];

  const lines = [
    "# Components Usage",
    "",
    // `Generated: ${generatedAt}`,
    // "",
    `| ${header.join(" | ")} |`,
    `| ${separator.join(" | ")} |`,
    `| ${totalRow.join(" | ")} |`,
    ...tableRows.map(row => `| ${row.join(" | ")} |`)
  ];

  return `${lines.join("\n")}\n`;
}

// A row is "unused" when it shows zero usage in Self and in every sibling named in
// unusedRequiredSiblings (currently CanDoItAll and Sandbox) at the same time. Siblings not listed
// there (e.g. CodeAnalysis) don't factor into this determination, though they still show up in
// USAGE.md as before. Names listed in usageExceptions are never considered unused, regardless of
// their actual usage counts.
function isRowUnused(row, requiredSiblingNames, usageExceptions) {
  if (usageExceptions.has(row.name)) {
    return false;
  }
  if (row.selfUsageCount > 0) {
    return false;
  }
  return requiredSiblingNames.every(name => (row.siblingUsage[name] ?? 0) === 0);
}

function computeUnused(rows, requiredSiblingNames, usageExceptions) {
  const unusedRows = rows.filter(row => isRowUnused(row, requiredSiblingNames, usageExceptions));

  const rowsByName = new Map();
  for (const row of rows) {
    const sameName = rowsByName.get(row.name) ?? [];
    sameName.push(row);
    rowsByName.set(row.name, sameName);
  }

  // A name only enters the deduped set if every discovered component sharing that name (across
  // all libraries/groups) is unused, so two differently-scoped components that happen to share a
  // name can't cause one to wrongly hide the other.
  const names = [...rowsByName.entries()]
    .filter(([, sameNameRows]) => sameNameRows.every(row => isRowUnused(row, requiredSiblingNames, usageExceptions)))
    .map(([name]) => name)
    .sort((a, b) => a.localeCompare(b));

  const auditRows = unusedRows
    .map(row => ({ library: row.library, group: row.group, component: row.name }))
    .sort(
      (a, b) =>
        a.library.localeCompare(b.library) ||
        a.group.localeCompare(b.group) ||
        a.component.localeCompare(b.component)
    );

  return { rows: auditRows, names };
}

function renderUnusedJson(rows) {
  return `${JSON.stringify(rows, null, 2)}\n`;
}

function renderUnusedCs(names) {
  const entries = names.map(name => `        "${name}",`).join("\n");
  return `// Auto-generated by tools/components-usage/components-usage.cjs. Do not edit directly.
namespace CanDoItAll.Components.Sandbox;

public static class SandboxUnusedComponents
{
    public static readonly IReadOnlySet<string> Names = new HashSet<string>(StringComparer.Ordinal)
    {
${entries}
    };
}
`;
}
