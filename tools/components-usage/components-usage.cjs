// Scans one or more src/*/Components directories (first-level group folders,
// first-level .razor files within each group; loose top-level .razor files fall
// under a "(root)" group) and cross-references every component name against
// .razor files in sibling repos, producing a Markdown usage matrix.
// Does not touch git. Writes only the configured outputPath.
// Usage: node components-usage.cjs [--config <path>]

"use strict";

const fs = require("node:fs");
const path = require("node:path");

const REQUIRED_KEYS = ["componentsPaths", "outputPath", "siblings", "excludeDirs"];

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

  const markdown = renderReport(groups, siblingResults, baseLibFiles);
  fs.writeFileSync(config.outputPath, markdown);
  console.log(`Report written to ${config.outputPath}`);
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

  return {
    componentsPaths,
    outputPath: path.resolve(configDir, config.outputPath),
    siblings,
    excludeDirs: new Set(config.excludeDirs)
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
  return new RegExp(`<(?:\\w+\\.)?${escapeRegex(componentName)}(?=[\\s/>])`);
}

function renderReport(groups, siblingResults, baseLibFiles) {
  const generatedAt = new Date().toISOString();
  const header = ["Library", "Group", "Component", "Self", ...siblingResults.map(sibling => sibling.name)];
  const separator = header.map(() => "---");

  const sortedGroups = [...groups].sort(
    (a, b) => a.library.localeCompare(b.library) || a.group.localeCompare(b.group)
  );

  const rows = [];
  const usedCounts = siblingResults.map(() => 0);
  let componentCount = 0;
  let selfCount = 0;

  for (const group of sortedGroups) {
    for (const component of group.components) {
      componentCount += 1;
      const regex = buildUsageRegex(component.name);

      const selfContent = baseLibFiles
        .filter(file => file.filePath !== component.filePath)
        .map(file => file.content)
        .join("\n");
      const selfUsed = regex.test(selfContent);
      if (selfUsed) {
        selfCount += 1;
      }

      const cells = siblingResults.map((sibling, index) => {
        const used = regex.test(sibling.content);
        if (used) {
          usedCounts[index] += 1;
        }
        return used ? "✅" : "—";
      });
      rows.push([group.library, group.group, component.name, selfUsed ? "⭐" : "—", ...cells]);
    }
  }

  const totalRow = [
    "**Total**",
    "",
    `${componentCount}`,
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
    ...rows.map(row => `| ${row.join(" | ")} |`)
  ];

  return `${lines.join("\n")}\n`;
}
