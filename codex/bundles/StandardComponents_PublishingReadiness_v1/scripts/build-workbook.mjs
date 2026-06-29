import fs from "node:fs/promises";
import path from "node:path";
import { SpreadsheetFile, Workbook } from "@oai/artifact-tool";

const repoRoot = path.resolve(process.cwd(), "..", "..", "..");
const bundleRoot = path.resolve(process.cwd());
const dataPath = path.join(bundleRoot, "inventories", "current-state-data.json");
const outputPath = path.join(bundleRoot, "inventories", "standard-components-publishing-map.xlsx");
const previewDir = path.join(bundleRoot, "reviews", "workbook-previews");

const data = JSON.parse(await fs.readFile(dataPath, "utf8"));

function colName(index) {
  let n = index + 1;
  let result = "";
  while (n > 0) {
    const rem = (n - 1) % 26;
    result = String.fromCharCode(65 + rem) + result;
    n = Math.floor((n - 1) / 26);
  }
  return result;
}

function tableRange(rowCount, colCount) {
  return `A1:${colName(colCount - 1)}${rowCount}`;
}

function writeTable(sheet, headers, rows, tableName) {
  const values = [headers, ...rows];
  sheet.getRange(tableRange(values.length, headers.length)).values = values;
  sheet.getRange(`A1:${colName(headers.length - 1)}1`).format = {
    fill: "#164E63",
    font: { bold: true, color: "#FFFFFF" },
    wrapText: true,
  };
  sheet.getRange(tableRange(values.length, headers.length)).format = {
    font: { name: "Aptos", size: 10 },
    wrapText: true,
    borders: { preset: "outside", style: "thin", color: "#CBD5E1" },
  };
  const table = sheet.tables.add(tableRange(values.length, headers.length), true, tableName);
  table.style = "TableStyleMedium2";
  sheet.freezePanes.freezeRows(1);
  sheet.showGridLines = false;
  sheet.getRange(tableRange(values.length, headers.length)).format.autofitColumns();
  sheet.getRange(tableRange(values.length, headers.length)).format.autofitRows();
  return table;
}

function setWidths(sheet, widths) {
  widths.forEach((width, index) => {
    sheet.getRange(`${colName(index)}:${colName(index)}`).format.columnWidth = width;
  });
}

function boolText(value) {
  return value ? "Yes" : "No";
}

function join(value) {
  return Array.isArray(value) ? value.join("; ") : (value ?? "");
}

const workbook = Workbook.create();

const summary = workbook.worksheets.add("Summary");
summary.showGridLines = false;
summary.getRange("A1:H1").merge();
summary.getRange("A1").values = [["Standard Components Publishing Readiness Map"]];
summary.getRange("A1").format = {
  fill: "#0F172A",
  font: { bold: true, color: "#FFFFFF", size: 16 },
};
summary.getRange("1:1").format.rowHeight = 28;
summary.getRange("A2:H2").merge();
summary.getRange("A2").values = [[`Generated ${data.generatedAt}. Scope excludes WebGL and Canvas implementation work.`]];
summary.getRange("A2").format = { font: { color: "#334155", size: 11 }, wrapText: true };
summary.getRange("2:2").format.rowHeight = 24;

const summaryRows = [
  ["Standard inventory rows", data.components.length],
  ["Standard non-sandbox Razor components", data.components.filter((c) => c.kind === "Razor component" && c.project !== "CanDoItAll.Components.Sandbox").length],
  ["Old AppComponents rows", data.appComponents.length],
  ["Old rows with standard-name match", data.appComponents.filter((c) => c.hasStandardMatch).length],
  ["Standard Razor components with no direct sandbox example", data.components.filter((c) => c.kind === "Razor component" && c.project !== "CanDoItAll.Components.Sandbox" && c.sandboxExamples.length === 0).length],
  ["Tailwind CSS files", data.tailwindFiles.length],
  ["High Tailwind refactor-pressure files", data.tailwindFiles.filter((f) => f.severity === "High").length],
  ["Non-WebGL/non-Canvas test projects found", 0],
];
summary.getRange("A4:B11").values = summaryRows;
summary.getRange("A4:A11").format = { fill: "#E0F2FE", font: { bold: true, color: "#0F172A" } };
summary.getRange("B4:B11").format = { fill: "#F8FAFC", font: { bold: true, color: "#0F172A" }, numberFormat: "#,##0" };
summary.getRange("A4:B11").format.borders = { preset: "all", style: "thin", color: "#CBD5E1" };

summary.getRange("D4:H4").values = [["Finding", "Severity", "Owner", "Reason", "Evidence"]];
summary.getRange("D4:H4").format = { fill: "#164E63", font: { bold: true, color: "#FFFFFF" }, wrapText: true };
summary.getRange(`D5:H${4 + data.findings.length}`).values = data.findings.map((finding) => [
  finding.title,
  finding.severity,
  finding.owner,
  finding.id,
  finding.evidence,
]);
summary.getRange(`D4:H${4 + data.findings.length}`).format = {
  font: { name: "Aptos", size: 10 },
  wrapText: true,
  borders: { preset: "all", style: "thin", color: "#CBD5E1" },
};
setWidths(summary, [58, 14, 4, 42, 12, 12, 16, 86]);

const components = workbook.worksheets.add("Components");
writeTable(
  components,
  [
    "Name",
    "Project",
    "Group",
    "Kind",
    "Category",
    "Repo Reference",
    "Lines",
    "Params",
    "Styled Base",
    "Additional Attributes",
    "CSS Isolation",
    "Sandbox Examples",
    "App Duplicates",
    "Owner",
    "Recommendation",
    "Old Behavior Signals",
  ],
  data.components.map((component) => [
    component.name,
    component.project,
    component.group,
    component.kind,
    component.category,
    component.repoRef,
    component.lineCount,
    component.parameterCount,
    boolText(component.inheritsStyledBase),
    boolText(component.usesAdditionalAttributes),
    boolText(component.hasCssIsolation),
    component.sandboxExamples.length,
    component.appComponentsDuplicateCount,
    component.owner,
    component.recommendation,
    join(component.oldBehaviorSignals),
  ]),
  "ComponentsTable",
);
setWidths(components, [22, 34, 18, 18, 34, 74, 10, 10, 13, 18, 13, 16, 15, 10, 88, 34]);

const duplicates = workbook.worksheets.add("App Duplicates");
writeTable(
  duplicates,
  [
    "Name",
    "Old AppComponents Path",
    "Has Standard Match",
    "Standard Matches",
    "Old Signals",
    "Classification",
    "Owner",
    "Recommendation",
  ],
  data.appComponents.map((component) => [
    component.name,
    component.localRef,
    boolText(component.hasStandardMatch),
    join(component.standardMatches),
    join(component.oldBehaviorSignals),
    component.classification,
    component.owner,
    component.recommendation,
  ]),
  "AppDuplicatesTable",
);
setWidths(duplicates, [24, 86, 18, 90, 34, 32, 10, 90]);

const tailwind = workbook.worksheets.add("Tailwind CSS");
writeTable(
  tailwind,
  [
    "File",
    "Lines",
    "Selectors",
    "@apply",
    "Raw CSS Decls",
    "Media",
    "CSS Vars",
    "Arbitrary Values",
    "Color Mix",
    "Important",
    "Severity",
    "Owner",
    "Recommendation",
  ],
  data.tailwindFiles.map((file) => [
    file.repoRef,
    file.lineCount,
    file.selectors,
    file.applyCount,
    file.rawCssDeclarations,
    file.mediaCount,
    file.cssVarCount,
    file.arbitraryValueCount,
    file.colorMixCount,
    file.importantCount,
    file.severity,
    file.owner,
    file.recommendation,
  ]),
  "TailwindCssTable",
);
setWidths(tailwind, [70, 10, 10, 10, 14, 10, 10, 16, 12, 12, 12, 10, 90]);

const sandbox = workbook.worksheets.add("Sandbox Coverage");
const sandboxRows = [];
for (const group of data.sandboxGroups) {
  const examples = data.sandboxExamples.filter((example) => example.group === group.key);
  sandboxRows.push([
    group.key,
    group.title,
    group.route,
    group.summary,
    examples.length,
    examples.map((example) => `${example.id} (${example.scenario})`).join("; "),
    [...new Set(examples.flatMap((example) => example.componentNames))].sort().join("; "),
  ]);
}
writeTable(
  sandbox,
  ["Group Key", "Title", "Route", "Summary", "Example Count", "Examples", "Components Referenced"],
  sandboxRows,
  "SandboxCoverageTable",
);
setWidths(sandbox, [18, 20, 26, 72, 14, 86, 100]);

const phases = workbook.worksheets.add("Phases");
const phaseRows = [
  ["SB01", "Current state inventory and publishing scope freeze", "Critical foundation", "R01,R02", "All later subbundles", "Inventory workbook, prepared validator"],
  ["SB02", "Tailwind component styling foundation hardening", "Critical foundation", "R03,R08", "SB06-SB11", "Tailwind build, screenshots"],
  ["SB03", "Shared bases helpers and primitives isolation", "Critical foundation", "R04,R08", "SB04-SB11", "Helper tests, source assertions"],
  ["SB04", "AppComponents duplicate migration audit", "Critical foundation", "R05", "SB10,SB12", "Cross-repo build, migration matrix"],
  ["SB05", "Sandbox taxonomy and standard coverage expansion", "Critical foundation", "R06,R10", "SB06-SB11", "Route coverage and Playwright smoke"],
  ["SB06", "Forms and inputs behavior visual hardening", "Component group", "R07,R10", "SB10-SB12", "Input screenshots and tests"],
  ["SB07", "Actions badges and feedback visual hardening", "Component group", "R08,R10", "SB10-SB12", "Open tooltip/toast/popover proof"],
  ["SB08", "Layout navigation and overlay hardening", "Component group", "R09,R10", "SB10-SB12", "Open overlay/menu/dialog proof"],
  ["SB09", "Data display charts and diagram hardening", "Component group", "R10", "SB10-SB12", "Nonblank chart/diagram proof"],
  ["SB10", "Compatibility cleanup packaging and public API hardening", "Critical closure foundation", "R11", "SB11-SB12", "Build/test/pack/API proof"],
  ["SB11", "Full Playwright visual validation matrix", "Critical closure foundation", "R12", "SB12", "Full screenshot matrix"],
  ["SB12", "Final publishing transfer readiness audit", "Critical closure foundation", "R13", "Final closure", "Completed validator and red-team report"],
];
writeTable(
  phases,
  ["Subbundle", "Title", "Type", "Requirements", "Unlocks", "Proof Gate"],
  phaseRows,
  "PhasesTable",
);
setWidths(phases, [12, 56, 28, 20, 34, 50]);

for (const sheet of [summary, components, duplicates, tailwind, sandbox, phases]) {
  sheet.getRange("A:Z").format.verticalAlignment = "top";
}

await fs.mkdir(previewDir, { recursive: true });
const previewSpecs = [
  ["Summary", "A1:H16"],
  ["Components", "A1:P35"],
  ["App Duplicates", "A1:H35"],
  ["Tailwind CSS", "A1:M25"],
  ["Sandbox Coverage", "A1:G20"],
  ["Phases", "A1:F14"],
];

for (const [sheetName, range] of previewSpecs) {
  const preview = await workbook.render({ sheetName, range, scale: 1, format: "png" });
  await fs.writeFile(
    path.join(previewDir, `${sheetName.toLowerCase().replaceAll(" ", "-")}.png`),
    new Uint8Array(await preview.arrayBuffer()),
  );
}

const inspectSummary = await workbook.inspect({
  kind: "table",
  range: "Summary!A1:H16",
  include: "values,formulas",
  tableMaxRows: 20,
  tableMaxCols: 10,
});
await fs.writeFile(path.join(previewDir, "inspect-summary.ndjson"), inspectSummary.ndjson, "utf8");

const errors = await workbook.inspect({
  kind: "match",
  searchTerm: "#REF!|#DIV/0!|#VALUE!|#NAME\\?|#N/A",
  options: { useRegex: true, maxResults: 300 },
  summary: "final formula error scan",
});
await fs.writeFile(path.join(previewDir, "formula-error-scan.ndjson"), errors.ndjson, "utf8");

const output = await SpreadsheetFile.exportXlsx(workbook);
await output.save(outputPath);
console.log(outputPath);
process.exit(0);
